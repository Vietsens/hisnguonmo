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
using DevExpress.Utils.Drawing;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Nodes;
using DevExpress.XtraTreeList.Nodes.Operations;
using DevExpress.XtraTreeList.ViewInfo;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Plugins.ExportMediMatePriceList.ADO;
using HIS.Desktop.Utilities.Extensions;
using HIS.Desktop.Utility;
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
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.ExportMediMatePriceList
{
    public partial class frmExportPriceList : FormBase
    {
        private List<MediMateADO> ListData = new List<MediMateADO>();
        V_HIS_MEDI_STOCK mediStock = null;
        List<HisMedicineTypeView1SDO> medicineResults = new List<HisMedicineTypeView1SDO>();
        List<HisMaterialTypeView1SDO> materialResults = new List<HisMaterialTypeView1SDO>();
        List<HIS_SUPPLIER> ListSupplier = new List<HIS_SUPPLIER>();
        List<HIS_MANUFACTURER> ListManufacturer = new List<HIS_MANUFACTURER>();
        List<V_HIS_MEDI_STOCK> MediStockSelected;
        List<V_HIS_MEDI_STOCK> TotalMediStock;
        short MediMatiType = 0;
        short VatTu = 2;
        short Thuoc = 1;
        List<HIS_SALE_PROFIT_CFG> saleProfits = null;
        private List<MediMateADO> ListDataSelected = new List<MediMateADO>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="module"></param>
        /// <param name="typeMediMate">1 thuoc. 2 vat tu</param>
        public frmExportPriceList(Inventec.Desktop.Common.Modules.Module module, short _MediMatiType)
            : base(module)
        {
            InitializeComponent();
            this.MediMatiType = _MediMatiType;
        }

        private void frmExportPriceList_Load(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                if (MediMatiType != 1 && MediMatiType != 2)
                {
                    Inventec.Common.Logging.LogSystem.Debug("MediMatiType: " + MediMatiType);
                }
                this.mediStock = BackendDataWorker.Get<V_HIS_MEDI_STOCK>().FirstOrDefault(o => o.ROOM_ID == this.currentModuleBase.RoomId);
                ProcessGetTotalMediStock();
                InitMediStockCheck();
                InitComboMediStock();
                SetValueCheck(cboMediStock, new List<V_HIS_MEDI_STOCK> { this.mediStock }, TotalMediStock);
                this.LoadDataTotalGrid();
                btnFind_Click(null, null);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessGetTotalMediStock()
        {
            try
            {
                var datas = BackendDataWorker.Get<V_HIS_USER_ROOM>().Where(p => p.LOGINNAME.Trim() == Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName().Trim() && p.BRANCH_ID == HIS.Desktop.LocalStorage.LocalData.BranchWorker.GetCurrentBranchId()).ToList();
                if (datas != null && datas.Count > 0)
                {
                    this.TotalMediStock = BackendDataWorker.Get<V_HIS_MEDI_STOCK>().Where(p => datas.Exists(e => e.ROOM_ID == p.ROOM_ID)).ToList();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetValueCheck(GridLookUpEdit gridLookUpEdit, List<V_HIS_MEDI_STOCK> listSelect, List<V_HIS_MEDI_STOCK> listAll)
        {
            try
            {
                if (listSelect != null)
                {
                    var selectFilter = listAll.Where(o => listSelect.Exists(p => o.ID == p.ID)).ToList();
                    if (selectFilter != null && selectFilter.Count > 0)
                    {
                        if (this.MediStockSelected == null)
                        {
                            this.MediStockSelected = new List<V_HIS_MEDI_STOCK>();
                        }

                        this.MediStockSelected.AddRange(selectFilter);
                    }

                    listAll = listAll.OrderByDescending(o => selectFilter.Exists(p => o.ID == p.ID)).ThenBy(o => o.MEDI_STOCK_NAME).ToList();
                    gridLookUpEdit.Properties.DataSource = listAll;
                    GridCheckMarksSelection gridCheckMark = gridLookUpEdit.Properties.Tag as GridCheckMarksSelection;
                    gridCheckMark.Selection.Clear();
                    gridCheckMark.Selection.AddRange(selectFilter);
                }
                gridLookUpEdit.Text = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitMediStockCheck()
        {
            try
            {
                GridCheckMarksSelection gridCheck = new GridCheckMarksSelection(cboMediStock.Properties);
                gridCheck.SelectionChanged += new GridCheckMarksSelection.SelectionChangedEventHandler(SelectionGrid__MediStock);
                cboMediStock.Properties.Tag = gridCheck;
                cboMediStock.Properties.View.OptionsSelection.MultiSelect = true;
                GridCheckMarksSelection gridCheckMark = cboMediStock.Properties.Tag as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    gridCheckMark.ClearSelection(cboMediStock.Properties.View);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SelectionGrid__MediStock(object sender, EventArgs e)
        {
            try
            {
                MediStockSelected = new List<V_HIS_MEDI_STOCK>();
                foreach (MOS.EFMODEL.DataModels.V_HIS_MEDI_STOCK rv in (sender as GridCheckMarksSelection).Selection)
                {
                    if (rv != null)
                        MediStockSelected.Add(rv);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboMediStock()
        {
            try
            {
                cboMediStock.Properties.DataSource = this.TotalMediStock;
                cboMediStock.Properties.DisplayMember = "MEDI_STOCK_NAME";
                cboMediStock.Properties.ValueMember = "ID";
                DevExpress.XtraGrid.Columns.GridColumn col2 = cboMediStock.Properties.View.Columns.AddField("MEDI_STOCK_NAME");
                col2.VisibleIndex = 1;
                col2.Width = 200;
                col2.Caption = "";
                cboMediStock.Properties.PopupFormWidth = 200;
                cboMediStock.Properties.View.OptionsView.ShowColumnHeaders = false;
                cboMediStock.Properties.View.OptionsSelection.MultiSelect = true;
                GridCheckMarksSelection gridCheckMark = cboMediStock.Properties.Tag as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    gridCheckMark.ClearSelection(cboMediStock.Properties.View);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataTotalGrid()
        {
            try
            {
                if (MediMatiType == Thuoc)
                {
                    var listRoot = BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>();
                    if (listRoot != null && listRoot.Count > 0)
                    {
                        ListData = (from r in listRoot select new MediMateADO(r)).ToList();
                    }
                }
                else if (MediMatiType == VatTu)
                {
                    var listRoot = BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>();
                    if (listRoot != null && listRoot.Count > 0)
                    {
                        ListData = (from r in listRoot select new MediMateADO(r)).ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetDatasource(List<MediMateADO> datas)
        {
            try
            {
                var MedicineTypeADOs = new List<MediMateADO>();
                if (datas != null && datas.Count > 0)
                {
                    MedicineTypeADOs = datas.OrderBy(o => o.NUM_ORDER).ThenBy(o => o.MEDI_MATE_TYPE_NAME).ToList();
                }

                var records = new BindingList<MediMateADO>(MedicineTypeADOs);
                trvMediMate.DataSource = records;
                trvMediMate.ExpandAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            try
            {
                if (!btnExport.Enabled || ListData == null || ListData.Count <= 0) return;
                List<MediMateADO> selecteds = GetListCheck();
                if (selecteds == null || selecteds.Count <= 0)
                {
                    XtraMessageBox.Show("Chưa chọn thuốc cần xuất", "Thông báo", DevExpress.Utils.DefaultBoolean.True);
                    return;
                }
                WaitingManager.Show();
                // get supplier

                this.ListSupplier = BackendDataWorker.Get<HIS_SUPPLIER>();
                this.ListManufacturer = BackendDataWorker.Get<HIS_MANUFACTURER>();

                MOS.Filter.HisSaleProfitCfgFilter saleFilter = new HisSaleProfitCfgFilter();
                saleFilter.IS_ACTIVE = 1;
                saleProfits = new BackendAdapter(new CommonParam()).Get<List<HIS_SALE_PROFIT_CFG>>("api/HisSaleProfitCfg/Get", ApiConsumer.ApiConsumers.MosConsumer, saleFilter, null);
                Inventec.Common.Logging.LogSystem.Debug("saleProfits count:" + (saleProfits != null ? saleProfits.Count() : 0));

                bool isBusiness = (this.MediStockSelected.Count == 1 && this.MediStockSelected.Any(a => a.IS_BUSINESS == 1));

                if (this.MediMatiType == Thuoc)
                {
                    HisMedicineTypeView1SDOFilter filter = new HisMedicineTypeView1SDOFilter();
                    filter.IS_BUSINESS = isBusiness;
                    filter.ColumnParams = new List<string>()
                    {
                        "ID",
                        "CONCENTRA",
                        "MEDICINE_TYPE_CODE",
                        "MEDICINE_TYPE_NAME",
                        "MEDICINE_INFO",
                        "PARENT_ID",
                        "SERVICE_UNIT_CODE",
                        "SERVICE_UNIT_NAME",
                        "IS_LEAF",
                        "IS_SALE_EQUAL_IMP_PRICE",
                        "MANUFACTURER_ID",
                        "NATIONAL_NAME",
                        "IS_BUSINESS"
                    };

                    this.medicineResults = new BackendAdapter(new CommonParam()).Get<List<HisMedicineTypeView1SDO>>("api/HisMedicineType/GetPriceLists", ApiConsumers.MosConsumer, filter, null);

                    Inventec.Common.Logging.LogSystem.Debug("HisMedicineType/GetPriceLists medicineResults count: " + (medicineResults != null ? medicineResults.Count() : 0));
                }
                else
                {
                    HisMaterialTypeView1SDOFilter filter = new HisMaterialTypeView1SDOFilter();
                    filter.IS_BUSINESS = isBusiness;
                    filter.ColumnParams = new List<string>()
                    {
                        "ID",
                        "CONCENTRA",
                        "MATERIAL_TYPE_CODE",
                        "MATERIAL_TYPE_NAME",
                        "MATERIAL_INFO",
                        "PARENT_ID",
                        "SERVICE_UNIT_CODE",
                        "SERVICE_UNIT_NAME",
                        "IS_LEAF",
                        "IS_SALE_EQUAL_IMP_PRICE",
                        "MANUFACTURER_ID",
                        "NATIONAL_NAME",
                        "IS_BUSINESS"
                    };

                    materialResults = new BackendAdapter(new CommonParam()).Get<List<HisMaterialTypeView1SDO>>("api/HisMaterialType/GetPriceLists", ApiConsumers.MosConsumer, filter, null);

                    Inventec.Common.Logging.LogSystem.Debug("HisMaterialType/GetPriceLists materialResults count: " + (materialResults != null ? materialResults.Count() : 0));
                }

                if (this.MediMatiType == Thuoc && medicineResults != null && medicineResults.Count > 0)
                {
                    Inventec.Common.RichEditor.RichEditorStore richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(HIS.Desktop.ApiConsumer.ApiConsumers.SarConsumer, HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), HIS.Desktop.LocalStorage.Location.PrintStoreLocation.PrintTemplatePath);
                    richEditorMain.RunPrintTemplate("Mps000335", DelegateRunPrinter);
                }
                else if (this.MediMatiType == VatTu && materialResults != null && materialResults.Count > 0)
                {
                    Inventec.Common.RichEditor.RichEditorStore richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(HIS.Desktop.ApiConsumer.ApiConsumers.SarConsumer, HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), HIS.Desktop.LocalStorage.Location.PrintStoreLocation.PrintTemplatePath);
                    richEditorMain.RunPrintTemplate("Mps000336", DelegateRunPrinterMaterial);
                }

                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barButton_Export_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            btnExport_Click(null, null);
        }

        private List<HIS_MEDICINE> ProcessGetApiMax(List<long> processImport)
        {
            List<HIS_MEDICINE> result = new List<HIS_MEDICINE>();
            try
            {
                var skip = 0;
                while (processImport.Count - skip >= 0)
                {
                    var imports = processImport.Skip(skip).Take(1000).ToList();
                    skip += 1000;
                    CommonParam param = new CommonParam();
                    MOS.Filter.HisMedicineFilter filter = new MOS.Filter.HisMedicineFilter();
                    filter.MEDICINE_TYPE_IDs = imports;
                    var rsApi = new BackendAdapter(param).Get<List<HIS_MEDICINE>>("api/HisMedicine/Get", ApiConsumer.ApiConsumers.MosConsumer, filter, param);
                    if (rsApi != null)
                    {
                        result.AddRange(rsApi);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private void GetMedicne(ref List<V_HIS_MEDICINE_PATY> medicinePaties, ref List<HIS_MEDICINE> medicineOthers, List<long> medicineTypeIds)
        {
            try
            {
                if (medicineTypeIds == null || medicineTypeIds.Count == 0)
                    return;

                var ListMedicine = ProcessGetApiMax(medicineTypeIds);

                List<long> medicineNotSaleEqualImps = ListMedicine != null && ListMedicine.Count() > 0 ? ListMedicine.Where(o =>
                    !o.IS_SALE_EQUAL_IMP_PRICE.HasValue
                    || o.IS_SALE_EQUAL_IMP_PRICE.Value != 1).Select(p => p.ID).ToList() : null;

                if (medicineNotSaleEqualImps != null && medicineNotSaleEqualImps.Count() > 0)
                {
                    MOS.Filter.HisMedicinePatyFilter medicinePatyFilter = new HisMedicinePatyFilter();
                    medicinePatyFilter.MEDICINE_IDs = medicineNotSaleEqualImps;
                    var medicinePatiesAdd = BackendDataWorker.Get<V_HIS_MEDICINE_PATY>().Where(o => medicineNotSaleEqualImps.Contains(o.MEDICINE_ID)).ToList();
                    if (medicinePatiesAdd != null && medicinePatiesAdd.Count() > 0)
                        medicinePaties.AddRange(medicinePatiesAdd);
                }

                //var medicineOtherAdds = ListMedicine.Where(o => o.IS_SALE_EQUAL_IMP_PRICE.HasValue && o.IS_SALE_EQUAL_IMP_PRICE == 1).ToList();
                if (ListMedicine != null && ListMedicine.Count() > 0)
                    medicineOthers.AddRange(ListMedicine);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private List<HIS_MATERIAL> ProcessGetApiMax(List<long> processImport, short type)
        {
            if (type != VatTu)
                return null;

            List<HIS_MATERIAL> result = new List<HIS_MATERIAL>();
            try
            {
                var skip = 0;
                while (processImport.Count - skip >= 0)
                {
                    var imports = processImport.Skip(skip).Take(1000).ToList();
                    skip += 1000;
                    CommonParam param = new CommonParam();
                    MOS.Filter.HisMaterialFilter filter = new MOS.Filter.HisMaterialFilter();
                    filter.MATERIAL_TYPE_IDs = imports;
                    var rsApi = new BackendAdapter(param).Get<List<HIS_MATERIAL>>("api/HisMaterial/Get", ApiConsumer.ApiConsumers.MosConsumer, filter, param);
                    if (rsApi != null)
                    {
                        result.AddRange(rsApi);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private void GetMaterial(ref List<V_HIS_MATERIAL_PATY> medicinePaties, ref List<HIS_MATERIAL> medicineOthers, List<long> medicineTypeIds)
        {
            try
            {
                if (medicineTypeIds == null || medicineTypeIds.Count == 0)
                    return;

                var ListMaterial = ProcessGetApiMax(medicineTypeIds, VatTu);

                List<long> medicineNotSaleEqualImps = ListMaterial != null && ListMaterial.Count() > 0 ? ListMaterial.Where(o =>
                    !o.IS_SALE_EQUAL_IMP_PRICE.HasValue
                    || o.IS_SALE_EQUAL_IMP_PRICE.Value != 1).Select(p => p.ID).ToList() : null;

                if (medicineNotSaleEqualImps != null && medicineNotSaleEqualImps.Count() > 0)
                {
                    MOS.Filter.HisMaterialPatyFilter medicinePatyFilter = new HisMaterialPatyFilter();
                    medicinePatyFilter.MATERIAL_IDs = medicineNotSaleEqualImps;
                    var medicinePatiesAdd = BackendDataWorker.Get<V_HIS_MATERIAL_PATY>().Where(o => medicineNotSaleEqualImps.Contains(o.MATERIAL_ID)).ToList();
                    if (medicinePatiesAdd != null && medicinePatiesAdd.Count() > 0)
                        medicinePaties.AddRange(medicinePatiesAdd);
                }

                //var medicineOtherAdds = ListMaterial.Where(o => o.IS_SALE_EQUAL_IMP_PRICE.HasValue && o.IS_SALE_EQUAL_IMP_PRICE == 1).ToList();
                if (ListMaterial != null && ListMaterial.Count() > 0)
                    medicineOthers.AddRange(ListMaterial);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private bool DelegateRunPrinter(string printTypeCode, string fileName)
        {
            bool result = false;
            try
            {
                if (medicineResults == null || medicineResults.Count == 0) return false;
                List<MediMateADO> selecteds = GetListCheck();
                if (selecteds == null || selecteds.Count <= 0)
                    return false;

                if (chkTachTheoNhom.CheckState == CheckState.Checked)
                {
                    var groupParent = selecteds.GroupBy(o => o.PARENT_ID).ToList();
                    foreach (var item in groupParent)
                    {
                        V_HIS_MEDICINE_TYPE parentType = BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>().FirstOrDefault(o => o.ID == item.Key);
                        List<V_HIS_MEDICINE_PATY> medicinePaties = new List<V_HIS_MEDICINE_PATY>();
                        List<HIS_MEDICINE> medicines = new List<HIS_MEDICINE>();
                        GetMedicne(ref medicinePaties, ref medicines, item.Select(o => o.ID).ToList());

                        List<HisMedicineTypeView1SDO> listPrintData = new List<HisMedicineTypeView1SDO>();
                        List<HisMedicineTypeView1SDO> isleafs = medicineResults.Where(o => item.Any(a => a.ID == o.ID)).ToList();
                        foreach (var datap in isleafs)
                        {
                            var mety = BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>().FirstOrDefault(o => o.ID == datap.ID);
                            if (mety != null)
                            {
                                HisMedicineTypeView1SDO sdo = new HisMedicineTypeView1SDO();
                                Inventec.Common.Mapper.DataObjectMapper.Map<HisMedicineTypeView1SDO>(sdo, mety);

                                sdo.ID = datap.ID;
                                sdo.CONCENTRA = datap.CONCENTRA;
                                sdo.MEDICINE_TYPE_CODE = datap.MEDICINE_TYPE_CODE;
                                sdo.MEDICINE_TYPE_NAME = datap.MEDICINE_TYPE_NAME;
                                sdo.MEDICINE_INFO = datap.MEDICINE_INFO;
                                sdo.PARENT_ID = datap.PARENT_ID;
                                sdo.SERVICE_UNIT_CODE = datap.SERVICE_UNIT_CODE;
                                sdo.SERVICE_UNIT_NAME = datap.SERVICE_UNIT_NAME;
                                sdo.IS_LEAF = datap.IS_LEAF;
                                sdo.IS_SALE_EQUAL_IMP_PRICE = datap.IS_SALE_EQUAL_IMP_PRICE;
                                sdo.MANUFACTURER_ID = datap.MANUFACTURER_ID;
                                sdo.NATIONAL_NAME = datap.NATIONAL_NAME;
                                sdo.IS_BUSINESS = datap.IS_BUSINESS;

                                sdo.EXP_PRICE = datap.EXP_PRICE;
                                sdo.EXP_VAT_RATIO = datap.EXP_VAT_RATIO;
                                sdo.IMP_TIME = datap.IMP_TIME;
                                sdo.IS_SALE_EQUAL_IMP_PRICE = datap.IS_SALE_EQUAL_IMP_PRICE;
                                sdo.MEDICINE_ID = datap.MEDICINE_ID;
                                sdo.MEDICINE_IMP_PRICE = datap.MEDICINE_IMP_PRICE;
                                sdo.MEDICINE_IMP_VAT_RATIO = datap.MEDICINE_IMP_VAT_RATIO;
                                sdo.PRICE = datap.PRICE;

                                listPrintData.Add(sdo);
                            }
                            else
                            {
                                listPrintData.Add(datap);
                            }
                        }

                        //có thuốc hết tồn
                        if (listPrintData.Count != item.Count())
                        {
                            List<MediMateADO> metyOutInventory = item.Where(o => !listPrintData.Exists(a => a.ID == o.ID)).ToList();
                            List<V_HIS_MEDICINE_TYPE> NewMety = ProcessGetNewMetyById(metyOutInventory.Select(s => s.ID).ToList());

                            foreach (var datap in metyOutInventory)
                            {
                                V_HIS_MEDICINE_TYPE mety = NewMety.FirstOrDefault(o => o.ID == datap.ID);
                                if (mety != null)
                                {
                                    HisMedicineTypeView1SDO sdo = new HisMedicineTypeView1SDO();
                                    Inventec.Common.Mapper.DataObjectMapper.Map<HisMedicineTypeView1SDO>(sdo, mety);

                                    sdo.ID = datap.ID;
                                    sdo.PARENT_ID = datap.PARENT_ID;
                                    sdo.IS_LEAF = datap.IS_LEAF;

                                    sdo.EXP_PRICE = mety.LAST_EXP_PRICE;
                                    sdo.EXP_VAT_RATIO = mety.LAST_EXP_VAT_RATIO;
                                    sdo.MEDICINE_ID = null;
                                    sdo.MEDICINE_IMP_PRICE = mety.LAST_IMP_PRICE;
                                    sdo.MEDICINE_IMP_VAT_RATIO = mety.LAST_IMP_VAT_RATIO;
                                    sdo.PRICE = mety.LAST_IMP_PRICE;

                                    listPrintData.Add(sdo);
                                }
                            }
                        }

                        MPS.Processor.Mps000335.PDO.Mps000335PDO pdo = new MPS.Processor.Mps000335.PDO.Mps000335PDO(null, parentType, listPrintData, medicines, medicinePaties, this.ListSupplier, this.ListManufacturer, saleProfits);
                        pdo.TotalMedicineType = BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>();
                        MPS.ProcessorBase.Core.PrintData PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "");
                        MPS.MpsPrinter.Run(PrintData);
                    }

                    result = true;
                }
                else
                {
                    List<V_HIS_MEDICINE_PATY> medicinePaties = new List<V_HIS_MEDICINE_PATY>();
                    List<HIS_MEDICINE> medicines = new List<HIS_MEDICINE>();
                    List<HisMedicineTypeView1SDO> isleafs = medicineResults.Where(o => selecteds.Any(a => a.ID == o.ID)).ToList();
                    List<HisMedicineTypeView1SDO> isleafsNew = new List<HisMedicineTypeView1SDO>();
                    if (isleafs != null && isleafs.Count > 0)
                    {
                        GetMedicne(ref medicinePaties, ref medicines, isleafs.Select(o => o.ID).ToList());

                        foreach (var datap in isleafs)
                        {
                            var mety = BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>().FirstOrDefault(o => o.ID == datap.ID);
                            if (mety != null)
                            {
                                HisMedicineTypeView1SDO sdo = new HisMedicineTypeView1SDO();
                                Inventec.Common.Mapper.DataObjectMapper.Map<HisMedicineTypeView1SDO>(sdo, mety);

                                sdo.ID = datap.ID;
                                sdo.CONCENTRA = datap.CONCENTRA;
                                sdo.MEDICINE_TYPE_CODE = datap.MEDICINE_TYPE_CODE;
                                sdo.MEDICINE_TYPE_NAME = datap.MEDICINE_TYPE_NAME;
                                sdo.MEDICINE_INFO = datap.MEDICINE_INFO;
                                sdo.PARENT_ID = datap.PARENT_ID;
                                sdo.SERVICE_UNIT_CODE = datap.SERVICE_UNIT_CODE;
                                sdo.SERVICE_UNIT_NAME = datap.SERVICE_UNIT_NAME;
                                sdo.IS_LEAF = datap.IS_LEAF;
                                sdo.IS_SALE_EQUAL_IMP_PRICE = datap.IS_SALE_EQUAL_IMP_PRICE;
                                sdo.MANUFACTURER_ID = datap.MANUFACTURER_ID;
                                sdo.NATIONAL_NAME = datap.NATIONAL_NAME;
                                sdo.IS_BUSINESS = datap.IS_BUSINESS;

                                sdo.EXP_PRICE = datap.EXP_PRICE;
                                sdo.EXP_VAT_RATIO = datap.EXP_VAT_RATIO;
                                sdo.IMP_TIME = datap.IMP_TIME;
                                sdo.IS_SALE_EQUAL_IMP_PRICE = datap.IS_SALE_EQUAL_IMP_PRICE;
                                sdo.MEDICINE_ID = datap.MEDICINE_ID;
                                sdo.MEDICINE_IMP_PRICE = datap.MEDICINE_IMP_PRICE;
                                sdo.MEDICINE_IMP_VAT_RATIO = datap.MEDICINE_IMP_VAT_RATIO;
                                sdo.PRICE = datap.PRICE;

                                isleafsNew.Add(sdo);
                            }
                            else
                            {
                                isleafsNew.Add(datap);
                            }
                        }
                    }

                    //có thuốc hết tồn
                    if (isleafsNew.Count != selecteds.Count)
                    {
                        List<MediMateADO> metyOutInventory = selecteds.Where(o => !isleafsNew.Exists(a => a.ID == o.ID)).ToList();
                        List<V_HIS_MEDICINE_TYPE> NewMety = ProcessGetNewMetyById(metyOutInventory.Select(s => s.ID).ToList());

                        foreach (var datap in metyOutInventory)
                        {
                            V_HIS_MEDICINE_TYPE mety = NewMety.FirstOrDefault(o => o.ID == datap.ID);
                            if (mety != null)
                            {
                                HisMedicineTypeView1SDO sdo = new HisMedicineTypeView1SDO();
                                Inventec.Common.Mapper.DataObjectMapper.Map<HisMedicineTypeView1SDO>(sdo, mety);

                                sdo.ID = datap.ID;
                                sdo.PARENT_ID = datap.PARENT_ID;
                                sdo.IS_LEAF = datap.IS_LEAF;

                                sdo.EXP_PRICE = mety.LAST_EXP_PRICE;
                                sdo.EXP_VAT_RATIO = mety.LAST_EXP_VAT_RATIO;
                                sdo.MEDICINE_ID = null;
                                sdo.MEDICINE_IMP_PRICE = mety.LAST_IMP_PRICE;
                                sdo.MEDICINE_IMP_VAT_RATIO = mety.LAST_IMP_VAT_RATIO;
                                sdo.PRICE = mety.LAST_IMP_PRICE;

                                isleafsNew.Add(sdo);
                            }
                        }
                    }

                    medicinePaties = medicinePaties != null ? medicinePaties.Distinct().ToList() : medicinePaties;
                    medicines = medicines != null ? medicines.Distinct().ToList() : medicines;

                    MPS.Processor.Mps000335.PDO.Mps000335PDO pdo = new MPS.Processor.Mps000335.PDO.Mps000335PDO(null, null, null, medicines, medicinePaties, this.ListSupplier, this.ListManufacturer, isleafsNew, saleProfits);
                    pdo.TotalMedicineType = BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>();
                    MPS.ProcessorBase.Core.PrintData PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "");
                    MPS.MpsPrinter.Run(PrintData);
                    result = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
            return result;
        }

        private List<V_HIS_MEDICINE_TYPE> ProcessGetNewMetyById(List<long> metyIds)
        {
            List<V_HIS_MEDICINE_TYPE> result = new List<V_HIS_MEDICINE_TYPE>();
            try
            {
                if (metyIds != null && metyIds.Count > 0)
                {
                    metyIds = metyIds.Distinct().ToList();
                    int skip = 0;
                    while (metyIds.Count - skip > 0)
                    {
                        var listIds = metyIds.Skip(skip).Take(100).ToList();
                        skip += 100;

                        CommonParam param = new CommonParam();
                        HisMedicineTypeViewFilter filter = new HisMedicineTypeViewFilter();
                        filter.IDs = listIds;
                        var lstMatys = new BackendAdapter(param).Get<List<V_HIS_MEDICINE_TYPE>>("api/HisMedicineType/GetView", ApiConsumers.MosConsumer, filter, param);
                        if (lstMatys != null && lstMatys.Count > 0)
                        {
                            result.AddRange(lstMatys);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                result = new List<V_HIS_MEDICINE_TYPE>();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private bool DelegateRunPrinterMaterial(string printTypeCode, string fileName)
        {
            bool result = false;
            try
            {
                if (materialResults == null || materialResults.Count == 0) return false;
                List<MediMateADO> selecteds = GetListCheck();
                if (selecteds == null || selecteds.Count <= 0)
                    return false;

                if (chkTachTheoNhom.CheckState == CheckState.Checked)
                {
                    var groupParent = selecteds.GroupBy(o => o.PARENT_ID).ToList();
                    foreach (var item in groupParent)
                    {
                        List<HisMaterialTypeView1SDO> listPrintData = new List<HisMaterialTypeView1SDO>();
                        List<V_HIS_MATERIAL_PATY> medicinePaties = new List<V_HIS_MATERIAL_PATY>();
                        List<HIS_MATERIAL> medicines = new List<HIS_MATERIAL>();
                        V_HIS_MATERIAL_TYPE parentType = BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>().FirstOrDefault(o => o.ID == item.Key);
                        List<HisMaterialTypeView1SDO> isleafs = materialResults.Where(o => item.Any(a => a.ID == o.ID)).ToList();
                        if (isleafs != null && isleafs.Count > 0)
                        {
                            GetMaterial(ref medicinePaties, ref medicines, isleafs.Select(o => o.ID).ToList());

                            foreach (var datap in isleafs)
                            {
                                var maty = BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>().FirstOrDefault(o => o.ID == datap.ID);
                                if (maty != null)
                                {
                                    HisMaterialTypeView1SDO sdo = new HisMaterialTypeView1SDO();
                                    Inventec.Common.Mapper.DataObjectMapper.Map<HisMaterialTypeView1SDO>(sdo, maty);

                                    sdo.ID = datap.ID;
                                    sdo.CONCENTRA = datap.CONCENTRA;
                                    sdo.MATERIAL_TYPE_CODE = datap.MATERIAL_TYPE_CODE;
                                    sdo.MATERIAL_TYPE_NAME = datap.MATERIAL_TYPE_NAME;
                                    sdo.MATERIAL_INFO = datap.MATERIAL_INFO;
                                    sdo.PARENT_ID = datap.PARENT_ID;
                                    sdo.SERVICE_UNIT_CODE = datap.SERVICE_UNIT_CODE;
                                    sdo.SERVICE_UNIT_NAME = datap.SERVICE_UNIT_NAME;
                                    sdo.IS_LEAF = datap.IS_LEAF;
                                    sdo.IS_SALE_EQUAL_IMP_PRICE = datap.IS_SALE_EQUAL_IMP_PRICE;
                                    sdo.MANUFACTURER_ID = datap.MANUFACTURER_ID;
                                    sdo.NATIONAL_NAME = datap.NATIONAL_NAME;
                                    sdo.IS_BUSINESS = datap.IS_BUSINESS;

                                    sdo.EXP_PRICE = datap.EXP_PRICE;
                                    sdo.EXP_VAT_RATIO = datap.EXP_VAT_RATIO;
                                    sdo.IMP_TIME = datap.IMP_TIME;
                                    sdo.IS_SALE_EQUAL_IMP_PRICE = datap.IS_SALE_EQUAL_IMP_PRICE;
                                    sdo.MATERIAL_ID = datap.MATERIAL_ID;
                                    sdo.MATERIAL_IMP_PRICE = datap.MATERIAL_IMP_PRICE;
                                    sdo.MATERIAL_IMP_VAT_RATIO = datap.MATERIAL_IMP_VAT_RATIO;
                                    sdo.PRICE = datap.PRICE;

                                    listPrintData.Add(sdo);
                                }
                                else
                                {
                                    listPrintData.Add(datap);
                                }
                            }
                        }

                        //có vật tư hết tồn
                        if (listPrintData.Count != item.Count())
                        {
                            List<MediMateADO> matyOutInventory = item.Where(o => !listPrintData.Exists(a => a.ID == o.ID)).ToList();
                            List<V_HIS_MATERIAL_TYPE> NewMaty = ProcessGetNewMatyById(matyOutInventory.Select(s => s.ID).ToList());

                            foreach (var datap in matyOutInventory)
                            {
                                var maty = NewMaty.FirstOrDefault(o => o.ID == datap.ID);
                                if (maty != null)
                                {
                                    HisMaterialTypeView1SDO sdo = new HisMaterialTypeView1SDO();
                                    Inventec.Common.Mapper.DataObjectMapper.Map<HisMaterialTypeView1SDO>(sdo, maty);

                                    sdo.ID = datap.ID;
                                    sdo.PARENT_ID = datap.PARENT_ID;
                                    sdo.IS_LEAF = datap.IS_LEAF;

                                    sdo.EXP_PRICE = maty.LAST_EXP_PRICE;
                                    sdo.EXP_VAT_RATIO = maty.LAST_EXP_VAT_RATIO;
                                    sdo.MATERIAL_ID = null;
                                    sdo.MATERIAL_IMP_PRICE = maty.LAST_IMP_PRICE;
                                    sdo.MATERIAL_IMP_VAT_RATIO = maty.LAST_IMP_VAT_RATIO;
                                    sdo.PRICE = maty.LAST_IMP_PRICE;

                                    listPrintData.Add(sdo);
                                }
                            }
                        }

                        MPS.Processor.Mps000336.PDO.Mps000336PDO pdo = new MPS.Processor.Mps000336.PDO.Mps000336PDO(null, parentType, listPrintData, medicines, medicinePaties, this.ListSupplier, this.ListManufacturer, saleProfits);
                        pdo.TotalMaterialType = BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>();
                        MPS.ProcessorBase.Core.PrintData PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "");
                        MPS.MpsPrinter.Run(PrintData);
                    }

                    result = true;
                }
                else
                {
                    List<V_HIS_MATERIAL_PATY> medicinePaties = new List<V_HIS_MATERIAL_PATY>();
                    List<HIS_MATERIAL> medicines = new List<HIS_MATERIAL>();
                    List<HisMaterialTypeView1SDO> isleafs = materialResults.Where(o => selecteds.Any(a => a.ID == o.ID)).ToList();
                    List<HisMaterialTypeView1SDO> isleafNews = new List<HisMaterialTypeView1SDO>();
                    if (isleafs != null && isleafs.Count > 0)
                    {
                        GetMaterial(ref medicinePaties, ref medicines, isleafs.Select(o => o.ID).ToList());

                        foreach (var datap in isleafs)
                        {
                            var maty = BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>().FirstOrDefault(o => o.ID == datap.ID);
                            if (maty != null)
                            {
                                HisMaterialTypeView1SDO sdo = new HisMaterialTypeView1SDO();
                                Inventec.Common.Mapper.DataObjectMapper.Map<HisMaterialTypeView1SDO>(sdo, maty);

                                sdo.ID = datap.ID;
                                sdo.CONCENTRA = datap.CONCENTRA;
                                sdo.MATERIAL_TYPE_CODE = datap.MATERIAL_TYPE_CODE;
                                sdo.MATERIAL_TYPE_NAME = datap.MATERIAL_TYPE_NAME;
                                sdo.MATERIAL_INFO = datap.MATERIAL_INFO;
                                sdo.PARENT_ID = datap.PARENT_ID;
                                sdo.SERVICE_UNIT_CODE = datap.SERVICE_UNIT_CODE;
                                sdo.SERVICE_UNIT_NAME = datap.SERVICE_UNIT_NAME;
                                sdo.IS_LEAF = datap.IS_LEAF;
                                sdo.IS_SALE_EQUAL_IMP_PRICE = datap.IS_SALE_EQUAL_IMP_PRICE;
                                sdo.MANUFACTURER_ID = datap.MANUFACTURER_ID;
                                sdo.NATIONAL_NAME = datap.NATIONAL_NAME;
                                sdo.IS_BUSINESS = datap.IS_BUSINESS;

                                sdo.EXP_PRICE = datap.EXP_PRICE;
                                sdo.EXP_VAT_RATIO = datap.EXP_VAT_RATIO;
                                sdo.IMP_TIME = datap.IMP_TIME;
                                sdo.IS_SALE_EQUAL_IMP_PRICE = datap.IS_SALE_EQUAL_IMP_PRICE;
                                sdo.MATERIAL_ID = datap.MATERIAL_ID;
                                sdo.MATERIAL_IMP_PRICE = datap.MATERIAL_IMP_PRICE;
                                sdo.MATERIAL_IMP_VAT_RATIO = datap.MATERIAL_IMP_VAT_RATIO;
                                sdo.PRICE = datap.PRICE;

                                isleafNews.Add(sdo);
                            }
                            else
                            {
                                isleafNews.Add(datap);
                            }
                        }
                    }

                    //có vật tư hết tồn
                    if (isleafNews.Count != selecteds.Count)
                    {
                        var matyOutInventory = selecteds.Where(o => !isleafNews.Exists(a => a.ID == o.ID)).ToList();
                        List<V_HIS_MATERIAL_TYPE> NewMaty = ProcessGetNewMatyById(matyOutInventory.Select(s => s.ID).ToList());

                        foreach (var datap in matyOutInventory)
                        {
                            var maty = NewMaty.FirstOrDefault(o => o.ID == datap.ID);
                            if (maty != null)
                            {
                                HisMaterialTypeView1SDO sdo = new HisMaterialTypeView1SDO();
                                Inventec.Common.Mapper.DataObjectMapper.Map<HisMaterialTypeView1SDO>(sdo, maty);

                                sdo.ID = datap.ID;
                                sdo.PARENT_ID = datap.PARENT_ID;
                                sdo.IS_LEAF = datap.IS_LEAF;

                                sdo.EXP_PRICE = maty.LAST_EXP_PRICE;
                                sdo.EXP_VAT_RATIO = maty.LAST_EXP_VAT_RATIO;
                                sdo.MATERIAL_ID = null;
                                sdo.MATERIAL_IMP_PRICE = maty.LAST_IMP_PRICE;
                                sdo.MATERIAL_IMP_VAT_RATIO = maty.LAST_IMP_VAT_RATIO;
                                sdo.PRICE = maty.LAST_IMP_PRICE;

                                isleafNews.Add(sdo);
                            }
                        }
                    }

                    medicinePaties = medicinePaties != null ? medicinePaties.Distinct().ToList() : medicinePaties;
                    medicines = medicines != null ? medicines.Distinct().ToList() : medicines;

                    MPS.Processor.Mps000336.PDO.Mps000336PDO pdo = new MPS.Processor.Mps000336.PDO.Mps000336PDO(null, null, null, medicines, medicinePaties, this.ListSupplier, this.ListManufacturer, isleafNews, saleProfits);
                    pdo.TotalMaterialType = BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>();
                    MPS.ProcessorBase.Core.PrintData PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "");
                    MPS.MpsPrinter.Run(PrintData);
                    result = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
            return result;
        }

        private List<V_HIS_MATERIAL_TYPE> ProcessGetNewMatyById(List<long> matyIds)
        {
            List<V_HIS_MATERIAL_TYPE> result = new List<V_HIS_MATERIAL_TYPE>();
            try
            {
                if (matyIds != null && matyIds.Count > 0)
                {
                    matyIds = matyIds.Distinct().ToList();
                    int skip = 0;
                    while (matyIds.Count - skip > 0)
                    {
                        var listIds = matyIds.Skip(skip).Take(100).ToList();
                        skip += 100;

                        CommonParam param = new CommonParam();
                        HisMaterialTypeViewFilter filter = new HisMaterialTypeViewFilter();
                        filter.IDs = listIds;
                        var lstMatys = new BackendAdapter(param).Get<List<V_HIS_MATERIAL_TYPE>>("api/HisMaterialType/GetView", ApiConsumers.MosConsumer, filter, param);
                        if (lstMatys != null && lstMatys.Count > 0)
                        {
                            result.AddRange(lstMatys);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                result = new List<V_HIS_MATERIAL_TYPE>();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private List<HisMedicineTypeView1SDO> GetAllChildByRoot(long parentId)
        {
            List<HisMedicineTypeView1SDO> results = new List<HisMedicineTypeView1SDO>();
            var childs = BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>().Where(o => o.PARENT_ID == parentId).ToList();
            var childIsLeafs = childs != null ? childs.Where(o => o.IS_LEAF == 1).ToList() : null;
            var childNotLeafs = childs != null ? childs.Where(o => o.IS_LEAF != 1).ToList() : null;
            if (childIsLeafs != null && childIsLeafs.Count > 0)
            {
                results.AddRange(medicineResults.Where(o => childIsLeafs.Any(a => a.ID == o.ID)).ToList());
            }
            if (childNotLeafs != null && childNotLeafs.Count > 0)
            {
                foreach (var c in childNotLeafs)
                {
                    var rs = this.GetAllChildByRoot(c.ID);
                    if (rs != null && rs.Count > 0)
                    {
                        results.AddRange(rs);
                    }
                }
            }
            return results;
        }

        private List<HisMaterialTypeView1SDO> GetAllChildByRoot(long parentId, short type)
        {
            if (type != VatTu)
                return null;

            List<HisMaterialTypeView1SDO> results = new List<HisMaterialTypeView1SDO>();
            var childs = BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>().Where(o => o.PARENT_ID == parentId).ToList();
            var childIsLeafs = childs != null ? childs.Where(o => o.IS_LEAF == 1).ToList() : null;
            var childNotLeafs = childs != null ? childs.Where(o => o.IS_LEAF != 1).ToList() : null;
            if (childIsLeafs != null && childIsLeafs.Count > 0)
            {
                results.AddRange(materialResults.Where(o => childIsLeafs.Any(a => a.ID == o.ID)).ToList());
            }
            if (childNotLeafs != null && childNotLeafs.Count > 0)
            {
                foreach (var c in childNotLeafs)
                {
                    var rs = this.GetAllChildByRoot(c.ID, type);
                    if (rs != null && rs.Count > 0)
                    {
                        results.AddRange(rs);
                    }
                }
            }
            return results;
        }

        private List<HisMaterialTypeView1SDO> GetAllChildByRoot(MediMateADO parentId, short mediType)
        {
            if (mediType != VatTu)
            {
                return null;
            }
            List<HisMaterialTypeView1SDO> results = new List<HisMaterialTypeView1SDO>();
            var childs = BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>().Where(o => o.PARENT_ID == parentId.ID).ToList();
            var childIsLeafs = childs != null ? childs.Where(o => o.IS_LEAF == 1).ToList() : null;
            var childNotLeafs = childs != null ? childs.Where(o => o.IS_LEAF != 1).ToList() : null;
            if (childIsLeafs != null && childIsLeafs.Count > 0)
            {
                Parallel.ForEach(childIsLeafs.Where(f => f.ID > 0), l => l.PARENT_ID = parentId.ID);
                results.AddRange(materialResults.Where(o => childIsLeafs.Any(a => a.ID == o.ID)).ToList());
            }
            if (childNotLeafs != null && childNotLeafs.Count > 0)
            {
                foreach (var c in childNotLeafs)
                {
                    var rs = this.GetAllChildByRoot(c.ID, VatTu);
                    if (rs != null && rs.Count > 0)
                    {
                        Parallel.ForEach(rs.Where(f => f.ID > 0), l => l.PARENT_ID = parentId.ID);
                        results.AddRange(rs);
                    }
                }
            }
            return results;
        }

        private List<HisMedicineTypeView1SDO> GetAllChildByRoot(MediMateADO parentId)
        {
            List<HisMedicineTypeView1SDO> results = new List<HisMedicineTypeView1SDO>();
            var childs = BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>().Where(o => o.PARENT_ID == parentId.ID).ToList();
            var childIsLeafs = childs != null ? childs.Where(o => o.IS_LEAF == 1).ToList() : null;
            var childNotLeafs = childs != null ? childs.Where(o => o.IS_LEAF != 1).ToList() : null;
            if (childIsLeafs != null && childIsLeafs.Count > 0)
            {
                Parallel.ForEach(childIsLeafs.Where(f => f.ID > 0), l => l.PARENT_ID = parentId.ID);
                results.AddRange(medicineResults.Where(o => childIsLeafs.Any(a => a.ID == o.ID)).ToList());
            }
            if (childNotLeafs != null && childNotLeafs.Count > 0)
            {
                foreach (var c in childNotLeafs)
                {
                    var rs = this.GetAllChildByRoot(c.ID);
                    if (rs != null && rs.Count > 0)
                    {
                        Parallel.ForEach(rs.Where(f => f.ID > 0), l => l.PARENT_ID = parentId.ID);
                        results.AddRange(rs);
                    }
                }
            }
            return results;
        }

        private void txtKeyword_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnFind_Click(null, null);
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
                List<MediMateADO> datas = null;
                if (this.MediStockSelected != null && this.MediStockSelected.Count > 0)
                {
                    CommonParam param = new CommonParam();
                    bool isIncludeBaseAmount = (this.MediStockSelected.Count == 1 && this.MediStockSelected.Any(a => a.IS_CABINET == 1));
                    if (MediMatiType == Thuoc)
                    {
                        MOS.Filter.HisMedicineStockViewFilter mediFilter = new MOS.Filter.HisMedicineStockViewFilter();
                        mediFilter.MEDI_STOCK_IDs = this.MediStockSelected.Select(s => s.ID).ToList();
                        mediFilter.INCLUDE_EMPTY = chkShowAllType.Checked;
                        mediFilter.INCLUDE_BASE_AMOUNT = isIncludeBaseAmount;

                        var lstMediInStocks = new BackendAdapter(param).Get<List<HisMedicineInStockSDO>>(HisRequestUriStore.HIS_MEDICINE_GETVIEW_IN_STOCK_MEDICINE_TYPE_TREE, ApiConsumers.MosConsumer, mediFilter, param);
                        if (lstMediInStocks != null && lstMediInStocks.Count > 0)
                        {
                            var dataMediStocks = BackendDataWorker.Get<V_HIS_MEDI_STOCK>().Where(p => this.MediStockSelected.Select(s => s.ID).Contains(p.ID) && p.IS_BUSINESS == 1).ToList();
                            var dataMediStocksBUSINESS = BackendDataWorker.Get<V_HIS_MEDI_STOCK>().Where(p => this.MediStockSelected.Select(s => s.ID).Contains(p.ID) && p.IS_BUSINESS != 1).ToList();
                            if (dataMediStocks != null && dataMediStocks.Count == this.MediStockSelected.Count)
                            {
                                datas = ListData.Where(o => lstMediInStocks.Exists(s => s.MEDICINE_TYPE_ID == o.ID) && o.IS_BUSINESS == 1).ToList();
                            }
                            else if (dataMediStocksBUSINESS != null && dataMediStocksBUSINESS.Count == this.MediStockSelected.Count)
                            {
                                datas = ListData.Where(o => lstMediInStocks.Exists(s => s.MEDICINE_TYPE_ID == o.ID) && o.IS_BUSINESS != 1).ToList();
                            }
                            else
                            {
                                datas = ListData.Where(o => lstMediInStocks.Exists(s => s.MEDICINE_TYPE_ID == o.ID)).ToList();
                            }
                        }
                    }
                    else if (MediMatiType == VatTu)
                    {
                        MOS.Filter.HisMaterialStockViewFilter mateFilter = new MOS.Filter.HisMaterialStockViewFilter();
                        mateFilter.MEDI_STOCK_IDs = this.MediStockSelected.Select(s => s.ID).ToList();
                        mateFilter.INCLUDE_EMPTY = chkShowAllType.Checked;
                        mateFilter.INCLUDE_BASE_AMOUNT = isIncludeBaseAmount;

                        var lstMateInStocks = new BackendAdapter(param).Get<List<HisMaterialInStockSDO>>(HisRequestUriStore.HIS_MATERIAL_GETVIEW_IN_STOCK_MATERIAL_TYPE_TREE, ApiConsumers.MosConsumer, mateFilter, param);
                        if (lstMateInStocks != null && lstMateInStocks.Count > 0)
                        {
                            var dataMediStocks = BackendDataWorker.Get<V_HIS_MEDI_STOCK>().Where(p => this.MediStockSelected.Select(s => s.ID).Contains(p.ID) && p.IS_BUSINESS == 1).ToList();
                            var dataMediStocksBUSINESS = BackendDataWorker.Get<V_HIS_MEDI_STOCK>().Where(p => this.MediStockSelected.Select(s => s.ID).Contains(p.ID) && p.IS_BUSINESS != 1).ToList();
                            if (dataMediStocks != null && dataMediStocks.Count == this.MediStockSelected.Count)
                            {
                                datas = ListData.Where(o => lstMateInStocks.Exists(s => s.MATERIAL_TYPE_ID == o.ID) && o.IS_BUSINESS == 1).ToList();
                            }
                            else if (dataMediStocksBUSINESS != null && dataMediStocksBUSINESS.Count == this.MediStockSelected.Count)
                            {
                                datas = ListData.Where(o => lstMateInStocks.Exists(s => s.MATERIAL_TYPE_ID == o.ID) && o.IS_BUSINESS != 1).ToList();
                            }
                            else
                            {
                                datas = ListData.Where(o => lstMateInStocks.Exists(s => s.MATERIAL_TYPE_ID == o.ID)).ToList();
                            }
                        }
                    }

                    if (!String.IsNullOrWhiteSpace(txtKeyword.Text.Trim()))
                    {
                        string key = txtKeyword.Text.Trim().ToLower();
                        datas = datas.Where(o => o.SERVICE_NAME_HIDDEN.ToLower().Contains(key) || o.SERVICE_CODE_HIDDEN.ToLower().Contains(key)).ToList();
                    }
                }

                //đã lọc dữ liệu
                //kiểm tra để bổ sung nhóm cha
                if (datas != null && datas.Count > 0 && ListData != null && ListData.Count > 0 && datas.Count < ListData.Count)
                {
                    List<long> listParent = datas.Where(o => o.PARENT_ID.HasValue).Select(s => s.PARENT_ID ?? 0).Distinct().ToList();
                    if (listParent != null && listParent.Count > 0)
                    {
                        var parentInData = datas.Where(o => listParent.Contains(o.ID)).ToList();

                        if (parentInData.Count != listParent.Count)
                        {
                            var parentAdd = ListData.Where(o => listParent.Contains(o.ID) && !parentInData.Select(s => s.ID).Contains(o.ID)).ToList();
                            if (parentAdd != null && parentAdd.Count > 0)
                            {
                                datas.AddRange(parentAdd);
                            }
                        }
                    }
                }

                SetDatasource(datas);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barButton_Find_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            btnFind_Click(null, null);
        }

        private void cboMediStock_CustomDisplayText(object sender, DevExpress.XtraEditors.Controls.CustomDisplayTextEventArgs e)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                GridCheckMarksSelection gridCheckMark = sender is GridLookUpEdit ? (sender as GridLookUpEdit).Properties.Tag as GridCheckMarksSelection : (sender as RepositoryItemGridLookUpEdit).Tag as GridCheckMarksSelection;
                if (gridCheckMark == null) return;
                foreach (MOS.EFMODEL.DataModels.V_HIS_MEDI_STOCK rv in gridCheckMark.Selection)
                {
                    if (sb.ToString().Length > 0) { sb.Append(", "); }
                    sb.Append(rv.MEDI_STOCK_NAME);
                }
                e.DisplayText = sb.ToString();
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
                if (e.CloseMode == PopupCloseMode.Normal || e.CloseMode == PopupCloseMode.Immediate)
                {
                    chkShowAllType.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public List<MediMateADO> GetListCheck()
        {
            List<MediMateADO> result = new List<MediMateADO>();
            try
            {
                foreach (TreeListNode node in trvMediMate.Nodes)
                {
                    GetListNodeCheck(ref result, node);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = new List<MediMateADO>();
            }
            return result;
        }

        private void GetListNodeCheck(ref List<MediMateADO> result, TreeListNode node)
        {
            try
            {
                if (node.Nodes == null || node.Nodes.Count == 0)
                {
                    if (node.Checked)
                    {
                        result.Add((MediMateADO)trvMediMate.GetDataRecordByNode(node));
                    }
                }
                else
                {
                    foreach (TreeListNode child in node.Nodes)
                    {
                        GetListNodeCheck(ref result, child);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void trvMediMate_CustomDrawColumnHeader(object sender, DevExpress.XtraTreeList.CustomDrawColumnHeaderEventArgs e)
        {
            try
            {
                if (e.Column != null && e.Column.VisibleIndex == 0)
                {
                    Rectangle checkRect = new Rectangle(e.Bounds.Left + 3, e.Bounds.Top + 3, 12, 12);
                    ColumnInfo info = (ColumnInfo)e.ObjectArgs;
                    if (info.CaptionRect.Left < 30)
                        info.CaptionRect = new Rectangle(new Point(info.CaptionRect.Left + 15, info.CaptionRect.Top), info.CaptionRect.Size);
                    e.Painter.DrawObject(info);

                    DrawCheckBox(e.Cache, checkEdit, checkRect, IsAllSelected(sender as TreeList));
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private bool IsAllSelected(TreeList tree)
        {
            return tree.GetAllCheckedNodes().Count > 0 && tree.GetAllCheckedNodes().Count == tree.AllNodesCount;
        }

        protected void DrawCheckBox(GraphicsCache cache, RepositoryItemCheckEdit edit, Rectangle r, bool Checked)
        {
            DevExpress.XtraEditors.ViewInfo.CheckEditViewInfo info;
            DevExpress.XtraEditors.Drawing.CheckEditPainter painter;
            DevExpress.XtraEditors.Drawing.ControlGraphicsInfoArgs args;
            info = edit.CreateViewInfo() as DevExpress.XtraEditors.ViewInfo.CheckEditViewInfo;
            painter = edit.CreatePainter() as DevExpress.XtraEditors.Drawing.CheckEditPainter;
            info.EditValue = Checked;
            info.Bounds = r;
            info.CalcViewInfo();
            args = new DevExpress.XtraEditors.Drawing.ControlGraphicsInfoArgs(info, cache, r);
            painter.Draw(args);
        }

        private void trvMediMate_MouseUp(object sender, MouseEventArgs e)
        {
            try
            {
                TreeList tree = sender as TreeList;
                Point pt = new Point(e.X, e.Y);
                TreeListHitInfo hit = tree.CalcHitInfo(pt);
                if (hit.Column != null && hit.Column.VisibleIndex == 0)
                {
                    ColumnInfo info = tree.ViewInfo.ColumnsInfo[hit.Column];
                    Rectangle checkRect = new Rectangle(info.Bounds.Left + 3, info.Bounds.Top + 3, 12, 12);
                    if (checkRect.Contains(pt))
                    {
                        hit.Column.OptionsColumn.AllowSort = false;
                        EmbeddedCheckBoxChecked(tree);
                    }
                    else
                    {
                        hit.Column.OptionsColumn.AllowSort = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void EmbeddedCheckBoxChecked(TreeList tree)
        {
            try
            {
                if (IsAllSelected(tree))
                {
                    tree.BeginUpdate();
                    tree.NodesIterator.DoOperation(new UnSelectNodeOperation());
                    tree.EndUpdate();
                }
                else
                {
                    tree.BeginUpdate();
                    tree.NodesIterator.DoOperation(new SelectNodeOperation());
                    tree.EndUpdate();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        class SelectNodeOperation : TreeListOperation
        {
            public override void Execute(TreeListNode node)
            {
                node.Checked = true;
            }
        }

        class UnSelectNodeOperation : TreeListOperation
        {
            public override void Execute(TreeListNode node)
            {
                node.Checked = false;
            }
        }

        private void trvMediMate_AfterCheckNode(object sender, NodeEventArgs e)
        {
            try
            {
                trvMediMate.FocusedNode = e.Node;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}

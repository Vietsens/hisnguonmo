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
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigSystem;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.LocalStorage.Location;
using HIS.Desktop.Plugins.MediStockInventory.ADO;
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
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.MediStockInventory
{
    public partial class frmMediStockInventory : HIS.Desktop.Utility.FormBase
    {
        Inventec.Desktop.Common.Modules.Module currentModule = null;

        List<VHisExpMestMedicineADO> listExpMestMedicineADO = new List<VHisExpMestMedicineADO>();
        List<HisMaterialInStockSDO> listExpMestMaterialADO = new List<HisMaterialInStockSDO>();

        HisImpMestResultSDO resultMobaSdo = null;
        public long medistockId { get; set; }

        public frmMediStockInventory(Inventec.Desktop.Common.Modules.Module module)
            : base(module)
        {
            InitializeComponent();
            try
            {
                Base.ResourceLangManager.InitResourceLanguageManager();
                this.SetIcon();
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

        private void frmMediStockInventory_Load(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                LoadKeyFrmLanguage();
                LoadMedistock();
                LoadExpMestMedicine();
                LoadExpMestMaterial();
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

        private void LoadMedistock()
        {
            try
            {
                if (this.currentModule != null)
                {
                    var medicosk = BackendDataWorker.Get<V_HIS_MEDI_STOCK>().FirstOrDefault(o => o.ROOM_ID == this.currentModule.RoomId);
                    if (medicosk != null)
                    {
                        this.medistockId = medicosk.ID;
                    }
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
                if (this.medistockId > 0)
                {
                    CommonParam param = new CommonParam();
                    MOS.Filter.HisMedicineStockViewFilter mediFilter = new MOS.Filter.HisMedicineStockViewFilter();
                    mediFilter.MEDI_STOCK_ID = this.medistockId;
                    //mediFilter.INCLUDE_EMPTY = chkShowLineZero.Checked;
                    //mediFilter.INCLUDE_BASE_AMOUNT = isIncludeBaseAmount;
                    var lstMediInStocks = new List<HisMedicineInStockSDO>();
                    lstMediInStocks = new BackendAdapter(param).Get<List<HisMedicineInStockSDO>>(HisRequestUriStore.HIS_MEDICINE_GETVIEW_IN_STOCK_MEDICINE_TYPE_TREE, ApiConsumers.MosConsumer, mediFilter, param);

                    if (lstMediInStocks != null && lstMediInStocks.Count > 0)
                    {
                        var dataMediStocks = BackendDataWorker.Get<V_HIS_MEDI_STOCK>().Where(p => this.medistockId == p.ID && p.IS_BUSINESS == 1).ToList();
                        if (dataMediStocks != null && dataMediStocks.Count > 0)
                        {
                            var dataMediTypes = BackendDataWorker.Get<HIS_MEDICINE_TYPE>().Where(p => p.IS_BUSINESS == 1).ToList();
                            if (dataMediTypes != null && dataMediTypes.Count > 0)
                            {
                                lstMediInStocks = lstMediInStocks.Where(p => dataMediTypes.Select(o => o.ID).Distinct().ToList().Contains(p.MEDICINE_TYPE_ID)).ToList();
                            }
                            else
                            {
                                lstMediInStocks = new List<HisMedicineInStockSDO>();
                            }
                        }
                        else
                        {
                            var dataMediStocksBUSINESS = BackendDataWorker.Get<V_HIS_MEDI_STOCK>().Where(p => this.medistockId == p.ID && p.IS_BUSINESS != 1).ToList();
                            if (dataMediStocksBUSINESS != null && dataMediStocksBUSINESS.Count > 0)
                            {
                                var dataMediTypes = BackendDataWorker.Get<HIS_MEDICINE_TYPE>().Where(p => p.IS_BUSINESS != 1).ToList();
                                if (dataMediTypes != null && dataMediTypes.Count > 0)
                                {
                                    lstMediInStocks = lstMediInStocks.Where(p => dataMediTypes.Select(o => o.ID).Distinct().ToList().Contains(p.MEDICINE_TYPE_ID)).ToList();
                                }
                                else
                                {
                                    lstMediInStocks = new List<HisMedicineInStockSDO>();
                                }
                            }
                        }
                    }

                    List<VHisExpMestMedicineADO> ExpMestMedicineADOList = new List<VHisExpMestMedicineADO>();

                    foreach (var item in lstMediInStocks)
                    {
                        VHisExpMestMedicineADO ExpMestMedicineADO = new VHisExpMestMedicineADO();
                        AutoMapper.Mapper.CreateMap<HisMedicineInStockSDO, VHisExpMestMedicineADO>();
                        ExpMestMedicineADO = AutoMapper.Mapper.Map<VHisExpMestMedicineADO>(item);
                        ExpMestMedicineADO.PARENT_ID = "1xxx";
                        ExpMestMedicineADO.CHILD_ID = item.ID + "." + item.MEDICINE_TYPE_ID;
                        ExpMestMedicineADO.EXPIRED_DATE_STR = item.EXPIRED_DATE.HasValue ? Inventec.Common.DateTime.Convert.TimeNumberToDateString((item.EXPIRED_DATE ?? 0).ToString()) : "";
                        ExpMestMedicineADOList.Add(ExpMestMedicineADO);
                    }

                    // add parent
                    VHisExpMestMedicineADO parent = new VHisExpMestMedicineADO();
                    parent.PARENT_ID = "";
                    parent.CHILD_ID = "1xxx";
                    parent.MEDICINE_TYPE_CODE = "Kiểm kê kho";
                    ExpMestMedicineADOList.Add(parent);
                    ExpMestMedicineADOList = ExpMestMedicineADOList.GroupBy(x => x.CHILD_ID).Select(x => x.FirstOrDefault()).ToList();
                    BindingList<VHisExpMestMedicineADO> records = new BindingList<VHisExpMestMedicineADO>(ExpMestMedicineADOList);
                    this.treeMedicine.RefreshDataSource();
                    this.treeMedicine.DataSource = null;
                    this.treeMedicine.DataSource = records;
                    this.treeMedicine.KeyFieldName = "CHILD_ID";
                    this.treeMedicine.ParentFieldName = "PARENT_ID";
                    this.treeMedicine.ExpandAll();
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
                if (this.medistockId > 0)
                {
                    MOS.Filter.HisMaterialStockViewFilter mateFilter = new MOS.Filter.HisMaterialStockViewFilter();
                    mateFilter.MEDI_STOCK_ID = this.medistockId;

                    CommonParam param = new CommonParam();
                    var lstMateInStocks = new List<HisMaterialInStockSDO>();
                    lstMateInStocks = new BackendAdapter(param).Get<List<HisMaterialInStockSDO>>(HisRequestUriStore.HIS_MATERIAL_GETVIEW_IN_STOCK_MATERIAL_TYPE_TREE, ApiConsumers.MosConsumer, mateFilter, param);
                    if (lstMateInStocks != null && lstMateInStocks.Count > 0)
                    {
                        var dataMediStocks = BackendDataWorker.Get<V_HIS_MEDI_STOCK>().Where(p => this.medistockId == p.ID && p.IS_BUSINESS == 1).ToList();
                        if (dataMediStocks != null && dataMediStocks.Count > 0)
                        {
                            var dataMateTypes = BackendDataWorker.Get<HIS_MATERIAL_TYPE>().Where(p => p.IS_BUSINESS == 1).ToList();
                            if (dataMateTypes != null && dataMateTypes.Count > 0)
                            {
                                lstMateInStocks = lstMateInStocks.Where(p => dataMateTypes.Select(o => o.ID).Distinct().ToList().Contains(p.MATERIAL_TYPE_ID)).ToList();
                            }
                            else
                            {
                                lstMateInStocks = new List<HisMaterialInStockSDO>();
                            }
                        }
                        else
                        {
                            var dataMediStocksBUSINESS = BackendDataWorker.Get<V_HIS_MEDI_STOCK>().Where(p => this.medistockId == p.ID && p.IS_BUSINESS != 1).ToList();
                            if (dataMediStocksBUSINESS != null && dataMediStocksBUSINESS.Count > 0)
                            {
                                var dataMateTypes = BackendDataWorker.Get<HIS_MATERIAL_TYPE>().Where(p => p.IS_BUSINESS != 1).ToList();
                                if (dataMateTypes != null && dataMateTypes.Count > 0)
                                {
                                    lstMateInStocks = lstMateInStocks.Where(p => dataMateTypes.Select(o => o.ID).Distinct().ToList().Contains(p.MATERIAL_TYPE_ID)).ToList();
                                }
                                else
                                {
                                    lstMateInStocks = new List<HisMaterialInStockSDO>();
                                }
                            }
                        }
                    }

                    List<VHisExpMestMaterialADO> ExpMestMaterialADOList = new List<VHisExpMestMaterialADO>();

                    foreach (var item in lstMateInStocks)
                    {
                        VHisExpMestMaterialADO expMestMedicine = new VHisExpMestMaterialADO();
                        AutoMapper.Mapper.CreateMap<HisMaterialInStockSDO, VHisExpMestMaterialADO>();
                        expMestMedicine = AutoMapper.Mapper.Map<VHisExpMestMaterialADO>(item);
                        expMestMedicine.PARENT_ID = "1xxx";
                        expMestMedicine.CHILD_ID = item.ID + "." + item.MATERIAL_TYPE_ID;
                        expMestMedicine.EXPIRED_DATE_STR = item.EXPIRED_DATE.HasValue ? Inventec.Common.DateTime.Convert.TimeNumberToDateString((item.EXPIRED_DATE ?? 0).ToString()) : "";
                        ExpMestMaterialADOList.Add(expMestMedicine);
                    }

                    //add parent
                    VHisExpMestMaterialADO parent = new VHisExpMestMaterialADO();
                    parent.CHILD_ID = "1xxx";
                    parent.PARENT_ID = "";
                    parent.MATERIAL_TYPE_CODE = "Nhập kiểm kê";
                    ExpMestMaterialADOList.Add(parent);
                    ExpMestMaterialADOList = ExpMestMaterialADOList.GroupBy(x => x.CHILD_ID).Select(x => x.FirstOrDefault()).ToList();
                    BindingList<VHisExpMestMaterialADO> records = new BindingList<VHisExpMestMaterialADO>(ExpMestMaterialADOList);
                    this.treeMaterial.RefreshDataSource();
                    this.treeMaterial.DataSource = null;
                    this.treeMaterial.DataSource = records;
                    this.treeMaterial.KeyFieldName = "CHILD_ID";
                    this.treeMaterial.ParentFieldName = "PARENT_ID";
                    this.treeMaterial.ExpandAll();
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
                    //var listSelect = listExpMestMaterialADO.Where(o => o.IsMoba).ToList();
                    //if (listSelect != null && listSelect.Count > 0)
                    //{
                    //    totalFeePrice += listSelect.Sum(s => ((s.PRICE ?? 0) * s.MOBA_AMOUNT));
                    //    totalVatPrice += listSelect.Sum(s => ((s.PRICE ?? 0) * s.MOBA_AMOUNT * (s.VAT_RATIO ?? 0)));
                    //}
                }
                if (listExpMestMedicineADO != null && listExpMestMedicineADO.Count > 0)
                {
                    //var listSelect = listExpMestMedicineADO.Where(o => o.IsMoba).ToList();
                    //if (listSelect != null && listSelect.Count > 0)
                    //{
                    //    totalFeePrice += listSelect.Sum(s => ((s.PRICE ?? 0) * s.MOBA_AMOUNT));
                    //    totalVatPrice += listSelect.Sum(s => ((s.PRICE ?? 0) * s.MOBA_AMOUNT * (s.VAT_RATIO ?? 0)));
                    //}
                }
                totalVatPrice = Math.Round(totalVatPrice, 0);
                totalPrice = totalFeePrice + totalVatPrice;
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
                if (!btnSave.Enabled)
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
                //HisImpMestMobaDepaSDO data = new HisImpMestMobaDepaSDO();
                //data.MobaMedicines = new List<HisMobaMedicineSDO>();
                //data.MobaMaterials = new List<HisMobaMaterialSDO>();
                //data.ExpMestId = this.expMestId;
                //data.RequestRoomId = this.currentModule.RoomId;
                //var listMedicine = listExpMestMedicineADO.Where(o => o.IsMoba).ToList();
                //var listMaterial = listExpMestMaterialADO.Where(o => o.IsMoba).ToList();
                //if ((listMaterial == null || listMaterial.Count == 0) && (listMedicine == null || listMedicine.Count == 0))
                //{
                //    param.Messages.Add(Base.ResourceMessageLang.NguoiDungChuaChonThuocVatTu);
                //    return;
                //}
                //if (listMedicine != null && listMedicine.Count > 0)
                //{
                //    foreach (var item in listMedicine)
                //    {
                //        if (item.MOBA_AMOUNT <= 0)
                //        {
                //            param.Messages.Add(Base.ResourceMessageLang.SoLuongThuHoiPhaiLonHonKhong);
                //            return;
                //        }
                //        if (item.MOBA_AMOUNT > item.CAN_MOBA_AMOUNT)
                //        {
                //            param.Messages.Add(Base.ResourceMessageLang.SoLuongThuHoiKhongDuocLonHonSoLuongKhaDung);
                //            return;
                //        }
                //        HisMobaMedicineSDO medi = new HisMobaMedicineSDO();
                //        medi.MedicineId = item.MEDICINE_ID ?? 0;
                //        medi.Amount = item.MOBA_AMOUNT;
                //        data.MobaMedicines.Add(medi);
                //    }
                //}

                //if (listMaterial != null && listMaterial.Count > 0)
                //{
                //    foreach (var item in listMaterial)
                //    {
                //        if (item.MOBA_AMOUNT <= 0)
                //        {
                //            param.Messages.Add(Base.ResourceMessageLang.SoLuongThuHoiPhaiLonHonKhong);
                //            return;
                //        }
                //        if (item.MOBA_AMOUNT > item.CAN_MOBA_AMOUNT)
                //        {
                //            param.Messages.Add(Base.ResourceMessageLang.SoLuongThuHoiKhongDuocLonHonSoLuongKhaDung);
                //            return;
                //        }
                //        HisMobaMaterialSDO medi = new HisMobaMaterialSDO();
                //        medi.MaterialId = item.MATERIAL_ID ?? 0;
                //        medi.Amount = item.MOBA_AMOUNT;
                //        data.MobaMaterials.Add(medi);
                //    }
                //}
                //var rs = new Inventec.Common.Adapter.BackendAdapter(param).Post<HisImpMestResultSDO>(RequestUri.HIS_MOBA_DEPA_CREATE, ApiConsumers.MosConsumer, data, param);
                //if (rs != null)
                //{
                //    sucsess = true;
                //    resultMobaSdo = rs;
                //}
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
                MOS.EFMODEL.DataModels.HIS_EXP_MEST expMest = new HIS_EXP_MEST();
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

                MPS.Processor.Mps000214.PDO.Mps000214PDO rdo = new MPS.Processor.Mps000214.PDO.Mps000214PDO(this.resultMobaSdo.ImpMest, this.resultMobaSdo.ImpMedicines, this.resultMobaSdo.ImpMaterials, expMestMedicines, expMestMaterials, expMest);
                result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, ""));
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

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void treeMedicine_NodeCellStyle(object sender, DevExpress.XtraTreeList.GetCustomNodeCellStyleEventArgs e)
        {
            try
            {
                var data = (VHisExpMestMedicineADO)this.treeMedicine.GetDataRecordByNode(e.Node);
                if (data != null)
                {
                    if (String.IsNullOrEmpty(data.PARENT_ID))
                    {
                        e.Appearance.FontStyleDelta = System.Drawing.FontStyle.Bold;
                    }
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void treeMaterial_NodeCellStyle(object sender, DevExpress.XtraTreeList.GetCustomNodeCellStyleEventArgs e)
        {
            try
            {
                var data = (VHisExpMestMaterialADO)this.treeMaterial.GetDataRecordByNode(e.Node);
                if (data != null)
                {
                    if (String.IsNullOrEmpty(data.PARENT_ID))
                        e.Appearance.FontStyleDelta = System.Drawing.FontStyle.Bold;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

    }
}

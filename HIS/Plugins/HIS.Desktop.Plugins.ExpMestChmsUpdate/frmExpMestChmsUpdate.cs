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
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MOS.EFMODEL.DataModels;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.ViewInfo;
using Inventec.Desktop.Common.Message;
using DevExpress.XtraEditors.Controls;
using HIS.Desktop.LocalStorage.BackendData;
using MOS.Filter;
using HIS.Desktop.ApiConsumer;
using Inventec.Core;
using HIS.Desktop.Plugins.ExpMestChmsUpdate.Validation;
using MOS.SDO;
using HIS.Desktop.Plugins.ExpMestChmsUpdate.ADO;
using DevExpress.Utils.Menu;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Common.Adapter;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.ExpMestChmsUpdate.ADO;
using HIS.Desktop.Plugins.ExpMestChmsUpdate.Validation;
using System.Collections;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.Data;
using HIS.Desktop.LibraryMessage;
using Inventec.Desktop.Common.Controls.ValidationRule;
using DevExpress.XtraEditors.DXErrorProvider;

namespace HIS.Desktop.Plugins.ExpMestChmsUpdate
{
    public partial class frmExpMestChmsUpdate : HIS.Desktop.Utility.FormBase
    {

        V_HIS_MEDI_STOCK mestRoom = null;
        V_HIS_MEDI_STOCK impMestStock = null;
        List<long> materialTypeIds;
        List<long> medicineTypeIds;

        //Dictionary<long, HisMedicineTypeInStockSDO> dicExpMetyInStock = new Dictionary<long, HisMedicineTypeInStockSDO>();
        //Dictionary<long, HisMaterialTypeInStockSDO> dicExpMatyInStock = new Dictionary<long, HisMaterialTypeInStockSDO>();
        Dictionary<long, HisBloodTypeInStockSDO> dicExpBloodInStock = new Dictionary<long, HisBloodTypeInStockSDO>();

        Dictionary<long, MediMateTypeADO> dicMediMateAdo = new Dictionary<long, MediMateTypeADO>();
        MediMateTypeADO currentMediMate = null;

        HisExpMestResultSDO resultSdo = null;
        V_HIS_EXP_MEST hisExpMest;
        //HIS_EXP_MEST hisChmsExpMest;

        Inventec.Desktop.Common.Modules.Module currentModule;

        bool isUpdate = false;

        bool isSupplement = false;

        long roomId;
        long roomTypeId;

        int positionHandleControl = -1;

        //mau
        List<HisBloodTypeInStockSDO> listBloodTypeInStock { get; set; }
        List<HisMedicineInStockSDO> listMediInStock { get; set; }
        List<HisMaterialInStockSDO> listMateInStock { get; set; }

        public frmExpMestChmsUpdate(Inventec.Desktop.Common.Modules.Module currentModule, V_HIS_EXP_MEST expMest)
            : base(currentModule)
        {
            InitializeComponent();
            try
            {
                this.Text = currentModule.text;
                Base.ResourceLangManager.InitResourceLanguageManager();
                this.currentModule = currentModule;
                this.roomTypeId = currentModule.RoomTypeId;
                this.roomId = currentModule.RoomId;
                this.hisExpMest = expMest;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void frmExpMestChmsUpdate_Load(object sender, EventArgs e)
        {
            try
            {
                SetIcon();
                ResetValueControlDetail();
                WaitingManager.Show();
                LoadKeyUCLanguage();
                ValidControl();
                //LoadChmsExpMest();
                InitComboBloodABO();
                InitComboBloodRH();
                InitComboRespositoryBloodABO();
                InitComboRespositoryBloodRH();
                ValidControlMaxLength1();
                ValidControlMaxLength2();
                if (this.hisExpMest != null)
                {
                    LoadDataToComboImpMediStock();
                    LoadDataToComboExpMediStock();
                    SetDefaultValueMediStock();

                    //xuandv
                    mestRoom = BackendDataWorker.Get<V_HIS_MEDI_STOCK>().FirstOrDefault(p => p.ID == hisExpMest.MEDI_STOCK_ID);

                    FillDataToTrees();
                    GetDataExpMest(hisExpMest);
                    txtDescription.Text = hisExpMest.DESCRIPTION;
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetIcon()
        {
            try
            {
                this.Icon = Icon.ExtractAssociatedIcon(System.IO.Path.Combine(LocalStorage.Location.ApplicationStoreLocation.ApplicationDirectory, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }


        private void SetDefaultValueMediStock()
        {
            try
            {
                if (this.hisExpMest != null)
                {
                    this.cboImpMediStock.EditValue = this.hisExpMest.IMP_MEDI_STOCK_ID;
                    if (cboImpMediStock.EditValue != null)
                    {
                        var listImpMediStock = BackendDataWorker.Get<V_HIS_MEDI_STOCK>();
                        impMestStock = listImpMediStock.FirstOrDefault(o => o.ID == Convert.ToInt64(cboImpMediStock.EditValue));
                        if (impMestStock.IS_GOODS_RESTRICT == 1)
                        {
                            List<V_HIS_MEDI_STOCK_MATY> material = new List<V_HIS_MEDI_STOCK_MATY>();
                            List<V_HIS_MEDI_STOCK_METY> medicine = new List<V_HIS_MEDI_STOCK_METY>();

                            HisMediStockMatyViewFilter matyFilter = new HisMediStockMatyViewFilter();
                            matyFilter.MEDI_STOCK_ID = impMestStock.ID;
                            material = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_MEDI_STOCK_MATY>>(
                                "api/HisMediStockMaty/GetView", ApiConsumers.MosConsumer, matyFilter, null);
                            material = material.Where(o => o.IS_GOODS_RESTRICT == 1).ToList();
                            if (material != null && material.Count > 0)
                            {
                                materialTypeIds = material.Select(o => o.MATERIAL_TYPE_ID).ToList();
                            }

                            HisMediStockMetyViewFilter metyFilter = new HisMediStockMetyViewFilter();
                            metyFilter.MEDI_STOCK_ID = impMestStock.ID;
                            medicine = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_MEDI_STOCK_METY>>(
                               "api/HisMediStockMety/GetView", ApiConsumers.MosConsumer, metyFilter, null);
                            medicine = medicine.Where(o => o.IS_GOODS_RESTRICT == 1).ToList();
                            if (medicine != null && medicine.Count > 0)
                            {
                                medicineTypeIds = medicine.Select(o => o.MEDICINE_TYPE_ID).ToList();
                            }
                        }
                    }
                    this.cboExpMediStock.EditValue = this.hisExpMest.MEDI_STOCK_ID;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void GetDataExpMest(V_HIS_EXP_MEST expMest)
        {
            try
            {
                dicMediMateAdo = new Dictionary<long, ADO.MediMateTypeADO>();

                List<string> _errMessMedicine = new List<string>();
                List<string> _errMessMaterial = new List<string>();
                if (expMest.IS_REQUEST_BY_PACKAGE == 1)
                {
                    chkHienThiLo.Checked = true;

                    MOS.Filter.HisExpMestMedicineFilter _mediFilter = new HisExpMestMedicineFilter();
                    _mediFilter.EXP_MEST_ID = expMest.ID;
                    var _ExpMestMedicines = new BackendAdapter(new CommonParam()).Get<List<HIS_EXP_MEST_MEDICINE>>("api/HisExpMestMedicine/Get", ApiConsumers.MosConsumer, _mediFilter, null);
                    if (_ExpMestMedicines != null && _ExpMestMedicines.Count > 0)
                    {
                        // var dataGroups = _ExpMestMedicines.GroupBy(p => p.MEDICINE_ID).Select(p => p.ToList()).ToList();
                        foreach (var item in _ExpMestMedicines)
                        {
                            var data = this.listMediInStock.FirstOrDefault(p => p.ID == item.MEDICINE_ID);
                            if (data != null)
                            {
                                MediMateTypeADO ado = new MediMateTypeADO(data);
                                ado.EXP_AMOUNT = item.AMOUNT;
                                ado.DESCRIPTION = item.DESCRIPTION;
                                ado.NUM_ORDER = item.NUM_ORDER;
                                ado.MEDI_MATE_REQ_ID = item.ID;
                                ado.AVAILABLE_AMOUNT = data.AvailableAmount;
                                ado.MEDICINE_ID = item.MEDICINE_ID ?? 0;
                                ado.IsPackage = chkHienThiLo.Checked;
                                dicMediMateAdo[ado.SERVICE_ID] = ado;
                            }
                            else
                            {
                                var medi = BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>().FirstOrDefault(p => p.ID == item.TDL_MEDICINE_TYPE_ID);
                                _errMessMedicine.Add(medi != null ? medi.MEDICINE_TYPE_NAME : "");
                            }
                        }
                    }

                    MOS.Filter.HisExpMestMaterialFilter _mateFilter = new HisExpMestMaterialFilter();
                    _mateFilter.EXP_MEST_ID = expMest.ID;
                    var _ExpMestMaterials = new BackendAdapter(new CommonParam()).Get<List<HIS_EXP_MEST_MATERIAL>>("api/HisExpMestMaterial/Get", ApiConsumers.MosConsumer, _mateFilter, null);
                    if (_ExpMestMaterials != null && _ExpMestMaterials.Count > 0)
                    {
                        // var dataGroups = _ExpMestMaterials.GroupBy(p => p.MATERIAL_ID).Select(p => p.ToList()).ToList();
                        foreach (var item in _ExpMestMaterials)
                        {
                            var data = this.listMateInStock.FirstOrDefault(p => p.ID == item.MATERIAL_ID);
                            if (data != null)
                            {
                                MediMateTypeADO ado = new MediMateTypeADO(data);
                                ado.EXP_AMOUNT = item.AMOUNT;
                                ado.DESCRIPTION = item.DESCRIPTION;
                                ado.NUM_ORDER = item.NUM_ORDER;
                                ado.MEDI_MATE_REQ_ID = item.ID;
                                ado.AVAILABLE_AMOUNT = data.AvailableAmount;
                                ado.MATERIAL_ID = item.MATERIAL_ID ?? 0;
                                ado.IsPackage = chkHienThiLo.Checked;
                                dicMediMateAdo[ado.SERVICE_ID] = ado;
                            }
                            else
                            {
                                var mate = BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>().FirstOrDefault(p => p.ID == item.TDL_MATERIAL_TYPE_ID);
                                _errMessMaterial.Add(mate != null ? mate.MATERIAL_TYPE_NAME : "");
                            }
                        }
                    }
                }
                else
                {
                    MOS.Filter.HisExpMestMetyReqFilter metyReqFilter = new HisExpMestMetyReqFilter();
                    metyReqFilter.EXP_MEST_ID = expMest.ID;
                    var _ExpMestMetyReqs = new BackendAdapter(new CommonParam()).Get<List<HIS_EXP_MEST_METY_REQ>>("api/HisExpMestMetyReq/Get", ApiConsumers.MosConsumer, metyReqFilter, null);
                    if (_ExpMestMetyReqs != null && _ExpMestMetyReqs.Count > 0)
                    {
                        //  var dataGroups = _ExpMestMetyReqs.GroupBy(p => p.MEDICINE_TYPE_ID).Select(p => p.ToList()).ToList();
                        foreach (var item in _ExpMestMetyReqs)
                        {
                            var data = this.listMediInStock.FirstOrDefault(p => p.MEDICINE_TYPE_ID == item.MEDICINE_TYPE_ID);
                            if (data != null)
                            {
                                MediMateTypeADO ado = new MediMateTypeADO(data);
                                ado.EXP_AMOUNT = item.AMOUNT;
                                ado.DESCRIPTION = item.DESCRIPTION;
                                ado.NUM_ORDER = item.NUM_ORDER;
                                ado.MEDI_MATE_REQ_ID = item.ID;
                                ado.AVAILABLE_AMOUNT = data.AvailableAmount;
                                dicMediMateAdo[ado.SERVICE_ID] = ado;
                            }
                            else
                            {
                                var medi = BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>().FirstOrDefault(p => p.ID == item.MEDICINE_TYPE_ID);
                                _errMessMedicine.Add(medi != null ? medi.MEDICINE_TYPE_NAME : "");
                            }
                        }
                    }

                    //Material
                    MOS.Filter.HisExpMestMatyReqFilter matyReqFilter = new HisExpMestMatyReqFilter();
                    matyReqFilter.EXP_MEST_ID = expMest.ID;
                    var _ExpMestMatyReqs = new BackendAdapter(new CommonParam()).Get<List<HIS_EXP_MEST_MATY_REQ>>("api/HisExpMestMatyReq/Get", ApiConsumers.MosConsumer, matyReqFilter, null);
                    if (_ExpMestMatyReqs != null && _ExpMestMatyReqs.Count > 0)
                    {
                        // var dataGroups = _ExpMestMatyReqs.GroupBy(p => p.MATERIAL_TYPE_ID).Select(p => p.ToList()).ToList();                     
                        foreach (var item in _ExpMestMatyReqs)
                        {
                            var data = this.listMateInStock.FirstOrDefault(p => p.MATERIAL_TYPE_ID == item.MATERIAL_TYPE_ID);
                            if (data != null)
                            {
                                MediMateTypeADO ado = new MediMateTypeADO(data);
                                ado.EXP_AMOUNT = item.AMOUNT;
                                ado.DESCRIPTION = item.DESCRIPTION;
                                ado.NUM_ORDER = item.NUM_ORDER;
                                ado.MEDI_MATE_REQ_ID = item.ID;
                                ado.AVAILABLE_AMOUNT = data.AvailableAmount;
                                dicMediMateAdo[ado.SERVICE_ID] = ado;
                            }
                            else
                            {
                                var mate = BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>().FirstOrDefault(p => p.ID == item.MATERIAL_TYPE_ID);
                                _errMessMedicine.Add(mate != null ? mate.MATERIAL_TYPE_NAME : "");
                            }
                        }
                    }
                }

                List<string> _errMessBlood = new List<string>();
                //Blood
                MOS.Filter.HisExpMestBltyReqFilter bltyReqFilter = new HisExpMestBltyReqFilter();
                bltyReqFilter.EXP_MEST_ID = expMest.ID;
                var _ExpMestBltyReqs = new BackendAdapter(new CommonParam()).Get<List<HIS_EXP_MEST_BLTY_REQ>>("api/HisExpMestBltyReq/Get", ApiConsumers.MosConsumer, bltyReqFilter, null);
                if (_ExpMestBltyReqs != null && _ExpMestBltyReqs.Count > 0)
                {
                    // var dataGroups = _ExpMestBltyReqs.GroupBy(p => p.BLOOD_TYPE_ID).Select(p => p.ToList()).ToList();
                    foreach (var item in _ExpMestBltyReqs)
                    {
                        if (dicExpBloodInStock.ContainsKey(item.BLOOD_TYPE_ID))
                        {
                            var data = dicExpBloodInStock[item.BLOOD_TYPE_ID];
                            MediMateTypeADO ado = new MediMateTypeADO(data);
                            ado.EXP_AMOUNT = item.AMOUNT;
                            ado.DESCRIPTION = item.DESCRIPTION;
                            ado.NUM_ORDER = item.NUM_ORDER;
                            ado.MEDI_MATE_REQ_ID = item.ID;
                            ado.BLOOD_ABO_ID = item.BLOOD_ABO_ID;
                            ado.BLOOD_RH_ID = item.BLOOD_RH_ID;
                            ado.AVAILABLE_AMOUNT = data.Amount;//Review
                            dicMediMateAdo[ado.SERVICE_ID] = ado;
                        }
                        else
                        {
                            var blood = BackendDataWorker.Get<V_HIS_BLOOD_TYPE>().FirstOrDefault(p => p.ID == item.BLOOD_TYPE_ID);
                            _errMessBlood.Add(blood != null ? blood.BLOOD_TYPE_NAME : "");
                        }
                    }
                }
                string _messAll = "";
                if (_errMessMedicine != null && _errMessMedicine.Count > 0)
                {
                    _errMessMedicine = _errMessMedicine.Distinct().ToList();
                    _messAll = "Thuốc " + string.Join(";", _errMessMedicine) + ", ";
                }
                if (_errMessMaterial != null && _errMessMaterial.Count > 0)
                {
                    _errMessMaterial = _errMessMaterial.Distinct().ToList();
                    _messAll += "Vật tư " + string.Join(";", _errMessMaterial) + ", ";
                }
                if (_errMessBlood != null && _errMessBlood.Count > 0)
                {
                    _errMessBlood = _errMessBlood.Distinct().ToList();
                    _messAll = "Máu " + string.Join(";", _errMessBlood) + ", ";
                }

                if (!string.IsNullOrEmpty(_messAll))
                {
                    WaitingManager.Hide();
                    DevExpress.XtraEditors.XtraMessageBox.Show(_messAll + " không có trong kho xuất", "Thông báo");
                }

                gridControlExpMestChmsDetail.BeginUpdate();
                gridControlExpMestChmsDetail.DataSource = dicMediMateAdo.Select(s => s.Value).ToList();
                gridControlExpMestChmsDetail.EndUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void FillDataToTrees()
        {
            try
            {
                listMateInStock = null;
                listMediInStock = null;
                listBloodTypeInStock = null;
                if (this.hisExpMest != null)
                {
                    List<Action> methods = new List<Action>();
                    methods.Add(LoadMedicineTypeFromStock);
                    methods.Add(LoadMaterialTypeFromStock);
                    methods.Add(LoadBloodTypeFromStock);
                    Inventec.Common.ThreadCustom.ThreadCustomManager.MultipleThreadWithJoin(methods);
                }

                gridControlBloodType__BloodPage.BeginUpdate();
                gridControlBloodType__BloodPage.DataSource = listBloodTypeInStock;
                gridControlBloodType__BloodPage.EndUpdate();

                gridControlMedicine.BeginUpdate();
                gridControlMedicine.DataSource = listMediInStock;
                gridControlMedicine.EndUpdate();

                gridControlMaterial.BeginUpdate();
                gridControlMaterial.DataSource = listMateInStock;
                gridControlMaterial.EndUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadMedicineTypeFromStock()
        {
            try
            {
                var medicineTypeList = BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>();
                CommonParam param = new CommonParam();
                HisMedicineStockViewFilter medicineFilter = new HisMedicineStockViewFilter();
                medicineFilter.MEDI_STOCK_ID = this.hisExpMest.MEDI_STOCK_ID;
                medicineFilter.IS_LEAF = 1;
                medicineFilter.MEDICINE_TYPE_IS_ACTIVE = true;
                medicineFilter.ORDER_DIRECTION = "ASC";
                medicineFilter.ORDER_FIELD = "MEDICINE_TYPE_NAME";

                this.listMediInStock = new List<HisMedicineInStockSDO>();
                List<HisMedicineInStockSDO> _datas = new BackendAdapter(param).Get<List<HisMedicineInStockSDO>>("/api/HisMedicine/GetInStockMedicine", ApiConsumers.MosConsumer, medicineFilter, param);

                if (_datas != null && _datas.Count > 0 && this.hisExpMest.IS_REQUEST_BY_PACKAGE != 1)
                {
                    var dataGroups = _datas.GroupBy(p => p.MEDICINE_TYPE_ID).Select(p => p.ToList()).ToList();
                    foreach (var item in dataGroups)
                    {
                        if (item.Sum(p => p.AvailableAmount) > 0
                       && IS_GOODS_RESTRICT(item.FirstOrDefault())
                       && IsCheckMedicine(
                       medicineTypeList,
                       this.mestRoom.IS_BUSINESS == 1 ? true : false,
                       item.FirstOrDefault().MEDICINE_TYPE_ID))
                        {
                            HisMedicineInStockSDO ado = new HisMedicineInStockSDO();
                            ado = item.FirstOrDefault();
                            ado.AvailableAmount = item.Sum(p => p.AvailableAmount);
                            ado.TotalAmount = item.Sum(p => p.TotalAmount);
                            ado.EXPIRED_DATE = null;
                            ado.PACKAGE_NUMBER = null;
                            ado.ID = 0;
                            this.listMediInStock.Add(ado);
                        }
                    }
                }
                else
                {
                    _datas = _datas.Where(o =>
                       o.AvailableAmount > 0
                       && IS_GOODS_RESTRICT(o)
                       && IsCheckMedicine(medicineTypeList, this.mestRoom.IS_BUSINESS == 1 ? true : false, o.MEDICINE_TYPE_ID)
                       ).ToList();
                    this.listMediInStock = _datas;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private bool IS_GOODS_RESTRICT(HisMedicineInStockSDO data)
        {
            bool result = false;
            try
            {
                if (this.impMestStock.IS_GOODS_RESTRICT != null
                    && this.impMestStock.IS_GOODS_RESTRICT != 0
                    && medicineTypeIds != null
                    && medicineTypeIds.Count > 0)
                {
                    if (data != null
                    && medicineTypeIds.Equals(data.MEDICINE_TYPE_ID))
                    {
                        result = true;
                    }
                }
                else result = true;
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private bool IsCheckMedicine(List<V_HIS_MEDICINE_TYPE> _medicineTypes, bool isBusiness, long medicineTypeId)
        {
            bool result = false;
            try
            {
                var data = _medicineTypes.FirstOrDefault(p => p.ID == medicineTypeId);
                if (data != null)// && data.IS_REUSABLE != 1)
                {
                    if (isBusiness)
                    {
                        if (data.IS_BUSINESS == 1)
                            result = true;
                    }
                    else
                    {
                        if (data.IS_BUSINESS != 1)
                            result = true;
                    }
                }
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private void LoadMaterialTypeFromStock()
        {
            try
            {
                var materialTypeList = BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>();
                HisMaterialStockViewFilter mateFilter = new HisMaterialStockViewFilter();
                mateFilter.MEDI_STOCK_ID = this.hisExpMest.MEDI_STOCK_ID;
                mateFilter.MATERIAL_TYPE_IS_ACTIVE = true;
                mateFilter.IS_LEAF = 1;
                mateFilter.ORDER_DIRECTION = "ASC";
                mateFilter.ORDER_FIELD = "MATERIAL_TYPE_NAME";

                CommonParam param = new CommonParam();
                this.listMateInStock = new List<HisMaterialInStockSDO>();
                List<HisMaterialInStockSDO> _datas = new BackendAdapter(param).Get<List<HisMaterialInStockSDO>>("/api/HisMaterial/GetInStockMaterial", ApiConsumers.MosConsumer, mateFilter, param);

                if (_datas != null && _datas.Count > 0 && this.hisExpMest.IS_REQUEST_BY_PACKAGE != 1)
                {
                    var dataGroups = _datas.GroupBy(p => p.MATERIAL_TYPE_ID).Select(p => p.ToList()).ToList();
                    foreach (var item in dataGroups)
                    {
                        if (item.Sum(p => p.AvailableAmount) > 0
                       && IS_GOODS_RESTRICT(item.FirstOrDefault())
                       && IsCheckMaterial(
                       materialTypeList,
                       this.mestRoom.IS_BUSINESS == 1 ? true : false,
                       item.FirstOrDefault().MATERIAL_TYPE_ID))
                        {
                            HisMaterialInStockSDO ado = new HisMaterialInStockSDO();
                            ado = item.FirstOrDefault();
                            ado.AvailableAmount = item.Sum(p => p.AvailableAmount);
                            ado.TotalAmount = item.Sum(p => p.TotalAmount);
                            ado.EXPIRED_DATE = null;
                            ado.PACKAGE_NUMBER = null;
                            ado.ID = 0;
                            this.listMateInStock.Add(ado);
                        }
                    }
                }
                else
                {
                    _datas = _datas.Where(o =>
                       o.AvailableAmount > 0
                       && IS_GOODS_RESTRICT(o)
                       && IsCheckMaterial(materialTypeList, this.mestRoom.IS_BUSINESS == 1 ? true : false, o.MATERIAL_TYPE_ID)
                       ).ToList();
                    this.listMateInStock = _datas;
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadBloodTypeFromStock()
        {
            try
            {
                //Mau
                listBloodTypeInStock = new List<HisBloodTypeInStockSDO>();
                HisBloodTypeStockViewFilter bltyFilter = new HisBloodTypeStockViewFilter();
                bltyFilter.MEDI_STOCK_ID = this.hisExpMest.MEDI_STOCK_ID;
                bltyFilter.IS_LEAF = true;
                bltyFilter.IS_AVAILABLE = true;
                bltyFilter.IS_ACTIVE = 1;
                listBloodTypeInStock = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<HisBloodTypeInStockSDO>>("api/HisBloodType/GetInStockBloodType", ApiConsumers.MosConsumer, bltyFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private bool IS_GOODS_RESTRICT(HisMaterialInStockSDO data)
        {
            bool result = false;
            try
            {
                if (this.impMestStock.IS_GOODS_RESTRICT != null
                    && this.impMestStock.IS_GOODS_RESTRICT != 0
                    && materialTypeIds != null
                    && materialTypeIds.Count > 0)
                {
                    if (data != null
                    && materialTypeIds.Equals(data.MATERIAL_TYPE_ID))
                    {
                        result = true;
                    }
                }
                else result = true;
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private bool IsCheckMaterial(List<V_HIS_MATERIAL_TYPE> _materialTypes, bool isBusiness, long materialTypeId)
        {
            bool result = false;
            try
            {
                var data = _materialTypes.FirstOrDefault(p => p.ID == materialTypeId);
                if (data != null)// && data.IS_REUSABLE != 1)
                {
                    if (isBusiness)
                    {
                        if (data.IS_BUSINESS == 1)
                            result = true;
                    }
                    else
                    {
                        if (data.IS_BUSINESS != 1)
                            result = true;
                    }
                }
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private void ResetValueControlDetail()
        {
            try
            {
                spinExpAmount.Value = 0;
                txtNote.Text = "";
                SetEnableButton(false);
                if (this.currentMediMate != null && !isSupplement)
                {
                    btnAddd.Enabled = true;
                    spinExpAmount.Focus();
                    spinExpAmount.SelectAll();
                }
                else
                {
                    btnAddd.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ResetGridControlDetail()
        {
            try
            {
                dicMediMateAdo.Clear();
                gridControlExpMestChmsDetail.BeginUpdate();
                gridControlExpMestChmsDetail.DataSource = null;
                gridControlExpMestChmsDetail.EndUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ResetValueControlCommon()
        {
            try
            {
                this.isSupplement = false;
                dicMediMateAdo.Clear();
                this.currentMediMate = null;
                this.resultSdo = null;
                isUpdate = false;
                ResetValueControlDetail();
                ResetGridControlDetail();
                //SetEnableCboMediStockAndButton(true);
                ddBtnPrint.Enabled = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataToComboImpMediStock()
        {
            try
            {

                cboImpMediStock.Properties.DataSource = BackendDataWorker.Get<V_HIS_MEDI_STOCK>();
                cboImpMediStock.Properties.DisplayMember = "MEDI_STOCK_NAME";
                cboImpMediStock.Properties.ValueMember = "ID";
                cboImpMediStock.Properties.ForceInitialize();
                cboImpMediStock.Properties.Columns.Clear();
                cboImpMediStock.Properties.Columns.Add(new LookUpColumnInfo("MEDI_STOCK_CODE", "", 50));
                cboImpMediStock.Properties.Columns.Add(new LookUpColumnInfo("MEDI_STOCK_NAME", "", 200));
                cboImpMediStock.Properties.ShowHeader = false;
                cboImpMediStock.Properties.ImmediatePopup = true;
                cboImpMediStock.Properties.DropDownRows = 10;
                cboImpMediStock.Properties.PopupWidth = 250;

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataToComboExpMediStock()
        {
            try
            {
                cboExpMediStock.Properties.DataSource = BackendDataWorker.Get<V_HIS_MEDI_STOCK>();
                cboExpMediStock.Properties.DisplayMember = "MEDI_STOCK_NAME";
                cboExpMediStock.Properties.ValueMember = "ID";
                cboExpMediStock.Properties.ForceInitialize();
                cboExpMediStock.Properties.Columns.Clear();
                cboExpMediStock.Properties.Columns.Add(new LookUpColumnInfo("MEDI_STOCK_CODE", "", 50));
                cboExpMediStock.Properties.Columns.Add(new LookUpColumnInfo("MEDI_STOCK_NAME", "", 200));
                cboExpMediStock.Properties.ShowHeader = false;
                cboExpMediStock.Properties.ImmediatePopup = true;
                cboExpMediStock.Properties.DropDownRows = 10;
                cboExpMediStock.Properties.PopupWidth = 250;

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataToCboMediStock()
        {
            try
            {
                //listImpMediStock = new List<V_HIS_MEDI_STOCK>();
                //listExpMediStock = new List<V_HIS_MEDI_STOCK>();
                //if (chkLinh.Checked)
                //{

                //    //Lúc sai cần sửa lại
                //    listExpMediStock = listCurrentMediStock;
                //    listImpMediStock = listCurrentMediStock;

                //    cboImpMediStock.EditValue = null;
                //    cboImpMediStock.Enabled = false;
                //    txtImpMediStock.Enabled = false;
                //    cboExpMediStock.EditValue = null;
                //    cboExpMediStock.Enabled = true;
                //    txtExpMediStock.Enabled = true;
                //    cboImpMediStock.Properties.DataSource = listImpMediStock;
                //    cboExpMediStock.Properties.DataSource = listExpMediStock;



                //    if (currentMediStock != null)
                //    {
                //        cboExpMediStock.EditValue = currentMediStock.ID;
                //        var listMediStockId = BackendDataWorker.Get<V_HIS_MEST_ROOM>().Where(o => o.ROOM_ID == currentMediStock.ROOM_ID).Select(s => s.MEDI_STOCK_ID).ToList();
                //        if (listMediStockId != null && listMediStockId.Count > 0)
                //        {
                //            listExpMediStock = BackendDataWorker.Get<V_HIS_MEDI_STOCK>().Where(o => listMediStockId.Contains(o.ID)).ToList();
                //        }
                //    }

                //    cboImpMediStock.EditValue = hisExpMest.MEDI_STOCK_ID;
                //    LoadDataToTreeListBegin(hisExpMest);
                //    loadComboExpMediStock(hisExpMest);
                //    GetDataExpMest(hisExpMest);


                //    checkLoadMedicine = false;
                //    cboExpMediStock.Properties.DataSource = listExpMediStock;

                //}
                //else
                //{
                //    listExpMediStock = listCurrentMediStock;
                //    cboImpMediStock.EditValue = null;
                //    cboImpMediStock.Enabled = true;
                //    txtImpMediStock.Enabled = true;
                //    cboExpMediStock.EditValue = null;
                //    cboExpMediStock.Enabled = false;
                //    txtExpMediStock.Enabled = false;

                //    //Lúc sai cần sửa lại
                //    cboExpMediStock.Properties.DataSource = listExpMediStock;
                //    cboImpMediStock.Properties.DataSource = listImpMediStock;


                //    if (currentMediStock != null)
                //    {
                //        cboExpMediStock.EditValue = currentMediStock.ID;
                //        var listRoomId = BackendDataWorker.Get<V_HIS_MEST_ROOM>().Where(o => o.MEDI_STOCK_ID == currentMediStock.ID).Select(s => s.ROOM_ID).ToList();
                //        if (listRoomId != null && listRoomId.Count > 0)
                //        {
                //            listImpMediStock = BackendDataWorker.Get<V_HIS_MEDI_STOCK>().Where(o => listRoomId.Contains(o.ROOM_ID)).ToList();
                //        }
                //    }

                //    cboExpMediStock.EditValue = hisExpMest.MEDI_STOCK_ID;
                //    LoadDataToTreeListBegin(hisExpMest);
                //    loadComboImpMediStock(hisExpMest);
                //    GetDataExpMest(hisExpMest);

                //    checkLoadMaterial = false;
                //    cboImpMediStock.Properties.DataSource = listImpMediStock;

                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToGridExpMest()
        {
            try
            {
                //Review
                //List<V_HIS_EXP_MEST_MEDICINE> listMedicine = null;
                //List<V_HIS_EXP_MEST_MATERIAL> listMaterial = null;
                //if (this.resultSdo != null)
                //{
                //    listMedicine = this.resultSdo.ExpMedicines;
                //    listMaterial = this.resultSdo.ExpMaterials;
                //}
                //if (listMedicine != null && listMedicine.Count > 0)
                //{
                //    listMedicine = listMedicine.OrderBy(o => o.ID).ToList();
                //}
                //if (listMaterial != null && listMaterial.Count > 0)
                //{
                //    listMaterial = listMaterial.OrderBy(o => o.ID).ToList();
                //}
                //this.expMestMediProcessor.Reload(this.ucExpMestMedi, listMedicine);
                //this.expMestMateProcessor.Reload(this.ucExpMestMate, listMaterial);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidControl()
        {
            try
            {
                ValidControlImpMediStock();
                ValidControlExpMediStock();
                ValidControlExpAmount();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidControlImpMediStock()
        {
            try
            {
                ImpMediStockValidationRule impMestRule = new ImpMediStockValidationRule();
                impMestRule.cboImpMediStock = cboImpMediStock;
                dxValidationProvider1.SetValidationRule(cboImpMediStock, impMestRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidControlExpMediStock()
        {
            try
            {
                ExpMediStockValidationRule expMestRule = new ExpMediStockValidationRule();
                expMestRule.cboExpMediStock = cboExpMediStock;
                dxValidationProvider1.SetValidationRule(cboExpMediStock, expMestRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidControlExpAmount()
        {
            try
            {
                ExpAmountValidationRule expAmountRule = new ExpAmountValidationRule();
                expAmountRule.spinExpAmount = spinExpAmount;
                dxValidationProvider2.SetValidationRule(spinExpAmount, expAmountRule);
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

        private void dxValidationProvider2_ValidationFailed(object sender, DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventArgs e)
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

        private void LoadKeyUCLanguage()
        {
            try
            {

                var cul = Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture();
                //Button
                this.btnAddd.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_CHMS_CREATE__BTN_ADD", Base.ResourceLangManager.LanguageUCExpMestChmsCreate, cul);
                // this.btnNew.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_CHMS_CREATE__BTN_NEW", Base.ResourceLangManager.LanguageUCExpMestChmsCreate, cul);
                this.btnSave.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_CHMS_CREATE__BTN_SAVE", Base.ResourceLangManager.LanguageUCExpMestChmsCreate, cul);
                // this.btnUpdate.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_CHMS_CREATE__BTN_UPDATE", Base.ResourceLangManager.LanguageUCExpMestChmsCreate, cul);
                this.ddBtnPrint.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_CHMS_CREATE__BTN_PRINT", Base.ResourceLangManager.LanguageUCExpMestChmsCreate, cul);


                //Layout
                this.layoutDescription.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_CHMS_CREATE__LAYOUT_DESCRIPTION", Base.ResourceLangManager.LanguageUCExpMestChmsCreate, cul);
                this.layoutExpAmount.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_CHMS_CREATE__LAYOUT_EXP_AMOUNT", Base.ResourceLangManager.LanguageUCExpMestChmsCreate, cul);
                this.layoutExpMediStock.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_CHMS_CREATE__LAYOUT_EXP_MEDI_STOCK", Base.ResourceLangManager.LanguageUCExpMestChmsCreate, cul);
                this.layoutImpMediStock.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_CHMS_CREATE__LAYOUT_IMP_MEDI_STOCK", Base.ResourceLangManager.LanguageUCExpMestChmsCreate, cul);
                this.layoutNote.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_CHMS_CREATE__LAYOUT_NOTE", Base.ResourceLangManager.LanguageUCExpMestChmsCreate, cul);

                //GridControl Detail
                this.gridColumn_ExpMestChmsDetail_Amount.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_CHMS_CREATE__GRID_CONTROL__COLUMN_AMOUNT", Base.ResourceLangManager.LanguageUCExpMestChmsCreate, cul);
                this.gridColumn_ExpMestChmsDetail_ManufactureName.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_CHMS_CREATE__GRID_CONTROL__COLUMN_MANUFACTURER_NAME", Base.ResourceLangManager.LanguageUCExpMestChmsCreate, cul);
                this.gridColumn_ExpMestChmsDetail_MediMateTypeName.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_CHMS_CREATE__GRID_CONTROL__COLUMN_MEDI_MATE_TYPE_NAME", Base.ResourceLangManager.LanguageUCExpMestChmsCreate, cul);
                this.gridColumn_ExpMestChmsDetail_NationalName.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_CHMS_CREATE__GRID_CONTROL__COLUMN_NATIONAL_NAME", Base.ResourceLangManager.LanguageUCExpMestChmsCreate, cul);
                this.gridColumn_ExpMestChmsDetail_ServiceUnitName.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_CHMS_CREATE__GRID_CONTROL__COLUMN_SERVICE_UNIT_NAME", Base.ResourceLangManager.LanguageUCExpMestChmsCreate, cul);


                this.xtraTabPageMaterial.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_CHMS_CREATE__XTRA_TAB_MATERIAL", Base.ResourceLangManager.LanguageUCExpMestChmsCreate, cul);
                this.xtraTabPageMedicine.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_CHMS_CREATE__XTRA_TAB_MEDICINE", Base.ResourceLangManager.LanguageUCExpMestChmsCreate, cul);

                //Repository Button
                this.repositoryItemBtnDeleteDetail.Buttons[0].ToolTip = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_CHMS_CREATE__REPOSITORY_BTN_DELETE", Base.ResourceLangManager.LanguageUCExpMestChmsCreate, cul);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void BtnAdd()
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

        public void BtnSave()
        {
            try
            {
                gridViewExpMestChmsDetail.PostEditor();
                btnSave_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void BtnNew()
        {
            try
            {
                btnNew_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void BtnUpdate()
        {
            try
            {
                if (btnCapNhat.Enabled)
                {
                    gridViewExpMestChmsDetail.PostEditor();
                    btnCapNhat_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemBtnDeleteDetail_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                var data = (MediMateTypeADO)gridViewExpMestChmsDetail.GetFocusedRow();
                if (data != null)
                {
                    dicMediMateAdo.Remove(data.SERVICE_ID);
                }
                gridControlExpMestChmsDetail.BeginUpdate();
                gridControlExpMestChmsDetail.DataSource = dicMediMateAdo.Select(s => s.Value).ToList();
                gridControlExpMestChmsDetail.EndUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemButton_Edit_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                try
                {
                    this.currentMediMate = null;
                    var data = (ADO.MediMateTypeADO)gridViewExpMestChmsDetail.GetFocusedRow();
                    if (data != null)
                    {
                        this.currentMediMate = data;
                        spinExpAmount.Value = data.EXP_AMOUNT;
                        txtNote.Text = data.DESCRIPTION;
                        SetEnableButton(true);
                        if (data.IsMedicine)
                        {
                            xtraTabControlMain.SelectedTabPageIndex = 0;
                        }
                        else if (data.IsBlood)
                        {
                            xtraTabControlMain.SelectedTabPageIndex = 2;
                            cboChooseABO.EditValue = data.BLOOD_ABO_ID;
                            cboChooseRH.EditValue = data.BLOOD_RH_ID;
                        }
                        else
                        {
                            xtraTabControlMain.SelectedTabPageIndex = 1;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Error(ex);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewBloodType__BloodPage_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            try
            {
                var data = (HisBloodTypeInStockSDO)gridViewBloodType__BloodPage.GetFocusedRow();
                if (data != null)
                {
                    this.currentMediMate = new ADO.MediMateTypeADO(data);
                }
                ResetValueControlDetail();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboBloodRH()
        {
            try
            {
                var data = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_BLOOD_RH>();
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("BLOOD_RH_CODE", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("BLOOD_RH_CODE", "ID", columnInfos, false, 250);
                ControlEditorLoader.Load(cboChooseRH, data, controlEditorADO);
                cboChooseRH.EditValue = (data != null && data.Count > 0) ? data[0].ID : 0;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboBloodABO()
        {
            try
            {
                var data = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_BLOOD_ABO>();
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("BLOOD_ABO_CODE", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("BLOOD_ABO_CODE", "ID", columnInfos, false, 250);
                ControlEditorLoader.Load(cboChooseABO, data, controlEditorADO);
                cboChooseABO.EditValue = (data != null && data.Count > 0) ? data[0].ID : 0;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboRespositoryBloodRH()
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("BLOOD_RH_CODE", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("BLOOD_RH_CODE", "ID", columnInfos, false, 250);
                ControlEditorLoader.Load(cboRH, BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_BLOOD_RH>(), controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboRespositoryBloodABO()
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("BLOOD_ABO_CODE", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("BLOOD_ABO_CODE", "ID", columnInfos, false, 250);
                ControlEditorLoader.Load(cboABO, BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_BLOOD_ABO>(), controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtSearch_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string keyword = txtSearch.Text.Trim();
                    BindingList<HisBloodTypeInStockSDO> listResult = null;
                    if (!String.IsNullOrEmpty(keyword.Trim()))
                    {
                        List<HisBloodTypeInStockSDO> rearchResult = new List<HisBloodTypeInStockSDO>();

                        rearchResult = listBloodTypeInStock.Where(o =>
                                                        ((o.BloodTypeCode ?? "").ToString().ToLower().Contains(keyword.Trim().ToLower())
                                                        || (o.BloodTypeName ?? "").ToString().ToLower().Contains(keyword.Trim().ToLower())
                                                        || (o.Amount.ToString() ?? "").ToString().ToLower().Contains(keyword.Trim().ToLower())
                                                        || (o.Volume.ToString() ?? "").ToString().ToLower().Contains(keyword.Trim().ToLower()))
                                                        ).Distinct().ToList();

                        listResult = new BindingList<HisBloodTypeInStockSDO>(rearchResult);
                    }
                    else
                    {
                        listResult = new BindingList<HisBloodTypeInStockSDO>(listBloodTypeInStock);
                    }
                    gridControlBloodType__BloodPage.DataSource = listResult;
                    //trvService.ExpandAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnHuy_Click(object sender, EventArgs e)
        {
            try
            {
                if (!btnHuy.Enabled)
                    return;
                SetEnableButton(false);
                ResetValueControlDetail();
                xtraTabControlMain.SelectedTabPageIndex = 0;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetEnableButton(bool enable)
        {
            try
            {
                if (enable)
                {
                    btnAddd.Visible = false;
                    btnAddd.Enabled = false;
                    btnCapNhat.Enabled = true;
                    btnHuy.Enabled = true;
                    btnCapNhat.Visible = true;
                    btnHuy.Visible = true;
                }
                else
                {
                    btnAddd.Visible = true;
                    btnAddd.Enabled = true;
                    btnCapNhat.Enabled = false;
                    btnHuy.Enabled = false;
                    btnCapNhat.Visible = false;
                    btnHuy.Visible = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnCapNhat_Click(object sender, EventArgs e)
        {
            try
            {
                positionHandleControl = -1;
                if (!btnCapNhat.Enabled || !dxValidationProvider2.Validate() || this.currentMediMate == null)
                    return;
                WaitingManager.Show();

                if ((decimal?)spinExpAmount.EditValue > this.currentMediMate.AVAILABLE_AMOUNT)
                {
                    if (DevExpress.XtraEditors.XtraMessageBox.Show(
                    Base.ResourceMessageLang.SoLuongXuatLonHonSoLuongKhaDungTrongKho,
                    MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao),
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                    {
                        WaitingManager.Hide();
                        return;
                    }
                }

                this.currentMediMate.EXP_AMOUNT = spinExpAmount.Value;
                this.currentMediMate.NOTE = txtNote.Text;
                this.currentMediMate.IsPackage = chkHienThiLo.Checked;
                if (this.currentMediMate.IsMedicine)
                {
                    this.currentMediMate.ExpMedicine.Amount = spinExpAmount.Value;
                    this.currentMediMate.ExpMedicine.Description = txtNote.Text;
                }
                else if (this.currentMediMate.IsBlood)
                {
                    this.currentMediMate.ExpBlood.Amount = (long)spinExpAmount.Value;
                    this.currentMediMate.ExpBlood.Description = txtNote.Text;
                    this.currentMediMate.ExpBlood.BloodAboId = Inventec.Common.TypeConvert.Parse.ToInt64((cboChooseABO.EditValue ?? 0).ToString());
                    this.currentMediMate.ExpBlood.BloodRhId = Inventec.Common.TypeConvert.Parse.ToInt64((cboChooseRH.EditValue ?? 0).ToString());
                    this.currentMediMate.BLOOD_ABO_ID = Inventec.Common.TypeConvert.Parse.ToInt64((cboChooseABO.EditValue ?? 0).ToString());
                    this.currentMediMate.BLOOD_RH_ID = Inventec.Common.TypeConvert.Parse.ToInt64((cboChooseRH.EditValue ?? 0).ToString());
                }
                else
                {
                    this.currentMediMate.ExpMaterial.Amount = spinExpAmount.Value;
                    this.currentMediMate.ExpMaterial.Description = txtNote.Text;
                }

                //if (dicMediMateAdo.ContainsKey(this.currentMediMate.SERVICE_ID))
                //{

                //}

                dicMediMateAdo[this.currentMediMate.SERVICE_ID] = this.currentMediMate;
                gridControlExpMestChmsDetail.BeginUpdate();
                gridControlExpMestChmsDetail.DataSource = dicMediMateAdo.Select(s => s.Value).ToList();
                gridControlExpMestChmsDetail.EndUpdate();
                ResetValueControlDetail();
                SetEnableButton(false);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barCapNhat_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                BtnUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void barHuy_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnHuy_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ddBtnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                onClickPrintPhieuXuatChuyenKho(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barButtonItem_In_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                ddBtnPrint_Click(null, null);
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
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    HisMedicineInStockSDO data = (HisMedicineInStockSDO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (e.Column.FieldName == "EXPIRED_DATE_STR" && data != null && data.EXPIRED_DATE > 0)
                    {
                        e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString((long)data.EXPIRED_DATE);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewMedicine_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            try
            {
                this.currentMediMate = null;
                var row = (HisMedicineInStockSDO)gridViewMedicine.GetFocusedRow();
                if (row != null)
                {
                    this.currentMediMate = new ADO.MediMateTypeADO(row);
                    ResetValueControlDetail();
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
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    HisMaterialInStockSDO data = (HisMaterialInStockSDO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (e.Column.FieldName == "EXPIRED_DATE_STR" && data != null && data.EXPIRED_DATE > 0)
                    {
                        e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString(data.EXPIRED_DATE.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewMaterial_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            try
            {
                this.currentMediMate = null;
                var row = (HisMaterialInStockSDO)gridViewMaterial.GetFocusedRow();
                if (row != null)
                {
                    this.currentMediMate = new ADO.MediMateTypeADO(row);
                    ResetValueControlDetail();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidControlMaxLength1()
        {
            try
            {
                ControlMaxLengthValidationRule maxLengthRule = new ControlMaxLengthValidationRule();
                maxLengthRule.editor = this.txtDescription;
                maxLengthRule.maxLength = 500;
                maxLengthRule.ErrorType = ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(txtDescription, maxLengthRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidControlMaxLength2()
        {
            try
            {
                ControlMaxLengthValidationRule maxLengthRule = new ControlMaxLengthValidationRule();
                maxLengthRule.editor = txtNote;
                maxLengthRule.maxLength = 200;
                maxLengthRule.ErrorType = ErrorType.Warning;
                dxValidationProvider2.SetValidationRule(txtNote, maxLengthRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtSearchMedicine_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                string strValue = (sender as DevExpress.XtraEditors.TextEdit).Text;
                SearchMedicine(strValue);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SearchMedicine(string keyword)
        {
            try
            {
                List<HisMedicineInStockSDO> rearchResult = new List<HisMedicineInStockSDO>();
                if (!String.IsNullOrEmpty(keyword.Trim()))
                {
                    rearchResult = this.listMediInStock.Where(o =>
                                                    ((o.MEDICINE_TYPE_CODE ?? "").ToString().ToLower().Contains(keyword.Trim().ToLower())
                                                    || (o.MEDICINE_TYPE_NAME ?? "").ToString().ToLower().Contains(keyword.Trim().ToLower())
                                                    )
                                                    ).Distinct().ToList();


                }
                else
                {
                    rearchResult = this.listMediInStock.ToList();
                }
                gridControlMedicine.BeginUpdate();
                gridControlMedicine.DataSource = rearchResult;
                gridControlMedicine.EndUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SearchMaterial(string keyword)
        {
            try
            {
                List<HisMaterialInStockSDO> rearchResult = new List<HisMaterialInStockSDO>();
                if (!String.IsNullOrEmpty(keyword.Trim()))
                {
                    rearchResult = this.listMateInStock.Where(o =>
                                                    ((o.MATERIAL_TYPE_CODE ?? "").ToString().ToLower().Contains(keyword.Trim().ToLower())
                                                    || (o.MATERIAL_TYPE_NAME ?? "").ToString().ToLower().Contains(keyword.Trim().ToLower())
                                                    )
                                                    ).Distinct().ToList();

                }
                else
                {
                    rearchResult = this.listMateInStock.ToList();
                }
                gridControlMaterial.BeginUpdate();
                gridControlMaterial.DataSource = rearchResult;
                gridControlMaterial.EndUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtSearchMaterial_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                string strValue = (sender as DevExpress.XtraEditors.TextEdit).Text;
                SearchMaterial(strValue);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}

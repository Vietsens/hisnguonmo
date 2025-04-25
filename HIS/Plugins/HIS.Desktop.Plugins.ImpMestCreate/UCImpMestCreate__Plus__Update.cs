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
using DevExpress.XtraEditors.Controls;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.ConfigSystem;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.ImpMestCreate.ADO;
using HIS.Desktop.Plugins.ImpMestCreate.Base;
using HIS.Desktop.Plugins.ImpMestCreate.Save;
using HIS.Desktop.Plugins.Library.EmrGenerate;
using HIS.Desktop.Print;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.ImpMestCreate
{
    public partial class UCImpMestCreate : UserControlBase
    {
        internal HIS_IMP_MEST _currentImpMestUp { get; set; }
        List<V_HIS_MEDI_CONTRACT_MATY> TotalContractMatyForUpdate = new List<V_HIS_MEDI_CONTRACT_MATY>();
        List<V_HIS_MEDI_CONTRACT_METY> TotalContractMetyForUpdate = new List<V_HIS_MEDI_CONTRACT_METY>();

        private void LoadExpMestUpdate(long _expMestId)
        {
            try
            {
                if (_expMestId > 0)
                {
                    CommonParam param = new CommonParam();
                    MOS.Filter.HisImpMestFilter _impMestFilter = new HisImpMestFilter();
                    _impMestFilter.ID = _expMestId;

                    var dataImpMest = new BackendAdapter(param).Get<List<HIS_IMP_MEST>>("api/HisImpMest/Get", ApiConsumers.MosConsumer, _impMestFilter, param);

                    if (dataImpMest == null || dataImpMest.Count <= 0)
                        return;

                    this._currentImpMestUp = dataImpMest.FirstOrDefault();

                    ThreadLoadDataContractForUpdate();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ThreadLoadDataContractForUpdate()
        {
            Thread maty = new Thread(GetContractMatyForUpdate);
            Thread mety = new Thread(GetContractMetyForUpdate);
            try
            {
                maty.Start();
                mety.Start();
            }
            catch (Exception ex)
            {
                maty.Abort();
                mety.Abort();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void GetContractMetyForUpdate()
        {
            try
            {
                CommonParam param = new CommonParam();
                HisMediContractMatyViewFilter matyFilter = new HisMediContractMatyViewFilter();
                matyFilter.IS_ACTIVE = 1;
                TotalContractMatyForUpdate = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<V_HIS_MEDI_CONTRACT_MATY>>("api/HisMediContractMaty/GetView", ApiConsumers.MosConsumer, matyFilter, param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void GetContractMatyForUpdate()
        {
            try
            {
                CommonParam param = new CommonParam();
                HisMediContractMetyViewFilter metyFilter = new HisMediContractMetyViewFilter();
                metyFilter.IS_ACTIVE = 1;
                TotalContractMetyForUpdate = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<V_HIS_MEDI_CONTRACT_METY>>("api/HisMediContractMety/GetView", ApiConsumers.MosConsumer, metyFilter, param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetDataByExpMestUp()
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => _currentImpMestUp), _currentImpMestUp));
                if (this._currentImpMestUp != null && this._currentImpMestUp.ID > 0)
                {
                    this.resultADO = new ResultImpMestADO();
                    this.resultADO.HisInveSDO = new HisImpMestInveSDO();
                    this.resultADO.HisInveSDO.ImpMest = new HIS_IMP_MEST();
                    this.resultADO.HisInveSDO.ImpMest = this._currentImpMestUp;

                    this.resultADO.HisInitSDO = new HisImpMestInitSDO();
                    this.resultADO.HisInitSDO.ImpMest = new HIS_IMP_MEST();
                    this.resultADO.HisInitSDO.ImpMest = this._currentImpMestUp;

                    this.resultADO.HisManuSDO = new HisImpMestManuSDO();
                    this.resultADO.HisManuSDO.ImpMest = new HIS_IMP_MEST();
                    this.resultADO.HisManuSDO.ImpMest = this._currentImpMestUp;

                    this.resultADO.HisOtherSDO = new HisImpMestOtherSDO();
                    this.resultADO.HisOtherSDO.ImpMest = new HIS_IMP_MEST();
                    this.resultADO.HisOtherSDO.ImpMest = this._currentImpMestUp;

                    this.resultADO.ImpMestTypeId = this._currentImpMestUp.IMP_MEST_TYPE_ID;
                    cboImpMestType.EditValue = this._currentImpMestUp.IMP_MEST_TYPE_ID;
                    cboMediStock.EditValue = this._currentImpMestUp.MEDI_STOCK_ID;

                    this.currentImpMestType = null;
                    this.currentBid = null;

                    ColorBlack();

                    this.currentImpMestType = listImpMestType.FirstOrDefault(o => o.ID == this._currentImpMestUp.IMP_MEST_TYPE_ID);

                    SetControlEnableImMestTypeManu();

                    if (this._currentImpMestUp.SUPPLIER_ID != null)
                    {
                        var dataSupp = BackendDataWorker.Get<HIS_SUPPLIER>().FirstOrDefault(p => p.ID == this._currentImpMestUp.SUPPLIER_ID);
                        if (dataSupp != null)
                        {
                            txtNhaCC.EditValue = dataSupp.ID;
                            medicineProcessor.EnableBid(this.ucMedicineTypeTree, true);
                            materialProcessor.EnableBid(this.ucMaterialTypeTree, true);
                            this.currentSupplierForEdit = dataSupp;
                            this.currentSupplier = this.currentSupplierForEdit;

                            List<V_HIS_BID_1> bid = listBids.Where(o => o.SUPPLIER_IDS != null && ("," + o.SUPPLIER_IDS + ",").Contains("," + currentSupplier.ID + ",")).ToList();

                            this._HisBidBySuppliers = bid;

                            ProcessBidByType();

                            medicineProcessor.ReloadBid(this.ucMedicineTypeTree, bid);
                            materialProcessor.ReloadBid(this.ucMaterialTypeTree, bid);
                        }
                    }


                    txtDeliverer.Text = this._currentImpMestUp.DELIVERER;
                    txtDocumentNumber.Text = this._currentImpMestUp.DOCUMENT_NUMBER;
                    dtDocumentDate.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(this._currentImpMestUp.DOCUMENT_DATE ?? 0);
                    txtDocumentDate.Text = dtDocumentDate.Text;
                    txtkyHieuHoaDon.Text = this._currentImpMestUp.INVOICE_SYMBOL;
                    spinDocumentPrice.Value = this._currentImpMestUp.DOCUMENT_PRICE ?? 0;
                    txtTaiKhoanNo.Text = this._currentImpMestUp.DEBIT_ACCOUNT;
                    txtTaiKhoanCo.Text = this._currentImpMestUp.CREDIT_ACCOUNT;
                    txtDescription.Text = this._currentImpMestUp.DESCRIPTION;
                    var receiver = BackendDataWorker.Get<ACS_USER>().Where(o => o.LOGINNAME == this._currentImpMestUp.RECEIVER_LOGINNAME).FirstOrDefault();
                    if (receiver != null)
                    {
                        cboRecieve.EditValue = receiver.ID;
                    }

                    cboImpMestType.Enabled = false;
                    cboMediStock.Enabled = false;
                    cboImpSource.Enabled = false;
                    txtNhaCC.Enabled = false;
                    checkOutBid.Enabled = false;
                    //checkInOutBid.Enabled = false;
                    txtImpMestCode.Text = this._currentImpMestUp.IMP_MEST_CODE;

                    btnNew.Enabled = false;
                    btnHoiDongKiemNhap.Enabled = true;
                    btnSaveDraft.Enabled = false;
                    dropDownButton__Print.Enabled = true;
                    isSave = true;
                    InitMenuToButtonPrint(this.resultADO);
                    LoadCurrentDataUpdate();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private async Task LoadCurrentDataUpdate()
        {
            try
            {
                CommonParam param = new CommonParam();
                HisImpMestMaterialViewFilter impMateFilter = new HisImpMestMaterialViewFilter();
                impMateFilter.IMP_MEST_ID = this._currentImpMestUp.ID;
                var impMaterials = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<V_HIS_IMP_MEST_MATERIAL>>(HisRequestUriStore.HIS_IMP_MEST_MATERIAL_GETVIEW, ApiConsumers.MosConsumer, impMateFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                #region
                if (impMaterials != null && impMaterials.Count > 0)
                {
                    List<V_HIS_BID_MATERIAL_TYPE> listBidMaterial = new List<V_HIS_BID_MATERIAL_TYPE>();
                    var checkBid = impMaterials.FirstOrDefault(o => o.BID_ID != null);
                    if (checkBid != null)
                    {
                        checkOutBid.Checked = false;
                        //InitControlStateCheckInOut();
                        HisBidMaterialTypeViewFilter bidMateFilter = new HisBidMaterialTypeViewFilter();
                        bidMateFilter.BID_IDs = impMaterials.Select(o => o.BID_ID ?? 0).ToList(); ;
                        listBidMaterial = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_BID_MATERIAL_TYPE>>(HisRequestUriStore.HIS_BID_MATERIAL_TYPE_GETVIEW, ApiConsumers.MosConsumer, bidMateFilter, null);
                    }
                    else if (this.currentImpMestType.ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__NCC || this.currentImpMestType.ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__DK)
                    {
                        checkOutBid.Checked = true;
                    }

                    HisMaterialFilter mateFilter = new HisMaterialFilter();
                    mateFilter.IDs = impMaterials.Select(p => p.MATERIAL_ID).ToList();
                    var _Materials = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<HIS_MATERIAL>>("api/HisMaterial/Get", ApiConsumers.MosConsumer, mateFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                    if (_Materials != null && _Materials.Count > 0)
                    {
                        _ImpSourceId = _Materials.FirstOrDefault().IMP_SOURCE_ID ?? 0;
                        if (_ImpSourceId > 0)
                            cboImpSource.EditValue = _ImpSourceId;
                        else
                            cboImpSource.EditValue = null;
                    }

                    impMaterials = impMaterials.OrderBy(o => o.ID).ToList();
                    foreach (var item in impMaterials)
                    {
                        var material = BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>().FirstOrDefault(o => o.ID == item.MATERIAL_TYPE_ID);
                        if (material == null) continue;
                        VHisServiceADO ado = new VHisServiceADO(material);
                        ado.IMP_PRICE_PREVIOUS = (material.LAST_IMP_PRICE ?? 0) * (1 + (material.LAST_IMP_VAT_RATIO ?? 0));
                        ado.GiaBan = (material.LAST_EXP_PRICE ?? 0) * (1 + material.LAST_EXP_VAT_RATIO ?? 0);
                        ado.EXPIRED_DATE = item.EXPIRED_DATE;
                        if (item.TDL_IMP_UNIT_ID.HasValue)
                        {
                            ado.IMP_AMOUNT = item.IMP_UNIT_AMOUNT ?? 0;
                            ado.IMP_PRICE = item.IMP_UNIT_PRICE ?? 0;
                        }
                        else
                        {
                            ado.IMP_AMOUNT = item.AMOUNT;
                            ado.IMP_PRICE = item.IMP_PRICE;
                        }
                        ado.IMP_VAT_RATIO = item.IMP_VAT_RATIO;
                        ado.ImpVatRatio = item.IMP_VAT_RATIO * 100;
                        ado.PACKAGE_NUMBER = item.PACKAGE_NUMBER;
                        ado.BidId = item.BID_ID;
                        ado.DOCUMENT_PRICE = item.DOCUMENT_PRICE;
                       
                        ado.NATIONAL_NAME = material.NATIONAL_NAME;
                        ado.CONCENTRA = material.CONCENTRA;
                        ado.MANUFACTURER_ID = material.MANUFACTURER_ID;
                        ado.SERIAL_NUMBER = item.SERIAL_NUMBER;
                        ado.MAX_REUSE_COUNT = item.MAX_REUSE_COUNT;
                        ado.PRICING_MAX_REUSE_COUNT = material.PRICING_MAX_REUSE_COUNT;
                        ado.SerialNumbers = new List<ImpMestMaterialReusableSDO>() { new ImpMestMaterialReusableSDO() { SerialNumber = this.IdentityMaterialOption != "2" ? item.SERIAL_NUMBER : null, ReusCount = item.MAX_REUSE_COUNT ?? 0 } };
                        if (item.BID_ID != null)
                        {
                            var bidtype = listBidMaterial.FirstOrDefault(o => o.BID_ID == item.BID_ID && o.MATERIAL_TYPE_ID == item.MATERIAL_TYPE_ID);
                            if (bidtype != null)
                            {
                                ado.TDL_BID_GROUP_CODE = bidtype.BID_GROUP_CODE;
                                ado.TDL_BID_NUM_ORDER = bidtype.BID_NUM_ORDER;
                                ado.TDL_BID_NUMBER = bidtype.BID_NUMBER;
                                ado.TDL_BID_YEAR = bidtype.BID_YEAR;
                                ado.TDL_BID_PACKAGE_CODE = bidtype.BID_PACKAGE_CODE;
                                ado.packingTypeName = bidtype.BID_MATERIAL_TYPE_CODE;
                                ado.heinServiceBhytName = bidtype.BID_MATERIAL_TYPE_NAME;
                                ado.monthLifespan = bidtype.MONTH_LIFESPAN;
                            }
                            else
                            {
                                var bid = listBidMaterial.FirstOrDefault(o => o.BID_ID == item.BID_ID);
                                if (bid != null)
                                {
                                    ado.TDL_BID_GROUP_CODE = bid.BID_GROUP_CODE;
                                    ado.TDL_BID_NUM_ORDER = bid.BID_NUM_ORDER;
                                    ado.TDL_BID_NUMBER = bid.BID_NUMBER;
                                    ado.TDL_BID_YEAR = bid.BID_YEAR;
                                    ado.TDL_BID_PACKAGE_CODE = bid.BID_PACKAGE_CODE;
                                    ado.monthLifespan = bid.MONTH_LIFESPAN;
                                }
                            }
                        }

                        HIS_MATERIAL mate = (_Materials != null && _Materials.Count > 0) ? _Materials.FirstOrDefault(p => p.ID == item.MATERIAL_ID) : null;
                        if (mate != null)
                        {
                            ado.HisMaterial = new HIS_MATERIAL();
                            Inventec.Common.Mapper.DataObjectMapper.Map<HIS_MATERIAL>(ado.HisMaterial, mate);
                            ado.HisMaterial.TDL_BID_GROUP_CODE = mate.TDL_BID_GROUP_CODE;
                            ado.HisMaterial.TDL_BID_NUM_ORDER = mate.TDL_BID_NUM_ORDER;
                            ado.HisMaterial.TDL_BID_NUMBER = mate.TDL_BID_NUMBER;
                            ado.HisMaterial.TDL_BID_PACKAGE_CODE = mate.TDL_BID_PACKAGE_CODE;
                            ado.HisMaterial.BID_ID = mate.BID_ID;
                            ado.HisMaterial.TDL_BID_YEAR = mate.TDL_BID_YEAR;
                            ado.HisMaterial.ID = mate.ID;
                            ado.HisMaterial.MATERIAL_TYPE_ID = mate.MATERIAL_TYPE_ID;
                            ado.HisMaterial.IS_SALE_EQUAL_IMP_PRICE = mate.IS_SALE_EQUAL_IMP_PRICE;
                            ado.HisMaterial.EXPIRED_DATE = mate.EXPIRED_DATE;
                            ado.TDL_BID_GROUP_CODE = mate.TDL_BID_GROUP_CODE;
                            ado.TDL_BID_NUM_ORDER = mate.TDL_BID_NUM_ORDER;
                            ado.TDL_BID_NUMBER = mate.TDL_BID_NUMBER;
                            ado.TDL_BID_EXTRA_CODE = mate.TDL_BID_EXTRA_CODE;
                            ado.TDL_BID_PACKAGE_CODE = mate.TDL_BID_PACKAGE_CODE;
                            ado.TT_THAU = mate.TT_THAU;
                            ado.BidId = mate.BID_ID;
                            ado.EXPIRED_DATE = mate.EXPIRED_DATE;
                            ado.NATIONAL_NAME = mate.NATIONAL_NAME;
                            ado.TDL_BID_YEAR = mate.TDL_BID_YEAR;
                            ado.CONCENTRA = mate.CONCENTRA;
                            ado.MANUFACTURER_ID = mate.MANUFACTURER_ID;
                            ado.NATIONAL_NAME = mate.NATIONAL_NAME;
                            ado.MEDICAL_CONTRACT_ID = mate.MEDICAL_CONTRACT_ID;
                            ado.CONTRACT_PRICE = mate.CONTRACT_PRICE;
                            ado.packingTypeName = mate.BID_MATERIAL_TYPE_CODE;
                            ado.heinServiceBhytName = mate.BID_MATERIAL_TYPE_NAME;
                            ado.TAX_RATIO = mate.TAX_RATIO;
                            ado.REGISTER_NUMBER = mate.MATERIAL_REGISTER_NUMBER;
                            if (mate.MEDICAL_CONTRACT_ID.HasValue && this.TotalContractMatyForUpdate != null && this.TotalContractMatyForUpdate.Count > 0)
                            {
                                var contractMaty = this.TotalContractMatyForUpdate.FirstOrDefault(o => o.BID_ID == mate.BID_ID && o.MATERIAL_TYPE_ID == mate.MATERIAL_TYPE_ID && o.MEDICAL_CONTRACT_ID == mate.MEDICAL_CONTRACT_ID);
                                if (contractMaty != null)
                                {
                                    ado.monthLifespan = contractMaty.MONTH_LIFESPAN;
                                }
                            }
                        }


                        List<HIS_MATERIAL_PATY> listMaterialPaty = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<HIS_MATERIAL_PATY>>("api/HisMaterialPaty/GetOfLast", ApiConsumers.MosConsumer, material.ID, param);

                        Dictionary<long, VHisServicePatyADO> dicPaty = new Dictionary<long, VHisServicePatyADO>();

                        foreach (var patient in BackendDataWorker.Get<HIS_PATIENT_TYPE>())
                        {
                            VHisServicePatyADO serviceAdo = new VHisServicePatyADO();
                            serviceAdo.PATIENT_TYPE_NAME = patient.PATIENT_TYPE_NAME;
                            serviceAdo.PATIENT_TYPE_ID = patient.ID;
                            serviceAdo.PATIENT_TYPE_CODE = patient.PATIENT_TYPE_CODE;
                            if (listMaterialPaty != null && listMaterialPaty.Count > 0)
                            {
                                var dataMaPaty = listMaterialPaty.Where(o => o.PATIENT_TYPE_ID == patient.ID).ToList();
                                serviceAdo.PRE_PRICE_Str = dataMaPaty != null && dataMaPaty.Count > 0 ? (dataMaPaty.FirstOrDefault().EXP_PRICE * (1 + dataMaPaty.FirstOrDefault().EXP_VAT_RATIO)) : 0;
                            }
                            serviceAdo.IsNotSell = true;
                            serviceAdo.SERVICE_TYPE_ID = material.SERVICE_TYPE_ID;
                            serviceAdo.SERVICE_ID = material.SERVICE_ID;
                            dicPaty[patient.ID] = serviceAdo;
                        }

                        HisMaterialPatyFilter matePatyFilter = new HisMaterialPatyFilter();
                        matePatyFilter.MATERIAL_ID = item.MATERIAL_ID;
                        var listServicePaty = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<HIS_MATERIAL_PATY>>("api/HisMaterialPaty/Get", ApiConsumers.MosConsumer, matePatyFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, null);//, "serviceId", material.SERVICE_ID, "treatmentTime", null);

                        if (listServicePaty != null && listServicePaty.Count > 0)
                        {
                            foreach (var service in listServicePaty)
                            {
                                if (item.TDL_IMP_UNIT_ID.HasValue)
                                {
                                    service.EXP_PRICE = service.IMP_UNIT_EXP_PRICE ?? 0;
                                }
                                if (dicPaty.ContainsKey(service.PATIENT_TYPE_ID))
                                {
                                    var pa = dicPaty[service.PATIENT_TYPE_ID];
                                    if (!pa.IsSetExpPrice)
                                    {
                                        pa.PRICE = service.EXP_PRICE * (1 + service.EXP_VAT_RATIO);
                                        pa.VAT_RATIO = service.EXP_VAT_RATIO;
                                        pa.ExpVatRatio = service.EXP_VAT_RATIO * 100;
                                        pa.ExpPriceVat = item.VIR_PRICE ?? 0;
                                        if (pa.PRICE > pa.ExpPriceVat)
                                        {
                                            pa.PercentProfit = 100 * (pa.PRICE - pa.ExpPriceVat) / pa.ExpPriceVat;
                                        }

                                        pa.IsNotSell = false;
                                        pa.IsNotEdit = true;
                                        pa.IsSetExpPrice = true;
                                        pa.ID = service.ID;
                                    }
                                }
                            }
                            ado.HisMaterialPatys = listServicePaty;
                        }

                        dicServicePaty[item.SERVICE_ID] = dicPaty.Select(s => s.Value).ToList();
                        var listData = dicServicePaty[item.SERVICE_ID];
                        ado.VHisServicePatys = listData;

                        listServiceADO.Add(ado);
                    }
                }
                #endregion
                #region
                HisImpMestMedicineViewFilter impMediFilter = new HisImpMestMedicineViewFilter();
                impMediFilter.IMP_MEST_ID = this._currentImpMestUp.ID;
                var impMedicine = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<V_HIS_IMP_MEST_MEDICINE>>(HisRequestUriStore.HIS_IMP_MEST_MEDICINE_GETVIEW, ApiConsumers.MosConsumer, impMediFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                if (impMedicine != null && impMedicine.Count > 0)
                {
                    List<V_HIS_BID_MEDICINE_TYPE> listBidMedicine = new List<V_HIS_BID_MEDICINE_TYPE>();
                    var checkBid = impMedicine.FirstOrDefault(o => o.BID_ID != null);
                    if (checkBid != null)
                    {
                        checkOutBid.Checked = false;
                        //InitControlStateCheckInOut();
                        HisBidMedicineTypeViewFilter bidMediFilter = new HisBidMedicineTypeViewFilter();
                        bidMediFilter.BID_IDs = impMedicine.Select(o => o.BID_ID ?? 0).ToList();
                        listBidMedicine = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_BID_MEDICINE_TYPE>>(HisRequestUriStore.HIS_BID_MEDICINE_TYPE_GETVIEW, ApiConsumers.MosConsumer, bidMediFilter, null);
                    }
                    else if (this.currentImpMestType.ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__NCC || this.currentImpMestType.ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__DK)
                        checkOutBid.Checked = true;

                    HisMedicineFilter mediFilter = new HisMedicineFilter();
                    mediFilter.IDs = impMedicine.Select(p => p.MEDICINE_ID).ToList();
                    var _Medicines = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<HIS_MEDICINE>>("api/HisMedicine/get", ApiConsumers.MosConsumer, mediFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                    if (_Medicines != null && _Medicines.Count > 0)
                    {
                        _ImpSourceId = _Medicines.FirstOrDefault().IMP_SOURCE_ID ?? 0;
                        if (_ImpSourceId > 0)
                            cboImpSource.EditValue = _ImpSourceId;
                        else
                            cboImpSource.EditValue = null;
                    }

                    impMedicine = impMedicine.OrderBy(o => o.ID).ToList();

                    foreach (var item in impMedicine)
                    {
                        var medicine = BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>().FirstOrDefault(o => o.ID == item.MEDICINE_TYPE_ID);
                        if (medicine == null) continue;
                        VHisServiceADO ado = new VHisServiceADO(medicine);
                        ado.TEMPERATURE = item.TEMPERATURE;
                        ado.IMP_PRICE_PREVIOUS = (medicine.LAST_IMP_PRICE ?? 0) * (1 + (medicine.LAST_IMP_VAT_RATIO ?? 0));
                        ado.GiaBan = (medicine.LAST_EXP_PRICE ?? 0) * (1 + medicine.LAST_EXP_VAT_RATIO ?? 0);
                        ado.EXPIRED_DATE = item.EXPIRED_DATE;
                        if (item.TDL_IMP_UNIT_ID.HasValue)
                        {
                            ado.IMP_AMOUNT = item.IMP_UNIT_AMOUNT ?? 0;
                            ado.IMP_PRICE = item.IMP_UNIT_PRICE ?? 0;
                        }
                        else
                        {
                            ado.IMP_AMOUNT = item.AMOUNT;
                            ado.IMP_PRICE = item.IMP_PRICE;
                        }
                        ado.IMP_VAT_RATIO = item.IMP_VAT_RATIO;
                        ado.ImpVatRatio = item.IMP_VAT_RATIO * 100;
                        ado.PACKAGE_NUMBER = item.PACKAGE_NUMBER;
                        ado.BidId = item.BID_ID;
                        ado.DOCUMENT_PRICE = item.DOCUMENT_PRICE;

                        ado.NATIONAL_NAME = medicine.NATIONAL_NAME;
                        ado.CONCENTRA = medicine.CONCENTRA;
                        ado.MANUFACTURER_ID = medicine.MANUFACTURER_ID;
                        ado.REGISTER_NUMBER = medicine.REGISTER_NUMBER;
                        ado.MEDICINE_LINE_ID = medicine.MEDICINE_LINE_ID;
                        ado.IsRequireHsd = medicine.IS_REQUIRE_HSD == 1 ? true : false;
                        if (item.CONTRACT_PRICE != null)
                        {
                            ado.CONTRACT_PRICE = item.CONTRACT_PRICE;
                        }

                        if (item.BID_ID != null)
                        {
                            var bidtype = listBidMedicine.FirstOrDefault(o => o.BID_ID == item.BID_ID && o.MEDICINE_TYPE_ID == item.MEDICINE_TYPE_ID);
                            if (bidtype != null)
                            {
                                ado.TDL_BID_GROUP_CODE = bidtype.BID_GROUP_CODE;
                                ado.TDL_BID_NUM_ORDER = bidtype.BID_NUM_ORDER;
                                ado.TDL_BID_NUMBER = bidtype.BID_NUMBER;
                                ado.TDL_BID_YEAR = bidtype.BID_YEAR;
                                ado.TDL_BID_PACKAGE_CODE = bidtype.BID_PACKAGE_CODE;
                                ado.packingTypeName = bidtype.PACKING_TYPE_NAME;
                                ado.heinServiceBhytName = bidtype.HEIN_SERVICE_BHYT_NAME;
                                ado.activeIngrBhytName = bidtype.ACTIVE_INGR_BHYT_NAME;
                                ado.HisMedicine.DOSAGE_FORM = bidtype.DOSAGE_FORM;
                                ado.medicineUseFormId = bidtype.MEDICINE_USE_FORM_ID;
                                ado.monthLifespan = bidtype.MONTH_LIFESPAN;
                            }
                            else
                            {
                                var bid = listBidMedicine.FirstOrDefault(o => o.BID_ID == item.BID_ID);
                                if (bid != null)
                                {
                                    ado.TDL_BID_GROUP_CODE = bid.BID_GROUP_CODE;
                                    ado.TDL_BID_NUM_ORDER = bid.BID_NUM_ORDER;
                                    ado.TDL_BID_NUMBER = bid.BID_NUMBER;
                                    ado.TDL_BID_YEAR = bid.BID_YEAR;
                                    ado.TDL_BID_PACKAGE_CODE = bid.BID_PACKAGE_CODE;
                                }
                            }
                        }

                        HIS_MEDICINE medi = (_Medicines != null && _Medicines.Count > 0) ? _Medicines.FirstOrDefault(p => p.ID == item.MEDICINE_ID) : null;
                        if (medi != null)
                        {
                            ado.HisMedicine = new HIS_MEDICINE();
                            Inventec.Common.Mapper.DataObjectMapper.Map<HIS_MEDICINE>(ado.HisMedicine, medi);
                            ado.HisMedicine.TDL_BID_GROUP_CODE = medi.TDL_BID_GROUP_CODE;
                            ado.HisMedicine.TDL_BID_NUM_ORDER = medi.TDL_BID_NUM_ORDER;
                            ado.HisMedicine.TDL_BID_NUMBER = medi.TDL_BID_NUMBER;
                            ado.HisMedicine.TDL_BID_PACKAGE_CODE = medi.TDL_BID_PACKAGE_CODE;
                            ado.HisMedicine.BID_ID = medi.BID_ID;
                            ado.HisMedicine.TDL_BID_YEAR = medi.TDL_BID_YEAR;
                            ado.HisMedicine.ID = medi.ID;
                            ado.HisMedicine.MEDICINE_TYPE_ID = medi.MEDICINE_TYPE_ID;
                            ado.HisMedicine.IS_SALE_EQUAL_IMP_PRICE = medi.IS_SALE_EQUAL_IMP_PRICE;
                            ado.HisMedicine.EXPIRED_DATE = medi.EXPIRED_DATE;
                            ado.TDL_BID_GROUP_CODE = medi.TDL_BID_GROUP_CODE;
                            ado.TDL_BID_NUM_ORDER = medi.TDL_BID_NUM_ORDER;
                            ado.TDL_BID_NUMBER = medi.TDL_BID_NUMBER;
                            ado.TDL_BID_EXTRA_CODE = medi.TDL_BID_EXTRA_CODE;
                            ado.TDL_BID_PACKAGE_CODE = medi.TDL_BID_PACKAGE_CODE;
                            ado.TT_THAU = medi.TT_THAU;
                            ado.BidId = medi.BID_ID;
                            ado.EXPIRED_DATE = medi.EXPIRED_DATE;
                            ado.NATIONAL_NAME = medi.NATIONAL_NAME;
                            ado.TDL_BID_YEAR = medi.TDL_BID_YEAR;
                            ado.CONCENTRA = medi.CONCENTRA;
                            ado.MANUFACTURER_ID = medi.MANUFACTURER_ID;
                            ado.REGISTER_NUMBER = medi.MEDICINE_REGISTER_NUMBER;
                            ado.NATIONAL_NAME = medi.NATIONAL_NAME;
                            ado.MEDICAL_CONTRACT_ID = medi.MEDICAL_CONTRACT_ID;
                            ado.CONTRACT_PRICE = medi.CONTRACT_PRICE;
                            ado.packingTypeName = medi.PACKING_TYPE_NAME;
                            ado.heinServiceBhytName = medi.HEIN_SERVICE_BHYT_NAME;
                            ado.activeIngrBhytName = medi.ACTIVE_INGR_BHYT_NAME;
                            ado.HisMedicine.DOSAGE_FORM = medi.DOSAGE_FORM;
                            ado.medicineUseFormId = medi.MEDICINE_USE_FORM_ID;
                            ado.TAX_RATIO = medi.TAX_RATIO;
                          
                            if (medi.MEDICAL_CONTRACT_ID.HasValue && this.TotalContractMetyForUpdate != null && this.TotalContractMetyForUpdate.Count > 0)
                            {
                                var contractMety = this.TotalContractMetyForUpdate.FirstOrDefault(o => o.BID_ID == medi.BID_ID && o.MEDICINE_TYPE_ID == medi.MEDICINE_TYPE_ID && o.MEDICAL_CONTRACT_ID == medi.MEDICAL_CONTRACT_ID);
                                if (contractMety != null)
                                {
                                    ado.monthLifespan = contractMety.MONTH_LIFESPAN;
                                }
                            }
                        }
                        List<HIS_MEDICINE_PATY> listMedcinePaty = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<HIS_MEDICINE_PATY>>("api/HisMedicinePaty/GetOfLast", ApiConsumers.MosConsumer, medicine.ID, param);
                   

                        Dictionary<long, VHisServicePatyADO> dicPaty = new Dictionary<long, VHisServicePatyADO>();

                        foreach (var patient in BackendDataWorker.Get<HIS_PATIENT_TYPE>())
                        {
                            VHisServicePatyADO serviceAdo = new VHisServicePatyADO();
                            serviceAdo.PATIENT_TYPE_NAME = patient.PATIENT_TYPE_NAME;
                            serviceAdo.PATIENT_TYPE_ID = patient.ID;
                            serviceAdo.PATIENT_TYPE_CODE = patient.PATIENT_TYPE_CODE;
                            serviceAdo.IsNotSell = true;
                            if (listMedcinePaty != null && listMedcinePaty.Count > 0)
                            {
                                var dataMaPaty = listMedcinePaty.Where(o => o.PATIENT_TYPE_ID == patient.ID).ToList();
                                serviceAdo.PRE_PRICE_Str = dataMaPaty != null && dataMaPaty.Count > 0 ? (dataMaPaty.FirstOrDefault().EXP_PRICE * (1 + dataMaPaty.FirstOrDefault().EXP_VAT_RATIO)) : 0;
                            }
                            serviceAdo.SERVICE_TYPE_ID = medicine.SERVICE_TYPE_ID;
                            serviceAdo.SERVICE_ID = medicine.SERVICE_ID;
                            dicPaty[patient.ID] = serviceAdo;
                        }

                        HisMedicinePatyFilter matePatyFilter = new HisMedicinePatyFilter();
                        matePatyFilter.MEDICINE_ID = item.MEDICINE_ID;
                        var listServicePaty = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<HIS_MEDICINE_PATY>>("api/HisMedicinePaty/Get", ApiConsumers.MosConsumer, matePatyFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, null);

                        if (listServicePaty != null && listServicePaty.Count > 0)
                        {
                            foreach (var service in listServicePaty)
                            {
                                if (item.TDL_IMP_UNIT_ID.HasValue)
                                {
                                    service.EXP_PRICE = service.IMP_UNIT_EXP_PRICE ?? 0;
                                }
                                if (dicPaty.ContainsKey(service.PATIENT_TYPE_ID))
                                {
                                    var pa = dicPaty[service.PATIENT_TYPE_ID];
                                    if (!pa.IsSetExpPrice)
                                    {
                                        pa.PRICE = service.EXP_PRICE * (1 + service.EXP_VAT_RATIO);
                                        pa.VAT_RATIO = service.EXP_VAT_RATIO;
                                        pa.ExpVatRatio = service.EXP_VAT_RATIO * 100;
                                        pa.ExpPriceVat = item.VIR_PRICE ?? 0;
                                        if (pa.PRICE > pa.ExpPriceVat)
                                        {
                                            pa.PercentProfit = 100 * (pa.PRICE - pa.ExpPriceVat) / pa.ExpPriceVat;
                                        }

                                        pa.IsNotSell = false;
                                        pa.IsNotEdit = true;
                                        pa.IsSetExpPrice = true;
                                        pa.ID = service.ID;
                                    }
                                }
                            }
                            ado.HisMedicinePatys = listServicePaty;
                        }

                        dicServicePaty[item.SERVICE_ID] = dicPaty.Select(s => s.Value).ToList();
                        var listData = dicServicePaty[item.SERVICE_ID];
                        ado.VHisServicePatys = listData;

                        listServiceADO.Add(ado);
                    }
                }
                #endregion

                listServiceADO = listServiceADO.OrderByDescending(o => o.MEDI_MATE_CODE).ToList();
                Inventec.Common.Logging.LogSystem.Debug("*_____________"+Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => listServiceADO), listServiceADO));
                gridControlImpMestDetail.BeginUpdate();
                gridControlImpMestDetail.DataSource = listServiceADO;
                gridControlImpMestDetail.EndUpdate();
                CalculTotalPrice();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ColorBlack()
        {
            layoutPackageNumber.AppearanceItemCaption.ForeColor = Color.Black;
            layoutExpiredDate.AppearanceItemCaption.ForeColor = Color.Black;
            layoutControlItem6.AppearanceItemCaption.ForeColor = Color.Black;
            lciBidNumber.AppearanceItemCaption.ForeColor = Color.Black;
            layoutControlItem8.AppearanceItemCaption.ForeColor = Color.Black;
            layoutBidNumber.AppearanceItemCaption.ForeColor = Color.Black;
            lciBid.AppearanceItemCaption.ForeColor = Color.Black;
            cboGoiThau.EditValue = null;
            cboGoiThau.Enabled = false;
        }
    }
}

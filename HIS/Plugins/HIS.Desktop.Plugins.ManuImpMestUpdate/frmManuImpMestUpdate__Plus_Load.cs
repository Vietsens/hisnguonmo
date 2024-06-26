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
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigSystem;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.ManuImpMestUpdate.ADO;
using HIS.Desktop.Plugins.ManuImpMestUpdate.Config;
using HIS.Desktop.Plugins.ManuImpMestUpdate.Resources;
using HIS.Desktop.Plugins.ManuImpMestUpdate.Validation;
using HIS.Desktop.Utility;
using HIS.UC.MaterialType;
using HIS.UC.MedicineType;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Core;
using Inventec.Desktop.Common.Controls.ValidationRule;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Resources;
using System.Threading;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.ManuImpMestUpdate
{
    public partial class frmManuImpMestUpdate : HIS.Desktop.Utility.FormBase
    {
        /// <summary>
        /// Khoi tao du lieu dau vao
        /// </summary>
        private void LoadCurrentData()
        {
            try
            {
                CommonParam param = new CommonParam();
                HisImpMestMaterialViewFilter impMateFilter = new HisImpMestMaterialViewFilter();
                impMateFilter.IMP_MEST_ID = this.impMestId;
                var impMaterials = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<V_HIS_IMP_MEST_MATERIAL>>(HisRequestUriStore.HIS_IMP_MEST_MATERIAL_GETVIEW, ApiConsumers.MosConsumer, impMateFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                if (impMaterials != null && impMaterials.Count > 0)
                {
                    var checkBid = impMaterials.FirstOrDefault(o => o.BID_ID != null);
                    if (checkBid != null)
                    {
                        checkOutBid.Checked = false;
                        listBidMaterial = new List<V_HIS_BID_MATERIAL_TYPE>();
                        HisBidMaterialTypeViewFilter bidMateFilter = new HisBidMaterialTypeViewFilter();
                        bidMateFilter.BID_IDs = impMaterials.Select(o => o.BID_ID ?? 0).ToList(); ;
                        listBidMaterial = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_BID_MATERIAL_TYPE>>(HisRequestUriStore.HIS_BID_MATERIAL_TYPE_GETVIEW, ApiConsumers.MosConsumer, bidMateFilter, null);
                    }
                    else
                    {
                        checkOutBid.Checked = true;
                    }

                    HisMaterialFilter mateFilter = new HisMaterialFilter();
                    mateFilter.IDs = impMaterials.Select(p => p.MATERIAL_ID).ToList();
                    var _Materials = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<HIS_MATERIAL>>(Base.GlobalStore.HIS_MATERIAL_GET, ApiConsumers.MosConsumer, mateFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                    if (cboImpSource.EditValue == null)
                    {
                        if (_Materials != null && _Materials.Count > 0)
                        {
                            cboImpSource.EditValue = _Materials.FirstOrDefault().IMP_SOURCE_ID;
                        }
                    }

                    impMaterials = impMaterials.OrderBy(o => o.ID).ToList();
                    foreach (var item in impMaterials)
                    {
                        var material = BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>().FirstOrDefault(o => o.ID == item.MATERIAL_TYPE_ID);
                        if (material == null) continue;
                        VHisServiceADO ado = new VHisServiceADO(material);

                        ado.EXPIRED_DATE = item.EXPIRED_DATE;
                        ado.IMP_AMOUNT = item.AMOUNT;
                        ado.IMP_PRICE = item.IMP_PRICE;
                        ado.IMP_VAT_RATIO = item.IMP_VAT_RATIO;
                        ado.ImpVatRatio = item.IMP_VAT_RATIO * 100;
                        ado.PACKAGE_NUMBER = item.PACKAGE_NUMBER;
                        ado.BidId = item.BID_ID;
                        if (item.BID_ID != null)
                        {
                            var bid = listBidMaterial.FirstOrDefault(o => o.BID_ID == item.BID_ID);
                            if (bid != null)
                            {
                                ado.BID_GROUP_CODE = bid.BID_GROUP_CODE;
                                ado.BID_NAME = bid.BID_NAME;
                                ado.BID_NUM_ORDER = bid.BID_NUM_ORDER;
                                ado.BID_NUMBER = bid.BID_NUMBER;
                                ado.BID_YEAR = bid.BID_YEAR;
                                ado.BID_PACKAGE_CODE = bid.BID_PACKAGE_CODE;
                            }
                        }

                        HIS_MATERIAL mate = (_Materials != null && _Materials.Count > 0) ? _Materials.FirstOrDefault(p => p.ID == item.MATERIAL_ID) : null;
                        if (mate != null)
                        {
                            ado.HisMaterial = new HIS_MATERIAL();
                            ado.HisMaterial.TDL_BID_GROUP_CODE = mate.TDL_BID_GROUP_CODE;
                            ado.HisMaterial.TDL_BID_NUM_ORDER = mate.TDL_BID_NUM_ORDER;
                            ado.HisMaterial.TDL_BID_NUMBER = mate.TDL_BID_NUMBER;
                            ado.HisMaterial.TDL_BID_PACKAGE_CODE = mate.TDL_BID_PACKAGE_CODE;
                            ado.HisMaterial.BID_ID = mate.BID_ID;
                            ado.HisMaterial.ID = mate.ID;
                            ado.HisMaterial.MATERIAL_TYPE_ID = mate.MATERIAL_TYPE_ID;
                            ado.HisMaterial.IS_SALE_EQUAL_IMP_PRICE = mate.IS_SALE_EQUAL_IMP_PRICE;
                            ado.BID_GROUP_CODE = mate.TDL_BID_GROUP_CODE;
                            ado.BID_NUM_ORDER = mate.TDL_BID_NUM_ORDER;
                            ado.BID_NUMBER = mate.TDL_BID_NUMBER;
                            ado.BID_PACKAGE_CODE = mate.TDL_BID_PACKAGE_CODE;
                            ado.BidId = mate.BID_ID;
                        }

                        //if (!dicServicePaty.ContainsKey(item.SERVICE_ID) || dicServicePaty[item.SERVICE_ID].Count == 0)
                        //{
                        Dictionary<long, VHisServicePatyADO> dicPaty = new Dictionary<long, VHisServicePatyADO>();

                        foreach (var patient in BackendDataWorker.Get<HIS_PATIENT_TYPE>())
                        {
                            VHisServicePatyADO serviceAdo = new VHisServicePatyADO();
                            serviceAdo.PATIENT_TYPE_NAME = patient.PATIENT_TYPE_NAME;
                            serviceAdo.PATIENT_TYPE_ID = patient.ID;
                            serviceAdo.PATIENT_TYPE_CODE = patient.PATIENT_TYPE_CODE;
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
                                if (dicPaty.ContainsKey(service.PATIENT_TYPE_ID))
                                {
                                    var pa = dicPaty[service.PATIENT_TYPE_ID];
                                    if (!pa.IsSetExpPrice)
                                    {
                                        pa.PRICE = service.EXP_PRICE;
                                        pa.VAT_RATIO = service.EXP_VAT_RATIO;
                                        pa.ExpVatRatio = service.EXP_VAT_RATIO * 100;
                                        pa.ExpPriceVat = service.EXP_PRICE * (1 + service.EXP_VAT_RATIO / 100);
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
                        //}
                        var listData = dicServicePaty[item.SERVICE_ID];
                        ado.VHisServicePatys = listData;

                        listServiceADO.Add(ado);
                    }
                }

                HisImpMestMedicineViewFilter impMediFilter = new HisImpMestMedicineViewFilter();
                impMediFilter.IMP_MEST_ID = this.impMestId;
                var impMedicine = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<V_HIS_IMP_MEST_MEDICINE>>(HisRequestUriStore.HIS_IMP_MEST_MEDICINE_GETVIEW, ApiConsumers.MosConsumer, impMediFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                if (impMedicine != null && impMedicine.Count > 0)
                {
                    var checkBid = impMedicine.FirstOrDefault(o => o.BID_ID != null);
                    if (checkBid != null)
                    {
                        checkOutBid.Checked = false;
                        HisBidMedicineTypeViewFilter bidMediFilter = new HisBidMedicineTypeViewFilter();
                        bidMediFilter.BID_IDs = impMedicine.Select(o => o.BID_ID ?? 0).ToList();
                        listBidMedicine = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_BID_MEDICINE_TYPE>>(HisRequestUriStore.HIS_BID_MEDICINE_TYPE_GETVIEW, ApiConsumers.MosConsumer, bidMediFilter, null);
                    }
                    else
                        checkOutBid.Checked = true;

                    HisMedicineFilter mediFilter = new HisMedicineFilter();
                    mediFilter.IDs = impMedicine.Select(p => p.MEDICINE_ID).ToList();
                    var _Medicines = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<HIS_MEDICINE>>(Base.GlobalStore.HIS_MEDICINE_GET, ApiConsumers.MosConsumer, mediFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                    if (cboImpSource.EditValue == null)
                    {
                        if (_Medicines != null && _Medicines.Count > 0)
                        {
                            cboImpSource.EditValue = _Medicines.FirstOrDefault().IMP_SOURCE_ID;
                        }
                    }

                    impMedicine = impMedicine.OrderBy(o => o.ID).ToList();

                    foreach (var item in impMedicine)
                    {
                        var medicine = BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>().FirstOrDefault(o => o.ID == item.MEDICINE_TYPE_ID);
                        if (medicine == null) continue;
                        VHisServiceADO ado = new VHisServiceADO(medicine);

                        ado.EXPIRED_DATE = item.EXPIRED_DATE;
                        ado.IMP_AMOUNT = item.AMOUNT;
                        ado.IMP_PRICE = item.IMP_PRICE;
                        ado.IMP_VAT_RATIO = item.IMP_VAT_RATIO;
                        ado.ImpVatRatio = item.IMP_VAT_RATIO * 100;
                        ado.PACKAGE_NUMBER = item.PACKAGE_NUMBER;
                        ado.BidId = item.BID_ID;
                        if (item.BID_ID != null)
                        {
                            var bid = listBidMedicine.FirstOrDefault(o => o.BID_ID == item.BID_ID);
                            if (bid != null)
                            {
                                ado.BID_GROUP_CODE = bid.BID_GROUP_CODE;
                                ado.BID_NAME = bid.BID_NAME;
                                ado.BID_NUM_ORDER = bid.BID_NUM_ORDER;
                                ado.BID_NUMBER = bid.BID_NUMBER;
                                ado.BID_YEAR = bid.BID_YEAR;
                                ado.BID_PACKAGE_CODE = bid.BID_PACKAGE_CODE;
                            }
                        }

                        HIS_MEDICINE medi = (_Medicines != null && _Medicines.Count > 0) ? _Medicines.FirstOrDefault(p => p.ID == item.MEDICINE_ID) : null;
                        if (medi != null)
                        {
                            ado.HisMedicine = new HIS_MEDICINE();
                            ado.HisMedicine.TDL_BID_GROUP_CODE = medi.TDL_BID_GROUP_CODE;
                            ado.HisMedicine.TDL_BID_NUM_ORDER = medi.TDL_BID_NUM_ORDER;
                            ado.HisMedicine.TDL_BID_NUMBER = medi.TDL_BID_NUMBER;
                            ado.HisMedicine.TDL_BID_PACKAGE_CODE = medi.TDL_BID_PACKAGE_CODE;
                            ado.HisMedicine.BID_ID = medi.BID_ID;
                            ado.HisMedicine.ID = medi.ID;
                            ado.HisMedicine.MEDICINE_TYPE_ID = medi.MEDICINE_TYPE_ID;
                            ado.HisMedicine.IS_SALE_EQUAL_IMP_PRICE = medi.IS_SALE_EQUAL_IMP_PRICE;
                            ado.BID_GROUP_CODE = medi.TDL_BID_GROUP_CODE;
                            ado.BID_NUM_ORDER = medi.TDL_BID_NUM_ORDER;
                            ado.BID_NUMBER = medi.TDL_BID_NUMBER;
                            ado.BID_PACKAGE_CODE = medi.TDL_BID_PACKAGE_CODE;
                            ado.BidId = medi.BID_ID;
                        }

                        //if (!dicServicePaty.ContainsKey(item.SERVICE_ID) || dicServicePaty[item.SERVICE_ID].Count == 0)
                        //{
                        Dictionary<long, VHisServicePatyADO> dicPaty = new Dictionary<long, VHisServicePatyADO>();

                        foreach (var patient in BackendDataWorker.Get<HIS_PATIENT_TYPE>())
                        {
                            VHisServicePatyADO serviceAdo = new VHisServicePatyADO();
                            serviceAdo.PATIENT_TYPE_NAME = patient.PATIENT_TYPE_NAME;
                            serviceAdo.PATIENT_TYPE_ID = patient.ID;
                            serviceAdo.PATIENT_TYPE_CODE = patient.PATIENT_TYPE_CODE;
                            serviceAdo.IsNotSell = true;
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

                                if (dicPaty.ContainsKey(service.PATIENT_TYPE_ID))
                                {
                                    var pa = dicPaty[service.PATIENT_TYPE_ID];
                                    if (!pa.IsSetExpPrice)
                                    {
                                        pa.PRICE = service.EXP_PRICE;
                                        pa.VAT_RATIO = service.EXP_VAT_RATIO;
                                        pa.ExpVatRatio = service.EXP_VAT_RATIO * 100;
                                        pa.ExpPriceVat = service.EXP_PRICE * (1 + service.EXP_VAT_RATIO / 100);
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
                        //}
                        var listData = dicServicePaty[item.SERVICE_ID];
                        ado.VHisServicePatys = listData;

                        listServiceADO.Add(ado);
                    }
                }
                listServiceADO = listServiceADO.OrderByDescending(o => o.MEDI_MATE_CODE).ToList();
                //this.resultADO = listServiceADO;
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

        private void LoadImpMestTypeAllow()
        {
            try
            {
                var listImpMestTypeId = new List<long>()
                {
                    IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__NCC, IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__KK, IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__DK,IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__KHAC
                 };

                if (HisImpMestTypeAuthorziedCFG.ImpMestType_IsAuthorized)
                {
                    CommonParam param = new CommonParam();
                    HisImpMestTypeUserFilter impMestTypeUserFilter = new HisImpMestTypeUserFilter();
                    //Review
                    //impMestTypeUserFilter.LOGINNAME = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                    var listData = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<HIS_IMP_MEST_TYPE_USER>>("api/HisImpMestTypeUser/Get", ApiConsumers.MosConsumer, impMestTypeUserFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                    if (listData != null && listData.Count > 0)
                    {
                        var listId = listData.Select(s => s.IMP_MEST_TYPE_ID).ToList();
                        listImpMestTypeId = listImpMestTypeId.Where(o => listId.Contains(o)).ToList();
                    }
                    else
                    {
                        listImpMestTypeId.Clear();
                    }
                }
                listImpMestType = BackendDataWorker.Get<HIS_IMP_MEST_TYPE>().Where(o => listImpMestTypeId.Contains(o.ID)).ToList();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadSupplier()
        {
            try
            {
                listSupplier = BackendDataWorker.Get<HIS_SUPPLIER>();
                if (listSupplierIds != null && listSupplierIds.Count > 0)
                {
                    listSupplier = listSupplier.Where(p => listSupplierIds.Contains(p.ID)).ToList();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadBid()
        {
            try
            {
                HisBidFilter hisBidFilter = new HisBidFilter();
                hisBidFilter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                listBid = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<HIS_BID>>("api/HisBid/Get", ApiConsumers.MosConsumer, hisBidFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void fillInfoBidType()
        {
            try
            {
                string BID_GROUP_CODE = "";
                string BID_NUM_ORDER = "";
                string BID_PACKAGE_CODE = "";
                if (this.currrentServiceAdo != null)
                {
                    if (this.currrentServiceAdo.IsMedicine)
                    {
                        if (listBidMedicine != null && listBidMedicine.Count > 0)
                        {
                            var bidMedi = listBidMedicine.FirstOrDefault(p => p.MEDICINE_TYPE_ID == this.currrentServiceAdo.MEDI_MATE_ID);
                            if (bidMedi != null)
                            {
                                BID_GROUP_CODE = bidMedi.BID_GROUP_CODE;
                                BID_NUM_ORDER = bidMedi.BID_NUM_ORDER;
                                BID_PACKAGE_CODE = bidMedi.BID_PACKAGE_CODE;
                            }
                        }
                    }
                    else
                    {
                        if (listBidMaterial != null && listBidMaterial.Count > 0)
                        {
                            var bidMate = listBidMaterial.FirstOrDefault(p => p.MATERIAL_TYPE_ID == this.currrentServiceAdo.MEDI_MATE_ID);
                            if (bidMate != null)
                            {
                                BID_GROUP_CODE = bidMate.BID_GROUP_CODE;
                                BID_NUM_ORDER = bidMate.BID_NUM_ORDER;
                                BID_PACKAGE_CODE = bidMate.BID_PACKAGE_CODE;
                            }
                        }
                    }
                }
                txtNhomThau.Text = BID_GROUP_CODE;
                txtSttThau.Text = BID_NUM_ORDER;
                txtGoiThau.Text = BID_PACKAGE_CODE;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}

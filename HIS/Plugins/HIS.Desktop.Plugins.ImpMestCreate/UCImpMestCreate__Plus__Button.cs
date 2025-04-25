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
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.ImpMestCreate
{
    public partial class UCImpMestCreate : UserControlBase
    {
        public const string PRINT_TYPE_CODE__BienBanNhap_MPS000170 = "Mps000199";
        public const string HIS_INIT_IMP_MEST_GETVIEW = "api/HisImpMest/GetView";
        public const string HIS_INVE_IMP_MEST_GETVIEW = "api/HisImpMest/GetView";
        public const string HIS_OTHER_IMP_MEST_GETVIEW = "api/HisImpMest/GetView";

        public HIS_IMP_MEST_TYPE impMestType { get; set; }

        private object ProccessSave(string uri, object obj, CommonParam param)
        {
            object result = null;
            try
            {
                result = new Inventec.Common.Adapter.BackendAdapter(param).Post<HisImpMestOtherSDO>(uri, ApiConsumers.MosConsumer, obj, param);
            }
            catch (Exception)
            {
                result = null;
                throw;
            }
            return result;
        }

        private void resetKeyWord()
        {
            try
            {
                medicineProcessor.ResetKeyword(this.ucMedicineTypeTree);
                materialProcessor.ResetKeyword(this.ucMaterialTypeTree);
                this.medicineProcessor.Reload(this.ucMedicineTypeTree, listMedicineType);
                this.materialProcessor.Reload(this.ucMaterialTypeTree, listMaterialType);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void UpdateExpPrice()
        {
            try
            {
                if (HIS.Desktop.Plugins.ImpMestCreate.Config.IsRoundAutoExpPriceCFG.IsRoundAutoExpPrice && listServicePatyAdo != null && listServicePatyAdo.Count() > 0)
                {
                    Parallel.ForEach(listServicePatyAdo.Where(f => f.ID > 0), l => l.PRICE
                        = Math.Round(l.PRICE, MidpointRounding.AwayFromZero));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private object getImpMestTypeSDORequest(HIS_IMP_MEST_TYPE impMestType, long impMestSttId)
        {
            try
            {
                List<HisMaterialWithPatySDO> hisMaterialSdos = new List<HisMaterialWithPatySDO>();
                List<HisMedicineWithPatySDO> hisMedicineSdos = new List<HisMedicineWithPatySDO>();
                long? impSourceId = null;
                if (cboImpSource.EditValue != null)
                {
                    var impSource = BackendDataWorker.Get<HIS_IMP_SOURCE>().FirstOrDefault(o => o.ID == Convert.ToInt64(cboImpSource.EditValue));
                    if (impSource != null)
                    {
                        impSourceId = impSource.ID;
                    }
                }
                long? supplierId = null;
                if (this.currentSupplierForEdit != null)
                {
                    supplierId = this.currentSupplierForEdit.ID;
                }

                long mediStockId = 0;
                if (cboMediStock.EditValue != null)
                {
                    var mediStock = listMediStock.FirstOrDefault(o => o.ID == Convert.ToInt64(cboMediStock.EditValue));
                    if (mediStock != null)
                    {
                        mediStockId = mediStock.ID;
                    }
                }

                foreach (var ado in listServiceADO)
                {
                    if (ado.IsMedicine)
                    {
                        HisMedicineWithPatySDO mediSdo = new HisMedicineWithPatySDO();
                        mediSdo.Medicine = ado.HisMedicine;

                        if (this.currentBid != null && CheckInBid(ado))
                        {
                            mediSdo.Medicine.BID_ID = this.currentBid.ID;
                            mediSdo.Medicine.TDL_BID_GROUP_CODE = dicBidMedicine[Base.StaticMethod.GetTypeKey(ado.MEDI_MATE_ID, ado.TDL_BID_GROUP_CODE)].BID_GROUP_CODE;
                            mediSdo.Medicine.TDL_BID_NUM_ORDER = dicBidMedicine[Base.StaticMethod.GetTypeKey(ado.MEDI_MATE_ID, ado.TDL_BID_GROUP_CODE)].BID_NUM_ORDER;
                            mediSdo.Medicine.TDL_BID_YEAR = dicBidMedicine[Base.StaticMethod.GetTypeKey(ado.MEDI_MATE_ID, ado.TDL_BID_GROUP_CODE)].BID_YEAR;
                            mediSdo.Medicine.TDL_BID_PACKAGE_CODE = dicBidMedicine[Base.StaticMethod.GetTypeKey(ado.MEDI_MATE_ID, ado.TDL_BID_GROUP_CODE)].BID_PACKAGE_CODE;
                        }
                        else
                        {
                            mediSdo.Medicine.TDL_BID_NUMBER = txtBidNumber.Text;
                            mediSdo.Medicine.TDL_BID_YEAR = txtBidYear.Text;
                            mediSdo.Medicine.TDL_BID_NUM_ORDER = txtBidNumOrder.Text;
                            mediSdo.Medicine.TDL_BID_GROUP_CODE = txtBidGroupCode.Text;
                            // mediSdo.Medicine.bid
                        }
                        mediSdo.Medicine.AMOUNT = ado.IMP_AMOUNT;
                        mediSdo.Medicine.IMP_PRICE = ado.IMP_PRICE;
                        mediSdo.Medicine.IMP_VAT_RATIO = ado.IMP_VAT_RATIO;
                        mediSdo.Medicine.IMP_SOURCE_ID = impSourceId;
                        mediSdo.Medicine.SUPPLIER_ID = supplierId;
                        mediSdo.Medicine.PACKAGE_NUMBER = ado.PACKAGE_NUMBER;
                        mediSdo.MedicinePaties = ado.HisMedicinePatys;
                        hisMedicineSdos.Add(mediSdo);
                    }
                    else
                    {
                        HisMaterialWithPatySDO mateSdo = new HisMaterialWithPatySDO();
                        mateSdo.Material = ado.HisMaterial;
                        if (this.currentBid != null && CheckInBid(ado))
                        {
                            mateSdo.Material.BID_ID = this.currentBid.ID;
                            mateSdo.Material.TDL_BID_GROUP_CODE = dicBidMaterial[Base.StaticMethod.GetTypeKey(ado.MEDI_MATE_ID, ado.TDL_BID_GROUP_CODE)].BID_GROUP_CODE;
                            mateSdo.Material.TDL_BID_NUM_ORDER = dicBidMaterial[Base.StaticMethod.GetTypeKey(ado.MEDI_MATE_ID, ado.TDL_BID_GROUP_CODE)].BID_NUM_ORDER;
                            mateSdo.Material.TDL_BID_YEAR = dicBidMaterial[Base.StaticMethod.GetTypeKey(ado.MEDI_MATE_ID, ado.TDL_BID_GROUP_CODE)].BID_YEAR;
                            mateSdo.Material.TDL_BID_PACKAGE_CODE = dicBidMaterial[Base.StaticMethod.GetTypeKey(ado.MEDI_MATE_ID, ado.TDL_BID_GROUP_CODE)].BID_PACKAGE_CODE;
                        }
                        else
                        {
                            mateSdo.Material.TDL_BID_NUMBER = txtBidNumber.Text;
                            mateSdo.Material.TDL_BID_NUM_ORDER = txtBidNumOrder.Text;
                            mateSdo.Material.TDL_BID_YEAR = txtBidYear.Text;
                            mateSdo.Material.TDL_BID_GROUP_CODE = txtBidGroupCode.Text;
                        }
                        mateSdo.Material.AMOUNT = ado.IMP_AMOUNT;
                        mateSdo.Material.MATERIAL_REGISTER_NUMBER = ado.REGISTER_NUMBER;
                        mateSdo.Material.IMP_PRICE = ado.IMP_PRICE;
                        mateSdo.Material.IMP_VAT_RATIO = ado.IMP_VAT_RATIO;
                        mateSdo.Material.IMP_SOURCE_ID = impSourceId;
                        mateSdo.Material.SUPPLIER_ID = supplierId;
                        mateSdo.Material.PACKAGE_NUMBER = ado.PACKAGE_NUMBER;
                        mateSdo.MaterialPaties = ado.HisMaterialPatys;
                        hisMaterialSdos.Add(mateSdo);
                    }
                }

                if (impMestType.ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__DK)
                {
                    HisImpMestInitSDO initSdo = new HisImpMestInitSDO();
                    if (resultADO != null)
                    {
                        initSdo = resultADO.HisInitSDO;
                    }
                    else
                    {
                        initSdo.ImpMest = new HIS_IMP_MEST();
                        //initSdo.InitImpMest = new HIS_INIT_IMP_MEST();
                    }
                    initSdo.ImpMest.REQ_ROOM_ID = this.roomId;
                    initSdo.ImpMest.MEDI_STOCK_ID = mediStockId;
                    initSdo.ImpMest.IMP_MEST_STT_ID = impMestSttId;
                    initSdo.ImpMest.IMP_MEST_TYPE_ID = impMestType.ID;
                    initSdo.ImpMest.DESCRIPTION = txtDescription.Text;
                    if (cboRecieve.EditValue != null)
                    {
                        var receiver = BackendDataWorker.Get<ACS_USER>().Where(o => o.ID == (long)cboRecieve.EditValue).FirstOrDefault();
                        if (receiver != null)
                        {
                            initSdo.ImpMest.RECEIVER_LOGINNAME = receiver.LOGINNAME;
                            initSdo.ImpMest.RECEIVER_USERNAME = receiver.USERNAME;
                        }
                    }

                    initSdo.InitMaterials = hisMaterialSdos;
                    initSdo.InitMedicines = hisMedicineSdos;
                    return initSdo;
                }
                else if (impMestType.ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__KK)
                {
                    HisImpMestInveSDO inveSdo = new HisImpMestInveSDO();
                    if (resultADO != null)
                    {
                        inveSdo = resultADO.HisInveSDO;
                    }
                    else
                    {
                        inveSdo.ImpMest = new HIS_IMP_MEST();
                        //inveSdo.InveImpMest = new HIS_INVE_IMP_MEST();
                    }
                    inveSdo.ImpMest.REQ_ROOM_ID = this.roomId;
                    inveSdo.ImpMest.MEDI_STOCK_ID = mediStockId;
                    inveSdo.ImpMest.IMP_MEST_STT_ID = impMestSttId;
                    inveSdo.ImpMest.IMP_MEST_TYPE_ID = impMestType.ID;
                    inveSdo.ImpMest.DESCRIPTION = txtDescription.Text;
                    if (cboRecieve.EditValue != null)
                    {
                        var receiver = BackendDataWorker.Get<ACS_USER>().Where(o => o.ID == (long)cboRecieve.EditValue).FirstOrDefault();
                        if (receiver != null)
                        {
                            inveSdo.ImpMest.RECEIVER_LOGINNAME = receiver.LOGINNAME;
                            inveSdo.ImpMest.RECEIVER_USERNAME = receiver.USERNAME;
                        }
                    }
                    inveSdo.InveMaterials = hisMaterialSdos;
                    inveSdo.InveMedicines = hisMedicineSdos;
                    return inveSdo;
                }
                else if (impMestType.ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__KHAC)
                {
                    HisImpMestOtherSDO otherSdo = new HisImpMestOtherSDO();
                    if (resultADO != null)
                    {
                        otherSdo = resultADO.HisOtherSDO;
                    }
                    else
                    {
                        otherSdo.ImpMest = new HIS_IMP_MEST();
                        // otherSdo.OtherImpMest = new HIS_OTHER_IMP_MEST();
                    }
                    otherSdo.ImpMest.REQ_ROOM_ID = this.roomId;
                    otherSdo.ImpMest.MEDI_STOCK_ID = mediStockId;
                    otherSdo.ImpMest.IMP_MEST_STT_ID = impMestSttId;
                    otherSdo.ImpMest.IMP_MEST_TYPE_ID = impMestType.ID;
                    otherSdo.ImpMest.DESCRIPTION = txtDescription.Text;
                    if (cboRecieve.EditValue != null)
                    {
                        var receiver = BackendDataWorker.Get<ACS_USER>().Where(o => o.ID == (long)cboRecieve.EditValue).FirstOrDefault();
                        if (receiver != null)
                        {
                            otherSdo.ImpMest.RECEIVER_LOGINNAME = receiver.LOGINNAME;
                            otherSdo.ImpMest.RECEIVER_USERNAME = receiver.USERNAME;
                        }
                    }
                    otherSdo.OtherMaterials = hisMaterialSdos;
                    otherSdo.OtherMedicines = hisMedicineSdos;
                    return otherSdo;
                }
                else if (impMestType.ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__NCC)
                {
                    HisImpMestManuSDO manuSdo = new HisImpMestManuSDO();
                    if (resultADO != null)
                    {
                        manuSdo = resultADO.HisManuSDO;
                    }
                    else
                    {
                        manuSdo.ImpMest = new HIS_IMP_MEST();
                        //manuSdo.ManuImpMest = new HIS_MANU_IMP_MEST();
                    }
                    manuSdo.ImpMest.REQ_ROOM_ID = this.roomId;
                    manuSdo.ImpMest.MEDI_STOCK_ID = mediStockId;
                    manuSdo.ImpMest.IMP_MEST_STT_ID = impMestSttId;
                    manuSdo.ImpMest.IMP_MEST_TYPE_ID = impMestType.ID;
                    manuSdo.ImpMest.DESCRIPTION = txtDescription.Text;
                    if (cboRecieve.EditValue != null)
                    {
                        var receiver = BackendDataWorker.Get<ACS_USER>().Where(o => o.ID == (long)cboRecieve.EditValue).FirstOrDefault();
                        if (receiver != null)
                        {
                            manuSdo.ImpMest.RECEIVER_LOGINNAME = receiver.LOGINNAME;
                            manuSdo.ImpMest.RECEIVER_USERNAME = receiver.USERNAME;
                        }
                    }
                    manuSdo.ImpMest.DELIVERER = txtDeliverer.Text;
                    manuSdo.ImpMest.DOCUMENT_PRICE = spinDocumentPrice.Value;
                    //manuSdo.ImpMest.DOCUMENT_PRICE = (long)Math.Round(listServiceADO.Sum(o => (o.IMP_AMOUNT * o.IMP_PRICE*(1+o.IMP_VAT_RATIO))), 0, MidpointRounding.AwayFromZero);
                    var totalPrice = listServiceADO.Sum(o => (o.IMP_AMOUNT * o.IMP_PRICE));

                    manuSdo.ImpMest.DOCUMENT_NUMBER = txtDocumentNumber.Text;
                    if (dtDocumentDate.EditValue != null && dtDocumentDate.DateTime != DateTime.MinValue)
                    {
                        manuSdo.ImpMest.DOCUMENT_DATE = Convert.ToInt64(dtDocumentDate.DateTime.ToString("yyyyMMddHHmmss"));
                    }

                    //Sửa bid
                    //if (this.currentBid != null)
                    //{
                    //    manuSdo.ImpMest.BID_ID = this.currentBid.ID;
                    //}
                    manuSdo.ImpMest.SUPPLIER_ID = supplierId.HasValue ? supplierId.Value : 0;
                    manuSdo.ManuMaterials = hisMaterialSdos;
                    manuSdo.ManuMedicines = hisMedicineSdos;
                    return manuSdo;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return null;
            }
        }

        private bool CheckOutBid(VHisServiceADO serviceADO)
        {
            bool result = true;
            try
            {
                if (this.currentBid != null && serviceADO != null)
                {
                    if (serviceADO.IsMedicine)
                    {
                        if (this._dicMedicineTypes != null
                        && this._dicMedicineTypes.ContainsKey(this.currentBid.ID))
                        {
                            var data = this._dicMedicineTypes[this.currentBid.ID].FirstOrDefault(p => p.MEDICINE_TYPE_ID == serviceADO.MEDI_MATE_ID && p.BID_GROUP_CODE == this.currrentServiceAdo.TDL_BID_GROUP_CODE);
                            if (data == null || data.ID == 0)
                                result = false;
                            else if (dicBidMedicine.ContainsKey(Base.StaticMethod.GetTypeKey(serviceADO.MEDI_MATE_ID, serviceADO.TDL_BID_GROUP_CODE)))
                                result = true;
                        }
                        else if (!dicBidMedicine.ContainsKey(Base.StaticMethod.GetTypeKey(serviceADO.MEDI_MATE_ID, serviceADO.TDL_BID_GROUP_CODE)))
                        {
                            result = false;
                        }
                    }
                    else
                    {
                        if (this._dicMaterialTypes != null
                            && this._dicMaterialTypes.ContainsKey(this.currentBid.ID))
                        {
                            var data = this._dicMaterialTypes[this.currentBid.ID].FirstOrDefault(p => p.MATERIAL_TYPE_ID == serviceADO.MEDI_MATE_ID);
                            if (data == null || data.ID == 0)
                                result = false;
                            else if (dicBidMaterial.ContainsKey(Base.StaticMethod.GetTypeKey(serviceADO.MEDI_MATE_ID, serviceADO.TDL_BID_GROUP_CODE)))
                                result = true;
                        }
                        else if (!dicBidMaterial.ContainsKey(Base.StaticMethod.GetTypeKey(serviceADO.MEDI_MATE_ID, serviceADO.TDL_BID_GROUP_CODE)))
                        {
                            result = false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = true;
            }
            return result;
        }

        private bool Check_EXPIRED_DATE(VHisServiceADO serviceADO)
        {
            bool result = false;
            try
            {
                // #15658
                if (this.currentBid != null && serviceADO != null)
                {
                    if (serviceADO.IsMedicine)
                    {
                        if (dicBidMedicine.ContainsKey(Base.StaticMethod.GetTypeKey(this.currrentServiceAdo.MEDI_MATE_ID, this.currrentServiceAdo.TDL_BID_GROUP_CODE)))
                        {
                            var data = dicBidMedicine[Base.StaticMethod.GetTypeKey(this.currrentServiceAdo.MEDI_MATE_ID, this.currrentServiceAdo.TDL_BID_GROUP_CODE)];
                            if (data != null && data.EXPIRED_DATE > 0)
                            {
                                DateTime? _EXPIRED_DATE = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(data.EXPIRED_DATE ?? 0);
                                if (_EXPIRED_DATE < DateTime.Now)
                                {
                                    result = true;
                                }
                            }
                        }
                    }
                    else
                    {
                        if (dicBidMaterial.ContainsKey(Base.StaticMethod.GetTypeKey(this.currrentServiceAdo.MEDI_MATE_ID, this.currrentServiceAdo.TDL_BID_GROUP_CODE)))
                        {
                            var data = dicBidMaterial[Base.StaticMethod.GetTypeKey(this.currrentServiceAdo.MEDI_MATE_ID, this.currrentServiceAdo.TDL_BID_GROUP_CODE)];
                            if (data != null && data.EXPIRED_DATE > 0)
                            {
                                DateTime? _EXPIRED_DATE = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(data.EXPIRED_DATE ?? 0);
                                if (_EXPIRED_DATE < DateTime.Now)
                                {
                                    result = true;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = true;
            }
            return result;
        }

        private bool CheckInBid(VHisServiceADO serviceADO)
        {
            bool result = false;
            try
            {
                if (this.currentBid != null && serviceADO != null)
                {
                    if (serviceADO.IsMedicine)
                    {
                        if (this._dicMedicineTypes != null
                            && this._dicMedicineTypes.ContainsKey(currentBid.ID))
                        {
                            var data = this._dicMedicineTypes[currentBid.ID].FirstOrDefault(p => p.MEDICINE_TYPE_ID == serviceADO.MEDI_MATE_ID && p.BID_GROUP_CODE == serviceADO.TDL_BID_GROUP_CODE);
                            if (data != null && data.ID > 0)
                                result = true;
                            else if (dicBidMedicine.ContainsKey(Base.StaticMethod.GetTypeKey(serviceADO.MEDI_MATE_ID, serviceADO.TDL_BID_GROUP_CODE)))
                                result = true;
                        }
                        else if (dicBidMedicine.ContainsKey(Base.StaticMethod.GetTypeKey(serviceADO.MEDI_MATE_ID, serviceADO.TDL_BID_GROUP_CODE)))
                        {
                            result = true;
                        }
                    }
                    else
                    {
                        if (this._dicMaterialTypes != null
                            && this._dicMaterialTypes.ContainsKey(currentBid.ID))
                        {
                            var data = this._dicMaterialTypes[currentBid.ID].FirstOrDefault(p => p.MATERIAL_TYPE_ID == serviceADO.MEDI_MATE_ID);
                            if (data != null && data.ID > 0)
                                result = true;
                            else if (dicBidMaterial.ContainsKey(Base.StaticMethod.GetTypeKey(serviceADO.MEDI_MATE_ID, serviceADO.TDL_BID_GROUP_CODE)))
                                result = true;
                        }
                        else if (dicBidMaterial.ContainsKey(Base.StaticMethod.GetTypeKey(serviceADO.MEDI_MATE_ID, serviceADO.TDL_BID_GROUP_CODE)))
                        {
                            result = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
            return result;
        }

        private void SetEnableControlCommon()
        {
            try
            {
                bool enable = (listServiceADO.Count > 0);
                cboImpMestType.Enabled = !enable;
                cboMediStock.Enabled = false;
                cboImpSource.Enabled = !enable;
                txtNhaCC.Enabled = !enable;
                medicineProcessor.EnableBid(this.ucMedicineTypeTree, enable);
                materialProcessor.EnableBid(this.ucMaterialTypeTree, enable);
                checkOutBid.Enabled = !enable;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetEnableControlCommonAdd()
        {
            try
            {
                bool enable = (listServiceADO.Count > 0);
                cboImpMestType.Enabled = !enable;
                cboMediStock.Enabled = false;
                cboImpSource.Enabled = !enable;
                txtNhaCC.Enabled = !enable;
                //medicineProcessor.EnableBid(this.ucMedicineTypeTree, enable);
                //materialProcessor.EnableBid(this.ucMaterialTypeTree, enable);
                checkOutBid.Enabled = !enable;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetEnableControlCommonNotSupplier()
        {
            try
            {
                bool enable = (listServiceADO.Count > 0);
                cboImpMestType.Enabled = !enable;
                cboMediStock.Enabled = false;
                cboImpSource.Enabled = !enable;
                //cboSupplier.Enabled = !enable;
                medicineProcessor.EnableBid(this.ucMedicineTypeTree, enable);
                materialProcessor.EnableBid(this.ucMaterialTypeTree, enable);
                checkOutBid.Enabled = !enable;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessSaveSuccess()
        {
            try
            {
                if (this.resultADO != null)
                {
                    cboImpMestType.EditValue = this.resultADO.ImpMestTypeId;
                    cboImpMestType.Enabled = false;
                    if (this.resultADO.HisMedicineSDOs != null && this.resultADO.HisMedicineSDOs.Count > 0)
                    {
                        foreach (var item in this.resultADO.HisMedicineSDOs)
                        {
                            var ado = listServiceADO.FirstOrDefault(o => o.IsMedicine && o.MEDI_MATE_ID == item.Medicine.MEDICINE_TYPE_ID && o.IMP_AMOUNT == item.Medicine.AMOUNT && o.IMP_PRICE == item.Medicine.IMP_PRICE && o.IMP_VAT_RATIO == item.Medicine.IMP_VAT_RATIO && o.PACKAGE_NUMBER == item.Medicine.PACKAGE_NUMBER && o.EXPIRED_DATE == item.Medicine.EXPIRED_DATE);
                            if (ado != null)
                            {
                                ado.HisMedicine = item.Medicine;
                                ado.HisMedicinePatys = item.MedicinePaties;
                                if (ado.HisMedicinePatys != null && ado.HisMedicinePatys.Count > 0)
                                {
                                    foreach (var mediPaty in ado.HisMedicinePatys)
                                    {
                                        var patyAdo = ado.VHisServicePatys.FirstOrDefault(o => o.PATIENT_TYPE_ID == mediPaty.PATIENT_TYPE_ID);
                                        if (patyAdo != null)
                                        {
                                            patyAdo.PRICE = mediPaty.EXP_PRICE;
                                            patyAdo.VAT_RATIO = mediPaty.EXP_VAT_RATIO;
                                            patyAdo.ExpVatRatio = mediPaty.EXP_VAT_RATIO * 100;
                                        }
                                    }
                                }
                            }
                        }
                    }

                    if (this.resultADO.HisMaterialSDOs != null && this.resultADO.HisMaterialSDOs.Count > 0)
                    {
                        foreach (var item in this.resultADO.HisMaterialSDOs)
                        {
                            var ado = listServiceADO.FirstOrDefault(o => !o.IsMedicine && o.MEDI_MATE_ID == item.Material.MATERIAL_TYPE_ID && o.IMP_AMOUNT == item.Material.AMOUNT && o.IMP_PRICE == item.Material.IMP_PRICE && o.IMP_VAT_RATIO == item.Material.IMP_VAT_RATIO && o.PACKAGE_NUMBER == item.Material.PACKAGE_NUMBER && o.EXPIRED_DATE == item.Material.EXPIRED_DATE);
                            if (ado != null)
                            {
                                ado.HisMaterial = item.Material;
                                ado.HisMaterialPatys = item.MaterialPaties;
                                if (ado.HisMaterialPatys != null && ado.HisMaterialPatys.Count > 0)
                                {
                                    foreach (var matePaty in ado.HisMaterialPatys)
                                    {
                                        var patyAdo = ado.VHisServicePatys.FirstOrDefault(o => o.PATIENT_TYPE_ID == matePaty.PATIENT_TYPE_ID);
                                        if (patyAdo != null)
                                        {
                                            patyAdo.PRICE = matePaty.EXP_PRICE;
                                            patyAdo.VAT_RATIO = matePaty.EXP_VAT_RATIO;
                                            patyAdo.ExpVatRatio = matePaty.EXP_VAT_RATIO * 100;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    gridControlImpMestDetail.BeginUpdate();
                    gridControlImpMestDetail.DataSource = listServiceADO;
                    gridControlImpMestDetail.EndUpdate();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessListImportADO(ref bool success, ref CommonParam param, List<ImportMediMateADO> listImport)
        {
            try
            {
                listServiceADO = new List<VHisServiceADO>();
                bool isCheck = false;
                foreach (var item in listImport)
                {
                    List<VHisServiceADO.Error> errors = new List<VHisServiceADO.Error>();
                    List<VHisServiceADO.Warm> warms = new List<VHisServiceADO.Warm>();
                    if (string.IsNullOrEmpty(item.MEDI_MATE_CODE))
                        continue;
                    if (item.IMP_AMOUNT == null)
                        errors.Add(HIS.Desktop.Plugins.ImpMestCreate.ADO.VHisServiceADO.Error.ThieuSoLuong);
                    else if (item.IMP_AMOUNT.Value <= 0)
                        errors.Add(HIS.Desktop.Plugins.ImpMestCreate.ADO.VHisServiceADO.Error.SaiSoLuong);
                    if (item.IMP_PRICE == null)
                        errors.Add(HIS.Desktop.Plugins.ImpMestCreate.ADO.VHisServiceADO.Error.ThieuGiaNhap);
                    else if (item.IMP_PRICE.Value < 0)
                        errors.Add(HIS.Desktop.Plugins.ImpMestCreate.ADO.VHisServiceADO.Error.SaiGiaNhap);
                    if (item.IMP_VAT_RATIO == null)
                        errors.Add(HIS.Desktop.Plugins.ImpMestCreate.ADO.VHisServiceADO.Error.ThieuVat);
                    else if (item.IMP_VAT_RATIO.Value < 0 || item.IMP_VAT_RATIO.Value > 100)
                        errors.Add(HIS.Desktop.Plugins.ImpMestCreate.ADO.VHisServiceADO.Error.SaiVat);

                    if (String.IsNullOrEmpty(item.IS_MEDICINE))
                    {
                        if (!string.IsNullOrEmpty(item.TDL_BID_PACKAGE_CODE) && Inventec.Common.String.CheckString.IsOverMaxLengthUTF8(item.TDL_BID_PACKAGE_CODE, 4))
                            errors.Add(HIS.Desktop.Plugins.ImpMestCreate.ADO.VHisServiceADO.Error.MaxLengthGoiThau);
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(item.TDL_BID_PACKAGE_CODE) && Inventec.Common.String.CheckString.IsOverMaxLengthUTF8(item.TDL_BID_PACKAGE_CODE, 2))
                            errors.Add(HIS.Desktop.Plugins.ImpMestCreate.ADO.VHisServiceADO.Error.MaxLengthGoiThau);
                    }

                    if (!string.IsNullOrEmpty(item.TDL_BID_GROUP_CODE) && Inventec.Common.String.CheckString.IsOverMaxLengthUTF8(item.TDL_BID_GROUP_CODE, 2))
                        errors.Add(HIS.Desktop.Plugins.ImpMestCreate.ADO.VHisServiceADO.Error.MaxLengthNhomThau);

                    if (!string.IsNullOrEmpty(item.NATIONAL_NAME) && Inventec.Common.String.CheckString.IsOverMaxLengthUTF8(item.NATIONAL_NAME, 100))
                        errors.Add(HIS.Desktop.Plugins.ImpMestCreate.ADO.VHisServiceADO.Error.MaxLengthQuocGia);

                    if (!string.IsNullOrEmpty(item.CONCENTRA) && Inventec.Common.String.CheckString.IsOverMaxLengthUTF8(item.CONCENTRA, 1000))
                        errors.Add(HIS.Desktop.Plugins.ImpMestCreate.ADO.VHisServiceADO.Error.MaxLenthNongDoHamLuong);

                    if (!string.IsNullOrEmpty(item.MANUFACTURER_CODE) && Inventec.Common.String.CheckString.IsOverMaxLengthUTF8(item.MANUFACTURER_CODE, 6))
                        errors.Add(HIS.Desktop.Plugins.ImpMestCreate.ADO.VHisServiceADO.Error.MaxLengthMaHangSX);

                    if (!string.IsNullOrEmpty(item.REGISTER_NUMBER) && Inventec.Common.String.CheckString.IsOverMaxLengthUTF8(item.REGISTER_NUMBER, 500))
                        errors.Add(HIS.Desktop.Plugins.ImpMestCreate.ADO.VHisServiceADO.Error.MaxLengthSoDangKy);

                    item.MEDI_MATE_CODE = item.MEDI_MATE_CODE.Trim().ToUpper();
                    VHisServiceADO ado = null;
                    V_HIS_MEDICINE_TYPE medicine = null;
                    V_HIS_MATERIAL_TYPE material = null;
                    if (String.IsNullOrEmpty(item.IS_MEDICINE))
                    {
                        material = BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>().FirstOrDefault(o => o.MATERIAL_TYPE_CODE.ToUpper() == item.MEDI_MATE_CODE);
                        if (material != null)
                        {
                            ado = new VHisServiceADO(material);
                            if (!isCheck && !CheckOutBid(ado))
                            {
                                if (DevExpress.XtraEditors.XtraMessageBox.Show(ResourceMessageManager.TonTaiThuocVatTuKhongNamTrongGoiThau, Base.ResourceMessageManager.TieuDeCuaSoThongBaoLaThongBao, System.Windows.Forms.MessageBoxButtons.YesNo) != System.Windows.Forms.DialogResult.Yes)
                                {
                                    listServiceADO = new List<VHisServiceADO>();
                                    break;
                                }
                                isCheck = true;
                            }
                            //if (Check_EXPIRED_DATE(ado))
                            //{
                            //    string _tittle = Base.ResourceMessageManager.TieuDeVatTu;

                            //    string mess = _tittle + " <color=red>" + item.MEDI_MATE_CODE + "</color> " + Base.ResourceMessageManager.TieuDeDaHetHanSuDung + "\n" + Base.ResourceMessageManager.TieuDeBanCoMuonTiepTuc;
                            //    if (DevExpress.XtraEditors.XtraMessageBox.Show(mess, Base.ResourceMessageManager.TieuDeCuaSoThongBaoLaThongBao, System.Windows.Forms.MessageBoxButtons.YesNo) != System.Windows.Forms.DialogResult.Yes)
                            //    {
                            //        listServiceADO = new List<VHisServiceADO>();
                            //        break;
                            //    }
                            //    isCheck = true;
                            //}
                            if (ado.HisMaterialPatys == null)
                            {
                                ado.HisMaterialPatys = new List<HIS_MATERIAL_PATY>();
                            }
                        }
                        else
                        {
                            param.Messages.Add(String.Format(ResourceMessageManager.KhongTimThayVatTuTheoMa, item.MEDI_MATE_CODE));
                            throw new Exception("Khong tim thay vat tu theo ma:" + item.MEDI_MATE_CODE);
                        }

                    }
                    else if (item.IS_MEDICINE.Trim().ToLower() == "x")
                    {
                        medicine = BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>().FirstOrDefault(o => o.MEDICINE_TYPE_CODE.ToUpper() == item.MEDI_MATE_CODE);
                        if (medicine != null)
                        {
                            ado = new VHisServiceADO(medicine);
                            if (!isCheck && !CheckOutBid(ado))
                            {
                                if (DevExpress.XtraEditors.XtraMessageBox.Show(ResourceMessageManager.TonTaiThuocVatTuKhongNamTrongGoiThau, Base.ResourceMessageManager.TieuDeCuaSoThongBaoLaThongBao, System.Windows.Forms.MessageBoxButtons.YesNo) != System.Windows.Forms.DialogResult.Yes)
                                {
                                    listServiceADO = new List<VHisServiceADO>();
                                    break;
                                }
                                isCheck = true;
                            }
                            //if (Check_EXPIRED_DATE(ado))
                            //{
                            //    string _tittle = Base.ResourceMessageManager.TieuDeThuoc;

                            //    string mess = _tittle + " <color=red>" + item.MEDI_MATE_CODE + "</color> " + Base.ResourceMessageManager.TieuDeDaHetHanSuDung + "\n" + Base.ResourceMessageManager.TieuDeBanCoMuonTiepTuc;
                            //    if (DevExpress.XtraEditors.XtraMessageBox.Show(mess, Base.ResourceMessageManager.TieuDeCuaSoThongBaoLaThongBao, System.Windows.Forms.MessageBoxButtons.YesNo) != System.Windows.Forms.DialogResult.Yes)
                            //    {
                            //        listServiceADO = new List<VHisServiceADO>();
                            //        break;
                            //    }
                            //    isCheck = true;
                            //}
                            if (ado.HisMedicinePatys == null)
                            {
                                ado.HisMedicinePatys = new List<HIS_MEDICINE_PATY>();
                            }
                        }
                        else
                        {
                            param.Messages.Add(String.Format(ResourceMessageManager.KhongTimThayThuocTheoMa, item.MEDI_MATE_CODE));
                            throw new Exception("Khong tim thay thuoc theo ma:" + item.MEDI_MATE_CODE);
                        }
                    }
                    else
                    {
                        param.Messages.Add(String.Format(ResourceMessageManager.KhongXacDinhDuocThuocHayVatTuMa, item.MEDI_MATE_CODE));
                        throw new Exception("Khong xac dinh duoc thuoc hay vat tu ma:" + item.MEDI_MATE_CODE);
                    }

                    if (ado != null)
                    {
                        ado.Errors = errors;
                        ado.Warms = warms;
                        if (item.IMP_AMOUNT != null)
                            ado.IMP_AMOUNT = item.IMP_AMOUNT.Value;
                        if (item.IMP_PRICE != null)
                            ado.IMP_PRICE = item.IMP_PRICE.Value;
                        if (item.IMP_VAT_RATIO != null)
                        {
                            ado.IMP_VAT_RATIO = item.IMP_VAT_RATIO.Value / 100;
                            ado.ImpVatRatio = item.IMP_VAT_RATIO.Value;
                        }

                        ado.PACKAGE_NUMBER = item.PACKAGE_NUMBER;
                        if (!String.IsNullOrEmpty(item.EXPIRED_DATE_STR))
                        {
                            DateTime? dt = null;
                            if (item.EXPIRED_DATE_STR.Length == 16)
                            {
                                dt = DateTimeHelper.ConvertDateTimeStringToSystemTime(item.EXPIRED_DATE_STR);
                                if (dt.HasValue && dt.Value != DateTime.MinValue)
                                {
                                    ado.EXPIRED_DATE = Convert.ToInt64(dt.Value.ToString("yyyyMMddHHmm") + "59");
                                }
                                else
                                {
                                    warms.Add(HIS.Desktop.Plugins.ImpMestCreate.ADO.VHisServiceADO.Warm.HanDungKhongHopLe);
                                }
                            }
                            else if (item.EXPIRED_DATE_STR.Length == 10)
                            {
                                dt = DateTimeHelper.ConvertDateStringToSystemDate(item.EXPIRED_DATE_STR);
                                if (dt.HasValue && dt.Value != DateTime.MinValue)
                                {
                                    ado.EXPIRED_DATE = Convert.ToInt64(dt.Value.ToString("yyyyMMdd") + "235959");
                                }
                                else
                                {
                                    warms.Add(HIS.Desktop.Plugins.ImpMestCreate.ADO.VHisServiceADO.Warm.HanDungKhongHopLe);
                                }
                            }
                            else
                            {
                                warms.Add(HIS.Desktop.Plugins.ImpMestCreate.ADO.VHisServiceADO.Warm.HanDungKhongHopLe);
                            }
                        }

                        ado.TDL_BID_NUM_ORDER = item.TDL_BID_NUM_ORDER;
                        ado.TDL_BID_PACKAGE_CODE = item.TDL_BID_PACKAGE_CODE;
                        ado.TDL_BID_GROUP_CODE = item.TDL_BID_GROUP_CODE;
                        ado.TDL_BID_YEAR = item.TDL_BID_YEAR;
                        ado.TDL_BID_NUMBER = item.TDL_BID_NUMBER;

                        if (!String.IsNullOrWhiteSpace(item.TDL_BID_NUMBER))
                        {
                            var bid = listBids.FirstOrDefault(o => o.BID_NUMBER.ToLower() == item.TDL_BID_NUMBER.ToLower());
                            if (bid != null)
                            {
                                ado.VALID_FROM_TIME = bid.VALID_FROM_TIME;
                                ado.VALID_TO_TIME = bid.VALID_TO_TIME;
                            }
                        }

                        if (item != null && !String.IsNullOrEmpty(item.NATIONAL_NAME))
                        {
                            ado.NATIONAL_NAME = item.NATIONAL_NAME;
                        }
                        else if (medicine != null)
                        {
                            ado.NATIONAL_NAME = medicine.NATIONAL_NAME;
                        }
                        else if (material != null)
                        {
                            ado.NATIONAL_NAME = material.NATIONAL_NAME;
                        }

                        if (item != null && !String.IsNullOrEmpty(item.CONCENTRA))
                        {
                            ado.CONCENTRA = item.CONCENTRA;
                        }
                        else if (medicine != null)
                        {
                            ado.CONCENTRA = medicine.CONCENTRA;
                        }
                        else if (material != null)
                        {
                            ado.CONCENTRA = material.CONCENTRA;
                        }

                        if(item!=null && !string.IsNullOrEmpty(item.ACTIVE_INGR_BHYT_NAME)){
                            ado.activeIngrBhytName= item.ACTIVE_INGR_BHYT_NAME;
                        }else if(medicine!=null){
                            ado.activeIngrBhytName=medicine.ACTIVE_INGR_BHYT_NAME;

                        }
                        if(item!=null && !string.IsNullOrEmpty(item.ACTIVE_INGR_BHYT_CODE)){
                            ado.activeIngrBhytCode= item.ACTIVE_INGR_BHYT_CODE;
                        }else if(medicine!=null){
                            ado.activeIngrBhytCode=medicine.ACTIVE_INGR_BHYT_CODE;
                        }

                       
                        if (item != null && !string.IsNullOrEmpty(item.MEDICINE_USE_FORM_CODE))
                        {
                            var useForm = BackendDataWorker.Get<HIS_MEDICINE_USE_FORM>().FirstOrDefault(o => o.MEDICINE_USE_FORM_CODE == item.MEDICINE_USE_FORM_CODE);
                            if(useForm !=null){
                                ado.medicineUseFormId =  useForm.ID;
                            }else{
                                ado.medicineUseFormId = null;
                                warms.Add(HIS.Desktop.Plugins.ImpMestCreate.ADO.VHisServiceADO.Warm.KhongTonTai);
                            }
                        }
                        else if (medicine != null)
                        {
                            ado.medicineUseFormId = medicine.MEDICINE_USE_FORM_ID;
                        }

                        if (item != null && !string.IsNullOrEmpty(item.HEIN_SERVICE_BHYT_NAME))
                        {
                            ado.heinServiceBhytName = item.HEIN_SERVICE_BHYT_NAME;
                        }
                        else if (medicine != null)
                        {
                            ado.heinServiceBhytName = medicine.HEIN_SERVICE_BHYT_NAME;
                        }
                        else if (material != null)
                        {
                            ado.heinServiceBhytName = material.HEIN_SERVICE_BHYT_NAME;
                        }

                        if (item != null && !string.IsNullOrEmpty(item.PACKING_TYPE_NAME))
                        {
                            ado.packingTypeName = item.PACKING_TYPE_NAME;
                        }
                        else if (medicine != null)
                        {
                            ado.packingTypeName = medicine.PACKING_TYPE_NAME;
                        }
                        else if (material != null)
                        {
                            ado.packingTypeName = material.PACKING_TYPE_NAME;
                        }

                        if (item != null && !string.IsNullOrEmpty(item.DOSAGE_FORM))
                        {
                            ado.HisMedicine.DOSAGE_FORM = item.DOSAGE_FORM;
                        }
                        else if (medicine != null)
                        {
                            ado.HisMedicine.DOSAGE_FORM = medicine.DOSAGE_FORM;
                        }

                        if (item != null && !string.IsNullOrWhiteSpace(item.MONTH_LIFESPAN_STR))
                        {
                            ado.MONTH_LIFESPAN_STR = ado.MONTH_LIFESPAN_STR;
                            long number = 0;
                            bool isValid = long.TryParse(item.MONTH_LIFESPAN_STR, out number);
                            if (isValid)
                            {
                                ado.MONTH_LIFESPAN = number;
                            }
                            else
                            {
                                ado.Warms.Add(HIS.Desktop.Plugins.ImpMestCreate.ADO.VHisServiceADO.Warm.ThangKhongHopLe);
                            }
                            if (Encoding.UTF8.GetByteCount(ado.MONTH_LIFESPAN.ToString()) > 19)
                            {
                                ado.Warms.Add(HIS.Desktop.Plugins.ImpMestCreate.ADO.VHisServiceADO.Warm.TuoiThoThangVuotQuaDoDaiChoPhep);
                            }
                        }

                        if (item != null && !String.IsNullOrWhiteSpace(item.DAY_LIFESPAN_STR))
                        {
                            ado.DAY_LIFESPAN_STR = item.DAY_LIFESPAN_STR;
                            long number = 0;
                            bool isValid = long.TryParse(item.DAY_LIFESPAN_STR, out number);
                            if (isValid)
                            {
                                ado.DAY_LIFESPAN = number;
                            }
                            else
                            {

                                ado.Warms.Add(HIS.Desktop.Plugins.ImpMestCreate.ADO.VHisServiceADO.Warm.NgayKhongHopLe);
                            }
                            if (Encoding.UTF8.GetByteCount(ado.DAY_LIFESPAN.ToString()) > 19)
                            {
                                ado.Warms.Add(HIS.Desktop.Plugins.ImpMestCreate.ADO.VHisServiceADO.Warm.TuoiThoNgayVuotQuaDoDaiChoPhep);
                            }
                        }

                        if (item != null && !String.IsNullOrWhiteSpace(item.HOUR_LIFESPAN_STR))
                        {
                            ado.HOUR_LIFESPAN_STR = item.HOUR_LIFESPAN_STR;
                            long number = 0;
                            bool isValid = long.TryParse(item.HOUR_LIFESPAN_STR, out number);
                            if (isValid)
                            {
                                ado.HOUR_LIFESPAN = number;
                            }
                            else
                            {
                                ado.Warms.Add(HIS.Desktop.Plugins.ImpMestCreate.ADO.VHisServiceADO.Warm.GioKhongHopLe);
                            }
                            if (Encoding.UTF8.GetByteCount(ado.HOUR_LIFESPAN.ToString()) > 19)
                            {
                                ado.Warms.Add(HIS.Desktop.Plugins.ImpMestCreate.ADO.VHisServiceADO.Warm.TuoiThoGioVuotQuaDoDaiChoPhep);
                            }
                        }

                        if (!String.IsNullOrWhiteSpace(ado.MONTH_LIFESPAN.ToString()) && !String.IsNullOrWhiteSpace(ado.DAY_LIFESPAN.ToString()) ||
                            !String.IsNullOrWhiteSpace(ado.MONTH_LIFESPAN.ToString()) && !String.IsNullOrWhiteSpace(ado.HOUR_LIFESPAN.ToString()) ||
                            !String.IsNullOrWhiteSpace(ado.DAY_LIFESPAN.ToString()) && !String.IsNullOrWhiteSpace(ado.HOUR_LIFESPAN.ToString()))
                        {
                            ado.Warms.Add(HIS.Desktop.Plugins.ImpMestCreate.ADO.VHisServiceADO.Warm.KhongHopLe);
                        }
                        else if (String.IsNullOrWhiteSpace(ado.MONTH_LIFESPAN.ToString()) &&
                            String.IsNullOrWhiteSpace(ado.DAY_LIFESPAN.ToString()) &&
                            String.IsNullOrWhiteSpace(ado.HOUR_LIFESPAN.ToString()))
                        {
                            ado.Warms.Add(HIS.Desktop.Plugins.ImpMestCreate.ADO.VHisServiceADO.Warm.KhongCoTuoiTho);
                        }

                        if (item != null && !String.IsNullOrEmpty(item.MANUFACTURER_CODE))
                        {
                            var manufacturer = BackendDataWorker.Get<HIS_MANUFACTURER>().FirstOrDefault(o => o.MANUFACTURER_CODE == item.MANUFACTURER_CODE);
                            if (manufacturer != null)
                                ado.MANUFACTURER_ID = manufacturer.ID;
                        }
                        else if (medicine != null)
                        {
                            ado.MANUFACTURER_ID = medicine.MANUFACTURER_ID;
                        }
                        else if (material != null)
                        {
                            ado.MANUFACTURER_ID = material.MANUFACTURER_ID;
                        }
                        if (item != null && !String.IsNullOrEmpty(item.REGISTER_NUMBER))
                        {
                            ado.REGISTER_NUMBER = item.REGISTER_NUMBER;
                        }
                        else if (medicine != null)
                        {
                            ado.REGISTER_NUMBER = medicine.REGISTER_NUMBER;
                        }

                        if (ado.IsMedicine)
                        {
                            ado.HisMedicine.AMOUNT = ado.IMP_AMOUNT;
                            ado.HisMedicine.IMP_PRICE = ado.IMP_PRICE;
                            ado.HisMedicine.IMP_VAT_RATIO = ado.IMP_VAT_RATIO;
                            ado.HisMedicine.PACKAGE_NUMBER = ado.PACKAGE_NUMBER;
                            ado.HisMedicine.EXPIRED_DATE = ado.EXPIRED_DATE;
                            ado.HisMedicine.TDL_BID_NUM_ORDER = item.TDL_BID_NUM_ORDER;
                            ado.HisMedicine.TDL_BID_PACKAGE_CODE = item.TDL_BID_PACKAGE_CODE;
                            ado.HisMedicine.TDL_BID_GROUP_CODE = item.TDL_BID_GROUP_CODE;
                            ado.HisMedicine.TDL_BID_YEAR = item.TDL_BID_YEAR;
                            ado.HisMedicine.TDL_BID_NUMBER = item.TDL_BID_NUMBER;
                            ado.HisMedicine.NATIONAL_NAME = !String.IsNullOrEmpty(item.NATIONAL_NAME)
                                ? item.NATIONAL_NAME
                                : (medicine != null ? medicine.NATIONAL_NAME : "");
                            ado.HisMedicine.CONCENTRA = !String.IsNullOrEmpty(item.CONCENTRA)
                                ? item.CONCENTRA
                                : (medicine != null ? medicine.CONCENTRA : "");
                            if (!String.IsNullOrEmpty(item.MANUFACTURER_CODE))
                            {
                                var manufacturer = BackendDataWorker.Get<HIS_MANUFACTURER>().FirstOrDefault(o => o.MANUFACTURER_CODE == item.MANUFACTURER_CODE);
                                if (manufacturer != null)
                                    ado.HisMedicine.MANUFACTURER_ID = manufacturer.ID;
                            }
                            else
                            {
                                ado.HisMedicine.MANUFACTURER_ID = medicine != null ? medicine.MANUFACTURER_ID : null;
                            }
                            ado.HisMedicine.MEDICINE_REGISTER_NUMBER = !String.IsNullOrEmpty(item.REGISTER_NUMBER)
                                ? item.REGISTER_NUMBER
                                : (medicine != null ? medicine.REGISTER_NUMBER : "");
                        }
                        else
                        {
                            ado.HisMaterial.AMOUNT = ado.IMP_AMOUNT;
                            ado.HisMaterial.IMP_PRICE = ado.IMP_PRICE;
                            ado.HisMaterial.IMP_VAT_RATIO = ado.IMP_VAT_RATIO;
                            ado.HisMaterial.PACKAGE_NUMBER = ado.PACKAGE_NUMBER;
                            ado.HisMaterial.EXPIRED_DATE = ado.EXPIRED_DATE;
                            ado.HisMaterial.TDL_BID_NUM_ORDER = item.TDL_BID_NUM_ORDER;
                            ado.HisMaterial.TDL_BID_PACKAGE_CODE = item.TDL_BID_PACKAGE_CODE;
                            ado.HisMaterial.TDL_BID_GROUP_CODE = item.TDL_BID_GROUP_CODE;
                            ado.HisMaterial.TDL_BID_YEAR = item.TDL_BID_YEAR;
                            ado.HisMaterial.TDL_BID_NUMBER = item.TDL_BID_NUMBER;

                            ado.HisMaterial.NATIONAL_NAME = !String.IsNullOrEmpty(item.NATIONAL_NAME)
                                ? item.NATIONAL_NAME
                                : (material != null ? material.NATIONAL_NAME : "");
                            ado.HisMaterial.CONCENTRA = !String.IsNullOrEmpty(item.CONCENTRA)
                                ? item.CONCENTRA
                                : (material != null ? material.CONCENTRA : "");
                            if (!String.IsNullOrEmpty(item.MANUFACTURER_CODE))
                            {
                                var manufacturer = BackendDataWorker.Get<HIS_MANUFACTURER>().FirstOrDefault(o => o.MANUFACTURER_CODE == item.MANUFACTURER_CODE);
                                if (manufacturer != null)
                                    ado.HisMaterial.MANUFACTURER_ID = manufacturer.ID;
                            }
                            else
                            {
                                ado.HisMaterial.MANUFACTURER_ID = (material != null ? material.MANUFACTURER_ID : null);
                            }
                        }

                        if (!string.IsNullOrEmpty(item.SALE_EQUAL_IMP_PRICE) && item.SALE_EQUAL_IMP_PRICE == "x")
                        {
                            if (ado.IsMedicine)
                            {
                                ado.HisMedicine.IS_SALE_EQUAL_IMP_PRICE = 1;
                            }
                            else
                            {
                                ado.HisMaterial.IS_SALE_EQUAL_IMP_PRICE = 1;
                            }
                            ado.BanBangGiaNhap = true;
                        }
                        else if (!ado.BanBangGiaNhap)
                        {
                            #region Chinh sach gia

                            if (String.IsNullOrEmpty(item.PATIENT_TYPE_CODE_01)
                                && String.IsNullOrEmpty(item.PATIENT_TYPE_CODE_02)
                                && String.IsNullOrEmpty(item.PATIENT_TYPE_CODE_03)
                                && String.IsNullOrEmpty(item.PATIENT_TYPE_CODE_04)
                                && String.IsNullOrEmpty(item.PATIENT_TYPE_CODE_05)
                                && String.IsNullOrEmpty(item.PATIENT_TYPE_CODE_06)
                                && String.IsNullOrEmpty(item.PATIENT_TYPE_CODE_07)
                                && String.IsNullOrEmpty(item.PATIENT_TYPE_CODE_08)
                                && String.IsNullOrEmpty(item.PATIENT_TYPE_CODE_09)
                                && String.IsNullOrEmpty(item.PATIENT_TYPE_CODE_10)
                                )
                            {
                                ado.CheckImportThieuChinhSachGia = true;
                            }

                            if (!String.IsNullOrEmpty(item.PATIENT_TYPE_CODE_01))
                            {
                                if (!(item.EXP_PRICE_01.HasValue && item.EXP_PRICE_01.Value >= 0))
                                {
                                    param.Messages.Add(String.Format(ResourceMessageManager.GiaBanPhaiLonHonKhongMa, item.MEDI_MATE_CODE));
                                    throw new Exception("Gia ban phai lon hon khong ma:" + item.MEDI_MATE_CODE);
                                }
                                var patientType01 = BackendDataWorker.Get<HIS_PATIENT_TYPE>().FirstOrDefault(o => o.PATIENT_TYPE_CODE == item.PATIENT_TYPE_CODE_01.Trim().ToUpper());
                                if (patientType01 != null)
                                {
                                    if (ado.IsMedicine)
                                    {
                                        HIS_MEDICINE_PATY patyAdo01 = new HIS_MEDICINE_PATY();
                                        patyAdo01.PATIENT_TYPE_ID = patientType01.ID;
                                        patyAdo01.EXP_PRICE = item.EXP_PRICE_01.Value;
                                        patyAdo01.EXP_VAT_RATIO = (item.EXP_VAT_RATIO_01 ?? 0) / 100;
                                        ado.HisMedicinePatys.Add(patyAdo01);
                                    }
                                    else
                                    {
                                        HIS_MATERIAL_PATY patyAdo01 = new HIS_MATERIAL_PATY();
                                        patyAdo01.PATIENT_TYPE_ID = patientType01.ID;
                                        patyAdo01.EXP_PRICE = item.EXP_PRICE_01.Value;
                                        patyAdo01.EXP_VAT_RATIO = (item.EXP_VAT_RATIO_01 ?? 0) / 100;
                                        ado.HisMaterialPatys.Add(patyAdo01);
                                    }
                                }
                                else
                                {
                                    param.Messages.Add(String.Format(ResourceMessageManager.KhongTimThayDoiTuongTheoMa, item.PATIENT_TYPE_CODE_01));
                                    throw new Exception("Khong tim thay doi tuong theo ma:" + item.PATIENT_TYPE_CODE_01);
                                }
                            }

                            if (!String.IsNullOrEmpty(item.PATIENT_TYPE_CODE_02) && item.EXP_PRICE_02.HasValue && item.EXP_PRICE_02.Value >= 0)
                            {
                                if (!(item.EXP_PRICE_02.HasValue && item.EXP_PRICE_02.Value >= 0))
                                {
                                    param.Messages.Add(String.Format(ResourceMessageManager.GiaBanPhaiLonHonKhongMa, item.MEDI_MATE_CODE));
                                    throw new Exception("Gia ban phai lon hon khong ma:" + item.MEDI_MATE_CODE);
                                }
                                var patientType02 = BackendDataWorker.Get<HIS_PATIENT_TYPE>().FirstOrDefault(o => o.PATIENT_TYPE_CODE == item.PATIENT_TYPE_CODE_02.Trim().ToUpper());
                                if (patientType02 != null)
                                {
                                    if (ado.IsMedicine)
                                    {
                                        HIS_MEDICINE_PATY patyAdo02 = new HIS_MEDICINE_PATY();
                                        patyAdo02.PATIENT_TYPE_ID = patientType02.ID;
                                        patyAdo02.EXP_PRICE = item.EXP_PRICE_02.Value;
                                        patyAdo02.EXP_VAT_RATIO = (item.EXP_VAT_RATIO_02 ?? 0) / 100;
                                        ado.HisMedicinePatys.Add(patyAdo02);
                                    }
                                    else
                                    {
                                        HIS_MATERIAL_PATY patyAdo02 = new HIS_MATERIAL_PATY();
                                        patyAdo02.PATIENT_TYPE_ID = patientType02.ID;
                                        patyAdo02.EXP_PRICE = item.EXP_PRICE_02.Value;
                                        patyAdo02.EXP_VAT_RATIO = (item.EXP_VAT_RATIO_02 ?? 0) / 100;
                                        ado.HisMaterialPatys.Add(patyAdo02);
                                    }
                                }
                                else
                                {
                                    param.Messages.Add(String.Format(ResourceMessageManager.KhongTimThayDoiTuongTheoMa, item.PATIENT_TYPE_CODE_02));
                                    throw new Exception("Khong tim thay doi tuong theo ma:" + item.PATIENT_TYPE_CODE_02);
                                }
                            }

                            if (!String.IsNullOrEmpty(item.PATIENT_TYPE_CODE_03) && item.EXP_PRICE_03.HasValue && item.EXP_PRICE_03.Value >= 0)
                            {
                                if (!(item.EXP_PRICE_03.HasValue && item.EXP_PRICE_03.Value >= 0))
                                {
                                    param.Messages.Add(String.Format(ResourceMessageManager.GiaBanPhaiLonHonKhongMa, item.MEDI_MATE_CODE));
                                    throw new Exception("Gia ban phai lon hon khong ma:" + item.MEDI_MATE_CODE);
                                }
                                var patientType03 = BackendDataWorker.Get<HIS_PATIENT_TYPE>().FirstOrDefault(o => o.PATIENT_TYPE_CODE == item.PATIENT_TYPE_CODE_03.Trim().ToUpper());
                                if (patientType03 != null)
                                {
                                    if (ado.IsMedicine)
                                    {
                                        HIS_MEDICINE_PATY patyAdo03 = new HIS_MEDICINE_PATY();
                                        patyAdo03.PATIENT_TYPE_ID = patientType03.ID;
                                        patyAdo03.EXP_PRICE = item.EXP_PRICE_03.Value;
                                        patyAdo03.EXP_VAT_RATIO = (item.EXP_VAT_RATIO_03 ?? 0) / 100;
                                        ado.HisMedicinePatys.Add(patyAdo03);
                                    }
                                    else
                                    {
                                        HIS_MATERIAL_PATY patyAdo03 = new HIS_MATERIAL_PATY();
                                        patyAdo03.PATIENT_TYPE_ID = patientType03.ID;
                                        patyAdo03.EXP_PRICE = item.EXP_PRICE_03.Value;
                                        patyAdo03.EXP_VAT_RATIO = (item.EXP_VAT_RATIO_03 ?? 0) / 100;
                                        ado.HisMaterialPatys.Add(patyAdo03);
                                    }
                                }
                                else
                                {
                                    param.Messages.Add(String.Format(ResourceMessageManager.KhongTimThayDoiTuongTheoMa, item.PATIENT_TYPE_CODE_03));
                                    throw new Exception("Khong tim thay doi tuong theo ma:" + item.PATIENT_TYPE_CODE_03);
                                }
                            }

                            if (!String.IsNullOrEmpty(item.PATIENT_TYPE_CODE_04) && item.EXP_PRICE_04.HasValue && item.EXP_PRICE_04.Value >= 0)
                            {
                                if (!(item.EXP_PRICE_04.HasValue && item.EXP_PRICE_04.Value >= 0))
                                {
                                    param.Messages.Add(String.Format(ResourceMessageManager.GiaBanPhaiLonHonKhongMa, item.MEDI_MATE_CODE));
                                    throw new Exception("Gia ban phai lon hon khong ma:" + item.MEDI_MATE_CODE);
                                }
                                var patientType04 = BackendDataWorker.Get<HIS_PATIENT_TYPE>().FirstOrDefault(o => o.PATIENT_TYPE_CODE == item.PATIENT_TYPE_CODE_04.Trim().ToUpper());
                                if (patientType04 != null)
                                {
                                    if (ado.IsMedicine)
                                    {
                                        HIS_MEDICINE_PATY patyAdo04 = new HIS_MEDICINE_PATY();
                                        patyAdo04.PATIENT_TYPE_ID = patientType04.ID;
                                        patyAdo04.EXP_PRICE = item.EXP_PRICE_04.Value;
                                        patyAdo04.EXP_VAT_RATIO = (item.EXP_VAT_RATIO_04 ?? 0) / 100;
                                        ado.HisMedicinePatys.Add(patyAdo04);
                                    }
                                    else
                                    {
                                        HIS_MATERIAL_PATY patyAdo04 = new HIS_MATERIAL_PATY();
                                        patyAdo04.PATIENT_TYPE_ID = patientType04.ID;
                                        patyAdo04.EXP_PRICE = item.EXP_PRICE_04.Value;
                                        patyAdo04.EXP_VAT_RATIO = (item.EXP_VAT_RATIO_04 ?? 0) / 100;
                                        ado.HisMaterialPatys.Add(patyAdo04);
                                    }
                                }
                                else
                                {
                                    param.Messages.Add(String.Format(ResourceMessageManager.KhongTimThayDoiTuongTheoMa, item.PATIENT_TYPE_CODE_04));
                                    throw new Exception("Khong tim thay doi tuong theo ma:" + item.PATIENT_TYPE_CODE_04);
                                }
                            }

                            if (!String.IsNullOrEmpty(item.PATIENT_TYPE_CODE_05) && item.EXP_PRICE_05.HasValue && item.EXP_PRICE_05.Value >= 0)
                            {
                                if (!(item.EXP_PRICE_05.HasValue && item.EXP_PRICE_05.Value >= 0))
                                {
                                    param.Messages.Add(String.Format(ResourceMessageManager.GiaBanPhaiLonHonKhongMa, item.MEDI_MATE_CODE));
                                    throw new Exception("Gia ban phai lon hon khong ma:" + item.MEDI_MATE_CODE);
                                }
                                var patientType05 = BackendDataWorker.Get<HIS_PATIENT_TYPE>().FirstOrDefault(o => o.PATIENT_TYPE_CODE == item.PATIENT_TYPE_CODE_05.Trim().ToUpper());
                                if (patientType05 != null)
                                {
                                    if (ado.IsMedicine)
                                    {
                                        HIS_MEDICINE_PATY patyAdo05 = new HIS_MEDICINE_PATY();
                                        patyAdo05.PATIENT_TYPE_ID = patientType05.ID;
                                        patyAdo05.EXP_PRICE = item.EXP_PRICE_05.Value;
                                        patyAdo05.EXP_VAT_RATIO = (item.EXP_VAT_RATIO_05 ?? 0) / 100;
                                        ado.HisMedicinePatys.Add(patyAdo05);
                                    }
                                    else
                                    {
                                        HIS_MATERIAL_PATY patyAdo05 = new HIS_MATERIAL_PATY();
                                        patyAdo05.PATIENT_TYPE_ID = patientType05.ID;
                                        patyAdo05.EXP_PRICE = item.EXP_PRICE_05.Value;
                                        patyAdo05.EXP_VAT_RATIO = (item.EXP_VAT_RATIO_05 ?? 0) / 100;
                                        ado.HisMaterialPatys.Add(patyAdo05);
                                    }
                                }
                                else
                                {
                                    param.Messages.Add(String.Format(ResourceMessageManager.KhongTimThayDoiTuongTheoMa, item.PATIENT_TYPE_CODE_05));
                                    throw new Exception("Khong tim thay doi tuong theo ma:" + item.PATIENT_TYPE_CODE_05);
                                }
                            }

                            if (!String.IsNullOrEmpty(item.PATIENT_TYPE_CODE_06) && item.EXP_PRICE_06.HasValue && item.EXP_PRICE_06.Value >= 0)
                            {
                                if (!(item.EXP_PRICE_06.HasValue && item.EXP_PRICE_06.Value >= 0))
                                {
                                    param.Messages.Add(String.Format(ResourceMessageManager.GiaBanPhaiLonHonKhongMa, item.MEDI_MATE_CODE));
                                    throw new Exception("Gia ban phai lon hon khong ma:" + item.MEDI_MATE_CODE);
                                }
                                var patientType06 = BackendDataWorker.Get<HIS_PATIENT_TYPE>().FirstOrDefault(o => o.PATIENT_TYPE_CODE == item.PATIENT_TYPE_CODE_06.Trim().ToUpper());
                                if (patientType06 != null)
                                {
                                    if (ado.IsMedicine)
                                    {
                                        HIS_MEDICINE_PATY patyAdo06 = new HIS_MEDICINE_PATY();
                                        patyAdo06.PATIENT_TYPE_ID = patientType06.ID;
                                        patyAdo06.EXP_PRICE = item.EXP_PRICE_06.Value;
                                        patyAdo06.EXP_VAT_RATIO = (item.EXP_VAT_RATIO_06 ?? 0) / 100;
                                        ado.HisMedicinePatys.Add(patyAdo06);
                                    }
                                    else
                                    {
                                        HIS_MATERIAL_PATY patyAdo06 = new HIS_MATERIAL_PATY();
                                        patyAdo06.PATIENT_TYPE_ID = patientType06.ID;
                                        patyAdo06.EXP_PRICE = item.EXP_PRICE_06.Value;
                                        patyAdo06.EXP_VAT_RATIO = (item.EXP_VAT_RATIO_06 ?? 0) / 100;
                                        ado.HisMaterialPatys.Add(patyAdo06);
                                    }
                                }
                                else
                                {
                                    param.Messages.Add(String.Format(ResourceMessageManager.KhongTimThayDoiTuongTheoMa, item.PATIENT_TYPE_CODE_06));
                                    throw new Exception("Khong tim thay doi tuong theo ma:" + item.PATIENT_TYPE_CODE_06);
                                }
                            }

                            if (!String.IsNullOrEmpty(item.PATIENT_TYPE_CODE_07) && item.EXP_PRICE_07.HasValue && item.EXP_PRICE_07.Value >= 0)
                            {
                                if (!(item.EXP_PRICE_07.HasValue && item.EXP_PRICE_07.Value >= 0))
                                {
                                    param.Messages.Add(String.Format(ResourceMessageManager.GiaBanPhaiLonHonKhongMa, item.MEDI_MATE_CODE));
                                    throw new Exception("Gia ban phai lon hon khong ma:" + item.MEDI_MATE_CODE);
                                }
                                var patientType07 = BackendDataWorker.Get<HIS_PATIENT_TYPE>().FirstOrDefault(o => o.PATIENT_TYPE_CODE == item.PATIENT_TYPE_CODE_07.Trim().ToUpper());
                                if (patientType07 != null)
                                {
                                    if (ado.IsMedicine)
                                    {
                                        HIS_MEDICINE_PATY patyAdo07 = new HIS_MEDICINE_PATY();
                                        patyAdo07.PATIENT_TYPE_ID = patientType07.ID;
                                        patyAdo07.EXP_PRICE = item.EXP_PRICE_07.Value;
                                        patyAdo07.EXP_VAT_RATIO = (item.EXP_VAT_RATIO_07 ?? 0) / 100;
                                        ado.HisMedicinePatys.Add(patyAdo07);
                                    }
                                    else
                                    {
                                        HIS_MATERIAL_PATY patyAdo07 = new HIS_MATERIAL_PATY();
                                        patyAdo07.PATIENT_TYPE_ID = patientType07.ID;
                                        patyAdo07.EXP_PRICE = item.EXP_PRICE_07.Value;
                                        patyAdo07.EXP_VAT_RATIO = (item.EXP_VAT_RATIO_07 ?? 0) / 100;
                                        ado.HisMaterialPatys.Add(patyAdo07);
                                    }
                                }
                                else
                                {
                                    param.Messages.Add(String.Format(ResourceMessageManager.KhongTimThayDoiTuongTheoMa, item.PATIENT_TYPE_CODE_07));
                                    throw new Exception("Khong tim thay doi tuong theo ma:" + item.PATIENT_TYPE_CODE_07);
                                }
                            }

                            if (!String.IsNullOrEmpty(item.PATIENT_TYPE_CODE_08) && item.EXP_PRICE_08.HasValue && item.EXP_PRICE_08.Value >= 0)
                            {
                                if (!(item.EXP_PRICE_08.HasValue && item.EXP_PRICE_08.Value >= 0))
                                {
                                    param.Messages.Add(String.Format(ResourceMessageManager.GiaBanPhaiLonHonKhongMa, item.MEDI_MATE_CODE));
                                    throw new Exception("Gia ban phai lon hon khong ma:" + item.MEDI_MATE_CODE);
                                }
                                var patientType08 = BackendDataWorker.Get<HIS_PATIENT_TYPE>().FirstOrDefault(o => o.PATIENT_TYPE_CODE == item.PATIENT_TYPE_CODE_08.Trim().ToUpper());
                                if (patientType08 != null)
                                {
                                    if (ado.IsMedicine)
                                    {
                                        HIS_MEDICINE_PATY patyAdo08 = new HIS_MEDICINE_PATY();
                                        patyAdo08.PATIENT_TYPE_ID = patientType08.ID;
                                        patyAdo08.EXP_PRICE = item.EXP_PRICE_08.Value;
                                        patyAdo08.EXP_VAT_RATIO = (item.EXP_VAT_RATIO_08 ?? 0) / 100;
                                        ado.HisMedicinePatys.Add(patyAdo08);
                                    }
                                    else
                                    {
                                        HIS_MATERIAL_PATY patyAdo08 = new HIS_MATERIAL_PATY();
                                        patyAdo08.PATIENT_TYPE_ID = patientType08.ID;
                                        patyAdo08.EXP_PRICE = item.EXP_PRICE_08.Value;
                                        patyAdo08.EXP_VAT_RATIO = (item.EXP_VAT_RATIO_08 ?? 0) / 100;
                                        ado.HisMaterialPatys.Add(patyAdo08);
                                    }
                                }
                                else
                                {
                                    param.Messages.Add(String.Format(ResourceMessageManager.KhongTimThayDoiTuongTheoMa, item.PATIENT_TYPE_CODE_08));
                                    throw new Exception("Khong tim thay doi tuong theo ma:" + item.PATIENT_TYPE_CODE_08);
                                }
                            }

                            if (!String.IsNullOrEmpty(item.PATIENT_TYPE_CODE_09) && item.EXP_PRICE_09.HasValue && item.EXP_PRICE_09.Value >= 0)
                            {
                                if (!(item.EXP_PRICE_09.HasValue && item.EXP_PRICE_09.Value >= 0))
                                {
                                    param.Messages.Add(String.Format(ResourceMessageManager.GiaBanPhaiLonHonKhongMa, item.MEDI_MATE_CODE));
                                    throw new Exception("Gia ban phai lon hon khong ma:" + item.MEDI_MATE_CODE);
                                }
                                var patientType09 = BackendDataWorker.Get<HIS_PATIENT_TYPE>().FirstOrDefault(o => o.PATIENT_TYPE_CODE == item.PATIENT_TYPE_CODE_09.Trim().ToUpper());
                                if (patientType09 != null)
                                {
                                    if (ado.IsMedicine)
                                    {
                                        HIS_MEDICINE_PATY patyAdo09 = new HIS_MEDICINE_PATY();
                                        patyAdo09.PATIENT_TYPE_ID = patientType09.ID;
                                        patyAdo09.EXP_PRICE = item.EXP_PRICE_09.Value;
                                        patyAdo09.EXP_VAT_RATIO = (item.EXP_VAT_RATIO_09 ?? 0) / 100;
                                        ado.HisMedicinePatys.Add(patyAdo09);
                                    }
                                    else
                                    {
                                        HIS_MATERIAL_PATY patyAdo09 = new HIS_MATERIAL_PATY();
                                        patyAdo09.PATIENT_TYPE_ID = patientType09.ID;
                                        patyAdo09.EXP_PRICE = item.EXP_PRICE_09.Value;
                                        patyAdo09.EXP_VAT_RATIO = (item.EXP_VAT_RATIO_09 ?? 0) / 100;
                                        ado.HisMaterialPatys.Add(patyAdo09);
                                    }
                                }
                                else
                                {
                                    param.Messages.Add(String.Format(ResourceMessageManager.KhongTimThayDoiTuongTheoMa, item.PATIENT_TYPE_CODE_09));
                                    throw new Exception("Khong tim thay doi tuong theo ma:" + item.PATIENT_TYPE_CODE_09);
                                }
                            }

                            if (!String.IsNullOrEmpty(item.PATIENT_TYPE_CODE_10) && item.EXP_PRICE_10.HasValue && item.EXP_PRICE_10.Value >= 0)
                            {
                                if (!(item.EXP_PRICE_10.HasValue && item.EXP_PRICE_10.Value >= 0))
                                {
                                    param.Messages.Add(String.Format(ResourceMessageManager.GiaBanPhaiLonHonKhongMa, item.MEDI_MATE_CODE));
                                    throw new Exception("Gia ban phai lon hon khong ma:" + item.MEDI_MATE_CODE);
                                }
                                var patientType10 = BackendDataWorker.Get<HIS_PATIENT_TYPE>().FirstOrDefault(o => o.PATIENT_TYPE_CODE == item.PATIENT_TYPE_CODE_10.Trim().ToUpper());
                                if (patientType10 != null)
                                {
                                    if (ado.IsMedicine)
                                    {
                                        HIS_MEDICINE_PATY patyAdo10 = new HIS_MEDICINE_PATY();
                                        patyAdo10.PATIENT_TYPE_ID = patientType10.ID;
                                        patyAdo10.EXP_PRICE = item.EXP_PRICE_10.Value;
                                        patyAdo10.EXP_VAT_RATIO = (item.EXP_VAT_RATIO_10 ?? 0) / 100;
                                        ado.HisMedicinePatys.Add(patyAdo10);
                                    }
                                    else
                                    {
                                        HIS_MATERIAL_PATY patyAdo10 = new HIS_MATERIAL_PATY();
                                        patyAdo10.PATIENT_TYPE_ID = patientType10.ID;
                                        patyAdo10.EXP_PRICE = item.EXP_PRICE_10.Value;
                                        patyAdo10.EXP_VAT_RATIO = (item.EXP_VAT_RATIO_10 ?? 0) / 100;
                                        ado.HisMaterialPatys.Add(patyAdo10);
                                    }
                                }
                                else
                                {
                                    param.Messages.Add(String.Format(ResourceMessageManager.KhongTimThayDoiTuongTheoMa, item.PATIENT_TYPE_CODE_10));
                                    throw new Exception("Khong tim thay doi tuong theo ma:" + item.PATIENT_TYPE_CODE_10);
                                }
                            }
                            #endregion
                        }

                        //Hiển thị chính sách giá dịch vụ

                        //if (!dicServicePaty.ContainsKey(ado.SERVICE_ID) || dicServicePaty[ado.SERVICE_ID].Count == 0)
                        //{
                        //    Dictionary<long, VHisServicePatyADO> dicPaty = new Dictionary<long, VHisServicePatyADO>();

                        //    var listServicePaty = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_SERVICE_PATY>>("api/HisServicePaty/GetAppliedView", ApiConsumers.MosConsumer, null, null, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, "serviceId", ado.SERVICE_ID, "treatmentTime", null);
                        //    int row = 1;
                        //    foreach (var paty in BackendDataWorker.Get<HIS_PATIENT_TYPE>())
                        //    {
                        //        VHisServicePatyADO adoSerPaty = new VHisServicePatyADO();
                        //        adoSerPaty.PATIENT_TYPE_NAME = paty.PATIENT_TYPE_NAME;
                        //        adoSerPaty.PATIENT_TYPE_ID = paty.ID;
                        //        adoSerPaty.PATIENT_TYPE_CODE = paty.PATIENT_TYPE_CODE;
                        //        adoSerPaty.IsNotSell = true;
                        //        adoSerPaty.SERVICE_TYPE_ID = ado.SERVICE_TYPE_ID;
                        //        adoSerPaty.SERVICE_ID = ado.SERVICE_ID;
                        //        adoSerPaty.ID = row;
                        //        row++;
                        //        dicPaty[paty.ID] = adoSerPaty;

                        //    }

                        //    if (listServicePaty != null && listServicePaty.Count > 0)
                        //    {
                        //        foreach (var serPaty in listServicePaty)
                        //        {
                        //            if (dicPaty.ContainsKey(serPaty.PATIENT_TYPE_ID))
                        //            {
                        //                var adoPaty = dicPaty[serPaty.PATIENT_TYPE_ID];
                        //                if (!adoPaty.IsSetExpPrice)
                        //                {
                        //                    adoPaty.PRICE = serPaty.PRICE;
                        //                    adoPaty.VAT_RATIO = serPaty.VAT_RATIO;
                        //                    adoPaty.ExpVatRatio = serPaty.VAT_RATIO * 100;
                        //                    adoPaty.ExpPriceVat = serPaty.PRICE * (1 + serPaty.VAT_RATIO);
                        //                    adoPaty.IsNotSell = false;
                        //                    //ado.IsNotEdit = true;
                        //                    adoPaty.IsSetExpPrice = true;
                        //                }
                        //            }
                        //        }
                        //    }
                        //    dicServicePaty[ado.SERVICE_ID] = dicPaty.Select(s => s.Value).ToList();
                        //}

                        //var listData = dicServicePaty[ado.SERVICE_ID];
                        //AutoMapper.Mapper.CreateMap<VHisServicePatyADO, VHisServicePatyADO>();
                        //listServicePatyAdo = AutoMapper.Mapper.Map<List<VHisServicePatyADO>>(listData);
                        //foreach (var serPaty in listServicePatyAdo)
                        //{
                        //    serPaty.ExpPriceVat = serPaty.PRICE * (1 + serPaty.VAT_RATIO);
                        //}
                        //listServicePatyAdo = listServicePatyAdo.OrderByDescending(o => o.IsNotSell == false).ToList();

                        //ado.VHisServicePatys = listServicePatyAdo;

                        //bo sung thong tin thau
                        if (!string.IsNullOrWhiteSpace(item.TDL_BID_EXTRA_CODE))
                        {
                            if(item.TDL_BID_EXTRA_CODE.Length > 50)
                            {
                                
                                param.Messages.Add("Mã quyết định đầu vượt quá kí tự");
                                throw new Exception("Mã quyết định đầu vượt quá kí tự");
                            }
                            else
                            {
                                ado.TDL_BID_EXTRA_CODE = item.TDL_BID_EXTRA_CODE;
                            }
                        }
                        if (!string.IsNullOrWhiteSpace(item.INFORMATION_BID))
                        {
                            if (item.INFORMATION_BID == "2" || item.INFORMATION_BID == "3" || item.INFORMATION_BID == "4" )
                            {
                                ado.INFORMATION_BID = item.INFORMATION_BID;
                            }
                            else
                            {
                                param.Messages.Add("Loại thông tin thầu không hợp lệ");
                                
                                throw new Exception("Loại thông tin thầu không hợp lệ");
                            }
                        }
                        
                        if (!string.IsNullOrWhiteSpace(item.TT_THAU))
                        {
                            if (item.TT_THAU.Length > 50)
                            {
                                param.Messages.Add("Thông tin thầu vượt quá kí tự");
                                throw new Exception("Thông tin thầu vượt quá kí tự");
                            }
                            else
                            {
                                ado.TT_THAU = item.TT_THAU;
                            }
                        }
                        else
                        {
                            if (ado.IsMedicine)
                            {
                                ado.TT_THAU = ado.TDL_BID_NUMBER + ";" + ado.TDL_BID_PACKAGE_CODE + ";" + ado.TDL_BID_GROUP_CODE + ";" + ado.TDL_BID_YEAR;
                            }
                            else
                            {
                                string key = "{0};{1};{2};{3}";
                                if (ado.INFORMATION_BID == "3") key = "{0};{3}";
                                else if (ado.INFORMATION_BID == "4") key = "{0};{1};{3}";
                                string tt_thau = string.Format(key, ado.TDL_BID_EXTRA_CODE, ado.TDL_BID_PACKAGE_CODE, ado.TDL_BID_GROUP_CODE, ado.TDL_BID_YEAR);
                                ado.TT_THAU = tt_thau;
                            }
                        }

                        listServiceADO.Add(ado);
                    }
                }
                Inventec.Common.Logging.LogSystem.Debug("listServiceADO______ "+Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => listServiceADO), listServiceADO));
                gridControlImpMestDetail.BeginUpdate();
                gridControlImpMestDetail.DataSource = listServiceADO;
                gridControlImpMestDetail.EndUpdate();
                CalculTotalPrice();

                if (chkWarningOldBid.Checked)
                {
                    WaitingManager.Hide();
                    ProcessCheckBidNewer(listServiceADO);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                listServiceADO = new List<VHisServiceADO>();
                success = false;
            }
        }

        private void onClickBienBanKiemNhapTuNcc(object sender, EventArgs e)
        {
            try
            {
                Inventec.Common.RichEditor.RichEditorStore store = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumers.SarConsumer, ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), GlobalVariables.TemnplatePathFolder);
                if (this.resultADO == null)
                    return;
                if (this.resultADO.ImpMestTypeId == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__NCC)
                {
                    store.RunPrintTemplate(PrintTypeCodeStore.PRINT_TYPE_CODE__BienBanKiemNhapTuNhaCungCap_MPS000085, delegateProcessPrint);
                }
                else
                {
                    store.RunPrintTemplate(PRINT_TYPE_CODE__BienBanNhap_MPS000170, delegateProcessPrint);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void inPhieuNhapNhaCungCap()
        {
            try
            {
                Inventec.Common.RichEditor.RichEditorStore store = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumers.SarConsumer, ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), GlobalVariables.TemnplatePathFolder);
                if (this.resultADO == null)
                    return;
                store.RunPrintTemplate("Mps000141", delegateProcessPrint);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private bool delegateProcessPrint(string printTypeCode, string fileName)
        {
            bool result = false;
            try
            {
                if (!String.IsNullOrEmpty(printTypeCode) && !String.IsNullOrEmpty(fileName))
                {
                    switch (printTypeCode)
                    {
                        case PrintTypeCodeStore.PRINT_TYPE_CODE__BienBanKiemNhapTuNhaCungCap_MPS000085:
                            InBienBanKiemNhapTuNCC(ref result, printTypeCode, fileName);
                            break;
                        case PRINT_TYPE_CODE__BienBanNhap_MPS000170:
                            InBienBanNhap(ref result, printTypeCode, fileName);
                            break;
                        case "Mps000141":
                            InPhieuNhapTuNhaCungCap(printTypeCode, fileName, ref result);
                            break;
                        default:
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
            return result;
        }

        private void InBienBanNhap(ref bool result, string printTypeCode, string fileName)
        {
            try
            {
                WaitingManager.Show();

                MPS.ProcessorBase.Core.PrintData PrintData = null;

                Inventec.Common.SignLibrary.ADO.InputADO inputADO = new EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((this.resultADO != null && this.resultADO.HisInitSDO != null && this.resultADO.HisInitSDO.ImpMest != null) ? this.resultADO.HisInitSDO.ImpMest.TDL_TREATMENT_CODE : "", printTypeCode, roomId);

                if (resultADO.ImpMestTypeId == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__DK && resultADO.HisInitSDO != null)
                {
                    CommonParam param = new CommonParam();
                    MOS.Filter.HisImpMestUserViewFilter userFilter = new MOS.Filter.HisImpMestUserViewFilter();
                    userFilter.IMP_MEST_ID = this.resultADO.HisInitSDO.ImpMest.ID;
                    var _ImpMestUser = new BackendAdapter(param).Get<List<V_HIS_IMP_MEST_USER>>("/api/HisImpMestUser/GetView", ApiConsumers.MosConsumer, userFilter, param);
                    _ImpMestUser = _ImpMestUser.OrderBy(p => p.ID).ToList();

                    HisImpMestViewFilter initImpMestFilter = new HisImpMestViewFilter();
                    initImpMestFilter.ID = this.resultADO.HisInitSDO.ImpMest.ID;
                    var hisInitImpMests = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_IMP_MEST>>(HIS_INIT_IMP_MEST_GETVIEW, ApiConsumers.MosConsumer, initImpMestFilter, null);
                    if (hisInitImpMests != null && hisInitImpMests.Count != 1)
                    {
                        throw new NullReferenceException("Khong lay duoc InitImpMest bang impMestId: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => resultADO.HisInitSDO), resultADO.HisInitSDO));
                    }
                    var initImpMest = hisInitImpMests.FirstOrDefault();

                    HisImpMestMedicineViewFilter mediFilter = new HisImpMestMedicineViewFilter();
                    mediFilter.IMP_MEST_ID = this.resultADO.HisInitSDO.ImpMest.ID;
                    var hisImpMestMedicines = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_IMP_MEST_MEDICINE>>(HisRequestUriStore.HIS_IMP_MEST_MEDICINE_GETVIEW, ApiConsumers.MosConsumer, mediFilter, null);

                    HisImpMestMaterialViewFilter mateFilter = new HisImpMestMaterialViewFilter();
                    mateFilter.IMP_MEST_ID = this.resultADO.HisInitSDO.ImpMest.ID;
                    var hisImpMestMaterials = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_IMP_MEST_MATERIAL>>(HisRequestUriStore.HIS_IMP_MEST_MATERIAL_GETVIEW, ApiConsumers.MosConsumer, mateFilter, null);

                    List<HIS_MEDICINE> medicines = new List<HIS_MEDICINE>();
                    if (hisImpMestMedicines != null && hisImpMestMedicines.Count > 0)
                    {
                        HisMedicineFilter medicineFilter = new HisMedicineFilter();
                        medicineFilter.IDs = hisImpMestMedicines.Select(o => o.MEDICINE_ID).Distinct().ToList();
                        medicines = new BackendAdapter(new CommonParam()).Get<List<HIS_MEDICINE>>("api/HisMedicine/Get", ApiConsumers.MosConsumer, medicineFilter, new CommonParam());
                    }
                    List<HIS_MATERIAL> materials = new List<HIS_MATERIAL>();
                    if (hisImpMestMaterials != null && hisImpMestMaterials.Count > 0)
                    {
                        HisMaterialFilter materialFilter = new HisMaterialFilter();
                        materialFilter.IDs = hisImpMestMaterials.Select(o => o.MATERIAL_ID).Distinct().ToList();
                        materials = new BackendAdapter(new CommonParam()).Get<List<HIS_MATERIAL>>("api/HisMaterial/Get", ApiConsumers.MosConsumer, materialFilter, new CommonParam());
                    }

                    List<V_HIS_MEDICAL_CONTRACT> MedicalContract = new List<V_HIS_MEDICAL_CONTRACT>();
                    List<long> MedicalContractIds = new List<long>();

                    if (medicines != null && medicines.Count > 0)
                    {
                        List<long> medicineContract = medicines.Where(o => o.MEDICAL_CONTRACT_ID.HasValue).Select(p=>p.MEDICAL_CONTRACT_ID.Value).Distinct().ToList();
                        
                        if (medicineContract != null && medicineContract.Count > 0)
                        {
                            MedicalContractIds.AddRange(medicineContract);
                        }
                    }

                    if (materials != null && materials.Count > 0)
                    {
                        List<long> materialContract = materials.Where(o => o.MEDICAL_CONTRACT_ID.HasValue).Select(p => p.MEDICAL_CONTRACT_ID.Value).Distinct().ToList();
                      
                        if (materialContract != null && materialContract.Count > 0)
                        {
                            MedicalContractIds.AddRange(materialContract);
                        }
                    }

                    if (MedicalContractIds != null && MedicalContractIds.Count > 0)
                    {
                        HisMedicalContractViewFilter MedicalContractFilter = new HisMedicalContractViewFilter();
                        MedicalContractFilter.IDs = MedicalContractIds;
                        MedicalContract = new BackendAdapter(new CommonParam()).Get<List<V_HIS_MEDICAL_CONTRACT>>("api/HisMedicalContract/GetView", ApiConsumers.MosConsumer, MedicalContractFilter, new CommonParam());
                    }

                    List<MPS.Processor.Mps000199.PDO.Mps000199PDO.MedicalContractADO> MedicalContractADO = new List<MPS.Processor.Mps000199.PDO.Mps000199PDO.MedicalContractADO>();
                    if (MedicalContract != null && MedicalContract.Count > 0)
                    {
                        foreach (var item in medicines)
                        {
                            MPS.Processor.Mps000199.PDO.Mps000199PDO.MedicalContractADO ado = new MPS.Processor.Mps000199.PDO.Mps000199PDO.MedicalContractADO();

                            var Contract = MedicalContract.FirstOrDefault(o => o.ID == item.MEDICAL_CONTRACT_ID);
                            Inventec.Common.Mapper.DataObjectMapper.Map<MPS.Processor.Mps000199.PDO.Mps000199PDO.MedicalContractADO>(ado, Contract);
                            ado.MEDICINE_ID = item.ID;
                            MedicalContractADO.Add(ado);
                        }

                        foreach (var item in materials)
                        {
                            MPS.Processor.Mps000199.PDO.Mps000199PDO.MedicalContractADO ado = new MPS.Processor.Mps000199.PDO.Mps000199PDO.MedicalContractADO();

                            var Contract = MedicalContract.FirstOrDefault(o => o.ID == item.MEDICAL_CONTRACT_ID);
                            Inventec.Common.Mapper.DataObjectMapper.Map<MPS.Processor.Mps000199.PDO.Mps000199PDO.MedicalContractADO>(ado, Contract);
                            ado.MATERIAL_ID = item.ID;
                            MedicalContractADO.Add(ado);
                        }
                    }

                    MPS.Processor.Mps000199.PDO.Mps000199PDO Mps000199RDO = new MPS.Processor.Mps000199.PDO.Mps000199PDO(
                       initImpMest,
                       hisImpMestMedicines,
                       hisImpMestMaterials,
                       null,
                       _ImpMestUser,
                       MedicalContractADO
                       );

                    if (HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, Mps000199RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, "");
                    }
                    else
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, Mps000199RDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "");
                    }
                }
                else if (resultADO.ImpMestTypeId == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__KK && resultADO.HisInveSDO != null)
                {
                    CommonParam param = new CommonParam();
                    MOS.Filter.HisImpMestUserViewFilter userFilter = new MOS.Filter.HisImpMestUserViewFilter();
                    userFilter.IMP_MEST_ID = this.resultADO.HisInveSDO.ImpMest.ID;
                    var _ImpMestUser = new BackendAdapter(param).Get<List<V_HIS_IMP_MEST_USER>>("/api/HisImpMestUser/GetView", ApiConsumers.MosConsumer, userFilter, param);
                    _ImpMestUser = _ImpMestUser.OrderBy(p => p.ID).ToList();

                    HisImpMestViewFilter InveImpMestFilter = new HisImpMestViewFilter();
                    InveImpMestFilter.ID = this.resultADO.HisInveSDO.ImpMest.ID;
                    var hisInveImpMests = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_IMP_MEST>>(HIS_INVE_IMP_MEST_GETVIEW, ApiConsumers.MosConsumer, InveImpMestFilter, null);
                    if (hisInveImpMests != null && hisInveImpMests.Count != 1)
                    {
                        throw new NullReferenceException("Khong lay duoc inveImpMest bang impMestId: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => resultADO.HisInveSDO), resultADO.HisInveSDO));
                    }
                    var inveImpMest = hisInveImpMests.FirstOrDefault();

                    HisImpMestMedicineViewFilter mediFilter = new HisImpMestMedicineViewFilter();
                    mediFilter.IMP_MEST_ID = this.resultADO.HisInveSDO.ImpMest.ID;
                    var hisImpMestMedicines = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_IMP_MEST_MEDICINE>>(HisRequestUriStore.HIS_IMP_MEST_MEDICINE_GETVIEW, ApiConsumers.MosConsumer, mediFilter, null);

                    HisImpMestMaterialViewFilter mateFilter = new HisImpMestMaterialViewFilter();
                    mateFilter.IMP_MEST_ID = this.resultADO.HisInveSDO.ImpMest.ID;
                    var hisImpMestMaterials = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_IMP_MEST_MATERIAL>>(HisRequestUriStore.HIS_IMP_MEST_MATERIAL_GETVIEW, ApiConsumers.MosConsumer, mateFilter, null);

                    List<HIS_MEDICINE> medicines = new List<HIS_MEDICINE>();
                    if (hisImpMestMedicines != null && hisImpMestMedicines.Count > 0)
                    {
                        HisMedicineFilter medicineFilter = new HisMedicineFilter();
                        medicineFilter.IDs = hisImpMestMedicines.Select(o => o.MEDICINE_ID).Distinct().ToList();
                        medicines = new BackendAdapter(new CommonParam()).Get<List<HIS_MEDICINE>>("api/HisMedicine/Get", ApiConsumers.MosConsumer, medicineFilter, new CommonParam());
                    }
                    List<HIS_MATERIAL> materials = new List<HIS_MATERIAL>();
                    if (hisImpMestMaterials != null && hisImpMestMaterials.Count > 0)
                    {
                        HisMaterialFilter materialFilter = new HisMaterialFilter();
                        materialFilter.IDs = hisImpMestMaterials.Select(o => o.MATERIAL_ID).Distinct().ToList();
                        materials = new BackendAdapter(new CommonParam()).Get<List<HIS_MATERIAL>>("api/HisMaterial/Get", ApiConsumers.MosConsumer, materialFilter, new CommonParam());
                    }

                    List<V_HIS_MEDICAL_CONTRACT> MedicalContract = new List<V_HIS_MEDICAL_CONTRACT>();
                    List<long> MedicalContractIds = new List<long>();

                    if (medicines != null && medicines.Count > 0)
                    {
                        List<long> medicineContract = medicines.Where(o => o.MEDICAL_CONTRACT_ID.HasValue).Select(p => p.MEDICAL_CONTRACT_ID.Value).Distinct().ToList();

                        if (medicineContract != null && medicineContract.Count > 0)
                        {
                            MedicalContractIds.AddRange(medicineContract);
                        }
                    }

                    if (materials != null && materials.Count > 0)
                    {
                        List<long> materialContract = materials.Where(o => o.MEDICAL_CONTRACT_ID.HasValue).Select(p => p.MEDICAL_CONTRACT_ID.Value).Distinct().ToList();

                        if (materialContract != null && materialContract.Count > 0)
                        {
                            MedicalContractIds.AddRange(materialContract);
                        }
                    }

                    if (MedicalContractIds != null && MedicalContractIds.Count > 0)
                    {
                        HisMedicalContractViewFilter MedicalContractFilter = new HisMedicalContractViewFilter();
                        MedicalContractFilter.IDs = MedicalContractIds;
                        MedicalContract = new BackendAdapter(new CommonParam()).Get<List<V_HIS_MEDICAL_CONTRACT>>("api/HisMedicalContract/GetView", ApiConsumers.MosConsumer, MedicalContractFilter, new CommonParam());
                    }

                    List<MPS.Processor.Mps000199.PDO.Mps000199PDO.MedicalContractADO> MedicalContractADO = new List<MPS.Processor.Mps000199.PDO.Mps000199PDO.MedicalContractADO>();
                    if (MedicalContract != null && MedicalContract.Count > 0)
                    {
                        foreach (var item in medicines)
                        {
                            MPS.Processor.Mps000199.PDO.Mps000199PDO.MedicalContractADO ado = new MPS.Processor.Mps000199.PDO.Mps000199PDO.MedicalContractADO();

                            var Contract = MedicalContract.FirstOrDefault(o => o.ID == item.MEDICAL_CONTRACT_ID);
                            Inventec.Common.Mapper.DataObjectMapper.Map<MPS.Processor.Mps000199.PDO.Mps000199PDO.MedicalContractADO>(ado, Contract);
                            ado.MEDICINE_ID = item.ID;
                            MedicalContractADO.Add(ado);
                        }

                        foreach (var item in materials)
                        {
                            MPS.Processor.Mps000199.PDO.Mps000199PDO.MedicalContractADO ado = new MPS.Processor.Mps000199.PDO.Mps000199PDO.MedicalContractADO();

                            var Contract = MedicalContract.FirstOrDefault(o => o.ID == item.MEDICAL_CONTRACT_ID);
                            Inventec.Common.Mapper.DataObjectMapper.Map<MPS.Processor.Mps000199.PDO.Mps000199PDO.MedicalContractADO>(ado, Contract);
                            ado.MATERIAL_ID = item.ID;
                            MedicalContractADO.Add(ado);
                        }
                    }

                    MPS.Processor.Mps000199.PDO.Mps000199PDO Mps000170RDO = new MPS.Processor.Mps000199.PDO.Mps000199PDO(
                        inveImpMest,
                        hisImpMestMedicines,
                        hisImpMestMaterials,
                        null,
                        _ImpMestUser,
                        MedicalContractADO
                        );

                    if (HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, Mps000170RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, "");
                    }
                    else
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, Mps000170RDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "");
                    }
                }
                else if (resultADO.ImpMestTypeId == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__KHAC && resultADO.HisOtherSDO != null)
                {
                    CommonParam param = new CommonParam();
                    MOS.Filter.HisImpMestUserViewFilter userFilter = new MOS.Filter.HisImpMestUserViewFilter();
                    userFilter.IMP_MEST_ID = this.resultADO.HisOtherSDO.ImpMest.ID;
                    var _ImpMestUser = new BackendAdapter(param).Get<List<V_HIS_IMP_MEST_USER>>("/api/HisImpMestUser/GetView", ApiConsumers.MosConsumer, userFilter, param);
                    _ImpMestUser = _ImpMestUser.OrderBy(p => p.ID).ToList();


                    HisImpMestViewFilter otherImpMestFilter = new HisImpMestViewFilter();
                    otherImpMestFilter.ID = this.resultADO.HisOtherSDO.ImpMest.ID;
                    var hisOtherImpMests = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_IMP_MEST>>(HIS_OTHER_IMP_MEST_GETVIEW, ApiConsumers.MosConsumer, otherImpMestFilter, null);
                    if (hisOtherImpMests != null && hisOtherImpMests.Count != 1)
                    {
                        throw new NullReferenceException("Khong lay duoc otherImpMest bang impMestId: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => resultADO.HisOtherSDO), resultADO.HisOtherSDO));
                    }
                    var otherImpMest = hisOtherImpMests.FirstOrDefault();

                    HisImpMestMedicineViewFilter mediFilter = new HisImpMestMedicineViewFilter();
                    mediFilter.IMP_MEST_ID = this.resultADO.HisOtherSDO.ImpMest.ID;
                    var hisImpMestMedicines = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_IMP_MEST_MEDICINE>>(HisRequestUriStore.HIS_IMP_MEST_MEDICINE_GETVIEW, ApiConsumers.MosConsumer, mediFilter, null);

                    HisImpMestMaterialViewFilter mateFilter = new HisImpMestMaterialViewFilter();
                    mateFilter.IMP_MEST_ID = this.resultADO.HisOtherSDO.ImpMest.ID;
                    var hisImpMestMaterials = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_IMP_MEST_MATERIAL>>(HisRequestUriStore.HIS_IMP_MEST_MATERIAL_GETVIEW, ApiConsumers.MosConsumer, mateFilter, null);

                    List<HIS_MEDICINE> medicines = new List<HIS_MEDICINE>();
                    if (hisImpMestMedicines != null && hisImpMestMedicines.Count > 0)
                    {
                        HisMedicineFilter medicineFilter = new HisMedicineFilter();
                        medicineFilter.IDs = hisImpMestMedicines.Select(o => o.MEDICINE_ID).Distinct().ToList();
                        medicines = new BackendAdapter(new CommonParam()).Get<List<HIS_MEDICINE>>("api/HisMedicine/Get", ApiConsumers.MosConsumer, medicineFilter, new CommonParam());
                    }
                    List<HIS_MATERIAL> materials = new List<HIS_MATERIAL>();
                    if (hisImpMestMaterials != null && hisImpMestMaterials.Count > 0)
                    {
                        HisMaterialFilter materialFilter = new HisMaterialFilter();
                        materialFilter.IDs = hisImpMestMaterials.Select(o => o.MATERIAL_ID).Distinct().ToList();
                        materials = new BackendAdapter(new CommonParam()).Get<List<HIS_MATERIAL>>("api/HisMaterial/Get", ApiConsumers.MosConsumer, materialFilter, new CommonParam());
                    }

                    List<V_HIS_MEDICAL_CONTRACT> MedicalContract = new List<V_HIS_MEDICAL_CONTRACT>();
                    List<long> MedicalContractIds = new List<long>();

                    if (medicines != null && medicines.Count > 0)
                    {
                        List<long> medicineContract = medicines.Where(o => o.MEDICAL_CONTRACT_ID.HasValue).Select(p => p.MEDICAL_CONTRACT_ID.Value).Distinct().ToList();

                        if (medicineContract != null && medicineContract.Count > 0)
                        {
                            MedicalContractIds.AddRange(medicineContract);
                        }
                    }

                    if (materials != null && materials.Count > 0)
                    {
                        List<long> materialContract = materials.Where(o => o.MEDICAL_CONTRACT_ID.HasValue).Select(p => p.MEDICAL_CONTRACT_ID.Value).Distinct().ToList();

                        if (materialContract != null && materialContract.Count > 0)
                        {
                            MedicalContractIds.AddRange(materialContract);
                        }
                    }

                    if (MedicalContractIds != null && MedicalContractIds.Count > 0)
                    {
                        HisMedicalContractViewFilter MedicalContractFilter = new HisMedicalContractViewFilter();
                        MedicalContractFilter.IDs = MedicalContractIds;
                        MedicalContract = new BackendAdapter(new CommonParam()).Get<List<V_HIS_MEDICAL_CONTRACT>>("api/HisMedicalContract/GetView", ApiConsumers.MosConsumer, MedicalContractFilter, new CommonParam());
                    }

                    List<MPS.Processor.Mps000199.PDO.Mps000199PDO.MedicalContractADO> MedicalContractADO = new List<MPS.Processor.Mps000199.PDO.Mps000199PDO.MedicalContractADO>();
                    if (MedicalContract != null && MedicalContract.Count > 0)
                    {
                        foreach (var item in medicines)
                        {
                            MPS.Processor.Mps000199.PDO.Mps000199PDO.MedicalContractADO ado = new MPS.Processor.Mps000199.PDO.Mps000199PDO.MedicalContractADO();

                            var Contract = MedicalContract.FirstOrDefault(o => o.ID == item.MEDICAL_CONTRACT_ID);
                            Inventec.Common.Mapper.DataObjectMapper.Map<MPS.Processor.Mps000199.PDO.Mps000199PDO.MedicalContractADO>(ado, Contract);
                            ado.MEDICINE_ID = item.ID;
                            MedicalContractADO.Add(ado);
                        }

                        foreach (var item in materials)
                        {
                            MPS.Processor.Mps000199.PDO.Mps000199PDO.MedicalContractADO ado = new MPS.Processor.Mps000199.PDO.Mps000199PDO.MedicalContractADO();

                            var Contract = MedicalContract.FirstOrDefault(o => o.ID == item.MEDICAL_CONTRACT_ID);
                            Inventec.Common.Mapper.DataObjectMapper.Map<MPS.Processor.Mps000199.PDO.Mps000199PDO.MedicalContractADO>(ado, Contract);
                            ado.MATERIAL_ID = item.ID;
                            MedicalContractADO.Add(ado);
                        }
                    }

                    MPS.Processor.Mps000199.PDO.Mps000199PDO Mps000199RDO = new MPS.Processor.Mps000199.PDO.Mps000199PDO(
                        otherImpMest,
                        hisImpMestMedicines,
                        hisImpMestMaterials,
                        null,
                        _ImpMestUser,
                        MedicalContractADO);

                    if (HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, Mps000199RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, "");
                    }
                    else
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, Mps000199RDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "");
                    }
                }
                WaitingManager.Hide();
                if (PrintData != null)
                {
                    PrintData.EmrInputADO = inputADO;
                    result = MPS.MpsPrinter.Run(PrintData);
                }
                else
                    throw new Exception("lỗi in mps000199");
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InBienBanKiemNhapTuNCC(ref bool result, string printTypeCode, string fileName)
        {
            try
            {
                if (this.resultADO == null)
                    return;
                if (this.resultADO.HisManuSDO == null || this.resultADO.ImpMestTypeId != IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__NCC)
                {
                    MessageManager.Show(ResourceMessageManager.KhongPhaiLoaiNhapTuNhaCungCap);
                    return;
                }
                WaitingManager.Show();

                Inventec.Common.SignLibrary.ADO.InputADO inputADO = new EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((this.resultADO.HisManuSDO != null && this.resultADO.HisManuSDO.ImpMest != null) ? this.resultADO.HisManuSDO.ImpMest.TDL_TREATMENT_CODE : "", printTypeCode, roomId);

                HisImpMestViewFilter manuImpMestFilter = new HisImpMestViewFilter();
                manuImpMestFilter.ID = this.resultADO.HisManuSDO.ImpMest.ID;
                var hisManuImpMests = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_IMP_MEST>>("api/HisImpMest/GetView", ApiConsumers.MosConsumer, manuImpMestFilter, null);
                if (hisManuImpMests != null && hisManuImpMests.Count != 1)
                {
                    throw new NullReferenceException("Khong lay duoc ManuImpMest bang impMestId: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => resultADO.HisManuSDO), resultADO.HisManuSDO));
                }
                var manuImpMest = hisManuImpMests.FirstOrDefault();
                HisImpMestMedicineViewFilter mediFilter = new HisImpMestMedicineViewFilter();
                mediFilter.IMP_MEST_ID = this.resultADO.HisManuSDO.ImpMest.ID;
                var hisImpMestMedicines = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_IMP_MEST_MEDICINE>>(HisRequestUriStore.HIS_IMP_MEST_MEDICINE_GETVIEW, ApiConsumers.MosConsumer, mediFilter, null);
                List<HIS_MEDICINE> medicines = new List<HIS_MEDICINE>();
                if (hisImpMestMedicines != null && hisImpMestMedicines.Count > 0)
                {
                    HisMedicineFilter medicineFilter = new HisMedicineFilter();
                    medicineFilter.IDs = hisImpMestMedicines.Select(o => o.MEDICINE_ID).Distinct().ToList();
                    medicines = new BackendAdapter(new CommonParam()).Get<List<HIS_MEDICINE>>("api/HisMedicine/Get", ApiConsumers.MosConsumer, medicineFilter, new CommonParam());
                }

                HisImpMestMaterialViewFilter mateFilter = new HisImpMestMaterialViewFilter();
                mateFilter.IMP_MEST_ID = this.resultADO.HisManuSDO.ImpMest.ID;
                var hisImpMestMaterials = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_IMP_MEST_MATERIAL>>(HisRequestUriStore.HIS_IMP_MEST_MATERIAL_GETVIEW, ApiConsumers.MosConsumer, mateFilter, null);
                List<HIS_MATERIAL> materials = new List<HIS_MATERIAL>();
                if (hisImpMestMaterials != null && hisImpMestMaterials.Count > 0)
                {
                    HisMaterialFilter materialFilter = new HisMaterialFilter();
                    materialFilter.IDs = hisImpMestMaterials.Select(o => o.MATERIAL_ID).Distinct().ToList();
                    materials = new BackendAdapter(new CommonParam()).Get<List<HIS_MATERIAL>>("api/HisMaterial/Get", ApiConsumers.MosConsumer, materialFilter, new CommonParam());
                }


                MOS.Filter.HisImpMestViewFilter impMestViewFilter = new HisImpMestViewFilter();
                impMestViewFilter.ID = this.resultADO.HisManuSDO.ImpMest.ID;
                var impMests = new BackendAdapter(new CommonParam()).Get<List<MOS.EFMODEL.DataModels.V_HIS_IMP_MEST>>(HisRequestUriStore.HIS_IMP_MEST_GETVIEW, ApiConsumer.ApiConsumers.MosConsumer, impMestViewFilter, new CommonParam()).FirstOrDefault();

                HIS_SUPPLIER supplier = new HIS_SUPPLIER();
                if (impMests != null && impMests.SUPPLIER_ID != null)
                {
                    MOS.Filter.HisSupplierFilter supplierFilter = new HisSupplierFilter();
                    supplierFilter.ID = impMests.SUPPLIER_ID;
                    supplier = new BackendAdapter(new CommonParam()).Get<List<MOS.EFMODEL.DataModels.HIS_SUPPLIER>>("api/HisSupplier/Get", ApiConsumer.ApiConsumers.MosConsumer, supplierFilter, new CommonParam()).FirstOrDefault();
                }

                MOS.Filter.HisImpMestUserViewFilter userFilter = new MOS.Filter.HisImpMestUserViewFilter();
                userFilter.IMP_MEST_ID = this.resultADO.HisManuSDO.ImpMest.ID;
                var rsUser = new BackendAdapter(new CommonParam()).Get<List<V_HIS_IMP_MEST_USER>>("/api/HisImpMestUser/GetView", ApiConsumers.MosConsumer, userFilter, new CommonParam());
                rsUser = rsUser.OrderBy(p => p.ID).ToList();

                List<V_HIS_MEDICAL_CONTRACT> MedicalContract = new List<V_HIS_MEDICAL_CONTRACT>();
                List<long> MedicalContractIds = new List<long>();

                if (medicines != null && medicines.Count > 0)
                {
                    List<long> medicineContract = new List<long>();
                    foreach (var item in medicines)
                    {
                        if (item.MEDICAL_CONTRACT_ID.HasValue)
                        {
                            medicineContract.Add(item.MEDICAL_CONTRACT_ID.Value);
                        }
                    }
                    if (medicineContract != null && medicineContract.Count > 0)
                    {
                        MedicalContractIds.AddRange(medicineContract);
                    }
                }

                if (materials != null && materials.Count > 0)
                {
                    List<long> materialContract = new List<long>();
                    foreach (var item in materials)
                    {
                        if (item.MEDICAL_CONTRACT_ID.HasValue)
                        {
                            materialContract.Add(item.MEDICAL_CONTRACT_ID.Value);
                        }
                    }

                    if (materialContract != null && materialContract.Count > 0)
                    {
                        MedicalContractIds.AddRange(materialContract);
                    }
                }

                if (MedicalContractIds != null && MedicalContractIds.Count > 0)
                {
                    HisMedicalContractViewFilter MedicalContractFilter = new HisMedicalContractViewFilter();
                    MedicalContractFilter.IDs = MedicalContractIds;
                    MedicalContract = new BackendAdapter(new CommonParam()).Get<List<V_HIS_MEDICAL_CONTRACT>>("api/HisMedicalContract/GetView", ApiConsumers.MosConsumer, MedicalContractFilter, new CommonParam());
                }

                List<MPS.Processor.Mps000085.PDO.MedicalContractADO> MedicalContractADO = new List<MPS.Processor.Mps000085.PDO.MedicalContractADO>();

                if (MedicalContract != null && MedicalContract.Count > 0)
                {
                    foreach (var item in medicines)
                    {
                        MPS.Processor.Mps000085.PDO.MedicalContractADO ado = new MPS.Processor.Mps000085.PDO.MedicalContractADO();

                        var Contract = MedicalContract.FirstOrDefault(o => o.ID == item.MEDICAL_CONTRACT_ID);
                        Inventec.Common.Mapper.DataObjectMapper.Map<MPS.Processor.Mps000085.PDO.MedicalContractADO>(ado, Contract);
                        ado.MEDICINE_ID = item.ID;
                        MedicalContractADO.Add(ado);
                    }

                    foreach (var item in materials)
                    {
                        MPS.Processor.Mps000085.PDO.MedicalContractADO ado = new MPS.Processor.Mps000085.PDO.MedicalContractADO();

                        var Contract = MedicalContract.FirstOrDefault(o => o.ID == item.MEDICAL_CONTRACT_ID);
                        Inventec.Common.Mapper.DataObjectMapper.Map<MPS.Processor.Mps000085.PDO.MedicalContractADO>(ado, Contract);
                        ado.MATERIAL_ID = item.ID;
                        MedicalContractADO.Add(ado);
                    }
                }

                MPS.Processor.Mps000085.PDO.Mps000085PDO pdo = new MPS.Processor.Mps000085.PDO.Mps000085PDO(
                    impMests,
                    hisImpMestMedicines,
                    hisImpMestMaterials,
                    rsUser,
                    medicines,
                    materials,
                    supplier,
                    MedicalContractADO
                    );
                MPS.ProcessorBase.Core.PrintData printData = null;
                if (ConfigApplications.CheDoInChoCacChucNangTrongPhanMem == 2)
                {
                    printData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, "");
                }
                else
                {
                    printData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "");
                }
                WaitingManager.Hide();
                printData.EmrInputADO = inputADO;
                result = MPS.MpsPrinter.Run(printData);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetEnableButton(bool enable)
        {
            try
            {
                if (enable)
                {
                    btnAdd1.Enabled = false;
                    layoutAdd.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    btnUpdate1.Enabled = true;
                    btnCancel1.Enabled = true;

                    layoutUpdate.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    layoutCancel.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                }
                else
                {
                    btnSave.Enabled = true;
                    btnAdd1.Enabled = false;

                    layoutAdd.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    btnUpdate1.Enabled = false;
                    btnCancel1.Enabled = false;
                    layoutUpdate.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    layoutCancel.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessCheckBidNewer(List<VHisServiceADO> listServiceADO)
        {
            try
            {
                if (listServiceADO != null && listServiceADO.Count > 0)
                {
                    var listCheckTime = listServiceADO.Where(o => o.VALID_TO_TIME.HasValue).ToList();
                    var medicineType = listCheckTime.Where(o => o.IsMedicine).ToList();
                    var materialType = listCheckTime.Where(o => !o.IsMedicine).ToList();

                    List<BidValidTimeADO> listValidTime = new List<BidValidTimeADO>();
                    int max_req = 100;

                    if (medicineType != null && medicineType.Count > 0)
                    {
                        int skip = 0;
                        while (medicineType.Count - skip > 0)
                        {
                            var lstMety = medicineType.Skip(skip).Take(max_req).ToList();
                            skip += max_req;

                            HisBidMedicineTypeViewFilter filter = new HisBidMedicineTypeViewFilter();
                            filter.MEDICINE_TYPE_IDs = lstMety.Select(s => s.MEDI_MATE_ID).ToList();
                            var datas = new BackendAdapter(new CommonParam()).Get<List<V_HIS_BID_MEDICINE_TYPE>>("api/HisBidMedicineType/GetView", ApiConsumers.MosConsumer, filter, null);
                            if (datas != null && datas.Count > 0)
                            {
                                foreach (var item in lstMety)
                                {
                                    var bidMetys = datas.Where(o => o.MEDICINE_TYPE_ID == item.MEDI_MATE_ID).ToList();
                                    if (bidMetys != null && bidMetys.Count > 1)//có 2 thầu trở lên thì kiểm tra thời gian.
                                    {
                                        var bm = bidMetys.Where(o => o.VALID_TO_TIME > item.VALID_TO_TIME).OrderByDescending(o => o.VALID_TO_TIME).FirstOrDefault();
                                        if (bm != null)
                                        {
                                            BidValidTimeADO ado = new BidValidTimeADO();
                                            ado.CURRENT_BID_NUMBER = item.TDL_BID_NUMBER;
                                            ado.METY_MATY_CODE = item.MEDI_MATE_CODE;
                                            ado.METY_MATY_NAME = item.MEDI_MATE_NAME;
                                            ado.NEWER_BID_NUMBER = bm.BID_NUMBER;

                                            listValidTime.Add(ado);
                                        }
                                    }
                                }
                            }
                        }
                    }

                    if (materialType != null && materialType.Count > 0)
                    {
                        int skip = 0;
                        while (materialType.Count - skip > 0)
                        {
                            var lstMaty = materialType.Skip(skip).Take(max_req).ToList();
                            skip += max_req;

                            HisBidMaterialTypeViewFilter filter = new HisBidMaterialTypeViewFilter();
                            filter.MATERIAL_TYPE_IDs = lstMaty.Select(s => s.MEDI_MATE_ID).ToList();
                            var datas = new BackendAdapter(new CommonParam()).Get<List<V_HIS_BID_MATERIAL_TYPE>>("api/HisBidMaterialType/GetView", ApiConsumers.MosConsumer, filter, null);
                            if (datas != null && datas.Count > 0)
                            {
                                foreach (var item in lstMaty)
                                {
                                    var bidMatys = datas.Where(o => o.MATERIAL_TYPE_ID == item.MEDI_MATE_ID).ToList();
                                    if (bidMatys != null && bidMatys.Count > 1)//có 2 thầu trở lên thì kiểm tra thời gian.
                                    {
                                        var bm = bidMatys.Where(o => o.VALID_TO_TIME > item.VALID_TO_TIME).OrderByDescending(o => o.VALID_TO_TIME).FirstOrDefault();
                                        if (bm != null)
                                        {
                                            BidValidTimeADO ado = new BidValidTimeADO();
                                            ado.CURRENT_BID_NUMBER = item.TDL_BID_NUMBER;
                                            ado.METY_MATY_CODE = item.MEDI_MATE_CODE;
                                            ado.METY_MATY_NAME = item.MEDI_MATE_NAME;
                                            ado.NEWER_BID_NUMBER = bm.BID_NUMBER;

                                            listValidTime.Add(ado);
                                        }
                                    }
                                }
                            }
                        }
                    }

                    if (listValidTime != null && listValidTime.Count > 0)
                    {
                        Form.FormBidValidTime form = new Form.FormBidValidTime(listValidTime);
                        if (form != null)
                        {
                            form.ShowDialog();
                        }
                    }
                }
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
                gridViewServicePaty.PostEditor();
                if (btnAdd1.Enabled)
                    btnAdd1_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void FocusSearchPanel()
        {
            try
            {
                if (xtraTabControlMain.SelectedTabPage == xtraTabPageMedicine)
                {
                    medicineProcessor.FocusKeyword(this.ucMedicineTypeTree);
                }
                else if ((xtraTabControlMain.SelectedTabPage == xtraTabPageMaterial))
                {
                    materialProcessor.FocusKeyword(this.ucMaterialTypeTree);
                }
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
                gridViewServicePaty.PostEditor();
                if (btnUpdate1.Enabled)
                    btnUpdate1_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void BtnCancel()
        {
            try
            {
                if (btnCancel1.Enabled)
                    btnCancel1_Click(null, null);
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
                if (btnSave.Enabled == false)
                {
                    return;
                }
                gridViewImpMestDetail.PostEditor();
                btnSave_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void BtnSaveDraft()
        {
            try
            {
                gridViewImpMestDetail.PostEditor();
                if (btnSaveDraft.Enabled)
                    btnSaveDraft_Click(null, null);
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
                if (btnNew.Enabled)
                    btnNew_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void BtnPrint()
        {
            try
            {
                if (dropDownButton__Print.Enabled)
                    dropDownButton__Print_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void BtnImportExcel()
        {
            try
            {
                if (btnImportExcel.Enabled)
                    btnImportExcel_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}

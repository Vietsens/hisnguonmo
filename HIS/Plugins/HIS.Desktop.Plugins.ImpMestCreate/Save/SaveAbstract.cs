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
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Plugins.ImpMestCreate.ADO;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.ImpMestCreate.Save
{
    abstract class SaveAbstract : EntityBase
    {
        protected List<VHisServiceADO> ServiceADOs { get; set; }

        protected long ImpMestTypeId { get; set; }
        protected long MediStockId { get; set; }
        protected long? ImpSourceId { get; set; }
        protected long? SupplierId { get; set; }

        protected long RoomId { get; set; }
        protected string Description { get; set; }
        protected string LogginName { get; set; }
        protected string UserName { get; set; }
        protected string InvoiceSymbol { get; set; }
        protected string CREDIT_ACCOUNT { get; set; }//tk co
        protected string DEBIT_ACCOUNT { get; set; }//tk no

        protected V_HIS_BID Bid { get; set; }
        protected Dictionary<string, V_HIS_BID_MEDICINE_TYPE> dicBidMedicine = new Dictionary<string, V_HIS_BID_MEDICINE_TYPE>();
        protected Dictionary<string, V_HIS_BID_MATERIAL_TYPE> dicBidMaterial = new Dictionary<string, V_HIS_BID_MATERIAL_TYPE>();
        protected ResultImpMestADO ResultADO { get; set; }

        protected List<HisMaterialWithPatySDO> MaterialWithPatySDOs = new List<HisMaterialWithPatySDO>();
        protected List<HisMedicineWithPatySDO> MedicineWithPatySDOs = new List<HisMedicineWithPatySDO>();
        protected HIS_IMP_MEST ImpMest { get; set; }
        protected CommonParam Param { get; set; }

        protected SaveAbstract(CommonParam param,
            List<VHisServiceADO> serviceADOs,
            UCImpMestCreate ucImpMestCreate,
            Dictionary<string, V_HIS_BID_MEDICINE_TYPE> dicbidmedicine,
            Dictionary<string, V_HIS_BID_MATERIAL_TYPE> dicbidmaterial,
            long roomId,
            ResultImpMestADO resultADO)
            : base()
        {
            this.ImpMestTypeId = Inventec.Common.TypeConvert.Parse.ToInt64((ucImpMestCreate.cboImpMestType.EditValue ?? "0").ToString());
            this.MediStockId = Inventec.Common.TypeConvert.Parse.ToInt64((ucImpMestCreate.cboMediStock.EditValue ?? "0").ToString());
            if (ucImpMestCreate.cboImpSource.EditValue != null)
            {
                long _impS = Inventec.Common.TypeConvert.Parse.ToInt64((ucImpMestCreate.cboImpSource.EditValue ?? "0").ToString());
                if (_impS > 0)
                    this.ImpSourceId = _impS;
            }
            if (ucImpMestCreate.currentSupplierForEdit != null)
            {
                this.SupplierId = ucImpMestCreate.currentSupplierForEdit.ID;
            }
            this.dicBidMaterial = dicbidmaterial;
            this.dicBidMedicine = dicbidmedicine;
            this.ServiceADOs = serviceADOs;
            this.RoomId = roomId;
            this.Description = ucImpMestCreate.txtDescription.Text;
            if (ucImpMestCreate.cboRecieve.EditValue != null)
            {
                var receiver = BackendDataWorker.Get<ACS_USER>().Where(o => o.ID == (long)ucImpMestCreate.cboRecieve.EditValue).FirstOrDefault();
                if (receiver != null)
                {
                    this.LogginName = receiver.LOGINNAME;
                    this.UserName = receiver.USERNAME;
                }
            }
            this.InvoiceSymbol = ucImpMestCreate.txtkyHieuHoaDon.Text;
            this.CREDIT_ACCOUNT = ucImpMestCreate.txtTaiKhoanCo.Text;
            this.DEBIT_ACCOUNT = ucImpMestCreate.txtTaiKhoanNo.Text;
            this.ResultADO = resultADO;
            this.Param = param;

            if (this.Param==null)
            {
                param = new CommonParam();
            }
        }

        protected bool CheckValid()
        {
            bool valid = true;
            //Code thêm ...
            return valid;
        }

        protected void InitBase()
        {
            this.GenerateListMediMaty();
            this.GenerateImpMestData();
        }

        private void GenerateListMediMaty()
        {
            foreach (var ado in this.ServiceADOs)
            {
                if (ado.IsMedicine)
                {
                    HisMedicineWithPatySDO mediSdo = new HisMedicineWithPatySDO();
                    mediSdo.Medicine = ado.HisMedicine;

                    mediSdo.Medicine.TDL_BID_GROUP_CODE = ado.TDL_BID_GROUP_CODE;
                    mediSdo.Medicine.TDL_BID_NUM_ORDER = ado.TDL_BID_NUM_ORDER;
                    mediSdo.Medicine.TDL_BID_YEAR = ado.TDL_BID_YEAR;
                    mediSdo.Medicine.TDL_BID_PACKAGE_CODE = ado.TDL_BID_PACKAGE_CODE;
                    mediSdo.Medicine.TDL_BID_NUMBER = ado.TDL_BID_NUMBER;
                    mediSdo.Medicine.TDL_BID_EXTRA_CODE = ado.TDL_BID_EXTRA_CODE;

                    mediSdo.Medicine.BID_ID = ado.BidId;
                    mediSdo.Medicine.AMOUNT = ado.IMP_AMOUNT;
                    mediSdo.Medicine.IMP_PRICE = ado.IMP_PRICE;
                    mediSdo.Medicine.IMP_VAT_RATIO = ado.IMP_VAT_RATIO;
                    mediSdo.Medicine.IMP_SOURCE_ID = this.ImpSourceId;
                    mediSdo.Medicine.SUPPLIER_ID = ado.SupplierId;
                    mediSdo.Medicine.PACKAGE_NUMBER = ado.PACKAGE_NUMBER;
                    mediSdo.Medicine.CONCENTRA = ado.CONCENTRA;
                    mediSdo.Medicine.MEDICINE_REGISTER_NUMBER = ado.REGISTER_NUMBER;
                    mediSdo.Medicine.MANUFACTURER_ID = ado.MANUFACTURER_ID;
                    mediSdo.Medicine.NATIONAL_NAME = ado.NATIONAL_NAME;
                    mediSdo.Medicine.HEIN_SERVICE_BHYT_NAME = ado.heinServiceBhytName;
                    mediSdo.Medicine.PACKING_TYPE_NAME = ado.packingTypeName;
                    mediSdo.Medicine.ACTIVE_INGR_BHYT_CODE = ado.activeIngrBhytCode;
                    mediSdo.Medicine.ACTIVE_INGR_BHYT_NAME = ado.activeIngrBhytName;
                    mediSdo.Medicine.DOSAGE_FORM = ado.HisMedicine.DOSAGE_FORM;
                    mediSdo.Medicine.MEDICINE_USE_FORM_ID = ado.medicineUseFormId;
                    // mediSdo.Medicine.DOCUMENT_PRICE = ado.DOCUMENT_PRICE;
                    mediSdo.Medicine.DOCUMENT_PRICE = (long)Math.Round((ado.IMP_AMOUNT * ado.IMP_PRICE * (1 + ado.IMP_VAT_RATIO)), 0, MidpointRounding.AwayFromZero);
                    if (ado.VHisServicePatys != null && ado.VHisServicePatys.Count > 0)
                    {
                        var servicePaty = ado.VHisServicePatys.FirstOrDefault(o => o.PATIENT_TYPE_ID != Config.PatientTypeCFG.PATIENT_TYPE_ID__BHYT && o.PercentProfit > 0 && !o.IsNotSell);
                        if (servicePaty != null)
                        {
                            mediSdo.Medicine.PROFIT_RATIO = servicePaty.PercentProfit / 100;
                        }
                    }
                    mediSdo.Medicine.TAX_RATIO = ado.TAX_RATIO;
                    mediSdo.Medicine.MEDICAL_CONTRACT_ID = ado.MEDICAL_CONTRACT_ID;
                    mediSdo.Medicine.CONTRACT_PRICE = ado.CONTRACT_PRICE;
                    if (ado.TEMPERATURE.HasValue)
                        mediSdo.Temperature = ado.TEMPERATURE;
                    mediSdo.MedicinePaties = ado.HisMedicinePatys;
                    MedicineWithPatySDOs.Add(mediSdo);
                }
                else
                {
                    HisMaterialWithPatySDO mateSdo = new HisMaterialWithPatySDO();
                    mateSdo.Material = ado.HisMaterial;
                    mateSdo.Material.TDL_BID_GROUP_CODE = ado.TDL_BID_GROUP_CODE;
                    mateSdo.Material.TDL_BID_NUM_ORDER = ado.TDL_BID_NUM_ORDER;
                    mateSdo.Material.TDL_BID_NUMBER = ado.TDL_BID_NUMBER;
                    mateSdo.Material.TDL_BID_EXTRA_CODE = ado.TDL_BID_EXTRA_CODE;
                    mateSdo.Material.TDL_BID_YEAR = ado.TDL_BID_YEAR;
                    mateSdo.Material.TDL_BID_PACKAGE_CODE = ado.TDL_BID_PACKAGE_CODE;
                    mateSdo.Material.BID_ID = ado.BidId;
                    mateSdo.Material.AMOUNT = ado.IMP_AMOUNT;
                    mateSdo.Material.IMP_PRICE = ado.IMP_PRICE;
                    mateSdo.Material.IMP_VAT_RATIO = ado.IMP_VAT_RATIO;
                    mateSdo.Material.DOCUMENT_PRICE = (long)Math.Round((ado.IMP_AMOUNT * ado.IMP_PRICE * (1 + ado.IMP_VAT_RATIO)), 0, MidpointRounding.AwayFromZero);
                    // mateSdo.Material.DOCUMENT_PRICE = ado.DOCUMENT_PRICE;
                    mateSdo.Material.IMP_SOURCE_ID = this.ImpSourceId;
                    mateSdo.Material.SUPPLIER_ID = ado.SupplierId;
                    mateSdo.Material.PACKAGE_NUMBER = ado.PACKAGE_NUMBER;
                    mateSdo.Material.CONCENTRA = ado.CONCENTRA;
                    mateSdo.Material.MANUFACTURER_ID = ado.MANUFACTURER_ID;
                    mateSdo.Material.NATIONAL_NAME = ado.NATIONAL_NAME;
                    mateSdo.MaterialPaties = ado.HisMaterialPatys;
                    mateSdo.Material.TAX_RATIO = ado.TAX_RATIO;
                    mateSdo.Material.MEDICAL_CONTRACT_ID = ado.MEDICAL_CONTRACT_ID;
                    mateSdo.Material.CONTRACT_PRICE = ado.CONTRACT_PRICE;
                    //mateSdo.Material.HEIN_SERVICE_BHYT_NAME = ado.heinServiceBhytName;
                    mateSdo.Material.MATERIAL_REGISTER_NUMBER = ado.REGISTER_NUMBER;
                    mateSdo.SerialNumbers = ado.SerialNumbers;//xuandv
                    if (ado.VHisServicePatys != null && ado.VHisServicePatys.Count > 0)
                    {
                        var servicePaty = ado.VHisServicePatys.FirstOrDefault(o => o.PATIENT_TYPE_ID != Config.PatientTypeCFG.PATIENT_TYPE_ID__BHYT && o.PercentProfit > 0 && !o.IsNotSell);
                        if (servicePaty != null)
                        {
                            mateSdo.Material.PROFIT_RATIO = servicePaty.PercentProfit / 100;
                        }
                    }

                    MaterialWithPatySDOs.Add(mateSdo);
                }
            }
        }

        private void GenerateImpMestData()
        {
            ImpMest = new HIS_IMP_MEST();
            //Set giá trị theo dữ liệu đang chọn trên form
            ImpMest.REQ_ROOM_ID = this.RoomId;
            ImpMest.MEDI_STOCK_ID = this.MediStockId;
            ImpMest.IMP_MEST_STT_ID = IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_STT.ID__REQUEST;
            ImpMest.IMP_MEST_TYPE_ID = this.ImpMestTypeId;
            ImpMest.DESCRIPTION = this.Description;
            ImpMest.CREDIT_ACCOUNT = this.CREDIT_ACCOUNT;
            ImpMest.DEBIT_ACCOUNT = this.DEBIT_ACCOUNT;
        }

        private bool CheckInBid(VHisServiceADO serviceADO)
        {
            bool result = false;
            try
            {
                if (this.Bid != null && serviceADO != null)
                {
                    if (serviceADO.IsMedicine)
                    {
                        if (dicBidMedicine.ContainsKey(Base.StaticMethod.GetTypeKey(serviceADO.MEDI_MATE_ID, serviceADO.TDL_BID_GROUP_CODE)))
                        {
                            result = true;
                        }
                    }
                    else
                    {
                        if (dicBidMaterial.ContainsKey(Base.StaticMethod.GetTypeKey(serviceADO.MEDI_MATE_ID, serviceADO.TDL_BID_GROUP_CODE)))
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
    }
}

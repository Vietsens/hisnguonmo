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
using DevExpress.XtraEditors.DXErrorProvider;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.AssignPrescriptionPK.Config;
using HIS.Desktop.Plugins.AssignPrescriptionPK.Resources;
using HIS.Desktop.Plugins.AssignPrescriptionPK.Worker;
using MOS.EFMODEL.DataModels;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.AssignPrescriptionPK.ADO
{
    public class
        MediMatyTypeADO : MOS.EFMODEL.DataModels.V_HIS_MEDICINE_TYPE
    {
        public MediMatyTypeADO()
        {

        }

        public MediMatyTypeADO(MOS.EFMODEL.DataModels.V_HIS_EMTE_MEDICINE_TYPE inputData, long? intructionTime, bool isChayThan, long useTime = 0)
        {
            try
            {
                var mety = BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>().FirstOrDefault(o => o.ID == inputData.MEDICINE_TYPE_ID);
                if (mety != null)
                {
                    Inventec.Common.Mapper.DataObjectMapper.Map<MediMatyTypeADO>(this, mety);
                    this.IS_OUT_MEDI_STOCK = inputData.IS_OUT_MEDI_STOCK;
                    this.SERVICE_UNIT_ID = (inputData.SERVICE_UNIT_ID ?? 0);
                    this.SERVICE_UNIT_CODE = inputData.SERVICE_UNIT_CODE;
                    this.SERVICE_UNIT_NAME = inputData.SERVICE_UNIT_NAME;
                    this.TUTORIAL = (String.IsNullOrEmpty(inputData.TUTORIAL) ? mety.TUTORIAL : inputData.TUTORIAL);
                    if (
                        (HisConfigCFG.SplitOffset == GlobalVariables.CommonStringTrue || (mety.IS_SPLIT_COMPENSATION.HasValue && mety.IS_SPLIT_COMPENSATION == 1))
                        && (GlobalStore.IsTreatmentIn && !GlobalStore.IsCabinet)
                        && (this.IS_ALLOW_ODD == null || this.IS_ALLOW_ODD.Value != 1)
                        && (int)inputData.AMOUNT != inputData.AMOUNT
                         )
                    {
                        this.RAW_AMOUNT = inputData.AMOUNT;
                        this.AMOUNT = (decimal)Inventec.Common.Number.Convert.RoundUpValue((double)inputData.AMOUNT, 0);
                    }
                    else
                        this.AMOUNT = inputData.AMOUNT;
                    this.PRES_AMOUNT = inputData.AMOUNT;
                    this.CONTRAINDICATION = mety.CONTRAINDICATION;
                    this.MEDICINE_GROUP_ID = mety.MEDICINE_GROUP_ID;

                    if (!isChayThan && inputData.IS_OUT_MEDI_STOCK == GlobalVariables.CommonNumberTrue)
                    {
                        if (inputData.MEDICINE_TYPE_ID > 0)
                        {
                            this.DataType = HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC_DM;
                            this.ID = (inputData.MEDICINE_TYPE_ID ?? 0);
                        }
                        else
                        {
                            this.DataType = HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC_TUTUC;
                        }
                    }
                    else
                    {
                        //Lay doi tuong mac dinh
                        MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE patientType = new MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE();
                        patientType = AssignPrescriptionWorker.Instance.MediMatyCreateWorker.choosePatientTypeDefaultlServiceOther(AssignPrescriptionWorker.Instance.MediMatyCreateWorker.getPatientTypeId(), (inputData.SERVICE_ID ?? 0), mety.SERVICE_TYPE_ID);
                        if (patientType != null)
                        {
                            this.PATIENT_TYPE_ID = patientType.ID;
                            this.PATIENT_TYPE_CODE = patientType.PATIENT_TYPE_CODE;
                            this.PATIENT_TYPE_NAME = patientType.PATIENT_TYPE_NAME;
                        }

                        this.DataType = HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC;
                        this.ID = (inputData.MEDICINE_TYPE_ID ?? 0);
                        //Kiem tra xem thuoc do co trong kho??
                        AssignPrescriptionWorker.Instance.MediMatyCreateWorker.setDefaultMediStockForData(this);
                        MestMetyUnitWorker.UpdateUnit(this, GlobalStore.HisMestMetyUnit);
                    }

                    this.AMOUNT = ((this.IsUseOrginalUnitForPres ?? false) == false && inputData.CONVERT_RATIO.HasValue) ? (this.AMOUNT * inputData.CONVERT_RATIO.Value) : this.AMOUNT;
                    this.PRICE = ((this.IsUseOrginalUnitForPres ?? false) == false && inputData.CONVERT_RATIO.HasValue) ? (this.PRICE / inputData.CONVERT_RATIO.Value) : this.PRICE;
                    this.RAW_AMOUNT = (this.RAW_AMOUNT.HasValue && (this.IsUseOrginalUnitForPres ?? false) == false && inputData.CONVERT_RATIO.HasValue) ? (this.RAW_AMOUNT * inputData.CONVERT_RATIO.Value) : this.RAW_AMOUNT;
                    if (!String.IsNullOrEmpty(inputData.MORNING))
                    {
                        this.Sang = ConvertNumber.ProcessNumberInterger(inputData.MORNING);
                    }
                    if (!String.IsNullOrEmpty(inputData.NOON))
                    {
                        this.Trua = ConvertNumber.ProcessNumberInterger(inputData.NOON);
                    }
                    if (!String.IsNullOrEmpty(inputData.AFTERNOON))
                    {
                        this.Chieu = ConvertNumber.ProcessNumberInterger(inputData.AFTERNOON);
                    }
                    if (!String.IsNullOrEmpty(inputData.EVENING))
                    {
                        this.Toi = ConvertNumber.ProcessNumberInterger(inputData.EVENING);
                    }
                    if (inputData.HTU_ID != null)
                        this.HTU_IDs = new List<long>() { inputData.HTU_ID ?? 0 };

                    this.IsExpend = ((inputData.IS_EXPEND ?? 0) == GlobalVariables.CommonNumberTrue ? true : false);
                    this.IsAllowOdd = (mety.IS_ALLOW_ODD == 1) ? true : false;
                    this.IsAllowOddAndExportOdd = (mety.IS_ALLOW_ODD == 1 && mety.IS_ALLOW_EXPORT_ODD == 1) ? true : false;
                    this.IsKHBHYT = false;
                    this.NUM_ORDER = AssignPrescriptionWorker.Instance.MediMatyCreateWorker.getNumRow();
                    if (inputData.DAY_COUNT.HasValue)
                    {
                        this.UseDays = inputData.DAY_COUNT;
                        DateTime dtTime = new DateTime();
                        if (useTime > 0)
                            dtTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(useTime).Value;
                        else if (intructionTime.HasValue)
                            dtTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(intructionTime.Value).Value;
                        DateTime dtUseTimeTo = dtTime.AddDays((double)this.UseDays - 1);
                        this.UseTimeTo = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtUseTimeTo);
                    }
                    else
                    {
                        this.UseDays = 1;
                    }

                    if (inputData.HTU_ID != null)
                        this.HTU_IDs = new List<long>() { inputData.HTU_ID ?? 0 };

                    this.PrimaryKey = ((inputData.SERVICE_ID ?? 0) + "__" + Inventec.Common.DateTime.Get.Now() + "__" + Guid.NewGuid().ToString());
                    AssignPrescriptionWorker.Instance.MediMatyCreateWorker.setNumRow();
                }
                else
                {
                    Inventec.Common.Logging.LogSystem.Warn("Create by V_HIS_EMTE_MEDICINE_TYPE fail => Khong tim thay thuoc theo id = " + this.ID + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => this), this));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public MediMatyTypeADO(MOS.EFMODEL.DataModels.V_HIS_EMTE_MATERIAL_TYPE inputData, bool isChayThan)
        {
            try
            {
                var maty = BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>().FirstOrDefault(o => o.ID == inputData.MATERIAL_TYPE_ID);
                if (maty != null)
                {
                    Inventec.Common.Mapper.DataObjectMapper.Map<MediMatyTypeADO>(this, maty);
                    this.IS_OUT_MEDI_STOCK = inputData.IS_OUT_MEDI_STOCK;
                    this.ID = (inputData.MATERIAL_TYPE_ID ?? 0);
                    this.MEDICINE_TYPE_CODE = inputData.MATERIAL_TYPE_CODE;
                    this.MEDICINE_TYPE_NAME = inputData.MATERIAL_TYPE_NAME;
                    this.SERVICE_UNIT_ID = (inputData.SERVICE_UNIT_ID ?? 0);
                    this.SERVICE_UNIT_NAME = inputData.SERVICE_UNIT_NAME;
                    this.SERVICE_UNIT_CODE = inputData.SERVICE_UNIT_CODE;
                    this.DataType = maty != null && (maty.IS_REUSABLE == 1 || maty.IS_IDENTITY_MANAGEMENT == 1) ? HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.VATTU_TSD : HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.VATTU;
                    if (
                        (HisConfigCFG.SplitOffset == GlobalVariables.CommonStringTrue)
                        && (GlobalStore.IsTreatmentIn && !GlobalStore.IsCabinet)
                        && (this.IS_ALLOW_ODD == null || this.IS_ALLOW_ODD.Value != 1)
                        && (int)inputData.AMOUNT != inputData.AMOUNT
                         )
                    {
                        this.RAW_AMOUNT = inputData.AMOUNT;
                        this.AMOUNT = (decimal)Inventec.Common.Number.Convert.RoundUpValue((double)inputData.AMOUNT, 0);
                    }
                    else if (this.IS_ALLOW_ODD == 1 && this.IS_ALLOW_EXPORT_ODD != 1)
                    {
                        this.AMOUNT = (decimal)Inventec.Common.Number.Convert.RoundUpValue((double)inputData.AMOUNT, 0);
                    }
                    else
                        this.AMOUNT = inputData.AMOUNT;

                    this.PRES_AMOUNT = inputData.AMOUNT;
                    if (!isChayThan && inputData.IS_OUT_MEDI_STOCK == GlobalVariables.CommonNumberTrue)
                    {
                        this.DataType = HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.VATTU_DM;
                        this.ID = (inputData.MATERIAL_TYPE_ID ?? 0);
                    }
                    else
                    {
                        //Lay doi tuong mac dinh
                        MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE patientType = new MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE();
                        patientType = AssignPrescriptionWorker.Instance.MediMatyCreateWorker.choosePatientTypeDefaultlServiceOther(AssignPrescriptionWorker.Instance.MediMatyCreateWorker.getPatientTypeId(), (inputData.SERVICE_ID ?? 0), maty.SERVICE_TYPE_ID);
                        if (patientType != null)
                        {
                            this.PATIENT_TYPE_ID = patientType.ID;
                            this.PATIENT_TYPE_CODE = patientType.PATIENT_TYPE_CODE;
                            this.PATIENT_TYPE_NAME = patientType.PATIENT_TYPE_NAME;
                        }

                        this.DataType = HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.VATTU;
                        this.ID = (inputData.MATERIAL_TYPE_ID ?? 0);
                        //Kiem tra xem thuoc do co trong kho??
                        AssignPrescriptionWorker.Instance.MediMatyCreateWorker.setDefaultMediStockForData(this);
                    }

                    if (inputData.CONVERT_RATIO.HasValue)
                    {
                        this.AMOUNT = this.AMOUNT * inputData.CONVERT_RATIO.Value;
                        this.RAW_AMOUNT = (this.RAW_AMOUNT.HasValue && (this.IsUseOrginalUnitForPres ?? false) == false && inputData.CONVERT_RATIO.HasValue) ? (this.RAW_AMOUNT * inputData.CONVERT_RATIO.Value) : this.RAW_AMOUNT;
                    }
                    this.TUTORIAL = "";
                    this.UseDays = 1;
                    this.MEDICINE_USE_FORM_ID = null;
                    this.MEDICINE_USE_FORM_NAME = "";
                    this.IsExpend = ((inputData.IS_EXPEND ?? 0) == GlobalVariables.CommonNumberTrue ? true : false);
                    this.IsStent = ((maty.IS_STENT ?? 0) == GlobalVariables.CommonNumberTrue ? true : false);
                    this.IsAllowOdd = maty.IS_ALLOW_ODD == 1 ? true : false;
                    this.IsAllowOddAndExportOdd = (maty.IS_ALLOW_ODD == 1 && maty.IS_ALLOW_EXPORT_ODD == 1) ? true : false;
                    this.IsKHBHYT = false;
                    this.NUM_ORDER = AssignPrescriptionWorker.Instance.MediMatyCreateWorker.getNumRow();

                    //Chi dinh tu man hinh phau thuat, thu thuat
                    //if (this.isAutoCheckExpend == true
                    //    && this.HEIN_SERVICE_TYPE_ID != HisHeinServiceTypeCFG.HisHeinServiceTypeId__MaterialReplace)
                    //{
                    //    this.IsExpend = true;
                    //}

                    this.PrimaryKey = ((inputData.SERVICE_ID ?? 0) + "__" + Inventec.Common.DateTime.Get.Now() + "__" + Guid.NewGuid().ToString());
                    AssignPrescriptionWorker.Instance.MediMatyCreateWorker.setNumRow();
                }
                else
                {
                    Inventec.Common.Logging.LogSystem.Warn("Create by V_HIS_EMTE_MEDICINE_TYPE fail => Khong tim thay thuoc theo id = " + this.ID + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => this), this));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public MediMatyTypeADO(MOS.EFMODEL.DataModels.V_HIS_EQUIPMENT_SET_MATY inputData)
        {
            try
            {
                var maty = BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>().FirstOrDefault(o => o.ID == inputData.MATERIAL_TYPE_ID);
                if (maty != null)
                {
                    Inventec.Common.Mapper.DataObjectMapper.Map<MediMatyTypeADO>(this, maty);
                    this.DataType = (maty.IS_REUSABLE == 1 || maty.IS_IDENTITY_MANAGEMENT == 1) ? HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.VATTU_TSD : HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.VATTU;
                    this.ID = inputData.MATERIAL_TYPE_ID;
                    this.MEDICINE_TYPE_CODE = inputData.MATERIAL_TYPE_CODE;
                    this.MEDICINE_TYPE_NAME = inputData.MATERIAL_TYPE_NAME;
                    this.SERVICE_UNIT_ID = inputData.SERVICE_UNIT_ID;
                    this.SERVICE_UNIT_NAME = inputData.SERVICE_UNIT_NAME;
                    this.SERVICE_UNIT_CODE = inputData.SERVICE_UNIT_CODE;
                    if (this.IS_ALLOW_ODD == 1 && this.IS_ALLOW_EXPORT_ODD != 1)
                    {
                        this.AMOUNT = (decimal)Inventec.Common.Number.Convert.RoundUpValue((double)inputData.AMOUNT, 0);
                    }
                    else
                        this.AMOUNT = inputData.AMOUNT;

                    this.PRES_AMOUNT = inputData.AMOUNT;
                    this.TUTORIAL = "";
                    this.MEDICINE_USE_FORM_ID = null;
                    this.MEDICINE_USE_FORM_NAME = "";
                    this.IsStent = ((maty.IS_STENT ?? 0) == GlobalVariables.CommonNumberTrue ? true : false);
                    this.IsAllowOdd = maty.IS_ALLOW_ODD == 1 ? true : false;
                    this.IsAllowOddAndExportOdd = (maty.IS_ALLOW_ODD == 1 && maty.IS_ALLOW_EXPORT_ODD == 1) ? true : false;
                    this.IsKHBHYT = false;
                    this.NUM_ORDER = AssignPrescriptionWorker.Instance.MediMatyCreateWorker.getNumRow();
                    this.EQUIPMENT_SET_ID = inputData.EQUIPMENT_SET_ID;
                    this.IS_OUT_HOSPITAL = maty.IS_OUT_HOSPITAL;

                    //Lay doi tuong mac dinh
                    MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE patientType = new MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE();
                    patientType = AssignPrescriptionWorker.Instance.MediMatyCreateWorker.choosePatientTypeDefaultlServiceOther(AssignPrescriptionWorker.Instance.MediMatyCreateWorker.getPatientTypeId(), (inputData.SERVICE_ID), maty.SERVICE_TYPE_ID);
                    if (patientType != null)
                    {
                        this.PATIENT_TYPE_ID = patientType.ID;
                        this.PATIENT_TYPE_CODE = patientType.PATIENT_TYPE_CODE;
                        this.PATIENT_TYPE_NAME = patientType.PATIENT_TYPE_NAME;
                    }
                    //Kiem tra xem thuoc do co trong kho??
                    AssignPrescriptionWorker.Instance.MediMatyCreateWorker.setDefaultMediStockForData(this);

                    this.PrimaryKey = ((inputData.SERVICE_ID) + "__" + Inventec.Common.DateTime.Get.Now() + "__" + Guid.NewGuid().ToString());
                    AssignPrescriptionWorker.Instance.MediMatyCreateWorker.setNumRow();
                }
                else
                {
                    Inventec.Common.Logging.LogSystem.Warn("Create by V_HIS_EMTE_MEDICINE_TYPE fail => Khong tim thay thuoc theo id = " + this.ID + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => this), this));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public MediMatyTypeADO(MOS.EFMODEL.DataModels.V_HIS_EXP_MEST_MEDICINE inputData, List<V_HIS_MEDICINE_BEAN_1> medicineBeans, bool isEdit)
        {
            try
            {
                this.IsEdit = isEdit;
                Inventec.Common.Mapper.DataObjectMapper.Map<MediMatyTypeADO>(this, inputData);
                this.DataType = HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC;
                this.ID = inputData.MEDICINE_TYPE_ID;
                this.SERVICE_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC;
                this.PATIENT_TYPE_ID = inputData.PATIENT_TYPE_ID;
                this.PATIENT_TYPE_CODE = inputData.PATIENT_TYPE_CODE;
                this.PATIENT_TYPE_NAME = inputData.PATIENT_TYPE_NAME;
                this.SereServParentId = inputData.SERE_SERV_PARENT_ID;
                this.MIXED_INFUSION = inputData.MIXED_INFUSION;
                this.EXCEED_LIMIT_IN_BATCH_REASON = inputData.EXCEED_LIMIT_IN_TREAT_REASON;
                var mst = BackendDataWorker.Get<V_HIS_MEDI_STOCK>().FirstOrDefault(o => o.ID == inputData.TDL_MEDI_STOCK_ID);
                if (mst != null)
                {
                    this.MEDI_STOCK_ID = mst.ID;
                    this.MEDI_STOCK_CODE = mst.MEDI_STOCK_CODE;
                    this.MEDI_STOCK_NAME = mst.MEDI_STOCK_NAME;
                    MestMetyUnitWorker.UpdateUnit(this, GlobalStore.HisMestMetyUnit);
                }

                this.PRES_AMOUNT = ((this.IsUseOrginalUnitForPres ?? false) == false && inputData.CONVERT_RATIO.HasValue) ? (this.PRES_AMOUNT * inputData.CONVERT_RATIO.Value) : this.PRES_AMOUNT;
                this.AMOUNT = ((this.IsUseOrginalUnitForPres ?? false) == false && inputData.CONVERT_RATIO.HasValue) ? (this.AMOUNT * inputData.CONVERT_RATIO.Value) : this.AMOUNT;
                this.PRICE = ((this.IsUseOrginalUnitForPres ?? false) == false && inputData.CONVERT_RATIO.HasValue) ? (this.PRICE / inputData.CONVERT_RATIO.Value) : this.PRICE;

                var mety = BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>().FirstOrDefault(o => o.ID == inputData.MEDICINE_TYPE_ID);
                if (inputData.VAT_RATIO > 0)
                {
                    this.TotalPrice = ((this.PRICE ?? 0) * (this.AMOUNT ?? 0)) * (1 + (inputData.VAT_RATIO ?? 0));
                }
                else if (mety != null)
                {
                    if (mety.LAST_EXP_PRICE.HasValue || mety.LAST_EXP_VAT_RATIO.HasValue)
                    {
                        decimal? priceRaw = this.AMOUNT * this.PRICE * (1 + (mety.LAST_EXP_VAT_RATIO ?? 0));
                        this.TotalPrice = priceRaw ?? 0;
                    }
                    else
                    {
                        this.TotalPrice = ((this.AMOUNT ?? 0) * (this.PRICE ?? 0)) * (1 + (mety.IMP_VAT_RATIO ?? 0));
                    }
                }
                else
                    this.TotalPrice = ((this.PRICE ?? 0) * (this.AMOUNT ?? 0)) * (1 + (inputData.VAT_RATIO ?? 0));

                if (mety != null)
                {
                    this.ALERT_MAX_IN_PRESCRIPTION = mety.ALERT_MAX_IN_PRESCRIPTION;
                    this.ALERT_MAX_IN_DAY = mety.ALERT_MAX_IN_DAY;
                    this.ALERT_MAX_IN_TREATMENT = mety.ALERT_MAX_IN_TREATMENT;
                    this.TDL_GENDER_ID = mety.TDL_GENDER_ID;
                    this.CONTRAINDICATION = mety.CONTRAINDICATION;
                    this.DESCRIPTION = mety.DESCRIPTION;
                    this.IS_BLOCK_MAX_IN_PRESCRIPTION = mety.IS_BLOCK_MAX_IN_PRESCRIPTION;
                    this.IS_BLOCK_MAX_IN_TREATMENT = mety.IS_BLOCK_MAX_IN_TREATMENT;
                    this.IS_BLOCK_MAX_IN_DAY = mety.IS_BLOCK_MAX_IN_DAY;
                    this.IS_OXYGEN = mety.IS_OXYGEN;
                    this.IS_OUT_HOSPITAL = mety.IS_OUT_HOSPITAL;
                    this.IS_ALLOW_ODD = mety.IS_ALLOW_ODD;
                    this.IS_ALLOW_EXPORT_ODD = mety.IS_ALLOW_EXPORT_ODD;
                    this.ODD_WARNING_CONTENT = mety.ODD_WARNING_CONTENT;

                    this.IsAllowOdd = mety.IS_ALLOW_ODD == 1 ? true : false;
                    this.IsAllowOddAndExportOdd = (mety.IS_ALLOW_ODD == 1 && mety.IS_ALLOW_EXPORT_ODD == 1) ? true : false;

                    this.MEDICINE_GROUP_ID = mety.MEDICINE_GROUP_ID;
                    this.ATC_CODES = mety.ATC_CODES;
                    this.ATC_GROUP_CODES = mety.ATC_GROUP_CODES;
                    this.ACTIVE_INGR_BHYT_CODE = mety.ACTIVE_INGR_BHYT_CODE;
                    this.ACTIVE_INGR_BHYT_NAME = mety.ACTIVE_INGR_BHYT_NAME;
                }


                this.SERVICE_UNIT_NAME = inputData.SERVICE_UNIT_NAME;

                if (inputData.OTHER_PAY_SOURCE_ID.HasValue && inputData.OTHER_PAY_SOURCE_ID.Value > 0)
                {
                    var otherPaysources = BackendDataWorker.Get<HIS_OTHER_PAY_SOURCE>();
                    var otherPaysource = otherPaysources != null ? otherPaysources.Where(o => o.ID == inputData.OTHER_PAY_SOURCE_ID.Value).FirstOrDefault() : null;
                    if (otherPaysource != null)
                    {
                        this.OTHER_PAY_SOURCE_ID = otherPaysource.ID;
                        this.OTHER_PAY_SOURCE_CODE = otherPaysource.OTHER_PAY_SOURCE_CODE;
                        this.OTHER_PAY_SOURCE_NAME = otherPaysource.OTHER_PAY_SOURCE_NAME;
                    }
                }

                this.IsExpend = ((inputData.IS_EXPEND ?? -1) == GlobalVariables.CommonNumberTrue ? true : false);
                this.IsExpendType = ((inputData.EXPEND_TYPE_ID ?? -1) == GlobalVariables.CommonNumberTrue ? true : false);
                this.Speed = inputData.SPEED;
                this.IsOutKtcFee = ((inputData.IS_OUT_PARENT_FEE ?? 0) == GlobalVariables.CommonNumberTrue);
                this.IsKHBHYT = false;

                var checkMatyInStock = AssignPrescriptionWorker.Instance.MediMatyCreateWorker.getDataAmountOutOfStock(this, inputData.SERVICE_ID, inputData.MEDI_STOCK_ID);
                MediMatyTypeADO checkMediMatyTypeADO = new MediMatyTypeADO();
                Inventec.Common.Mapper.DataObjectMapper.Map<MediMatyTypeADO>(checkMediMatyTypeADO, checkMatyInStock);

                if (!String.IsNullOrEmpty(inputData.MORNING))
                {
                    this.Sang = ConvertNumber.ProcessNumberInterger(inputData.MORNING);
                }
                if (!String.IsNullOrEmpty(inputData.NOON))
                {
                    this.Trua = ConvertNumber.ProcessNumberInterger(inputData.NOON);
                }
                if (!String.IsNullOrEmpty(inputData.AFTERNOON))
                {
                    this.Chieu = ConvertNumber.ProcessNumberInterger(inputData.AFTERNOON);
                }
                if (!String.IsNullOrEmpty(inputData.EVENING))
                {
                    this.Toi = ConvertNumber.ProcessNumberInterger(inputData.EVENING);
                }
                if (!String.IsNullOrEmpty(inputData.BREATH_SPEED))
                {
                    this.BREATH_SPEED = ConvertNumber.ProcessNumberInterger(inputData.BREATH_SPEED);
                }
                if (!String.IsNullOrEmpty(inputData.BREATH_TIME))
                {
                    this.BREATH_TIME = ConvertNumber.ProcessNumberInterger(inputData.BREATH_TIME);
                }
                var htuIds = !string.IsNullOrEmpty(inputData.HTU_IDS) ? inputData.HTU_IDS.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).ToList() : null;
                if (htuIds != null && htuIds.Count > 0)
                {
                    foreach (var htu in htuIds)
                    {
                        if (HTU_IDs == null)
                            HTU_IDs = new List<long>();
                        this.HTU_IDs.Add(Int64.Parse(htu));
                    }
                }
                this.HTU_ID_NOT_CHECK_ACIN_INTERACTIVE = inputData.HTU_ID;
                if (isEdit)
                {
                    this.PRE_AMOUNT = this.AMOUNT;
                    this.NUM_ORDER = inputData.NUM_ORDER;
                    this.UseTimeTo = inputData.USE_TIME_TO;

                    this.MedicineBean1Result = medicineBeans != null ? medicineBeans.Where(o => o.EXP_MEST_MEDICINE_ID == inputData.ID).ToList() : null;
                    this.MAME_ID = inputData.MEDICINE_ID;
                    this.TDL_PACKAGE_NUMBER = inputData.PACKAGE_NUMBER;
                    this.REGISTER_NUMBER = (!String.IsNullOrEmpty(inputData.MEDICINE_REGISTER_NUMBER) ? inputData.MEDICINE_REGISTER_NUMBER : inputData.REGISTER_NUMBER);

                    if (medicineBeans != null && medicineBeans.Count > 0)
                    {
                        this.BeanIds = medicineBeans.Where(o => o.EXP_MEST_MEDICINE_ID == inputData.ID)
                            .Select(o => o.ID).ToList();
                    }

                    if (this.ExpMestDetailIds == null)
                    {
                        this.ExpMestDetailIds = new List<long>();
                    }
                    this.ExpMestDetailIds.Add(inputData.ID);

                }
                else
                {
                    //Trường hợp thuốc đang có trong kho
                    if (checkMediMatyTypeADO == null || checkMediMatyTypeADO.SERVICE_ID == 0)
                    {
                        //Nếu số lượng trong đơn cũ kiểm tra lớn hơn số lượng tồn kho hiện tại thì set cảnh báo
                        this.AmountAlert = this.AMOUNT;
                    }
                    else
                    {
                        if (checkMediMatyTypeADO.AMOUNT < this.AMOUNT)
                            this.AmountAlert = this.AMOUNT;//Nếu số lượng trong đơn cũ kiểm tra lớn hơn số lượng tồn kho hiện tại thì set cảnh báo
                    }
                    this.NUM_ORDER = AssignPrescriptionWorker.Instance.MediMatyCreateWorker.getNumRow();
                }

                UpdateMedicineUseFormInDataRow(this);

                if ((this.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC)
                    && this.PATIENT_TYPE_ID == HisConfigCFG.PatientTypeId__BHYT
                    && String.IsNullOrEmpty(this.TUTORIAL)
                    && !HisConfigCFG.IsNotAutoGenerateTutorial)
                {
                    this.ErrorMessageTutorial = ResourceMessage.DoiTuongBHYTBatBuocPhaiNhapHDSD;
                    this.ErrorTypeTutorial = ErrorType.Warning;
                }
                else
                {
                    this.ErrorMessageTutorial = "";
                    this.ErrorTypeTutorial = ErrorType.None;
                }

                this.PrimaryKey = (inputData.SERVICE_ID + "__" + Inventec.Common.DateTime.Get.Now() + "__" + Guid.NewGuid().ToString());

                AssignPrescriptionWorker.Instance.MediMatyCreateWorker.setNumRow();
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => inputData), inputData)
                    + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => isEdit), isEdit)
                    + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => mety), mety)
                    + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => AMOUNT), AMOUNT)
                    + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => PRICE), PRICE)
                    + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => TotalPrice), TotalPrice)
                    + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => this.MEDICINE_TYPE_NAME), this.MEDICINE_TYPE_NAME));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Đơn cũ
        /// </summary>
        /// <param name="inputData"></param>
        /// <param name="medicineBeans"></param>
        /// <param name="serviceReq"></param>
        public MediMatyTypeADO(MOS.EFMODEL.DataModels.V_HIS_EXP_MEST_MEDICINE inputData, long currentInstructionTime, List<HIS_MEDICINE_BEAN> medicineBeans, V_HIS_SERVICE_REQ_7 serviceReq)
        {
            try
            {
                Inventec.Common.Mapper.DataObjectMapper.Map<MediMatyTypeADO>(this, inputData);

                this.SERVICE_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC;
                this.PATIENT_TYPE_ID = inputData.PATIENT_TYPE_ID;
                this.PATIENT_TYPE_CODE = inputData.PATIENT_TYPE_CODE;
                this.PATIENT_TYPE_NAME = inputData.PATIENT_TYPE_NAME;
                this.SereServParentId = inputData.SERE_SERV_PARENT_ID;
                this.MIXED_INFUSION = inputData.MIXED_INFUSION;
                this.EXCEED_LIMIT_IN_BATCH_REASON = inputData.EXCEED_LIMIT_IN_TREAT_REASON;
                var mety = BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>().FirstOrDefault(o => o.ID == inputData.MEDICINE_TYPE_ID);
                if (mety != null)
                {
                    this.ALERT_MAX_IN_PRESCRIPTION = mety.ALERT_MAX_IN_PRESCRIPTION;
                    this.ALERT_MAX_IN_TREATMENT = mety.ALERT_MAX_IN_TREATMENT;
                    this.ALERT_MAX_IN_DAY = mety.ALERT_MAX_IN_DAY;
                    this.TDL_GENDER_ID = mety.TDL_GENDER_ID;
                    this.CONTRAINDICATION = mety.CONTRAINDICATION;
                    this.DESCRIPTION = mety.DESCRIPTION;
                    this.IS_BLOCK_MAX_IN_PRESCRIPTION = mety.IS_BLOCK_MAX_IN_PRESCRIPTION;
                    this.IS_BLOCK_MAX_IN_TREATMENT = mety.IS_BLOCK_MAX_IN_TREATMENT;
                    this.IS_BLOCK_MAX_IN_DAY = mety.IS_BLOCK_MAX_IN_DAY;
                    this.IS_OXYGEN = mety.IS_OXYGEN;
                    this.IS_OUT_HOSPITAL = mety.IS_OUT_HOSPITAL;
                    this.IS_ALLOW_ODD = mety.IS_ALLOW_ODD;
                    this.IS_ALLOW_EXPORT_ODD = mety.IS_ALLOW_EXPORT_ODD;
                    this.ODD_WARNING_CONTENT = mety.ODD_WARNING_CONTENT;

                    this.IsAllowOdd = mety.IS_ALLOW_ODD == 1 ? true : false;
                    this.IsAllowOddAndExportOdd = (mety.IS_ALLOW_ODD == 1 && mety.IS_ALLOW_EXPORT_ODD == 1) ? true : false;
                    this.MEDICINE_GROUP_ID = mety.MEDICINE_GROUP_ID;
                    this.ATC_CODES = mety.ATC_CODES;
                    this.ATC_GROUP_CODES = mety.ATC_GROUP_CODES;
                    this.ACTIVE_INGR_BHYT_CODE = mety.ACTIVE_INGR_BHYT_CODE;
                    this.ACTIVE_INGR_BHYT_NAME = mety.ACTIVE_INGR_BHYT_NAME;
                }

                this.SERVICE_UNIT_NAME = inputData.SERVICE_UNIT_NAME;
                this.ID = inputData.MEDICINE_TYPE_ID;
                this.DataType = HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC;

                var mst = BackendDataWorker.Get<V_HIS_MEDI_STOCK>().FirstOrDefault(o => o.ID == inputData.TDL_MEDI_STOCK_ID);
                if (mst != null)
                {
                    this.MEDI_STOCK_ID = mst.ID;
                    this.MEDI_STOCK_CODE = mst.MEDI_STOCK_CODE;
                    this.MEDI_STOCK_NAME = mst.MEDI_STOCK_NAME;
                    MestMetyUnitWorker.UpdateUnit(this, GlobalStore.HisMestMetyUnit);
                }

                this.PRES_AMOUNT = ((this.IsUseOrginalUnitForPres ?? false) == false && inputData.CONVERT_RATIO.HasValue) ? (this.PRES_AMOUNT * inputData.CONVERT_RATIO.Value) : this.PRES_AMOUNT;
                this.AMOUNT = ((this.IsUseOrginalUnitForPres ?? false) == false && inputData.CONVERT_RATIO.HasValue) ? (this.AMOUNT * inputData.CONVERT_RATIO.Value) : this.AMOUNT;
                this.PRICE = ((this.IsUseOrginalUnitForPres ?? false) == false && inputData.CONVERT_RATIO.HasValue) ? (this.PRICE / inputData.CONVERT_RATIO.Value) : this.PRICE;

                if (inputData.OTHER_PAY_SOURCE_ID.HasValue && inputData.OTHER_PAY_SOURCE_ID.Value > 0)
                {
                    var otherPaysources = BackendDataWorker.Get<HIS_OTHER_PAY_SOURCE>();
                    var otherPaysource = otherPaysources != null ? otherPaysources.Where(o => o.ID == inputData.OTHER_PAY_SOURCE_ID.Value).FirstOrDefault() : null;
                    if (otherPaysource != null)
                    {
                        this.OTHER_PAY_SOURCE_ID = otherPaysource.ID;
                        this.OTHER_PAY_SOURCE_CODE = otherPaysource.OTHER_PAY_SOURCE_CODE;
                        this.OTHER_PAY_SOURCE_NAME = otherPaysource.OTHER_PAY_SOURCE_NAME;
                    }
                }

                if (!String.IsNullOrEmpty(inputData.MORNING))
                {
                    this.Sang = ConvertNumber.ProcessNumberInterger(inputData.MORNING);
                }
                if (!String.IsNullOrEmpty(inputData.NOON))
                {
                    this.Trua = ConvertNumber.ProcessNumberInterger(inputData.NOON);
                }
                if (!String.IsNullOrEmpty(inputData.AFTERNOON))
                {
                    this.Chieu = ConvertNumber.ProcessNumberInterger(inputData.AFTERNOON);
                }
                if (!String.IsNullOrEmpty(inputData.EVENING))
                {
                    this.Toi = ConvertNumber.ProcessNumberInterger(inputData.EVENING);
                }
                if (!String.IsNullOrEmpty(inputData.BREATH_SPEED))
                {
                    this.BREATH_SPEED = ConvertNumber.ProcessNumberInterger(inputData.BREATH_SPEED);
                }
                if (!String.IsNullOrEmpty(inputData.BREATH_TIME))
                {
                    this.BREATH_TIME = ConvertNumber.ProcessNumberInterger(inputData.BREATH_TIME);
                }
                var htuIds = !string.IsNullOrEmpty(inputData.HTU_IDS) ? inputData.HTU_IDS.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).ToList() : null;
                if (htuIds != null && htuIds.Count > 0)
                {
                    foreach (var htu in htuIds)
                    {
                        if (HTU_IDs == null)
                            HTU_IDs = new List<long>();
                        this.HTU_IDs.Add(Int64.Parse(htu));
                    }
                }
                this.HTU_ID_NOT_CHECK_ACIN_INTERACTIVE = inputData.HTU_ID;

                this.IsExpend = ((inputData.IS_EXPEND ?? -1) == GlobalVariables.CommonNumberTrue ? true : false);
                this.IsExpendType = ((inputData.EXPEND_TYPE_ID ?? -1) == GlobalVariables.CommonNumberTrue ? true : false);
                this.Speed = inputData.SPEED;
                this.IsOutKtcFee = ((inputData.IS_OUT_PARENT_FEE ?? 0) == GlobalVariables.CommonNumberTrue);
                this.IsKHBHYT = false;

                //this.TotalPrice = ((this.PRICE ?? 0) * (this.AMOUNT ?? 0)) * (1 + (inputData.VAT_RATIO ?? 0));
                if (inputData.VAT_RATIO > 0)
                {
                    this.TotalPrice = ((this.PRICE ?? 0) * (this.AMOUNT ?? 0)) * (1 + (inputData.VAT_RATIO ?? 0));
                }
                else if (mety != null)
                {
                    if (mety.LAST_EXP_PRICE.HasValue || mety.LAST_EXP_VAT_RATIO.HasValue)
                    {
                        decimal? priceRaw = this.AMOUNT * this.PRICE * (1 + (mety.LAST_EXP_VAT_RATIO ?? 0));
                        this.TotalPrice = priceRaw ?? 0;
                    }
                    else
                    {
                        this.TotalPrice = ((this.AMOUNT ?? 0) * (this.PRICE ?? 0)) * (1 + (mety.IMP_VAT_RATIO ?? 0));
                    }
                }
                else
                    this.TotalPrice = ((this.PRICE ?? 0) * (this.AMOUNT ?? 0)) * (1 + (inputData.VAT_RATIO ?? 0));
                if (serviceReq != null)
                {
                    if (inputData.USE_TIME_TO.HasValue)
                    {
                        DateTime dtUseTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(serviceReq.USE_TIME ?? serviceReq.INTRUCTION_TIME).Value;
                        DateTime dtUseTimeTo = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(inputData.USE_TIME_TO.Value).Value;
                        TimeSpan ts = dtUseTimeTo.Date - dtUseTime.Date;
                        this.UseDays = ts.Days + 1;
                        DateTime dtInstructionTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(currentInstructionTime).Value;
                        this.UseTimeTo = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtInstructionTime.AddDays((double)(ts.Days)));
                    }
                    this.UseTime = serviceReq.USE_TIME;
                }

                this.NUM_ORDER = AssignPrescriptionWorker.Instance.MediMatyCreateWorker.getNumRow();

                //if (HisConfigCFG.SplitOffset == GlobalVariables.CommonStringTrue
                //   && ((this.IS_ALLOW_ODD ?? 0) != 1)
                //   && (this.AMOUNT != (int)this.AMOUNT))
                //{
                //    decimal amountDiv = (decimal)Inventec.Common.Number.Convert.RoundUpValue((double)(this.AMOUNT ?? 0), 0);
                //    this.RAW_AMOUNT = (item.RAW_AMOUNT ?? 0) - (item.AMOUNT ?? 0);
                //    result = ((item.IsUseOrginalUnitForPres ?? false) == false && (item.CONVERT_RATIO ?? 0) > 0) ? amountDiv / (item.CONVERT_RATIO ?? 1) : amountDiv;
                //}
                //this.TotalPrice = (this.AMOUNT ?? 0 * this.PRICE ?? 0);
                UpdateMedicineUseFormInDataRow(this);

                if ((this.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC)
                    && this.PATIENT_TYPE_ID == HisConfigCFG.PatientTypeId__BHYT
                    && String.IsNullOrEmpty(this.TUTORIAL)
                    && !HisConfigCFG.IsNotAutoGenerateTutorial)
                {
                    this.ErrorMessageTutorial = ResourceMessage.DoiTuongBHYTBatBuocPhaiNhapHDSD;
                    this.ErrorTypeTutorial = ErrorType.Warning;
                }
                else
                {
                    this.ErrorMessageTutorial = "";
                    this.ErrorTypeTutorial = ErrorType.None;
                }

                this.PrimaryKey = (inputData.SERVICE_ID + "__" + Inventec.Common.DateTime.Get.Now() + "__" + Guid.NewGuid().ToString());

                AssignPrescriptionWorker.Instance.MediMatyCreateWorker.setNumRow();

                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => inputData), inputData)
                    + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => mety), mety)
                    + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => AMOUNT), AMOUNT)
                    + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => PRICE), PRICE)
                    + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => TotalPrice), TotalPrice)
                    + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => this.MEDICINE_TYPE_NAME), this.MEDICINE_TYPE_NAME));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public MediMatyTypeADO(MOS.EFMODEL.DataModels.V_HIS_EXP_MEST_MATERIAL inputData, List<HIS_MATERIAL_BEAN> materialBeans, bool isEdit)
        {
            try
            {
                this.IsEdit = isEdit;
                Inventec.Common.Mapper.DataObjectMapper.Map<MediMatyTypeADO>(this, inputData);


                if (inputData.CONVERT_RATIO.HasValue)
                {
                    this.PRES_AMOUNT = this.PRES_AMOUNT * inputData.CONVERT_RATIO.Value;
                    this.AMOUNT = this.AMOUNT * inputData.CONVERT_RATIO.Value;
                    this.PRICE = this.PRICE / inputData.CONVERT_RATIO.Value;
                }

                this.SERVICE_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__VT;

                this.PATIENT_TYPE_ID = inputData.PATIENT_TYPE_ID;
                this.PATIENT_TYPE_CODE = inputData.PATIENT_TYPE_CODE;
                this.PATIENT_TYPE_NAME = inputData.PATIENT_TYPE_NAME;
                this.SereServParentId = inputData.SERE_SERV_PARENT_ID;
                this.SERVICE_UNIT_NAME = inputData.SERVICE_UNIT_NAME;
                this.ID = inputData.MATERIAL_TYPE_ID;
                this.MEDICINE_TYPE_CODE = inputData.MATERIAL_TYPE_CODE;
                this.MEDICINE_TYPE_NAME = inputData.MATERIAL_TYPE_NAME;
                this.MEDICINE_USE_FORM_NAME = "";
                this.TUTORIAL = inputData.TUTORIAL;

                if (inputData.OTHER_PAY_SOURCE_ID.HasValue && inputData.OTHER_PAY_SOURCE_ID.Value > 0)
                {
                    var otherPaysources = BackendDataWorker.Get<HIS_OTHER_PAY_SOURCE>();
                    var otherPaysource = otherPaysources != null ? otherPaysources.Where(o => o.ID == inputData.OTHER_PAY_SOURCE_ID.Value).FirstOrDefault() : null;
                    if (otherPaysource != null)
                    {
                        this.OTHER_PAY_SOURCE_ID = otherPaysource.ID;
                        this.OTHER_PAY_SOURCE_CODE = otherPaysource.OTHER_PAY_SOURCE_CODE;
                        this.OTHER_PAY_SOURCE_NAME = otherPaysource.OTHER_PAY_SOURCE_NAME;
                    }
                }

                this.IsExpend = ((inputData.IS_EXPEND ?? -1) == GlobalVariables.CommonNumberTrue ? true : false);
                this.IsExpendType = ((inputData.EXPEND_TYPE_ID ?? -1) == GlobalVariables.CommonNumberTrue ? true : false);
                this.IsOutKtcFee = ((inputData.IS_OUT_PARENT_FEE ?? 0) == GlobalVariables.CommonNumberTrue);

                if (inputData.EQUIPMENT_SET_ID.HasValue)
                {
                    this.EQUIPMENT_SET_ID = inputData.EQUIPMENT_SET_ID.Value;
                }



                var maty = BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>().FirstOrDefault(o => o.ID == inputData.MATERIAL_TYPE_ID);
                if (maty != null)
                {
                    this.IsStent = ((maty.IS_STENT ?? 0) == GlobalVariables.CommonNumberTrue ? true : false);
                    this.IsAllowOdd = maty.IS_ALLOW_ODD == 1 ? true : false;
                    this.IsAllowOddAndExportOdd = (maty.IS_ALLOW_ODD == 1 && maty.IS_ALLOW_EXPORT_ODD == 1) ? true : false;
                    this.IS_ALLOW_ODD = maty.IS_ALLOW_ODD;
                    this.IS_ALLOW_EXPORT_ODD = maty.IS_ALLOW_EXPORT_ODD;
                    this.ALERT_MAX_IN_PRESCRIPTION = maty.ALERT_MAX_IN_PRESCRIPTION;
                    this.ALERT_MAX_IN_DAY = maty.ALERT_MAX_IN_DAY;
                    this.TDL_GENDER_ID = maty.TDL_GENDER_ID;
                    this.MAX_REUSE_COUNT = maty.MAX_REUSE_COUNT;
                    this.IS_OUT_HOSPITAL = maty.IS_OUT_HOSPITAL;
                    this.IS_REUSABLE = maty.IS_REUSABLE;
                    this.IS_IDENTITY_MANAGEMENT = maty.IS_IDENTITY_MANAGEMENT;
                }
                if (this.IS_ALLOW_ODD == 1 && this.IS_ALLOW_EXPORT_ODD != 1)
                {
                    this.AMOUNT = (decimal)Inventec.Common.Number.Convert.RoundUpValue((double)this.AMOUNT, 0);
                }

                this.TotalPrice = ((this.PRICE ?? 0) * (this.AMOUNT ?? 0)) * (1 + (inputData.VAT_RATIO ?? 0));

                if (!String.IsNullOrEmpty(inputData.SERIAL_NUMBER))
                {
                    Inventec.Common.Logging.LogSystem.Debug("Truong hop vat tu trong don cu la vat tu TSD, tu dng chuyen sang vat tu trong kho" + inputData.MATERIAL_TYPE_NAME + " - " + inputData.SERIAL_NUMBER);
                }
                this.DataType = maty != null && (maty.IS_REUSABLE == 1 || maty.IS_IDENTITY_MANAGEMENT == 1) ? HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.VATTU_TSD : HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.VATTU;
                this.SERIAL_NUMBER = inputData.SERIAL_NUMBER;
                if (isEdit)
                {
                    this.USE_REMAIN_COUNT = inputData.REMAIN_REUSE_COUNT;

                    this.PRE_AMOUNT = this.AMOUNT;
                    this.NUM_ORDER = inputData.NUM_ORDER;

                    if (materialBeans != null && materialBeans.Count > 0)
                    {
                        this.BeanIds = materialBeans.Where(o => o.EXP_MEST_MATERIAL_ID == inputData.ID).Select(o => o.ID).ToList();
                    }

                    if (this.ExpMestDetailIds == null)
                    {
                        this.ExpMestDetailIds = new List<long>();
                    }
                    this.ExpMestDetailIds.Add(inputData.ID);
                }
                else
                {
                    this.USE_REMAIN_COUNT = null;

                    //Kiem tra xem vat tu do co trong kho??
                    var checkMatyInStock = AssignPrescriptionWorker.Instance.MediMatyCreateWorker.getDataAmountOutOfStock(this, inputData.SERVICE_ID, inputData.MEDI_STOCK_ID);
                    MediMatyTypeADO checkMediMatyTypeADO = new MediMatyTypeADO();
                    Inventec.Common.Mapper.DataObjectMapper.Map<MediMatyTypeADO>(checkMediMatyTypeADO, checkMatyInStock);
                    if (checkMediMatyTypeADO == null || checkMediMatyTypeADO.SERVICE_ID == 0)
                    {
                        //Nếu số lượng trong đơn cũ kiểm tra lớn hơn số lượng tồn kho hiện tại thì set cảnh báo
                        this.AmountAlert = this.AMOUNT;
                    }
                    else
                    {
                        if (checkMediMatyTypeADO.AMOUNT < this.AMOUNT)
                            this.AmountAlert = this.AMOUNT;//Nếu số lượng trong đơn cũ kiểm tra lớn hơn số lượng tồn kho hiện tại thì set cảnh báo
                    }


                    var mst = BackendDataWorker.Get<V_HIS_MEDI_STOCK>().FirstOrDefault(o => o.ID == inputData.TDL_MEDI_STOCK_ID);
                    if (mst != null)
                    {
                        this.MEDI_STOCK_ID = mst.ID;
                        this.MEDI_STOCK_CODE = mst.MEDI_STOCK_CODE;
                        this.MEDI_STOCK_NAME = mst.MEDI_STOCK_NAME;
                    }
                    this.NUM_ORDER = AssignPrescriptionWorker.Instance.MediMatyCreateWorker.getNumRow();
                }

                //this.TotalPrice = (this.AMOUNT ?? 0 * this.PRICE ?? 0);
                this.IsKHBHYT = false;

                this.PrimaryKey = (inputData.SERVICE_ID + "__" + Inventec.Common.DateTime.Get.Now() + "__" + Guid.NewGuid().ToString());
                AssignPrescriptionWorker.Instance.MediMatyCreateWorker.setNumRow();

                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => inputData), inputData)
                    + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => isEdit), isEdit)
                    + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => maty), maty)
                    + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => AMOUNT), AMOUNT)
                    + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => PRICE), PRICE)
                    + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => TotalPrice), TotalPrice)
                    + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => this.MEDICINE_TYPE_NAME), this.MEDICINE_TYPE_NAME));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public MediMatyTypeADO(MOS.EFMODEL.DataModels.V_HIS_EXP_MEST_MATERIAL inputData, List<V_HIS_MATERIAL_BEAN_1> materialBeans, bool isEdit)
        {
            try
            {
                this.IsEdit = isEdit;
                Inventec.Common.Mapper.DataObjectMapper.Map<MediMatyTypeADO>(this, inputData);
                if (inputData.CONVERT_RATIO.HasValue)
                {
                    this.PRES_AMOUNT = this.PRES_AMOUNT * inputData.CONVERT_RATIO.Value;
                    this.AMOUNT = this.AMOUNT * inputData.CONVERT_RATIO.Value;
                    this.PRICE = this.PRICE / inputData.CONVERT_RATIO.Value;
                }

                this.SERVICE_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__VT;

                this.PATIENT_TYPE_ID = inputData.PATIENT_TYPE_ID;
                this.PATIENT_TYPE_CODE = inputData.PATIENT_TYPE_CODE;
                this.PATIENT_TYPE_NAME = inputData.PATIENT_TYPE_NAME;
                this.SereServParentId = inputData.SERE_SERV_PARENT_ID;
                this.SERVICE_UNIT_NAME = inputData.SERVICE_UNIT_NAME;
                this.ID = inputData.MATERIAL_TYPE_ID;
                this.MEDICINE_TYPE_CODE = inputData.MATERIAL_TYPE_CODE;
                this.MEDICINE_TYPE_NAME = inputData.MATERIAL_TYPE_NAME;
                this.MEDICINE_USE_FORM_NAME = "";
                this.TUTORIAL = inputData.TUTORIAL;

                if (inputData.OTHER_PAY_SOURCE_ID.HasValue && inputData.OTHER_PAY_SOURCE_ID.Value > 0)
                {
                    var otherPaysources = BackendDataWorker.Get<HIS_OTHER_PAY_SOURCE>();
                    var otherPaysource = otherPaysources != null ? otherPaysources.Where(o => o.ID == inputData.OTHER_PAY_SOURCE_ID.Value).FirstOrDefault() : null;
                    if (otherPaysource != null)
                    {
                        this.OTHER_PAY_SOURCE_ID = otherPaysource.ID;
                        this.OTHER_PAY_SOURCE_CODE = otherPaysource.OTHER_PAY_SOURCE_CODE;
                        this.OTHER_PAY_SOURCE_NAME = otherPaysource.OTHER_PAY_SOURCE_NAME;
                    }
                }

                this.IsExpend = ((inputData.IS_EXPEND ?? -1) == GlobalVariables.CommonNumberTrue ? true : false);
                this.IsExpendType = ((inputData.EXPEND_TYPE_ID ?? -1) == GlobalVariables.CommonNumberTrue ? true : false);
                this.IsOutKtcFee = ((inputData.IS_OUT_PARENT_FEE ?? 0) == GlobalVariables.CommonNumberTrue);

                if (inputData.EQUIPMENT_SET_ID.HasValue)
                {
                    this.EQUIPMENT_SET_ID = inputData.EQUIPMENT_SET_ID.Value;
                }

                var maty = BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>().FirstOrDefault(o => o.ID == inputData.MATERIAL_TYPE_ID);
                if (maty != null)
                {
                    this.IS_ALLOW_ODD = maty.IS_ALLOW_ODD;
                    this.IS_ALLOW_EXPORT_ODD = maty.IS_ALLOW_EXPORT_ODD;
                    this.IsStent = ((maty.IS_STENT ?? 0) == GlobalVariables.CommonNumberTrue ? true : false);
                    this.IsAllowOdd = maty.IS_ALLOW_ODD == 1 ? true : false;
                    this.IsAllowOddAndExportOdd = (maty.IS_ALLOW_ODD == 1 && maty.IS_ALLOW_EXPORT_ODD == 1) ? true : false;
                    this.ALERT_MAX_IN_PRESCRIPTION = maty.ALERT_MAX_IN_PRESCRIPTION;
                    this.ALERT_MAX_IN_DAY = maty.ALERT_MAX_IN_DAY;
                    this.TDL_GENDER_ID = maty.TDL_GENDER_ID;
                    this.MAX_REUSE_COUNT = maty.MAX_REUSE_COUNT;
                    this.IS_OUT_HOSPITAL = maty.IS_OUT_HOSPITAL;
                    this.IS_REUSABLE = maty.IS_REUSABLE;
                    this.IS_IDENTITY_MANAGEMENT = maty.IS_IDENTITY_MANAGEMENT;
                }
                this.DataType = maty != null && (maty.IS_REUSABLE == 1 || maty.IS_IDENTITY_MANAGEMENT == 1) ? HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.VATTU_TSD : HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.VATTU;
                if (this.IS_ALLOW_ODD == 1 && this.IS_ALLOW_EXPORT_ODD != 1)
                {
                    this.AMOUNT = (decimal)Inventec.Common.Number.Convert.RoundUpValue((double)this.AMOUNT, 0);
                }
                if (inputData.VAT_RATIO > 0)
                {
                    this.TotalPrice = ((this.PRICE ?? 0) * (this.AMOUNT ?? 0)) * (1 + (inputData.VAT_RATIO ?? 0));
                }
                else if (maty != null)
                {
                    if (maty.LAST_EXP_PRICE.HasValue || maty.LAST_EXP_VAT_RATIO.HasValue)
                    {
                        decimal? priceRaw = this.AMOUNT * this.PRICE * (1 + (maty.LAST_EXP_VAT_RATIO ?? 0));
                        this.TotalPrice = priceRaw ?? 0;
                    }
                    else
                    {
                        this.TotalPrice = ((this.AMOUNT ?? 0) * (this.PRICE ?? 0)) * (1 + (maty.IMP_VAT_RATIO ?? 0));
                    }
                }
                else
                    this.TotalPrice = ((this.PRICE ?? 0) * (this.AMOUNT ?? 0)) * (1 + (inputData.VAT_RATIO ?? 0));



                if (!String.IsNullOrEmpty(inputData.SERIAL_NUMBER))
                {
                    Inventec.Common.Logging.LogSystem.Debug("Truong hop vat tu trong don cu la vat tu TSD, tu dng chuyen sang vat tu trong kho" + inputData.MATERIAL_TYPE_NAME + " - " + inputData.SERIAL_NUMBER);
                }

                this.SERIAL_NUMBER = inputData.SERIAL_NUMBER;
                if (isEdit)
                {
                    this.USE_REMAIN_COUNT = inputData.REMAIN_REUSE_COUNT;

                    this.PRE_AMOUNT = this.AMOUNT;
                    this.NUM_ORDER = inputData.NUM_ORDER;
                    this.MaterialBean1Result = materialBeans != null ? materialBeans.Where(o => o.EXP_MEST_MATERIAL_ID == inputData.ID).ToList() : null;
                    this.MAME_ID = inputData.MATERIAL_ID;
                    this.TDL_PACKAGE_NUMBER = inputData.PACKAGE_NUMBER;
                    if (materialBeans != null && materialBeans.Count > 0)
                    {
                        this.BeanIds = materialBeans.Where(o => o.EXP_MEST_MATERIAL_ID == inputData.ID).Select(o => o.ID).ToList();
                    }

                    if (this.ExpMestDetailIds == null)
                    {
                        this.ExpMestDetailIds = new List<long>();
                    }
                    this.ExpMestDetailIds.Add(inputData.ID);
                }
                else
                {
                    this.USE_REMAIN_COUNT = null;

                    //Kiem tra xem vat tu do co trong kho??
                    var checkMatyInStock = AssignPrescriptionWorker.Instance.MediMatyCreateWorker.getDataAmountOutOfStock(this, inputData.SERVICE_ID, inputData.MEDI_STOCK_ID);
                    MediMatyTypeADO checkMediMatyTypeADO = new MediMatyTypeADO();
                    Inventec.Common.Mapper.DataObjectMapper.Map<MediMatyTypeADO>(checkMediMatyTypeADO, checkMatyInStock);
                    if (checkMediMatyTypeADO == null || checkMediMatyTypeADO.SERVICE_ID == 0)
                    {
                        //Nếu số lượng trong đơn cũ kiểm tra lớn hơn số lượng tồn kho hiện tại thì set cảnh báo
                        this.AmountAlert = this.AMOUNT;
                    }
                    else
                    {
                        if (checkMediMatyTypeADO.AMOUNT < this.AMOUNT)
                            this.AmountAlert = this.AMOUNT;//Nếu số lượng trong đơn cũ kiểm tra lớn hơn số lượng tồn kho hiện tại thì set cảnh báo
                    }


                    var mst = BackendDataWorker.Get<V_HIS_MEDI_STOCK>().FirstOrDefault(o => o.ID == inputData.TDL_MEDI_STOCK_ID);
                    if (mst != null)
                    {
                        this.MEDI_STOCK_ID = mst.ID;
                        this.MEDI_STOCK_CODE = mst.MEDI_STOCK_CODE;
                        this.MEDI_STOCK_NAME = mst.MEDI_STOCK_NAME;
                    }
                    this.NUM_ORDER = AssignPrescriptionWorker.Instance.MediMatyCreateWorker.getNumRow();
                }

                this.IsKHBHYT = false;

                this.PrimaryKey = (inputData.SERVICE_ID + "__" + Inventec.Common.DateTime.Get.Now() + "__" + Guid.NewGuid().ToString());
                AssignPrescriptionWorker.Instance.MediMatyCreateWorker.setNumRow();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public MediMatyTypeADO(MOS.EFMODEL.DataModels.HIS_EXP_MEST_METY_REQ inputData, bool isEdit)
        {
            try
            {
                this.IsEdit = isEdit;
                var mety = BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>().FirstOrDefault(o => o.ID == inputData.MEDICINE_TYPE_ID);
                if (mety != null)
                {
                    Inventec.Common.Mapper.DataObjectMapper.Map<MediMatyTypeADO>(this, mety);

                    this.IsAllowOdd = mety.IS_ALLOW_ODD == 1 ? true : false;
                    this.IsAllowOddAndExportOdd = (mety.IS_ALLOW_ODD == 1 && mety.IS_ALLOW_EXPORT_ODD == 1) ? true : false;
                    this.ID = inputData.MEDICINE_TYPE_ID;
                    this.DataType = HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC;
                    this.CONTRAINDICATION = mety.CONTRAINDICATION;
                    this.MEDICINE_GROUP_ID = mety.MEDICINE_GROUP_ID;

                    var mst = BackendDataWorker.Get<V_HIS_MEDI_STOCK>().FirstOrDefault(o => o.ID == inputData.TDL_MEDI_STOCK_ID);
                    if (mst != null)
                    {
                        this.MEDI_STOCK_ID = mst.ID;
                        this.MEDI_STOCK_CODE = mst.MEDI_STOCK_CODE;
                        this.MEDI_STOCK_NAME = mst.MEDI_STOCK_NAME;
                        MestMetyUnitWorker.UpdateUnit(this, GlobalStore.HisMestMetyUnit);
                    }

                    this.AMOUNT = inputData.AMOUNT;
                    this.PRES_AMOUNT = ((this.IsUseOrginalUnitForPres ?? false) == false && this.CONVERT_RATIO.HasValue) ? (this.PRES_AMOUNT * this.CONVERT_RATIO.Value) : this.PRES_AMOUNT;
                    this.AMOUNT = ((this.IsUseOrginalUnitForPres ?? false) == false && this.CONVERT_RATIO.HasValue) ? (this.AMOUNT * this.CONVERT_RATIO.Value) : this.AMOUNT;
                    this.PRICE = ((this.IsUseOrginalUnitForPres ?? false) == false && this.CONVERT_RATIO.HasValue) ? (this.PRICE / this.CONVERT_RATIO.Value) : this.PRICE;

                    //Trường hợp thuốc đang có trong kho
                    var checkMatyInStock = AssignPrescriptionWorker.Instance.MediMatyCreateWorker.getDataAmountOutOfStock(this, mety.SERVICE_ID, inputData.TDL_MEDI_STOCK_ID);
                    MediMatyTypeADO checkMediMatyTypeADO = new MediMatyTypeADO();
                    Inventec.Common.Mapper.DataObjectMapper.Map<MediMatyTypeADO>(checkMediMatyTypeADO, checkMatyInStock);
                    if (checkMediMatyTypeADO == null || checkMediMatyTypeADO.SERVICE_ID == 0)
                    {
                        //Nếu số lượng trong đơn cũ kiểm tra lớn hơn số lượng tồn kho hiện tại thì set cảnh báo
                        this.AmountAlert = this.AMOUNT;
                    }
                    else if (checkMediMatyTypeADO.AMOUNT < this.AMOUNT)
                    {
                        this.AmountAlert = this.AMOUNT;//Nếu số lượng trong đơn cũ kiểm tra lớn hơn số lượng tồn kho hiện tại thì set cảnh báo
                    }

                    //Lay doi tuong mac dinh
                    MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE patientType = new MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE();
                    patientType = AssignPrescriptionWorker.Instance.MediMatyCreateWorker.choosePatientTypeDefaultlServiceOther(AssignPrescriptionWorker.Instance.MediMatyCreateWorker.getPatientTypeId(), this.SERVICE_ID, mety.SERVICE_TYPE_ID);
                    if (patientType != null)
                    {
                        this.PATIENT_TYPE_ID = patientType.ID;
                        this.PATIENT_TYPE_CODE = patientType.PATIENT_TYPE_CODE;
                        this.PATIENT_TYPE_NAME = patientType.PATIENT_TYPE_NAME;
                    }
                    if (isEdit)
                    {
                        this.PRE_AMOUNT = inputData.AMOUNT;
                        this.NUM_ORDER = inputData.NUM_ORDER;
                    }
                    else
                    {
                        this.NUM_ORDER = AssignPrescriptionWorker.Instance.MediMatyCreateWorker.getNumRow();
                    }
                    this.IsExpend = false;
                    this.IsKHBHYT = false;

                    this.PrimaryKey = (mety.SERVICE_ID + "__" + Inventec.Common.DateTime.Get.Now() + "__" + Guid.NewGuid().ToString());
                    AssignPrescriptionWorker.Instance.MediMatyCreateWorker.setNumRow();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public MediMatyTypeADO(MOS.EFMODEL.DataModels.HIS_EXP_MEST_MATY_REQ inputData, bool isEdit)
        {
            try
            {
                this.IsEdit = isEdit;
                var maty = BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>().FirstOrDefault(o => o.ID == inputData.MATERIAL_TYPE_ID);
                if (maty != null)
                {
                    Inventec.Common.Mapper.DataObjectMapper.Map<MediMatyTypeADO>(this, maty);
                    this.AMOUNT = inputData.AMOUNT;
                    if (this.CONVERT_RATIO.HasValue)
                    {

                        this.PRES_AMOUNT = this.PRES_AMOUNT * this.CONVERT_RATIO.Value;
                        this.AMOUNT = this.AMOUNT * this.CONVERT_RATIO.Value;
                        this.PRICE = this.PRICE / this.CONVERT_RATIO.Value;
                    }
                    if (this.IS_ALLOW_ODD == 1 && this.IS_ALLOW_EXPORT_ODD != 1)
                    {
                        this.AMOUNT = (decimal)Inventec.Common.Number.Convert.RoundUpValue((double)this.AMOUNT, 0);
                    }
                    this.MEDICINE_TYPE_CODE = maty.MATERIAL_TYPE_CODE;
                    this.MEDICINE_TYPE_NAME = maty.MATERIAL_TYPE_NAME;
                    this.IsExpend = false;
                    this.IsKHBHYT = false;
                    this.ID = inputData.MATERIAL_TYPE_ID;
                    this.DataType = maty != null && (maty.IS_REUSABLE == 1 || maty.IS_IDENTITY_MANAGEMENT == 1) ? HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.VATTU_TSD : HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.VATTU;

                    this.IsAllowOdd = maty.IS_ALLOW_ODD == 1 ? true : false;
                    //Trường hợp thuốc đang có trong kho
                    var checkMatyInStock = AssignPrescriptionWorker.Instance.MediMatyCreateWorker.getDataAmountOutOfStock(this, maty.SERVICE_ID, inputData.TDL_MEDI_STOCK_ID);
                    MediMatyTypeADO checkMediMatyTypeADO = new MediMatyTypeADO();
                    Inventec.Common.Mapper.DataObjectMapper.Map<MediMatyTypeADO>(checkMediMatyTypeADO, checkMatyInStock);
                    if (checkMediMatyTypeADO == null || checkMediMatyTypeADO.SERVICE_ID == 0)
                    {
                        //Nếu số lượng trong đơn cũ kiểm tra lớn hơn số lượng tồn kho hiện tại thì set cảnh báo
                        this.AmountAlert = this.AMOUNT;
                    }
                    else if (checkMediMatyTypeADO.AMOUNT < this.AMOUNT)
                    {
                        this.AmountAlert = this.AMOUNT;//Nếu số lượng trong đơn cũ kiểm tra lớn hơn số lượng tồn kho hiện tại thì set cảnh báo
                    }

                    var mst = BackendDataWorker.Get<V_HIS_MEDI_STOCK>().FirstOrDefault(o => o.ID == inputData.TDL_MEDI_STOCK_ID);
                    if (mst != null)
                    {
                        this.MEDI_STOCK_ID = mst.ID;
                        this.MEDI_STOCK_CODE = mst.MEDI_STOCK_CODE;
                        this.MEDI_STOCK_NAME = mst.MEDI_STOCK_NAME;
                    }
                    //Lay doi tuong mac dinh
                    MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE patientType = new MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE();
                    patientType = AssignPrescriptionWorker.Instance.MediMatyCreateWorker.choosePatientTypeDefaultlServiceOther(AssignPrescriptionWorker.Instance.MediMatyCreateWorker.getPatientTypeId(), this.SERVICE_ID, maty.SERVICE_TYPE_ID);
                    if (patientType != null)
                    {
                        this.PATIENT_TYPE_ID = patientType.ID;
                        this.PATIENT_TYPE_CODE = patientType.PATIENT_TYPE_CODE;
                        this.PATIENT_TYPE_NAME = patientType.PATIENT_TYPE_NAME;
                    }
                    if (isEdit)
                    {
                        this.PRE_AMOUNT = inputData.AMOUNT;
                        this.NUM_ORDER = inputData.NUM_ORDER;
                    }
                    else
                    {
                        this.NUM_ORDER = AssignPrescriptionWorker.Instance.MediMatyCreateWorker.getNumRow();
                    }
                    this.PrimaryKey = (maty.SERVICE_ID + "__" + Inventec.Common.DateTime.Get.Now() + "__" + Guid.NewGuid().ToString());
                    AssignPrescriptionWorker.Instance.MediMatyCreateWorker.setNumRow();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public MediMatyTypeADO(MOS.EFMODEL.DataModels.HIS_SERVICE_REQ_METY inputData, bool isEdit)
        {
            try
            {
                this.IsEdit = isEdit;
                var mety = BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>().FirstOrDefault(o => o.ID == (inputData.MEDICINE_TYPE_ID ?? 0));
                if (mety != null)
                {
                    Inventec.Common.Mapper.DataObjectMapper.Map<MediMatyTypeADO>(this, mety);

                    if (inputData.IS_SUB_PRES != 1)
                    {
                        this.DataType = HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC_DM;
                    }
                    else
                    {
                        this.DataType = HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC;
                    }
                    this.ID = (inputData.MEDICINE_TYPE_ID ?? 0);
                    this.IsAllowOdd = mety.IS_ALLOW_ODD == 1 ? true : false;
                    this.CONTRAINDICATION = mety.CONTRAINDICATION;
                    this.MEDICINE_GROUP_ID = mety.MEDICINE_GROUP_ID;
                }
                else
                {
                    this.SERVICE_UNIT_NAME = inputData.UNIT_NAME;
                    this.MEDICINE_TYPE_NAME = inputData.MEDICINE_TYPE_NAME;
                    if (inputData.IS_SUB_PRES != 1)
                    {
                        this.DataType = HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC_TUTUC;
                    }
                    else
                    {
                        this.DataType = HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC;
                    }
                }

                this.HTU_TEXT = inputData.HTU_TEXT;
                //Chi dinh tu man hinh phau thuat, thu thuat
                //#18124
                //Bổ sung cấu hình và sửa lại cách tự động hao phí
                //Bổ sung cấu hình: có tự động hao phí khi kê ở PTTT hay không? (mặc định có)
                //Cách tự động hao phí: sửa lại là chỉ hao phí với vật tư có loại dịch vụ bhyt: vật tư trong danh mục.
                //trường hợp vật tư ở danh mục có check tự động hao phí thì luôn check không quan tâm điều kiện khác.
                if ((AssignPrescriptionWorker.Instance.MediMatyCreateWorker.getIsAutoCheckExpend() == true
                    && HisConfigCFG.IsAutoTickExpendWithAssignPresPTTT)
                    || (mety != null && mety.IS_AUTO_EXPEND == GlobalVariables.CommonNumberTrue))
                {
                    this.IsExpend = true;
                }
                this.EXCEED_LIMIT_IN_PRES_REASON = inputData.EXCEED_LIMIT_IN_PRES_REASON;
                this.EXCEED_LIMIT_IN_DAY_REASON = inputData.EXCEED_LIMIT_IN_DAY_REASON;
                this.EXCEED_LIMIT_IN_BATCH_REASON = inputData.EXCEED_LIMIT_IN_TREAT_REASON;
                this.ODD_PRES_REASON = inputData.ODD_PRES_REASON;
                this.UseTimeTo = inputData.USE_TIME_TO;
                this.TUTORIAL = inputData.TUTORIAL;
                this.AMOUNT = inputData.AMOUNT;
                this.PRICE = inputData.PRICE;
                this.IS_SUB_PRES = inputData.IS_SUB_PRES;
                this.PRES_AMOUNT = inputData.PRES_AMOUNT;
                this.PREVIOUS_USING_COUNT = inputData.PREVIOUS_USING_COUNT;
                this.OVER_RESULT_TEST_REASON = inputData.OVER_RESULT_TEST_REASON;
                this.OVER_KIDNEY_REASON = inputData.OVER_KIDNEY_REASON;
                if (this.CONVERT_RATIO.HasValue)
                {
                    this.PRES_AMOUNT = inputData.PRES_AMOUNT * this.CONVERT_RATIO.Value;
                    this.AMOUNT = this.AMOUNT * this.CONVERT_RATIO.Value;
                    this.PRICE = this.PRICE / this.CONVERT_RATIO.Value;
                }

                if (!String.IsNullOrEmpty(inputData.MORNING))
                {
                    this.Sang = ConvertNumber.ProcessNumberInterger(inputData.MORNING);
                }
                if (!String.IsNullOrEmpty(inputData.NOON))
                {
                    this.Trua = ConvertNumber.ProcessNumberInterger(inputData.NOON);
                }
                if (!String.IsNullOrEmpty(inputData.AFTERNOON))
                {
                    this.Chieu = ConvertNumber.ProcessNumberInterger(inputData.AFTERNOON);
                }
                if (!String.IsNullOrEmpty(inputData.EVENING))
                    this.Toi = ConvertNumber.ProcessNumberInterger(inputData.EVENING);
                if (inputData.HTU_ID != null)
                    this.HTU_IDs = new List<long>() { inputData.HTU_ID ?? 0 };

                this.SERVICE_REQ_ID = inputData.SERVICE_REQ_ID;
                this.SERVICE_REQ_METY_MATY_ID = inputData.ID;
                if (isEdit)
                {
                    this.PRE_AMOUNT = this.AMOUNT;
                    this.NUM_ORDER = inputData.NUM_ORDER;
                }
                else
                {
                    this.NUM_ORDER = AssignPrescriptionWorker.Instance.MediMatyCreateWorker.getNumRow();
                }

                if (mety != null)
                {
                    if (mety.LAST_EXP_PRICE.HasValue || mety.LAST_EXP_VAT_RATIO.HasValue)
                    {
                        decimal? priceRaw = this.AMOUNT * (this.PRICE ?? 0) * (1 + (mety.LAST_EXP_VAT_RATIO ?? 0));
                        //priceRaw = (mety.CONVERT_RATIO.HasValue && mety.CONVERT_RATIO > 0) ? priceRaw / mety.CONVERT_RATIO.Value : priceRaw;

                        this.TotalPrice = priceRaw ?? 0;
                    }
                    else
                    {
                        this.TotalPrice = ((this.AMOUNT ?? 0) * (this.PRICE ?? 0)) * (1 + (mety.IMP_VAT_RATIO ?? 0));
                    }
                }
                else
                    this.TotalPrice = (this.PRICE ?? 0) * (this.AMOUNT ?? 0);
                Inventec.Common.Logging.LogSystem.Debug("Create model by HIS_SERVICE_REQ_METY for edit prescription____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => mety), mety)
                    + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => TotalPrice), TotalPrice)
                    + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => AMOUNT), AMOUNT)
                    + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => IMP_VAT_RATIO), IMP_VAT_RATIO)
                    + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => PRICE), PRICE));
                this.PrimaryKey = ((mety != null ? mety.SERVICE_ID : 0) + "__" + Inventec.Common.DateTime.Get.Now() + "__" + Guid.NewGuid().ToString());
                AssignPrescriptionWorker.Instance.MediMatyCreateWorker.setNumRow();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Đơn cũ
        /// </summary>
        /// <param name="inputData"></param>
        /// <param name="currentInstructionTime"></param>
        /// <param name="serviceReq"></param>
        public MediMatyTypeADO(MOS.EFMODEL.DataModels.HIS_SERVICE_REQ_METY inputData, long currentInstructionTime, V_HIS_SERVICE_REQ_7 serviceReq)
        {
            try
            {
                var mety = BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>().FirstOrDefault(o => o.ID == (inputData.MEDICINE_TYPE_ID ?? 0));

                if (mety != null)
                {
                    Inventec.Common.Mapper.DataObjectMapper.Map<MediMatyTypeADO>(this, mety);
                    this.ID = (inputData.MEDICINE_TYPE_ID ?? 0);
                    this.IsAllowOdd = mety.IS_ALLOW_ODD == 1 ? true : false;
                    this.CONTRAINDICATION = mety.CONTRAINDICATION;
                    this.MEDICINE_GROUP_ID = mety.MEDICINE_GROUP_ID;
                    this.PREVIOUS_USING_COUNT = inputData.PREVIOUS_USING_COUNT;
                    if (inputData.IS_SUB_PRES == 1)
                    {
                        this.DataType = HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC;
                        //Lay doi tuong mac dinh
                        MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE patientType = new MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE();
                        patientType = AssignPrescriptionWorker.Instance.MediMatyCreateWorker.choosePatientTypeDefaultlServiceOther(AssignPrescriptionWorker.Instance.MediMatyCreateWorker.getPatientTypeId(), (mety.SERVICE_ID), mety.SERVICE_TYPE_ID);
                        if (patientType != null)
                        {
                            this.PATIENT_TYPE_ID = patientType.ID;
                            this.PATIENT_TYPE_CODE = patientType.PATIENT_TYPE_CODE;
                            this.PATIENT_TYPE_NAME = patientType.PATIENT_TYPE_NAME;
                        }
                        this.IS_SUB_PRES = null;
                    }
                    else
                    {
                        this.DataType = HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC_DM;
                    }
                }
                else
                {
                    this.SERVICE_UNIT_NAME = inputData.UNIT_NAME;
                    this.MEDICINE_TYPE_NAME = inputData.MEDICINE_TYPE_NAME;
                    this.DataType = HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC_TUTUC;
                }
                //Chi dinh tu man hinh phau thuat, thu thuat
                if ((AssignPrescriptionWorker.Instance.MediMatyCreateWorker.getIsAutoCheckExpend() == true
                      && HisConfigCFG.IsAutoTickExpendWithAssignPresPTTT)
                      || (mety != null && mety.IS_AUTO_EXPEND == GlobalVariables.CommonNumberTrue))
                {
                    this.IsExpend = true;
                }
                this.UseTimeTo = inputData.USE_TIME_TO;

                this.HTU_TEXT = inputData.HTU_TEXT;
                if (!String.IsNullOrEmpty(inputData.MORNING))
                    this.Sang = ConvertNumber.ProcessNumberInterger(inputData.MORNING);
                if (!String.IsNullOrEmpty(inputData.NOON))
                    this.Trua = ConvertNumber.ProcessNumberInterger(inputData.NOON);
                if (!String.IsNullOrEmpty(inputData.AFTERNOON))
                    this.Chieu = ConvertNumber.ProcessNumberInterger(inputData.AFTERNOON);
                if (!String.IsNullOrEmpty(inputData.EVENING))
                    this.Toi = ConvertNumber.ProcessNumberInterger(inputData.EVENING);
                if (inputData.HTU_ID != null)
                    this.HTU_IDs = new List<long>() { inputData.HTU_ID ?? 0 };
                this.EXCEED_LIMIT_IN_PRES_REASON = inputData.EXCEED_LIMIT_IN_PRES_REASON;
                this.EXCEED_LIMIT_IN_DAY_REASON = inputData.EXCEED_LIMIT_IN_DAY_REASON;
                this.EXCEED_LIMIT_IN_BATCH_REASON = inputData.EXCEED_LIMIT_IN_TREAT_REASON;
                this.ODD_PRES_REASON = inputData.ODD_PRES_REASON;
                this.TUTORIAL = inputData.TUTORIAL;
                this.AMOUNT = inputData.AMOUNT;
                this.PRICE = inputData.PRICE;
                this.PRES_AMOUNT = inputData.PRES_AMOUNT ?? this.AMOUNT;
                if (this.CONVERT_RATIO.HasValue)
                {
                    this.PRES_AMOUNT = inputData.PRES_AMOUNT * this.CONVERT_RATIO.Value;
                    this.AMOUNT = this.AMOUNT * this.CONVERT_RATIO.Value;
                    this.PRICE = this.PRICE / this.CONVERT_RATIO.Value;
                }
                this.SERVICE_REQ_ID = inputData.SERVICE_REQ_ID;
                this.SERVICE_REQ_METY_MATY_ID = inputData.ID;
                this.TotalPrice = (this.PRICE ?? 0) * (this.AMOUNT ?? 0);
                this.OVER_RESULT_TEST_REASON = inputData.OVER_RESULT_TEST_REASON;
                this.OVER_KIDNEY_REASON = inputData.OVER_KIDNEY_REASON;
                if (serviceReq != null)
                {
                    if (inputData.USE_TIME_TO.HasValue)
                    {
                        DateTime dtUseTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(serviceReq.USE_TIME ?? serviceReq.INTRUCTION_TIME).Value;
                        DateTime dtUseTimeTo = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(inputData.USE_TIME_TO.Value).Value;
                        TimeSpan ts = dtUseTimeTo.Date - dtUseTime.Date;
                        this.UseDays = ts.Days + 1;
                        DateTime dtInstructionTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(currentInstructionTime).Value;
                        this.UseTimeTo = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtInstructionTime.AddDays((double)(ts.Days)));
                    }
                    this.UseTime = serviceReq.USE_TIME;
                }


                if (mety != null)
                {
                    if (mety.LAST_EXP_PRICE.HasValue || mety.LAST_EXP_VAT_RATIO.HasValue)
                    {
                        decimal? priceRaw = this.AMOUNT * (this.PRICE ?? 0) * (1 + (mety.LAST_EXP_VAT_RATIO ?? 0));
                        //priceRaw = (mety.CONVERT_RATIO.HasValue && mety.CONVERT_RATIO > 0) ? priceRaw / mety.CONVERT_RATIO.Value : priceRaw;

                        this.TotalPrice = priceRaw ?? 0;
                    }
                    else
                    {
                        this.TotalPrice = ((this.AMOUNT ?? 0) * (this.PRICE ?? 0)) * (1 + (mety.IMP_VAT_RATIO ?? 0));
                    }
                }
                else
                    this.TotalPrice = (this.PRICE ?? 0) * (this.AMOUNT ?? 0);

                this.NUM_ORDER = AssignPrescriptionWorker.Instance.MediMatyCreateWorker.getNumRow();

                this.PrimaryKey = ((mety != null ? mety.SERVICE_ID : 0) + "__" + Inventec.Common.DateTime.Get.Now() + "__" + Guid.NewGuid().ToString());
                AssignPrescriptionWorker.Instance.MediMatyCreateWorker.setNumRow();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public MediMatyTypeADO(MOS.EFMODEL.DataModels.HIS_SERVICE_REQ_MATY inputData, bool isEdit)
        {
            try
            {
                this.IsEdit = isEdit;
                var maty = BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>().FirstOrDefault(o => o.ID == inputData.MATERIAL_TYPE_ID);
                if (maty != null)
                {
                    Inventec.Common.Mapper.DataObjectMapper.Map<MediMatyTypeADO>(this, maty);
                    this.ID = (inputData.MATERIAL_TYPE_ID ?? 0);
                    this.MEDICINE_TYPE_CODE = maty.MATERIAL_TYPE_CODE;
                    this.MEDICINE_TYPE_NAME = maty.MATERIAL_TYPE_NAME;
                    this.IsAllowOdd = maty.IS_ALLOW_ODD == 1 ? true : false;
                }
                else
                {
                    this.SERVICE_UNIT_NAME = inputData.UNIT_NAME;
                    this.MEDICINE_TYPE_NAME = inputData.MATERIAL_TYPE_NAME;
                }
                this.HTU_TEXT = inputData.HTU_TEXT;
                this.SERVICE_REQ_ID = inputData.SERVICE_REQ_ID;
                this.SERVICE_REQ_METY_MATY_ID = inputData.ID;
                this.TUTORIAL = inputData.TUTORIAL;
                this.EXCEED_LIMIT_IN_PRES_REASON = inputData.EXCEED_LIMIT_IN_PRES_REASON;
                this.EXCEED_LIMIT_IN_DAY_REASON = inputData.EXCEED_LIMIT_IN_DAY_REASON;
                if (inputData.IS_SUB_PRES != 1)
                {
                    this.DataType = HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.VATTU_DM;
                }
                else
                {
                    this.DataType = HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.VATTU;
                    this.IS_SUB_PRES = null;

                    //Lay doi tuong mac dinh
                    MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE patientType = new MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE();
                    patientType = AssignPrescriptionWorker.Instance.MediMatyCreateWorker.choosePatientTypeDefaultlServiceOther(AssignPrescriptionWorker.Instance.MediMatyCreateWorker.getPatientTypeId(), (maty.SERVICE_ID), maty.SERVICE_TYPE_ID);
                    if (patientType != null)
                    {
                        this.PATIENT_TYPE_ID = patientType.ID;
                        this.PATIENT_TYPE_CODE = patientType.PATIENT_TYPE_CODE;
                        this.PATIENT_TYPE_NAME = patientType.PATIENT_TYPE_NAME;
                    }
                }

                this.AMOUNT = inputData.AMOUNT;
                this.PRICE = inputData.PRICE;
                this.PRES_AMOUNT = inputData.PRES_AMOUNT ?? this.AMOUNT;
                if (this.CONVERT_RATIO.HasValue)
                {

                    this.PRES_AMOUNT = inputData.PRES_AMOUNT * this.CONVERT_RATIO.Value;
                    this.AMOUNT = this.AMOUNT * this.CONVERT_RATIO.Value;
                    this.PRICE = this.PRICE / this.CONVERT_RATIO.Value;
                }
                if (this.IS_ALLOW_ODD == 1 && this.IS_ALLOW_EXPORT_ODD != 1)
                {
                    this.AMOUNT = (decimal)Inventec.Common.Number.Convert.RoundUpValue((double)this.AMOUNT, 0);
                }
                if (maty != null)
                {
                    if (maty.LAST_EXP_PRICE.HasValue || maty.LAST_EXP_VAT_RATIO.HasValue)
                    {
                        decimal? priceRaw = this.AMOUNT * (this.PRICE ?? 0) * (1 + (maty.LAST_EXP_VAT_RATIO ?? 0));
                        //priceRaw = (maty.CONVERT_RATIO.HasValue && maty.CONVERT_RATIO > 0) ? priceRaw / maty.CONVERT_RATIO.Value : priceRaw;

                        this.TotalPrice = priceRaw ?? 0;
                    }
                    else
                    {
                        this.TotalPrice = ((this.AMOUNT ?? 0) * (this.PRICE ?? 0)) * (1 + (maty.IMP_VAT_RATIO ?? 0));
                    }
                }
                else
                    this.TotalPrice = (inputData.PRICE ?? 0) * (this.AMOUNT ?? 0);

                if (isEdit)
                {
                    this.PRE_AMOUNT = inputData.AMOUNT;
                    this.NUM_ORDER = inputData.NUM_ORDER;
                }
                else
                {
                    this.NUM_ORDER = AssignPrescriptionWorker.Instance.MediMatyCreateWorker.getNumRow();
                }
                if (((AssignPrescriptionWorker.Instance.MediMatyCreateWorker.getIsAutoCheckExpend() == true && (this.HEIN_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__VT_TDM || this.HEIN_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__VT_NDM))
                   && HisConfigCFG.IsAutoTickExpendWithAssignPresPTTT)
                 || (maty != null && maty.IS_AUTO_EXPEND == GlobalVariables.CommonNumberTrue))
                {
                    this.IsExpend = true;
                }

                this.PrimaryKey = ((maty != null ? maty.SERVICE_ID : 0) + "__" + Inventec.Common.DateTime.Get.Now() + "__" + Guid.NewGuid().ToString());
                AssignPrescriptionWorker.Instance.MediMatyCreateWorker.setNumRow();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public MediMatyTypeADO(MediMatyTypeADO s)
        {
            try
            {
                System.Reflection.PropertyInfo[] pi = Inventec.Common.Repository.Properties.Get<MediMatyTypeADO>();
                foreach (var item in pi)
                {
                    item.SetValue(this, (item.GetValue(s)));
                }
                this.MedicineBean1Result = s.MedicineBean1Result;
                this.MaterialBean1Result = s.MaterialBean1Result;
                this.BeanIds = s.BeanIds;
                this.ExpMestDetailIds = s.ExpMestDetailIds;
                this.CONTRAINDICATION = s.CONTRAINDICATION;
                this.IS_OUT_HOSPITAL = s.IS_OUT_HOSPITAL;
                this.MEDICINE_GROUP_ID = s.MEDICINE_GROUP_ID;
                this.HTU_IDs = s.HTU_IDs;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void UpdateMedicineUseFormInDataRow(MediMatyTypeADO medicineTypeSDO)
        {
            try
            {
                bool hasUseForm = false;
                if (medicineTypeSDO.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC)
                {
                    V_HIS_MEDICINE_TYPE mety = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_MEDICINE_TYPE>().FirstOrDefault(o => o.SERVICE_ID == medicineTypeSDO.SERVICE_ID);
                    if (mety != null)
                    {
                        MOS.EFMODEL.DataModels.HIS_MEDICINE_USE_FORM useForm = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_MEDICINE_USE_FORM>().FirstOrDefault(o => o.ID == mety.MEDICINE_USE_FORM_ID);
                        if (useForm != null)
                        {
                            medicineTypeSDO.MEDICINE_USE_FORM_ID = useForm.ID;
                            medicineTypeSDO.MEDICINE_USE_FORM_CODE = useForm.MEDICINE_USE_FORM_CODE;
                            medicineTypeSDO.MEDICINE_USE_FORM_NAME = useForm.MEDICINE_USE_FORM_NAME;
                            hasUseForm = true;
                        }
                        medicineTypeSDO.MEDICINE_GROUP_ID = mety.MEDICINE_GROUP_ID;
                    }
                }
                if (!hasUseForm)
                {
                    medicineTypeSDO.MEDICINE_USE_FORM_ID = null;
                    medicineTypeSDO.MEDICINE_USE_FORM_CODE = "";
                    medicineTypeSDO.MEDICINE_USE_FORM_NAME = "";
                    medicineTypeSDO.ErrorMessageMedicineUseForm = "";
                    medicineTypeSDO.ErrorTypeMedicineUseForm = ErrorType.None;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal void UpdateAutoRoundUpByConvertUnitRatioInDataRow(MediMatyTypeADO medicineTypeSDO, V_HIS_TREATMENT treatmentView)
        {
            try
            {
                if (medicineTypeSDO.CONVERT_RATIO.HasValue && medicineTypeSDO.CONVERT_RATIO.Value > 1)
                {
                    if ((HisConfigCFG.IsAutoRoundUpByConvertUnitRatio && (medicineTypeSDO.IsUseOrginalUnitForPres ?? false) == false) || (HisConfigCFG.IsShowPresAmount && treatmentView != null && treatmentView.TDL_TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__KHAM))
                    {
                        {
                            decimal? presAmountTmp = medicineTypeSDO.PRES_AMOUNT ?? medicineTypeSDO.AMOUNT;
                            medicineTypeSDO.PRES_AMOUNT = presAmountTmp;
                            decimal amount = (Math.Ceiling((medicineTypeSDO.AMOUNT ?? 0) / medicineTypeSDO.CONVERT_RATIO.Value)) * (medicineTypeSDO.CONVERT_RATIO.Value);
                            if (amount != medicineTypeSDO.AMOUNT)
                            {
                                Inventec.Common.Logging.LogSystem.Debug("Ke don co cau hinh IsAutoRoundUpByConvertUnitRatio || IsShowPresAmount & Khám & CONVERT_RATIO > 1 & so luong lam tron theo don vi quy doi khac so luong ke____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => medicineTypeSDO.MEDICINE_TYPE_NAME), medicineTypeSDO.MEDICINE_TYPE_NAME) + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => medicineTypeSDO.MEDICINE_TYPE_CODE), medicineTypeSDO.MEDICINE_TYPE_CODE) + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => medicineTypeSDO.AMOUNT), medicineTypeSDO.AMOUNT) + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => amount), amount) + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => medicineTypeSDO.CONVERT_RATIO), medicineTypeSDO.CONVERT_RATIO));
                                medicineTypeSDO.AMOUNT = amount;
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

        internal void UpdateAmountAutoRoundUpByAllowOddInDataRow(MediMatyTypeADO medicineTypeSDO)
        {
            try
            {
                if (medicineTypeSDO.IsAllowOdd == true
                    && !GlobalStore.IsTreatmentIn && !GlobalStore.IsCabinet
                    )
                {
                    decimal amount = Math.Ceiling(medicineTypeSDO.AMOUNT ?? 0);
                    if (amount != medicineTypeSDO.AMOUNT)
                    {
                        Inventec.Common.Logging.LogSystem.Debug("Ke don thuoc/vt " + medicineTypeSDO.MEDICINE_TYPE_CODE + " - " + medicineTypeSDO.MEDICINE_TYPE_NAME + " co IsAllowOdd=" + medicineTypeSDO.IsAllowOdd + ", IsAllowOddAndExportOdd=" + (medicineTypeSDO.IsAllowOddAndExportOdd ?? false) + " ===> tu dong lam tron so luong thanh so nguyen sau khi nhan bo sung khi so luong ke le" + "____so luong cu:AMOUNT=" + medicineTypeSDO.AMOUNT + ", so luong moi amount=" + amount);
                        medicineTypeSDO.AMOUNT = amount;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal decimal CalculateAmount(MediMatyTypeADO item, int ActionType)
        {
            decimal result = 0;
            if (
                (HisConfigCFG.SplitOffset == GlobalVariables.CommonStringTrue || (item.IS_SPLIT_COMPENSATION.HasValue && item.IS_SPLIT_COMPENSATION == 1))
                && ((item.IS_ALLOW_ODD ?? 0) != 1)
                && (item.AMOUNT != (int)item.AMOUNT || (item.RAW_AMOUNT != null && item.RAW_AMOUNT != (int)item.RAW_AMOUNT))
                && (item.RAW_AMOUNT ?? 0) > 0
                && (item.RAW_AMOUNT ?? 0) < (item.AMOUNT ?? 0))
            {
                result = ((item.IsUseOrginalUnitForPres ?? false) == false && (item.CONVERT_RATIO ?? 0) > 0) ? (item.RAW_AMOUNT ?? 0) / (item.CONVERT_RATIO ?? 1) : (item.RAW_AMOUNT ?? 0);
            }
            else
            {
                result = ((item.IsUseOrginalUnitForPres ?? false) == false && (item.CONVERT_RATIO ?? 0) > 0) ? (item.AMOUNT ?? 0) / (item.CONVERT_RATIO ?? 1) : (item.AMOUNT ?? 0);
            }

            if ((item.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.VATTU || item.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.VATTU_DM)
                && (item.IS_ALLOW_ODD == 1 && item.IS_ALLOW_EXPORT_ODD != 1)
                && result != (int)result)
            {
                result = (decimal)Inventec.Common.Number.Convert.RoundUpValue((double)result, 0);
            }

            return result;
        }

        internal decimal CalculateAmountNoPres(MediMatyTypeADO item, int ActionType)
        {
            decimal result = 0;
            if (
                (HisConfigCFG.SplitOffset == GlobalVariables.CommonStringTrue || (item.IS_SPLIT_COMPENSATION.HasValue && item.IS_SPLIT_COMPENSATION == 1))
                && ((item.IS_ALLOW_ODD ?? 0) != 1)
                && (item.RAW_AMOUNT ?? 0) > 0
                && (item.AMOUNT ?? 0) > (item.RAW_AMOUNT ?? 0))
            {
                decimal amountDiv = (item.AMOUNT ?? 0) - (item.RAW_AMOUNT ?? 0);
                result = ((item.IsUseOrginalUnitForPres ?? false) == false && (item.CONVERT_RATIO ?? 0) > 0) ? amountDiv / (item.CONVERT_RATIO ?? 1) : amountDiv;
            }
            Inventec.Common.Logging.LogSystem.Debug("CalculateAmountNoPres____"
                + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => HisConfigCFG.SplitOffset), HisConfigCFG.SplitOffset)
                + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => ActionType), ActionType)
                + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => item), item)
                + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => result), result));
            return result;
        }
        public Dictionary<long, List<long>> dicUseTimeBeanIds { get; set; }
        public long? SERVICE_CONDITION_ID { get; set; }
        public string SERVICE_CONDITION_NAME { get; set; }
        public short? IS_NOT_PRES { get; set; }
        public string BREATH_SPEED { get; set; }
        public string BREATH_TIME { get; set; }
        public long? MATERIAL_TYPE_MAP_ID { get; set; }
        public string MATERIAL_TYPE_MAP_CODE { get; set; }
        public string MATERIAL_TYPE_MAP_NAME { get; set; }
        public bool IsMultiDateState { get; set; }
        public List<long> IntructionTimeSelecteds { get; set; }
        public bool? IsUseOrginalUnitForPres { get; set; }
        public bool? IsKidneyShift { get; set; }
        public decimal? KidneyShiftCount { get; set; }
        public long? MAX_REUSE_COUNT { get; set; }//Số lần sử dụng tối đa
        public long? USE_COUNT { get; set; }//Số lần sử dụng
        public long? USE_REMAIN_COUNT { get; set; }//Số lần sử dụng còn lại
        public string SERIAL_NUMBER { get; set; }
        public bool? IsNotTakeBean { get; set; }
        public bool? IsStent { get; set; }
        public List<long> ExpMestDetailIds { get; set; }
        public decimal? PRICE { get; set; }
        public List<long> BeanIds { get; set; }
        public string PrimaryKey { get; set; }
        public int DataType { get; set; }
        public List<MOS.EFMODEL.DataModels.V_HIS_MEDICINE_BEAN_1> MedicineBean1Result { get; set; }
        public List<MOS.EFMODEL.DataModels.V_HIS_MATERIAL_BEAN_1> MaterialBean1Result { get; set; }
        public decimal? Speed { get; set; }
        public bool IsExpendType { get; set; }
        public long? PATIENT_TYPE_ID { get; set; }
        public string PATIENT_TYPE_CODE { get; set; }
        public string PATIENT_TYPE_NAME { get; set; }
        public decimal? AMOUNT { get; set; }
        public decimal? BK_AMOUNT { get; set; }
        public decimal? PRE_AMOUNT { get; set; }
        public decimal? RAW_AMOUNT { get; set; }
        public bool IsExpend { get; set; }
        public bool IsKHBHYT { get; set; }
        public bool IsOutParentFee { get; set; }
        public bool? IsOutKtcFee { get; set; }
        public bool IsAssignDay { get; set; }
        public bool? IsAllowOdd { get; set; }
        public bool? IsAllowOddAndExportOdd { get; set; }
        public decimal? AmountAlert { get; set; }
        public string Sang { get; set; }
        public string Trua { get; set; }
        public string Chieu { get; set; }
        public string Toi { get; set; }
        public List<long> HTU_IDs { get; set; }
        public long? HTU_ID_NOT_CHECK_ACIN_INTERACTIVE { get; set; }
        public long? UseTimeTo { get; set; }
        public decimal? UseDays { get; set; }
        public decimal TotalPrice { get; set; }
        public long? MEDI_STOCK_ID { get; set; }
        public string MEDI_STOCK_CODE { get; set; }
        public string MEDI_STOCK_NAME { get; set; }
        public decimal? TDL_MEDICINE_IMP_PRICE { get; set; }
        public long SERVICE_REQ_ID { get; set; }
        public long SERVICE_REQ_METY_MATY_ID { get; set; }
        public bool IsEdit { get; set; }
        public long? EQUIPMENT_SET_ID { get; set; }
        public long? SereServParentId { get; set; }
        public string TDL_PACKAGE_NUMBER { get; set; }
        public long? EXPIRED_DATE { get; set; }
        public bool? IsAssignPackage { get; set; }
        public long? MAME_ID { get; set; }
        public long? PREVIOUS_USING_COUNT { get; set; }
        public long? OTHER_PAY_SOURCE_ID { get; set; }
        public string OTHER_PAY_SOURCE_CODE { get; set; }
        public string OTHER_PAY_SOURCE_NAME { get; set; }
        public short? IS_SUB_PRES { get; set; }
        public long? MIXED_INFUSION { get; set; }
        public short? IS_MIXED_MAIN { get; set; }
        public string TUTORIAL_INFUSION { get; set; }
        public long? EXP_MEST_REASON_ID { get; set; }
        public string EXP_MEST_REASON_CODE { get; set; }
        public string EXP_MEST_REASON_NAME { get; set; }
        public long? UseTime { get; set; }
        public long IdRow { get; set; }
        public decimal? PRES_AMOUNT { get; set; }
        public short? IS_OUT_MEDI_STOCK { get; set; }
        public bool IsDisableExpend { get; set; }
        public bool IsNotOutStock { get; set; }
        public string EXCEED_LIMIT_IN_PRES_REASON { get; set; }
        public string EXCEED_LIMIT_IN_DAY_REASON { get; set; }
        public string EXCEED_LIMIT_IN_BATCH_REASON { get; set; }
        public string PATIENT_NAME_BY_TREATMENT_CODE { get; set; }
        public bool IsAlertInTreatPresciption { get; set; }
        public decimal? NUMBER_EXCEED_IN_TREATMENT { get; set; }
        public decimal? NUMBER_PRESCIPTION_IN_TREATMENT { get; set; }
        public string ODD_PRES_REASON { get; set; }
        public List<string> IcdsWarning { get; set; }
        public bool IsWarned { get; set; }
        public string OVER_RESULT_TEST_REASON { get; set; }
        public Dictionary<long, List<TreatmentOverReason>> dicTreatmentOverResultTestReason { get; set; }
        public bool IsEditOverResultTestReason { get; set; }
        public string OVER_KIDNEY_REASON { get; set; }
        public long? OVER_REASON_ID { get; set; }
        public Dictionary<long, List<TreatmentOverReason>> dicTreatmentOverKidneyReason { get; set; }
        public bool IsEditOverKidneyReason { get; set; }
        public bool IsNoPrescription { get; set; }
        public long IdRowPopupGrid { get; set; }
        public long IntructionTime { get; set; }
        public short? IS_IDENTITY_MANAGEMENT { get; set; }
        public short? IS_REUSABLE { get; set; }
        public string HTU_TEXT { get; set; }
        public DevExpress.XtraEditors.DXErrorProvider.ErrorType ErrorTypeAmountHasRound { get; set; }
        public string ErrorMessageAmountHasRound { get; set; }
        public DevExpress.XtraEditors.DXErrorProvider.ErrorType ErrorTypeAmount { get; set; }
        public bool IsNotShowMessage { get; set; }
        public string ErrorMessageAmount { get; set; }
        public DevExpress.XtraEditors.DXErrorProvider.ErrorType ErrorTypePatientTypeId { get; set; }
        public string ErrorMessagePatientTypeId { get; set; }
        public DevExpress.XtraEditors.DXErrorProvider.ErrorType ErrorTypeMedicineUseForm { get; set; }
        public string ErrorMessageMedicineUseForm { get; set; }
        public DevExpress.XtraEditors.DXErrorProvider.ErrorType ErrorTypeIsAssignDay { get; set; }
        public string ErrorMessageIsAssignDay { get; set; }
        public DevExpress.XtraEditors.DXErrorProvider.ErrorType ErrorTypeAmountAlert { get; set; }
        public string ErrorMessageAmountAlert { get; set; }
        public DevExpress.XtraEditors.DXErrorProvider.ErrorType ErrorTypeMediMatyBean { get; set; }
        public string ErrorMessageMediMatyBean { get; set; }
        public DevExpress.XtraEditors.DXErrorProvider.ErrorType ErrorTypeTutorial { get; set; }
        public string ErrorMessageTutorial { get; set; }
        public DevExpress.XtraEditors.DXErrorProvider.ErrorType ErrorTypeOddPres { get; set; }
        public string ErrorMessageOddPres { get; set; }
        public DevExpress.XtraEditors.DXErrorProvider.ErrorType ErrorTypeOverResultTestReason { get; set; }
        public string ErrorMessageOverResultTestReason { get; set; }
        public DevExpress.XtraEditors.DXErrorProvider.ErrorType ErrorTypeOverKidneyReason { get; set; }
        public string ErrorMessageOverKidneyReason { get; set; }
    }
}

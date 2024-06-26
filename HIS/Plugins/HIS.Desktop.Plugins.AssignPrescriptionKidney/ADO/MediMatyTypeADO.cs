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
using HIS.Desktop.Plugins.AssignPrescriptionKidney.Config;
using HIS.Desktop.Plugins.AssignPrescriptionKidney.Resources;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.AssignPrescriptionKidney.ADO
{
    public class
        MediMatyTypeADO : MOS.EFMODEL.DataModels.V_HIS_MEDICINE_TYPE
    {
        public MediMatyTypeADO()
        {

        }

        public MediMatyTypeADO(MOS.EFMODEL.DataModels.V_HIS_MEDICINE_TYPE medicineType, long currentInstructionTime, HIS_SERVICE_REQ serviceReq, HIS_SERE_SERV sereServ)
        {
            try
            {
                Inventec.Common.Mapper.DataObjectMapper.Map<MediMatyTypeADO>(this, medicineType);
                this.DataType = HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC;
                this.AMOUNT = sereServ.AMOUNT / (serviceReq.KIDNEY_TIMES ?? 1);
                if (medicineType.CONVERT_RATIO.HasValue)
                {
                    this.AMOUNT = this.AMOUNT * medicineType.CONVERT_RATIO.Value;
                    this.PRICE = this.PRICE / medicineType.CONVERT_RATIO.Value;
                }
                this.IsExpend = ((sereServ.IS_EXPEND ?? 0) == GlobalVariables.CommonNumberTrue ? true : false);
                this.IsAllowOdd = medicineType.IS_ALLOW_ODD == 1 ? true : false;
                this.IsKHBHYT = false;
                this.NUM_ORDER = AssignPrescriptionWorker.MediMatyCreateWorker.getNumRow();

                if (serviceReq != null)
                {
                    if (serviceReq.USE_TIME_TO.HasValue)
                    {
                        DateTime dtUseTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(serviceReq.INTRUCTION_TIME).Value;
                        DateTime dtUseTimeTo = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(serviceReq.USE_TIME_TO.Value).Value;
                        TimeSpan ts = dtUseTimeTo.Date - dtUseTime.Date;
                        this.UseDays = ts.Days + 1;
                        DateTime dtInstructionTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(currentInstructionTime).Value;
                        this.UseTimeTo = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtInstructionTime.AddDays((double)(ts.Days)));
                    }
                }

                //Lay doi tuong mac dinh
                MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE patientType = new MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE();
                patientType = AssignPrescriptionWorker.MediMatyCreateWorker.choosePatientTypeDefaultlService(AssignPrescriptionWorker.MediMatyCreateWorker.getPatientTypeId(), medicineType.SERVICE_ID, medicineType.SERVICE_TYPE_ID);
                if (patientType != null)
                {
                    this.PATIENT_TYPE_ID = patientType.ID;
                    this.PATIENT_TYPE_CODE = patientType.PATIENT_TYPE_CODE;
                    this.PATIENT_TYPE_NAME = patientType.PATIENT_TYPE_NAME;
                }

                //Kiem tra xem thuoc do co trong kho??
                AssignPrescriptionWorker.MediMatyCreateWorker.setDefaultMediStockForData(this);

                this.TotalPrice = (this.AMOUNT ?? 0 * this.PRICE ?? 0);
                UpdateMedicineUseFormInDataRow(this);

                //if ((this.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC)
                //    && this.PATIENT_TYPE_ID == HisConfigCFG.PatientTypeId__BHYT
                //    && String.IsNullOrEmpty(this.TUTORIAL.Trim()))
                //{
                //    this.ErrorMessageTutorial = ResourceMessage.DoiTuongBHYTBatBuocPhaiNhapHDSD;
                //    this.ErrorTypeTutorial = ErrorType.Warning;
                //}
                //else
                //{
                //    this.ErrorMessageTutorial = "";
                //    this.ErrorTypeTutorial = ErrorType.None;
                //}

                this.PrimaryKey = (medicineType.SERVICE_ID + "__" + Inventec.Common.DateTime.Get.Now() + "__" + Guid.NewGuid().ToString());

                AssignPrescriptionWorker.MediMatyCreateWorker.setNumRow();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public MediMatyTypeADO(MOS.EFMODEL.DataModels.V_HIS_MATERIAL_TYPE materialType)
        {
            try
            {
                Inventec.Common.Mapper.DataObjectMapper.Map<MediMatyTypeADO>(this, materialType);
                this.MEDICINE_TYPE_CODE = materialType.MATERIAL_TYPE_CODE;
                this.MEDICINE_TYPE_NAME = materialType.MATERIAL_TYPE_NAME;
                this.IS_CHEMICAL_SUBSTANCE = materialType.IS_CHEMICAL_SUBSTANCE;
                this.IsStent = ((materialType.IS_STENT ?? 0) == GlobalVariables.CommonNumberTrue ? true : false);
                this.IsAllowOdd = materialType.IS_ALLOW_ODD == 1 ? true : false;
                if (materialType.CONVERT_RATIO.HasValue)
                {
                    this.AMOUNT = this.AMOUNT * materialType.CONVERT_RATIO.Value;
                    this.PRICE = this.PRICE / materialType.CONVERT_RATIO.Value;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public MediMatyTypeADO(MOS.EFMODEL.DataModels.V_HIS_EMTE_MEDICINE_TYPE inputData, long? intructionTime)
        {
            try
            {
                var mety = BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>().FirstOrDefault(o => o.ID == inputData.MEDICINE_TYPE_ID);
                if (mety != null)
                {
                    Inventec.Common.Mapper.DataObjectMapper.Map<MediMatyTypeADO>(this, mety);

                    this.SERVICE_UNIT_ID = (inputData.SERVICE_UNIT_ID ?? 0);
                    this.SERVICE_UNIT_CODE = inputData.SERVICE_UNIT_CODE;
                    this.SERVICE_UNIT_NAME = inputData.SERVICE_UNIT_NAME;
                    this.TUTORIAL = (String.IsNullOrEmpty(inputData.TUTORIAL) ? mety.TUTORIAL : inputData.TUTORIAL);
                    this.AMOUNT = inputData.AMOUNT;
                    if (inputData.CONVERT_RATIO.HasValue)
                    {
                        this.AMOUNT = this.AMOUNT * inputData.CONVERT_RATIO.Value;
                    }
                    this.IsExpend = ((inputData.IS_EXPEND ?? 0) == GlobalVariables.CommonNumberTrue ? true : false);
                    this.IsAllowOdd = mety.IS_ALLOW_ODD == 1 ? true : false;
                    //this.MEDICINE_USE_FORM_ID = inputData.MEDICINE_USE_FORM_ID;
                    //this.MEDICINE_USE_FORM_CODE = inputData.MEDICINE_USE_FORM_CODE;
                    //this.MEDICINE_USE_FORM_NAME = inputData.MEDICINE_USE_FORM_NAME;
                    this.IsKHBHYT = false;
                    this.NUM_ORDER = AssignPrescriptionWorker.MediMatyCreateWorker.getNumRow();

                    if (intructionTime.HasValue && inputData.DAY_COUNT.HasValue)
                    {
                        this.UseDays = inputData.DAY_COUNT;
                        DateTime dtIntructionTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(intructionTime.Value).Value;
                        DateTime dtUseTimeTo = dtIntructionTime.AddDays((double)this.UseDays - 1);
                        this.UseTimeTo = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtUseTimeTo);
                    }
                    else
                    {
                        this.UseDays = 1;
                    }

                    //this.UseDays = inputData.DAY_COUNT - 1;
                    if (!String.IsNullOrEmpty(inputData.MORNING))
                        this.Sang = Inventec.Common.TypeConvert.Parse.ToDouble(inputData.MORNING);
                    if (!String.IsNullOrEmpty(inputData.NOON))
                        this.Trua = Inventec.Common.TypeConvert.Parse.ToDouble(inputData.NOON);
                    if (!String.IsNullOrEmpty(inputData.AFTERNOON))
                        this.Chieu = Inventec.Common.TypeConvert.Parse.ToDouble(inputData.AFTERNOON);
                    if (!String.IsNullOrEmpty(inputData.EVENING))
                        this.Toi = Inventec.Common.TypeConvert.Parse.ToDouble(inputData.EVENING);
                    //Chi dinh tu man hinh phau thuat, thu thuat

                    //if (this.isAutoCheckExpend == true
                    //    && this.HEIN_SERVICE_TYPE_ID != HisHeinServiceTypeCFG.HisHeinServiceTypeId__MaterialReplace)
                    //{
                    //    this.IsExpend = true;
                    //}

                    if (inputData.IS_OUT_MEDI_STOCK == GlobalVariables.CommonNumberTrue)
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
                        patientType = AssignPrescriptionWorker.MediMatyCreateWorker.choosePatientTypeDefaultlService(AssignPrescriptionWorker.MediMatyCreateWorker.getPatientTypeId(), (inputData.SERVICE_ID ?? 0), mety.SERVICE_TYPE_ID);
                        if (patientType != null)
                        {
                            this.PATIENT_TYPE_ID = patientType.ID;
                            this.PATIENT_TYPE_CODE = patientType.PATIENT_TYPE_CODE;
                            this.PATIENT_TYPE_NAME = patientType.PATIENT_TYPE_NAME;
                        }

                        this.DataType = HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC;
                        this.ID = (inputData.MEDICINE_TYPE_ID ?? 0);
                        //Kiem tra xem thuoc do co trong kho??
                        AssignPrescriptionWorker.MediMatyCreateWorker.setDefaultMediStockForData(this);
                    }
                    this.PrimaryKey = ((inputData.SERVICE_ID ?? 0) + "__" + Inventec.Common.DateTime.Get.Now() + "__" + Guid.NewGuid().ToString());
                    AssignPrescriptionWorker.MediMatyCreateWorker.setNumRow();
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

        public MediMatyTypeADO(MOS.EFMODEL.DataModels.V_HIS_EMTE_MATERIAL_TYPE inputData)
        {
            try
            {
                var maty = BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>().FirstOrDefault(o => o.ID == inputData.MATERIAL_TYPE_ID);
                if (maty != null)
                {
                    Inventec.Common.Mapper.DataObjectMapper.Map<MediMatyTypeADO>(this, maty);
                    this.DataType = HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.VATTU;
                    this.ID = (inputData.MATERIAL_TYPE_ID ?? 0);
                    this.MEDICINE_TYPE_CODE = inputData.MATERIAL_TYPE_CODE;
                    this.MEDICINE_TYPE_NAME = inputData.MATERIAL_TYPE_NAME;
                    this.SERVICE_UNIT_ID = (inputData.SERVICE_UNIT_ID ?? 0);
                    this.SERVICE_UNIT_NAME = inputData.SERVICE_UNIT_NAME;
                    this.SERVICE_UNIT_CODE = inputData.SERVICE_UNIT_CODE;
                    this.AMOUNT = inputData.AMOUNT;
                    if (inputData.CONVERT_RATIO.HasValue)
                    {
                        this.AMOUNT = this.AMOUNT * inputData.CONVERT_RATIO.Value;
                    }
                    this.TUTORIAL = "";
                    this.UseDays = 1;
                    this.MEDICINE_USE_FORM_ID = null;
                    this.MEDICINE_USE_FORM_NAME = "";
                    this.IsExpend = ((inputData.IS_EXPEND ?? 0) == GlobalVariables.CommonNumberTrue ? true : false);
                    this.IsStent = ((maty.IS_STENT ?? 0) == GlobalVariables.CommonNumberTrue ? true : false);
                    this.IsAllowOdd = maty.IS_ALLOW_ODD == 1 ? true : false;
                    this.IsKHBHYT = false;
                    this.NUM_ORDER = AssignPrescriptionWorker.MediMatyCreateWorker.getNumRow();



                    //Chi dinh tu man hinh phau thuat, thu thuat
                    //if (this.isAutoCheckExpend == true
                    //    && this.HEIN_SERVICE_TYPE_ID != HisHeinServiceTypeCFG.HisHeinServiceTypeId__MaterialReplace)
                    //{
                    //    this.IsExpend = true;
                    //}

                    if (inputData.IS_OUT_MEDI_STOCK == GlobalVariables.CommonNumberTrue)
                    {
                        this.DataType = HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.VATTU_DM;
                        this.ID = (inputData.MATERIAL_TYPE_ID ?? 0);
                    }
                    else
                    {
                        //Lay doi tuong mac dinh
                        MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE patientType = new MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE();
                        patientType = AssignPrescriptionWorker.MediMatyCreateWorker.choosePatientTypeDefaultlService(AssignPrescriptionWorker.MediMatyCreateWorker.getPatientTypeId(), (inputData.SERVICE_ID ?? 0), maty.SERVICE_TYPE_ID);
                        if (patientType != null)
                        {
                            this.PATIENT_TYPE_ID = patientType.ID;
                            this.PATIENT_TYPE_CODE = patientType.PATIENT_TYPE_CODE;
                            this.PATIENT_TYPE_NAME = patientType.PATIENT_TYPE_NAME;
                        }

                        this.DataType = HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.VATTU;
                        this.ID = (inputData.MATERIAL_TYPE_ID ?? 0);
                        //Kiem tra xem thuoc do co trong kho??
                        AssignPrescriptionWorker.MediMatyCreateWorker.setDefaultMediStockForData(this);
                    }

                    this.PrimaryKey = ((inputData.SERVICE_ID ?? 0) + "__" + Inventec.Common.DateTime.Get.Now() + "__" + Guid.NewGuid().ToString());
                    AssignPrescriptionWorker.MediMatyCreateWorker.setNumRow();
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
                    this.DataType = HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.VATTU;
                    this.ID = inputData.MATERIAL_TYPE_ID;
                    this.MEDICINE_TYPE_CODE = inputData.MATERIAL_TYPE_CODE;
                    this.MEDICINE_TYPE_NAME = inputData.MATERIAL_TYPE_NAME;
                    this.SERVICE_UNIT_ID = inputData.SERVICE_UNIT_ID;
                    this.SERVICE_UNIT_NAME = inputData.SERVICE_UNIT_NAME;
                    this.SERVICE_UNIT_CODE = inputData.SERVICE_UNIT_CODE;
                    this.AMOUNT = inputData.AMOUNT;
                    this.TUTORIAL = "";
                    this.MEDICINE_USE_FORM_ID = null;
                    this.MEDICINE_USE_FORM_NAME = "";
                    this.IsStent = ((maty.IS_STENT ?? 0) == GlobalVariables.CommonNumberTrue ? true : false);
                    this.IsAllowOdd = maty.IS_ALLOW_ODD == 1 ? true : false;
                    this.IsKHBHYT = false;
                    this.NUM_ORDER = AssignPrescriptionWorker.MediMatyCreateWorker.getNumRow();
                    this.EQUIPMENT_SET_ID = inputData.EQUIPMENT_SET_ID;

                    //Lay doi tuong mac dinh
                    MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE patientType = new MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE();
                    patientType = AssignPrescriptionWorker.MediMatyCreateWorker.choosePatientTypeDefaultlService(AssignPrescriptionWorker.MediMatyCreateWorker.getPatientTypeId(), (inputData.SERVICE_ID), maty.SERVICE_TYPE_ID);
                    if (patientType != null)
                    {
                        this.PATIENT_TYPE_ID = patientType.ID;
                        this.PATIENT_TYPE_CODE = patientType.PATIENT_TYPE_CODE;
                        this.PATIENT_TYPE_NAME = patientType.PATIENT_TYPE_NAME;
                    }
                    //Kiem tra xem thuoc do co trong kho??
                    AssignPrescriptionWorker.MediMatyCreateWorker.setDefaultMediStockForData(this);

                    this.PrimaryKey = ((inputData.SERVICE_ID) + "__" + Inventec.Common.DateTime.Get.Now() + "__" + Guid.NewGuid().ToString());
                    AssignPrescriptionWorker.MediMatyCreateWorker.setNumRow();
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

        public MediMatyTypeADO(MOS.EFMODEL.DataModels.V_HIS_EXP_MEST_MEDICINE inputData, List<HIS_MEDICINE_BEAN> medicineBeans, bool isEdit)
        {
            try
            {
                this.IsEdit = isEdit;
                Inventec.Common.Mapper.DataObjectMapper.Map<MediMatyTypeADO>(this, inputData);

                this.SERVICE_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC;
                this.PATIENT_TYPE_ID = inputData.PATIENT_TYPE_ID;
                this.PATIENT_TYPE_CODE = inputData.PATIENT_TYPE_CODE;
                this.PATIENT_TYPE_NAME = inputData.PATIENT_TYPE_NAME;
                this.SereServParentId = inputData.SERE_SERV_PARENT_ID;

                if (inputData.CONVERT_RATIO.HasValue)
                {
                    this.AMOUNT = this.AMOUNT * inputData.CONVERT_RATIO.Value;
                    this.PRICE = this.PRICE / inputData.CONVERT_RATIO.Value;
                }

                var mety = BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>().FirstOrDefault(o => o.ID == inputData.MEDICINE_TYPE_ID);
                if (mety != null)
                {
                    this.ALERT_MAX_IN_PRESCRIPTION = mety.ALERT_MAX_IN_PRESCRIPTION;
                    this.TDL_GENDER_ID = mety.TDL_GENDER_ID;
                }

                this.SERVICE_UNIT_NAME = inputData.SERVICE_UNIT_NAME;
                this.ID = inputData.MEDICINE_TYPE_ID;
                this.IsAllowOdd = inputData.IS_ALLOW_ODD == 1 ? true : false;
                this.DataType = HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC;
                this.IsExpend = ((inputData.IS_EXPEND ?? -1) == GlobalVariables.CommonNumberTrue ? true : false);
                this.IsExpendType = ((inputData.EXPEND_TYPE_ID ?? -1) == GlobalVariables.CommonNumberTrue ? true : false);
                this.Speed = inputData.SPEED;
                this.IsOutKtcFee = ((inputData.IS_OUT_PARENT_FEE ?? 0) == GlobalVariables.CommonNumberTrue);
                this.IsKHBHYT = false;

                this.TotalPrice = ((this.PRICE ?? 0) * (this.AMOUNT ?? 0)) * (1 + (inputData.VAT_RATIO ?? 0));
                var checkMatyInStock = AssignPrescriptionWorker.MediMatyCreateWorker.getDataAmountOutOfStock(this, inputData.SERVICE_ID, inputData.MEDI_STOCK_ID);
                MediMatyTypeADO checkMediMatyTypeADO = new MediMatyTypeADO();
                Inventec.Common.Mapper.DataObjectMapper.Map<MediMatyTypeADO>(checkMediMatyTypeADO, checkMatyInStock);

                if (isEdit)
                {
                    this.PRE_AMOUNT = this.AMOUNT;
                    this.NUM_ORDER = inputData.NUM_ORDER;
                    this.UseTimeTo = inputData.USE_TIME_TO;

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
                    this.NUM_ORDER = AssignPrescriptionWorker.MediMatyCreateWorker.getNumRow();
                }
                var mst = BackendDataWorker.Get<V_HIS_MEDI_STOCK>().FirstOrDefault(o => o.ID == inputData.TDL_MEDI_STOCK_ID);
                if (mst != null)
                {
                    this.MEDI_STOCK_ID = mst.ID;
                    this.MEDI_STOCK_CODE = mst.MEDI_STOCK_CODE;
                    this.MEDI_STOCK_NAME = mst.MEDI_STOCK_NAME;
                }
                UpdateMedicineUseFormInDataRow(this);

                if ((this.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC)
                    && this.PATIENT_TYPE_ID == HisConfigCFG.PatientTypeId__BHYT
                    && String.IsNullOrEmpty(this.TUTORIAL))
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

                AssignPrescriptionWorker.MediMatyCreateWorker.setNumRow();
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
                if (inputData.CONVERT_RATIO.HasValue)
                {
                    this.AMOUNT = this.AMOUNT * inputData.CONVERT_RATIO.Value;
                    this.PRICE = this.PRICE / inputData.CONVERT_RATIO.Value;
                }
                this.SERVICE_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC;
                this.PATIENT_TYPE_ID = inputData.PATIENT_TYPE_ID;
                this.PATIENT_TYPE_CODE = inputData.PATIENT_TYPE_CODE;
                this.PATIENT_TYPE_NAME = inputData.PATIENT_TYPE_NAME;
                this.SereServParentId = inputData.SERE_SERV_PARENT_ID;

                var mety = BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>().FirstOrDefault(o => o.ID == inputData.MEDICINE_TYPE_ID);
                if (mety != null)
                {
                    this.ALERT_MAX_IN_PRESCRIPTION = mety.ALERT_MAX_IN_PRESCRIPTION;
                    this.TDL_GENDER_ID = mety.TDL_GENDER_ID;
                }

                this.SERVICE_UNIT_NAME = inputData.SERVICE_UNIT_NAME;
                this.ID = inputData.MEDICINE_TYPE_ID;
                this.IsAllowOdd = inputData.IS_ALLOW_ODD == 1 ? true : false;
                this.DataType = HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC;
                this.IsExpend = ((inputData.IS_EXPEND ?? -1) == GlobalVariables.CommonNumberTrue ? true : false);
                this.IsExpendType = ((inputData.EXPEND_TYPE_ID ?? -1) == GlobalVariables.CommonNumberTrue ? true : false);
                this.Speed = inputData.SPEED;
                this.IsOutKtcFee = ((inputData.IS_OUT_PARENT_FEE ?? 0) == GlobalVariables.CommonNumberTrue);
                this.IsKHBHYT = false;

                this.TotalPrice = ((this.PRICE ?? 0) * (this.AMOUNT ?? 0)) * (1 + (inputData.VAT_RATIO ?? 0));

                if (serviceReq != null)
                {
                    if (inputData.USE_TIME_TO.HasValue)
                    {
                        DateTime dtUseTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(serviceReq.INTRUCTION_TIME).Value;
                        DateTime dtUseTimeTo = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(inputData.USE_TIME_TO.Value).Value;
                        TimeSpan ts = dtUseTimeTo.Date - dtUseTime.Date;
                        this.UseDays = ts.Days + 1;
                        DateTime dtInstructionTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(currentInstructionTime).Value;
                        this.UseTimeTo = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtInstructionTime.AddDays((double)(ts.Days)));
                    }
                }

                this.NUM_ORDER = AssignPrescriptionWorker.MediMatyCreateWorker.getNumRow();

                var mst = BackendDataWorker.Get<V_HIS_MEDI_STOCK>().FirstOrDefault(o => o.ID == inputData.TDL_MEDI_STOCK_ID);
                if (mst != null)
                {
                    this.MEDI_STOCK_ID = mst.ID;
                    this.MEDI_STOCK_CODE = mst.MEDI_STOCK_CODE;
                    this.MEDI_STOCK_NAME = mst.MEDI_STOCK_NAME;
                }
                this.TotalPrice = (this.AMOUNT ?? 0 * this.PRICE ?? 0);
                UpdateMedicineUseFormInDataRow(this);

                if ((this.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC)
                    && this.PATIENT_TYPE_ID == HisConfigCFG.PatientTypeId__BHYT
                    && String.IsNullOrEmpty(this.TUTORIAL))
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

                AssignPrescriptionWorker.MediMatyCreateWorker.setNumRow();
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
                this.TUTORIAL = "";

                this.IsExpend = ((inputData.IS_EXPEND ?? -1) == GlobalVariables.CommonNumberTrue ? true : false);
                this.IsExpendType = ((inputData.EXPEND_TYPE_ID ?? -1) == GlobalVariables.CommonNumberTrue ? true : false);
                this.IsOutKtcFee = ((inputData.IS_OUT_PARENT_FEE ?? 0) == GlobalVariables.CommonNumberTrue);

                if (inputData.EQUIPMENT_SET_ID.HasValue)
                {
                    this.EQUIPMENT_SET_ID = inputData.EQUIPMENT_SET_ID.Value;
                }

                this.TotalPrice = ((this.PRICE ?? 0) * (this.AMOUNT ?? 0)) * (1 + (inputData.VAT_RATIO ?? 0));

                var maty = BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>().FirstOrDefault(o => o.ID == inputData.MATERIAL_TYPE_ID);
                if (maty != null)
                {
                    this.IsStent = ((maty.IS_STENT ?? 0) == GlobalVariables.CommonNumberTrue ? true : false);
                    this.IsAllowOdd = maty.IS_ALLOW_ODD == 1 ? true : false;
                    this.ALERT_MAX_IN_PRESCRIPTION = maty.ALERT_MAX_IN_PRESCRIPTION;
                    this.TDL_GENDER_ID = maty.TDL_GENDER_ID;
                    this.MAX_REUSE_COUNT = maty.MAX_REUSE_COUNT;
                }

                if (!String.IsNullOrEmpty(inputData.SERIAL_NUMBER))
                {
                    Inventec.Common.Logging.LogSystem.Debug("Truong hop vat tu trong don cu la vat tu TSD, tu dng chuyen sang vat tu trong kho" + inputData.MATERIAL_TYPE_NAME + " - " + inputData.SERIAL_NUMBER);
                }

                if (isEdit)
                {
                    this.DataType = (!String.IsNullOrEmpty(inputData.SERIAL_NUMBER) ? HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.VATTU_TSD : HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.VATTU);
                    this.SERIAL_NUMBER = inputData.SERIAL_NUMBER;
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
                    this.DataType = HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.VATTU;
                    this.SERIAL_NUMBER = "";
                    this.USE_REMAIN_COUNT = null;

                    //Kiem tra xem vat tu do co trong kho??
                    var checkMatyInStock = AssignPrescriptionWorker.MediMatyCreateWorker.getDataAmountOutOfStock(this, inputData.SERVICE_ID, inputData.MEDI_STOCK_ID);
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
                    this.NUM_ORDER = AssignPrescriptionWorker.MediMatyCreateWorker.getNumRow();
                }

                //this.TotalPrice = (this.AMOUNT ?? 0 * this.PRICE ?? 0);
                this.IsKHBHYT = false;

                this.PrimaryKey = (inputData.SERVICE_ID + "__" + Inventec.Common.DateTime.Get.Now() + "__" + Guid.NewGuid().ToString());
                AssignPrescriptionWorker.MediMatyCreateWorker.setNumRow();
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

                    this.AMOUNT = inputData.AMOUNT;
                    if (this.CONVERT_RATIO.HasValue)
                    {
                        this.AMOUNT = this.AMOUNT * this.CONVERT_RATIO.Value;
                        this.PRICE = this.PRICE / this.CONVERT_RATIO.Value;
                    }
                    this.IsAllowOdd = mety.IS_ALLOW_ODD == 1 ? true : false;
                    //Trường hợp thuốc đang có trong kho
                    var checkMatyInStock = AssignPrescriptionWorker.MediMatyCreateWorker.getDataAmountOutOfStock(this, mety.SERVICE_ID, inputData.TDL_MEDI_STOCK_ID);
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
                    patientType = AssignPrescriptionWorker.MediMatyCreateWorker.choosePatientTypeDefaultlService(AssignPrescriptionWorker.MediMatyCreateWorker.getPatientTypeId(), this.SERVICE_ID, this.SERVICE_TYPE_ID);
                    if (patientType != null)
                    {
                        this.PATIENT_TYPE_ID = patientType.ID;
                        this.PATIENT_TYPE_CODE = patientType.PATIENT_TYPE_CODE;
                        this.PATIENT_TYPE_NAME = patientType.PATIENT_TYPE_NAME;
                    }
                    this.ID = inputData.MEDICINE_TYPE_ID;
                    this.DataType = HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC;
                    if (isEdit)
                    {
                        this.PRE_AMOUNT = inputData.AMOUNT;
                        this.NUM_ORDER = inputData.NUM_ORDER;
                    }
                    else
                    {
                        this.NUM_ORDER = AssignPrescriptionWorker.MediMatyCreateWorker.getNumRow();
                    }
                    //this.IS_OUT_PARENT_FEE = inputData.IS_OUT_PARENT_FEE;
                    this.IsExpend = false;
                    this.IsKHBHYT = false;

                    this.PrimaryKey = (mety.SERVICE_ID + "__" + Inventec.Common.DateTime.Get.Now() + "__" + Guid.NewGuid().ToString());
                    AssignPrescriptionWorker.MediMatyCreateWorker.setNumRow();
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
                        this.AMOUNT = this.AMOUNT * this.CONVERT_RATIO.Value;
                        this.PRICE = this.PRICE / this.CONVERT_RATIO.Value;
                    }
                    this.IsAllowOdd = maty.IS_ALLOW_ODD == 1 ? true : false;
                    //Trường hợp thuốc đang có trong kho
                    var checkMatyInStock = AssignPrescriptionWorker.MediMatyCreateWorker.getDataAmountOutOfStock(this, maty.SERVICE_ID, inputData.TDL_MEDI_STOCK_ID);
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
                    patientType = AssignPrescriptionWorker.MediMatyCreateWorker.choosePatientTypeDefaultlService(AssignPrescriptionWorker.MediMatyCreateWorker.getPatientTypeId(), this.SERVICE_ID, this.SERVICE_TYPE_ID);
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
                        this.NUM_ORDER = AssignPrescriptionWorker.MediMatyCreateWorker.getNumRow();
                    }
                    this.MEDICINE_TYPE_CODE = maty.MATERIAL_TYPE_CODE;
                    this.MEDICINE_TYPE_NAME = maty.MATERIAL_TYPE_NAME;
                    //this.IS_OUT_PARENT_FEE = inputData.IS_OUT_PARENT_FEE;
                    this.IsExpend = false;
                    this.IsKHBHYT = false;
                    this.ID = inputData.MATERIAL_TYPE_ID;
                    this.DataType = HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.VATTU;

                    this.PrimaryKey = (maty.SERVICE_ID + "__" + Inventec.Common.DateTime.Get.Now() + "__" + Guid.NewGuid().ToString());
                    AssignPrescriptionWorker.MediMatyCreateWorker.setNumRow();
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

                    this.DataType = HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC_DM;
                    this.ID = (inputData.MEDICINE_TYPE_ID ?? 0);
                    this.IsAllowOdd = mety.IS_ALLOW_ODD == 1 ? true : false;
                }
                else
                {
                    this.SERVICE_UNIT_NAME = inputData.UNIT_NAME;
                    this.MEDICINE_TYPE_NAME = inputData.MEDICINE_TYPE_NAME;
                    this.DataType = HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC_TUTUC;
                }

                //Chi dinh tu man hinh phau thuat, thu thuat
                //#18124
                //Bổ sung cấu hình và sửa lại cách tự động hao phí
                //Bổ sung cấu hình: có tự động hao phí khi kê ở PTTT hay không? (mặc định có)
                //Cách tự động hao phí: sửa lại là chỉ hao phí với vật tư có loại dịch vụ bhyt: vật tư trong danh mục.
                //trường hợp vật tư ở danh mục có check tự động hao phí thì luôn check không quan tâm điều kiện khác.
                if ((AssignPrescriptionWorker.MediMatyCreateWorker.getIsAutoCheckExpend() == true
                    && HisConfigCFG.IsAutoTickExpendWithAssignPresPTTT) || (mety != null && mety.IS_AUTO_EXPEND == GlobalVariables.CommonNumberTrue))
                    this.IsExpend = true;
                this.UseTimeTo = inputData.USE_TIME_TO;
                this.TUTORIAL = inputData.TUTORIAL;
                this.AMOUNT = inputData.AMOUNT;
                this.PRICE = inputData.PRICE;
                if (this.CONVERT_RATIO.HasValue)
                {
                    this.AMOUNT = this.AMOUNT * this.CONVERT_RATIO.Value;
                    this.PRICE = this.PRICE / this.CONVERT_RATIO.Value;
                }
                this.SERVICE_REQ_ID = inputData.SERVICE_REQ_ID;
                this.SERVICE_REQ_METY_MATY_ID = inputData.ID;
                if (isEdit)
                {
                    this.PRE_AMOUNT = this.AMOUNT;
                    this.NUM_ORDER = inputData.NUM_ORDER;
                }
                else
                {
                    this.NUM_ORDER = AssignPrescriptionWorker.MediMatyCreateWorker.getNumRow();
                }


                this.TotalPrice = (this.PRICE ?? 0) * (this.AMOUNT ?? 0);

                this.PrimaryKey = ((mety != null ? mety.SERVICE_ID : 0) + "__" + Inventec.Common.DateTime.Get.Now() + "__" + Guid.NewGuid().ToString());
                AssignPrescriptionWorker.MediMatyCreateWorker.setNumRow();
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
        public MediMatyTypeADO(MOS.EFMODEL.DataModels.HIS_SERVICE_REQ_METY inputData, long currentInstructionTime, HIS_SERVICE_REQ serviceReq)
        {
            try
            {
                var mety = BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>().FirstOrDefault(o => o.ID == (inputData.MEDICINE_TYPE_ID ?? 0));
                if (mety != null)
                {
                    Inventec.Common.Mapper.DataObjectMapper.Map<MediMatyTypeADO>(this, mety);
                    this.ID = (inputData.MEDICINE_TYPE_ID ?? 0);
                    this.IsAllowOdd = mety.IS_ALLOW_ODD == 1 ? true : false;

                    //Lay doi tuong mac dinh
                    MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE patientType = new MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE();
                    patientType = AssignPrescriptionWorker.MediMatyCreateWorker.choosePatientTypeDefaultlService(AssignPrescriptionWorker.MediMatyCreateWorker.getPatientTypeId(), mety.SERVICE_ID, mety.SERVICE_TYPE_ID);
                    if (patientType != null)
                    {
                        this.PATIENT_TYPE_ID = patientType.ID;
                        this.PATIENT_TYPE_CODE = patientType.PATIENT_TYPE_CODE;
                        this.PATIENT_TYPE_NAME = patientType.PATIENT_TYPE_NAME;
                    }
                }

                this.DataType = HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC;
                this.UseTimeTo = inputData.USE_TIME_TO;
                this.TUTORIAL = inputData.TUTORIAL;
                this.AMOUNT = inputData.AMOUNT / (serviceReq.KIDNEY_TIMES ?? 1);
                this.PRICE = inputData.PRICE;
                if (this.CONVERT_RATIO.HasValue)
                {
                    this.AMOUNT = this.AMOUNT * this.CONVERT_RATIO.Value;
                    this.PRICE = this.PRICE / this.CONVERT_RATIO.Value;
                }
                this.SERVICE_REQ_ID = inputData.SERVICE_REQ_ID;
                this.SERVICE_REQ_METY_MATY_ID = inputData.ID;
                this.TotalPrice = (this.PRICE ?? 0) * (this.AMOUNT ?? 0);

                if (serviceReq != null)
                {
                    if (inputData.USE_TIME_TO.HasValue)
                    {
                        DateTime dtUseTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(serviceReq.INTRUCTION_TIME).Value;
                        DateTime dtUseTimeTo = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(inputData.USE_TIME_TO.Value).Value;
                        TimeSpan ts = dtUseTimeTo.Date - dtUseTime.Date;
                        this.UseDays = ts.Days + 1;
                        DateTime dtInstructionTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(currentInstructionTime).Value;
                        this.UseTimeTo = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtInstructionTime.AddDays((double)(ts.Days)));
                    }
                }

                //Kiem tra xem thuoc do co trong kho??
                AssignPrescriptionWorker.MediMatyCreateWorker.setDefaultMediStockForData(this);

                this.NUM_ORDER = AssignPrescriptionWorker.MediMatyCreateWorker.getNumRow();

                this.PrimaryKey = ((mety != null ? mety.SERVICE_ID : 0) + "__" + Inventec.Common.DateTime.Get.Now() + "__" + Guid.NewGuid().ToString());
                AssignPrescriptionWorker.MediMatyCreateWorker.setNumRow();
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
                this.SERVICE_REQ_ID = inputData.SERVICE_REQ_ID;
                this.SERVICE_REQ_METY_MATY_ID = inputData.ID;

                this.DataType = HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.VATTU_DM;
                this.AMOUNT = inputData.AMOUNT;
                this.PRICE = inputData.PRICE;
                if (this.CONVERT_RATIO.HasValue)
                {
                    this.AMOUNT = this.AMOUNT * this.CONVERT_RATIO.Value;
                    this.PRICE = this.PRICE / this.CONVERT_RATIO.Value;
                }
                this.TotalPrice = (inputData.PRICE ?? 0) * (this.AMOUNT ?? 0);

                if (isEdit)
                {
                    this.PRE_AMOUNT = inputData.AMOUNT;
                    this.NUM_ORDER = inputData.NUM_ORDER;
                }
                else
                {
                    this.NUM_ORDER = AssignPrescriptionWorker.MediMatyCreateWorker.getNumRow();
                }
                if (((AssignPrescriptionWorker.MediMatyCreateWorker.getIsAutoCheckExpend() == true && (this.HEIN_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__VT_TDM || this.HEIN_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__VT_NDM))
                   && HisConfigCFG.IsAutoTickExpendWithAssignPresPTTT)
                 || (maty != null && maty.IS_AUTO_EXPEND == GlobalVariables.CommonNumberTrue))
                {
                    this.IsExpend = true;
                }

                this.PrimaryKey = ((maty != null ? maty.SERVICE_ID : 0) + "__" + Inventec.Common.DateTime.Get.Now() + "__" + Guid.NewGuid().ToString());
                AssignPrescriptionWorker.MediMatyCreateWorker.setNumRow();
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
        public bool IsMultiDateState { get; set; }
        public List<long> IntructionTimeSelecteds { get; set; }
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
        public short? IS_CHEMICAL_SUBSTANCE { get; set; }
        public bool IsExpendType { get; set; }
        public long? PATIENT_TYPE_ID { get; set; }
        public string PATIENT_TYPE_CODE { get; set; }
        public string PATIENT_TYPE_NAME { get; set; }
        public decimal? AMOUNT { get; set; }
        public decimal? BK_AMOUNT { get; set; }
        public decimal? PRE_AMOUNT { get; set; }
        public bool IsExpend { get; set; }
        public bool IsKHBHYT { get; set; }
        public bool IsOutParentFee { get; set; }
        public bool? IsOutKtcFee { get; set; }
        public bool IsAssignDay { get; set; }
        public bool? IsAllowOdd { get; set; }
        public decimal? AmountAlert { get; set; }
        public double? Sang { get; set; }
        public double? Trua { get; set; }
        public double? Chieu { get; set; }
        public double? Toi { get; set; }
        public long? HTU_ID { get; set; }
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

        public DevExpress.XtraEditors.DXErrorProvider.ErrorType ErrorTypeAmountHasRound { get; set; }
        public string ErrorMessageAmountHasRound { get; set; }
        public DevExpress.XtraEditors.DXErrorProvider.ErrorType ErrorTypeAmount { get; set; }
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
    }
}

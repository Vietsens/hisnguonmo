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
using HIS.Desktop.Plugins.AssignPrescriptionYHCT.Config;
using HIS.Desktop.Plugins.AssignPrescriptionYHCT.Resources;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.AssignPrescriptionYHCT.ADO
{
    public class MediMatyTypeADO : MOS.EFMODEL.DataModels.V_HIS_MEDICINE_TYPE
    {
        public MediMatyTypeADO()
        {

        }

        public MediMatyTypeADO(MOS.EFMODEL.DataModels.V_HIS_MEDICINE_TYPE midicineType)
        {
            try
            {
                Inventec.Common.Mapper.DataObjectMapper.Map<MediMatyTypeADO>(this, midicineType);
                this.DO_NOT_REQUIRED_USE_FORM = midicineType.DO_NOT_REQUIRED_USE_FORM;
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
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public MediMatyTypeADO(MOS.EFMODEL.DataModels.V_HIS_EMTE_MEDICINE_TYPE inputData)
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
                    this.RemedyCount = inputData.REMEDY_COUNT;
                    if (inputData.REMEDY_COUNT > 0)
                    {
                        this.AmountOneRemedy = inputData.AMOUNT / inputData.REMEDY_COUNT;
                    }
                    this.IsExpend = ((inputData.IS_EXPEND ?? 0) == GlobalVariables.CommonNumberTrue ? true : false);
                    this.IsAllowOdd = mety.IS_ALLOW_ODD == 1 ? true : false;
                    //this.MEDICINE_USE_FORM_ID = inputData.MEDICINE_USE_FORM_ID;
                    //this.MEDICINE_USE_FORM_CODE = inputData.MEDICINE_USE_FORM_CODE;
                    //this.MEDICINE_USE_FORM_NAME = inputData.MEDICINE_USE_FORM_NAME;
                    this.IsKHBHYT = false;
                    this.NUM_ORDER = AssignPrescriptionWorker.Instance.MediMatyCreateWorker.getNumRow();
                    this.CONTRAINDICATION = mety.CONTRAINDICATION;
                    this.DO_NOT_REQUIRED_USE_FORM = mety.DO_NOT_REQUIRED_USE_FORM;

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
                        patientType = AssignPrescriptionWorker.Instance.MediMatyCreateWorker.choosePatientTypeDefaultlService(AssignPrescriptionWorker.Instance.MediMatyCreateWorker.getPatientTypeId(), (inputData.SERVICE_ID ?? 0), mety.SERVICE_TYPE_ID);
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
                    }
                    this.PrimaryKey = ((inputData.SERVICE_ID ?? 0) + "__" + Inventec.Common.DateTime.Get.Now() + "__" + Guid.NewGuid().ToString());
                    AssignPrescriptionWorker.Instance.MediMatyCreateWorker.setNumRow();
                }
                else
                {
                    Inventec.Common.Logging.LogSystem.Debug("Create by V_HIS_EMTE_MEDICINE_TYPE fail => Khong tim thay thuoc theo id = " + this.ID + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => this), this));
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
                    this.TUTORIAL = "";
                    this.MEDICINE_USE_FORM_ID = null;
                    this.MEDICINE_USE_FORM_NAME = "";
                    this.IsExpend = ((inputData.IS_EXPEND ?? 0) == GlobalVariables.CommonNumberTrue ? true : false);
                    this.IsStent = ((maty.IS_STENT ?? 0) == GlobalVariables.CommonNumberTrue ? true : false);
                    this.IsAllowOdd = maty.IS_ALLOW_ODD == 1 ? true : false;
                    this.IsKHBHYT = false;
                    this.NUM_ORDER = AssignPrescriptionWorker.Instance.MediMatyCreateWorker.getNumRow();



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
                        patientType = AssignPrescriptionWorker.Instance.MediMatyCreateWorker.choosePatientTypeDefaultlService(AssignPrescriptionWorker.Instance.MediMatyCreateWorker.getPatientTypeId(), (inputData.SERVICE_ID ?? 0), maty.SERVICE_TYPE_ID);
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

                    this.PrimaryKey = ((inputData.SERVICE_ID ?? 0) + "__" + Inventec.Common.DateTime.Get.Now() + "__" + Guid.NewGuid().ToString());
                    AssignPrescriptionWorker.Instance.MediMatyCreateWorker.setNumRow();
                }
                else
                {
                    Inventec.Common.Logging.LogSystem.Debug("Create by V_HIS_EMTE_MEDICINE_TYPE fail => Khong tim thay thuoc theo id = " + this.ID + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => this), this));
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
                    this.NUM_ORDER = AssignPrescriptionWorker.Instance.MediMatyCreateWorker.getNumRow();

                    //Lay doi tuong mac dinh
                    MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE patientType = new MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE();
                    patientType = AssignPrescriptionWorker.Instance.MediMatyCreateWorker.choosePatientTypeDefaultlService(AssignPrescriptionWorker.Instance.MediMatyCreateWorker.getPatientTypeId(), (inputData.SERVICE_ID), maty.SERVICE_TYPE_ID);
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
                    Inventec.Common.Logging.LogSystem.Debug("Create by V_HIS_EMTE_MEDICINE_TYPE fail => Khong tim thay thuoc theo id = " + this.ID + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => this), this));
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
                this.SERVICE_UNIT_NAME = inputData.SERVICE_UNIT_NAME;
                this.ID = inputData.MEDICINE_TYPE_ID;
                this.IsAllowOdd = inputData.IS_ALLOW_ODD == 1 ? true : false;
                this.DataType = HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC;
                this.IsExpend = ((inputData.IS_EXPEND ?? -1) == GlobalVariables.CommonNumberTrue ? true : false);
                this.IsOutKtcFee = ((inputData.IS_OUT_PARENT_FEE ?? 0) == GlobalVariables.CommonNumberTrue);
                this.IsKHBHYT = false;

                this.TUTORIAL = inputData.TUTORIAL;
                this.TotalPrice = ((inputData.PRICE ?? 0) * inputData.AMOUNT) * (1 + (inputData.VAT_RATIO ?? 0));
                var checkMatyInStock = AssignPrescriptionWorker.Instance.MediMatyCreateWorker.getDataAmountOutOfStock(this, inputData.SERVICE_ID, inputData.MEDI_STOCK_ID);
                MediMatyTypeADO checkMediMatyTypeADO = new MediMatyTypeADO();
                Inventec.Common.Mapper.DataObjectMapper.Map<MediMatyTypeADO>(checkMediMatyTypeADO, checkMatyInStock);
                var mety = BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>().FirstOrDefault(o => o.ID == inputData.MEDICINE_TYPE_ID);
                if(mety != null)
                {
                    this.ATC_CODES = mety.ATC_CODES;
                    this.ATC_GROUP_CODES = mety.ATC_GROUP_CODES;
                    this.ACTIVE_INGR_BHYT_CODE = mety.ACTIVE_INGR_BHYT_CODE;
                    this.ACTIVE_INGR_BHYT_NAME = mety.ACTIVE_INGR_BHYT_NAME;
                }
                if (isEdit)
                {
                    this.PRE_AMOUNT = inputData.AMOUNT;
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
                        this.AmountAlert = inputData.AMOUNT;
                    }
                    else
                    {
                        if (checkMediMatyTypeADO.AMOUNT < inputData.AMOUNT)
                            this.AmountAlert = inputData.AMOUNT;//Nếu số lượng trong đơn cũ kiểm tra lớn hơn số lượng tồn kho hiện tại thì set cảnh báo
                    }
                    this.NUM_ORDER = AssignPrescriptionWorker.Instance.MediMatyCreateWorker.getNumRow();
                }
                var mst = BackendDataWorker.Get<V_HIS_MEDI_STOCK>().FirstOrDefault(o => o.ID == inputData.TDL_MEDI_STOCK_ID);
                if (mst != null)
                {
                    this.MEDI_STOCK_ID = mst.ID;
                    this.MEDI_STOCK_CODE = mst.MEDI_STOCK_CODE;
                    this.MEDI_STOCK_NAME = mst.MEDI_STOCK_NAME;
                }
                //this.TotalPrice = (this.AMOUNT ?? 0 * this.PRICE ?? 0);
                UpdateMedicineUseFormInDataRow(this);

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
                this.SERVICE_UNIT_NAME = inputData.SERVICE_UNIT_NAME;
                this.ID = inputData.MEDICINE_TYPE_ID;
                this.IsAllowOdd = inputData.IS_ALLOW_ODD == 1 ? true : false;
                this.DataType = HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC;
                this.IsExpend = ((inputData.IS_EXPEND ?? -1) == GlobalVariables.CommonNumberTrue ? true : false);
                this.IsOutKtcFee = ((inputData.IS_OUT_PARENT_FEE ?? 0) == GlobalVariables.CommonNumberTrue);
                this.IsKHBHYT = false;

                this.TUTORIAL = inputData.TUTORIAL;
                if (serviceReq.REMEDY_COUNT > 0)
                {
                    this.RemedyCount = serviceReq.REMEDY_COUNT;
                    this.AmountOneRemedy = inputData.AMOUNT / serviceReq.REMEDY_COUNT;
                }
                var mety = BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>().FirstOrDefault(o => o.ID == inputData.MEDICINE_TYPE_ID);
                if (mety != null)
                {
                    this.ATC_CODES = mety.ATC_CODES;
                    this.ATC_GROUP_CODES = mety.ATC_GROUP_CODES;
                    this.ACTIVE_INGR_BHYT_CODE = mety.ACTIVE_INGR_BHYT_CODE;
                    this.ACTIVE_INGR_BHYT_NAME = mety.ACTIVE_INGR_BHYT_NAME;
                }
                this.TotalPrice = ((inputData.PRICE ?? 0) * inputData.AMOUNT) * (1 + (inputData.VAT_RATIO ?? 0));
                var checkMatyInStock = AssignPrescriptionWorker.Instance.MediMatyCreateWorker.getDataAmountOutOfStock(this, inputData.SERVICE_ID, inputData.MEDI_STOCK_ID);
                MediMatyTypeADO checkMediMatyTypeADO = new MediMatyTypeADO();
                Inventec.Common.Mapper.DataObjectMapper.Map<MediMatyTypeADO>(checkMediMatyTypeADO, checkMatyInStock);

                if (serviceReq != null)
                {
                    if (inputData.USE_TIME_TO.HasValue)
                    {
                        DateTime dtUseTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(serviceReq.INTRUCTION_TIME).Value;
                        DateTime dtUseTimeTo = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(inputData.USE_TIME_TO.Value).Value;
                        TimeSpan ts = dtUseTimeTo - dtUseTime;
                        this.UseDays = ts.Days + 1;
                        DateTime dtInstructionTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(currentInstructionTime).Value;
                        this.UseTimeTo = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtInstructionTime.AddDays((double)(this.UseDays)));
                    }
                }

                //Trường hợp thuốc đang có trong kho
                if (checkMediMatyTypeADO == null || checkMediMatyTypeADO.SERVICE_ID == 0)
                {
                    //Nếu số lượng trong đơn cũ kiểm tra lớn hơn số lượng tồn kho hiện tại thì set cảnh báo
                    this.AmountAlert = inputData.AMOUNT;
                }
                else
                {
                    if (checkMediMatyTypeADO.AMOUNT < inputData.AMOUNT)
                        this.AmountAlert = inputData.AMOUNT;//Nếu số lượng trong đơn cũ kiểm tra lớn hơn số lượng tồn kho hiện tại thì set cảnh báo
                }
                this.NUM_ORDER = AssignPrescriptionWorker.Instance.MediMatyCreateWorker.getNumRow();

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

                var mst = BackendDataWorker.Get<V_HIS_MEDI_STOCK>().FirstOrDefault(o => o.ID == inputData.TDL_MEDI_STOCK_ID);
                if (mst != null)
                {
                    this.MEDI_STOCK_ID = mst.ID;
                    this.MEDI_STOCK_CODE = mst.MEDI_STOCK_CODE;
                    this.MEDI_STOCK_NAME = mst.MEDI_STOCK_NAME;
                }
                //this.TotalPrice = (this.AMOUNT ?? 0 * this.PRICE ?? 0);
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

                AssignPrescriptionWorker.Instance.MediMatyCreateWorker.setNumRow();
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
                this.DataType = HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.VATTU;
                this.IsExpend = ((inputData.IS_EXPEND ?? -1) == GlobalVariables.CommonNumberTrue ? true : false);
                this.IsOutKtcFee = ((inputData.IS_OUT_PARENT_FEE ?? 0) == GlobalVariables.CommonNumberTrue);
                if (inputData.EQUIPMENT_SET_ID.HasValue)
                {
                    this.EQUIPMENT_SET_ID = inputData.EQUIPMENT_SET_ID.Value;
                }

                this.TotalPrice = ((inputData.PRICE ?? 0) * inputData.AMOUNT) * (1 + (inputData.VAT_RATIO ?? 0));

                var maty = BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>().FirstOrDefault(o => o.ID == inputData.MATERIAL_TYPE_ID);
                if (maty != null)
                {
                    this.IsStent = ((maty.IS_STENT ?? 0) == GlobalVariables.CommonNumberTrue ? true : false);
                    this.IsAllowOdd = maty.IS_ALLOW_ODD == 1 ? true : false;
                }
                if (isEdit)
                {
                    this.PRE_AMOUNT = inputData.AMOUNT;
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
                    //Kiem tra xem vat tu do co trong kho??
                    var checkMatyInStock = AssignPrescriptionWorker.Instance.MediMatyCreateWorker.getDataAmountOutOfStock(this, inputData.SERVICE_ID, inputData.MEDI_STOCK_ID);
                    MediMatyTypeADO checkMediMatyTypeADO = new MediMatyTypeADO();
                    Inventec.Common.Mapper.DataObjectMapper.Map<MediMatyTypeADO>(checkMediMatyTypeADO, checkMatyInStock);
                    if (checkMediMatyTypeADO == null || checkMediMatyTypeADO.SERVICE_ID == 0)
                    {
                        //Nếu số lượng trong đơn cũ kiểm tra lớn hơn số lượng tồn kho hiện tại thì set cảnh báo
                        this.AmountAlert = inputData.AMOUNT;
                    }
                    else
                    {
                        if (checkMediMatyTypeADO.AMOUNT < inputData.AMOUNT)
                            this.AmountAlert = inputData.AMOUNT;//Nếu số lượng trong đơn cũ kiểm tra lớn hơn số lượng tồn kho hiện tại thì set cảnh báo
                    }
                    this.NUM_ORDER = AssignPrescriptionWorker.Instance.MediMatyCreateWorker.getNumRow();
                }

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

                var mst = BackendDataWorker.Get<V_HIS_MEDI_STOCK>().FirstOrDefault(o => o.ID == inputData.TDL_MEDI_STOCK_ID);
                if (mst != null)
                {
                    this.MEDI_STOCK_ID = mst.ID;
                    this.MEDI_STOCK_CODE = mst.MEDI_STOCK_CODE;
                    this.MEDI_STOCK_NAME = mst.MEDI_STOCK_NAME;
                }
                //this.TotalPrice = (this.AMOUNT ?? 0 * this.PRICE ?? 0);
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

                    this.AMOUNT = inputData.AMOUNT;
                    this.IsAllowOdd = mety.IS_ALLOW_ODD == 1 ? true : false;
                    this.CONTRAINDICATION = mety.CONTRAINDICATION;
                    this.DO_NOT_REQUIRED_USE_FORM = mety.DO_NOT_REQUIRED_USE_FORM;
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

                    var mst = BackendDataWorker.Get<V_HIS_MEDI_STOCK>().FirstOrDefault(o => o.ID == inputData.TDL_MEDI_STOCK_ID);
                    if (mst != null)
                    {
                        this.MEDI_STOCK_ID = mst.ID;
                        this.MEDI_STOCK_CODE = mst.MEDI_STOCK_CODE;
                        this.MEDI_STOCK_NAME = mst.MEDI_STOCK_NAME;
                    }
                    //Lay doi tuong mac dinh
                    MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE patientType = new MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE();
                    patientType = AssignPrescriptionWorker.Instance.MediMatyCreateWorker.choosePatientTypeDefaultlService(AssignPrescriptionWorker.Instance.MediMatyCreateWorker.getPatientTypeId(), this.SERVICE_ID, this.SERVICE_TYPE_ID);
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
                        this.NUM_ORDER = AssignPrescriptionWorker.Instance.MediMatyCreateWorker.getNumRow();
                    }
                    //this.IS_OUT_PARENT_FEE = inputData.IS_OUT_PARENT_FEE;
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
                    patientType = AssignPrescriptionWorker.Instance.MediMatyCreateWorker.choosePatientTypeDefaultlService(AssignPrescriptionWorker.Instance.MediMatyCreateWorker.getPatientTypeId(), this.SERVICE_ID, this.SERVICE_TYPE_ID);
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
                    this.MEDICINE_TYPE_CODE = maty.MATERIAL_TYPE_CODE;
                    this.MEDICINE_TYPE_NAME = maty.MATERIAL_TYPE_NAME;
                    //this.IS_OUT_PARENT_FEE = inputData.IS_OUT_PARENT_FEE;
                    this.IsExpend = false;
                    this.IsKHBHYT = false;
                    this.ID = inputData.MATERIAL_TYPE_ID;
                    this.DataType = HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.VATTU;

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
                    this.DataType = HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC_DM;
                    this.ID = (inputData.MEDICINE_TYPE_ID ?? 0);
                    this.IsAllowOdd = mety.IS_ALLOW_ODD == 1 ? true : false;
                    this.CONTRAINDICATION = mety.CONTRAINDICATION;
                    this.DO_NOT_REQUIRED_USE_FORM = mety.DO_NOT_REQUIRED_USE_FORM;
                }
                else
                {
                    this.SERVICE_UNIT_NAME = inputData.UNIT_NAME;
                    this.MEDICINE_TYPE_NAME = inputData.MEDICINE_TYPE_NAME;
                    this.DataType = HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC_TUTUC;
                }
                this.HTU_TEXT = inputData.HTU_TEXT;
                //Chi dinh tu man hinh phau thuat, thu thuat
                if ((AssignPrescriptionWorker.Instance.MediMatyCreateWorker.getIsAutoCheckExpend() == true && this.HEIN_SERVICE_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__VT_TT))
                    this.IsExpend = true;
                this.UseTimeTo = inputData.USE_TIME_TO;
                this.TUTORIAL = inputData.TUTORIAL;
                this.AMOUNT = inputData.AMOUNT;
                this.SERVICE_REQ_ID = inputData.SERVICE_REQ_ID;
                this.SERVICE_REQ_METY_MATY_ID = inputData.ID;
                if (isEdit)
                {
                    this.PRE_AMOUNT = inputData.AMOUNT;
                    this.NUM_ORDER = inputData.NUM_ORDER;
                }
                else
                {
                    this.NUM_ORDER = AssignPrescriptionWorker.Instance.MediMatyCreateWorker.getNumRow();
                }

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
                    this.DataType = HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC_DM;
                    this.ID = (inputData.MEDICINE_TYPE_ID ?? 0);
                    this.IsAllowOdd = mety.IS_ALLOW_ODD == 1 ? true : false;
                    this.CONTRAINDICATION = mety.CONTRAINDICATION;
                    this.DO_NOT_REQUIRED_USE_FORM = mety.DO_NOT_REQUIRED_USE_FORM;
                }
                else
                {
                    this.SERVICE_UNIT_NAME = inputData.UNIT_NAME;
                    this.MEDICINE_TYPE_NAME = inputData.MEDICINE_TYPE_NAME;
                    this.DataType = HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC_TUTUC;
                }

                this.HTU_TEXT = inputData.HTU_TEXT;
                //Chi dinh tu man hinh phau thuat, thu thuat
                if ((AssignPrescriptionWorker.Instance.MediMatyCreateWorker.getIsAutoCheckExpend() == true && this.HEIN_SERVICE_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__VT_TT))
                    this.IsExpend = true;
                this.UseTimeTo = inputData.USE_TIME_TO;
                this.TUTORIAL = inputData.TUTORIAL;
                this.AMOUNT = inputData.AMOUNT;
                

                if (serviceReq.REMEDY_COUNT > 0)
                {
                    this.RemedyCount = serviceReq.REMEDY_COUNT;
                    this.AmountOneRemedy = inputData.AMOUNT / serviceReq.REMEDY_COUNT;
                }
                this.SERVICE_REQ_ID = inputData.SERVICE_REQ_ID;
                this.SERVICE_REQ_METY_MATY_ID = inputData.ID;

                if (serviceReq != null)
                {
                    if (inputData.USE_TIME_TO.HasValue)
                    {
                        DateTime dtUseTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(serviceReq.INTRUCTION_TIME).Value;
                        DateTime dtUseTimeTo = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(inputData.USE_TIME_TO.Value).Value;
                        TimeSpan ts = dtUseTimeTo - dtUseTime;
                        this.UseDays = ts.Days;
                        DateTime dtInstructionTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(currentInstructionTime).Value;
                        this.UseTimeTo = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtInstructionTime.AddDays((double)(this.UseDays)));
                    }
                }

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
                this.TUTORIAL = inputData.TUTORIAL;
                this.HTU_TEXT = inputData.HTU_TEXT;
                this.SERVICE_REQ_ID = inputData.SERVICE_REQ_ID;
                this.SERVICE_REQ_METY_MATY_ID = inputData.ID;
                this.DataType = HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.VATTU_DM;
                this.AMOUNT = inputData.AMOUNT;
                if (isEdit)
                {
                    this.PRE_AMOUNT = inputData.AMOUNT;
                    this.NUM_ORDER = inputData.NUM_ORDER;
                }
                else
                {
                    this.NUM_ORDER = AssignPrescriptionWorker.Instance.MediMatyCreateWorker.getNumRow();
                }
                if ((AssignPrescriptionWorker.Instance.MediMatyCreateWorker.getIsAutoCheckExpend() == true && this.HEIN_SERVICE_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__VT_TT)
                 || (maty.IS_AUTO_EXPEND == GlobalVariables.CommonNumberTrue))
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
                this.DO_NOT_REQUIRED_USE_FORM = s.DO_NOT_REQUIRED_USE_FORM;
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
                        medicineTypeSDO.CONTRAINDICATION = mety.CONTRAINDICATION;
                        medicineTypeSDO.DO_NOT_REQUIRED_USE_FORM = mety.DO_NOT_REQUIRED_USE_FORM;
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
        public bool? IsNotTakeBean { get; set; }
        public bool? IsStent { get; set; }
        public List<long> ExpMestDetailIds { get; set; }
        public decimal? PRICE { get; set; }
        public List<long> BeanIds { get; set; }
        public string PrimaryKey { get; set; }
        public int DataType { get; set; }
        public List<MOS.EFMODEL.DataModels.V_HIS_MEDICINE_BEAN_1> MedicineBean1Result { get; set; }
        public List<MOS.EFMODEL.DataModels.V_HIS_MATERIAL_BEAN_1> MaterialBean1Result { get; set; }
        public short? IS_CHEMICAL_SUBSTANCE { get; set; }
        public long? PATIENT_TYPE_ID { get; set; }
        public string PATIENT_TYPE_CODE { get; set; }
        public string PATIENT_TYPE_NAME { get; set; }
        public decimal? AMOUNT { get; set; }
        public long Assign_time { get; set; }
        public decimal? BK_AMOUNT { get; set; }
        public decimal? PRE_AMOUNT { get; set; }
        public bool IsExpend { get; set; }
        public bool IsKHBHYT { get; set; }
        public bool IsOutParentFee { get; set; }
        public bool? IsOutKtcFee { get; set; }
        public bool IsAssignDay { get; set; }
        public bool? IsAllowOdd { get; set; }
        public decimal? AmountAlert { get; set; }
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
        public decimal? AmountOneRemedy { get; set; }
        public long? RemedyCount { get; set; }
        public long? OTHER_PAY_SOURCE_ID { get; set; }
        public string OTHER_PAY_SOURCE_CODE { get; set; }
        public string OTHER_PAY_SOURCE_NAME { get; set; }
        public long? EXP_MEST_REASON_ID { get; set; }
        public string EXP_MEST_REASON_CODE { get; set; }
        public string EXP_MEST_REASON_NAME { get; set; }
        public string HTU_TEXT { get; set; }
        public bool IsDisableExpend { get; set; }
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

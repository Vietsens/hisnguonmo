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
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Plugins.AssignPrescriptionPK.ADO;
using HIS.Desktop.Plugins.AssignPrescriptionPK.Config;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HIS.Desktop.Plugins.AssignPrescriptionPK
{
    class TakeOrReleaseBeanWorker
    {
        internal static bool TakeForCreateBeanTSD(long expMestId, MediMatyTypeADO medicineTypeSDO, bool isHasBean, CommonParam param, ref List<OutPatientPresADO> lstOut, List<long> useTimes = null)
        {
            bool result = true;
            try
            {
                if (medicineTypeSDO.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.VATTU_TSD
                    && (!GlobalStore.IsTreatmentIn || GlobalStore.IsCabinet)
                    )
                {
                    LogSystem.Debug("TakeForCreateBeanTSD => 1.");
                    string uriForTakeBean = RequestUriStore.HIS_MATERIAL_BEAN__TAKEBEAN_BY_SERIALANDPATIENTTYPE;

                    TakeBeanBySerialSDO takeBeanSDO = new TakeBeanBySerialSDO();
                    //if (medicineTypeSDO.AMOUNT == 0) return true;

                    takeBeanSDO.ClientSessionKey = GlobalStore.ClientSessionKey;
                    takeBeanSDO.MediStockId = medicineTypeSDO.MEDI_STOCK_ID ?? 0;
                    takeBeanSDO.SerialNumber = medicineTypeSDO.SERIAL_NUMBER;
                    takeBeanSDO.PatientTypeId = medicineTypeSDO.PATIENT_TYPE_ID ?? 0;

                    var rs = CallApiTakeBean<HIS_MATERIAL_BEAN>(uriForTakeBean, takeBeanSDO, param);
                    if (rs == null || !string.IsNullOrEmpty(param.GetMessage()))
                    {
                        result = false;
                        throw new AggregateException("TakeForCreateBeanTSD => 2.2 Them/sua dong du lieu vat tu that bai. Du lieu tra ve count = 0");
                    }
                    V_HIS_MATERIAL_BEAN_1 materialBeanAdd = new MOS.EFMODEL.DataModels.V_HIS_MATERIAL_BEAN_1();
                    Inventec.Common.Mapper.DataObjectMapper.Map<V_HIS_MATERIAL_BEAN_1>(materialBeanAdd, rs);
                    medicineTypeSDO.MaterialBean1Result = medicineTypeSDO.MaterialBean1Result == null ? new List<MOS.EFMODEL.DataModels.V_HIS_MATERIAL_BEAN_1>() : medicineTypeSDO.MaterialBean1Result;
                    OutPatientPresADO ado = new OutPatientPresADO();
                    var maty = medicineTypeSDO.MaterialBean1Result.Where(o => o.MATERIAL_TYPE_ID == rs.TDL_MATERIAL_TYPE_ID && o.SERIAL_NUMBER == rs.SERIAL_NUMBER && o.MEDI_STOCK_ID == rs.MEDI_STOCK_ID).FirstOrDefault();
                    if (maty != null)
                    {
                        medicineTypeSDO.AMOUNT = 1;
                        medicineTypeSDO.TotalPrice = medicineTypeSDO.MaterialBean1Result.Sum(o => ((o.TDL_MATERIAL_IMP_PRICE * o.AMOUNT) * (1 + o.TDL_MATERIAL_IMP_VAT_RATIO)));
                    }
                    else
                    {
                        materialBeanAdd.AMOUNT = 1;
                        medicineTypeSDO.MaterialBean1Result.Add(materialBeanAdd);
                        medicineTypeSDO.TotalPrice = medicineTypeSDO.MaterialBean1Result.Sum(o => ((o.TDL_MATERIAL_IMP_PRICE * o.AMOUNT) * (1 + o.TDL_MATERIAL_IMP_VAT_RATIO)));
                    }
                    if (lstOut == null)
                        lstOut = new List<OutPatientPresADO>();
                    ado.TAKE_BEAN_ID = medicineTypeSDO.MaterialBean1Result.Select(o => o.ID).ToList();
                    ado.PrimaryKey = medicineTypeSDO.PrimaryKey;
                    ado.MEDI_MATE_ID = medicineTypeSDO.ID;
                    ado.SERVICE_TYPE_ID = medicineTypeSDO.SERVICE_TYPE_ID;
                    if (useTimes != null && useTimes.Count > 0)
                    {
                        ado.USE_TIME = useTimes.First();
                    }
                    else
                    {
                        ado.USE_TIME = 0;
                    }
                    if (medicineTypeSDO.dicUseTimeBeanIds == null)
                    {
                        medicineTypeSDO.dicUseTimeBeanIds = new Dictionary<long, List<long>>();
                    }
                    medicineTypeSDO.dicUseTimeBeanIds[ado.USE_TIME] = new List<long>();
                    medicineTypeSDO.dicUseTimeBeanIds[ado.USE_TIME] = ado.TAKE_BEAN_ID;
                    lstOut.Add(ado);
                }

                LogSystem.Debug("TakeForCreateBeanTSD => 2.");
            }
            catch (AggregateException ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        internal static bool TakeForUpdateBeanTSD(long expMestId, MediMatyTypeADO medicineTypeSDO, decimal amount, bool hasWarmError, CommonParam param)
        {
            bool result = true;
            try
            {
                if ((medicineTypeSDO.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.VATTU_TSD)
                    && (!GlobalStore.IsTreatmentIn || GlobalStore.IsCabinet)
                    )
                {
                    LogSystem.Debug("TakeForUpdateBeanTSD => 1.");
                    string uriForTakeBean = RequestUriStore.HIS_MATERIAL_BEAN__TAKEBEAN_BY_SERIALANDPATIENTTYPE;

                    TakeBeanBySerialSDO takeBeanSDO = new TakeBeanBySerialSDO();

                    takeBeanSDO.ClientSessionKey = GlobalStore.ClientSessionKey;
                    takeBeanSDO.MediStockId = medicineTypeSDO.MEDI_STOCK_ID ?? 0;
                    takeBeanSDO.SerialNumber = medicineTypeSDO.SERIAL_NUMBER;
                    takeBeanSDO.PatientTypeId = medicineTypeSDO.PATIENT_TYPE_ID ?? 0;

                    var rs = CallApiTakeBean<HIS_MATERIAL_BEAN>(uriForTakeBean, takeBeanSDO, param);
                    if (rs == null)
                        throw new AggregateException("TakeForUpdateBeanTSD => 2.2 Them/sua dong du lieu vat tu that bai. Du lieu tra ve count = 0");
                    V_HIS_MATERIAL_BEAN_1 materialBeanAdd = new MOS.EFMODEL.DataModels.V_HIS_MATERIAL_BEAN_1();
                    Inventec.Common.Mapper.DataObjectMapper.Map<V_HIS_MATERIAL_BEAN_1>(materialBeanAdd, rs);

                    var maty = medicineTypeSDO.MaterialBean1Result.Where(o => o.MATERIAL_TYPE_ID == rs.TDL_MATERIAL_TYPE_ID && o.SERIAL_NUMBER == rs.SERIAL_NUMBER && o.MEDI_STOCK_ID == rs.MEDI_STOCK_ID).FirstOrDefault();
                    if (maty != null)
                    {
                        medicineTypeSDO.AMOUNT = 1;
                    }
                    else
                    {
                        materialBeanAdd.AMOUNT = 1;
                        medicineTypeSDO.MaterialBean1Result.Add(materialBeanAdd);
                    }

                    medicineTypeSDO.TotalPrice = medicineTypeSDO.MaterialBean1Result.Sum(o => ((o.TDL_MATERIAL_IMP_PRICE * o.AMOUNT) * (1 + o.TDL_MATERIAL_IMP_VAT_RATIO)));
                    medicineTypeSDO.ErrorTypeMediMatyBean = DevExpress.XtraEditors.DXErrorProvider.ErrorType.None;
                    medicineTypeSDO.ErrorMessageMediMatyBean = "";
                }

                LogSystem.Debug("TakeForUpdateBeanTSD => 4.");
            }
            catch (AggregateException ex)
            {
                result = false;
                //if (hasWarmError)
                //{
                //    medicineTypeSDO.ErrorTypeMediMatyBean = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                //    medicineTypeSDO.ErrorMessageMediMatyBean = ResourceMessage.TakeBeanForUpdateThatBai;
                //}
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        internal static bool TakeForCreateBean(List<long> intructionTimes, long expMestId, MediMatyTypeADO medicineTypeSDO, bool isHasBean, CommonParam param,List<long> useTimeSelecteds,List<OutPatientPresADO> lstOut,bool IsCellChange = false)
        {
            bool result = true;
            try
            {
                if ((medicineTypeSDO.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC
                    || medicineTypeSDO.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.VATTU)
                    && (!GlobalStore.IsTreatmentIn || GlobalStore.IsCabinet)
                    )
                {
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => medicineTypeSDO), medicineTypeSDO));
                    List<long> beanIds = null;
                    if (isHasBean)
                    {
                        if (medicineTypeSDO.MedicineBean1Result != null && medicineTypeSDO.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC)
                        {
                            beanIds = medicineTypeSDO.MedicineBean1Result.Select(o => o.ID).ToList();
                        }
                        else if (medicineTypeSDO.MaterialBean1Result != null && medicineTypeSDO.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__VT)
                        {
                            beanIds = medicineTypeSDO.MaterialBean1Result.Select(o => o.ID).ToList();
                        }
                    }
                    LogSystem.Debug("TakeOrUpdateBean => 1.");
                    if (medicineTypeSDO.AMOUNT == 0) return true;

                    string uriForTakeBean = GetUriForTakeBean(medicineTypeSDO);
                    object takeBeanSDO = GetDataForTakeBean(intructionTimes, medicineTypeSDO, beanIds);
                    bool IsSucess = true;
                    if (useTimeSelecteds != null && useTimeSelecteds.Count > 0)
                    {
                        for (int i = 0; i < useTimeSelecteds.Count; i++)
                        {
                            CallApiTakeBean(ref medicineTypeSDO, uriForTakeBean, takeBeanSDO, ref param, i,ref lstOut, ref IsSucess, new List<long>() { useTimeSelecteds[i] });
                        }
                    }
                    else
                    {
                        CallApiTakeBean(ref medicineTypeSDO, uriForTakeBean, takeBeanSDO, ref param, 0, ref lstOut,ref IsSucess);
                    }

					Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => lstOut), lstOut));
                    if (medicineTypeSDO.MedicineBean1Result != null && medicineTypeSDO.MedicineBean1Result.Count > 0)
                    {
                        medicineTypeSDO.TotalPrice = medicineTypeSDO.MedicineBean1Result.Sum(o => ((o.TDL_MEDICINE_IMP_PRICE * o.AMOUNT) * (1 + o.TDL_MEDICINE_IMP_VAT_RATIO)));
                    }
                    else if (medicineTypeSDO.MaterialBean1Result != null && medicineTypeSDO.MaterialBean1Result.Count > 0)
                    {
                        medicineTypeSDO.TotalPrice = medicineTypeSDO.MaterialBean1Result.Sum(o => ((o.TDL_MATERIAL_IMP_PRICE * o.AMOUNT) * (1 + o.TDL_MATERIAL_IMP_VAT_RATIO)));
                    }
                    if(!IsSucess || !string.IsNullOrEmpty(param.GetMessage()))
                        return false;                     
                }

                LogSystem.Debug("TakeOrUpdateBean => 4.");
            }
            catch (AggregateException ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private static void CallApiTakeBean(ref MediMatyTypeADO medicineTypeSDO, string uriForTakeBean, object takeBeanSDO,ref CommonParam param,int count,ref List<OutPatientPresADO> lstOut, ref bool IsSuccess, List<long> useTimes = null)
		{
			try
			{
                OutPatientPresADO ado = new OutPatientPresADO();
                if (useTimes != null && useTimes.Count > 0)
                {
                    ado.USE_TIME = useTimes.First();
                }
                else
                {
                    ado.USE_TIME = 0;
                }
                if(takeBeanSDO is TakeBeanSDO)
                {
                    var obj = takeBeanSDO as TakeBeanSDO;
                    if(obj.BeanIds != null && obj.BeanIds.Count > 0 && medicineTypeSDO.dicUseTimeBeanIds != null && medicineTypeSDO.dicUseTimeBeanIds.Count > 0 && medicineTypeSDO.dicUseTimeBeanIds.ContainsKey(ado.USE_TIME))
                    {
                        obj.BeanIds = medicineTypeSDO.dicUseTimeBeanIds[ado.USE_TIME];
                    }    
                }
                else if (takeBeanSDO is TakeBeanByMameSDO)
                {
                    var obj = takeBeanSDO as TakeBeanByMameSDO;
                    if (obj.BeanIds != null && obj.BeanIds.Count > 0 && medicineTypeSDO.dicUseTimeBeanIds != null && medicineTypeSDO.dicUseTimeBeanIds.Count > 0 && medicineTypeSDO.dicUseTimeBeanIds.ContainsKey(ado.USE_TIME))
                    {
                        obj.BeanIds = medicineTypeSDO.dicUseTimeBeanIds[ado.USE_TIME];
                    }
                }    
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => takeBeanSDO), takeBeanSDO));
                if (medicineTypeSDO.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC)
                {
                    var rs = CallApiTakeBean<List<V_HIS_MEDICINE_BEAN_1>>(uriForTakeBean, takeBeanSDO,param, useTimes != null);
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => rs), rs));
                    if (rs == null || rs.Count == 0)
                    {
                        IsSuccess = false;
                        throw new AggregateException("TakeOrUpdateBean => 2.1 Them/sua dong du lieu thuoc that bai. Du lieu tra ve count = 0");
                    }
                    ado.TAKE_BEAN_ID = rs.Select(o => o.ID).ToList();
                    if(medicineTypeSDO.dicUseTimeBeanIds == null)
                    {
                        medicineTypeSDO.dicUseTimeBeanIds = new Dictionary<long, List<long>>();                       
                    }
                    medicineTypeSDO.dicUseTimeBeanIds[ado.USE_TIME] = new List<long>();
                    medicineTypeSDO.dicUseTimeBeanIds[ado.USE_TIME].AddRange(ado.TAKE_BEAN_ID);
                    if (count == 0)
                        medicineTypeSDO.MedicineBean1Result = rs;
                    else
                        medicineTypeSDO.MedicineBean1Result.AddRange(rs);
                }
                else if (medicineTypeSDO.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__VT)
                {
                    var rs = CallApiTakeBean<List<V_HIS_MATERIAL_BEAN_1>>(uriForTakeBean, takeBeanSDO, param, useTimes != null);
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => rs), rs));
                    if (rs == null || rs.Count == 0)
                    {
                        IsSuccess = false;
                        throw new AggregateException("TakeOrUpdateBean => 2.2 Them/sua dong du lieu vat tu that bai. Du lieu tra ve count = 0");
                    }
                    ado.TAKE_BEAN_ID = rs.Select(o => o.ID).ToList();
                    if (medicineTypeSDO.dicUseTimeBeanIds == null)
                    {
                        medicineTypeSDO.dicUseTimeBeanIds = new Dictionary<long, List<long>>();
                    }
                    medicineTypeSDO.dicUseTimeBeanIds[ado.USE_TIME] = new List<long>();
                    medicineTypeSDO.dicUseTimeBeanIds[ado.USE_TIME].AddRange(ado.TAKE_BEAN_ID);
                    if (count == 0)
                        medicineTypeSDO.MaterialBean1Result = rs;
                    else
                        medicineTypeSDO.MaterialBean1Result.AddRange(rs);
                }
                else
                {
                    throw new AggregateException("TakeOrUpdateBean => 3. Loi khong xac dinh duoc loai dich vu, servicetypeid khong hop le.");
                }
                if (lstOut == null)
                    lstOut = new List<OutPatientPresADO>();
                ado.PrimaryKey = medicineTypeSDO.PrimaryKey;
                ado.MEDI_MATE_ID = medicineTypeSDO.ID;
                ado.SERVICE_TYPE_ID = medicineTypeSDO.SERVICE_TYPE_ID;
                lstOut.Add(ado);
            }
			catch (Exception ex)
			{
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
		}

        static object GetDataForTakeBean(List<long> intructionTimes, MediMatyTypeADO medicineTypeSDO, List<long> beanIds)
        {
            //if (HisConfigCFG.IsAutoRoundUpByConvertUnitRatio && medicineTypeSDO.CONVERT_RATIO.HasValue && medicineTypeSDO.CONVERT_RATIO.Value < 1)
            //{
            //    decimal amount = (Math.Ceiling((medicineTypeSDO.AMOUNT ?? 0) * medicineTypeSDO.CONVERT_RATIO.Value)) / (medicineTypeSDO.CONVERT_RATIO.Value);

            //    if (amount != medicineTypeSDO.AMOUNT)
            //    {
            //        Inventec.Common.Logging.LogSystem.Debug("Ke don co cau hinh IsAutoRoundUpByConvertUnitRatio & CONVERT_RATIO < 1 & so luong lam tron theo don vi quy doi khac so luong ke____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => medicineTypeSDO.MEDICINE_TYPE_NAME), medicineTypeSDO.MEDICINE_TYPE_NAME) + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => medicineTypeSDO.MEDICINE_TYPE_CODE), medicineTypeSDO.MEDICINE_TYPE_CODE) + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => medicineTypeSDO.AMOUNT), medicineTypeSDO.AMOUNT) + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => amount), amount) + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => medicineTypeSDO.CONVERT_RATIO), medicineTypeSDO.CONVERT_RATIO));
            //        medicineTypeSDO.AMOUNT = amount;
            //    }
            //}

            if (medicineTypeSDO.IsAssignPackage.HasValue && medicineTypeSDO.IsAssignPackage.Value)
            {
                TakeBeanByMameSDO takeBeanByMameSDO = new TakeBeanByMameSDO();
                takeBeanByMameSDO.Amount = ((medicineTypeSDO.IsUseOrginalUnitForPres ?? false) == false && (medicineTypeSDO.CONVERT_RATIO ?? 0) > 0) ? (medicineTypeSDO.AMOUNT ?? 0) / medicineTypeSDO.CONVERT_RATIO.Value : (medicineTypeSDO.AMOUNT ?? 0);
                takeBeanByMameSDO.BeanIds = beanIds;
                takeBeanByMameSDO.ClientSessionKey = GlobalStore.ClientSessionKey;
                takeBeanByMameSDO.MediStockId = medicineTypeSDO.MEDI_STOCK_ID ?? 0;
                takeBeanByMameSDO.PatientTypeId = medicineTypeSDO.PATIENT_TYPE_ID;
                takeBeanByMameSDO.MameId = medicineTypeSDO.MAME_ID ?? 0;
                if (medicineTypeSDO.ExpMestDetailIds != null && medicineTypeSDO.ExpMestDetailIds.Count > 0)
                    takeBeanByMameSDO.ExpMestDetailIds = medicineTypeSDO.ExpMestDetailIds;

                return takeBeanByMameSDO;
            }
            else
            {
                TakeBeanSDO takeBeanSDO = new TakeBeanSDO();
                if (HisConfigCFG.IsDontPresExpiredTime)
                {
                    takeBeanSDO.ExpiredDate = medicineTypeSDO.IntructionTimeSelecteds != null && medicineTypeSDO.IntructionTimeSelecteds.Count > 0 ? (long?)medicineTypeSDO.IntructionTimeSelecteds.OrderByDescending(o => o).First() : intructionTimes.OrderByDescending(o => o).First();
                }
                takeBeanSDO.ClientSessionKey = GlobalStore.ClientSessionKey;
                takeBeanSDO.MediStockId = medicineTypeSDO.MEDI_STOCK_ID ?? 0;
                takeBeanSDO.TypeId = medicineTypeSDO.ID;
                takeBeanSDO.Amount = ((medicineTypeSDO.IsUseOrginalUnitForPres ?? false) == false && (medicineTypeSDO.CONVERT_RATIO ?? 0) > 0) ? (medicineTypeSDO.AMOUNT ?? 0) / medicineTypeSDO.CONVERT_RATIO.Value : (medicineTypeSDO.AMOUNT ?? 0);
                takeBeanSDO.BeanIds = beanIds;
                takeBeanSDO.PatientTypeId = medicineTypeSDO.PATIENT_TYPE_ID;
                if (medicineTypeSDO.ExpMestDetailIds != null && medicineTypeSDO.ExpMestDetailIds.Count > 0)
                    takeBeanSDO.ExpMestDetailIds = medicineTypeSDO.ExpMestDetailIds;

                return takeBeanSDO;
            }
            return null;
        }

        static object GetDataForTakeBean(List<long> intructionTimes, MediMatyTypeADO medicineTypeSDO, List<long> beanIds, decimal amount)
        {
            //if (HisConfigCFG.IsAutoRoundUpByConvertUnitRatio && medicineTypeSDO.CONVERT_RATIO.HasValue && medicineTypeSDO.CONVERT_RATIO.Value < 1)
            //{
            //    amount = (Math.Ceiling(amount * medicineTypeSDO.CONVERT_RATIO.Value)) / (medicineTypeSDO.CONVERT_RATIO.Value);
            //    if (amount != medicineTypeSDO.AMOUNT)
            //    {
            //        Inventec.Common.Logging.LogSystem.Debug("Ke don co cau hinh IsAutoRoundUpByConvertUnitRatio & CONVERT_RATIO < 1 & so luong lam tron theo don vi quy doi khac so luong ke____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => medicineTypeSDO.MEDICINE_TYPE_NAME), medicineTypeSDO.MEDICINE_TYPE_NAME) + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => medicineTypeSDO.MEDICINE_TYPE_CODE), medicineTypeSDO.MEDICINE_TYPE_CODE) + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => medicineTypeSDO.AMOUNT), medicineTypeSDO.AMOUNT) + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => amount), amount) + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => medicineTypeSDO.CONVERT_RATIO), medicineTypeSDO.CONVERT_RATIO));
            //        medicineTypeSDO.AMOUNT = amount;
            //    }
            //}

            if (medicineTypeSDO.IsAssignPackage.HasValue && medicineTypeSDO.IsAssignPackage.Value)
            {
                TakeBeanByMameSDO takeBeanByMameSDO = new TakeBeanByMameSDO();
                //if (HisConfigCFG.IsDontPresExpiredTime)
                //{
                //    takeBeanByMameSDO.ExpiredDate = medicineTypeSDO.IntructionTimeSelecteds != null && medicineTypeSDO.IntructionTimeSelecteds.Count > 0 ? (long?)medicineTypeSDO.IntructionTimeSelecteds.OrderByDescending(o => o).First() : intructionTimes.OrderByDescending(o => o).First();
                //}
                takeBeanByMameSDO.Amount = ((medicineTypeSDO.IsUseOrginalUnitForPres ?? false) == false && (medicineTypeSDO.CONVERT_RATIO ?? 0) > 0) ? amount / medicineTypeSDO.CONVERT_RATIO.Value : amount;
                takeBeanByMameSDO.BeanIds = beanIds;
                takeBeanByMameSDO.ClientSessionKey = GlobalStore.ClientSessionKey;
                takeBeanByMameSDO.MediStockId = medicineTypeSDO.MEDI_STOCK_ID ?? 0;
                takeBeanByMameSDO.PatientTypeId = medicineTypeSDO.PATIENT_TYPE_ID;
                takeBeanByMameSDO.MameId = medicineTypeSDO.MAME_ID ?? 0;
                if (medicineTypeSDO.ExpMestDetailIds != null && medicineTypeSDO.ExpMestDetailIds.Count > 0)
                    takeBeanByMameSDO.ExpMestDetailIds = medicineTypeSDO.ExpMestDetailIds;

                return takeBeanByMameSDO;
            }
            else
            {
                TakeBeanSDO takeBeanSDO = new TakeBeanSDO();
                if (HisConfigCFG.IsDontPresExpiredTime)
                {
                    takeBeanSDO.ExpiredDate = medicineTypeSDO.IntructionTimeSelecteds != null && medicineTypeSDO.IntructionTimeSelecteds.Count > 0 ? (long?)medicineTypeSDO.IntructionTimeSelecteds.OrderByDescending(o => o).First() : intructionTimes.OrderByDescending(o => o).First();
                }
                takeBeanSDO.ClientSessionKey = GlobalStore.ClientSessionKey;
                takeBeanSDO.MediStockId = medicineTypeSDO.MEDI_STOCK_ID ?? 0;
                takeBeanSDO.TypeId = medicineTypeSDO.ID;
                takeBeanSDO.Amount = ((medicineTypeSDO.IsUseOrginalUnitForPres ?? false) == false && (medicineTypeSDO.CONVERT_RATIO ?? 0) > 0) ? amount / medicineTypeSDO.CONVERT_RATIO.Value : amount;
                takeBeanSDO.BeanIds = beanIds;
                takeBeanSDO.PatientTypeId = medicineTypeSDO.PATIENT_TYPE_ID;
                if (medicineTypeSDO.ExpMestDetailIds != null && medicineTypeSDO.ExpMestDetailIds.Count > 0)
                    takeBeanSDO.ExpMestDetailIds = medicineTypeSDO.ExpMestDetailIds;

                return takeBeanSDO;
            }
            return null;
        }

        static string GetUriForTakeBean(MediMatyTypeADO medicineTypeSDO)
        {
            string uriForTakeBean = ((medicineTypeSDO.IsAssignPackage.HasValue && medicineTypeSDO.IsAssignPackage.Value) ?
                (medicineTypeSDO.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC ? RequestUriStore.HIS_MEDICINE_BEAN__TAKEBEAN_BY_MEDICINE : RequestUriStore.HIS_MATERIAL_BEAN__TAKEBEAN_BY_MATERIAL) :
                medicineTypeSDO.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC ? RequestUriStore.HIS_MEDICINE_BEAN__TAKEBEAN : RequestUriStore.HIS_MATERIAL_BEAN__TAKEBEAN);
            return uriForTakeBean;
        }

        internal static bool TakeForUpdateBean(List<long> intructionTimes, long expMestId, MediMatyTypeADO medicineTypeSDO, decimal amount, bool hasWarmError, CommonParam param, List<long> useTimeSelecteds, List<OutPatientPresADO> lstOut, bool IsCellChange = false)
        {
            bool result = true;
            try
            {
                if ((medicineTypeSDO.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC
                    || medicineTypeSDO.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.VATTU)
                    && (!GlobalStore.IsTreatmentIn || GlobalStore.IsCabinet)
                    )
                {
                    List<long> beanIds = null;
                    if (medicineTypeSDO.MedicineBean1Result != null && medicineTypeSDO.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC)
                    {
                        beanIds = medicineTypeSDO.MedicineBean1Result.Select(o => o.ID).ToList();

                    }
                    else if (medicineTypeSDO.MaterialBean1Result != null && medicineTypeSDO.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__VT)
                    {
                        beanIds = medicineTypeSDO.MaterialBean1Result.Select(o => o.ID).ToList();
                    }
                    else if (medicineTypeSDO.BeanIds != null && medicineTypeSDO.BeanIds.Count > 0)
                    {
                        beanIds = medicineTypeSDO.BeanIds;
                    }

                    LogSystem.Debug("TakeOrUpdateBean => 1.");
                    //string uriForTakeBean = (medicineTypeSDO.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC ? RequestUriStore.HIS_MEDICINE_BEAN__TAKEBEAN : RequestUriStore.HIS_MATERIAL_BEAN__TAKEBEAN);

                    if (amount == 0) return true;

                    string uriForTakeBean = GetUriForTakeBean(medicineTypeSDO);
                    object takeBeanSDO = GetDataForTakeBean(intructionTimes, medicineTypeSDO, beanIds, amount);
                    bool IsSucess = true;
                    if (useTimeSelecteds != null && useTimeSelecteds.Count > 0)
                    {
                        for (int i = 0; i < useTimeSelecteds.Count; i++)
                        {
                            CallApiTakeBean(ref medicineTypeSDO, uriForTakeBean, takeBeanSDO, ref param, i, ref lstOut, ref IsSucess, new List<long>() { useTimeSelecteds[i] });
                        }
                    }
                    else
                    {
                        CallApiTakeBean(ref medicineTypeSDO, uriForTakeBean, takeBeanSDO, ref param, 0, ref lstOut, ref IsSucess);
                    }
                    //if (medicineTypeSDO.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC)
                    //{
                    //    var rs = CallApiTakeBean<List<V_HIS_MEDICINE_BEAN_1>>(uriForTakeBean, takeBeanSDO, param);
                    //    if (rs == null || rs.Count == 0)
                    //        throw new AggregateException("TakeOrUpdateBean => 2.1 Them/sua dong du lieu thuoc that bai. Du lieu tra ve count = 0");
                    //    medicineTypeSDO.MedicineBean1Result = rs;
                    //}
                    //else if (medicineTypeSDO.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__VT)
                    //{
                    //    var rs = CallApiTakeBean<List<V_HIS_MATERIAL_BEAN_1>>(uriForTakeBean, takeBeanSDO, param);
                    //    if (rs == null || rs.Count == 0)
                    //        throw new AggregateException("TakeOrUpdateBean => 2.2 Them/sua dong du lieu vat tu that bai. Du lieu tra ve count = 0");
                    //    medicineTypeSDO.MaterialBean1Result = rs;
                    //}
                    //else
                    //{
                    //    throw new AggregateException("TakeOrUpdateBean => 3. Loi khong xac dinh duoc loai dich vu, servicetypeid khong hop le.");
                    //}

                    if (medicineTypeSDO.MedicineBean1Result != null && medicineTypeSDO.MedicineBean1Result.Count > 0)
                    {
                        // medicineTypeSDO.AMOUNT = medicineTypeSDO.MedicineBean1Result.Sum(o => o.AMOUNT);
                        medicineTypeSDO.TotalPrice = medicineTypeSDO.MedicineBean1Result.Sum(o => ((o.TDL_MEDICINE_IMP_PRICE * o.AMOUNT) * (1 + o.TDL_MEDICINE_IMP_VAT_RATIO)));
                    }
                    else if (medicineTypeSDO.MaterialBean1Result != null && medicineTypeSDO.MaterialBean1Result.Count > 0)
                    {
                        //medicineTypeSDO.AMOUNT = medicineTypeSDO.MaterialBean1Result.Sum(o => o.AMOUNT);
                        medicineTypeSDO.TotalPrice = medicineTypeSDO.MaterialBean1Result.Sum(o => ((o.TDL_MATERIAL_IMP_PRICE * o.AMOUNT) * (1 + o.TDL_MATERIAL_IMP_VAT_RATIO)));
                    }

                    medicineTypeSDO.ErrorTypeMediMatyBean = DevExpress.XtraEditors.DXErrorProvider.ErrorType.None;
                    medicineTypeSDO.ErrorMessageMediMatyBean = "";
                    if (!IsSucess ||!string.IsNullOrEmpty(param.GetMessage()))
                    {
                        return false;
                    }
                }
                LogSystem.Debug("TakeOrUpdateBean => 4.");
            }
            catch (AggregateException ex)
            {
                result = false;
                //if (hasWarmError)
                //{
                //    medicineTypeSDO.ErrorTypeMediMatyBean = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                //    medicineTypeSDO.ErrorMessageMediMatyBean = ResourceMessage.TakeBeanForUpdateThatBai;
                //}
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        internal static T CallApiTakeBean<T>(string uri, object data, CommonParam param, bool IsUseTime = false)
        {
            T t = default(T);
            try
            {
                t = new Inventec.Common.Adapter.BackendAdapter(param).Post<T>(uri, ApiConsumers.MosConsumer, data, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                Inventec.Common.Logging.LogSystem.Debug("uri = " + uri + ". Du lieu dau vao____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => data), data) + ". Du lieu dau ra____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => t), t) + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => param), param));
            }
            catch (Exception ex)
            {
                t = default(T);
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return t;
        }

        internal static bool ProcessTakeListMedi(List<long> intructionTimes, List<MediMatyTypeADO> medis, HIS_SERVICE_REQ serviceReqMain, List<long> useTimeSelecteds, List<OutPatientPresADO> lstOut)
        {
            bool result = true;
            try
            {
                if (!GlobalStore.IsTreatmentIn || GlobalStore.IsCabinet)
                {
                    if (medis == null || medis.Count == 0)
                    {
                        throw new ArgumentNullException("medis");
                    }
                    List<TakeBeanSDO> takeBeanSDOs = new List<TakeBeanSDO>();
                    foreach (var item in medis)
                    {
                        if ((item.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC)
                            && ((!GlobalStore.IsTreatmentIn || !GlobalStore.IsCabinet)
                            && !(serviceReqMain != null && serviceReqMain.IS_MAIN_EXAM != 1 && HisConfigCFG.IsUsingSubPrescriptionMechanism == "1")) || GlobalStore.IsCabinet)
                        {
                            TakeBeanSDO takeBeanSDO = new TakeBeanSDO();
                            takeBeanSDO.ClientSessionKey = GlobalStore.ClientSessionKey;

                            //if (HisConfigCFG.IsAutoRoundUpByConvertUnitRatio && item.CONVERT_RATIO.HasValue && item.CONVERT_RATIO.Value < 1)
                            //{
                            //    decimal amount = (Math.Ceiling((item.AMOUNT ?? 0) * item.CONVERT_RATIO.Value)) / (item.CONVERT_RATIO.Value);
                            //    if (amount != item.AMOUNT)
                            //    {
                            //        Inventec.Common.Logging.LogSystem.Debug("Ke don co cau hinh IsAutoRoundUpByConvertUnitRatio & CONVERT_RATIO < 1 & so luong lam tron theo don vi quy doi khac so luong ke____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => item.MEDICINE_TYPE_NAME), item.MEDICINE_TYPE_NAME) + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => item.MEDICINE_TYPE_CODE), item.MEDICINE_TYPE_CODE) + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => item.AMOUNT), item.AMOUNT) + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => amount), amount) + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => item.CONVERT_RATIO), item.CONVERT_RATIO));
                            //        item.AMOUNT = amount;
                            //    }
                            //}

                            takeBeanSDO.Amount = ((item.IsUseOrginalUnitForPres ?? false) == false && (item.CONVERT_RATIO ?? 0) > 0) ? (item.AMOUNT ?? 0) / item.CONVERT_RATIO.Value : (item.AMOUNT ?? 0);
                            takeBeanSDO.MediStockId = (item.MEDI_STOCK_ID ?? 0);
                            takeBeanSDO.TypeId = item.ID;
                            takeBeanSDO.PatientTypeId = item.PATIENT_TYPE_ID;
                            if (HisConfigCFG.IsDontPresExpiredTime)
                            {
                                takeBeanSDO.ExpiredDate = item.IntructionTimeSelecteds != null && item.IntructionTimeSelecteds.Count > 0 ? (long?)item.IntructionTimeSelecteds.OrderByDescending(o => o).First() : intructionTimes.OrderByDescending(o => o).First();
                            }
                            takeBeanSDOs.Add(takeBeanSDO);
                        }
                    }
                    if (takeBeanSDOs != null && takeBeanSDOs.Count > 0)
                    {
                        if (useTimeSelecteds != null && useTimeSelecteds.Count > 0)
                        {
                            for (int i = 0; i < useTimeSelecteds.Count; i++)
                            {
                                CallApiTakenListMedi(takeBeanSDOs,ref medis,ref lstOut,new List<long>() { useTimeSelecteds[i] });
                            }
                        }
                        else
                        {
                            CallApiTakenListMedi(takeBeanSDOs,ref medis, ref lstOut);
                        }
                        Inventec.Common.Logging.LogSystem.Debug("ProcessTakeListMedi => 2");
                    }
                }
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private static void CallApiTakenListMedi(List<TakeBeanSDO> takeBeanSDOs, ref List<MediMatyTypeADO> medis,ref List<OutPatientPresADO> lstOut, List<long> useTimes = null)
		{
			try
            {
                CommonParam param = new CommonParam();
                var rs = new Inventec.Common.Adapter.BackendAdapter(param).Post<List<TakeMedicineBeanListResultSDO>>(
                    RequestUriStore.HIS_MEDICINE_BEAN__TAKEBEANLIST,
                    ApiConsumers.MosConsumer,
                    takeBeanSDOs,
                    HIS.Desktop.Controls.Session.SessionManager.ActionLostToken,
                    param);

                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => takeBeanSDOs), takeBeanSDOs));
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => rs), rs));
                bool result = (rs != null && rs.Count > 0);
                if (result)
                {
                    foreach (var mbSDO in rs)
                    {
                        var mdst = BackendDataWorker.Get<V_HIS_MEDI_STOCK>().FirstOrDefault(o => o.ID == mbSDO.Request.MediStockId);
                        var mediOne = medis.FirstOrDefault(o => o.ID == mbSDO.Request.TypeId
                            && ((mdst != null && mdst.IS_BUSINESS != 1) || (mdst != null && mdst.IS_BUSINESS == 1 && o.PATIENT_TYPE_ID == mbSDO.Request.PatientTypeId))
                            && o.MEDI_STOCK_ID == mbSDO.Request.MediStockId
                            && ((o.IsUseOrginalUnitForPres ?? false) == false ? (o.AMOUNT / (o.CONVERT_RATIO ?? 1)) : o.AMOUNT) == mbSDO.Request.Amount);

                        if (mediOne != null)
                        {
                            mediOne.MedicineBean1Result = new List<V_HIS_MEDICINE_BEAN_1>();
                            if (mbSDO.Result != null && mbSDO.Result.Count > 0)
                            {
                                OutPatientPresADO ado = new OutPatientPresADO();
                                foreach (var vmb in mbSDO.Result)
                                {
                                    V_HIS_MEDICINE_BEAN_1 mb1 = new V_HIS_MEDICINE_BEAN_1();
                                    Inventec.Common.Mapper.DataObjectMapper.Map<V_HIS_MEDICINE_BEAN_1>(mb1, vmb);
                                    mediOne.MedicineBean1Result.Add(mb1);
                                }
                                if (lstOut == null)
                                    lstOut = new List<OutPatientPresADO>();
                                ado.TAKE_BEAN_ID = mbSDO.Result.Select(o => o.ID).ToList();
                                ado.PrimaryKey = mediOne.PrimaryKey;
                                ado.MEDI_MATE_ID = mediOne.ID;
                                ado.SERVICE_TYPE_ID = mediOne.SERVICE_TYPE_ID;
                                if (useTimes != null && useTimes.Count > 0)
                                {
                                    ado.USE_TIME = useTimes.First();
                                }
                                else
                                {
                                    ado.USE_TIME = 0;
                                }
                                if (mediOne.dicUseTimeBeanIds == null)
                                {
                                    mediOne.dicUseTimeBeanIds = new Dictionary<long, List<long>>();
                                }
                                mediOne.dicUseTimeBeanIds[ado.USE_TIME] = new List<long>();
                                mediOne.dicUseTimeBeanIds[ado.USE_TIME] = ado.TAKE_BEAN_ID;
                                lstOut.Add(ado);
                                if (mediOne.MedicineBean1Result != null && mediOne.MedicineBean1Result.Count > 0)
                                {
                                    mediOne.TotalPrice = mediOne.MedicineBean1Result.Sum(o => ((o.TDL_MEDICINE_IMP_PRICE * o.AMOUNT) * (1 + o.TDL_MEDICINE_IMP_VAT_RATIO)));
                                }
                            }
                            else
                            {
                                Inventec.Common.Logging.LogSystem.Debug("Take bean tra ve du lieu nhung chi tiet bean khong co du lieu. Du lieu ket qua tra ve____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => mbSDO), mbSDO));
                            }
                        }
                    }
                    if (rs.Count < takeBeanSDOs.Count)
                    {
                        Inventec.Common.Logging.LogSystem.Debug("Take medicine bean thanh cong, tra ve du lieu nhung chi tiet co 1 so dong that bai, cac dong that bai se remove khoi danh sach tra ve, rs.Count = " + rs.Count + ", takeBeanSDOs.Count = " + takeBeanSDOs.Count + ". Du lieu ket qua tra ve____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => rs), rs) + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => param), param) + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => takeBeanSDOs), takeBeanSDOs));

                        if ((param.Messages != null && param.Messages.Count > 0) || (param.BugCodes != null && param.BugCodes.Count > 0))
                        {
                            var mtErrors = medis.Where(o => (o.MedicineBean1Result == null || o.MedicineBean1Result.Count == 0) && (o.MaterialBean1Result == null || o.MaterialBean1Result.Count == 0)).Select(o => o.MEDICINE_TYPE_NAME);
                            param.Messages.Insert(0, String.Format("Thuốc {0}", String.Join(", ", mtErrors)));
                            MessageManager.Show(param, null);
                        }
                    }
                }
                else
                {
                    MessageManager.Show(param, result);
                    Inventec.Common.Logging.LogSystem.Debug("ProcessTakeListMedi => Goi api take list medicine bean that bai, url = " + RequestUriStore.HIS_MEDICINE_BEAN__TAKEBEANLIST + ". Du lieu dau vao____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => takeBeanSDOs), takeBeanSDOs) + ". Du lieu dau ra____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => result), result) + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => param), param));
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private static void CallApiTakenListMaty(List<TakeBeanSDO> takeBeanSDOs, ref List<MediMatyTypeADO> matys, ref List<OutPatientPresADO> lstOut,ref bool IsSucess,ref CommonParam param, List<long> useTimes = null)
        {
            try
            {
                var rs = new Inventec.Common.Adapter.BackendAdapter(param).Post<List<TakeMaterialBeanListResultSDO>>(
                    RequestUriStore.HIS_MATERIAL_BEAN__TAKEBEANLIST,
                    ApiConsumers.MosConsumer,
                    takeBeanSDOs,
                    HIS.Desktop.Controls.Session.SessionManager.ActionLostToken,
                    param);

                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => takeBeanSDOs), takeBeanSDOs));
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => rs), rs));
                bool result = IsSucess =(rs != null && rs.Count > 0);
                if (result)
                {
                    foreach (var mbSDO in rs)
                    {
                        var mdst = BackendDataWorker.Get<V_HIS_MEDI_STOCK>().FirstOrDefault(o => o.ID == mbSDO.Request.MediStockId);
                        var mediOne = matys.FirstOrDefault(o => o.ID == mbSDO.Request.TypeId
                            && ((mdst != null && mdst.IS_BUSINESS != 1) || (mdst != null && mdst.IS_BUSINESS == 1 && o.PATIENT_TYPE_ID == mbSDO.Request.PatientTypeId))
                            && o.MEDI_STOCK_ID == mbSDO.Request.MediStockId
                            && ((o.IsUseOrginalUnitForPres ?? false) == false ? (o.AMOUNT / (o.CONVERT_RATIO ?? 1)) : o.AMOUNT) == mbSDO.Request.Amount);

                        if (mediOne != null)
                        {
                            mediOne.MaterialBean1Result = new List<V_HIS_MATERIAL_BEAN_1>();
                            if (mbSDO.Result != null && mbSDO.Result.Count > 0)
                            {
                                OutPatientPresADO ado = new OutPatientPresADO();
                                foreach (var vmb in mbSDO.Result)
                                {
                                    V_HIS_MATERIAL_BEAN_1 mb1 = new V_HIS_MATERIAL_BEAN_1();
                                    Inventec.Common.Mapper.DataObjectMapper.Map<V_HIS_MATERIAL_BEAN_1>(mb1, vmb);
                                    mediOne.MaterialBean1Result.Add(mb1);
                                }
                                if (lstOut == null)
                                    lstOut = new List<OutPatientPresADO>();
                                ado.TAKE_BEAN_ID = mbSDO.Result.Select(o => o.ID).ToList();
                                ado.PrimaryKey = mediOne.PrimaryKey;
                                ado.MEDI_MATE_ID = mediOne.ID;
                                ado.SERVICE_TYPE_ID = mediOne.SERVICE_TYPE_ID;
                                if (useTimes != null && useTimes.Count > 0)
                                {
                                    ado.USE_TIME = useTimes.First();
                                }
                                else
                                {
                                    ado.USE_TIME = 0;
                                }
                                if (mediOne.dicUseTimeBeanIds == null)
                                {
                                    mediOne.dicUseTimeBeanIds = new Dictionary<long, List<long>>();
                                }
                                mediOne.dicUseTimeBeanIds[ado.USE_TIME] = new List<long>();
                                mediOne.dicUseTimeBeanIds[ado.USE_TIME] = ado.TAKE_BEAN_ID;
                                lstOut.Add(ado);
                                if (mediOne.MaterialBean1Result != null && mediOne.MaterialBean1Result.Count > 0)
                                {
                                    mediOne.TotalPrice = mediOne.MaterialBean1Result.Sum(o => ((o.TDL_MATERIAL_IMP_PRICE * o.AMOUNT) * (1 + o.TDL_MATERIAL_IMP_VAT_RATIO)));
                                }
                            }
                            else
                            {
                                Inventec.Common.Logging.LogSystem.Debug("Take bean tra ve du lieu nhung chi tiet bean khong co du lieu. Du lieu ket qua tra ve____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => mbSDO), mbSDO));
                            }
                        }
                    }
                    if (rs.Count < takeBeanSDOs.Count)
                    {
                        Inventec.Common.Logging.LogSystem.Debug("Take material bean thanh cong, tra ve du lieu nhung chi tiet co 1 so dong that bai, cac dong that bai se remove khoi danh sach tra ve, rs.Count = " + rs.Count + ", takeBeanSDOs.Count = " + takeBeanSDOs.Count + ". Du lieu ket qua tra ve____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => rs), rs) + "____" + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => takeBeanSDOs), takeBeanSDOs));
                        if ((param.Messages != null && param.Messages.Count > 0) || (param.BugCodes != null && param.BugCodes.Count > 0))
                        {
                            var mtErrors = matys.Where(o => (o.MedicineBean1Result == null || o.MedicineBean1Result.Count == 0) && (o.MaterialBean1Result == null || o.MaterialBean1Result.Count == 0)).Select(o => o.MEDICINE_TYPE_NAME);
                            param.Messages.Insert(0, String.Format("Vật tư {0}", String.Join(", ", mtErrors)));
                            MessageManager.Show(param, null);
                        }
                    }
                }
                else
                {
                    MessageManager.Show(param, result);
                    Inventec.Common.Logging.LogSystem.Debug("ProcessTakeListMaty => Goi api take list material bean that bai, url = " + RequestUriStore.HIS_MATERIAL_BEAN__TAKEBEANLIST + ". Du lieu dau vao____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => takeBeanSDOs), takeBeanSDOs) + ". Du lieu dau ra____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => result), result));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal static bool ProcessTakeListMaty(List<long> intructionTimes, List<MediMatyTypeADO> matys, HIS_SERVICE_REQ serviceReqMain, List<long> useTimeSelecteds, List<OutPatientPresADO> lstOut, ref CommonParam param)
        {
            bool result = true;
            try
            {
                if (!GlobalStore.IsTreatmentIn || GlobalStore.IsCabinet)
                {
                    if (matys == null || matys.Count == 0)
                    {
                        throw new ArgumentNullException("matys");
                    }
                    List<TakeBeanSDO> takeBeanSDOs = new List<TakeBeanSDO>();
                    foreach (var item in matys)
                    {
                        if ((item.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.VATTU)
                                && ((!GlobalStore.IsTreatmentIn || !GlobalStore.IsCabinet)
                                && !(serviceReqMain != null && serviceReqMain.IS_MAIN_EXAM != 1 && HisConfigCFG.IsUsingSubPrescriptionMechanism == "1")) || GlobalStore.IsCabinet)
                        {
                            TakeBeanSDO takeBeanSDO = new TakeBeanSDO();
                            takeBeanSDO.ClientSessionKey = GlobalStore.ClientSessionKey;

                            //if (HisConfigCFG.IsAutoRoundUpByConvertUnitRatio && item.CONVERT_RATIO.HasValue && item.CONVERT_RATIO.Value < 1)
                            //{
                            //    decimal amount = (Math.Ceiling((item.AMOUNT ?? 0) * item.CONVERT_RATIO.Value)) / (item.CONVERT_RATIO.Value);
                            //    if (amount != item.AMOUNT)
                            //    {
                            //        Inventec.Common.Logging.LogSystem.Debug("Ke don co cau hinh IsAutoRoundUpByConvertUnitRatio & CONVERT_RATIO < 1 & so luong lam tron theo don vi quy doi khac so luong ke____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => item.MEDICINE_TYPE_NAME), item.MEDICINE_TYPE_NAME) + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => item.MEDICINE_TYPE_CODE), item.MEDICINE_TYPE_CODE) + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => item.AMOUNT), item.AMOUNT) + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => amount), amount) + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => item.CONVERT_RATIO), item.CONVERT_RATIO));
                            //        item.AMOUNT = amount;
                            //    }
                            //}

                            takeBeanSDO.Amount = ((item.IsUseOrginalUnitForPres ?? false) == false && (item.CONVERT_RATIO ?? 0) > 0) ? (item.AMOUNT ?? 0) / item.CONVERT_RATIO.Value : (item.AMOUNT ?? 0);
                            takeBeanSDO.MediStockId = (item.MEDI_STOCK_ID ?? 0);
                            takeBeanSDO.TypeId = item.ID;
                            takeBeanSDO.PatientTypeId = item.PATIENT_TYPE_ID;
                            if (HisConfigCFG.IsDontPresExpiredTime)
                            {
                                takeBeanSDO.ExpiredDate = item.IntructionTimeSelecteds != null && item.IntructionTimeSelecteds.Count > 0 ? (long?)item.IntructionTimeSelecteds.OrderByDescending(o => o).First() : intructionTimes.OrderByDescending(o => o).First();
                            }

                            takeBeanSDOs.Add(takeBeanSDO);
                        }
                    }
                    bool IsSucess = true;
                    if (takeBeanSDOs != null && takeBeanSDOs.Count > 0)
                    {
                        if (useTimeSelecteds != null && useTimeSelecteds.Count > 0)
                        {
                            for (int i = 0; i < useTimeSelecteds.Count; i++)
                            {
                                CallApiTakenListMaty(takeBeanSDOs, ref matys, ref lstOut,ref IsSucess,ref param, new List<long>() { useTimeSelecteds[i] });
                            }
                        }
                        else
                        {
                            CallApiTakenListMaty(takeBeanSDOs, ref matys, ref lstOut, ref IsSucess, ref param);
                        }
                    }
                    if (!IsSucess || !string.IsNullOrEmpty(param.GetMessage()))
                        result = false;
                }
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        internal static bool ProcessTakeListMatyTSD(List<long> intructionTimes, List<MediMatyTypeADO> matys, HIS_SERVICE_REQ serviceReqMain, List<long> useTimeSelecteds, List<OutPatientPresADO> lstOut)
        {
            bool result = true;
            try
            {
                if (((!GlobalStore.IsTreatmentIn || !GlobalStore.IsCabinet)
                        && !(serviceReqMain != null && serviceReqMain.IS_MAIN_EXAM != 1 && HisConfigCFG.IsUsingSubPrescriptionMechanism == "1")) || GlobalStore.IsCabinet)
                {
                    if (matys == null || matys.Count == 0)
                    {
                        throw new ArgumentNullException("matys");
                    }

                    CommonParam param = new CommonParam();
                    CommonParam paramAlert = new CommonParam() { Messages = new List<string>() };
                    List<TakeBeanSDO> takeBeanSDOs = new List<TakeBeanSDO>();
                    foreach (var item in matys)
                    {
                        result = TakeForCreateBeanTSD(0, item, false, param,ref lstOut, useTimeSelecteds) && result;
                        if ((param.Messages != null && param.Messages.Count > 0) || (param.BugCodes != null && param.BugCodes.Count > 0))
                        {
                            paramAlert.Messages.AddRange(param.Messages);
                            paramAlert.BugCodes.AddRange(param.BugCodes);
                        }
                    }

                    if (!result && paramAlert.Messages.Count > 0)
                    {
                        MessageManager.Show(paramAlert, null);
                    }
                }
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        internal static bool ProcessReleaseAllMedi()
        {
            bool result = true;
            try
            {
                if (!GlobalStore.IsTreatmentIn || GlobalStore.IsCabinet)
                {
                    Inventec.Common.Logging.LogSystem.Debug("ProcessReleaseAllMedi => 1");
                    CommonParam param = new CommonParam();
                    Inventec.Common.Logging.LogSystem.Debug("ProcessReleaseAllMedi => Goi api release all bean, uri = " + RequestUriStore.HIS_MEDICINE_BEAN__RELEASEBEANALL + ". Du lieu dau vao____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => GlobalStore.ClientSessionKey), GlobalStore.ClientSessionKey));
                    result = new Inventec.Common.Adapter.BackendAdapter(param).Post<bool>(
                        RequestUriStore.HIS_MEDICINE_BEAN__RELEASEBEANALL,
                        ApiConsumers.MosConsumer,
                        GlobalStore.ClientSessionKey,
                        HIS.Desktop.Controls.Session.SessionManager.ActionLostToken,
                        param);

                    if (!result)
                    {
                        MessageManager.Show(param, result);
                        Inventec.Common.Logging.LogSystem.Debug("Goi api release list medicine bean that bai, url = " + RequestUriStore.HIS_MEDICINE_BEAN__RELEASEBEANALL + ". Du lieu dau vao____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => GlobalStore.ClientSessionKey), GlobalStore.ClientSessionKey) + ". Du lieu dau ra____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => result), result) + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => param), param));
                    }

                    Inventec.Common.Logging.LogSystem.Debug("ProcessReleaseAllMedi => 2");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        internal static bool ProcessReleaseAllMaty()
        {
            bool result = true;
            try
            {
                if (!GlobalStore.IsTreatmentIn || GlobalStore.IsCabinet)
                {
                    Inventec.Common.Logging.LogSystem.Debug("ProcessReleaseAllMaty => 1");
                    CommonParam param = new CommonParam();
                    Inventec.Common.Logging.LogSystem.Debug("ProcessReleaseAllMaty => Goi api release all bean, uri = " + RequestUriStore.HIS_MATERIAL_BEAN__RELEASEBEANALL + ". Du lieu dau vao____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => GlobalStore.ClientSessionKey), GlobalStore.ClientSessionKey));
                    result = new Inventec.Common.Adapter.BackendAdapter(param).Post<bool>(
                        RequestUriStore.HIS_MATERIAL_BEAN__RELEASEBEANALL,
                        ApiConsumers.MosConsumer,
                        GlobalStore.ClientSessionKey,
                        HIS.Desktop.Controls.Session.SessionManager.ActionLostToken,
                        param);

                    if (!result)
                    {
                        MessageManager.Show(param, result);
                        Inventec.Common.Logging.LogSystem.Debug("Goi api release list material bean that bai, url = " + RequestUriStore.HIS_MATERIAL_BEAN__RELEASEBEANALL + ". Du lieu dau vao____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => GlobalStore.ClientSessionKey), GlobalStore.ClientSessionKey) + ". Du lieu dau ra____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => result), result) + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => param), param));
                    }

                    Inventec.Common.Logging.LogSystem.Debug("ProcessReleaseAllMaty => 2");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        /// <summary>
        /// Nếu là thuốc hoặc vật tư trong kho thì sẽ gọi api bỏ bean, ngược lại trả về true để cho phép các xử lý tiếp theo
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        internal static bool ProcessDeleteRowMediMaty(List<long> intructionTimes, MediMatyTypeADO data)
        {
            bool result = true;
            try
            {
                if ((data.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC
                    || data.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.VATTU
                    || data.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.VATTU_TSD)
                    && (!GlobalStore.IsTreatmentIn || GlobalStore.IsCabinet)
                    )
                {
                    CommonParam param = new CommonParam();
                    object releaseBeanSDO = GetDataForReleaseBean(intructionTimes, data);
                    string url = GetUriForReleaseBean(data);
                    Inventec.Common.Logging.LogSystem.Debug("ProcessDeleteRowMediMaty => Goi api release one bean, uri = " + url + ". Du lieu dau vao____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => releaseBeanSDO), releaseBeanSDO));
                    result = new Inventec.Common.Adapter.BackendAdapter(param).Post<bool>(
                        url,
                        ApiConsumers.MosConsumer,
                        releaseBeanSDO,
                        HIS.Desktop.Controls.Session.SessionManager.ActionLostToken,
                        param);
                    if (!result)
                    {
                        MessageManager.Show(param, result);
                        Inventec.Common.Logging.LogSystem.Debug("Goi api bỏ bean that bai, url = " + url + ". Du lieu dau vao____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => releaseBeanSDO), releaseBeanSDO) + ". Du lieu dau ra____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => result), result) + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => param), param));
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        static object GetDataForReleaseBean(List<long> intructionTimes, MediMatyTypeADO medicineTypeSDO)
        {
            ReleaseBeanSDO releaseBeanSDO = new ReleaseBeanSDO();
            releaseBeanSDO.ClientSessionKey = GlobalStore.ClientSessionKey;
            releaseBeanSDO.MediStockId = medicineTypeSDO.MEDI_STOCK_ID ?? 0;
            releaseBeanSDO.TypeId = medicineTypeSDO.MAME_ID ?? 0;//TODO
            if (medicineTypeSDO.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC && medicineTypeSDO.MedicineBean1Result != null)
            {
                releaseBeanSDO.BeanIds = medicineTypeSDO.MedicineBean1Result.Select(o => o.ID).ToList();
            }
            else if ((medicineTypeSDO.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.VATTU || medicineTypeSDO.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.VATTU_TSD) && medicineTypeSDO.MaterialBean1Result != null)
            {
                releaseBeanSDO.BeanIds = medicineTypeSDO.MaterialBean1Result.Select(o => o.ID).ToList();
            }
            releaseBeanSDO.TypeId = medicineTypeSDO.ID;

            if (medicineTypeSDO.IsAssignPackage.HasValue && medicineTypeSDO.IsAssignPackage.Value)
            {
                releaseBeanSDO.MameId = medicineTypeSDO.MAME_ID;
            }
            return releaseBeanSDO;
        }

        static string GetUriForReleaseBean(MediMatyTypeADO medicineTypeSDO)
        {
            string uriForTakeBean = (
                medicineTypeSDO.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC ?
                RequestUriStore.HIS_MEDICINE_BEAN__RELEASEBEAN :
                RequestUriStore.HIS_MATERIAL_BEAN__RELEASEBEAN);
            return uriForTakeBean;
        }
    }
}

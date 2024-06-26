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
using HIS.Desktop.Plugins.AssignPrescriptionCLS.ADO;
using HIS.Desktop.Plugins.AssignPrescriptionCLS.Config;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HIS.Desktop.Plugins.AssignPrescriptionCLS
{
    class TakeOrReleaseBeanWorker
    {
        internal static bool TakeForCreateBeanTSD(long expMestId, MediMatyTypeADO medicineTypeSDO, bool isHasBean, CommonParam param)
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
                    if (rs == null)
                        throw new AggregateException("TakeForCreateBeanTSD => 2.2 Them/sua dong du lieu vat tu that bai. Du lieu tra ve count = 0");
                    V_HIS_MATERIAL_BEAN_1 materialBeanAdd = new MOS.EFMODEL.DataModels.V_HIS_MATERIAL_BEAN_1();
                    Inventec.Common.Mapper.DataObjectMapper.Map<V_HIS_MATERIAL_BEAN_1>(materialBeanAdd, rs);
                    medicineTypeSDO.MaterialBean1Result = medicineTypeSDO.MaterialBean1Result == null ? new List<MOS.EFMODEL.DataModels.V_HIS_MATERIAL_BEAN_1>() : medicineTypeSDO.MaterialBean1Result;

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

        internal static bool TakeForCreateBean(List<long> intructionTimes, long expMestId, MediMatyTypeADO medicineTypeSDO, bool isHasBean, CommonParam param)
        {
            bool result = true;
            try
            {
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => intructionTimes), intructionTimes)
                       + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => expMestId), expMestId)
                       + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => medicineTypeSDO), medicineTypeSDO)
                    + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => GlobalStore.IsCabinet), GlobalStore.IsCabinet)
                    + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => GlobalStore.IsTreatmentIn), GlobalStore.IsTreatmentIn));
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

                    if (medicineTypeSDO.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC)
                    {
                        var rs = CallApiTakeBean<List<V_HIS_MEDICINE_BEAN_1>>(uriForTakeBean, takeBeanSDO, param);
                        if (rs == null || rs.Count == 0)
                            throw new AggregateException("TakeOrUpdateBean => 2.1 Them/sua dong du lieu thuoc that bai. Du lieu tra ve count = 0");
                        medicineTypeSDO.MedicineBean1Result = rs;
                    }
                    else if (medicineTypeSDO.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__VT)
                    {
                        var rs = CallApiTakeBean<List<V_HIS_MATERIAL_BEAN_1>>(uriForTakeBean, takeBeanSDO, param);
                        if (rs == null || rs.Count == 0)
                            throw new AggregateException("TakeOrUpdateBean => 2.2 Them/sua dong du lieu vat tu that bai. Du lieu tra ve count = 0");
                        medicineTypeSDO.MaterialBean1Result = rs;
                    }
                    else
                    {
                        throw new AggregateException("TakeOrUpdateBean => 3. Loi khong xac dinh duoc loai dich vu, servicetypeid khong hop le.");
                    }

                    if (medicineTypeSDO.MedicineBean1Result != null && medicineTypeSDO.MedicineBean1Result.Count > 0)
                    {
                        medicineTypeSDO.TotalPrice = medicineTypeSDO.MedicineBean1Result.Sum(o => ((o.TDL_MEDICINE_IMP_PRICE * o.AMOUNT) * (1 + o.TDL_MEDICINE_IMP_VAT_RATIO)));
                    }
                    else if (medicineTypeSDO.MaterialBean1Result != null && medicineTypeSDO.MaterialBean1Result.Count > 0)
                    {
                        medicineTypeSDO.TotalPrice = medicineTypeSDO.MaterialBean1Result.Sum(o => ((o.TDL_MATERIAL_IMP_PRICE * o.AMOUNT) * (1 + o.TDL_MATERIAL_IMP_VAT_RATIO)));
                    }
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

        static object GetDataForTakeBean(List<long> intructionTimes, MediMatyTypeADO medicineTypeSDO, List<long> beanIds)
        {
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

        internal static bool TakeForUpdateBean(List<long> intructionTimes, long expMestId, MediMatyTypeADO medicineTypeSDO, decimal amount, bool hasWarmError, CommonParam param)
        {
            bool result = true;
            try
            {
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => intructionTimes), intructionTimes)
                    + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => expMestId), expMestId)
                    + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => amount), amount) 
                    + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => medicineTypeSDO), medicineTypeSDO)
                    + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => GlobalStore.IsCabinet), GlobalStore.IsCabinet)
                    + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => GlobalStore.IsTreatmentIn), GlobalStore.IsTreatmentIn));
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

                    if (medicineTypeSDO.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC)
                    {
                        var rs = CallApiTakeBean<List<V_HIS_MEDICINE_BEAN_1>>(uriForTakeBean, takeBeanSDO, param);
                        if (rs == null || rs.Count == 0)
                            throw new AggregateException("TakeOrUpdateBean => 2.1 Them/sua dong du lieu thuoc that bai. Du lieu tra ve count = 0");
                        medicineTypeSDO.MedicineBean1Result = rs;
                    }
                    else if (medicineTypeSDO.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__VT)
                    {
                        var rs = CallApiTakeBean<List<V_HIS_MATERIAL_BEAN_1>>(uriForTakeBean, takeBeanSDO, param);
                        if (rs == null || rs.Count == 0)
                            throw new AggregateException("TakeOrUpdateBean => 2.2 Them/sua dong du lieu vat tu that bai. Du lieu tra ve count = 0");
                        medicineTypeSDO.MaterialBean1Result = rs;
                    }
                    else
                    {
                        throw new AggregateException("TakeOrUpdateBean => 3. Loi khong xac dinh duoc loai dich vu, servicetypeid khong hop le.");
                    }

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

        internal static T CallApiTakeBean<T>(string uri, object data, CommonParam param)
        {
            T t = default(T);
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("CallApiTakeBean => Goi api take/release bean, uri = " + uri + ". Du lieu dau vao____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => data), data));
                t = new Inventec.Common.Adapter.BackendAdapter(param).Post<T>(uri, ApiConsumers.MosConsumer, data, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                if (t == null)
                {
                    Inventec.Common.Logging.LogSystem.Debug("CallApiTakeBean => Goi api take/release bean that bai, uri = " + uri + ". Du lieu dau vao____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => data), data) + ". Du lieu dau ra____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => t), t) + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => param), param));
                }
            }
            catch (Exception ex)
            {
                t = default(T);
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return t;
        }

        internal static bool ProcessTakeListMedi(List<long> intructionTimes, List<MediMatyTypeADO> medis)
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
                            && (!GlobalStore.IsTreatmentIn || GlobalStore.IsCabinet)
                            )
                        {
                            TakeBeanSDO takeBeanSDO = new TakeBeanSDO();
                            takeBeanSDO.ClientSessionKey = GlobalStore.ClientSessionKey;
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
                        CommonParam param = new CommonParam();
                        var rs = new Inventec.Common.Adapter.BackendAdapter(param).Post<List<TakeMedicineBeanListResultSDO>>(
                            RequestUriStore.HIS_MEDICINE_BEAN__TAKEBEANLIST,
                            ApiConsumers.MosConsumer,
                            takeBeanSDOs,
                            HIS.Desktop.Controls.Session.SessionManager.ActionLostToken,
                            param);

                        Inventec.Common.Logging.LogSystem.Info("HIS_MEDICINE_BEAN__TAKEBEANLIST: "+Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => rs), rs));
                        result = (rs != null && rs.Count > 0);
                        if (result)
                        {


                            foreach (var mbSDO in rs)
                            {
                                var mdst = BackendDataWorker.Get<V_HIS_MEDI_STOCK>().FirstOrDefault(o => o.ID == mbSDO.Request.MediStockId);
                                var mediOne = medis.FirstOrDefault(o => o.ID == mbSDO.Request.TypeId
                                    && ((mdst != null && mdst.IS_BUSINESS != 1) || (mdst != null && mdst.IS_BUSINESS == 1))
                                    && o.PATIENT_TYPE_ID == mbSDO.Request.PatientTypeId
                                    && o.MEDI_STOCK_ID == mbSDO.Request.MediStockId
                                    && ((o.IsUseOrginalUnitForPres ?? false) == false ? (o.AMOUNT / (o.CONVERT_RATIO ?? 1)) : o.AMOUNT) == mbSDO.Request.Amount);

                                if (mediOne != null)
                                {
                                    mediOne.MedicineBean1Result = new List<V_HIS_MEDICINE_BEAN_1>();
                                    if (mbSDO.Result != null && mbSDO.Result.Count > 0)
                                    {
                                        foreach (var vmb in mbSDO.Result)
                                        {
                                            V_HIS_MEDICINE_BEAN_1 mb1 = new V_HIS_MEDICINE_BEAN_1();
                                            Inventec.Common.Mapper.DataObjectMapper.Map<V_HIS_MEDICINE_BEAN_1>(mb1, vmb);
                                            mediOne.MedicineBean1Result.Add(mb1);
                                        }

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

        internal static bool ProcessTakeListMaty(List<long> intructionTimes, List<MediMatyTypeADO> matys)
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
                            && (!GlobalStore.IsTreatmentIn || GlobalStore.IsCabinet)
                            )
                        {
                            TakeBeanSDO takeBeanSDO = new TakeBeanSDO();
                            takeBeanSDO.ClientSessionKey = GlobalStore.ClientSessionKey;
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
                        CommonParam param = new CommonParam();
                        var rs = new Inventec.Common.Adapter.BackendAdapter(param).Post<List<TakeMaterialBeanListResultSDO>>(
                            RequestUriStore.HIS_MATERIAL_BEAN__TAKEBEANLIST,
                            ApiConsumers.MosConsumer,
                            takeBeanSDOs,
                            HIS.Desktop.Controls.Session.SessionManager.ActionLostToken,
                            param);

                        result = (rs != null && rs.Count > 0);
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
                                        foreach (var vmb in mbSDO.Result)
                                        {
                                            V_HIS_MATERIAL_BEAN_1 mb1 = new V_HIS_MATERIAL_BEAN_1();
                                            Inventec.Common.Mapper.DataObjectMapper.Map<V_HIS_MATERIAL_BEAN_1>(mb1, vmb);
                                            mediOne.MaterialBean1Result.Add(mb1);
                                        }

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
                                Inventec.Common.Logging.LogSystem.Debug("Take material bean thanh cong, tra ve du lieu nhung chi tiet co 1 so dong that bai, cac dong that bai se remove khoi danh sach tra ve, rs.Count = " + rs.Count + ", takeBeanSDOs.Count = " + takeBeanSDOs.Count + ". Du lieu ket qua tra ve____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => rs), rs) + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => param), param) + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => takeBeanSDOs), takeBeanSDOs));
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
                            Inventec.Common.Logging.LogSystem.Debug("ProcessTakeListMaty => Goi api take list material bean that bai, url = " + RequestUriStore.HIS_MATERIAL_BEAN__TAKEBEANLIST + ". Du lieu dau vao____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => takeBeanSDOs), takeBeanSDOs) + ". Du lieu dau ra____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => result), result) + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => param), param));
                        }
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

        internal static bool ProcessTakeListMatyTSD(List<long> intructionTimes, List<MediMatyTypeADO> matys)
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

                    CommonParam param = new CommonParam();
                    CommonParam paramAlert = new CommonParam() { Messages = new List<string>() };
                    List<TakeBeanSDO> takeBeanSDOs = new List<TakeBeanSDO>();
                    foreach (var item in matys)
                    {
                        result = TakeForCreateBeanTSD(0, item, false, param) && result;
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

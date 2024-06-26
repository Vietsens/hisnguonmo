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
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.AssignPrescriptionPK
{
    class TakeOrReleaseBeanUtil
    {
        internal static bool TakeOrReleaseBean(MediMatyTypeADO medicineTypeSDO, decimal amount, string InBean, CommonParam param)
        {
            bool result = false;
            try
            {
                if ((medicineTypeSDO.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC
                    || medicineTypeSDO.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.VATTU)
                    && !GlobalStore.IsTreatmentIn)
                {
                    List<long> beanIds = null;
                    if (InBean == "")
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
                    LogSystem.Info("TakeOrReleaseBean => 1");
                    string uriForTakeBean = (medicineTypeSDO.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC ? RequestUriStore.HIS_MEDICINE_BEAN__TAKEBEAN : RequestUriStore.HIS_MATERIAL_BEAN__TAKEBEAN);

                    TakeBeanSDO takeBeanSDO = new TakeBeanSDO();
                    if (amount == 0) return true;

                    takeBeanSDO.ClientSessionKey = GlobalStore.ClientSessionKey;
                    takeBeanSDO.MediStockId = medicineTypeSDO.MEDI_STOCK_ID ?? 0;
                    takeBeanSDO.TypeId = medicineTypeSDO.ID;
                    takeBeanSDO.Amount = amount;
                    takeBeanSDO.BeanIds = beanIds;

                    var mdst = BackendDataWorker.Get<V_HIS_MEDI_STOCK>().FirstOrDefault(o => o.ID == (medicineTypeSDO.MEDI_STOCK_ID ?? 0));
                    if (mdst != null && mdst.IS_BUSINESS == 1)
                        takeBeanSDO.PatientTypeId = medicineTypeSDO.PATIENT_TYPE_ID;

                    if (medicineTypeSDO.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC)
                    {
                        var rs = CallApiTakeReleaseBean<List<V_HIS_MEDICINE_BEAN_1>>(uriForTakeBean, takeBeanSDO, param);
                        if (rs == null || rs.Count == 0)
                            throw new AggregateException("TakeOrReleaseBean => Them/sua dong du lieu thuoc that bai. Du lieu tra ve count = 0");
                        medicineTypeSDO.MedicineBean1Result = rs;
                    }
                    else if (medicineTypeSDO.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__VT)
                    {
                        var rs = CallApiTakeReleaseBean<List<V_HIS_MATERIAL_BEAN_1>>(uriForTakeBean, takeBeanSDO, param);
                        if (rs == null || rs.Count == 0)
                            throw new AggregateException("TakeOrReleaseBean => Them/sua dong du lieu vat tu that bai. Du lieu tra ve count = 0");
                        medicineTypeSDO.MaterialBean1Result = rs;
                    }
                    else
                    {
                        throw new AggregateException("TakeOrReleaseBean => Loi khong xac dinh duoc loai dich vu, servicetypeid khong hop le.");
                    }
                }
                else
                    result = true;

                LogSystem.Info("TakeOrReleaseBean => 2");
            }
            catch (AggregateException ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        internal static T CallApiTakeReleaseBean<T>(string uri, object data, CommonParam param)
        {
            T t = default(T);
            try
            {
                t = new Inventec.Common.Adapter.BackendAdapter(param).Post<T>(uri, ApiConsumers.MosConsumer, data, param);
                if (t == null)
                {
                    Inventec.Common.Logging.LogSystem.Info("CallApiTakeReleaseBean => Goi api take/release bean that bai, uri = " + uri + ". Du lieu dau vao____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => data), data) + ". Du lieu dau ra____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => t), t) + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => param), param));
                }
            }
            catch (Exception ex)
            {
                t = default(T);
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return t;
        }

        internal static bool ProcessTakeListMedi(List<MediMatyTypeADO> medis)
        {
            bool result = false;
            try
            {
                if (!GlobalStore.IsTreatmentIn)
                {
                    Inventec.Common.Logging.LogSystem.Info("ProcessTakeListMedi => 1");
                    List<TakeBeanSDO> takeBeanSDOs = new List<TakeBeanSDO>();
                    foreach (var item in medis)
                    {
                        if ((item.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC
                            || item.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.VATTU)
                            && !GlobalStore.IsTreatmentIn)
                        {
                            TakeBeanSDO takeBeanSDO = new TakeBeanSDO();
                            takeBeanSDO.ClientSessionKey = GlobalStore.ClientSessionKey;
                            takeBeanSDO.Amount = (item.AMOUNT ?? 0);
                            takeBeanSDO.MediStockId = (item.MEDI_STOCK_ID ?? 0);
                            takeBeanSDO.TypeId = item.ID;

                            var mdst = BackendDataWorker.Get<V_HIS_MEDI_STOCK>().FirstOrDefault(o => o.ID == (item.MEDI_STOCK_ID ?? 0));
                            if (mdst != null && mdst.IS_BUSINESS == 1)
                                takeBeanSDO.PatientTypeId = item.PATIENT_TYPE_ID;

                            takeBeanSDOs.Add(takeBeanSDO);
                        }
                    }

                    if (takeBeanSDOs != null && takeBeanSDOs.Count > 0)
                    {
                        CommonParam param = new CommonParam();
                        var rs = new Inventec.Common.Adapter.BackendAdapter(param).Post<List<HIS_MEDICINE_BEAN>>(
                            RequestUriStore.HIS_MEDICINE_BEAN__TAKEBEANLIST,
                            ApiConsumers.MosConsumer,
                            takeBeanSDOs,
                            param);

                        result = (rs != null && rs.Count > 0);

                        if (!result)
                        {
                            MessageManager.Show(param, result);
                            Inventec.Common.Logging.LogSystem.Info("ProcessTakeListMedi => Goi api take list medicine bean that bai, url = " + RequestUriStore.HIS_MEDICINE_BEAN__TAKEBEANLIST + ". Du lieu dau vao____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => takeBeanSDOs), takeBeanSDOs) + ". Du lieu dau ra____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => result), result) + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => param), param));
                        }
                        Inventec.Common.Logging.LogSystem.Info("ProcessTakeListMedi => 2");
                    }
                    else
                        result = true;
                }
                else
                    result = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        internal static bool ProcessTakeListMaty(List<MediMatyTypeADO> matys)
        {
            bool result = false;
            try
            {
                if (!GlobalStore.IsTreatmentIn)
                {
                    Inventec.Common.Logging.LogSystem.Info("ProcessTakeListMaty => 1");
                    List<TakeBeanSDO> takeBeanSDOs = new List<TakeBeanSDO>();
                    foreach (var item in matys)
                    {
                        if ((item.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC
                            || item.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.VATTU)
                            && !GlobalStore.IsTreatmentIn)
                        {
                            TakeBeanSDO takeBeanSDO = new TakeBeanSDO();
                            takeBeanSDO.ClientSessionKey = GlobalStore.ClientSessionKey;
                            takeBeanSDO.Amount = (item.AMOUNT ?? 0);
                            takeBeanSDO.MediStockId = (item.MEDI_STOCK_ID ?? 0);
                            takeBeanSDO.TypeId = item.ID;
                            takeBeanSDOs.Add(takeBeanSDO);
                        }
                    }

                    if (takeBeanSDOs != null && takeBeanSDOs.Count > 0)
                    {
                        CommonParam param = new CommonParam();
                        var rs = new Inventec.Common.Adapter.BackendAdapter(param).Post<List<HIS_MATERIAL_BEAN>>(
                            RequestUriStore.HIS_MATERIAL_BEAN__TAKEBEANLIST,
                            ApiConsumers.MosConsumer,
                            takeBeanSDOs,
                            param);

                        result = (rs != null && rs.Count > 0);

                        if (!result)
                        {
                            MessageManager.Show(param, result);
                            Inventec.Common.Logging.LogSystem.Info("ProcessTakeListMaty => Goi api take list material bean that bai, url = " + RequestUriStore.HIS_MATERIAL_BEAN__TAKEBEANLIST + ". Du lieu dau vao____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => takeBeanSDOs), takeBeanSDOs) + ". Du lieu dau ra____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => result), result) + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => param), param));
                        }
                        Inventec.Common.Logging.LogSystem.Info("ProcessTakeListMaty => 2");
                    }
                    else
                        result = true;
                }
                else
                    result = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        internal static bool ProcessReleaseAllMedi()
        {
            bool result = false;
            try
            {
                if (!GlobalStore.IsTreatmentIn)
                {
                    Inventec.Common.Logging.LogSystem.Info("ProcessReleaseAllMedi => 1");
                    CommonParam param = new CommonParam();
                    result = new Inventec.Common.Adapter.BackendAdapter(param).Post<bool>(
                        RequestUriStore.HIS_MEDICINE_BEAN__RELEASEBEANALL,
                        ApiConsumers.MosConsumer,
                        GlobalStore.ClientSessionKey,
                        param);

                    if (!result)
                    {
                        MessageManager.Show(param, result);
                        Inventec.Common.Logging.LogSystem.Info("Goi api release list medicine bean that bai, url = " + RequestUriStore.HIS_MEDICINE_BEAN__RELEASEBEANALL + ". Du lieu dau vao____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => GlobalStore.ClientSessionKey), GlobalStore.ClientSessionKey) + ". Du lieu dau ra____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => result), result) + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => param), param));
                    }
                    Inventec.Common.Logging.LogSystem.Info("ProcessReleaseAllMedi => 2");
                }
                else
                    result = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        internal static bool ProcessReleaseAllMaty()
        {
            bool result = false;
            try
            {
                if (!GlobalStore.IsTreatmentIn)
                {
                    Inventec.Common.Logging.LogSystem.Info("ProcessReleaseAllMaty => 1");
                    CommonParam param = new CommonParam();
                    result = new Inventec.Common.Adapter.BackendAdapter(param).Post<bool>(
                        RequestUriStore.HIS_MATERIAL_BEAN__RELEASEBEANALL,
                        ApiConsumers.MosConsumer,
                        GlobalStore.ClientSessionKey,
                        param);

                    if (!result)
                    {
                        MessageManager.Show(param, result);
                        Inventec.Common.Logging.LogSystem.Info("Goi api release list material bean that bai, url = " + RequestUriStore.HIS_MATERIAL_BEAN__RELEASEBEANALL + ". Du lieu dau vao____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => GlobalStore.ClientSessionKey), GlobalStore.ClientSessionKey) + ". Du lieu dau ra____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => result), result) + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => param), param));
                    }
                    Inventec.Common.Logging.LogSystem.Info("ProcessReleaseAllMaty => 2");
                }
                else
                    result = true;
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
        internal static bool ProcessDeleteRowMediMaty(MediMatyTypeADO data)
        {
            bool result = false;
            try
            {
                if ((data.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC
                    || data.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.VATTU)
                    && !GlobalStore.IsTreatmentIn)
                {
                    CommonParam param = new CommonParam();
                    ReleaseBeanSDO releaseBeanSDO = new ReleaseBeanSDO();
                    releaseBeanSDO.ClientSessionKey = GlobalStore.ClientSessionKey;
                    releaseBeanSDO.MediStockId = data.MEDI_STOCK_ID ?? 0;
                    releaseBeanSDO.TypeId = data.ID;
                    if (data.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC && data.MedicineBean1Result != null)
                    {
                        releaseBeanSDO.BeanIds = data.MedicineBean1Result.Select(o => o.ID).ToList();
                    }
                    else if (data.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.VATTU && data.MaterialBean1Result != null)
                    {
                        releaseBeanSDO.BeanIds = data.MaterialBean1Result.Select(o => o.ID).ToList();
                    }
                    string url = (data.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC) ? RequestUriStore.HIS_MEDICINE_BEAN__RELEASEBEAN : RequestUriStore.HIS_MATERIAL_BEAN__RELEASEBEAN;
                    result = new Inventec.Common.Adapter.BackendAdapter(param).Post<bool>(
                        url,
                        ApiConsumers.MosConsumer,
                        releaseBeanSDO,
                        param);
                    if (!result)
                    {
                        MessageManager.Show(param, result);
                        Inventec.Common.Logging.LogSystem.Info("Goi api bỏ bean that bai, url = " + url + ". Du lieu dau vao____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => releaseBeanSDO), releaseBeanSDO) + ". Du lieu dau ra____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => result), result) + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => param), param));
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

    }
}

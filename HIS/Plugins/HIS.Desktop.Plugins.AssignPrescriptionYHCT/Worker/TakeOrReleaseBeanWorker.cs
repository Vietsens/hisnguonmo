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
using HIS.Desktop.Plugins.AssignPrescriptionYHCT.ADO;
using HIS.Desktop.Plugins.AssignPrescriptionYHCT.Resources;
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

namespace HIS.Desktop.Plugins.AssignPrescriptionYHCT
{
    class TakeOrReleaseBeanWorker
    {
        internal static bool TakeForCreateBean(long expMestId, MediMatyTypeADO medicineTypeSDO, bool isHasBean, CommonParam param)
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
                    string uriForTakeBean = (medicineTypeSDO.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC ? RequestUriStore.HIS_MEDICINE_BEAN__TAKEBEAN : RequestUriStore.HIS_MATERIAL_BEAN__TAKEBEAN);

                    TakeBeanSDO takeBeanSDO = new TakeBeanSDO();
                    if (medicineTypeSDO.AMOUNT == 0) return true;

                    takeBeanSDO.ClientSessionKey = GlobalStore.ClientSessionKey;
                    takeBeanSDO.MediStockId = medicineTypeSDO.MEDI_STOCK_ID ?? 0;
                    takeBeanSDO.TypeId = medicineTypeSDO.ID;
                    takeBeanSDO.Amount = medicineTypeSDO.AMOUNT ?? 0;
                    takeBeanSDO.BeanIds = beanIds;
                    takeBeanSDO.PatientTypeId = medicineTypeSDO.PATIENT_TYPE_ID;

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

        internal static bool TakeForUpdateBean(long expMestId, MediMatyTypeADO medicineTypeSDO, decimal amount, bool hasWarmError, CommonParam param)
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
                    string uriForTakeBean = (medicineTypeSDO.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC ? RequestUriStore.HIS_MEDICINE_BEAN__TAKEBEAN : RequestUriStore.HIS_MATERIAL_BEAN__TAKEBEAN);

                    TakeBeanSDO takeBeanSDO = new TakeBeanSDO();
                    if (amount == 0) return true;

                    takeBeanSDO.ClientSessionKey = GlobalStore.ClientSessionKey;
                    takeBeanSDO.MediStockId = medicineTypeSDO.MEDI_STOCK_ID ?? 0;
                    takeBeanSDO.TypeId = medicineTypeSDO.ID;
                    takeBeanSDO.Amount = amount;
                    takeBeanSDO.BeanIds = beanIds;
                    takeBeanSDO.PatientTypeId = medicineTypeSDO.PATIENT_TYPE_ID;

                    if (medicineTypeSDO.ExpMestDetailIds != null && medicineTypeSDO.ExpMestDetailIds.Count > 0)
                        takeBeanSDO.ExpMestDetailIds = medicineTypeSDO.ExpMestDetailIds;

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

        internal static bool ProcessTakeListMedi(List<MediMatyTypeADO> medis)
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
                    Inventec.Common.Logging.LogSystem.Debug("ProcessTakeListMedi => 1");
                    List<TakeBeanSDO> takeBeanSDOs = new List<TakeBeanSDO>();
                    foreach (var item in medis)
                    {
                        if ((item.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC
                            || item.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.VATTU)
                            && (!GlobalStore.IsTreatmentIn || GlobalStore.IsCabinet)
                            )
                        {
                            TakeBeanSDO takeBeanSDO = new TakeBeanSDO();
                            takeBeanSDO.ClientSessionKey = GlobalStore.ClientSessionKey;
                            takeBeanSDO.Amount = (item.AMOUNT ?? 0);
                            takeBeanSDO.MediStockId = (item.MEDI_STOCK_ID ?? 0);
                            takeBeanSDO.TypeId = item.ID;
                            takeBeanSDO.PatientTypeId = item.PATIENT_TYPE_ID;

                            takeBeanSDOs.Add(takeBeanSDO);
                        }
                    }

                    if (takeBeanSDOs != null && takeBeanSDOs.Count > 0)
                    {
                        CommonParam param = new CommonParam();
                        Inventec.Common.Logging.LogSystem.Debug("ProcessTakeListMedi => Goi api take list bean, uri = " + RequestUriStore.HIS_MEDICINE_BEAN__TAKEBEANLIST + ". Du lieu dau vao____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => takeBeanSDOs), takeBeanSDOs));
                        var rs = new Inventec.Common.Adapter.BackendAdapter(param).Post<List<TakeMedicineBeanListResultSDO>>(
                            RequestUriStore.HIS_MEDICINE_BEAN__TAKEBEANLIST,
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
                                var mediOne = medis.FirstOrDefault(o => o.ID == mbSDO.Request.TypeId
                                    && ((mdst != null && mdst.IS_BUSINESS != 1) || (mdst != null && mdst.IS_BUSINESS == 1 && o.PATIENT_TYPE_ID == mbSDO.Request.PatientTypeId))
                                    && o.MEDI_STOCK_ID == mbSDO.Request.MediStockId
                                    && o.AMOUNT == mbSDO.Request.Amount);

                                

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
                            //if (rs.Count < takeBeanSDOs.Count)
                            //{
                            //    Inventec.Common.Logging.LogSystem.Debug("Take medicine bean thanh cong, tra ve du lieu nhung chi tiet co 1 so dong that bai, cac dong that bai se remove khoi danh sach tra ve, rs.Count = " + rs.Count + ", takeBeanSDOs.Count = " + takeBeanSDOs.Count + ". Du lieu ket qua tra ve____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => rs), rs) + "____Du lieu dau vao____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => takeBeanSDOs), takeBeanSDOs));
                            //    StringBuilder messageWarm = new StringBuilder();
                            //    StringBuilder detailMediErrors = new StringBuilder();
                            //    var medisTakeBeanFails = medis.Where(o => o.MedicineBean1Result == null || o.MedicineBean1Result.Count == 0).ToList();
                            //    foreach (var mdf in medisTakeBeanFails)
                            //    {
                            //        detailMediErrors.Append(String.Format(ResourceMessage.ThuocVatTu__SoLuong__Kho, Inventec.Desktop.Common.HtmlString.ProcessorString.InsertFontStyle(mdf.MEDICINE_TYPE_NAME, System.Drawing.FontStyle.Bold), Inventec.Desktop.Common.HtmlString.ProcessorString.InsertColor(Inventec.Common.Number.Convert.NumberToString(mdf.AMOUNT ?? 0, 0), System.Drawing.Color.Maroon), mdf.MEDI_STOCK_NAME) + "\r\n");
                            //    }
                            //    messageWarm.Append(String.Format(ResourceMessage.ThuocVatTuTachBeanThatBai, "\r\n" + detailMediErrors.ToString()));
                            //    MessageManager.Show(messageWarm.ToString());
                            //}
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

        internal static bool ProcessTakeListMaty(List<MediMatyTypeADO> matys)
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
                    Inventec.Common.Logging.LogSystem.Debug("ProcessTakeListMaty => 1");
                    List<TakeBeanSDO> takeBeanSDOs = new List<TakeBeanSDO>();
                    foreach (var item in matys)
                    {
                        if ((item.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC
                            || item.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.VATTU)
                            && (!GlobalStore.IsTreatmentIn || GlobalStore.IsCabinet)
                            )
                        {
                            TakeBeanSDO takeBeanSDO = new TakeBeanSDO();
                            takeBeanSDO.ClientSessionKey = GlobalStore.ClientSessionKey;
                            takeBeanSDO.Amount = (item.AMOUNT ?? 0);
                            takeBeanSDO.MediStockId = (item.MEDI_STOCK_ID ?? 0);
                            takeBeanSDO.TypeId = item.ID;
                            takeBeanSDO.PatientTypeId = item.PATIENT_TYPE_ID;

                            takeBeanSDOs.Add(takeBeanSDO);
                        }
                    }

                    if (takeBeanSDOs != null && takeBeanSDOs.Count > 0)
                    {
                        CommonParam param = new CommonParam();
                        Inventec.Common.Logging.LogSystem.Debug("ProcessTakeListMaty => Goi api take list bean, uri = " + RequestUriStore.HIS_MATERIAL_BEAN__TAKEBEANLIST + ". Du lieu dau vao____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => takeBeanSDOs), takeBeanSDOs));
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
                                    && o.AMOUNT == mbSDO.Request.Amount);

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
                            //if (rs.Count < takeBeanSDOs.Count)
                            //{
                            //    Inventec.Common.Logging.LogSystem.Debug("Take material bean thanh cong, tra ve du lieu nhung chi tiet co 1 so dong that bai, cac dong that bai se remove khoi danh sach tra ve, rs.Count = " + rs.Count + ", takeBeanSDOs.Count = " + takeBeanSDOs.Count + ". Du lieu ket qua tra ve____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => rs), rs) + "____Du lieu dau vao____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => takeBeanSDOs), takeBeanSDOs));
                            //    StringBuilder messageWarm = new StringBuilder();
                            //    StringBuilder detailMediErrors = new StringBuilder();
                            //    var madisTakeBeanFails = matys.Where(o => o.MaterialBean1Result == null || o.MaterialBean1Result.Count == 0).ToList();
                            //    foreach (var mdf in madisTakeBeanFails)
                            //    {
                            //        detailMediErrors.Append(String.Format(ResourceMessage.ThuocVatTu__SoLuong__Kho, Inventec.Desktop.Common.HtmlString.ProcessorString.InsertFontStyle(mdf.MEDICINE_TYPE_NAME, System.Drawing.FontStyle.Bold), Inventec.Desktop.Common.HtmlString.ProcessorString.InsertColor(Inventec.Common.Number.Convert.NumberToString(mdf.AMOUNT ?? 0, 0), System.Drawing.Color.Maroon), mdf.MEDI_STOCK_NAME) + "\r\n");
                            //    }
                            //    messageWarm.Append(String.Format(ResourceMessage.ThuocVatTuTachBeanThatBai, "\r\n" + detailMediErrors.ToString()));
                            //    MessageManager.Show(messageWarm.ToString());
                            //}
                        }
                        else
                        {
                            MessageManager.Show(param, result);
                            Inventec.Common.Logging.LogSystem.Debug("ProcessTakeListMaty => Goi api take list material bean that bai, url = " + RequestUriStore.HIS_MATERIAL_BEAN__TAKEBEANLIST + ". Du lieu dau vao____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => takeBeanSDOs), takeBeanSDOs) + ". Du lieu dau ra____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => result), result) + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => param), param));
                        }

                        Inventec.Common.Logging.LogSystem.Debug("ProcessTakeListMaty => 2");
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
        internal static bool ProcessDeleteRowMediMaty(MediMatyTypeADO data)
        {
            bool result = true;
            try
            {
                if ((data.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC
                    || data.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.VATTU)
                    && (!GlobalStore.IsTreatmentIn || GlobalStore.IsCabinet)
                    )
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

    }
}

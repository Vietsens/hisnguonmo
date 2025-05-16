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
using DevExpress.XtraEditors.DXErrorProvider;
using HIS.Desktop.ADO;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.HisConfig;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.AssignPrescriptionCLS.ADO;
using HIS.Desktop.Plugins.AssignPrescriptionCLS.Config;
using HIS.Desktop.Plugins.AssignPrescriptionCLS.Resources;
using HIS.Desktop.Plugins.AssignPrescriptionCLS.Worker;
using HIS.Desktop.Utilities.Extensions;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.AssignPrescriptionCLS.AssignPrescription
{
    public partial class frmAssignPrescription : HIS.Desktop.Utility.FormBase
    {

        /// <summary>
        /// - Khi mở form, nếu có thông tin kho (cấu hình tài khoản chọn mặc định kho), thì:
        ///+ Tự động focus vào ô chọn thuốc/vật tư
        ///+ Căn cứ vào cấu hình "Dịch vụ - thuốc" (his_service_mety) và "Dịch vụ - vật tư" (his_service_maty), để tự động chọn các thuốc/vật tư (bổ sung vào grid bên dưới), và cho phép người dùng thêm/bớt.
        ///+ Nếu số lượng thuốc/vật tư được cấu hình vượt quá số lượng khả dụng có trong kho, thì hiển thị cảnh báo "Thuốc/vật tư xxxx vượt quá số lượng khả dụng trong kho", và trên grid chọn thuốc, tự động bôi đỏ các dòng này để cho phép người dùng sửa (tham khảo chức năng "kê đơn")
        /// </summary>
        private void InitMediMatyWithHasMediStockDefault()
        {
            List<long> serviceIds = new List<long>();
            try
            {
                if (this.currentSereServ != null && this.currentSereServ.ID > 0 && this.currentSereServ.SERVICE_ID > 0)
                {
                    var serviceMetys = BackendDataWorker.Get<HIS_SERVICE_METY>();
                    var serviceMatys = BackendDataWorker.Get<HIS_SERVICE_MATY>();
                    var serviceMetyFilter = serviceMetys != null ? serviceMetys.Where(o => o.SERVICE_ID == this.currentSereServ.SERVICE_ID).ToList() : null;
                    if (serviceMetyFilter != null && serviceMetyFilter.Count > 0)
                    {
                        var metyIds = serviceMetyFilter.Select(o => o.MEDICINE_TYPE_ID).ToArray();
                        var serviceIds1 = this.currentMedicineTypes.Where(o => metyIds.Contains(o.ID)).Select(o => o.SERVICE_ID).ToList();
                        if (serviceIds1 != null)
                        {
                            serviceIds.AddRange(serviceIds1);
                        }
                    }

                    var serviceMatyFilter = serviceMatys != null ? serviceMatys.Where(o => o.SERVICE_ID == this.currentSereServ.SERVICE_ID).ToList() : null;
                    if (serviceMatyFilter != null && serviceMatyFilter.Count > 0)
                    {
                        var matyIds = serviceMatyFilter.Select(o => o.MATERIAL_TYPE_ID).ToArray();
                        var serviceIds2 = this.currentMaterialTypes.Where(o => matyIds.Contains(o.ID)).Select(o => o.SERVICE_ID).ToList();
                        if (serviceIds2 != null)
                        {
                            serviceIds.AddRange(serviceIds2);
                        }
                    }

                    serviceInMaMeConfigs = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_SERVICE>().Where(o => serviceIds.Contains(o.ID)).ToList();
                    if (serviceInMaMeConfigs == null || serviceInMaMeConfigs.Count == 0)
                        throw new ArgumentNullException("serviceInMaMeConfigs is null");

                    if (this.mediMatyTypeAvailables == null || this.mediMatyTypeAvailables.Count == 0)
                        this.InitDataMetyMatyTypeInStockD(this.currentMediStock);

                    //Release tat ca cac thuoc/ vat tu da duoc take bean truoc do nhung chua duoc luu
                    //this.ReleaseAllMediByUser();

                    //Load data for material in package
                    this.LoadPageMateMetyByListServiceConfig(serviceInMaMeConfigs, serviceMetyFilter, serviceMatyFilter);
                    this.ProcessInstructionTimeMediForEdit();
                    this.ProcessMergeDuplicateRowForListProcessing();
                    this.ProcessAddListRowDataIntoGridWithTakeBean();
                    this.ReloadDataAvaiableMediBeanInCombo();

                    //Kiểm tra trần hao phí của dịch vụ chính & các thuốc/vật tư đã kê, đưa ra cảnh báo nếu vượt trần
                    this.AlertOutofMaxExpend();
                }
                else
                {
                    MessageManager.Show(LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.ThieuTruongDuLieuBatBuoc));
                    Inventec.Common.Logging.LogSystem.Debug("Du lieu SereServ truyen vao khong hop le____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => currentSereServ), currentSereServ));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => currentSereServ), currentSereServ) + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => currentMediStock), currentMediStock) + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => serviceIds), serviceIds));
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        internal decimal CalculatePrice(MediMatyTypeADO item)
        {
            decimal result = 0;
            try
            {
                if (item.LAST_EXP_PRICE.HasValue || item.LAST_EXP_VAT_RATIO.HasValue)
                {
                    decimal? priceRaw = (item.LAST_EXP_PRICE ?? 0) * (1 + (item.LAST_EXP_VAT_RATIO ?? 0));
                    //decimal? price = (item.CONVERT_RATIO.HasValue && item.CONVERT_RATIO > 0) ? priceRaw / item.CONVERT_RATIO.Value : priceRaw;
                    result = (priceRaw * item.AMOUNT) ?? 0;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return result;
        }

        private List<long> GetInstructionTimeMedi()
        {
            List<long> intructionTimeTemps = new List<long>();
            try
            {
                if (HisConfigCFG.ManyDayPrescriptionOption == 2 && (GlobalStore.IsTreatmentIn && !GlobalStore.IsCabinet))
                {
                    if (this.mediMatyTypeADOs != null && this.mediMatyTypeADOs.Count > 0)
                    {
                        foreach (var item in this.mediMatyTypeADOs)
                        {
                            if (item.IntructionTimeSelecteds != null && item.IntructionTimeSelecteds.Count > 0)
                            {
                                intructionTimeTemps.AddRange(item.IntructionTimeSelecteds);
                            }
                        }
                        intructionTimeTemps = intructionTimeTemps.Distinct().ToList();
                    }
                }
                else
                {
                    intructionTimeTemps.AddRange(this.intructionTimeSelecteds);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return intructionTimeTemps;
        }

        private bool ProcessInstructionTimeMediForEdit()
        {
            bool result = false;
            try
            {
                if (HisConfigCFG.ManyDayPrescriptionOption == 2 && (GlobalStore.IsTreatmentIn && !GlobalStore.IsCabinet))
                {
                    if (this.mediMatyTypeADOs != null && this.mediMatyTypeADOs.Count > 0)
                    {
                        this.mediMatyTypeADOs.ForEach(o => o.IntructionTimeSelecteds = this.intructionTimeSelecteds);
                    }
                }
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private string GetDateFromManyDayPres(List<long> intructiontimes)
        {
            string strTimeDisplay = "";
            try
            {
                int num = 0;
                intructiontimes = intructiontimes.OrderBy(o => o).ToList();
                foreach (var item in intructiontimes)
                {
                    DateTime? itemDate = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(item);
                    if (itemDate != null && itemDate.Value != DateTime.MinValue)
                    {
                        strTimeDisplay += (num == 0 ? "" : "; ") + itemDate.Value.ToString("dd/MM");
                        num++;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

            return strTimeDisplay;
        }

        /// <summary>
        /// Mong muốn có cấu hình cài đặt mức trần BHYT cho 3 loại đối tượng BN này (Khám bệnh, Ngoại trú, Nội trú), mức trần này là tổng chi phí BHYT của bệnh nhân.
        ///- Với BN khám bệnh sẽ áp dụng với mức trần Khám bệnh (các BN được chỉ định trực tiếp tại tiếp đón cũng được áp dụng trong trường hợp này)
        ///- Với BN điều trị ngoại trú sẽ áp dụng mức trần Ngoại trú (tính cả chi phí BHYT từ PK)
        ///- Với BN điều trị nội trú sẽ áp dụng mức trần Nội trú (tính cả chi phí BHYT từ PK và cả điều trị ngoại trú trong trường hợp điều trị kết hợp). 
        ///Chú ý: 
        ///+ Với BN đang điều trị kết hợp giữa nội trú và ngoại trú, khi được chỉ định tại khoa điều trị Ngoại trú thì cũng phải áp dụng mức trần đã cài đặt đối với Nội trú.
        ///+ Với Bn được chỉ định tại các bộ phận CLS thì áp dụng với loại bệnh án tương ứng (với trường hợp BN đang điều trị kết hợp Nội trú và ngoại trú --> khoa ngoại trú chỉ định CLS --> phòng CLS chỉ định --> áp dụng mức trần của Nội trú).
        ///+ Chức năng cảnh báo áp dụng cả với lúc chuyển đối tượng dịch vụ và chuyển đối tượng BN từ đối tượng thu phí sang BHYT.
        /// </summary>
        internal void VerifyWarningOverCeiling()
        {
            try
            {
                decimal totalPriceSum = totalHeinByTreatment + GetTotalPriceServiceSelected();

                decimal warningOverCeiling = (this.currentHisPatientTypeAlter.TREATMENT_TYPE_CODE == HisConfigCFG.TreatmentTypeCode__Exam ? HisConfigCFG.WarningOverCeiling__Exam : (this.currentHisPatientTypeAlter.TREATMENT_TYPE_CODE == HisConfigCFG.TreatmentTypeCode__TreatOut ? HisConfigCFG.WarningOverCeiling__Out : HisConfigCFG.WarningOverCeiling__In));

                bool inValid = (warningOverCeiling > 0 && (totalPriceSum > warningOverCeiling));
                if (inValid)
                {
                    MessageManager.Show(String.Format(ResourceMessage.TongTienTheoDoiTuongDieuTriChoBHYTDaVuotquaMucGioiHan, GetTreatmentTypeNameByCode(this.currentHisPatientTypeAlter.TREATMENT_TYPE_CODE), Inventec.Common.Number.Convert.NumberToString(totalPriceSum, 0), Inventec.Common.Number.Convert.NumberToString(warningOverCeiling, 0)));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private decimal GetTotalPriceServiceSelected()
        {
            decimal totalPrice = 0;
            try
            {
                totalPrice = mediMatyTypeADOs.Where(item => item.PATIENT_TYPE_ID == HisConfigCFG.PatientTypeId__BHYT && (item.IsExpend) == false).Sum(o => o.TotalPrice);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return totalPrice;
        }

        string GetTreatmentTypeNameByCode(string code)
        {
            string name = "";
            try
            {
                name = BackendDataWorker.Get<HIS_TREATMENT_TYPE>().FirstOrDefault(o => o.TREATMENT_TYPE_CODE == code).TREATMENT_TYPE_NAME;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return name;
        }

        //HIS.UC.TreatmentFinish.ADO.DataInputADO GetDateADO()
        //{
        //    HIS.UC.TreatmentFinish.ADO.DataInputADO result = new UC.TreatmentFinish.ADO.DataInputADO();
        //    try
        //    {
        //        if (spinSoNgay.EditValue != null)
        //            result.SoNgay = (long)spinSoNgay.Value;
        //        long useTimeMax = 0;
        //        if (this.mediMatyTypeADOs != null && this.mediMatyTypeADOs.Count > 0)
        //        {
        //            var useTimeFind = this.mediMatyTypeADOs.Where(o => o.UseTimeTo > 0).ToList();
        //            useTimeMax = ((useTimeFind != null && useTimeFind.Count > 0) ? useTimeFind.Max(o => o.UseTimeTo ?? 0) : 0);
        //            result.UseTimeTo = useTimeMax;
        //        }
        //        if (this.intructionTimeSelecteds != null && this.intructionTimeSelecteds.Count > 0)
        //        {
        //            long useTime = this.intructionTimeSelecteds.OrderBy(o => o).First();
        //            result.UseTime = useTime;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Warn(ex);
        //    }
        //    return result;
        //}

        private void ProcessMergeDuplicateRowForListProcessingForCheckChangePackage(bool isShowLo)
        {
            try
            {
                if (HisConfigCFG.IsAllowAssignPresByPackage && this.mediMatyTypeADOBKs != null && this.mediMatyTypeADOBKs.Count > 0)
                {
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => this.mediMatyTypeADOs.Count), this.mediMatyTypeADOs.Count) + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => this.mediMatyTypeADOBKs.Count), this.mediMatyTypeADOBKs.Count));
                    if (isShowLo)
                    {
                        List<MediMatyTypeADO> mediMatyTypeADOForBackups = new List<MediMatyTypeADO>();
                        foreach (var item in this.mediMatyTypeADOs)
                        {
                            var raws = this.mediMatyTypeADOBKs.Where(o => o.ID == item.ID && o.MEDI_STOCK_ID == item.MEDI_STOCK_ID && (o.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC || o.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__VT)).ToList();
                            int countRaw = raws.Count;
                            decimal? amount = raws.Sum(o => o.AMOUNT);
                            if (countRaw > 1 && amount == item.AMOUNT)
                            {
                                Inventec.Common.Logging.LogSystem.Debug("Tim thay thuoc/vat tu cua cung loai co lo khac nhau____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => countRaw), countRaw) + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => amount), amount) + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => item.AMOUNT), item.AMOUNT));

                                var mediMatyTypeADOTemps = new List<MediMatyTypeADO>();
                                AutoMapper.Mapper.CreateMap<MediMatyTypeADO, MediMatyTypeADO>();
                                foreach (var r1 in raws)
                                {
                                    MediMatyTypeADO mediMatyTypeTmp = new MediMatyTypeADO();
                                    Inventec.Common.Mapper.DataObjectMapper.Map<MediMatyTypeADO>(mediMatyTypeTmp, r1);
                                    mediMatyTypeTmp.IsAssignPackage = item.IsAssignPackage;
                                    mediMatyTypeADOTemps.Add(mediMatyTypeTmp);
                                }

                                mediMatyTypeADOForBackups.AddRange(mediMatyTypeADOTemps);
                            }
                            else
                            {
                                mediMatyTypeADOForBackups.Add(item);
                                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => countRaw), countRaw) + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => amount), amount) + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => item.AMOUNT), item.AMOUNT));
                            }
                        }

                        this.mediMatyTypeADOs.Clear();
                        this.mediMatyTypeADOs.AddRange(mediMatyTypeADOForBackups);
                        this.mediMatyTypeADOs = this.mediMatyTypeADOs.OrderBy(o => o.NUM_ORDER).ToList();
                    }

                    this.ProcessMergeDuplicateRowForListProcessingOnly();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ProcessMergeDuplicateRowForListProcessing()
        {
            try
            {
                if (HisConfigCFG.IsAllowAssignPresByPackage)
                {
                    try
                    {
                        this.mediMatyTypeADOBKs = new List<MediMatyTypeADO>();
                        AutoMapper.Mapper.CreateMap<MediMatyTypeADO, MediMatyTypeADO>();
                        foreach (var item in this.mediMatyTypeADOs)
                        {
                            MediMatyTypeADO mediMatyTypeTmp = new MediMatyTypeADO();
                            Inventec.Common.Mapper.DataObjectMapper.Map<MediMatyTypeADO>(mediMatyTypeTmp, item);
                            this.mediMatyTypeADOBKs.Add(mediMatyTypeTmp);
                        }
                    }
                    catch (Exception exx)
                    {
                        Inventec.Common.Logging.LogSystem.Warn(exx);
                    }
                }
                ProcessMergeDuplicateRowForListProcessingOnly();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ProcessMergeDuplicateRowForListProcessingOnly()
        {
            try
            {

                List<MediMatyTypeADO> mediMatyTypeADOsTemp = new List<MediMatyTypeADO>();
                foreach (var item in this.mediMatyTypeADOs)
                {
                    if (item.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC)
                    {
                        var checkPresExists = mediMatyTypeADOsTemp
                            .FirstOrDefault(o => o.ID == item.ID
                                            && ((!HisConfigCFG.IsAllowAssignPresByPackage || (o.IsAssignPackage ?? false) == false || String.IsNullOrEmpty(o.TDL_PACKAGE_NUMBER)) || (!String.IsNullOrEmpty(o.TDL_PACKAGE_NUMBER) && o.TDL_PACKAGE_NUMBER == item.TDL_PACKAGE_NUMBER))
                                            && (!HisConfigCFG.IsAllowAssignPresByPackage || (o.IsAssignPackage ?? false) == false || (o.IsAssignPackage.Value && o.MAME_ID == item.MAME_ID))
                                            && o.MEDI_STOCK_ID == item.MEDI_STOCK_ID
                                            && o.PATIENT_TYPE_ID == item.PATIENT_TYPE_ID
                                            && o.IsExpend == item.IsExpend
                                            && o.IsExpendType == item.IsExpendType
                                            && o.IsOutParentFee == item.IsOutParentFee
                                            && o.MEDICINE_USE_FORM_ID == item.MEDICINE_USE_FORM_ID
                                            && o.DataType == item.DataType
                            //&& o.UseTimeTo == item.UseTimeTo
                            //&& o.PRICE == item.PRICE
                                            );

                        if (checkPresExists != null && checkPresExists.ID > 0)
                        {
                            checkPresExists.AMOUNT = (checkPresExists.AMOUNT ?? 0) + (item.AMOUNT ?? 0);
                            checkPresExists.PRE_AMOUNT = ((checkPresExists.PRE_AMOUNT ?? 0) + (item.AMOUNT ?? 0));
                            checkPresExists.TotalPrice = (checkPresExists.TotalPrice + item.TotalPrice);

                            if ((checkPresExists.BeanIds != null && checkPresExists.BeanIds.Count > 0)
                                && (item.BeanIds != null && item.BeanIds.Count > 0))
                                checkPresExists.BeanIds.AddRange(item.BeanIds);

                            if ((checkPresExists.MedicineBean1Result != null && checkPresExists.MedicineBean1Result.Count > 0)
                                && (item.MedicineBean1Result != null && item.MedicineBean1Result.Count > 0))
                                checkPresExists.MedicineBean1Result.AddRange(item.MedicineBean1Result);

                            if ((checkPresExists.ExpMestDetailIds != null && checkPresExists.ExpMestDetailIds.Count > 0)
                                && (item.ExpMestDetailIds != null && item.ExpMestDetailIds.Count > 0))
                                checkPresExists.ExpMestDetailIds.AddRange(item.ExpMestDetailIds);
                        }
                        else
                        {
                            mediMatyTypeADOsTemp.Add(item);
                        }
                    }
                    else if (item.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.VATTU)
                    {
                        var checkPresExists = mediMatyTypeADOsTemp
                         .FirstOrDefault(o => o.ID == item.ID
                                            && o.MEDI_STOCK_ID == item.MEDI_STOCK_ID
                                            && o.DataType == item.DataType
                                            && o.PATIENT_TYPE_ID == item.PATIENT_TYPE_ID
                                            && o.IsExpend == item.IsExpend
                                            && o.PRICE == item.PRICE
                                            );

                        if (checkPresExists != null && checkPresExists.ID > 0 && item.IsStent == false)
                        {
                            checkPresExists.AMOUNT = (checkPresExists.AMOUNT ?? 0) + (item.AMOUNT ?? 0);
                            checkPresExists.PRE_AMOUNT = ((checkPresExists.PRE_AMOUNT ?? 0) + (item.AMOUNT ?? 0));
                            checkPresExists.TotalPrice = (checkPresExists.TotalPrice + item.TotalPrice);

                            if ((checkPresExists.BeanIds != null && checkPresExists.BeanIds.Count > 0)
                                && (item.BeanIds != null && item.BeanIds.Count > 0))
                                checkPresExists.BeanIds.AddRange(item.BeanIds);

                            if ((checkPresExists.MaterialBean1Result != null && checkPresExists.MaterialBean1Result.Count > 0)
                                && (item.MaterialBean1Result != null && item.MaterialBean1Result.Count > 0))
                                checkPresExists.MaterialBean1Result.AddRange(item.MaterialBean1Result);

                            if ((checkPresExists.ExpMestDetailIds != null && checkPresExists.ExpMestDetailIds.Count > 0)
                                && (item.ExpMestDetailIds != null && item.ExpMestDetailIds.Count > 0))
                                checkPresExists.ExpMestDetailIds.AddRange(item.ExpMestDetailIds);
                        }
                        else
                        {
                            mediMatyTypeADOsTemp.Add(item);
                        }
                    }
                    else if (item.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.VATTU_TSD)
                    {
                        mediMatyTypeADOsTemp.Add(item);
                    }
                    else if (item.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC_DM)
                    {
                        var checkPresExists = mediMatyTypeADOsTemp
                        .FirstOrDefault(o => o.ID == item.ID
                                        && o.PATIENT_TYPE_ID == item.PATIENT_TYPE_ID
                                        && o.IsExpend == item.IsExpend
                                        && o.DataType == item.DataType
                                        && o.MEDICINE_USE_FORM_ID == item.MEDICINE_USE_FORM_ID
                                        && o.UseTimeTo == item.UseTimeTo
                                        );

                        if (checkPresExists != null && checkPresExists.ID > 0)
                        {
                            checkPresExists.AMOUNT = (checkPresExists.AMOUNT ?? 0) + (item.AMOUNT ?? 0);
                            checkPresExists.PRE_AMOUNT = ((checkPresExists.PRE_AMOUNT ?? 0) + (item.AMOUNT ?? 0));
                        }
                        else
                        {
                            mediMatyTypeADOsTemp.Add(item);
                        }
                    }
                    else if (item.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC_TUTUC)
                    {
                        var checkPresExists = mediMatyTypeADOsTemp
                        .FirstOrDefault(o => o.MEDICINE_TYPE_NAME == item.MEDICINE_TYPE_NAME
                                        && o.IsExpend == item.IsExpend
                                        && o.DataType == item.DataType
                                        && o.TUTORIAL == item.TUTORIAL
                                        && o.HTU_TEXT == item.HTU_TEXT
                                        && o.SERVICE_UNIT_NAME == item.SERVICE_UNIT_NAME
                                        && o.UseTimeTo == item.UseTimeTo
                                        );

                        if (checkPresExists != null && checkPresExists.ID > 0)
                        {
                            checkPresExists.AMOUNT = (checkPresExists.AMOUNT ?? 0) + (item.AMOUNT ?? 0);
                            checkPresExists.PRE_AMOUNT = ((checkPresExists.PRE_AMOUNT ?? 0) + (item.AMOUNT ?? 0));
                        }
                        else
                        {
                            mediMatyTypeADOsTemp.Add(item);
                        }
                    }
                    else if (item.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.VATTU_DM)
                    {
                        var checkPresExists = mediMatyTypeADOsTemp
                           .FirstOrDefault(o => o.ID == item.ID
                                            && o.DataType == item.DataType
                                            && o.IsExpend == item.IsExpend);

                        if (checkPresExists != null && checkPresExists.ID > 0)
                        {
                            checkPresExists.AMOUNT = (checkPresExists.AMOUNT ?? 0) + (item.AMOUNT ?? 0);
                            checkPresExists.PRE_AMOUNT = ((checkPresExists.PRE_AMOUNT ?? 0) + (item.AMOUNT ?? 0));
                        }
                        else
                        {
                            mediMatyTypeADOsTemp.Add(item);
                        }
                    }
                }
                this.mediMatyTypeADOs.Clear();
                this.mediMatyTypeADOs.AddRange(mediMatyTypeADOsTemp);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void RefeshResourceGridMedicine()
        {
            try
            {
                this.gridControlServiceProcess.DataSource = null;
                this.gridControlServiceProcess.DataSource = this.mediMatyTypeADOs.OrderBy(o => o.NUM_ORDER).ToList();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private List<MediMatyTypeADO> ProcessMergeDuplicateRowForListProcessingForShow()
        {
            try
            {
                List<MediMatyTypeADO> mediMatyTypeADOsTemp = new List<MediMatyTypeADO>();

                foreach (var item in this.mediMatyTypeADOs)
                {
                    if (item.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC)
                    {
                        var checkPresExists = mediMatyTypeADOsTemp
                            .FirstOrDefault(o => o.ID == item.ID
                                            && o.MEDI_STOCK_ID == item.MEDI_STOCK_ID
                                            && o.PATIENT_TYPE_ID == item.PATIENT_TYPE_ID
                                            && (((o.IsAssignPackage ?? false) == false || String.IsNullOrEmpty(o.TDL_PACKAGE_NUMBER)) || (!String.IsNullOrEmpty(o.TDL_PACKAGE_NUMBER) && o.TDL_PACKAGE_NUMBER == item.TDL_PACKAGE_NUMBER))
                                            && ((o.IsAssignPackage ?? false) == false || (o.IsAssignPackage.Value && o.MAME_ID == item.MAME_ID))
                                            && o.IsExpend == item.IsExpend
                                            && o.IsOutParentFee == item.IsOutParentFee
                                            && o.MEDICINE_USE_FORM_ID == item.MEDICINE_USE_FORM_ID
                                            && o.DataType == item.DataType
                                            && o.UseTimeTo == item.UseTimeTo
                                            && o.PRICE == item.PRICE
                            //&& !GlobalStore.IsTreatmentIn ? o.PRICE == item.PRICE : true
                                            );

                        if (checkPresExists != null && checkPresExists.ID > 0)
                        {
                            checkPresExists.AMOUNT = (checkPresExists.AMOUNT ?? 0) + (item.AMOUNT ?? 0);
                            checkPresExists.PRE_AMOUNT = ((checkPresExists.PRE_AMOUNT ?? 0) + (item.AMOUNT ?? 0));
                            checkPresExists.TotalPrice = (checkPresExists.TotalPrice + item.TotalPrice);

                            if ((checkPresExists.BeanIds != null && checkPresExists.BeanIds.Count > 0)
                                && (item.BeanIds != null && item.BeanIds.Count > 0))
                                checkPresExists.BeanIds.AddRange(item.BeanIds);

                            if ((checkPresExists.ExpMestDetailIds != null && checkPresExists.ExpMestDetailIds.Count > 0)
                                && (item.ExpMestDetailIds != null && item.ExpMestDetailIds.Count > 0))
                                checkPresExists.ExpMestDetailIds.AddRange(item.ExpMestDetailIds);
                            //checkPresExists.PRICE = (((checkPresExists.PRICE ?? 0) + (item.PRICE ?? 0)) / 2);
                        }
                        else
                        {
                            mediMatyTypeADOsTemp.Add(item);
                        }
                    }
                    else if (item.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.VATTU)
                    {
                        var checkPresExists = mediMatyTypeADOsTemp
                         .FirstOrDefault(o => o.ID == item.ID
                                            && o.MEDI_STOCK_ID == item.MEDI_STOCK_ID
                                            && o.DataType == item.DataType
                                            && o.PATIENT_TYPE_ID == item.PATIENT_TYPE_ID
                                            && o.IsExpend == item.IsExpend
                                            && o.PRICE == item.PRICE
                                            //&& !GlobalStore.IsTreatmentIn ? o.PRICE == item.PRICE : true
                                            );

                        if (checkPresExists != null && checkPresExists.ID > 0 && item.IsStent == false)
                        {
                            checkPresExists.AMOUNT = (checkPresExists.AMOUNT ?? 0) + (item.AMOUNT ?? 0);
                            checkPresExists.PRE_AMOUNT = ((checkPresExists.PRE_AMOUNT ?? 0) + (item.AMOUNT ?? 0));
                            checkPresExists.TotalPrice = (checkPresExists.TotalPrice + item.TotalPrice);

                            if ((checkPresExists.BeanIds != null && checkPresExists.BeanIds.Count > 0)
                                && (item.BeanIds != null && item.BeanIds.Count > 0))
                                checkPresExists.BeanIds.AddRange(item.BeanIds);

                            if ((checkPresExists.ExpMestDetailIds != null && checkPresExists.ExpMestDetailIds.Count > 0)
                                && (item.ExpMestDetailIds != null && item.ExpMestDetailIds.Count > 0))
                                checkPresExists.ExpMestDetailIds.AddRange(item.ExpMestDetailIds);
                        }
                        else
                        {
                            mediMatyTypeADOsTemp.Add(item);
                        }
                    }
                    else if (item.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.VATTU_TSD)
                    {
                        mediMatyTypeADOsTemp.Add(item);
                    }
                    else if (item.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC_DM)
                    {
                        var checkPresExists = mediMatyTypeADOsTemp
                        .FirstOrDefault(o => o.ID == item.ID
                                        && o.PATIENT_TYPE_ID == item.PATIENT_TYPE_ID
                                        && o.IsExpend == item.IsExpend
                                        && o.DataType == item.DataType
                                        && o.MEDICINE_USE_FORM_ID == item.MEDICINE_USE_FORM_ID
                                        && o.UseTimeTo == item.UseTimeTo
                                        );

                        if (checkPresExists != null && checkPresExists.ID > 0)
                        {
                            checkPresExists.AMOUNT = (checkPresExists.AMOUNT ?? 0) + (item.AMOUNT ?? 0);
                            checkPresExists.PRE_AMOUNT = ((checkPresExists.PRE_AMOUNT ?? 0) + (item.AMOUNT ?? 0));
                        }
                        else
                        {
                            mediMatyTypeADOsTemp.Add(item);
                        }
                    }
                    else if (item.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC_TUTUC)
                    {
                        var checkPresExists = mediMatyTypeADOsTemp
                        .FirstOrDefault(o => o.MEDICINE_TYPE_NAME == item.MEDICINE_TYPE_NAME
                                        && o.IsExpend == item.IsExpend
                                        && o.DataType == item.DataType
                                        && o.TUTORIAL == item.TUTORIAL
                                        && o.HTU_TEXT == item.HTU_TEXT
                                        && o.SERVICE_UNIT_NAME == item.SERVICE_UNIT_NAME
                                        && o.UseTimeTo == item.UseTimeTo
                                        );

                        if (checkPresExists != null && checkPresExists.ID > 0)
                        {
                            checkPresExists.AMOUNT = (checkPresExists.AMOUNT ?? 0) + (item.AMOUNT ?? 0);
                            checkPresExists.PRE_AMOUNT = ((checkPresExists.PRE_AMOUNT ?? 0) + (item.AMOUNT ?? 0));
                        }
                        else
                        {
                            mediMatyTypeADOsTemp.Add(item);
                        }
                    }
                    else if (item.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.VATTU_DM)
                    {
                        var checkPresExists = mediMatyTypeADOsTemp
                           .FirstOrDefault(o => o.ID == item.ID
                                            && o.DataType == item.DataType
                                            && o.IsExpend == item.IsExpend);

                        if (checkPresExists != null && checkPresExists.ID > 0)
                        {
                            checkPresExists.AMOUNT = (checkPresExists.AMOUNT ?? 0) + (item.AMOUNT ?? 0);
                            checkPresExists.PRE_AMOUNT = ((checkPresExists.PRE_AMOUNT ?? 0) + (item.AMOUNT ?? 0));
                        }
                        else
                        {
                            mediMatyTypeADOsTemp.Add(item);
                        }
                    }
                }
                mediMatyTypeADOsTemp = mediMatyTypeADOsTemp.OrderBy(o => o.NUM_ORDER).ToList();
                return mediMatyTypeADOsTemp;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return null;
        }

        private void ProcessAddListRowDataIntoGridWithTakeBean()
        {
            try
            {
                List<MediMatyTypeADO> mediMatyTypeADOsTemp = new List<MediMatyTypeADO>();
                this.ValidDataMediMaty();
                if (this.ProcessValidMedicineTypeAge(true) && this.ProcessAmountInStockWarning())
                {
                    var mediMatyTypeADOsForTakeBeans__Other = (this.mediMatyTypeADOs.Where(o => o.DataType != HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC && o.DataType != HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.VATTU && o.DataType != HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.VATTU_TSD).ToList());
                    if (mediMatyTypeADOsForTakeBeans__Other != null && mediMatyTypeADOsForTakeBeans__Other.Count > 0)
                        mediMatyTypeADOsTemp.AddRange(mediMatyTypeADOsForTakeBeans__Other);

                    var mediMatyTypeADOsForTakeBeans__Medicine = (this.mediMatyTypeADOs.Where(o => o.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC && (o.AmountAlert ?? 0) == 0).ToList());

                    if (mediMatyTypeADOsForTakeBeans__Medicine != null && mediMatyTypeADOsForTakeBeans__Medicine.Count > 0)
                    {
                        if (!GlobalStore.IsTreatmentIn || GlobalStore.IsCabinet)
                        {
                            if (TakeOrReleaseBeanWorker.ProcessTakeListMedi(this.intructionTimeSelecteds, mediMatyTypeADOsForTakeBeans__Medicine))
                            {
                            }

                            mediMatyTypeADOsTemp.AddRange(mediMatyTypeADOsForTakeBeans__Medicine);
                        }
                        else
                        {
                            mediMatyTypeADOsForTakeBeans__Medicine.ForEach(o => SetPriceOne(o));
                            mediMatyTypeADOsTemp.AddRange(mediMatyTypeADOsForTakeBeans__Medicine);
                        }
                    }
                    var mediMatyTypeADOsForTakeBeans__Material = (this.mediMatyTypeADOs.Where(o => o.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.VATTU && (o.AmountAlert ?? 0) == 0).ToList());
                    if (mediMatyTypeADOsForTakeBeans__Material != null && mediMatyTypeADOsForTakeBeans__Material.Count > 0)
                    {
                        if (!GlobalStore.IsTreatmentIn || GlobalStore.IsCabinet)
                        {
                            if (TakeOrReleaseBeanWorker.ProcessTakeListMaty(this.intructionTimeSelecteds, mediMatyTypeADOsForTakeBeans__Material))
                            {
                            }
                            mediMatyTypeADOsTemp.AddRange(mediMatyTypeADOsForTakeBeans__Material);
                        }
                        else
                        {
                            mediMatyTypeADOsForTakeBeans__Material.ForEach(o => SetPriceOne(o));
                            mediMatyTypeADOsTemp.AddRange(mediMatyTypeADOsForTakeBeans__Material);
                        }
                    }

                    var mediMatyTypeADOsForTakeBeans__MaterialTSD = (this.mediMatyTypeADOs.Where(o => o.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.VATTU_TSD && (o.AmountAlert ?? 0) == 0).ToList());
                    if (mediMatyTypeADOsForTakeBeans__MaterialTSD != null && mediMatyTypeADOsForTakeBeans__MaterialTSD.Count > 0)
                    {
                        if (!GlobalStore.IsTreatmentIn || GlobalStore.IsCabinet)
                        {
                            if (TakeOrReleaseBeanWorker.ProcessTakeListMatyTSD(this.intructionTimeSelecteds, mediMatyTypeADOsForTakeBeans__MaterialTSD))
                            {
                            }
                            mediMatyTypeADOsTemp.AddRange(mediMatyTypeADOsForTakeBeans__MaterialTSD);
                        }
                        else
                        {
                            mediMatyTypeADOsForTakeBeans__MaterialTSD.ForEach(o => SetPriceOne(o));
                            mediMatyTypeADOsTemp.AddRange(mediMatyTypeADOsForTakeBeans__MaterialTSD);
                        }
                    }

                    var mediMatyTypeInStockWarnings = this.mediMatyTypeADOs.Where(o => (o.AmountAlert ?? 0) > 0).ToList();
                    if (mediMatyTypeInStockWarnings != null && mediMatyTypeInStockWarnings.Count > 0)
                    {
                        mediMatyTypeADOsTemp.AddRange(mediMatyTypeInStockWarnings);
                    }

                    if (!GlobalStore.IsTreatmentIn || GlobalStore.IsCabinet)
                    {
                        foreach (var mtItem in mediMatyTypeADOsTemp)
                        {
                            if ((mtItem.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC || mtItem.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.VATTU || mtItem.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.VATTU_TSD) && ((mtItem.MedicineBean1Result == null || mtItem.MedicineBean1Result.Count == 0) && (mtItem.MaterialBean1Result == null || mtItem.MaterialBean1Result.Count == 0)))
                            {
                                Inventec.Common.Logging.LogSystem.Debug("Thuoc/vat tu " + mtItem.MEDICINE_TYPE_NAME + " - " + mtItem.ID + " - DataType = " + mtItem.DataType + " take bean that bai");
                                mtItem.AmountAlert = mtItem.AMOUNT;
                            }
                        }
                    }
                }
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => mediMatyTypeADOs), mediMatyTypeADOs));
                this.mediMatyTypeADOs = mediMatyTypeADOsTemp.OrderBy(o => o.NUM_ORDER).ToList();
                this.RefeshResourceGridMedicine();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetPriceOne(MediMatyTypeADO item)
        {
            try
            {
                item.TotalPrice = CalculatePrice(item);
                //if (this.servicePatyAllows != null && this.servicePatyAllows.ContainsKey(item.SERVICE_ID))
                //{
                //    var data_ServicePrice = this.servicePatyAllows[item.SERVICE_ID].Where(o => o.PATIENT_TYPE_ID == item.PATIENT_TYPE_ID).OrderByDescending(m => m.MODIFY_TIME).ToList();
                //    if (data_ServicePrice != null && data_ServicePrice.Count > 0)
                //    {
                //        item.TotalPrice = (data_ServicePrice[0].PRICE * item.AMOUNT) ?? 0;
                //    }
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        bool IsWarnAtcByCode(MediMatyTypeADO mdADO, List<MediMatyTypeADO> mdADOs, ref Dictionary<string, List<MediMatyTypeADO>> dicAtcWarn)
        {
            bool valid = false;
            if (!String.IsNullOrEmpty(mdADO.ATC_CODES))
            {
                var atcCodeArr = mdADO.ATC_CODES.Split(new string[] { ";", "," }, StringSplitOptions.RemoveEmptyEntries);
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => atcCodeArr), atcCodeArr));
                if (atcCodeArr != null && atcCodeArr.Length > 0)
                {
                    foreach (var item in atcCodeArr)
                    {
                        var mdADOsWarnDuplicates = (mdADOs != null && mdADOs.Exists(t => mdADO.SERVICE_ID != t.SERVICE_ID && t.ATC_CODES != null && t.ATC_CODES.Split(new string[] { ";", "," }, StringSplitOptions.RemoveEmptyEntries).Contains(item))) ? mdADOs.Where(t => mdADO.SERVICE_ID != t.SERVICE_ID && t.ATC_CODES.Split(new string[] { ";", "," }, StringSplitOptions.RemoveEmptyEntries).Contains(item)).ToList() : null;
                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => item), item)
                        + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => mdADOsWarnDuplicates), mdADOsWarnDuplicates));
                        if (mdADOsWarnDuplicates != null && mdADOsWarnDuplicates.Count > 0)
                        {
                            valid = true;
                            if (dicAtcWarn == null) dicAtcWarn = new Dictionary<string, List<MediMatyTypeADO>>();
                            dicAtcWarn.Add(item, mdADOsWarnDuplicates);
                        }
                    }
                }
            }
            return valid;
        }
        bool IsWarnAtcGroupByCode(MediMatyTypeADO mdADO, List<MediMatyTypeADO> mdADOs, ref Dictionary<string, List<MediMatyTypeADO>> dicAtcWarn)
        {
            bool valid = false;
            if (!String.IsNullOrEmpty(mdADO.ATC_GROUP_CODES))
            {
                var atcCodeArr = mdADO.ATC_GROUP_CODES.Split(new string[] { ";", "," }, StringSplitOptions.RemoveEmptyEntries);
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => atcCodeArr), atcCodeArr));
                if (atcCodeArr != null && atcCodeArr.Length > 0)
                {
                    foreach (var item in atcCodeArr)
                    {
                        var mdADOsWarnDuplicates = (mdADOs != null && mdADOs.Exists(t => mdADO.SERVICE_ID != t.SERVICE_ID && t.ATC_GROUP_CODES != null && t.ATC_GROUP_CODES.Split(new string[] { ";", "," }, StringSplitOptions.RemoveEmptyEntries).Contains(item))) ? mdADOs.Where(t => mdADO.SERVICE_ID != t.SERVICE_ID && t.ATC_GROUP_CODES.Split(new string[] { ";", "," }, StringSplitOptions.RemoveEmptyEntries).Contains(item)).ToList() : null;
                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => item), item)
                        + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => mdADOsWarnDuplicates), mdADOsWarnDuplicates));
                        if (mdADOsWarnDuplicates != null && mdADOsWarnDuplicates.Count > 0)
                        {
                            valid = true;
                            if (dicAtcWarn == null) dicAtcWarn = new Dictionary<string, List<MediMatyTypeADO>>();
                            dicAtcWarn.Add(item, mdADOsWarnDuplicates);
                        }
                    }
                }
            }
            return valid;
        }
        private bool CheckOverlapWarningOption()
        {
            bool result = true;
            try
            {
                if (HisConfigCFG.AtcCodeOverlarWarningOption == "1")
                {
                    var mediMatyTypeOverlapWarnings = this.mediMatyTypeADOs.Where(o => !String.IsNullOrEmpty(o.ATC_CODES) && (o.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC
                    || o.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC_DM
                    || o.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC_TUTUC)).ToList();
                    if (mediMatyTypeOverlapWarnings != null && mediMatyTypeOverlapWarnings.Count > 0)
                    {
                        List<string> medicineTypeNames = new List<string>();
                        var atcDatas = BackendDataWorker.Get<HIS_ATC>().Where(o => o.IS_ACTIVE == 1).ToList();
                        Dictionary<string, List<MediMatyTypeADO>> dicAtcWarn = new Dictionary<string, List<MediMatyTypeADO>>();

                        foreach (var item in mediMatyTypeOverlapWarnings)
                        {
                            Dictionary<string, List<MediMatyTypeADO>> dicAtcWarnOne = new Dictionary<string, List<MediMatyTypeADO>>();
                            List<MediMatyTypeADO> mdADOsWarn = new List<MediMatyTypeADO>();
                            if (IsWarnAtcByCode(item, mediMatyTypeOverlapWarnings, ref dicAtcWarnOne))
                            {
                                if (dicAtcWarnOne != null && dicAtcWarnOne.Count > 0)
                                {
                                    foreach (var itemAtcSearch in dicAtcWarnOne)
                                    {
                                        var atc = atcDatas != null && atcDatas.Count > 0 ? atcDatas.Where(kk => kk.ATC_CODE == itemAtcSearch.Key).FirstOrDefault() : null;
                                        if (atc != null)
                                        {
                                            mdADOsWarn = itemAtcSearch.Value;
                                            if (dicAtcWarn.ContainsKey(atc.ATC_CODE))
                                            {
                                                var listValue = dicAtcWarn[atc.ATC_CODE];
                                                listValue.AddRange(mdADOsWarn);
                                                dicAtcWarn[atc.ATC_CODE] = listValue.Distinct().ToList();
                                            }
                                            else
                                            {
                                                dicAtcWarn.Add(atc.ATC_CODE, mdADOsWarn);
                                            }
                                        }
                                        else
                                        {
                                            Inventec.Common.Logging.LogSystem.Debug("Khong tim thay atc theo ma:" + itemAtcSearch.Key
                                                + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => atcDatas), atcDatas)
                                                + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => item), item));
                                        }
                                    }
                                }

                            }
                        }

                        if (dicAtcWarn != null && dicAtcWarn.Count > 0)
                        {
                            foreach (var aw in dicAtcWarn)
                            {
                                string medicineTypeName = (aw.Value.Aggregate((i, j) => new MediMatyTypeADO { ACTIVE_INGR_BHYT_NAME = i.ACTIVE_INGR_BHYT_NAME + " - " + j.ACTIVE_INGR_BHYT_NAME }).ACTIVE_INGR_BHYT_NAME) + " (" + aw.Key + " - " + GetAtcNameByCode(aw.Key) + ")";
                                medicineTypeNames.Add(medicineTypeName);
                            }
                        }

                        if (medicineTypeNames != null && medicineTypeNames.Count > 0)
                        {
                            if (DevExpress.XtraEditors.XtraMessageBox.Show("Trùng nhóm dược tính:\r\n" + String.Join("\r\n", medicineTypeNames) + "\r\nBạn có muốn tiếp tục?", Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaCanhBao), MessageBoxButtons.YesNo, DevExpress.Utils.DefaultBoolean.True) != System.Windows.Forms.DialogResult.Yes)
                                result = false;
                        }
                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => medicineTypeNames), medicineTypeNames)
                            + "_______" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => dicAtcWarn), dicAtcWarn));
                    }
                }
                else if (HisConfigCFG.AtcCodeOverlarWarningOption == "2")
                {
                    var mediMatyTypeOverlapWarnings = this.mediMatyTypeADOs.Where(o => !String.IsNullOrEmpty(o.ATC_GROUP_CODES) && (o.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC)).ToList();
                    if (mediMatyTypeOverlapWarnings != null && mediMatyTypeOverlapWarnings.Count > 0)
                    {
                        List<string> medicineTypeNames = new List<string>();
                        var atcGroupDatas = BackendDataWorker.Get<HIS_ATC_GROUP>().Where(o => o.IS_ACTIVE == 1).ToList();
                        Dictionary<string, List<MediMatyTypeADO>> dicAtcWarn = new Dictionary<string, List<MediMatyTypeADO>>();

                        foreach (var item in mediMatyTypeOverlapWarnings)
                        {
                            Dictionary<string, List<MediMatyTypeADO>> dicAtcWarnOne = new Dictionary<string, List<MediMatyTypeADO>>();
                            List<MediMatyTypeADO> mdADOsWarn = new List<MediMatyTypeADO>();
                            if (IsWarnAtcGroupByCode(item, mediMatyTypeOverlapWarnings, ref dicAtcWarnOne))
                            {
                                if (dicAtcWarnOne != null && dicAtcWarnOne.Count > 0)
                                {
                                    foreach (var itemAtcSearch in dicAtcWarnOne)
                                    {
                                        var atcGroup = atcGroupDatas != null && atcGroupDatas.Count > 0 ? atcGroupDatas.Where(kk => kk.ATC_GROUP_CODE == itemAtcSearch.Key).FirstOrDefault() : null;
                                        if (atcGroup != null)
                                        {
                                            mdADOsWarn = itemAtcSearch.Value;
                                            if (dicAtcWarn.ContainsKey(atcGroup.ATC_GROUP_CODE))
                                            {
                                                var listValue = dicAtcWarn[atcGroup.ATC_GROUP_CODE];
                                                listValue.AddRange(mdADOsWarn);
                                                dicAtcWarn[atcGroup.ATC_GROUP_CODE] = listValue.Distinct().ToList();
                                            }
                                            else
                                            {
                                                dicAtcWarn.Add(atcGroup.ATC_GROUP_CODE, mdADOsWarn);
                                            }
                                        }
                                        else
                                        {
                                            Inventec.Common.Logging.LogSystem.Debug("Khong tim thay atc group theo ma:" + itemAtcSearch.Key
                                                + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => atcGroupDatas), atcGroupDatas)
                                                + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => item), item));
                                        }
                                    }
                                }

                            }
                        }

                        if (dicAtcWarn != null && dicAtcWarn.Count > 0)
                        {
                            foreach (var aw in dicAtcWarn)
                            {
                                string medicineTypeName = (aw.Value.Aggregate((i, j) => new MediMatyTypeADO { MEDICINE_TYPE_NAME = i.MEDICINE_TYPE_NAME + " - " + j.MEDICINE_TYPE_NAME }).MEDICINE_TYPE_NAME) + " (" + GetAtcGroupNameByCode(aw.Key) + ")";
                                medicineTypeNames.Add(medicineTypeName);
                            }
                        }

                        if (medicineTypeNames != null && medicineTypeNames.Count > 0)
                        {
                            if (DevExpress.XtraEditors.XtraMessageBox.Show("Trùng nhóm dược lý:\r\n" + String.Join("\r\n", medicineTypeNames) + "\r\nBạn có muốn tiếp tục?", Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaCanhBao), MessageBoxButtons.YesNo, DevExpress.Utils.DefaultBoolean.True) != System.Windows.Forms.DialogResult.Yes)
                                result = false;
                        }
                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => medicineTypeNames), medicineTypeNames)
                            + "_______" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => dicAtcWarn), dicAtcWarn));
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
        private string GetAtcNameByCode(string code)
        {
            string rs = "";
            var atcDatas = BackendDataWorker.Get<HIS_ATC>();
            var atc = atcDatas != null && atcDatas.Count > 0 ? atcDatas.Where(o => o.ATC_CODE == code).FirstOrDefault() : null;
            rs = atc != null ? atc.ATC_NAME : "";
            return rs;
        }
        private string GetAtcGroupNameByCode(string code)
        {
            string rs = "";
            var atcDatas = BackendDataWorker.Get<HIS_ATC_GROUP>();
            var atc = atcDatas != null && atcDatas.Count > 0 ? atcDatas.Where(o => o.ATC_GROUP_CODE == code).FirstOrDefault() : null;
            rs = atc != null ? atc.ATC_GROUP_NAME : "";
            return rs;
        }
        private bool ProcessValidMedicineTypeAge()
        {
            return ProcessValidMedicineTypeAge(false);
        }

        private bool ProcessValidMedicineTypeAge(bool isRemoveRowError)
        {
            bool valid = true;
            try
            {
                string messageErr = "";
                List<MediMatyTypeADO> mediMatyTypeADOTemps = new List<MediMatyTypeADO>();
                foreach (var item in this.mediMatyTypeADOs)
                {
                    if (item.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC
                        || item.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.VATTU
                        || item.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC_DM
                        || item.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.VATTU_DM
                        || item.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.VATTU_TSD)
                    {
                        string messageDetail = "";
                        if ((item.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC
                            || item.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC_DM)
                            && !MedicineAgeWorker.ValidThuocCoGioiHanTuoi(item.SERVICE_ID, this.patientDob, ref messageDetail, false))
                        {
                            if (!String.IsNullOrEmpty(messageDetail))
                            {
                                messageErr += messageDetail + "\r\n";
                            }
                        }
                        else
                            mediMatyTypeADOTemps.Add(item);
                    }
                }
                if (!String.IsNullOrEmpty(messageErr))
                {
                    if (isRemoveRowError)
                    {
                        this.mediMatyTypeADOs = new List<MediMatyTypeADO>();
                        this.mediMatyTypeADOs.AddRange(mediMatyTypeADOTemps);
                    }
                    else
                        valid = false;
                    MessageManager.Show(messageErr);
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => messageErr), messageErr) + "____PatientAge:" + this.patientDob);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return valid;
        }

        /// <summary>
        /// Cảnh báo thuốc không có trong kho hoặc số lượng khả dụng không đủ
        /// </summary>
        private bool ProcessAmountInStockWarning()
        {
            bool result = true;
            try
            {
                string message = "";
                var mediMatyTypeInStockWarnings = this.mediMatyTypeADOs.Where(o => (o.AmountAlert ?? 0) > 0).Select(o => new { o.MEDICINE_TYPE_NAME, o.AmountAlert, o.SERVICE_ID }).Distinct().ToList();
                if (mediMatyTypeInStockWarnings != null && mediMatyTypeInStockWarnings.Count > 0)
                {
                    foreach (var item in mediMatyTypeInStockWarnings)
                    {
                        message += (Inventec.Desktop.Common.HtmlString.ProcessorString.InsertColor(item.MEDICINE_TYPE_NAME, System.Drawing.Color.Red)
                            + " : " + Inventec.Desktop.Common.HtmlString.ProcessorString.InsertColor(Inventec.Common.Number.Convert.NumberToString((item.AmountAlert ?? 0), ConfigApplications.NumberSeperator), System.Drawing.Color.Maroon) + "; ");
                    }

                    DevExpress.XtraEditors.XtraMessageBox.Show(String.Format(ResourceMessage.VuotQuaSoLuongKhaDungTrongKho, message), Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaCanhBao), MessageBoxButtons.OK, MessageBoxIcon.Warning, DevExpress.Utils.DefaultBoolean.True);
                    //message += ResourceMessage.VuotQuaSoLuongKhaDungTrongKho__CoMuonTiepTuc;
                    //DialogResult dialogResult = DevExpress.XtraEditors.XtraMessageBox.Show(String.Format(ResourceMessage.VuotQuaSoLuongKhaDungTrongKho, message), Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaCanhBao), MessageBoxButtons.OK, MessageBoxIcon.Warning, DevExpress.Utils.DefaultBoolean.True);
                    //if (dialogResult == DialogResult.Yes)
                    //{
                    //    //Chuyển các thuốc hết khả dụng sang thuốc ngoài kho
                    //    foreach (var item in mediMatyTypeInStockWarnings)
                    //    {
                    //        var medimatyWorker = this.mediMatyTypeADOs.FirstOrDefault(o => o.SERVICE_ID == item.SERVICE_ID);
                    //        medimatyWorker.PATIENT_TYPE_ID = null;
                    //        medimatyWorker.PATIENT_TYPE_CODE = null;
                    //        medimatyWorker.PATIENT_TYPE_NAME = null;
                    //        medimatyWorker.DataType = (item.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC ? HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC_DM : HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.VATTU_DM);
                    //        medimatyWorker.AmountAlert = null;
                    //        medimatyWorker.MEDI_STOCK_ID = null;
                    //        medimatyWorker.MEDI_STOCK_CODE = null;
                    //        medimatyWorker.MEDI_STOCK_NAME = null;
                    //        medimatyWorker.ErrorMessageAmountAlert = "";
                    //        medimatyWorker.ErrorMessagePatientTypeId = "";
                    //        medimatyWorker.ErrorTypeAmountAlert = ErrorType.None;
                    //        medimatyWorker.ErrorTypePatientTypeId = ErrorType.None;
                    //        medimatyWorker.ErrorTypeMediMatyBean = ErrorType.None;
                    //    }
                    //}
                    //else if (dialogResult == DialogResult.No)
                    //{
                    //    //this.mediMatyTypeADOs = new List<MediMatyTypeADO>();
                    //    //this.gridControlServiceProcess.DataSource = null;
                    //    //result = false;
                    //}
                    //else if (dialogResult == DialogResult.Cancel)
                    //{
                    //    this.mediMatyTypeADOs = new List<MediMatyTypeADO>();
                    //    this.gridControlServiceProcess.DataSource = null;
                    //    result = false;
                    //}
                }
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private void ReleaseAllMediByUser()
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("ReleaseAllMediByUser => 1");

                if (!GlobalStore.IsTreatmentIn || GlobalStore.IsCabinet)
                {
                    TakeOrReleaseBeanWorker.ProcessReleaseAllMaty();
                    TakeOrReleaseBeanWorker.ProcessReleaseAllMedi();
                }

                if (this.mediMatyTypeADOs != null && this.mediMatyTypeADOs.Count > 0)
                {
                    this.idRow = 1;
                    this.gridControlServiceProcess.DataSource = null;
                    this.mediMatyTypeADOs = new List<MediMatyTypeADO>();
                }

                Inventec.Common.Logging.LogSystem.Debug("ReleaseAllMediByUser => 2");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private bool CheckChangeSelectedPatientWhileHasPrescription()
        {
            bool hasPrescription = true;
            try
            {
                if (this.mediMatyTypeADOs != null
                    && this.mediMatyTypeADOs.Count > 0
                    && (this.actionType == GlobalVariables.ActionAdd || this.actionType == GlobalVariables.ActionEdit)
                    )
                {
                    if (MessageBox.Show(ResourceMessage.ThuocDaduocKeVaoDanhSachThuocChobenNhanBanCoMuonChuyenSangBNKhac, HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        hasPrescription = true;
                    }
                    else
                    {
                        hasPrescription = false;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return hasPrescription;
        }

        private int GetDataTypeSelected()
        {
            int dataType = 0;
            try
            {
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => currentMedicineTypeADOForEdit), currentMedicineTypeADOForEdit) + "____" + Inventec.Common.Logging.LogUtil.TraceData("IS_EXECUTE_KIDNEY_PRES", (this.oldServiceReq != null ? this.oldServiceReq.IS_EXECUTE_KIDNEY_PRES : null)));
                dataType = (this.currentMedicineTypeADOForEdit.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC ?
                        HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC : HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.VATTU);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return dataType;
        }

        private object GetDataMediMatySelected()
        {
            object dataInput = null;
            try
            {
                dataInput = this.currentMedicineTypeADOForEdit;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return dataInput;
        }

        private void UpdateMediMatyClickHandler()
        {
            CommonParam param = new CommonParam();
            try
            {
                int datatype = GetDataTypeSelected();
                HIS.Desktop.Plugins.AssignPrescriptionCLS.Edit.IEdit iEdit = HIS.Desktop.Plugins.AssignPrescriptionCLS.Edit.EditFactory.MakeIEdit(
                param,
                this,
                ValidAddRow,
                ChoosePatientTypeDefaultlService,
                ChoosePatientTypeDefaultlServiceOther,
                CalulateUseTimeTo,
                ExistsAssianInDay,
                this.currentMedicineTypeADOForEdit,
                datatype);

                if (iEdit != null)
                {
                    var success = iEdit.Run();
                    if (!success)
                    {
                        //LogSystem.Debug("Edit medicine/ material row error => success fail");
                    }
                }
                else
                {
                    MessageManager.Show(param, false);
                    LogSystem.Error("Edit medicine/ material row error => iEdit is null.");
                }
            }
            catch (Exception ex)
            {
                MessageManager.Show(param, false);
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void AddMediMatyClickHandler()
        {
            CommonParam param = new CommonParam();
            try
            {
                int datatype = GetDataTypeSelected();
                HIS.Desktop.Plugins.AssignPrescriptionCLS.Add.IAdd iAdd = HIS.Desktop.Plugins.AssignPrescriptionCLS.Add.AddFactory.MakeIAdd(
                param,
                this,
                ValidAddRow,
                ChoosePatientTypeDefaultlService,
                ChoosePatientTypeDefaultlServiceOther,
                CalulateUseTimeTo,
                ExistsAssianInDay,
                this.currentMedicineTypeADOForEdit,
                datatype);

                if (iAdd != null)
                {
                    var success = iAdd.Run();
                    if (!success)
                    {
                        //LogSystem.Debug("Add medicine/ material row error => success fail");
                    }
                }
                else
                {
                    MessageManager.Show(param, false);
                    LogSystem.Debug("Add medicine/ material row error => iAdd is null.");
                }
            }
            catch (Exception ex)
            {
                MessageManager.Show(param, false);
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }


        /// <summary>
        /// Load lại dữ liệu của tab page đang chọn
        /// </summary>
        private async Task LoadDataToGridMetyMatyTypeInStock()
        {
            try
            {
                this.theRequiredWidth = 1130;
                this.theRequiredHeight = 200;
                this.RebuildMediMatyWithInControlContainer();
                this.ResetFocusMediMaty(true);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal void ResetFocusMediMaty(bool isFocus, bool fucusOnly = false)
        {
            try
            {
                if (!fucusOnly)
                {
                    currentMedicineTypeADOForEdit = null;
                    txtMediMatyForPrescription.Text = "";
                }

                if (isFocus)
                    txtMediMatyForPrescription.Focus();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetEnableControlMedicine()
        {
            try
            {
                UC.DateEditor.ADO.DateInputADO dateInputADO = new UC.DateEditor.ADO.DateInputADO();
                dateInputADO.Time = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(this.intructionTimeSelecteds.First()) ?? new DateTime();
                dateInputADO.Dates = new List<DateTime?>();
                dateInputADO.Dates.Add(dateInputADO.Time);

                spinAmount.Enabled =
                txtMediMatyForPrescription.Enabled = txtTutorial.Enabled = true;
                dateInputADO.IsVisibleMultiDate = true;

                if (GlobalStore.IsTreatmentIn && (this.actionType != GlobalVariables.ActionEdit)
                         && !GlobalStore.IsCabinet)
                {
                    this.UcDateReload(dateInputADO);

                    //this.ucDateProcessor.Reload(this.ucDate, dateInputADO);
                    //this.isMultiDateState = this.ucDateProcessor.GetChkMultiDateState(this.ucDate);
                    //this.intructionTimeSelecteds = this.ucDateProcessor.GetValue(this.ucDate);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidRowChange(MediMatyTypeADO mediMatyTypeADO)
        {
            try
            {
                mediMatyTypeADO.IsAssignDay = false;
                mediMatyTypeADO.ErrorMessageIsAssignDay = "";
                mediMatyTypeADO.ErrorTypeIsAssignDay = ErrorType.None;
                mediMatyTypeADO.AmountAlert = null;
                //Kiểm tra khả dụng của thuốc trong kho
                //Nếu số thuốc nhập lại - số thuốc cũ > khả dung -> đưa ra cảnh báo
                var listData = this.gridControlServiceProcess.DataSource as List<MediMatyTypeADO>;
                if (listData != null && (mediMatyTypeADO.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC || mediMatyTypeADO.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.VATTU))
                {
                    if (!(mediMatyTypeADO.PRE_AMOUNT > 0 && mediMatyTypeADO.PRE_AMOUNT == mediMatyTypeADO.AMOUNT))
                    {
                        var checkMatyInStock = AmountOutOfStock(mediMatyTypeADO, mediMatyTypeADO.SERVICE_ID, (mediMatyTypeADO.MEDI_STOCK_ID ?? 0));
                        //checkMatyInStock = ((mediMatyTypeADO.IsUseOrginalUnitForPres ?? false) == false && mediMatyTypeADO.CONVERT_RATIO.HasValue && mediMatyTypeADO.CONVERT_RATIO > 0) ? checkMatyInStock * mediMatyTypeADO.CONVERT_RATIO : checkMatyInStock;

                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => checkMatyInStock), checkMatyInStock));
                        if ((checkMatyInStock ?? 0) <= 0)//Trường hợp thuốc đã hết khả dụng yêu cầu trong kho
                        {
                            Inventec.Common.Logging.LogSystem.Debug("ValidRowChange. 1");
                            if ((mediMatyTypeADO.AMOUNT ?? 0) > ((mediMatyTypeADO.PRE_AMOUNT ?? 0) > 0 ? (mediMatyTypeADO.PRE_AMOUNT ?? 0) : (mediMatyTypeADO.BK_AMOUNT ?? 0)))
                            {
                                mediMatyTypeADO.AmountAlert = mediMatyTypeADO.AMOUNT;
                                Inventec.Common.Logging.LogSystem.Debug("ValidRowChange. 1.1");
                            }
                        }
                        else
                        {
                            Inventec.Common.Logging.LogSystem.Debug("ValidRowChange. 2");
                            var amountProcess = listData
                                .Where(o => o.MEDI_STOCK_ID == mediMatyTypeADO.MEDI_STOCK_ID
                                        && o.SERVICE_ID == mediMatyTypeADO.SERVICE_ID)
                                        .Sum(o => (((o.AMOUNT ?? 0) - ((!GlobalStore.IsTreatmentIn || GlobalStore.IsCabinet) ? o.BK_AMOUNT ?? 0 : 0)) - (o.PRE_AMOUNT ?? 0)));

                            if (amountProcess > 0 && amountProcess > checkMatyInStock)
                            {
                                mediMatyTypeADO.AmountAlert = amountProcess;
                                Inventec.Common.Logging.LogSystem.Debug("ValidRowChange. 2.1");
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

        internal void ReSetDataInputAfterAdd__MedicinePage()
        {
            try
            {
                isNotProcessRunWhileFilmChangedValue = true;
                this.cboMedicineUseForm.EditValue = null;
                this.spinAmount.Text = "";
                txtHtu.Text = "";
                txtTutorial.Text = "";
                this.spinSoPhimHong.EditValue = null;
                this.spinSoPhimHong.Enabled = false;
                this.chkPhimHong.Checked = false;
                this.currentSoPhimHong = 0;
                isNotProcessRunWhileFilmChangedValue = false;
                Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(this.dxValidProviderBoXung, this.dxErrorProvider1);
                Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(this.dxValidationProviderMaterialTypeTSD, this.dxErrorProvider1);
                Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(this.dxValidProviderBoXung__DuongDung, this.dxErrorProvider1);

                this.gridControlTutorial.DataSource = null;
                this.gridViewMediMaty.ActiveFilter.Clear();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal void SetEnableButtonControl(int actionType)
        {
            try
            {
                this.actionBosung = GlobalVariables.ActionAdd;
                this.VisibleButton(this.actionBosung);

                if (this.actionType == GlobalVariables.ActionAdd)
                {
                    List<MediMatyTypeADO> serviceCheckeds__Send = this.mediMatyTypeADOs;
                    this.btnSave.Enabled = btnSaveAndPrint.Enabled = btnSaveAndShowPrintPreview.Enabled = (serviceCheckeds__Send != null && serviceCheckeds__Send.Count > 0);
                    this.lciPrintAssignPrescription.Enabled = false;
                    this.btnAdd.Enabled = true;//this.btnNew.Enabled =
                    this.gridViewServiceProcess.OptionsBehavior.Editable = true;
                }
                else if (this.actionType == GlobalVariables.ActionEdit)
                {
                    List<MediMatyTypeADO> serviceCheckeds__Send = this.mediMatyTypeADOs;
                    this.btnSave.Enabled = btnSaveAndPrint.Enabled = btnSaveAndShowPrintPreview.Enabled = true;
                    this.lciPrintAssignPrescription.Enabled = true;
                    this.btnAdd.Enabled = true;
                    this.btnNew.Enabled = (oldServiceReq != null ? false : true);
                    this.gridViewServiceProcess.OptionsBehavior.Editable = true;
                }
                else
                {
                    this.btnSave.Enabled = this.btnAdd.Enabled = btnSaveAndPrint.Enabled = btnSaveAndShowPrintPreview.Enabled = false;
                    this.lciPrintAssignPrescription.Enabled = true;
                    if (this.assignPrescriptionEditADO == null)
                        this.btnNew.Enabled = true;
                    this.gridViewServiceProcess.OptionsBehavior.Editable = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitDataByExpMestTemplate()
        {
            try
            {
                if (actionType == GlobalVariables.ActionAdd)
                {
                    if (this.expMestTemplateId.HasValue && this.expMestTemplateId > 0)
                    {
                        Inventec.Common.Logging.LogSystem.Debug("InitDataByExpMestTemplate. 1");
                        MOS.EFMODEL.DataModels.HIS_EXP_MEST_TEMPLATE data = BackendDataWorker.Get<HIS_EXP_MEST_TEMPLATE>().Where(o => o.ID == this.expMestTemplateId).FirstOrDefault();
                        if (data != null)
                        {
                            this.txtExpMestTemplateCode.Text = data.EXP_MEST_TEMPLATE_CODE;
                            this.cboExpMestTemplate.EditValue = data.ID;
                            this.cboExpMestTemplate.Properties.Buttons[1].Visible = true;

                            //Trường hợp chưa chọn kho thì tự động hiển thị kho cho người dùng chọn
                            if (this.currentMediStock == null || this.currentMediStock.Count == 0)
                            {
                                Inventec.Common.Logging.LogSystem.Debug("Truong hop khoi tao goi vat tu khi load form ke don & danh sach kho duoc chon dang trong ==> tu dong mo form chon kho de nguoi dung chon ==> khoi tao san du lieu thuoc theo goi vat tu da chon");
                                frmWorkingMediStock frmWorkingMediStock = new frmWorkingMediStock(currentWorkingMestRooms, ProcessAfterChooseformMediStock);
                                frmWorkingMediStock.ShowDialog();
                            }
                            else
                            {
                                Inventec.Common.Logging.LogSystem.Debug("Truong hop khoi tao goi vat tu khi load form ke don & da co danh sach kho duoc chon ==> khoi tao san du lieu thuoc theo goi vat tu da chon");
                                ProcessAfterChooseformMediStock(this.currentMediStock);
                            }
                        }
                        else
                        {
                            Inventec.Common.Logging.LogSystem.Debug("Du lieu truyen vao expMestTemplateId khong ton tai ban ghi tuong ung trong DB, khong the khoi tao goi vat tu (don mau) khi load form ke don: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => this.expMestTemplateId), this.expMestTemplateId));
                        }
                        Inventec.Common.Logging.LogSystem.Debug("InitDataByExpMestTemplate. 2");
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ProcessAfterChooseformMediStock(List<MOS.EFMODEL.DataModels.V_HIS_MEST_ROOM> selectedMestRooms)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("ProcessAfterChooseformMediStock. 1");
                this.currentMediStock = selectedMestRooms;
                GridCheckMarksSelection gridCheckMark = this.cboMediStockExport.Properties.Tag as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    if (gridCheckMark.Selection.Count == 0)
                        gridCheckMark.SelectAll(this.currentMediStock);
                }
                else
                {
                    if (this.currentMediStock != null && this.currentMediStock.Count > 0)
                        this.cboMediStockExport.EditValue = this.currentMediStock.First().MEDI_STOCK_ID;
                }

                if (this.mediMatyTypeAvailables == null || this.mediMatyTypeAvailables.Count == 0)
                    this.InitDataMetyMatyTypeInStockD(this.currentMediStock);
                MOS.EFMODEL.DataModels.HIS_EXP_MEST_TEMPLATE data = BackendDataWorker.Get<HIS_EXP_MEST_TEMPLATE>().Where(o => o.ID == this.expMestTemplateId).FirstOrDefault();
                this.ProcessChoiceExpMestTemplateForChayThan(data);
                this.ResetFocusMediMaty(true);
                Inventec.Common.Logging.LogSystem.Debug("ProcessAfterChooseformMediStock. 2");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitDataByServicePackage()
        {
            try
            {
                if (actionType == GlobalVariables.ActionAdd)
                {
                    if (this.currentSereServ != null || this.currentSereServInEkip != null)
                    {
                        LogSystem.Debug("Loaded InitDataByServicePackage. 1");
                        var serviceLeaf__Medicine_Materials = (
                              from m in BackendDataWorker.Get<V_HIS_SERVICE>()
                              where
                               m.IS_LEAF == 1
                              select m
                              ).Distinct().ToList();

                        if (this.currentSereServ != null)
                        {
                            CreateThreadLoadDataByPackageService(currentSereServ);
                            this.Service__Main = serviceLeaf__Medicine_Materials
                                .Where(o => o.ID == this.currentSereServ.SERVICE_ID)
                                .FirstOrDefault();
                        }

                        if (this.currentSereServInEkip != null)
                        {
                            CreateThreadLoadDataByPackageService(currentSereServInEkip);
                            this.Service__Main = serviceLeaf__Medicine_Materials
                                .Where(o => o.ID == this.currentSereServInEkip.SERVICE_ID)
                                .FirstOrDefault();
                        }
                        LogSystem.Debug("Loaded InitDataByServicePackage. 2");
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Kiểm tra dịch vụ đã được chỉ định trong ngày hay chưa
        /// </summary>
        /// <param name="serviceId"></param>
        /// <returns></returns>
        private bool IsAssignDay(long serviceId)
        {
            bool result = false;
            try
            {
                if (this.sereServWithTreatment != null)
                {
                    result = this.sereServWithTreatment.Any(o => o.SERVICE_ID == serviceId && o.TDL_INTRUCTION_TIME.ToString().Substring(0, 8) == intructionTimeSelecteds.OrderByDescending(t => t).First().ToString().Substring(0, 8));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        /// <summary>
        /// Nếu phòng đang làm việc là buồng bệnh (nội trú) thì cho phép kê số lượng nguyên, phân số vd: 1; 1/2
        /// Ngược lại nếu đối tượng ngoại trú & kho đang chọn là "tủ trực" thì sẽ cho kê số lượng lẻ vd: 1; 1.15; 1/2
        /// </summary>
        /// <param name="mediStockId"></param>
        private void ChangeControlSoLuongNgayNhapChanLe(long mediStockId)
        {
            try
            {
                long roomTypeId = (currentModule != null ? currentModule.RoomTypeId : 0);
                if (roomTypeId == IMSys.DbConfig.HIS_RS.HIS_ROOM_TYPE.ID__BUONG)
                    return;

                var mediStock = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_MEDI_STOCK>().FirstOrDefault(o => o.ID == mediStockId);
                if (mediStock != null && mediStock.IS_CABINET == GlobalVariables.CommonNumberTrue)
                {
                    this.repositoryItemSpinAmount__MedicinePage.BeginUpdate();
                    this.spinAmount.Properties.DisplayFormat.FormatString = "#,##0.00";
                    this.spinAmount.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.None;
                    this.spinAmount.Properties.EditFormat.FormatString = "#,##0.00";
                    this.spinAmount.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.None;
                    this.spinAmount.Properties.Mask.EditMask = "n";
                    this.spinAmount.Properties.Mask.UseMaskAsDisplayFormat = true;

                    this.repositoryItemSpinAmount__MedicinePage.Properties.DisplayFormat.FormatString = "#,##0.00";
                    this.repositoryItemSpinAmount__MedicinePage.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.None;
                    this.repositoryItemSpinAmount__MedicinePage.Properties.EditFormat.FormatString = "#,##0.00";
                    this.repositoryItemSpinAmount__MedicinePage.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.None;
                    this.repositoryItemSpinAmount__MedicinePage.Properties.Mask.EditMask = "n";
                    this.repositoryItemSpinAmount__MedicinePage.Properties.Mask.UseMaskAsDisplayFormat = true;
                }
                else
                {
                    this.spinAmount.Properties.Mask.EditMask = "d";
                    this.spinAmount.Properties.Mask.UseMaskAsDisplayFormat = true;
                    this.spinAmount.Properties.DisplayFormat.FormatString = "";
                    this.spinAmount.Properties.EditFormat.FormatString = "";

                    this.repositoryItemSpinAmount__MedicinePage.Properties.Mask.EditMask = "d";
                    this.repositoryItemSpinAmount__MedicinePage.Properties.Mask.UseMaskAsDisplayFormat = true;
                    this.repositoryItemSpinAmount__MedicinePage.Properties.DisplayFormat.FormatString = "";
                    this.repositoryItemSpinAmount__MedicinePage.Properties.EditFormat.FormatString = "";
                }
                this.spinAmount.Update();
                this.repositoryItemSpinAmount__MedicinePage.EndUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ResetDataForm()
        {
            try
            {
                repositoryItemSpinAmount_Disable__MedicinePage.Enabled = false;
                grcTocDoTruyen.Visible = false;
                repositoryItemchkIsExpendType_Disable.Enabled = false;
                repositoryItemchkIsExpendType_Disable.ReadOnly = true;
                repositoryItemChkIsExpend__MedicinePage_Disable.Enabled = false;
                repositoryItemChkIsExpend__MedicinePage_Disable.ReadOnly = true;

                gridColumnManyDate.Visible = (HisConfigCFG.ManyDayPrescriptionOption == 2 && (GlobalStore.IsTreatmentIn && !GlobalStore.IsCabinet));
                IsHandlerWhileOpionGroupSelectedIndexChanged = this.assignPrescriptionEditADO != null && this.assignPrescriptionEditADO.ServiceReq != null && this.assignPrescriptionEditADO.ServiceReq.ID > 0;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Gan gia trị mac dinh cho cac control can khoi tao san gia tri
        /// </summary>
        private void SetDefaultData()
        {
            try
            {
                //Load so ngay theo cau hinh tai khoan ke don phong kham
                if (!GlobalStore.IsTreatmentIn && !GlobalStore.IsCabinet && !GlobalStore.IsExecutePTTT)
                {
                    int numOfDay = ConfigApplicationWorker.Get<int>(AppConfigKeys.CONFIG_KEY__HIS_DESKTOP__ASSIGN_PRESCRIPTION__DEFAULT_NUM_OF_DAY);
                    if (numOfDay > 0)
                    {
                        //this.spinSoNgay.Value = numOfDay;
                        Inventec.Common.Logging.LogSystem.Debug("SetDefaultData => kiem tra co cau hình CONFIG_KEY__HIS_DESKTOP__ASSIGN_PRESCRIPTION__DEFAULT_NUM_OF_DAY= " + numOfDay + ", lay gan gia tri vao spinSoNgay ");
                    }
                }

                this.lblTongTien.Text = "";
                this.idRow = 1;

                this.btnSave.Enabled = false;
                this.btnSaveAndPrint.Enabled = false;
                this.btnSaveAndShowPrintPreview.Enabled = false;
                this.lciPrintAssignPrescription.Enabled = false;
                this.btnAdd.Enabled = false;

                this.actionType = (this.assignPrescriptionEditADO != null ? GlobalVariables.ActionEdit : GlobalVariables.ActionAdd);
                this.actionBosung = GlobalVariables.ActionAdd;

                this.gridControlServiceProcess.DataSource = null;
                this.mediMatyTypeADOs = new List<MediMatyTypeADO>();
                this.cboExpMestTemplate.EditValue = null;
                this.cboExpMestTemplate.Properties.Buttons[1].Visible = false;
                this.txtExpMestTemplateCode.Text = "";

                GlobalStore.ClientSessionKey = Guid.NewGuid().ToString();

                this.currentMedicineTypes = BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>().Where(o => o.IS_LEAF == GlobalVariables.CommonNumberTrue).ToList();
                //long isOnlyDisplayMediMateIsBusiness = Inventec.Common.TypeConvert.Parse.ToInt64(HisConfigs.Get<string>(HisConfigCFG.ONLY_DISPLAY_MEDIMATE_IS_BUSINESS));
                //if (isOnlyDisplayMediMateIsBusiness == 1 && this.currentMedicineTypes != null && this.currentMedicineTypes.Count > 0)
                //    this.currentMedicineTypes = this.currentMedicineTypes.Where(o => o.IS_BUSINESS.HasValue && o.IS_BUSINESS.Value == 1).ToList();

                this.currentMaterialTypes = BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>().Where(o => o.IS_LEAF == GlobalVariables.CommonNumberTrue).ToList();

                Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(this.dxValidationProviderControl, dxErrorProvider1);

                cboPatientType.EditValue = null;
                cboPatientType.Properties.DataSource = null;

                this.requestRoom = BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == this.currentModule.RoomId);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void CreateThreadLoadDataByPackageService(object param)
        {
            Thread thread = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(LoadDataByPackageServiceNewThread));
            //thread.Priority = ThreadPriority.Highest;
            try
            {
                thread.Start(param);
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                thread.Abort();
            }
        }

        private void LoadDataByPackageServiceNewThread(object data)
        {
            try
            {
                if (this.InvokeRequired)
                {
                    this.Invoke(new MethodInvoker(delegate { LoadDataByPackageService((V_HIS_SERE_SERV)data); }));
                }
                else
                {
                    LoadDataByPackageService((V_HIS_SERE_SERV)data);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetTotalPriceInPackage(V_HIS_SERE_SERV sereServ)
        {
            try
            {
                CommonParam param = new CommonParam();
                //Lấy list service package
                HisSereServFilter sereServFilter = new HisSereServFilter();
                sereServFilter.IS_EXPEND = true;
                sereServFilter.PARENT_ID = sereServ.ID;
                var serviceInPackage__Expends = new BackendAdapter(param).Get<List<HIS_SERE_SERV>>(HisRequestUriStore.HIS_SERE_SERV_GET, ApiConsumers.MosConsumer, sereServFilter, ProcessLostToken, param);
                if (serviceInPackage__Expends != null && serviceInPackage__Expends.Count > 0)
                {
                    this.currentExpendInServicePackage = serviceInPackage__Expends.Sum(o => (o.AMOUNT * o.PRICE));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadDataByPackageService(V_HIS_SERE_SERV sereServ)
        {
            try
            {
                if (sereServ != null)
                {
                    CommonParam param = new CommonParam();
                    //Lấy list service package
                    HisServicePackageViewFilter filter = new HisServicePackageViewFilter();
                    filter.SERVICE_ID = sereServ.SERVICE_ID;
                    servicePackageByServices = new BackendAdapter(param).Get<List<V_HIS_SERVICE_PACKAGE>>(HisRequestUriStore.HIS_SERVICE_PACKAGE_GETVIEW, ApiConsumers.MosConsumer, filter, ProcessLostToken, param);
                    if (servicePackageByServices != null && servicePackageByServices.Count > 0)
                    {
                        List<long> serviceIds = servicePackageByServices.Select(o => o.SERVICE_ATTACH_ID).Distinct().ToList();
                        serviceInMaMeConfigs = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_SERVICE>().Where(o => serviceIds.Contains(o.ID)).ToList();
                        if (serviceInMaMeConfigs == null || serviceInMaMeConfigs.Count == 0)
                            throw new ArgumentNullException("serviceInPackages is null");

                        if (this.mediMatyTypeAvailables == null || this.mediMatyTypeAvailables.Count == 0)
                            this.InitDataMetyMatyTypeInStockD(this.currentMediStock);

                        //Release tat ca cac thuoc/ vat tu da duoc take bean truoc do nhung chua duoc luu
                        this.ReleaseAllMediByUser();

                        //Load data for material in package
                        //this.LoadPageMateMetyByListServiceConfig(serviceInMaMeConfigs);
                        this.ProcessInstructionTimeMediForEdit();
                        this.ProcessMergeDuplicateRowForListProcessing();
                        this.ProcessAddListRowDataIntoGridWithTakeBean();
                        this.ReloadDataAvaiableMediBeanInCombo();

                        //Kiểm tra trần hao phí của dịch vụ chính & các thuốc/vật tư đã kê, đưa ra cảnh báo nếu vượt trần
                        this.AlertOutofMaxExpend();
                    }

                    this.SetTotalPriceInPackage(sereServ);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private decimal? AmountOutOfStock(MediMatyTypeADO model, long serviceId, long meidStockId)
        {
            decimal? result = null;
            try
            {
                var checkMatyInStock = GetDataAmountOutOfStock(model, serviceId, meidStockId);
                if (checkMatyInStock != null)
                {
                    if (checkMatyInStock is DMediStock1ADO)
                    {
                        var medi1 = checkMatyInStock as DMediStock1ADO;
                        result = (medi1.AMOUNT ?? 0);
                    }
                    else if (checkMatyInStock is D_HIS_MEDI_STOCK_1)
                    {
                        var medi1 = checkMatyInStock as D_HIS_MEDI_STOCK_1;
                        result = ((model.IsUseOrginalUnitForPres ?? false) == false && model.CONVERT_RATIO.HasValue && model.CONVERT_RATIO > 0) ? medi1.AMOUNT * model.CONVERT_RATIO : medi1.AMOUNT;
                        result = (result ?? 0);
                    }
                    else if (checkMatyInStock is D_HIS_MEDI_STOCK_2)
                    {
                        var medi1 = checkMatyInStock as D_HIS_MEDI_STOCK_2;
                        result = ((model.IsUseOrginalUnitForPres ?? false) == false && model.CONVERT_RATIO.HasValue && model.CONVERT_RATIO > 0) ? medi1.AMOUNT * model.CONVERT_RATIO : medi1.AMOUNT;
                        result = (result ?? 0);
                    }
                }
            }
            catch (Exception ex)
            {
                result = null;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private object GetDataAmountOutOfStock(MediMatyTypeADO model, long serviceId, long meidStockId)
        {
            object result = null;
            try
            {
                if (this.mediMatyTypeAvailables == null || this.mediMatyTypeAvailables.Count == 0)
                {
                    Inventec.Common.Logging.LogSystem.Debug("GetDataAmountOutOfStock. 1 mediMatyTypeAvailables.count= 0");
                    this.InitDataMetyMatyTypeInStockD(this.currentMediStock);
                    Inventec.Common.Logging.LogSystem.Debug("GetDataAmountOutOfStock. 2 mediMatyTypeAvailables.count= " + (this.mediMatyTypeAvailables != null ? this.mediMatyTypeAvailables.Count : 0));
                }
                bool isTSD = model.DataType == LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.VATTU_TSD;
                var mediMatyByService = this.mediMatyTypeAvailables.Where(o => o.SERVICE_ID == serviceId && (meidStockId == 0 || o.MEDI_STOCK_ID == meidStockId)).ToList();

                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => mediMatyByService), mediMatyByService) + "" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => model), model));

                var result1 = this.mediMatyTypeAvailables.FirstOrDefault(o => o.SERVICE_ID == serviceId
                    && (((model.IsAssignPackage ?? false) == false) || (o.MAME_ID == model.MAME_ID))
                    && (((model.IsAssignPackage ?? false) == false || String.IsNullOrEmpty(model.TDL_PACKAGE_NUMBER)) || (!String.IsNullOrEmpty(model.TDL_PACKAGE_NUMBER) && o.TDL_PACKAGE_NUMBER == model.TDL_PACKAGE_NUMBER))
                    && (meidStockId == 0 || o.MEDI_STOCK_ID == meidStockId)
                    && ((isTSD && o.IS_REUSABLE == 1) || (!isTSD && (o.IS_REUSABLE == null || o.IS_REUSABLE != 1))));
                if (result1 != null && (model.AMOUNT ?? 0) > (result1.AMOUNT ?? 0))
                {
                    //model.AMOUNT = result1.AMOUNT;
                    //model.AmountAlert = result1.AMOUNT;
                }
                result = result1;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private void LoadPageMateMetyByListServiceConfig(List<MOS.EFMODEL.DataModels.V_HIS_SERVICE> serviceMaMes, List<HIS_SERVICE_METY> serviceMetys, List<HIS_SERVICE_MATY> serviceMatys)
        {
            try
            {
                if (serviceMaMes == null || serviceMaMes.Count == 0)
                    return;
                this.mediMatyTypeADOs = new List<MediMatyTypeADO>();
                foreach (var item in serviceMaMes)
                {
                    MediMatyTypeADO model = new MediMatyTypeADO();
                    Inventec.Common.Mapper.DataObjectMapper.Map<MediMatyTypeADO>(model, item);
                    model.MEDICINE_TYPE_CODE = item.SERVICE_CODE;
                    model.MEDICINE_TYPE_NAME = item.SERVICE_NAME;
                    model.SERVICE_ID = item.ID;
                    if (model.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC)
                    {
                        model.DataType = HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC;

                        var mety = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_MEDICINE_TYPE>().FirstOrDefault(o => o.SERVICE_ID == item.ID);
                        if (mety != null)
                        {
                            model.ID = mety.ID;
                            model.MEDICINE_TYPE_CODE = mety.MEDICINE_TYPE_CODE;
                            model.MEDICINE_TYPE_NAME = mety.MEDICINE_TYPE_NAME;
                            model.MEDICINE_USE_FORM_ID = mety.MEDICINE_USE_FORM_ID;
                            model.MEDICINE_USE_FORM_CODE = mety.MEDICINE_USE_FORM_CODE;
                            model.MEDICINE_USE_FORM_NAME = mety.MEDICINE_USE_FORM_NAME;
                            model.MANUFACTURER_CODE = mety.MANUFACTURER_CODE;
                            model.MANUFACTURER_ID = mety.MANUFACTURER_ID;
                            model.MANUFACTURER_NAME = mety.MANUFACTURER_NAME;
                            model.ACTIVE_INGR_BHYT_CODE = mety.ACTIVE_INGR_BHYT_CODE;
                            model.ACTIVE_INGR_BHYT_NAME = mety.ACTIVE_INGR_BHYT_NAME;
                            model.IS_AUTO_EXPEND = mety.IS_AUTO_EXPEND;
                            model.HEIN_SERVICE_TYPE_ID = mety.HEIN_SERVICE_TYPE_ID;
                            model.CONVERT_RATIO = mety.CONVERT_RATIO;
                            model.CONVERT_UNIT_CODE = mety.CONVERT_UNIT_CODE;
                            model.CONVERT_UNIT_NAME = mety.CONVERT_UNIT_NAME;
                            model.IsAllowOdd = mety.IS_ALLOW_ODD == 1 ? true : false;

                            var smety = serviceMetys != null && serviceMetys.Count > 0 ? serviceMetys.Where(o => o.MEDICINE_TYPE_ID == model.ID).FirstOrDefault() : null;
                            if (smety != null)
                            {
                                model.AMOUNT = smety.EXPEND_AMOUNT;
                                model.IsNotExpend = smety.IS_NOT_EXPEND;
                                model.AMOUNT_BHYT = smety.AMOUNT_BHYT;
                                model.EXPEND_AMOUNT = smety.EXPEND_AMOUNT;
                                //model.PRICE = smety.EXPEND_PRICE;
                            }

                            //if (mety.CONVERT_RATIO.HasValue)
                            //{
                            //    model.AMOUNT = model.AMOUNT * mety.CONVERT_RATIO.Value;
                            //    model.PRICE = model.PRICE / mety.CONVERT_RATIO.Value;
                            //}
                        }
                    }
                    else if (model.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__VT)
                    {
                        model.MEDICINE_USE_FORM_ID = null;
                        model.MEDICINE_USE_FORM_CODE = "";
                        model.MEDICINE_USE_FORM_NAME = "";
                        model.ErrorTypeMedicineUseForm = ErrorType.None;
                        model.ErrorMessageMedicineUseForm = "";
                        model.DataType = HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.VATTU;
                        var maty = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_MATERIAL_TYPE>().FirstOrDefault(o => o.SERVICE_ID == item.ID);
                        if (maty != null)
                        {
                            model.ID = maty.ID;
                            model.MEDICINE_TYPE_CODE = maty.MATERIAL_TYPE_CODE;
                            model.MEDICINE_TYPE_NAME = maty.MATERIAL_TYPE_NAME;
                            model.MANUFACTURER_CODE = maty.MANUFACTURER_CODE;
                            model.MANUFACTURER_ID = maty.MANUFACTURER_ID;
                            model.MANUFACTURER_NAME = maty.MANUFACTURER_NAME;
                            model.IS_AUTO_EXPEND = maty.IS_AUTO_EXPEND;
                            model.HEIN_SERVICE_TYPE_ID = maty.HEIN_SERVICE_TYPE_ID;
                            model.CONVERT_RATIO = maty.CONVERT_RATIO;
                            model.CONVERT_UNIT_CODE = maty.CONVERT_UNIT_CODE;
                            model.CONVERT_UNIT_NAME = maty.CONVERT_UNIT_NAME;
                            model.IsStent = ((maty.IS_STENT ?? 0) == GlobalVariables.CommonNumberTrue ? true : false);
                            model.IsAllowOdd = maty.IS_ALLOW_ODD == 1 ? true : false;

                            var smaty = serviceMatys != null && serviceMatys.Count > 0 ? serviceMatys.Where(o => o.MATERIAL_TYPE_ID == model.ID).FirstOrDefault() : null;
                            if (smaty != null)
                            {
                                model.AMOUNT = smaty.EXPEND_AMOUNT;
                                model.IsNotExpend = smaty.IS_NOT_EXPEND;
                                model.AMOUNT_BHYT = smaty.AMOUNT_BHYT;
                                model.EXPEND_AMOUNT = smaty.EXPEND_AMOUNT;
                                //model.PRICE = smaty.EXPEND_PRICE;
                            }

                            //if (maty.CONVERT_RATIO.HasValue)
                            //{
                            //    model.AMOUNT = model.AMOUNT * maty.CONVERT_RATIO.Value;
                            //    model.PRICE = model.PRICE / maty.CONVERT_RATIO.Value;
                            //}
                        }
                    }

                    //Lay doi tuong mac dinh
                    MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE patientType = new MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE();
                    patientType = this.ChoosePatientTypeDefaultlServiceOther(this.currentHisPatientTypeAlter.PATIENT_TYPE_ID, model.SERVICE_ID, model.SERVICE_TYPE_ID);
                    if (patientType != null && patientType.ID > 0)
                    {
                        model.PATIENT_TYPE_ID = patientType.ID;
                        model.PATIENT_TYPE_CODE = patientType.PATIENT_TYPE_CODE;
                        model.PATIENT_TYPE_NAME = patientType.PATIENT_TYPE_NAME;
                    }

                    SetDefaultMediStockForData(model);
                    //model.AmountAlert = null;
                    MestMetyUnitWorker.UpdateUnit(model);

                    model.AMOUNT = ((model.IsUseOrginalUnitForPres ?? false) == false && model.CONVERT_RATIO.HasValue) ? (model.AMOUNT * model.CONVERT_RATIO.Value) : model.AMOUNT;
                    //model.PRICE = ((model.IsUseOrginalUnitForPres ?? false) == false && model.CONVERT_RATIO.HasValue) ? ((model.PRICE ?? 0) / model.CONVERT_RATIO.Value) : model.PRICE;

                    if (model.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC)
                    {
                        model.TUTORIAL = String.Format(ResourceMessage._NgayXVienBuoiYZ, (model.MEDICINE_USE_FORM_NAME + " "), "" + (int)model.AMOUNT, model.SERVICE_UNIT_NAME, "");
                    }

                    model.UseDays = 1;
                    model.PrimaryKey = ((model.SERVICE_ID) + "__" + Inventec.Common.DateTime.Get.Now() + "__" + Guid.NewGuid().ToString());
                    //Kiem tra thuoc trong kho, neu so luong trong goi lon hon 
                    //so luong kha dung trong kho thi gan vao so luong canh bao
                    decimal damount = (AmountOutOfStock(model, item.ID, (model.MEDI_STOCK_ID ?? 0)) ?? 0);
                    if (damount < 0 || (model.AMOUNT > damount && (model.AmountAlert == null || model.AmountAlert == 0)))
                    {
                        //model.AMOUNT = damount;
                        model.AmountAlert = damount;
                    }
                    Inventec.Common.Logging.LogSystem.Debug("model.AMOUNT: " + model.AMOUNT + "__" + "damount: " + damount + "__" + "model.AmountAlert: " + model.AmountAlert);
                    model.NUM_ORDER = idRow;

                    //if (
                    //    ((model.IS_AUTO_EXPEND ?? -1) == 1)
                    //    || (HisConfigCFG.IsAutoTickExpendWithAssignPresPTTT && this.isAutoCheckExpend == true
                    //        && (model.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC
                    //            || (model.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__VT
                    //                && model.HEIN_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__VT_TDM
                    //                || model.HEIN_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__VT_NDM))))

                    if ((model.IsNotExpend ?? -1) != 1)
                    {
                        if ((HisConfigCFG.IsAutoTickExpendWithAssignPresPTTT) || (!HisConfigCFG.IsAutoTickExpendWithAssignPresPTTT && (model.IS_AUTO_EXPEND ?? -1) == 1))
                        {
                            model.IsExpend = true;
                        }
                        else
                        {
                            model.IsExpend = false;
                        }

                        this.mediMatyTypeADOs.Add(model);
                    }
                    else
                    {
                        if (this.currentHisPatientTypeAlter.PATIENT_TYPE_ID == HisConfigCFG.PatientTypeId__BHYT && model.AMOUNT_BHYT > 0 && model.EXPEND_AMOUNT > model.AMOUNT_BHYT)
                        {
                            model.AMOUNT = model.AMOUNT_BHYT;

                            if (damount < 0 || (model.AMOUNT > damount && (model.AmountAlert == null || model.AmountAlert == 0)))
                            {
                                model.AmountAlert = damount;
                            }

                            model.PATIENT_TYPE_ID = HisConfigCFG.PatientTypeId__BHYT;
                            model.PATIENT_TYPE_CODE = HisConfigCFG.PatientTypeCode__BHYT;
                            model.PATIENT_TYPE_NAME = HisConfigCFG.PatientTypeName__BHYT;
                            model.IsExpend = false;
                            this.mediMatyTypeADOs.Add(model);

                            MediMatyTypeADO model1 = new MediMatyTypeADO();
                            #region nhân đôi dòng dữ liệu
                            Inventec.Common.Mapper.DataObjectMapper.Map<MediMatyTypeADO>(model1, item);
                            model1.MEDICINE_TYPE_CODE = item.SERVICE_CODE;
                            model1.MEDICINE_TYPE_NAME = item.SERVICE_NAME;
                            model1.SERVICE_ID = item.ID;
                            if (model1.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC)
                            {
                                model1.DataType = HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC;

                                var mety = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_MEDICINE_TYPE>().FirstOrDefault(o => o.SERVICE_ID == item.ID);
                                if (mety != null)
                                {
                                    model1.ID = mety.ID;
                                    model1.MEDICINE_TYPE_CODE = mety.MEDICINE_TYPE_CODE;
                                    model1.MEDICINE_TYPE_NAME = mety.MEDICINE_TYPE_NAME;
                                    model1.MEDICINE_USE_FORM_ID = mety.MEDICINE_USE_FORM_ID;
                                    model1.MEDICINE_USE_FORM_CODE = mety.MEDICINE_USE_FORM_CODE;
                                    model1.MEDICINE_USE_FORM_NAME = mety.MEDICINE_USE_FORM_NAME;
                                    model1.MANUFACTURER_CODE = mety.MANUFACTURER_CODE;
                                    model1.MANUFACTURER_ID = mety.MANUFACTURER_ID;
                                    model1.MANUFACTURER_NAME = mety.MANUFACTURER_NAME;
                                    model1.ACTIVE_INGR_BHYT_CODE = mety.ACTIVE_INGR_BHYT_CODE;
                                    model1.ACTIVE_INGR_BHYT_NAME = mety.ACTIVE_INGR_BHYT_NAME;
                                    model1.IS_AUTO_EXPEND = mety.IS_AUTO_EXPEND;
                                    model1.HEIN_SERVICE_TYPE_ID = mety.HEIN_SERVICE_TYPE_ID;
                                    model1.CONVERT_RATIO = mety.CONVERT_RATIO;
                                    model1.CONVERT_UNIT_CODE = mety.CONVERT_UNIT_CODE;
                                    model1.CONVERT_UNIT_NAME = mety.CONVERT_UNIT_NAME;
                                    model1.IsAllowOdd = mety.IS_ALLOW_ODD == 1 ? true : false;

                                    var smety = serviceMetys != null && serviceMetys.Count > 0 ? serviceMetys.Where(o => o.MEDICINE_TYPE_ID == model1.ID).FirstOrDefault() : null;
                                    if (smety != null)
                                    {
                                        model1.AMOUNT = smety.EXPEND_AMOUNT;
                                        model1.IsNotExpend = smety.IS_NOT_EXPEND;
                                        model1.AMOUNT_BHYT = smety.AMOUNT_BHYT;
                                        model1.EXPEND_AMOUNT = smety.EXPEND_AMOUNT;
                                    }
                                }
                            }
                            else if (model1.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__VT)
                            {
                                model1.MEDICINE_USE_FORM_ID = null;
                                model1.MEDICINE_USE_FORM_CODE = "";
                                model1.MEDICINE_USE_FORM_NAME = "";
                                model1.ErrorTypeMedicineUseForm = ErrorType.None;
                                model1.ErrorMessageMedicineUseForm = "";
                                model1.DataType = HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.VATTU;
                                var maty = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_MATERIAL_TYPE>().FirstOrDefault(o => o.SERVICE_ID == item.ID);
                                if (maty != null)
                                {
                                    model1.ID = maty.ID;
                                    model1.MEDICINE_TYPE_CODE = maty.MATERIAL_TYPE_CODE;
                                    model1.MEDICINE_TYPE_NAME = maty.MATERIAL_TYPE_NAME;
                                    model1.MANUFACTURER_CODE = maty.MANUFACTURER_CODE;
                                    model1.MANUFACTURER_ID = maty.MANUFACTURER_ID;
                                    model1.MANUFACTURER_NAME = maty.MANUFACTURER_NAME;
                                    model1.IS_AUTO_EXPEND = maty.IS_AUTO_EXPEND;
                                    model1.HEIN_SERVICE_TYPE_ID = maty.HEIN_SERVICE_TYPE_ID;
                                    model1.CONVERT_RATIO = maty.CONVERT_RATIO;
                                    model1.CONVERT_UNIT_CODE = maty.CONVERT_UNIT_CODE;
                                    model1.CONVERT_UNIT_NAME = maty.CONVERT_UNIT_NAME;
                                    model1.IsStent = ((maty.IS_STENT ?? 0) == GlobalVariables.CommonNumberTrue ? true : false);
                                    model1.IsAllowOdd = maty.IS_ALLOW_ODD == 1 ? true : false;

                                    var smaty = serviceMatys != null && serviceMatys.Count > 0 ? serviceMatys.Where(o => o.MATERIAL_TYPE_ID == model1.ID).FirstOrDefault() : null;
                                    if (smaty != null)
                                    {
                                        model1.AMOUNT = smaty.EXPEND_AMOUNT;
                                        model1.IsNotExpend = smaty.IS_NOT_EXPEND;
                                        model1.AMOUNT_BHYT = smaty.AMOUNT_BHYT;
                                        model1.EXPEND_AMOUNT = smaty.EXPEND_AMOUNT;
                                    }
                                }
                            }


                            SetDefaultMediStockForData(model1);
                            //model1.AmountAlert = null;
                            MestMetyUnitWorker.UpdateUnit(model1);

                            model1.AMOUNT = ((model1.IsUseOrginalUnitForPres ?? false) == false && model1.CONVERT_RATIO.HasValue) ? (model1.AMOUNT * model1.CONVERT_RATIO.Value) : model1.AMOUNT;

                            if (model1.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC)
                            {
                                model1.TUTORIAL = String.Format(ResourceMessage._NgayXVienBuoiYZ, (model1.MEDICINE_USE_FORM_NAME + " "), "" + (int)model1.AMOUNT, model1.SERVICE_UNIT_NAME, "");
                            }

                            model1.UseDays = 1;
                            model1.PrimaryKey = ((model1.SERVICE_ID) + "__" + Inventec.Common.DateTime.Get.Now() + "__" + Guid.NewGuid().ToString());
                            //Kiem tra thuoc trong kho, neu so luong trong goi lon hon 
                            //so luong kha dung trong kho thi gan vao so luong canh bao
                            decimal damount1 = (AmountOutOfStock(model1, item.ID, (model1.MEDI_STOCK_ID ?? 0)) ?? 0);

                            model1.AMOUNT = model1.EXPEND_AMOUNT - model1.AMOUNT_BHYT;
                            if (damount1 < 0 || (model1.AMOUNT > damount1 && (model1.AmountAlert == null || model1.AmountAlert == 0) && (model.AmountAlert == null || model.AmountAlert == 0)))
                            {
                                model1.AmountAlert = damount1;
                            }

                            Inventec.Common.Logging.LogSystem.Debug("model1.AMOUNT: " + model1.AMOUNT + "__" + "damount1: " + damount1 + "__" + "model1.AmountAlert: " + model1.AmountAlert);


                            model1.PATIENT_TYPE_ID = HisConfigCFG.PatientTypeId__VP;
                            model1.PATIENT_TYPE_CODE = HisConfigCFG.PatientTypeCode__VP;
                            model1.PATIENT_TYPE_NAME = HisConfigCFG.PatientTypeName__VP;
                            model1.IsExpend = false;
                            this.idRow += stepRow;
                            model1.NUM_ORDER = idRow;
                            #endregion

                            this.mediMatyTypeADOs.Add(model1);
                        }
                        else
                        {
                            model.AMOUNT = model.EXPEND_AMOUNT;

                            if (damount < 0 || (model.AMOUNT > damount && (model.AmountAlert == null || model.AmountAlert == 0)))
                            {
                                model.AmountAlert = damount;
                            }
                            model.PATIENT_TYPE_ID = this.currentHisPatientTypeAlter.PATIENT_TYPE_ID;
                            model.PATIENT_TYPE_CODE = this.currentHisPatientTypeAlter.PATIENT_TYPE_CODE;
                            model.PATIENT_TYPE_NAME = this.currentHisPatientTypeAlter.PATIENT_TYPE_NAME;
                            model.IsExpend = false;

                            this.mediMatyTypeADOs.Add(model);
                        }
                    }

                    this.idRow += stepRow;
                }

                Inventec.Common.Logging.LogSystem.Info("this.mediMatyTypeADOs: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => mediMatyTypeADOs), mediMatyTypeADOs));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private async Task FillTreatmentInfo__PatientType()
        {
            try
            {
                //this.lblPatientName.Text = this.currentTreatmentWithPatientType.TDL_PATIENT_NAME;
                //if (this.currentTreatmentWithPatientType.TDL_PATIENT_DOB > 0)
                //    this.lblDob.Text = Inventec.Common.DateTime.Convert.TimeNumberToDateString(this.currentTreatmentWithPatientType.TDL_PATIENT_DOB);
                //this.lblGenderName.Text = this.currentTreatmentWithPatientType.TDL_PATIENT_GENDER_NAME;

                //if (this.currentHisPatientTypeAlter != null)
                //{
                //    this.lblPatientTypeName.Text = this.currentHisPatientTypeAlter.PATIENT_TYPE_NAME;
                //    this.lblTreatmentTypeName.Text = this.currentHisPatientTypeAlter.TREATMENT_TYPE_NAME;
                //}

                //if (!String.IsNullOrEmpty(this.currentHisPatientTypeAlter.HEIN_CARD_NUMBER))
                //    lblHeinCardNumberInfo.Text = String.Format("{0} \r\n({1} - {2})",
                //      Inventec.Desktop.Common.HtmlString.ProcessorString.InsertColor(HeinCardHelper.SetHeinCardNumberDisplayByNumber(this.currentHisPatientTypeAlter.HEIN_CARD_NUMBER), System.Drawing.Color.Green),
                //        Inventec.Common.DateTime.Convert.TimeNumberToDateString(this.currentHisPatientTypeAlter.HEIN_CARD_FROM_TIME ?? 0),
                //        Inventec.Common.DateTime.Convert.TimeNumberToDateString(this.currentHisPatientTypeAlter.HEIN_CARD_TO_TIME ?? 0));
                //else
                //    lblHeinCardNumberInfo.Text = "";
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        //private async Task FillDataToComboPriviousExpMest(HisTreatmentWithPatientTypeInfoSDO treatment)
        //{
        //    try
        //    {
        //        LogSystem.Debug("Begin FillDataToComboPriviousExpMest");
        //        if (this.periousExpMestListProcessor != null && this.ucPeriousExpMestList != null)
        //        {
        //            this.currentPrescriptions = new List<MOS.EFMODEL.DataModels.V_HIS_SERVICE_REQ_7>();
        //            int prescriptionOldLimit = ConfigApplicationWorker.Get<int>(AppConfigKeys.CONFIG_KEY__HIS_DESKTOP__ASSIGN_PRESCRIPTION__OLD_PRECRIPTIONS_DISPLAY_LIMIT);
        //            CommonParam param = new CommonParam();

        //            //TODO
        //            if (prescriptionOldLimit > 0)
        //                param = new CommonParam(0, prescriptionOldLimit);

        //            this.currentPrescriptionFilter.TDL_PATIENT_ID = treatment.PATIENT_ID;
        //            this.currentPrescriptions = await new BackendAdapter(param).GetAsync<List<MOS.EFMODEL.DataModels.V_HIS_SERVICE_REQ_7>>(RequestUriStore.HIS_SERVICE_REQ_GETVIEW_7, ApiConsumers.MosConsumer, this.currentPrescriptionFilter, ProcessLostToken, param);
        //            if (currentPrescriptions != null && currentPrescriptions.Count > 0)
        //            {
        //                List<MOS.EFMODEL.DataModels.V_HIS_SERVICE_REQ_7> prescriptionTemps = new List<MOS.EFMODEL.DataModels.V_HIS_SERVICE_REQ_7>();
        //                var listServiceReq7Group = currentPrescriptions.GroupBy(o => new { o.INTRUCTION_TIME, o.SESSION_CODE, o.REQUEST_LOGINNAME });
        //                foreach (var g in listServiceReq7Group)
        //                {
        //                    MOS.EFMODEL.DataModels.V_HIS_SERVICE_REQ_7 sr = g.First();
        //                    prescriptionTemps.Add(sr);
        //                }
        //                prescriptionTemps = prescriptionTemps.Where(o => o.ID > 0).OrderByDescending(o => o.INTRUCTION_TIME).ToList();
        //                this.periousExpMestListProcessor.Reload(this.ucPeriousExpMestList, prescriptionTemps);
        //            }
        //        }
        //        LogSystem.Debug("End FillDataToComboPriviousExpMest");
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Warn(ex);
        //    }
        //}

        private void FillDataIntoPatientTypeCombo(MediMatyTypeADO data, GridLookUpEdit patientTypeCombo)
        {
            try
            {
                if (patientTypeCombo != null
                    )
                {
                    if (this.currentPatientTypeWithPatientTypeAlter != null && this.currentPatientTypeWithPatientTypeAlter.Count > 0)
                    {
                        GridCheckMarksSelection gridCheckMark = cboMediStockExport.Properties.Tag as GridCheckMarksSelection;
                        List<long> mediStockSelecteds = new List<long>();
                        if (gridCheckMark != null)
                        {
                            foreach (MOS.EFMODEL.DataModels.V_HIS_MEST_ROOM rv in gridCheckMark.Selection)
                            {
                                mediStockSelecteds.Add(rv.MEDI_STOCK_ID);
                            }
                        }
                        else if (cboMediStockExport.EditValue != null)
                        {
                            mediStockSelecteds.Add(Inventec.Common.TypeConvert.Parse.ToInt64(cboMediStockExport.EditValue.ToString()));
                        }

                        if (mediStockSelecteds == null || mediStockSelecteds.Count == 0)
                            throw new Exception("mediStockSelecteds null");

                        List<MOS.EFMODEL.DataModels.HIS_MEST_PATIENT_TYPE> lstMestPatientType = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_MEST_PATIENT_TYPE>();
                        if (lstMestPatientType == null || lstMestPatientType.Count == 0)
                            throw new Exception("Khong load duoc HIS_MEST_PATIENT_TYPE");

                        List<long> mediStockInMestPatientTypeIds = lstMestPatientType.Where(o => mediStockSelecteds.Contains(o.MEDI_STOCK_ID)).Select(o => o.PATIENT_TYPE_ID).ToList();
                        this.InitComboPatientType(patientTypeCombo, this.currentPatientTypeWithPatientTypeAlter);
                    }
                    else
                    {
                        this.InitComboPatientType(patientTypeCombo, null);
                    }
                }
            }
            catch (Exception ex)
            {
                this.InitComboPatientType(patientTypeCombo, null);

                Inventec.Common.Logging.LogSystem.Warn(ex);
                Inventec.Common.Logging.LogSystem.Error(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => this.currentPatientTypeWithPatientTypeAlter), this.currentPatientTypeWithPatientTypeAlter));
            }
        }

        private void FillDataIntoExcuteRoomCombo(MediMatyTypeADO data, DevExpress.XtraEditors.GridLookUpEdit excuteRoomCombo)
        {
            try
            {
                var serviceRoomViews = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_SERVICE_ROOM>();
                var executeRoomViews = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_EXECUTE_ROOM>();
                if (excuteRoomCombo != null && executeRoomViews != null && serviceRoomViews != null && serviceRoomViews.Count > 0)
                {
                    var arrExcuteRoomCode = serviceRoomViews.Where(o => data != null && o.SERVICE_ID == data.SERVICE_ID).Select(o => o.ROOM_ID).ToList();
                    if (arrExcuteRoomCode != null && arrExcuteRoomCode.Count > 0)
                    {
                        List<MOS.EFMODEL.DataModels.V_HIS_EXECUTE_ROOM> dataCombo = executeRoomViews.Where(o => arrExcuteRoomCode.Contains(o.ROOM_ID)).ToList();
                        this.InitComboExecuteRoom(excuteRoomCombo, dataCombo);
                    }
                    else
                    {
                        this.InitComboExecuteRoom(excuteRoomCombo, null);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetDefaultMediStockForData(MediMatyTypeADO model)
        {
            try
            {
                if (this.mediMatyTypeAvailables == null || this.mediMatyTypeAvailables.Count == 0)
                {
                    Inventec.Common.Logging.LogSystem.Debug("SetDefaultMediStockForData. 1 mediMatyTypeAvailables.count= 0");
                    InitDataMetyMatyTypeInStockD(this.currentMediStock);
                    Inventec.Common.Logging.LogSystem.Debug("SetDefaultMediStockForData. 2 mediMatyTypeAvailables.count= 0");
                }

                if (this.mediMatyTypeAvailables != null && this.mediMatyTypeAvailables.Count > 0)
                {
                    bool isTSD = model.DataType == LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.VATTU_TSD;

                    var medicineInStock = this.mediMatyTypeAvailables.Where(o => o.SERVICE_ID == model.SERVICE_ID && ((isTSD && o.IS_REUSABLE == 1) || (!isTSD && (o.IS_REUSABLE == null || o.IS_REUSABLE != 1)))).FirstOrDefault();//Mac dinh lay kho dau tien
                    if (medicineInStock != null)
                    {
                        decimal amountAvailable = ((((model.IsUseOrginalUnitForPres ?? false) == false && model.CONVERT_RATIO.HasValue && model.CONVERT_RATIO > 0) ? medicineInStock.AMOUNT * model.CONVERT_RATIO : medicineInStock.AMOUNT) ?? 0);

                        //Trường hợp thuốc không đủ để xuất, số lượng khả dụng không đủ
                        //Set AmountAlert đánh dấu cảnh báo thuốc đã hết
                        if (amountAvailable < model.AMOUNT)
                            model.AmountAlert = model.AMOUNT;

                        model.MEDI_STOCK_ID = medicineInStock.MEDI_STOCK_ID;
                        model.MEDI_STOCK_CODE = medicineInStock.MEDI_STOCK_CODE;
                        model.MEDI_STOCK_NAME = medicineInStock.MEDI_STOCK_NAME;
                    }
                    //Trường hợp thuốc không còn trong kho, Set AmountAlert đánh dấu cảnh báo thuốc đã hết
                    else
                        model.AmountAlert = model.AMOUNT;
                }
                else
                    model.AmountAlert = model.AMOUNT;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ProcessGetEmteMedcineType(List<V_HIS_EMTE_MEDICINE_TYPE> listEmteMedcineType, bool isChayThan)
        {
            try
            {
                if (listEmteMedcineType != null && listEmteMedcineType.Count > 0)
                {
                    Inventec.Common.Logging.LogSystem.Debug("this.InstructionTime " + this.InstructionTime);

                    //this.intructionTimeSelecteds = this.ucDateProcessor.GetValue(this.ucDate);
                    //this.isMultiDateState = this.ucDateProcessor.GetChkMultiDateState(this.ucDate);
                    this.InstructionTime = intructionTimeSelecteds.OrderByDescending(o => o).First();

                    var q1 = (from m in listEmteMedcineType
                              select new MediMatyTypeADO(m, this.InstructionTime, isChayThan)).ToList();
                    if (q1 != null && q1.Count > 0)
                        this.mediMatyTypeADOs.AddRange(q1);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ProcessGetEmteMaterialType(List<V_HIS_EMTE_MATERIAL_TYPE> listEmteMaterialType, bool isChayThan)
        {
            try
            {
                if (listEmteMaterialType != null && listEmteMaterialType.Count > 0)
                {
                    var q1 = (from m in listEmteMaterialType
                              select new MediMatyTypeADO(m, isChayThan)).ToList();
                    if (q1 != null && q1.Count > 0)
                        this.mediMatyTypeADOs.AddRange(q1);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ProcessGetEquipmentSetMaterial(List<V_HIS_EQUIPMENT_SET_MATY> listEquipmentSetMaty)
        {
            try
            {
                if (listEquipmentSetMaty != null && listEquipmentSetMaty.Count > 0)
                {
                    var q1 = (from m in listEquipmentSetMaty
                              select new MediMatyTypeADO(m)).ToList();
                    if (q1 != null && q1.Count > 0)
                        this.mediMatyTypeADOs.AddRange(q1);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ProcessChoiceExpMestTemplate(MOS.EFMODEL.DataModels.HIS_EXP_MEST_TEMPLATE expTemplate)
        {
            try
            {
                if (expTemplate == null) return;
                if (this.actionType == GlobalVariables.ActionView)
                {
                    LogSystem.Debug("ProcessChoiceExpMestTemplate => thao tac khong hop le. actionType = " + this.actionType);
                    return;
                }

                //Release tat ca cac thuoc/ vat tu da duoc take bean truoc do nhung chua duoc luu
                this.ReleaseAllMediByUser();
                this.ProcessGetEmteMedcineType(this.GetEmteMedicineTypeByExpMestId(expTemplate.ID), false);
                this.ProcessGetEmteMaterialType(this.GetEmteMaterialTypeByExpMestId(expTemplate.ID), false);
                this.ProcessInstructionTimeMediForEdit();
                this.ProcessMergeDuplicateRowForListProcessing();
                this.ProcessAddListRowDataIntoGridWithTakeBean();
                //this.VerifyWarningOverCeiling();
                this.ReloadDataAvaiableMediBeanInCombo();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessChoiceExpMestTemplateForChayThan(MOS.EFMODEL.DataModels.HIS_EXP_MEST_TEMPLATE expTemplate)
        {
            try
            {
                if (expTemplate == null) return;
                if (this.actionType == GlobalVariables.ActionView)
                {
                    LogSystem.Debug("ProcessChoiceExpMestTemplate => thao tac khong hop le. actionType = " + this.actionType);
                    return;
                }

                //Release tat ca cac thuoc/ vat tu da duoc take bean truoc do nhung chua duoc luu
                this.ReleaseAllMediByUser();

                this.ProcessGetEmteMedcineType(this.GetEmteMedicineTypeByExpMestId(expTemplate.ID), true);
                this.ProcessGetEmteMaterialType(this.GetEmteMaterialTypeByExpMestId(expTemplate.ID), true);
                this.ProcessInstructionTimeMediForEdit();
                this.ProcessMergeDuplicateRowForListProcessing();
                this.ProcessAddListRowDataIntoGridWithTakeBean();
                //this.VerifyWarningOverCeiling();
                this.ReloadDataAvaiableMediBeanInCombo();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessChoiceEquipmentSet(MOS.EFMODEL.DataModels.HIS_EQUIPMENT_SET equipmentSet)
        {
            try
            {
                if (equipmentSet == null) return;
                if (this.actionType == GlobalVariables.ActionView)
                {
                    LogSystem.Debug("btnAdd_TabMedicine_Click => thao tac khong hop le. actionType = " + this.actionType);
                    return;
                }

                //Release tat ca cac thuoc/ vat tu da duoc take bean truoc do nhung chua duoc luu
                this.ReleaseAllMediByUser();

                this.ProcessGetEquipmentSetMaterial(this.GetMaterialTypeByEquipmentSetId(equipmentSet.ID));
                this.ProcessInstructionTimeMediForEdit();
                this.ProcessMergeDuplicateRowForListProcessing();
                this.ProcessAddListRowDataIntoGridWithTakeBean();
                //this.VerifyWarningOverCeiling();
                this.ReloadDataAvaiableMediBeanInCombo();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void CreateThreadIsLimitHeinMedicinePrice(object param)
        {
            Thread thread = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(this.IsLimitHeinMedicinePriceNewThread));
            //thread.Priority = ThreadPriority.Highest;
            try
            {
                thread.Start(param);
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                thread.Abort();
            }
        }

        private void IsLimitHeinMedicinePriceNewThread(object param)
        {
            try
            {
                if (this.InvokeRequired)
                {
                    this.Invoke(new MethodInvoker(delegate { this.limitHeinMedicinePrice = this.IsLimitHeinMedicinePrice((long)param); }));
                }
                else
                {
                    this.limitHeinMedicinePrice = this.IsLimitHeinMedicinePrice((long)param);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal bool IsLimitHeinMedicinePrice(long treatmentId)
        {
            bool result = false;
            try
            {
                //WaitingManager.Show();
                LogSystem.Debug("Begining  IsLimitHeinMedicinePrice ...");
                //Lấy giá trị cấu hình trần giới hạn BHYT thanh toán theo đúng tuyến & theo chuyển tuyến
                //Nếu không có cấu hình thì return luôn -> không check giới hạn
                if (HisConfigCFG.LimitHeinMedicinePrice__RightMediOrg == 0 && HisConfigCFG.LimitHeinMedicinePrice__NotRightMediOrg == 0)
                    return result;

                if (this.currentHisPatientTypeAlter == null)
                    throw new NullReferenceException("IsLimitHeinMedicinePrice: currentHisPatientTypeAlter is null");

                if (this.currentHisPatientTypeAlter.PATIENT_TYPE_ID == HisConfigCFG.PatientTypeId__BHYT)
                {
                    //Trường hợp người dùng chọn thời gian chỉ định khác ngày hiện tại
                    //Tự get dữ liệu thuốc thanh toán BHYT về để tổng hợp và kiểm tra
                    if (this.currentHisPatientTypeAlter.ID > 0)
                    {
                        result = this.IsLimitHein(treatmentId);
                    }
                    //Trường hợp intructiontime la ngay hien tai, Truy vấn tổng số tiền thuốc BHYT đã kê trong hồ sơ điều trị
                    //Lấy thông tin từ d_his_sere_serv_1
                    else
                    {
                        result = this.IsLimitHeinByDView(treatmentId);
                    }
                }
                //WaitingManager.Hide();
                LogSystem.Debug("Ending  IsLimitHeinMedicinePrice ...");
            }
            catch (NullReferenceException ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                result = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                result = false;
            }
            return result;
        }

        private bool IsLimitHein(long treatmentId)
        {
            bool result = false;
            try
            {
                CommonParam param = new CommonParam();
                if (this.currentHisPatientTypeAlter.TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__KHAM
                   && this.currentHisPatientTypeAlter.PATIENT_TYPE_ID == HisConfigCFG.PatientTypeId__BHYT)
                {
                    HisPatientTypeAlterViewFilter patyAlterBhytFilter = new HisPatientTypeAlterViewFilter();
                    patyAlterBhytFilter.ID = this.currentHisPatientTypeAlter.ID;
                    this.currentHisPatientTypeAlter = new BackendAdapter(param).Get<List<V_HIS_PATIENT_TYPE_ALTER>>(RequestUriStore.HIS_PATIENT_TYPE_ALTER__GETVIEW, ApiConsumers.MosConsumer, patyAlterBhytFilter, ProcessLostToken, param).SingleOrDefault();

                    if (this.currentHisPatientTypeAlter != null)
                    {
                        //Lấy tổng tiển BHYT đã sử dụng
                        HisSereServView7Filter sereServFilter = new HisSereServView7Filter();
                        sereServFilter.TDL_TREATMENT_ID = treatmentId;
                        sereServFilter.SERVICE_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC;
                        sereServFilter.PATIENT_TYPE_ID = HisConfigCFG.PatientTypeId__BHYT;
                        var sereServTotalHeinPriceWithTreatments = new BackendAdapter(param).Get<List<V_HIS_SERE_SERV_7>>(RequestUriStore.HIS_SERE_SERV_GETVIEW_7, ApiConsumers.MosConsumer, sereServFilter, ProcessLostToken, param);

                        //Nếu sửa đơn thuốc thì lấy tổng tiền thuốc bảo hiểm hồ sơ trừ đi đơn đang sửa
                        if (this.assignPrescriptionEditADO != null && this.assignPrescriptionEditADO.ServiceReq != null && this.assignPrescriptionEditADO.ServiceReq.ID > 0)
                        {
                            this.totalPriceBHYT = sereServTotalHeinPriceWithTreatments.Where(o => o.SERVICE_REQ_ID != this.assignPrescriptionEditADO.ServiceReq.ID).Sum(o => o.VIR_TOTAL_PRICE ?? 0);
                        }
                        //Ngược lại lấy tất cả tổng tiền thuốc bảo hiểm trong hồ sơ
                        else
                        {
                            this.totalPriceBHYT = sereServTotalHeinPriceWithTreatments.Sum(o => o.VIR_TOTAL_PRICE ?? 0);
                        }

                        decimal limitHeinMedicinePrice__RightMediOrg = HisConfigCFG.LimitHeinMedicinePrice__RightMediOrg;
                        decimal limitHeinMedicinePrice__NotRightMediOrg = HisConfigCFG.LimitHeinMedicinePrice__NotRightMediOrg;

                        //Nếu có cấu hình trần trong hồ sơ điều trị thì lấy trần từ hsdt ra
                        //if ((this.currentHisTreatment.LimitHeinMedicinePrice__RightMediOrg ?? 0) > 0)
                        //{
                        //    limitHeinMedicinePrice__RightMediOrg = this.currentHisTreatment.LimitHeinMedicinePrice__RightMediOrg.Value;
                        //}
                        //if ((this.currentHisTreatment.LimitHeinMedicinePrice__TranPati ?? 0) > 0)
                        //{
                        //    limitHeinMedicinePrice__TranPati = this.currentHisTreatment.LimitHeinMedicinePrice__TranPati.Value;
                        //}

                        if (limitHeinMedicinePrice__RightMediOrg > 0
                            && this.currentHisPatientTypeAlter.HEIN_MEDI_ORG_CODE == HisMediOrgCFG.MEDI_ORG_VALUE__CURRENT)
                        {
                            result = (this.totalPriceBHYT > limitHeinMedicinePrice__RightMediOrg);
                        }

                        //Đối với bệnh nhân chuyển tuyến
                        if (limitHeinMedicinePrice__NotRightMediOrg > 0
                            && this.currentHisPatientTypeAlter.RIGHT_ROUTE_CODE == MOS.LibraryHein.Bhyt.HeinRightRoute.HeinRightRouteCode.TRUE
                                && this.currentHisPatientTypeAlter.HEIN_MEDI_ORG_CODE != HisMediOrgCFG.MEDI_ORG_VALUE__CURRENT
                            )
                        {
                            result = (this.totalPriceBHYT > limitHeinMedicinePrice__NotRightMediOrg);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                result = false;
            }
            return result;
        }

        private bool IsLimitHeinByDView(long treatmentId)
        {
            bool result = false;
            try
            {
                if (this.totalPriceBHYT > 0)
                {
                    if (this.currentTreatmentWithPatientType != null
                        && this.currentTreatmentWithPatientType.IS_NOT_CHECK_LHMP.HasValue
                        && this.currentTreatmentWithPatientType.IS_NOT_CHECK_LHMP == GlobalVariables.CommonNumberTrue)
                    {
                        return false;
                    }

                    decimal limitHeinMedicinePrice__RightMediOrg = HisConfigCFG.LimitHeinMedicinePrice__RightMediOrg;
                    decimal limitHeinMedicinePrice__NotRightMediOrg = HisConfigCFG.LimitHeinMedicinePrice__NotRightMediOrg;

                    //Nếu có cấu hình trần trong hồ sơ điều trị thì lấy trần từ hsdt ra
                    //if ((this.currentHisTreatment.LimitHeinMedicinePrice__RightMediOrg ?? 0) > 0)
                    //{
                    //    limitHeinMedicinePrice__RightMediOrg = this.currentHisTreatment.LimitHeinMedicinePrice__RightMediOrg.Value;
                    //}
                    //if ((this.currentHisTreatment.LimitHeinMedicinePrice__TranPati ?? 0) > 0)
                    //{
                    //    limitHeinMedicinePrice__TranPati = this.currentHisTreatment.LimitHeinMedicinePrice__TranPati.Value;
                    //}

                    if (limitHeinMedicinePrice__RightMediOrg > 0
                          && this.currentHisPatientTypeAlter.RIGHT_ROUTE_CODE == MOS.LibraryHein.Bhyt.HeinRightRoute.HeinRightRouteCode.TRUE
                                && this.currentHisPatientTypeAlter.HEIN_MEDI_ORG_CODE != HisMediOrgCFG.MEDI_ORG_VALUE__CURRENT)
                    {
                        result = (this.totalPriceBHYT > limitHeinMedicinePrice__RightMediOrg);
                    }

                    //Đối với bệnh nhân chuyển tuyến
                    if (limitHeinMedicinePrice__NotRightMediOrg > 0

                        && this.currentHisPatientTypeAlter.RIGHT_ROUTE_TYPE_CODE == MOS.LibraryHein.Bhyt.HeinRightRouteType.HeinRightRouteTypeCode.PRESENT)
                    {
                        result = (totalPriceBHYT > limitHeinMedicinePrice__NotRightMediOrg);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                result = false;
            }
            return result;
        }

        private void ProcessGetExpMestMetyReq(List<HIS_EXP_MEST_METY_REQ> lstExpMestMetyReq, bool isEdit)
        {
            try
            {
                if (lstExpMestMetyReq != null && lstExpMestMetyReq.Count > 0)
                {
                    var q1 = (from m in lstExpMestMetyReq
                              select new MediMatyTypeADO(m, isEdit)).ToList();
                    if (q1 != null && q1.Count > 0)
                        this.mediMatyTypeADOs.AddRange(q1);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessGetExpMestMatyReq(List<HIS_EXP_MEST_MATY_REQ> lstExpMestMatyReq, bool isEdit)
        {
            try
            {
                if (lstExpMestMatyReq != null && lstExpMestMatyReq.Count > 0)
                {
                    var q1 = (from m in lstExpMestMatyReq
                              select new MediMatyTypeADO(m, isEdit)).ToList();
                    if (q1 != null && q1.Count > 0)
                        this.mediMatyTypeADOs.AddRange(q1);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        //Thuốc trong kho
        private void ProcessGetExpMestMedicine(List<V_HIS_EXP_MEST_MEDICINE> lstExpMestMedicine, bool isEdit)
        {
            try
            {
                //V_HIS_MEDICINE_BEAN>
                if (lstExpMestMedicine != null && lstExpMestMedicine.Count > 0)
                {
                    List<V_HIS_MEDICINE_BEAN_1> medicineBeans = null;
                    if (isEdit)
                    {
                        medicineBeans = GetMedicineBeanByExpMestMedicine(lstExpMestMedicine.Select(o => o.ID).ToList());
                    }
                    var q1 = (from m in lstExpMestMedicine
                              select new MediMatyTypeADO(m, medicineBeans, isEdit)).ToList();
                    if (q1 != null && q1.Count > 0)
                        this.mediMatyTypeADOs.AddRange(q1);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Đơn cũ
        /// </summary>
        /// <param name="lstExpMestMedicine"></param>
        /// <param name="serviceReq"></param>
        private void ProcessGetExpMestMedicine_Prescription(List<V_HIS_EXP_MEST_MEDICINE> lstExpMestMedicine, V_HIS_SERVICE_REQ_7 serviceReq, long currentInstructionTime)
        {
            try
            {

                //V_HIS_MEDICINE_BEAN>
                if (lstExpMestMedicine != null && lstExpMestMedicine.Count > 0)
                {
                    List<MediMatyTypeADO> temps = new List<MediMatyTypeADO>();
                    List<HIS_MEDICINE_BEAN> medicineBeans = null;
                    var q1 = (from m in lstExpMestMedicine
                              select new MediMatyTypeADO(m, currentInstructionTime, medicineBeans, serviceReq)).ToList();
                    //Check trong kho
                    this.ProcessDataMediStock(q1);

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessDataMediStock(List<MediMatyTypeADO> q)
        {
            List<MediMatyTypeADO> result = new List<MediMatyTypeADO>();
            try
            {
                if (q != null && q.Count > 0)
                {
                    foreach (var item in q)
                    {
                        item.AmountAlert = null;
                        if (item.MEDI_STOCK_ID == null || item.MEDI_STOCK_ID == 0)
                        {
                            item.MEDI_STOCK_ID = null;
                            item.MEDI_STOCK_CODE = null;
                            item.MEDI_STOCK_NAME = null;
                        }
                        MediMatyTypeADO ado = new MediMatyTypeADO(item);
                        if (this.mediMatyTypeAvailables != null && this.mediMatyTypeAvailables.Count > 0)
                        {
                            var dMediStock1ADOs = this.mediMatyTypeAvailables.Where(o => o.ID == item.ID && o.MEDI_STOCK_ID == item.MEDI_STOCK_ID).ToList();
                            dMediStock1ADOs = (dMediStock1ADOs == null || dMediStock1ADOs.Count == 0) ? this.mediMatyTypeAvailables.Where(o => o.ID == item.ID).OrderByDescending(o => o.AMOUNT).ToList() : dMediStock1ADOs;
                            decimal amountAvailable = (dMediStock1ADOs != null && dMediStock1ADOs.Count > 0) ? ((((item.IsUseOrginalUnitForPres ?? false) == false && item.CONVERT_RATIO.HasValue && item.CONVERT_RATIO > 0) ? dMediStock1ADOs[0].AMOUNT * item.CONVERT_RATIO : dMediStock1ADOs[0].AMOUNT) ?? 0) : 0;
                            if (dMediStock1ADOs == null || dMediStock1ADOs.Count == 0 || item.AMOUNT > amountAvailable)
                            {
                                Inventec.Common.Logging.LogSystem.Debug("Thuoc/vt khong con trong kho hoac ke vuot qua so luong kha dung____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => item.MEDICINE_TYPE_NAME), item.MEDICINE_TYPE_NAME) + "___" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => item.MEDI_STOCK_NAME), item.MEDI_STOCK_NAME) + "___" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => item.AMOUNT), item.AMOUNT) + "___" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => amountAvailable), amountAvailable));
                                ado.AmountAlert = item.AMOUNT;
                                result.Add(ado);
                                continue;
                            }

                            if (item.AMOUNT > 0)
                            {
                                if (amountAvailable < item.AMOUNT)
                                {
                                    Inventec.Common.Logging.LogSystem.Debug("Thuoc/vt ke vuot qua so luong kha dung____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => item.MEDICINE_TYPE_NAME), item.MEDICINE_TYPE_NAME) + "___" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => item.MEDI_STOCK_NAME), item.MEDI_STOCK_NAME) + "___" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => item.AMOUNT), item.AMOUNT) + "___" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => amountAvailable), amountAvailable));
                                    ado.AMOUNT = amountAvailable;
                                    item.AMOUNT = item.AMOUNT - amountAvailable;
                                }
                                else
                                {
                                    ado.AMOUNT = item.AMOUNT;
                                    item.AMOUNT = 0;
                                }
                                ado.MEDI_STOCK_ID = dMediStock1ADOs[0].MEDI_STOCK_ID;
                                ado.MEDI_STOCK_CODE = dMediStock1ADOs[0].MEDI_STOCK_CODE;
                                ado.MEDI_STOCK_NAME = dMediStock1ADOs[0].MEDI_STOCK_NAME;
                                result.Add(ado);
                            }
                        }
                        else
                        {
                            if (currentMediStock == null || currentMediStock.Count == 0)
                            {
                                Inventec.Common.Logging.LogSystem.Debug("Thuoc/vt ke khong con kha dung trong kho hoac chua chon kho nen chua co danh sach thuoc trong kho de check____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => item.MEDICINE_TYPE_NAME), item.MEDICINE_TYPE_NAME) + "___" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => item.MEDI_STOCK_NAME), item.MEDI_STOCK_NAME) + "___" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => item.AMOUNT), item.AMOUNT));
                                ado.AmountAlert = item.AMOUNT;
                            }
                            result.Add(ado);
                        }
                    }
                    this.mediMatyTypeADOs.AddRange(result);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        //Vật tư trong kho

        private void ProcessGetExpMestMaterial(List<V_HIS_EXP_MEST_MATERIAL> lstExpMestMaterial, bool isEdit)
        {
            try
            {
                if (lstExpMestMaterial != null && lstExpMestMaterial.Count > 0)
                {
                    List<V_HIS_MATERIAL_BEAN_1> materialBeans = null;
                    if (isEdit)
                    {
                        materialBeans = GetMaterialBeanByExpMestMedicine1(lstExpMestMaterial.Select(o => o.ID).ToList());
                    }

                    var q1 = (from m in lstExpMestMaterial
                              select new MediMatyTypeADO(m, materialBeans, isEdit)).ToList();
                    if (q1 != null && q1.Count > 0)
                        this.mediMatyTypeADOs.AddRange(q1);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessGetExpMestMaterial_Prescription(List<V_HIS_EXP_MEST_MATERIAL> lstExpMestMaterial, bool isEdit)
        {
            try
            {
                if (lstExpMestMaterial != null && lstExpMestMaterial.Count > 0)
                {
                    List<HIS_MATERIAL_BEAN> materialBeans = null;
                    if (isEdit)
                    {
                        materialBeans = GetMaterialBeanByExpMestMedicine(lstExpMestMaterial.Select(o => o.ID).ToList());
                    }

                    var q1 = (from m in lstExpMestMaterial
                              select new MediMatyTypeADO(m, materialBeans, isEdit)).ToList();

                    this.ProcessDataMediStock(q1);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        //Thuoc trong danh muc
        private void ProcessGetServiceReqMety(List<HIS_SERVICE_REQ_METY> lstExpMestMety, bool isEdit)
        {
            try
            {
                if (lstExpMestMety != null)
                {
                    var q1 = (from m in lstExpMestMety
                              select new MediMatyTypeADO(m, isEdit)).ToList();
                    if (q1 != null && q1.Count > 0)
                        this.mediMatyTypeADOs.AddRange(q1);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Đơn cũ
        /// </summary>
        /// <param name="lstExpMestMety"></param>
        /// <param name="serviceReq"></param>
        /// <param name="currentInstructionTime"></param>
        private void ProcessGetServiceReqMety(List<HIS_SERVICE_REQ_METY> lstExpMestMety, V_HIS_SERVICE_REQ_7 serviceReq, long currentInstructionTime)
        {
            try
            {
                if (lstExpMestMety != null)
                {
                    var q1 = (from m in lstExpMestMety
                              select new MediMatyTypeADO(m, currentInstructionTime, serviceReq)).ToList();
                    if (q1 != null && q1.Count > 0)
                        this.mediMatyTypeADOs.AddRange(q1);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        //vat tu trong danh muc
        private void ProcessGetServiceReqMaty(List<HIS_SERVICE_REQ_MATY> lstExpMestMaty, bool isEdit)
        {
            try
            {
                if (lstExpMestMaty != null)
                {
                    var q1 = (from m in lstExpMestMaty
                              select new MediMatyTypeADO(m, isEdit)).ToList();
                    if (q1 != null && q1.Count > 0)
                        this.mediMatyTypeADOs.AddRange(q1);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ViewPrescriptionPreviousButtonClick(MOS.EFMODEL.DataModels.V_HIS_SERVICE_REQ_7 serviceReq)
        {
            try
            {
                if (serviceReq != null)
                {
                    Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.ServiceReqSessionDetail").FirstOrDefault();
                    if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.ServiceReqSessionDetail");
                    if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                    {
                        List<object> listArgs = new List<object>();

                        if (!String.IsNullOrEmpty(serviceReq.SESSION_CODE))
                        {
                            ServiceReqSessionDetailADO ado = new ServiceReqSessionDetailADO(serviceReq.SESSION_CODE, serviceReq.INTRUCTION_TIME);
                            listArgs.Add(ado);
                        }
                        else
                        {
                            ServiceReqSessionDetailADO ado = new ServiceReqSessionDetailADO(serviceReq.ID);
                            listArgs.Add(ado);
                        }
                        var extenceInstance = PluginInstance.GetPluginInstance(HIS.Desktop.Utility.PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId), listArgs);
                        if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");
                        ((Form)extenceInstance).Show();
                    }
                }
            }
            catch (NullReferenceException ex)
            {
                WaitingManager.Hide();
                DevExpress.XtraEditors.XtraMessageBox.Show(LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBKhongTimThayPluginsCuaChucNangNay), Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageBoxButtons.OK, MessageBoxIcon.Warning, DevExpress.Utils.DefaultBoolean.True);
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void RefeshDataMedicineTutorial(object data)
        {
            try
            {
                if (data != null && data is HIS_MEDICINE_TYPE_TUT)
                {
                    BackendDataWorker.Reset<MOS.EFMODEL.DataModels.HIS_MEDICINE_TYPE_TUT>();
                    BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_MEDICINE_TYPE_TUT>();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private long GetSereServInKip()
        {
            long result = 0;
            try
            {
                if (this.currentSereServ != null)
                    result = this.currentSereServ.ID;

                if (this.currentSereServInEkip != null)
                    result = this.currentSereServInEkip.ID;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        internal long GetRoomId()
        {
            long roomId = 0;
            try
            {
                if (this.assignPrescriptionEditADO != null && this.assignPrescriptionEditADO.ServiceReq != null)
                {
                    roomId = this.assignPrescriptionEditADO.ServiceReq.REQUEST_ROOM_ID;
                }
                else
                {
                    roomId = (this.currentModule != null ? this.currentModule.RoomId : 0);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return roomId;
        }

        internal long GetRoomTypeId()
        {
            long roomTypeId = 0;
            try
            {
                if (this.assignPrescriptionEditADO != null && this.assignPrescriptionEditADO.ServiceReq != null)
                {
                    roomTypeId = (BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == this.assignPrescriptionEditADO.ServiceReq.REQUEST_ROOM_ID) ?? new V_HIS_ROOM()).ROOM_TYPE_ID;
                }
                else
                {
                    roomTypeId = (this.currentModule != null ? this.currentModule.RoomTypeId : 0);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return roomTypeId;
        }

        private void RefeshExpMestTemplate()
        {
            try
            {
                BackendDataWorker.Reset<MOS.EFMODEL.DataModels.HIS_EXP_MEST_TEMPLATE>();
                this.InitComboExpMestTemplate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FocusMedicineUseForm()
        {
            try
            {
                this.spinAmount.Focus();
                this.spinAmount.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal void SetTotalPrice__TrongDon()
        {
            try
            {
                List<MediMatyTypeADO> medicineTypeADOs = (List<MediMatyTypeADO>)this.gridControlServiceProcess.DataSource;
                decimal totalPrice = 0;
                if (medicineTypeADOs != null && medicineTypeADOs.Count > 0)
                {
                    foreach (var item in medicineTypeADOs)
                    {
                        totalPrice += item.TotalPrice;
                    }
                }
                //if (this.actionType == GlobalVariables.ActionEdit && this.totalHeinByTreatment > 0)
                //    this.totalHeinByTreatment = this.totalHeinByTreatment - totalPrice;
                this.lblTongTien.Text = Inventec.Common.Number.Convert.NumberToStringRoundAuto(totalPrice, ConfigApplications.NumberSeperator);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ProcessDataTextLib(MOS.EFMODEL.DataModels.HIS_TEXT_LIB textLib)
        {
            try
            {
                //this.txtAdvise.Text = HIS.Desktop.Utility.TextLibHelper.BytesToString(textLib.CONTENT);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void CalculateAmount()
        {
            try
            {
                //long roomTypeId = GetRoomTypeId();
                //double tongCong = 1;

                //if (currentMedicineTypeADOForEdit != null && currentMedicineTypeADOForEdit.ID > 0)
                //{
                //    bool isNotOdd = false;
                //    if ((GlobalStore.IsTreatmentIn || GlobalStore.IsCabinet) && ((currentMedicineTypeADOForEdit.IsAllowOdd.HasValue && currentMedicineTypeADOForEdit.IsAllowOdd.Value == true) || (currentMedicineTypeADOForEdit.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__VT)))
                //    {

                //    }
                //    else
                //    {
                //        isNotOdd = true;
                //    }

                //    if (isNotOdd)
                //    {
                //        int plusSeperate = 1;
                //        double amount = (tongCong);

                //        this.spinAmount.Text = (Inventec.Common.Number.Convert.RoundUpValue(amount, 0) != (int)(amount) ? (int)(amount) + plusSeperate : amount).ToString();
                //        if (amount != (int)(amount))
                //        {
                //            Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => amount), amount) + "____" + Inventec.Common.Logging.LogUtil.TraceData("(int)amount", (int)(amount)));
                //        }
                //    }
                //    else
                //    {
                //        this.spinAmount.Text = ((double)(tongCong)) + "";
                //    }
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetHuongDanFromSoLuongNgay()
        {
            try
            {
                //var mediMate = this.mediMatyTypeADOs != null ? this.mediMatyTypeADOs.FirstOrDefault(o => !String.IsNullOrEmpty(o.TUTORIAL)) : null;
                string serviceUnitName = "";
                bool isUse = false;
                if (this.currentMedicineTypeADOForEdit != null && this.currentMedicineTypeADOForEdit.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC)
                {
                    isUse = true;
                    serviceUnitName = !String.IsNullOrEmpty(this.currentMedicineTypeADOForEdit.CONVERT_UNIT_NAME) ? this.currentMedicineTypeADOForEdit.CONVERT_UNIT_NAME : (this.currentMedicineTypeADOForEdit.SERVICE_UNIT_NAME ?? "").ToLower();
                }
                if (isUse)
                {
                    StringBuilder huongDan = new StringBuilder();
                    string format__NgayUong = ResourceMessage.NgayUong;
                    string format__NgayUongTemp2 = ResourceMessage.NgayUongTemp2;
                    string format__NgayUongTemp3 = ResourceMessage.NgayUongTemp3;
                    string format__NgayUongTemp4 = ResourceMessage.NgayUongTemp4;
                    string format__NgayUongTemp5 = ResourceMessage.NgayUongTemp5;
                    string format___NgayXVienBuoiYZ = ResourceMessage._NgayXVienBuoiYZ;
                    string format__Sang = ResourceMessage.Sang;
                    string format__Trua = ResourceMessage.Trua;
                    string format__Chieu = ResourceMessage.Chieu;
                    string format__Toi = ResourceMessage.Toi;
                    double soLuongTrenlanMin = 0;
                    string strSeperator = ", ";
                    int solan = 1;
                    string buoiChon = "";

                    double tongCong = (double)spinAmount.Value;

                    if (tongCong == 0)
                    {
                        huongDan = new StringBuilder();
                    }
                    else
                    {
                        if (HisConfigCFG.TutorialFormat == 1)
                        {
                            huongDan.AppendFormat("Ngày uống {0} {1}/ {2} lần : {3} {4}", (String.IsNullOrEmpty(this.cboMedicineUseForm.Text) ? "" : this.cboMedicineUseForm.Text), tongCong, serviceUnitName, solan, " ");
                        }
                        else if (HisConfigCFG.TutorialFormat == 2)
                        {
                            huongDan.AppendFormat("Ngày uống {0} lần : {1} {2}", (String.IsNullOrEmpty(this.cboMedicineUseForm.Text) ? "" : this.cboMedicineUseForm.Text), tongCong, serviceUnitName, solan, " ");
                        }
                        else if (HisConfigCFG.TutorialFormat == 3)
                        {
                            double moiLan = tongCong / solan;
                            huongDan.AppendFormat("{0} {1}/lần * {2} lần/ngày", (String.IsNullOrEmpty(this.cboMedicineUseForm.Text) ? "" : this.cboMedicineUseForm.Text), moiLan, serviceUnitName, solan, " ");
                        }
                        if (HisConfigCFG.TutorialFormat == 4)
                        {
                            huongDan.AppendFormat("{0} ngày. Ngày uống {1} {2}/ {3} lần", (String.IsNullOrEmpty(this.cboMedicineUseForm.Text) ? "" : this.cboMedicineUseForm.Text), tongCong, serviceUnitName, solan, " ");
                        }
                        if (HisConfigCFG.TutorialFormat == 5)
                        {
                            huongDan = new StringBuilder();
                            if (!String.IsNullOrEmpty(this.cboMedicineUseForm.Text))
                            {
                                huongDan.AppendFormat("{0}, ", this.cboMedicineUseForm.Text.ToLower());
                            }
                            if (!String.IsNullOrEmpty(this.txtHtu.Text))
                            {
                                huongDan.AppendFormat("{0}, ", this.txtHtu.Text.ToLower());
                            }
                            if (soLuongTrenlanMin > 0)
                            {
                                huongDan.AppendFormat("{0} {1}", ConvertNumber.ConvertDecToFracByConfig(soLuongTrenlanMin, 4), serviceUnitName);
                            }
                            string result = huongDan.ToString().TrimEnd(',', ' ');
                            this.txtTutorial.Text = result;
                            //huongDan.Append(soLuongTrenlanMin > 0 ? String.Format(format__NgayUongTemp5, (String.IsNullOrEmpty(this.cboMedicineUseForm.Text) ? "" : " " + this.cboMedicineUseForm.Text.ToLower() + " "), !String.IsNullOrEmpty(this.txtHtu.Text) ? string.Join(", ", DataHtuList.Where(o => o.IsChecked).OrderByDescending(o => o.CHECK_ACIN_INTERACTIVE ?? Int64.MaxValue).ThenBy(o => o.NUM_ORDER).Select(o => o.HTU_NAME).ToList()).ToLower() : "") : "");
                        }
                        if (HisConfigCFG.TutorialFormat == 6)
                        {
                            huongDan = new StringBuilder();
                            double tongSoLuong = (double)spinAmount.Value;
                            int soLanTrongNgay = solan > 0 ? solan : 1;
                            if (tongSoLuong > 0)
                            {
                                double moiLan = tongSoLuong / soLanTrongNgay;
                                string strMoiLan = Inventec.Common.Number.Convert.NumberToStringRoundAuto((decimal)moiLan, 2);
                                string strTong = Inventec.Common.Number.Convert.NumberToStringRoundAuto((decimal)tongSoLuong, 2);

                                huongDan.AppendFormat("{0} {1}/lần * {2} lần/ngày * 1 ngày", strMoiLan, serviceUnitName, soLanTrongNgay);
                                huongDan.AppendFormat(" [{0} {1}/ngày]", strTong, serviceUnitName);
                            }
                            //if ((int)tongCong == tongCong)
                            //    huongDan.Append(!String.IsNullOrEmpty(this.spinAmount.Text) ? string.Format(" [{0} {1}/ngày]", (int)tongCong, serviceUnitName) : "");
                            //else
                            //    huongDan.Append(!String.IsNullOrEmpty(this.spinAmount.Text) ? String.Format(" [{0} {1}/ngày]", ConvertNumber.ConvertDecToFracByConfig(tongCong, 4), serviceUnitName) : "");
                        }
                        this.txtTutorial.Text = string.IsNullOrEmpty(huongDan.ToString()) ? this.txtTutorial.Text : huongDan.ToString().Replace("  ", " ").Replace(", ,", ",");
                    }
                    //else
                    //{
                    //    if ((int)tongCong == tongCong)
                    //        huongDan.Append(!String.IsNullOrEmpty(this.spinAmount.Text) ? String.Format(format___NgayXVienBuoiYZ, (String.IsNullOrEmpty(this.cboMedicineUseForm.Text) ? "" : this.cboMedicineUseForm.Text + " "), "" + (int)tongCong, serviceUnitName, buoiChon) : "");
                    //    else
                    //        huongDan.Append(!String.IsNullOrEmpty(this.spinAmount.Text) ? String.Format(format___NgayXVienBuoiYZ, (String.IsNullOrEmpty(this.cboMedicineUseForm.Text) ? "" : this.cboMedicineUseForm.Text + ""), this.ConvertDecToFracByConfig(tongCong), serviceUnitName, buoiChon) : "");

                    //    string hdTemp = huongDan.ToString().Replace("  ", " ").Replace(", ,", ",");
                    //    huongDan = new StringBuilder().Append(hdTemp.First().ToString().ToUpper() + String.Join("", hdTemp.Skip(1)).ToLower());
                    //}
                    //this.txtTutorial.Text = huongDan.ToString();                    
                }
                else
                {
                    this.txtTutorial.Text = "";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private string ConvertDecToFracByConfig(double dec)
        {
            string result = "";
            try
            {
                if (HisConfigCFG.IsTutorialNumberIsFrac)
                {
                    result = ConvertNumber.Dec2frac(dec);
                }
                else
                {
                    result = dec.ToString();
                }
            }
            catch (Exception ex)
            {
                result = "";
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private void SpinValidating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                decimal amountInput = 0;
                string vl = (sender as DevExpress.XtraEditors.TextEdit).Text;
                try
                {
                    if (vl.Contains("/"))
                    {
                        vl = vl.Replace(".", System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator);
                        vl = vl.Replace(",", System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator);
                        var arrNumber = vl.Split('/');
                        if (arrNumber != null && arrNumber.Count() > 1)
                        {
                            amountInput = Convert.ToDecimal(arrNumber[0]) / Convert.ToDecimal(arrNumber[1]);
                        }
                    }
                }
                catch (Exception ex)
                {
                    amountInput = 0;
                    e.Cancel = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        string FirstCharToUpper(string str)
        {
            string result = "";
            try
            {
                result = !String.IsNullOrEmpty(str) ? (str.First().ToString().ToUpper() + String.Join("", str.Skip(1)).ToLower()) : "";
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }
        private void SpinKeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) &&
        (e.KeyChar != '.') && (e.KeyChar != ',') && (e.KeyChar != '/'))
                {
                    e.Handled = true;
                }

                // only allow one decimal point                
                if ((e.KeyChar == '/') && ((sender as TextEdit).Text.IndexOf('/') > -1))
                {
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal double GetValueSpin(string strValue)
        {
            double value = 0;
            try
            {
                if (!String.IsNullOrEmpty(strValue))
                {
                    string vl = strValue;
                    vl = vl.Replace(".", System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator);
                    vl = vl.Replace(",", System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator);
                    if (vl.Contains("/"))
                    {
                        var arrNumber = vl.Split('/');
                        if (arrNumber != null && arrNumber.Count() > 1)
                        {
                            value = Convert.ToDouble(arrNumber[0]) / Convert.ToDouble(arrNumber[1]);
                        }
                    }
                    else
                    {
                        value = Convert.ToDouble(vl);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return value;
        }

    }
}

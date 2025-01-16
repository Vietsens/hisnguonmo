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
using AutoMapper;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.DXErrorProvider;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.HisConfig;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.AssignPrescriptionYHCT.ADO;
using HIS.Desktop.Plugins.AssignPrescriptionYHCT.Config;
using HIS.Desktop.Plugins.AssignPrescriptionYHCT.Resources;
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

namespace HIS.Desktop.Plugins.AssignPrescriptionYHCT.AssignPrescription
{
    public partial class frmAssignPrescription : HIS.Desktop.Utility.FormBase
    {
        private void ProcessAfterChangeTrackingTime(HIS_TRACKING tracking)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("ProcessAfterChangeTrackingTime.1__" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => tracking), tracking));
                //Trường hợp tạo/sửa tờ điều trị trước khi lưu đơn
                UC.DateEditor.ADO.DateInputADO dateInputADO = new UC.DateEditor.ADO.DateInputADO();

                dateInputADO.Time = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(tracking.TRACKING_TIME).Value;
                dateInputADO.Dates = new List<DateTime?>();
                dateInputADO.Dates.Add(dateInputADO.Time);

                this.ucDateProcessor.Reload(this.ucDate, dateInputADO);
                this.intructionTimeSelecteds = this.ucDateProcessor.GetValue(this.ucDate);
                this.isMultiDateState = false;
                if(string.IsNullOrEmpty(HisConfigs.Get<string>("HIS.HIS_TRACKING.SERVICE_REQ_ICD_OPTION")) && !string.IsNullOrEmpty(HisConfigs.Get<string>("HIS.Desktop.Plugins.TrackingCreate.UpdateTreatmentIcd")))
                {
                    LogSystem.Debug("cau hinh :  HIS.HIS_TRACKING.SERVICE_REQ_ICD_OPTION khong duoc bat va cau hinh HIS.Desktop.Plugins.TrackingCreate.UpdateTreatmentIcd duoc bat. -> load icd yhct");
                    if(tracking.TRADITIONAL_ICD_CODE != null)
                    {
                        HIS.UC.Icd.ADO.IcdInputADO ado = new UC.Icd.ADO.IcdInputADO();
                        ado.ICD_CODE = tracking.TRADITIONAL_ICD_CODE;
                        ado.ICD_NAME = tracking.TRADITIONAL_ICD_NAME;
                        if (ucIcdYHCT != null)
                        {
                            icdProcessorYHCT.Reload(ucIcdYHCT, ado);
                        }
                    }
                    if(tracking.TRADITIONAL_ICD_SUB_CODE != null)
                    {
                        HIS.UC.SecondaryIcd.ADO.SecondaryIcdDataADO ado = new UC.SecondaryIcd.ADO.SecondaryIcdDataADO();
                        ado.ICD_SUB_CODE = tracking.TRADITIONAL_ICD_SUB_CODE;
                        ado.ICD_TEXT = tracking.TRADITIONAL_ICD_TEXT;
                        if (ucSecondaryIcdYHCT != null)
                        {
                            subIcdProcessorYHCT.Reload(ucSecondaryIcdYHCT, ado);
                        }
                    }
                }

                Inventec.Common.Logging.LogSystem.Debug("ProcessAfterChangeTrackingTime.2");
                if (this.actionType == GlobalVariables.ActionView)
                {
                    //Trường hợp tạo/sửa tờ điều trị sau khi lưu đơn(nút lưu bị disable) ==> tự động gọi hàm lưu kê đơn để cập nhật lại ngày kê đơn theo ngày ở tờ điều trị
                    Inventec.Common.Logging.LogSystem.Debug("ProcessAfterChangeTrackingTime.3");
                    this.InitWorker();
                    this.actionType = GlobalVariables.ActionEdit;

                    this.ProcessSaveData(false);//TODO cần check tiếp
                    Inventec.Common.Logging.LogSystem.Debug("ProcessAfterChangeTrackingTime.4");
                }
                Inventec.Common.Logging.LogSystem.Debug("ProcessAfterChangeTrackingTime.5");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn("Co loi khi delegate ProcessAfterChangeTrackingTime duoc goi tu chuc nang tao/sua to dieu tri", ex);
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
                    WaitingManager.Hide();
                    MessageManager.Show(String.Format(ResourceMessage.TongTienTheoDoiTuongDieuTriChoBHYTDaVuotquaMucGioiHan, GetTreatmentTypeNameByCode(this.currentHisPatientTypeAlter.TREATMENT_TYPE_CODE), Inventec.Common.Number.Convert.NumberToString(totalPriceSum, 0), Inventec.Common.Number.Convert.NumberToString(warningOverCeiling, 0)));
                    WaitingManager.Show();
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

        HIS.UC.TreatmentFinish.ADO.DataInputADO GetDateADO()
        {
            HIS.UC.TreatmentFinish.ADO.DataInputADO result = new UC.TreatmentFinish.ADO.DataInputADO();
            try
            {
                if (spinSoNgay.EditValue != null)
                    result.SoNgay = (long)spinSoNgay.Value;
                long useTimeMax = 0;
                if (this.mediMatyTypeADOs != null && this.mediMatyTypeADOs.Count > 0)
                {
                    var useTimeFind = this.mediMatyTypeADOs.Where(o => o.UseTimeTo > 0).ToList();
                    useTimeMax = ((useTimeFind != null && useTimeFind.Count > 0) ? useTimeFind.Max(o => o.UseTimeTo ?? 0) : 0);
                    result.UseTimeTo = useTimeMax;
                }
                if (this.intructionTimeSelecteds != null && this.intructionTimeSelecteds.Count > 0)
                {
                    long useTime = this.intructionTimeSelecteds.OrderBy(o => o).First();
                    result.UseTime = useTime;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private void ProcessMergeDuplicateRowForListProcessing()
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
                            checkPresExists.AmountOneRemedy = (checkPresExists.AmountOneRemedy ?? 0) + (item.AmountOneRemedy ?? 0);
                            checkPresExists.AmountOneRemedy = (checkPresExists.AMOUNT ?? 0) / (checkPresExists.RemedyCount ?? 1);
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
                            checkPresExists.AmountOneRemedy = (checkPresExists.AMOUNT ?? 0) / (checkPresExists.RemedyCount ?? 1);
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
                            checkPresExists.AmountOneRemedy = (checkPresExists.AMOUNT ?? 0) / (checkPresExists.RemedyCount ?? 1);
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

        private void ProcessAddListRowDataIntoGridWithTakeBean()
        {
            try
            {
                List<MediMatyTypeADO> mediMatyTypeADOsTemp = new List<MediMatyTypeADO>();
                this.ValidDataMediMaty();
                if (this.ProcessValidMedicineTypeAge(true) && this.ProcessAmountInStockWarning())
                {
                    var mediMatyTypeADOsForTakeBeans__Other = (this.mediMatyTypeADOs.Where(o => o.DataType != HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC && o.DataType != HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.VATTU).ToList());
                    if (mediMatyTypeADOsForTakeBeans__Other != null && mediMatyTypeADOsForTakeBeans__Other.Count > 0)
                        mediMatyTypeADOsTemp.AddRange(mediMatyTypeADOsForTakeBeans__Other);

                    var mediMatyTypeADOsForTakeBeans__Medicine = (this.mediMatyTypeADOs.Where(o => o.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC && (o.AmountAlert ?? 0) == 0).ToList());
                    if (mediMatyTypeADOsForTakeBeans__Medicine != null && mediMatyTypeADOsForTakeBeans__Medicine.Count > 0)
                    {
                        if (!GlobalStore.IsTreatmentIn || GlobalStore.IsCabinet)
                        {
                            if (TakeOrReleaseBeanWorker.ProcessTakeListMedi(mediMatyTypeADOsForTakeBeans__Medicine))
                            {
                                mediMatyTypeADOsForTakeBeans__Medicine = mediMatyTypeADOsForTakeBeans__Medicine.Where(o => o.MedicineBean1Result != null && o.MedicineBean1Result.Count > 0).ToList();
                                if (mediMatyTypeADOsForTakeBeans__Medicine != null && mediMatyTypeADOsForTakeBeans__Medicine.Count > 0)
                                {
                                    mediMatyTypeADOsTemp.AddRange(mediMatyTypeADOsForTakeBeans__Medicine);
                                }
                            }
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
                            if (TakeOrReleaseBeanWorker.ProcessTakeListMaty(mediMatyTypeADOsForTakeBeans__Material))
                            {
                                mediMatyTypeADOsForTakeBeans__Material = mediMatyTypeADOsForTakeBeans__Material.Where(o => o.MaterialBean1Result != null && o.MaterialBean1Result.Count > 0).ToList();
                                if (mediMatyTypeADOsForTakeBeans__Material != null && mediMatyTypeADOsForTakeBeans__Material.Count > 0)
                                {
                                    mediMatyTypeADOsTemp.AddRange(mediMatyTypeADOsForTakeBeans__Material);
                                }
                            }
                        }
                        else
                        {
                            mediMatyTypeADOsForTakeBeans__Material.ForEach(o => SetPriceOne(o));
                            mediMatyTypeADOsTemp.AddRange(mediMatyTypeADOsForTakeBeans__Material);
                        }
                    }

                    var mediMatyTypeInStockWarnings = this.mediMatyTypeADOs.Where(o => (o.AmountAlert ?? 0) > 0).ToList();
                    if (mediMatyTypeInStockWarnings != null && mediMatyTypeInStockWarnings.Count > 0)
                    {
                        mediMatyTypeADOsTemp.AddRange(mediMatyTypeInStockWarnings);
                    }
                    foreach (var item in mediMatyTypeADOsTemp)
                    {
                        UpdateExpMestReasonInDataRow(item);
                    }
                }

                this.gridControlServiceProcess.DataSource = null;
                this.mediMatyTypeADOs = mediMatyTypeADOsTemp.OrderBy(o => o.NUM_ORDER).ToList();
                this.gridControlServiceProcess.DataSource = this.mediMatyTypeADOs;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// - Khi nhấn nút "Lưu", nếu tồn tại dịch vụ chưa chọn "Nguồn khác" thì hiển thị cảnh báo: "Bạn chưa chọn "Nguồn khác" đối với dịch vụ XXX, YYY", không cho phép lưu.
        //Trong đó: XXX, YYY là tên các dịch vụ chưa chọn nguồn khác và có Đối tượng thanh toán có cấu hình "Nguồn khác"
        //- Khi lưu, gửi thông tin nguồn khác vào OtherPaySourceId vào SDO để xử lý lưu thông tin vào server
        /// </summary>
        /// <param name="serviceCheckeds__Send"></param>
        /// <returns></returns>
        private bool ValidSereServWithOtherPaySource(List<MediMatyTypeADO> serviceCheckeds__Send)
        {
            bool valid = true;
            try
            {
                if (serviceCheckeds__Send != null && serviceCheckeds__Send.Count > 0)
                {
                    string sereServOtherpaysourceStr = "";
                    foreach (var item in serviceCheckeds__Send)
                    {
                        var workingPatientType = currentPatientTypes.Where(t => t.ID == item.PATIENT_TYPE_ID).FirstOrDefault();
                        if (workingPatientType != null && !String.IsNullOrEmpty(workingPatientType.OTHER_PAY_SOURCE_IDS) && (item.OTHER_PAY_SOURCE_ID ?? 0) <= 0)
                        {
                            sereServOtherpaysourceStr += item.MEDICINE_TYPE_NAME + ",";
                        }
                    }

                    if (!String.IsNullOrEmpty(sereServOtherpaysourceStr))
                    {
                        sereServOtherpaysourceStr = sereServOtherpaysourceStr.TrimEnd(',');
                        MessageBox.Show(string.Format(ResourceMessage.SereServOtherpaySourceAlert__DVChuaDuocNhapNguonChiTra, sereServOtherpaysourceStr), Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao));
                        Inventec.Common.Logging.LogSystem.Warn("ValidSereServWithOtherPaySource: valid = false_____" + sereServOtherpaysourceStr);
                        valid = false;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return valid;
        }

        private bool ProcessValidMedicineTypeAge()
        {
            return ProcessValidMedicineTypeAge(false);
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
        bool IsWarnAtcByGroupCode(MediMatyTypeADO mdADO, List<MediMatyTypeADO> mdADOs, ref Dictionary<string, List<MediMatyTypeADO>> dicAtcWarn)
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
                    var mediMatyTypeOverlapWarnings = this.mediMatyTypeADOs.Where(o => !String.IsNullOrEmpty(o.ATC_CODES) && (o.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC || o.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC_DM || o.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC_TUTUC)).ToList();
                    if (mediMatyTypeOverlapWarnings != null && mediMatyTypeOverlapWarnings.Count > 0)
                    {
                        string medicineTypeNames = "";
                        var atcDatas = BackendDataWorker.Get<HIS_ATC>();
                        atcDatas = atcDatas != null && atcDatas.Count > 0 ? atcDatas.Where(o => o.IS_ACTIVE == 1).ToList() : null;
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
                                        var atc = atcDatas != null ? atcDatas.Where(kk => kk.ATC_CODE == itemAtcSearch.Key).FirstOrDefault() : null;
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
                                            Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => atc), atc)
                                                + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => mdADOsWarn), mdADOsWarn));
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
                                medicineTypeNames += "[" + spinSoNgay.Text + "]" + (aw.Value.Aggregate((i, j) => new MediMatyTypeADO { ACTIVE_INGR_BHYT_NAME = i.ACTIVE_INGR_BHYT_NAME + " - " + j.ACTIVE_INGR_BHYT_NAME }).ACTIVE_INGR_BHYT_NAME) + "  - (" + aw.Key + " - " + GetAtcNameByCode(aw.Key) + ")" + "\r\n";
                            }
                        }

                        if (!String.IsNullOrEmpty(medicineTypeNames))
                        {
                            DialogResult myResult;
                            string tt = ResourceMessage.BanCoMuonTiepTuc.Replace("{1}.", "");

                            myResult = DevExpress.XtraEditors.XtraMessageBox.Show(
                         String.Format(tt, Inventec.Desktop.Common.HtmlString.ProcessorString.InsertSpacialTag(String.Format("Trùng nhóm dược tính:\r\n{0}", medicineTypeNames), Inventec.Desktop.Common.HtmlString.SpacialTag.Tag.Br)), Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaCanhBao), MessageBoxButtons.YesNo, DevExpress.Utils.DefaultBoolean.True);
                            if (myResult != System.Windows.Forms.DialogResult.Yes)
                                result = false;
                        }
                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => medicineTypeNames), medicineTypeNames)
                            + "_______" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => dicAtcWarn), dicAtcWarn));
                    }
                }
                else if (HisConfigCFG.AtcCodeOverlarWarningOption == "2")
                {
                    var mediMatyTypeOverlapWarnings = this.mediMatyTypeADOs.Where(o => !String.IsNullOrEmpty(o.ATC_GROUP_CODES) && (o.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC || o.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC_DM || o.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC_TUTUC)).ToList();
                    if (mediMatyTypeOverlapWarnings != null && mediMatyTypeOverlapWarnings.Count > 0)
                    {
                        string medicineTypeNames = "";
                        var atcDatas = BackendDataWorker.Get<HIS_ATC_GROUP>();
                        atcDatas = atcDatas != null && atcDatas.Count > 0 ? atcDatas.Where(o => o.IS_ACTIVE == 1).ToList() : null;
                        Dictionary<string, List<MediMatyTypeADO>> dicAtcWarn = new Dictionary<string, List<MediMatyTypeADO>>();

                        foreach (var item in mediMatyTypeOverlapWarnings)
                        {
                            Dictionary<string, List<MediMatyTypeADO>> dicAtcWarnOne = new Dictionary<string, List<MediMatyTypeADO>>();
                            List<MediMatyTypeADO> mdADOsWarn = new List<MediMatyTypeADO>();
                            if (IsWarnAtcByGroupCode(item, mediMatyTypeOverlapWarnings, ref dicAtcWarnOne))
                            {
                                if (dicAtcWarnOne != null && dicAtcWarnOne.Count > 0)
                                {
                                    foreach (var itemAtcSearch in dicAtcWarnOne)
                                    {
                                        var atc = atcDatas != null ? atcDatas.Where(kk => kk.ATC_GROUP_CODE == itemAtcSearch.Key).FirstOrDefault() : null;
                                        if (atc != null)
                                        {
                                            mdADOsWarn = itemAtcSearch.Value;
                                            if (dicAtcWarn.ContainsKey(atc.ATC_GROUP_CODE))
                                            {
                                                var listValue = dicAtcWarn[atc.ATC_GROUP_CODE];
                                                listValue.AddRange(mdADOsWarn);
                                                dicAtcWarn[atc.ATC_GROUP_CODE] = listValue.Distinct().ToList();
                                            }
                                            else
                                            {
                                                dicAtcWarn.Add(atc.ATC_GROUP_CODE, mdADOsWarn);
                                            }
                                            Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => atc), atc)
                                                + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => mdADOsWarn), mdADOsWarn));
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
                                medicineTypeNames += (aw.Value.Aggregate((i, j) => new MediMatyTypeADO { MEDICINE_TYPE_NAME = i.MEDICINE_TYPE_NAME + " - " + j.MEDICINE_TYPE_NAME }).MEDICINE_TYPE_NAME) + " - (" + GetAtcNameByGroupCode(aw.Key) + ")" + "\r\n";
                            }
                        }

                        if (!String.IsNullOrEmpty(medicineTypeNames))
                        {
                            DialogResult myResult;
                            string tt = ResourceMessage.BanCoMuonTiepTuc.Replace("{1}.", "");

                            myResult = DevExpress.XtraEditors.XtraMessageBox.Show(
                         String.Format(tt, Inventec.Desktop.Common.HtmlString.ProcessorString.InsertSpacialTag(String.Format("Trùng nhóm dược lý:\r\n{0}", medicineTypeNames), Inventec.Desktop.Common.HtmlString.SpacialTag.Tag.Br)), Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaCanhBao), MessageBoxButtons.YesNo, DevExpress.Utils.DefaultBoolean.True);
                            if (myResult != System.Windows.Forms.DialogResult.Yes)
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
        private string GetAtcNameByGroupCode(string code)
        {
            string rs = "";
            var atcDatas = BackendDataWorker.Get<HIS_ATC_GROUP>();
            var atc = atcDatas != null && atcDatas.Count > 0 ? atcDatas.Where(o => o.ATC_GROUP_CODE == code).FirstOrDefault() : null;
            rs = atc != null ? atc.ATC_GROUP_NAME : "";
            return rs;
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
                        || item.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.VATTU_DM)
                    {
                        string messageDetail = "";
                        if (!MedicineAgeWorker.ValidThuocCoGioiHanTuoi(item.SERVICE_ID, this.patientDob, ref messageDetail, false))
                        {
                            messageErr += messageDetail + "\r\n";
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

        private void ProcessUpdateTutorialForSave()
        {
            try
            {
                if (HisConfigCFG.TutorialFormat == 1 || HisConfigCFG.TutorialFormat == 2)
                {
                    List<MediMatyTypeADO> mediMatyTypeADOTemps = new List<MediMatyTypeADO>();
                    string strTutorial = txtHuongDan.Text;
                    if (!String.IsNullOrEmpty(strTutorial) && this.mediMatyTypeADOs != null && this.mediMatyTypeADOs.Count > 0 && this.mediMatyTypeADOs.Exists(o => o.TUTORIAL != strTutorial))
                    {
                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("Old mediMatyTypeADOs:", mediMatyTypeADOs) + "____strTutorial:" + strTutorial);
                        DialogResult myResult;
                        myResult = MessageBox.Show(ResourceMessage.BanCoMuonCapNhatHuongDanThuocTrongDOnTheoHUongDanMoiKhong, HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (myResult == DialogResult.Yes)
                        {
                            foreach (var item in this.mediMatyTypeADOs)
                            {
                                item.TUTORIAL = strTutorial;
                            }
                        }
                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("new mediMatyTypeADOs:", mediMatyTypeADOs) + "____strTutorial:" + strTutorial);
                    }
                }
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

        /// <summary>
        /// Cảnh báo thuốc không có trong kho hoặc số lượng khả dụng không đủ
        /// </summary>
        private bool ProcessAmountInStockWarning()
        {
            bool result = true;
            try
            {
                string message = "";
                var mediMatyTypeInStockWarnings = this.mediMatyTypeADOs.Where(o => (o.AmountAlert ?? 0) > 0).ToList();
                if (mediMatyTypeInStockWarnings != null && mediMatyTypeInStockWarnings.Count > 0)
                {
                    foreach (var item in mediMatyTypeInStockWarnings)
                    {
                        message += (Inventec.Desktop.Common.HtmlString.ProcessorString.InsertColor(item.MEDICINE_TYPE_NAME, System.Drawing.Color.Red)
                            + " : " + Inventec.Desktop.Common.HtmlString.ProcessorString.InsertColor(Inventec.Common.Number.Convert.NumberToString((item.AmountAlert ?? 0), ConfigApplications.NumberSeperator), System.Drawing.Color.Maroon) + "; ");
                    }
                    message += ResourceMessage.VuotQuaSoLuongKhaDungTrongKho__CoMuonTiepTuc;
                    DialogResult dialogResult = DevExpress.XtraEditors.XtraMessageBox.Show(message, Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaCanhBao), MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning, DevExpress.Utils.DefaultBoolean.True);
                    if (dialogResult == DialogResult.Yes)
                    {
                        //Chuyển các thuốc hết khả dụng sang thuốc ngoài kho
                        foreach (var item in mediMatyTypeInStockWarnings)
                        {
                            var medimatyWorker = this.mediMatyTypeADOs.FirstOrDefault(o => o.SERVICE_ID == item.SERVICE_ID);
                            medimatyWorker.PATIENT_TYPE_ID = null;
                            medimatyWorker.PATIENT_TYPE_CODE = null;
                            medimatyWorker.PATIENT_TYPE_NAME = null;
                            medimatyWorker.DataType = HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC_DM;
                            medimatyWorker.AmountAlert = null;
                            medimatyWorker.MEDI_STOCK_ID = null;
                            medimatyWorker.MEDI_STOCK_CODE = null;
                            medimatyWorker.MEDI_STOCK_NAME = null;
                            medimatyWorker.ErrorMessageAmountAlert = "";
                            medimatyWorker.ErrorMessagePatientTypeId = "";
                            medimatyWorker.ErrorTypeAmountAlert = ErrorType.None;
                            medimatyWorker.ErrorTypePatientTypeId = ErrorType.None;
                            medimatyWorker.ErrorTypeMediMatyBean = ErrorType.None;
                        }
                    }
                    else if (dialogResult == DialogResult.No)
                    {
                        //this.mediMatyTypeADOs = new List<MediMatyTypeADO>();
                        //this.gridControlServiceProcess.DataSource = null;
                        //result = false;
                    }
                    else if (dialogResult == DialogResult.Cancel)
                    {
                        this.mediMatyTypeADOs = new List<MediMatyTypeADO>();
                        this.gridControlServiceProcess.DataSource = null;
                        result = false;
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
                if (rdOpionGroup.SelectedIndex == 0 && this.currentMedicineTypeADOForEdit != null && this.currentMedicineTypeADOForEdit.DataType != HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC_DM && this.currentMedicineTypeADOForEdit.DataType != HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.VATTU_DM)
                {
                    dataType = (this.currentMedicineTypeADOForEdit.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC ?
                        HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC : HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.VATTU);
                }
                else
                {
                    //Thuốc - vật tư tự mua
                    if (!String.IsNullOrEmpty(this.txtMedicineTypeOther.Text.Trim()))
                        dataType = HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC_TUTUC;
                    //Thuốc - Vật tư
                    else if (this.currentMedicineTypeADOForEdit.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC)
                        dataType = HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC_DM;
                    else
                        dataType = HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.VATTU_DM;
                }
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
                //Thuốc - vật tư tự mua
                if (!String.IsNullOrEmpty(this.txtMedicineTypeOther.Text.Trim()) && this.rdOpionGroup.SelectedIndex != 0)
                {
                    dataInput = txtMedicineTypeOther.Text;
                }
                //Thuốc - Vật tư
                else
                {
                    dataInput = this.currentMedicineTypeADOForEdit;
                }
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
                HIS.Desktop.Plugins.AssignPrescriptionYHCT.Edit.IEdit iEdit = HIS.Desktop.Plugins.AssignPrescriptionYHCT.Edit.EditFactory.MakeIEdit(
                param,
                this,
                ValidAddRow,
                ChoosePatientTypeDefaultlService,
                CalulateUseTimeTo,
                ExistsAssianInDay,
                this.currentMedicineTypeADOForEdit,
                datatype);

                if (iEdit != null)
                {
                    var success = iEdit.Run();
                    if (!success)
                    {
                        LogSystem.Debug("Edit medicine/ material row error => success fail");
                    }
                }
                else
                {
                    MessageManager.Show(param, false);
                    LogSystem.Debug("Edit medicine/ material row error => iEdit is null.");
                }

            }
            catch (Exception ex)
            {
                MessageManager.Show(param, false);
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void AddMediMatyClickHandler()
        {
            CommonParam param = new CommonParam();
            try
            {
                int datatype = GetDataTypeSelected();
                HIS.Desktop.Plugins.AssignPrescriptionYHCT.Add.IAdd iAdd = HIS.Desktop.Plugins.AssignPrescriptionYHCT.Add.AddFactory.MakeIAdd(
                param,
                this,
                ValidAddRow,
                ChoosePatientTypeDefaultlService,
                CalulateUseTimeTo,
                ExistsAssianInDay,
                this.currentMedicineTypeADOForEdit,
                datatype);

                if (iAdd != null)
                {
                    var success = iAdd.Run();
                    if (!success)
                    {
                        LogSystem.Debug("Add medicine/ material row error => success fail");
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
        private void LoadDataToGridMetyMatyTypeInStock()
        {
            try
            {
                if (this.rdOpionGroup.SelectedIndex == 0)
                {
                    if (!this.showMediStockDSDOs)
                    {
                        this.InitDataMetyMatyTypeInStockD();
                        this.showMediStockDSDOs = true;
                    }
                    this.theRequiredWidth = 1130;
                    this.theRequiredHeight = 200;
                    this.RebuildMediMatyWithInControlContainer(this.GetDataMediMatyInStock());
                    this.ResetComboNhaThuoc();
                }
                else
                {
                    this.theRequiredWidth = 850;
                    this.theRequiredHeight = 200;

                    this.InitComboNhaThuoc();
                    if (this.currentMediStockNhaThuocSelecteds == null || this.currentMediStockNhaThuocSelecteds.Count == 0)
                        RebuildMedicineTypeWithInControlContainer();
                    else
                        RebuildNhaThuocMediMatyWithInControlContainer();
                }
                this.ResetFocusMediMaty(true);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal void ResetFocusMediMaty(bool isFocus)
        {
            try
            {
                currentMedicineTypeADOForEdit = null;
                txtMediMatyForPrescription.Text = "";
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
                if (this.rdOpionGroup.SelectedIndex == 0)
                {
                    txtMedicineTypeOther.Enabled = txtUnitOther.Enabled = false;
                }
                else if (this.rdOpionGroup.SelectedIndex == 1)
                {
                    txtMedicineTypeOther.Enabled = txtUnitOther.Enabled = true;
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
                        if ((checkMatyInStock ?? 0) <= 0)//Trường hợp thuốc đã hết khả dụng yêu cầu trong kho
                        {
                            if ((mediMatyTypeADO.AMOUNT ?? 0) > ((mediMatyTypeADO.PRE_AMOUNT ?? 0) > 0 ? (mediMatyTypeADO.PRE_AMOUNT ?? 0) : (mediMatyTypeADO.BK_AMOUNT ?? 0)))
                            {
                                mediMatyTypeADO.AmountAlert = mediMatyTypeADO.AMOUNT;
                            }
                        }
                        else
                        {
                            var amountProcess = listData
                                .Where(o => o.MEDI_STOCK_ID == mediMatyTypeADO.MEDI_STOCK_ID
                                        && o.SERVICE_ID == mediMatyTypeADO.SERVICE_ID)
                                        .Sum(o => (((o.AMOUNT ?? 0) - ((!GlobalStore.IsTreatmentIn || GlobalStore.IsCabinet) ? o.BK_AMOUNT ?? 0 : 0)) - (o.PRE_AMOUNT ?? 0)));

                            if (amountProcess > 0 && amountProcess > checkMatyInStock)
                            {
                                mediMatyTypeADO.AmountAlert = amountProcess;
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
                this.cboMedicineUseForm.EditValue = null;
                this.spinAmount.Text = "";

                this.txtMedicineTypeOther.Text = "";
                this.txtUnitOther.Text = "";

                Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(this.dxValidProviderBoXung, this.dxErrorProvider1);
                Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(this.dxValidProviderBoXung__DuongDung, this.dxErrorProvider1);
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
                //this.actionBosung = GlobalVariables.ActionAdd;
                this.VisibleButton(this.actionBosung);

                if (this.actionType == GlobalVariables.ActionAdd)
                {
                    List<MediMatyTypeADO> serviceCheckeds__Send = this.mediMatyTypeADOs;
                    this.btnSave.Enabled = btnSaveAndPrint.Enabled = isCheckAssignServiceSimultaneityOption ? false : (serviceCheckeds__Send != null && serviceCheckeds__Send.Count > 0);
                    this.lciPrintAssignPrescription.Enabled = false;
                    this.btnAdd.Enabled = true;//this.btnNew.Enabled =
                    if (this.treatmentFinishProcessor != null && this.ucTreatmentFinish != null)
                        this.treatmentFinishProcessor.EnableChangeAutoTreatmentFinish(this.ucTreatmentFinish, (serviceCheckeds__Send != null && serviceCheckeds__Send.Count > 0));
                    this.gridViewServiceProcess.OptionsBehavior.Editable = true;
                }
                else if (this.actionType == GlobalVariables.ActionEdit)
                {
                    List<MediMatyTypeADO> serviceCheckeds__Send = this.mediMatyTypeADOs;
                    this.btnSave.Enabled = btnSaveAndPrint.Enabled = isCheckAssignServiceSimultaneityOption ? false : true;
                    this.lciPrintAssignPrescription.Enabled = true;
                    this.btnAdd.Enabled = true;
                    this.btnNew.Enabled = (oldServiceReq != null ? false : true);
                    if (this.treatmentFinishProcessor != null && this.ucTreatmentFinish != null)
                        this.treatmentFinishProcessor.EnableChangeAutoTreatmentFinish(this.ucTreatmentFinish, (serviceCheckeds__Send != null && serviceCheckeds__Send.Count > 0));
                    this.gridViewServiceProcess.OptionsBehavior.Editable = true;

                    //Nếu sửa đơn thuốc thì không cho chọn nhiều ngày
                    if (this.ucDateProcessor != null && this.ucDate != null)
                        this.ucDateProcessor.ReadOnly(this.ucDate, true);
                }
                else
                {
                    this.btnSave.Enabled = this.btnAdd.Enabled = btnSaveAndPrint.Enabled = false;
                    this.lciPrintAssignPrescription.Enabled = true;
                    if (this.assignPrescriptionEditADO == null)
                        this.btnNew.Enabled = true;
                    this.gridViewServiceProcess.OptionsBehavior.Editable = false;

                    //Nếu đã lưu đơn thuốc hoặc sửa đơn thuốc thì không cho chọn nhiều ngày
                    if (this.ucDateProcessor != null && this.ucDate != null)
                        this.ucDateProcessor.ReadOnly(this.ucDate, true);

                    //Nếu đã lưu đơn thuốc 
                    //-----> Đã check chọn tự động kết thúc điều trị luôn => disable check "kết thúc điều trị"
                    //-----> Chưa check chọn tự động kết thúc điều trị => enable check "kết thúc điều trị"
                    //chkAutoTreatmentFinish.Enabled = (!chkAutoTreatmentFinish.Checked);
                    if (this.treatmentFinishProcessor != null && this.ucTreatmentFinish != null)
                    {
                        var treatDT = this.treatmentFinishProcessor.GetDataOutput(this.ucTreatmentFinish);
                        if (treatDT != null)
                            this.treatmentFinishProcessor.EnableChangeAutoTreatmentFinish(this.ucTreatmentFinish, !treatDT.IsAutoTreatmentFinish);
                    }
                }


                this.lciExpMestReason.Visibility = actionType == GlobalVariables.ActionEdit ? DevExpress.XtraLayout.Utils.LayoutVisibility.Always : DevExpress.XtraLayout.Utils.LayoutVisibility.Never;

                if (HisConfigCFG.icdServiceAllowUpdate == 1)
                    btnBoSungPhacDo.Enabled = true;
                else
                    btnBoSungPhacDo.Enabled = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadDataByServicePackage()
        {
            try
            {
                if (actionType == GlobalVariables.ActionAdd)
                {
                    if (this.currentSereServ != null || this.currentSereServInEkip != null)
                    {
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
                        LogSystem.Debug("Loaded CreateThreadLoadDataByPackageService");
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
                    result = this.sereServWithTreatment.Any(o => o.SERVICE_ID == serviceId && o.INTRUCTION_TIME.ToString().Substring(0, 8) == intructionTimeSelecteds.OrderByDescending(t => t).First().ToString().Substring(0, 8));
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
                spinSoNgay.EditValue = 1;
                repositoryItemChkIsExpend__MedicinePage_Disable.ReadOnly = true;
                repositoryItemChkIsExpend__MedicinePage_Disable.Enabled = false;
                txtDuTruTime.Text = "";
                cboDuTruTime.EditValue = null;
                txtTimeTo.Text = "";

                this.IsManyDay = true;
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
                        this.spinSoNgay.Value = numOfDay;
                    }
                }


                this.lblTongTien.Text = "";
                this.idRow = 1;

                this.btnSave.Enabled = false;
                this.btnSaveAndPrint.Enabled = false;
                this.lciPrintAssignPrescription.Enabled = false;
                this.btnAdd.Enabled = false;

                this.txtLadder.Text = "";
                this.txtAdvise.Text = "";
                this.txtHuongDan.Text = "";
                this.spinSoNgay.Enabled = true;

                this.actionType = (this.assignPrescriptionEditADO != null ? GlobalVariables.ActionEdit : GlobalVariables.ActionAdd);
                this.actionBosung = GlobalVariables.ActionAdd;

                this.gridControlServiceProcess.DataSource = null;
                this.mediMatyTypeADOs = new List<MediMatyTypeADO>();
                this.cboExpMestTemplate.EditValue = null;
                this.cboExpMestTemplate.Properties.Buttons[1].Visible = false;
                this.txtExpMestTemplateCode.Text = "";

                this.ResetComboNhaThuoc();

                GlobalStore.ClientSessionKey = Guid.NewGuid().ToString();
                this.currentPatientTypes = BackendDataWorker.Get<HIS_PATIENT_TYPE>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
                this.currentMedicineTypes = BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>().Where(o => o.IS_LEAF == GlobalVariables.CommonNumberTrue).ToList();
                long isOnlyDisplayMediMateIsBusiness = Inventec.Common.TypeConvert.Parse.ToInt64(HisConfigs.Get<string>(HisConfigCFG.ONLY_DISPLAY_MEDIMATE_IS_BUSINESS));
                if (isOnlyDisplayMediMateIsBusiness == 1 && this.currentMedicineTypes != null && this.currentMedicineTypes.Count > 0)
                    this.currentMedicineTypes = this.currentMedicineTypes.Where(o => o.IS_BUSINESS.HasValue && o.IS_BUSINESS.Value == 1).ToList();

                this.currentMaterialTypes = BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>().Where(o => o.IS_LEAF == GlobalVariables.CommonNumberTrue).ToList();

                Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(this.dxValidationProviderControl, dxErrorProvider1);

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
                        serviceInPackages = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_SERVICE>().Where(o => serviceIds.Contains(o.ID)).ToList();
                        if (serviceInPackages == null || serviceInPackages.Count == 0)
                            throw new ArgumentNullException("serviceInPackages is null");

                        if (this.mediStockD1ADOs == null || this.mediStockD1ADOs.Count == 0)
                            this.InitDataMetyMatyTypeInStockD1(this.currentMediStock);

                        //Release tat ca cac thuoc/ vat tu da duoc take bean truoc do nhung chua duoc luu
                        this.ReleaseAllMediByUser();

                        //Load data for material in package
                        this.LoadPageMateMetyInServicePackage(serviceInPackages);
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

        private object GetDataMediMatyInStock()
        {
            object result = null;
            try
            {
                result = this.mediStockD1ADOs;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private decimal? AmountOutOfStock(MediMatyTypeADO model, long serviceId, long meidStockId)
        {
            decimal? result = null;
            try
            {
                var checkMatyInStock = GetDataAmountOutOfStock(model, serviceId, meidStockId);
                var medi1 = checkMatyInStock as DMediStock1ADO;
                if (medi1 != null)
                {
                    result = (medi1.AMOUNT ?? 0);
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
                if (this.mediStockD1ADOs == null || this.mediStockD1ADOs.Count == 0)
                    this.InitDataMetyMatyTypeInStockD1(this.currentMediStock);

                var result1 = this.mediStockD1ADOs.FirstOrDefault(o => o.SERVICE_ID == serviceId && (meidStockId == 0 || o.MEDI_STOCK_ID == meidStockId));
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

        private void LoadPageMateMetyInServicePackage(List<MOS.EFMODEL.DataModels.V_HIS_SERVICE> serviceMaMes)
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
                    if (model.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC)
                    {
                        var mety = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_MEDICINE_TYPE>().FirstOrDefault(o => o.SERVICE_ID == item.ID);
                        if (mety != null)
                        {
                            model.MEDICINE_USE_FORM_ID = mety.MEDICINE_USE_FORM_ID;
                            model.MEDICINE_USE_FORM_CODE = mety.MEDICINE_USE_FORM_CODE;
                            model.MEDICINE_USE_FORM_NAME = mety.MEDICINE_USE_FORM_NAME;
                            model.TUTORIAL = mety.TUTORIAL;
                            model.MANUFACTURER_CODE = mety.MANUFACTURER_CODE;
                            model.MANUFACTURER_ID = mety.MANUFACTURER_ID;
                            model.MANUFACTURER_NAME = mety.MANUFACTURER_NAME;
                            model.MEDICINE_USE_FORM_ID = mety.MEDICINE_USE_FORM_ID;
                            model.MEDICINE_USE_FORM_CODE = mety.MEDICINE_USE_FORM_CODE;
                            model.MEDICINE_USE_FORM_NAME = mety.MEDICINE_USE_FORM_NAME;
                            model.TUTORIAL = mety.TUTORIAL;
                            model.ACTIVE_INGR_BHYT_CODE = mety.ACTIVE_INGR_BHYT_CODE;
                            model.ACTIVE_INGR_BHYT_NAME = mety.ACTIVE_INGR_BHYT_NAME;
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
                            model.MANUFACTURER_CODE = maty.MANUFACTURER_CODE;
                            model.MANUFACTURER_ID = maty.MANUFACTURER_ID;
                            model.MANUFACTURER_NAME = maty.MANUFACTURER_NAME;
                        }
                    }

                    //Lay doi tuong mac dinh
                    MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE patientType = new MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE();
                    patientType = this.ChoosePatientTypeDefaultlService(this.currentHisPatientTypeAlter.PATIENT_TYPE_ID, item.ID, item.SERVICE_TYPE_ID);
                    if (patientType != null && patientType.ID > 0)
                    {
                        model.PATIENT_TYPE_ID = patientType.ID;
                        model.PATIENT_TYPE_CODE = patientType.PATIENT_TYPE_CODE;
                        model.PATIENT_TYPE_NAME = patientType.PATIENT_TYPE_NAME;
                    }

                    FillDataOtherPaySourceDataRow(model);

                    //Lay so luong thuoc trong goi
                    var matyInSePa = this.servicePackageByServices.FirstOrDefault(o => o.SERVICE_ATTACH_ID == item.ID);
                    if (matyInSePa != null)
                        model.AMOUNT = matyInSePa.AMOUNT;

                    //Kiem tra thuoc trong kho, neu so luong trong goi lon hon 
                    //so luong kha dung trong kho thi gan vao so lung canh bao
                    decimal damount = (AmountOutOfStock(model, item.ID, 0) ?? 0);
                    if (damount > 0)
                    {
                        model.AMOUNT = damount;
                        model.AmountAlert = damount;
                    }

                    model.NUM_ORDER = idRow;

                    this.mediMatyTypeADOs.Add(model);
                    this.idRow += stepRow;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitMediStock(long? patientTypeId)
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("MEDI_STOCK_CODE", "", 150, 1));
                columnInfos.Add(new ColumnInfo("MEDI_STOCK_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("MEDI_STOCK_NAME", "MEDI_STOCK_ID", columnInfos, false, 400);
                ControlEditorLoader.Load(this.cboMediStockExport, this.currentMestRoomByRooms, controlEditorADO);

                if (this.currentMestRoomByRooms != null && this.currentMestRoomByRooms.Count > 0)
                {
                    this.cboMediStockExport.EditValue = this.currentMestRoomByRooms[0].MEDI_STOCK_ID;
                    this.cboMediStockExport.Properties.Buttons[1].Visible = true;
                }
                else
                {
                    this.cboMediStockExport.EditValue = null;
                    this.cboMediStockExport.Properties.Buttons[1].Visible = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FillTreatmentInfo__PatientType()
        {
            try
            {
                this.lblPatientName.Text = this.currentTreatmentWithPatientType.TDL_PATIENT_NAME;
                if (this.currentTreatmentWithPatientType.TDL_PATIENT_DOB > 0)
                    this.lblDob.Text = Inventec.Common.DateTime.Convert.TimeNumberToDateString(this.currentTreatmentWithPatientType.TDL_PATIENT_DOB);
                this.lblGenderName.Text = this.currentTreatmentWithPatientType.TDL_PATIENT_GENDER_NAME;

                if (this.currentHisPatientTypeAlter != null)
                {
                    this.lblPatientTypeName.Text = this.currentHisPatientTypeAlter.PATIENT_TYPE_NAME;
                    this.lblTreatmentTypeName.Text = this.currentHisPatientTypeAlter.TREATMENT_TYPE_NAME;
                }

                if (!String.IsNullOrEmpty(this.currentHisPatientTypeAlter.HEIN_CARD_NUMBER))
                    lblHeinCardNumberInfo.Text = String.Format("{0} \r\n({1} - {2})",
                      Inventec.Desktop.Common.HtmlString.ProcessorString.InsertColor(HeinCardHelper.SetHeinCardNumberDisplayByNumber(this.currentHisPatientTypeAlter.HEIN_CARD_NUMBER), System.Drawing.Color.Green),
                        Inventec.Common.DateTime.Convert.TimeNumberToDateString(this.currentHisPatientTypeAlter.HEIN_CARD_FROM_TIME ?? 0),
                        Inventec.Common.DateTime.Convert.TimeNumberToDateString(this.currentHisPatientTypeAlter.HEIN_CARD_TO_TIME ?? 0));
                else
                    lblHeinCardNumberInfo.Text = "";

                if (this.totalPriceBHYT > 0)
                {
                    //this.lblluyKeTienThuoc.Text = Inventec.Common.Number.Convert.NumberToStringMoneyAfterRound(this.totalPriceBHYT);//tong tien                 
                    //if (this.totalPriceBHYT < 0)
                    //{
                    //    this.lblluyKeTienThuoc.Appearance.ForeColor = Color.Blue;
                    //}
                    //else
                    //{
                    //    this.lblluyKeTienThuoc.Appearance.ForeColor = Color.Red;
                    //}
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void CreateThreadFillDataToComboPriviousExpMest(object param)
        {
            Thread thread = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(this.FillDataToComboPriviousExpMestNewThread));
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

        private void FillDataToComboPriviousExpMestNewThread(object param)
        {
            try
            {
                Thread.SetData(TokenCodeStore.SlotTokenCode, Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetTokenData().TokenCode);

                if (this.InvokeRequired)
                {
                    this.Invoke(new MethodInvoker(delegate { this.FillDataToComboPriviousExpMest((HisTreatmentWithPatientTypeInfoSDO)param); }));
                }
                else
                {
                    this.FillDataToComboPriviousExpMest((HisTreatmentWithPatientTypeInfoSDO)param);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private async Task FillDataToComboPriviousExpMest(HisTreatmentWithPatientTypeInfoSDO treatment)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("FillDataToComboPriviousExpMest. 1");
                if (this.periousExpMestListProcessor != null && this.ucPeriousExpMestList != null)
                {
                    this.periousExpMestListProcessor.Load(this.ucPeriousExpMestList);
                    Inventec.Common.Logging.LogSystem.Debug("FillDataToComboPriviousExpMest. 2");
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FillDataIntoPatientTypeCombo(MediMatyTypeADO data, GridLookUpEdit patientTypeCombo)
        {
            try
            {
                this.InitComboPatientType(patientTypeCombo, currentPatientTypeWithPatientTypeAlter);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
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
                if (this.mediStockD1ADOs == null || this.mediStockD1ADOs.Count == 0)
                    InitDataMetyMatyTypeInStockD1(this.currentMediStock);

                if (this.mediStockD1ADOs != null && this.mediStockD1ADOs.Count > 0)
                {
                    var medicineInStock = this.mediStockD1ADOs.FirstOrDefault(o => o.SERVICE_ID == model.SERVICE_ID);//Mac dinh lay kho dau tien
                    if (medicineInStock != null)
                    {
                        //Trường hợp thuốc không đủ để xuất, số lượng khả dụng không đủ
                        //Set AmountAlert đánh dấu cảnh báo thuốc đã hết
                        if (medicineInStock.AMOUNT < model.AMOUNT)
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

        private void ProcessGetEmteMedcineType(List<V_HIS_EMTE_MEDICINE_TYPE> listEmteMedcineType)
        {
            try
            {
                if (listEmteMedcineType != null && listEmteMedcineType.Count > 0)
                {
                    var checkIsOutMediStocK = listEmteMedcineType.Where(o => o.IS_OUT_MEDI_STOCK == null).ToList();
                    if (checkIsOutMediStocK != null && checkIsOutMediStocK.Count > 0 && (this.currentMediStock == null || this.currentMediStock.Count == 0))
                    {
                        this.MediStockNull = true;
                        DevExpress.XtraEditors.XtraMessageBox.Show(ResourceMessage.BanChuaChonKhoXuat, Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageBoxButtons.OK);
                        cboMediStockExport.Focus();
                        return;
                    }
                    this.MediStockNull = false;

                    var q1 = (from m in listEmteMedcineType
                              select new MediMatyTypeADO(m)).ToList();
                    if (q1 != null && q1.Count > 0)
                        this.mediMatyTypeADOs.AddRange(q1);

                    MediMatyTypeADO mediMatyTypeADO = this.mediMatyTypeADOs.FirstOrDefault(o => o.RemedyCount > 0);
                    if (mediMatyTypeADO != null)
                    {
                        txtLadder.Text = mediMatyTypeADO.RemedyCount.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ProcessGetEmteMaterialType(List<V_HIS_EMTE_MATERIAL_TYPE> listEmteMaterialType)
        {
            try
            {
                if (listEmteMaterialType != null && listEmteMaterialType.Count > 0)
                {
                    var checkIsOutMediStocK = listEmteMaterialType.Where(o => o.IS_OUT_MEDI_STOCK == null).ToList();
                    if (checkIsOutMediStocK != null && checkIsOutMediStocK.Count > 0 && (this.currentMediStock == null || this.currentMediStock.Count == 0))
                    {
                        this.MediStockNull = true;
                        DevExpress.XtraEditors.XtraMessageBox.Show(ResourceMessage.BanChuaChonKhoXuat, Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageBoxButtons.OK);
                        cboMediStockExport.Focus();
                        return;
                    }

                    this.MediStockNull = false;

                    var q1 = (from m in listEmteMaterialType
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
                    LogSystem.Debug("btnAdd_TabMedicine_Click => thao tac khong hop le. actionType = " + this.actionType);
                    return;
                }

                //Release tat ca cac thuoc/ vat tu da duoc take bean truoc do nhung chua duoc luu
                this.ReleaseAllMediByUser();

                this.ProcessGetEmteMedcineType(this.GetEmteMedicineTypeByExpMestId(expTemplate.ID));
                this.ProcessGetEmteMaterialType(this.GetEmteMaterialTypeByExpMestId(expTemplate.ID));
                this.ProcessRemecountTutorial();
                this.ProcessInstructionTimeMediForEdit();
                this.ProcessMergeDuplicateRowForListProcessing();
                foreach (var item in this.mediMatyTypeADOs)
                {
                    if (!HIS.Desktop.Plugins.AssignPrescriptionYHCT.ValidAcinInteractiveWorker.Valid(item, mediMatyTypeADOs, this.LstExpMestMedicine))
                        this.mediMatyTypeADOs.Remove(item);
                }
                this.ProcessAddListRowDataIntoGridWithTakeBean();
                //this.VerifyWarningOverCeiling();
                this.ReloadDataAvaiableMediBeanInCombo();
                this.ProcessMediStock(this.mediMatyTypeADOs);
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

                    long ladder = Inventec.Common.TypeConvert.Parse.ToInt64(txtLadder.Text);
                    if (ladder > 0)
                    {
                        foreach (var item in this.mediMatyTypeADOs)
                        {
                            if (item.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC
                                || item.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC_DM
                                || item.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC_TUTUC)
                            {
                                item.AmountOneRemedy = (item.AMOUNT ?? 0) / ladder;
                                item.RemedyCount = ladder;
                            }
                        }
                    }
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
                    List<HIS_MEDICINE_BEAN> medicineBeans = null;
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
        private void ProcessGetExpMestMedicine(List<V_HIS_EXP_MEST_MEDICINE> lstExpMestMedicine, V_HIS_SERVICE_REQ_7 serviceReq, long currentInstructionTime)
        {
            try
            {
                //V_HIS_MEDICINE_BEAN>
                if (lstExpMestMedicine != null && lstExpMestMedicine.Count > 0)
                {
                    if (this.currentMediStock == null || this.currentMediStock.Count == 0)
                    {
                        DevExpress.XtraEditors.XtraMessageBox.Show(ResourceMessage.BanChuaChonKhoXuat, Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageBoxButtons.OK);
                        cboMediStockExport.Focus();
                        return;
                    }

                    List<HIS_MEDICINE_BEAN> medicineBeans = null;
                    var q1 = (from m in lstExpMestMedicine
                              select new MediMatyTypeADO(m, currentInstructionTime, medicineBeans, serviceReq)).ToList();
                    if (q1 != null && q1.Count > 0)
                        this.mediMatyTypeADOs.AddRange(q1);

                    //if (this.mediMatyTypeADOs != null && this.mediMatyTypeADOs.Count > 0)
                    //{
                    //    MediMatyTypeADO mediMatyTypeADO = this.mediMatyTypeADOs.FirstOrDefault(o => o.RemedyCount > 0);
                    //    txtLadder.Text = mediMatyTypeADO != null ? mediMatyTypeADO.RemedyCount.ToString() : null;
                    //}
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
                    if (this.currentMediStock == null || this.currentMediStock.Count == 0)
                    {
                        DevExpress.XtraEditors.XtraMessageBox.Show(ResourceMessage.BanChuaChonKhoXuat, Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageBoxButtons.OK);
                        cboMediStockExport.Focus();
                        return;
                    }

                    List<HIS_MATERIAL_BEAN> materialBeans = null;
                    if (isEdit)
                    {
                        materialBeans = GetMaterialBeanByExpMestMedicine(lstExpMestMaterial.Select(o => o.ID).ToList());
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

                    //long ladder = Inventec.Common.TypeConvert.Parse.ToInt64(txtLadder.Text);
                    //if (ladder > 0)
                    //{
                    //    foreach (var item in this.mediMatyTypeADOs)
                    //    {
                    //        if (item.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC
                    //            || item.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC_DM
                    //            || item.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC_TUTUC)
                    //        {
                    //            item.AmountOneRemedy = (item.AMOUNT ?? 0) / ladder;
                    //            item.RemedyCount = ladder;
                    //        }
                    //    }
                    //}
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

                    //long ladder = Inventec.Common.TypeConvert.Parse.ToInt64(txtLadder.Text);
                    //foreach (var item in this.mediMatyTypeADOs)
                    //{
                    //    if (item.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC
                    //            || item.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC_DM
                    //            || item.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC_TUTUC)
                    //    {
                    //        item.AmountOneRemedy = (item.AMOUNT ?? 0) / ladder;
                    //        item.RemedyCount = ladder;
                    //    }
                    //}
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

        private void UpdateRemyCountForDetailEditPres()
        {
            try
            {
                long ladder = Inventec.Common.TypeConvert.Parse.ToInt64(txtLadder.Text);
                if (ladder > 0)
                {
                    foreach (var item in this.mediMatyTypeADOs)
                    {
                        if (item.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC
                            || item.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC_DM
                            || item.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC_TUTUC)
                        {
                            item.AmountOneRemedy = (item.AMOUNT ?? 0) / ladder;
                            item.RemedyCount = ladder;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ViewPrescriptionPreviousButtonClick(V_HIS_SERVICE_REQ_7 row)
        {
            try
            {
                if (row != null)
                {
                    WaitingManager.Show();

                    Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.ExpMestViewDetail").FirstOrDefault();
                    if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'HIS.Desktop.Plugins.ExpMestViewDetail'");
                    if (!moduleData.IsPlugin || moduleData.ExtensionInfo == null) throw new NullReferenceException("Module 'HIS.Desktop.Plugins.ExpMestViewDetail' is not plugins");

                    V_HIS_EXP_MEST expMest = ExpMestWorker.CreateView(row);
                    List<object> listArgs = new List<object>();
                    listArgs.Add(false);
                    listArgs.Add(expMest);
                    var extenceInstance = HIS.Desktop.Utility.PluginInstance.GetPluginInstance(moduleData, listArgs);
                    if (extenceInstance == null) throw new ArgumentNullException("Khoi tao moduleData that bai. extenceInstance = null");

                    WaitingManager.Hide();
                    ((Form)extenceInstance).Show();
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

        private void ProcessChoicePrescriptionPrevious(List<MOS.EFMODEL.DataModels.V_HIS_SERVICE_REQ_7> lstServiceReq)
        {
            try
            {
                if (lstServiceReq == null || lstServiceReq.Count() <= 0) return;
                if (this.actionType == GlobalVariables.ActionView)
                {
                    LogSystem.Debug("ProcessChoicePrescriptionPrevious => thao tac khong hop le. actionType = " + this.actionType);
                    return;
                }
                Inventec.Common.Logging.LogSystem.Debug("ProcessChoicePrescriptionPrevious. 1");
                //Release tat ca cac thuoc/ vat tu da duoc take bean truoc do nhung chua duoc luu
                this.ReleaseAllMediByUser();

                foreach (var serviceReq in lstServiceReq)
                {
                    if (serviceReq.EXP_MEST_ID.HasValue && serviceReq.EXP_MEST_TYPE_ID.HasValue)
                    {
                        Inventec.Common.Logging.LogSystem.Debug("ProcessChoicePrescriptionPrevious. 2");
                        this.ProcessGetExpMestMedicine(this.GetExpMestMedicineByExpMestId(serviceReq.EXP_MEST_ID ?? 0), serviceReq, this.intructionTimeSelecteds.First());
                        Inventec.Common.Logging.LogSystem.Debug("ProcessChoicePrescriptionPrevious. 3");
                        this.ProcessGetExpMestMaterial(this.GetExpMestMaterialByExpMestId(serviceReq.EXP_MEST_ID ?? 0), false);
                        Inventec.Common.Logging.LogSystem.Debug("ProcessChoicePrescriptionPrevious. 4");
                        //if (GlobalStore.IsTreatmentIn && !GlobalStore.IsCabinet)
                        //{
                        //    this.ProcessGetExpMestMetyReq(this.GetExpMestMetyReqByExpMestId(serviceReq.EXP_MEST_ID ?? 0));
                        //    this.ProcessGetExpMestMatyReq(this.GetExpMestMatyReqByExpMestId(serviceReq.EXP_MEST_ID ?? 0));
                        //}
                        //else
                        //{
                        //    this.ProcessGetExpMestMedicine(this.GetExpMestMedicineByExpMestId(serviceReq.EXP_MEST_ID ?? 0));
                        //    this.ProcessGetExpMestMaterial(this.GetExpMestMaterialByExpMestId(serviceReq.EXP_MEST_ID ?? 0));
                        //}
                    }

                    this.ProcessGetServiceReqMety(this.GetServiceReqMetyByServiceReqId(serviceReq.ID), serviceReq, this.intructionTimeSelecteds.First());
                    Inventec.Common.Logging.LogSystem.Debug("ProcessChoicePrescriptionPrevious. 5");
                    this.ProcessGetServiceReqMaty(this.GetServiceReqMatyByServiceReqId(serviceReq.ID), false);
                    Inventec.Common.Logging.LogSystem.Debug("ProcessChoicePrescriptionPrevious. 6");
                }
                this.ProcessRemecountTutorial();
                this.ProcessInstructionTimeMediForEdit();
                this.ProcessMergeDuplicateRowForListProcessing();

                foreach (var item in this.mediMatyTypeADOs)
                {
                    if (!HIS.Desktop.Plugins.AssignPrescriptionYHCT.ValidAcinInteractiveWorker.Valid(item, mediMatyTypeADOs, this.LstExpMestMedicine))
                        this.mediMatyTypeADOs.Remove(item);
                }

                this.ProcessAddListRowDataIntoGridWithTakeBean();
                Inventec.Common.Logging.LogSystem.Debug("ProcessChoicePrescriptionPrevious. 7");
                //this.VerifyWarningOverCeiling();
                this.ReloadDataAvaiableMediBeanInCombo();
                this.ProcessMediStock(this.mediMatyTypeADOs);
                
                Inventec.Common.Logging.LogSystem.Debug("ProcessChoicePrescriptionPrevious. 8");
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessMediStock(List<MediMatyTypeADO> mediMatyTypeADOs)
        {
            try
            {
                foreach (var item in mediMatyTypeADOs)
                {
                    if (!item.MEDI_STOCK_ID.HasValue)
                        continue;
                    var stock = BackendDataWorker.Get<V_HIS_MEDI_STOCK>().FirstOrDefault(o=>o.ID == item.MEDI_STOCK_ID);
                    if(stock!=null && stock.IS_EXPEND == 1)
                    {
                        item.IsExpend = true;
                        item.IsDisableExpend = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetHuongDanFromSoLuongNgay()
        {
            try
            {
                if (isNotChangeTutorial)
                {
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => isNotChangeTutorial), isNotChangeTutorial));
                    return;
                }

                if (HisConfigCFG.TutorialFormat == 3)
                {
                    StringBuilder huongDan = new StringBuilder();
                    if (spinAmount.Value > 0 && !String.IsNullOrEmpty(txtLadder.Text))
                    {
                        string format__NgayUongTemp3 = ResourceMessage.NgayUongTemp3;
                        //Nếu key cấu hình có giá trị 3 thì xử lý điền hướng dẫn theo dạng: <số lượng> <đơn vị> * 1 thang * <số ngày>.
                        //Ví dụ: Thuốc thang A: 12g/thang, uống 5 thang: Trên xml thể hiện: 12g*1 thang*5 ngày.
                        decimal soLuongTrenlan = spinAmount.Value;
                        decimal soNgay = spinSoNgay.Value;
                        string serviceUnitName = this.currentMedicineTypeADOForEdit != null ? (!String.IsNullOrEmpty(this.currentMedicineTypeADOForEdit.CONVERT_UNIT_NAME) ? this.currentMedicineTypeADOForEdit.CONVERT_UNIT_NAME : (this.currentMedicineTypeADOForEdit.SERVICE_UNIT_NAME ?? "").ToLower()) : "";

                        huongDan.Append(soLuongTrenlan > 0 ? String.Format(format__NgayUongTemp3, Inventec.Common.Number.Convert.NumberToStringRoundAuto(soLuongTrenlan, 4), serviceUnitName, soNgay) : "");
                        string hdTemp = huongDan.ToString().Replace("  ", " ").Replace(", ,", ",");
                        txtHuongDan.Text = hdTemp;
                    }
                }
                else if (HisConfigCFG.TutorialFormat == 6)
                {
                    StringBuilder huongDan = new StringBuilder();
                    if (spinAmount.Value > 0 && !String.IsNullOrEmpty(txtLadder.Text))
                    {
                        string format__NgayUongTemp6 = "{0} {1} * {2} thang * {3} ngày";
                        //Nếu key cấu hình có giá trị 3 thì xử lý điền hướng dẫn theo dạng: <số lượng><đơn vị> * <số thang> * <số ngày>
                        decimal soLuongTrenlan = spinAmount.Value;
                        decimal soNgay = spinSoNgay.Value;
                        string serviceUnitName = this.currentMedicineTypeADOForEdit != null ? (!String.IsNullOrEmpty(this.currentMedicineTypeADOForEdit.CONVERT_UNIT_NAME) ? this.currentMedicineTypeADOForEdit.CONVERT_UNIT_NAME : (this.currentMedicineTypeADOForEdit.SERVICE_UNIT_NAME ?? "").ToLower()) : "";
                        long ladder = Inventec.Common.TypeConvert.Parse.ToInt64(txtLadder.Text);
                        huongDan.Append(soLuongTrenlan > 0 ? String.Format(format__NgayUongTemp6, Inventec.Common.Number.Convert.NumberToStringRoundAuto(soLuongTrenlan, 4), serviceUnitName, ladder, soNgay) : "");
                        string hdTemp = huongDan.ToString().Replace("  ", " ").Replace(", ,", ",");
                        txtHuongDan.Text = hdTemp;
                    }
                }
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => HisConfigCFG.TutorialFormat), HisConfigCFG.TutorialFormat)
                     + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => currentMedicineTypeADOForEdit), currentMedicineTypeADOForEdit)
                     + Inventec.Common.Logging.LogUtil.TraceData("spinAmount.Value", spinAmount.Value)
                     + Inventec.Common.Logging.LogUtil.TraceData("txtLadder.Text", txtLadder.Text)
                     + Inventec.Common.Logging.LogUtil.TraceData("txtHuongDan.Text", txtHuongDan.Text));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetHuongDanFromSoLuongNgayForOne(MediMatyTypeADO medicineTypeADOForEdit)
        {
            try
            {
                if (HisConfigCFG.TutorialFormat == 3)
                {
                    StringBuilder huongDan = new StringBuilder();
                    string format__NgayUongTemp3 = ResourceMessage.NgayUongTemp3;
                    //Nếu key cấu hình có giá trị 3 thì xử lý điền hướng dẫn theo dạng: <số lượng> <đơn vị> * 1 thang * <số ngày>.
                    //Ví dụ: Thuốc thang A: 12g/thang, uống 5 thang: Trên xml thể hiện: 12g*1 thang*5 ngày.
                    decimal soLuongTrenlan = (medicineTypeADOForEdit.AmountOneRemedy ?? 0);
                    decimal soNgay = (medicineTypeADOForEdit.UseDays ?? 0);
                    string serviceUnitName = !String.IsNullOrEmpty(medicineTypeADOForEdit.CONVERT_UNIT_NAME) ? medicineTypeADOForEdit.CONVERT_UNIT_NAME : (medicineTypeADOForEdit.SERVICE_UNIT_NAME ?? "").ToLower();

                    huongDan.Append(soLuongTrenlan > 0 ? String.Format(format__NgayUongTemp3, Inventec.Common.Number.Convert.NumberToStringRoundAuto(soLuongTrenlan, 4), serviceUnitName, soNgay) : "");

                    string hdTemp = huongDan.ToString().Replace("  ", " ").Replace(", ,", ",");
                    huongDan = new StringBuilder().Append(hdTemp.First().ToString().ToUpper() + String.Join("", hdTemp.Skip(1)).ToLower());

                    medicineTypeADOForEdit.TUTORIAL = huongDan.ToString();
                }
                else if (HisConfigCFG.TutorialFormat == 6)
                {
                    StringBuilder huongDan = new StringBuilder();

                    string format__NgayUongTemp6 = "{0} {1} * {2} thang * {3} ngày";
                    //Nếu key cấu hình có giá trị 3 thì xử lý điền hướng dẫn theo dạng: <số lượng><đơn vị> * <số thang> * <số ngày>
                    decimal soLuongTrenlan = (medicineTypeADOForEdit.AmountOneRemedy ?? 0);
                    decimal soNgay = (medicineTypeADOForEdit.UseDays ?? 0);
                    string serviceUnitName = !String.IsNullOrEmpty(medicineTypeADOForEdit.CONVERT_UNIT_NAME) ? medicineTypeADOForEdit.CONVERT_UNIT_NAME : (medicineTypeADOForEdit.SERVICE_UNIT_NAME ?? "").ToLower();
                    long ladder = medicineTypeADOForEdit.RemedyCount ?? 0;
                    huongDan.Append(soLuongTrenlan > 0 ? String.Format(format__NgayUongTemp6, Inventec.Common.Number.Convert.NumberToStringRoundAuto(soLuongTrenlan, 4), serviceUnitName, ladder, soNgay) : "");
                    medicineTypeADOForEdit.TUTORIAL = huongDan.ToString().Replace("  ", " ").Replace(", ,", ",");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        void ProcessRemecountTutorial()
        {
            try
            {
                this.isNotChangeTutorial = true;
                var remedyCountMax = this.mediMatyTypeADOs.Max(o => o.RemedyCount);
                if (remedyCountMax > 0)
                {
                    MediMatyTypeADO mediMatyTypeADO = this.mediMatyTypeADOs.FirstOrDefault(o => o.RemedyCount > 0);
                    txtLadder.Text = (remedyCountMax ?? 0).ToString();
                }
                var medimaty = this.mediMatyTypeADOs.FirstOrDefault(o => !String.IsNullOrEmpty(o.TUTORIAL));

                txtHuongDan.Text = (medimaty != null ? medimaty.TUTORIAL : "");
                this.isNotChangeTutorial = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
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

                LogSystem.Debug("__________currentSereServ " + currentSereServ + " __________currentSereServInEkip: " + currentSereServInEkip);
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
                this.txtAdvise.Text = HIS.Desktop.Utility.TextLibHelper.BytesToString(textLib.CONTENT);
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

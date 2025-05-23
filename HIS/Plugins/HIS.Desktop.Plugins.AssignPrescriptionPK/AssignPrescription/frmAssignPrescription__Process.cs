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
using DevExpress.XtraPrinting.Native;
using HIS.Desktop.ADO;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.BackendData.ADO;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.HisConfig;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.AssignPrescriptionPK.ADO;
using HIS.Desktop.Plugins.AssignPrescriptionPK.Config;
using HIS.Desktop.Plugins.AssignPrescriptionPK.Resources;
using HIS.Desktop.Plugins.AssignPrescriptionPK.Worker;
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
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.AssignPrescriptionPK.AssignPrescription
{
    public partial class frmAssignPrescription : HIS.Desktop.Utility.FormBase
    {
        private decimal totalPriceNotBHYT;

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

                UcDateReload(dateInputADO);

                Inventec.Common.Logging.LogSystem.Debug("ProcessAfterChangeTrackingTime.2");
                if (this.actionType == GlobalVariables.ActionView)
                {
                    //Trường hợp tạo/sửa tờ điều trị sau khi lưu đơn(nút lưu bị disable) ==> tự động gọi hàm lưu kê đơn để cập nhật lại ngày kê đơn theo ngày ở tờ điều trị
                    Inventec.Common.Logging.LogSystem.Debug("ProcessAfterChangeTrackingTime.3");
                    this.InitWorker();
                    this.actionType = GlobalVariables.ActionEdit;

                    this.ProcessSaveData(HIS.Desktop.Plugins.AssignPrescriptionPK.SAVETYPE.SAVE);//TODO cần check tiếp
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

                long useTime = (this.intructionTimeSelecteds != null && this.intructionTimeSelecteds.Count > 0) ? this.intructionTimeSelecteds.OrderBy(o => o).First() : 0;
                if (HisConfigCFG.IsShowServerTimeByDefault && dteCommonParam != null && dteCommonParam != DateTime.MinValue)
                    result.UseTime = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dteCommonParam);
                else if (dteTreatmentFinishIntructionTime > 0)
                {
                    result.UseTime = dteTreatmentFinishIntructionTime;
                }
                result.IntructionTime = useTime;
                if (this.currentTreatmentWithPatientType != null)
                {
                    if (!string.IsNullOrEmpty(this.currentTreatmentWithPatientType.SHOW_ICD_CODE)
                        || !string.IsNullOrEmpty(this.currentTreatmentWithPatientType.SHOW_ICD_NAME)
                         || !string.IsNullOrEmpty(this.currentTreatmentWithPatientType.SHOW_ICD_SUB_CODE)
                         || !string.IsNullOrEmpty(this.currentTreatmentWithPatientType.SHOW_ICD_TEXT)
                        )
                    {
                        result.icdCode = this.currentTreatmentWithPatientType.SHOW_ICD_CODE;
                        result.icdName = this.currentTreatmentWithPatientType.SHOW_ICD_NAME;
                        result.icdSubCode = this.currentTreatmentWithPatientType.SHOW_ICD_SUB_CODE;
                        result.icdText = this.currentTreatmentWithPatientType.SHOW_ICD_TEXT;
                    }
                    else
                    {
                        result.icdCode = this.currentTreatmentWithPatientType.ICD_CODE;
                        result.icdName = this.currentTreatmentWithPatientType.ICD_NAME;
                        result.icdSubCode = this.currentTreatmentWithPatientType.ICD_SUB_CODE;
                        result.icdText = this.currentTreatmentWithPatientType.ICD_TEXT;
                    }
                    result.patientId = currentTreatmentWithPatientType.PATIENT_ID;
                }
                result.IsBhyt = !String.IsNullOrEmpty(this.currentHisPatientTypeAlter.HEIN_CARD_NUMBER);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

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

        private bool ProcessCheckContraindicaterWarningOptionAfterChoose()
        {
            bool result = true;
            try
            {
                result = CheckICDServiceForContraindicaterWarningOption(this.mediMatyTypeADOs);
                if (result == false)
                {
                    this.mediMatyTypeADOs = null;
                    this.gridControlServiceProcess.DataSource = this.mediMatyTypeADOs;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private bool ProcessCheckAllergenicByPatientAfterChoose()
        {
            bool result = true;
            try
            {
                if (allergenics == null || allergenics.Count == 0)
                    return result;

                string messageTitle__IS_SURE = "";
                string messageTitle__IS_DOUBT = "";
                string messageError__IS_SURE = "";
                string messageError__IS_DOUBT = "";
                foreach (var item in this.mediMatyTypeADOs)
                {
                    if (item.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC)
                    {
                        HIS_ALLERGENIC allergencic = allergenics.FirstOrDefault(o => o.MEDICINE_TYPE_ID == item.ID);
                        if (allergencic != null)
                        {
                            if (allergencic.IS_SURE == 1)
                            {
                                messageTitle__IS_SURE = "Cảnh báo dị ứng thuốc:";
                                messageError__IS_SURE += Inventec.Desktop.Common.HtmlString.ProcessorString.InsertSpacialTag(String.Format("- {0}: Biểu hiện lâm sàng ({1}).", item.MEDICINE_TYPE_NAME, allergencic.CLINICAL_EXPRESSION), Inventec.Desktop.Common.HtmlString.SpacialTag.Tag.Br);
                            }
                            else if (allergencic.IS_DOUBT == 1)
                            {
                                messageTitle__IS_DOUBT = "Cảnh báo NGHI NGỜ dị ứng thuốc:";
                                messageError__IS_DOUBT += Inventec.Desktop.Common.HtmlString.ProcessorString.InsertSpacialTag(String.Format("- {0}: Biểu hiện lâm sàng ({1}).", item.MEDICINE_TYPE_NAME, allergencic.CLINICAL_EXPRESSION), Inventec.Desktop.Common.HtmlString.SpacialTag.Tag.Br);
                            }
                        }
                    }
                }

                if (!String.IsNullOrEmpty(messageError__IS_SURE) || !String.IsNullOrEmpty(messageError__IS_DOUBT))
                {
                    DialogResult myResult;
                    string message = "";
                    if (!String.IsNullOrEmpty(messageError__IS_SURE))
                    {
                        message += Inventec.Desktop.Common.HtmlString.ProcessorString.InsertSpacialTag(String.Format("{0}{1}.", Inventec.Desktop.Common.HtmlString.ProcessorString.InsertSpacialTag(messageTitle__IS_SURE, Inventec.Desktop.Common.HtmlString.SpacialTag.Tag.Br), messageError__IS_SURE), Inventec.Desktop.Common.HtmlString.SpacialTag.Tag.Br);
                    }
                    if (!String.IsNullOrEmpty(messageError__IS_DOUBT))
                    {
                        message += Inventec.Desktop.Common.HtmlString.ProcessorString.InsertSpacialTag(String.Format("{0}{1}.", Inventec.Desktop.Common.HtmlString.ProcessorString.InsertSpacialTag(messageTitle__IS_DOUBT, Inventec.Desktop.Common.HtmlString.SpacialTag.Tag.Br), messageError__IS_DOUBT), Inventec.Desktop.Common.HtmlString.SpacialTag.Tag.Br);
                    }
                    message += "Bạn có muốn tiếp tục?";
                    myResult = XtraMessageBox.Show(message, "Thông báo", MessageBoxButtons.OKCancel, MessageBoxIcon.Question, DevExpress.Utils.DefaultBoolean.True);
                    if (myResult != DialogResult.OK)
                    {
                        result = false;
                        Inventec.Common.Logging.LogSystem.Debug(message + "___nguoi dung chon khong tiep tuc");
                    }
                }

                if (result == false)
                {
                    this.mediMatyTypeADOs = null;
                    this.gridControlServiceProcess.DataSource = this.mediMatyTypeADOs;
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
                                            && o.TUTORIAL == item.TUTORIAL
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
                                        && o.TUTORIAL == item.TUTORIAL
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
                Inventec.Common.Logging.LogSystem.Debug("RefeshResourceGridMedicine.1");
                this.ProcessMediStock(this.mediMatyTypeADOs);
                this.ProcessAddListMaxInTreat(this.mediMatyTypeADOs);
                this.gridControlServiceProcess.DataSource = null;
                this.gridControlServiceProcess.DataSource = this.mediMatyTypeADOs.OrderBy(o => o.NUM_ORDER).ToList();
                Inventec.Common.Logging.LogSystem.Debug("RefeshResourceGridMedicine.2");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void ProcessAddListMaxInTreat(List<MediMatyTypeADO> mediMatyTypeADOs)
        {

            try
            {
                foreach (var item in mediMatyTypeADOs)
                {
                    if (item.ALERT_MAX_IN_TREATMENT != null && !string.IsNullOrEmpty(item.EXCEED_LIMIT_IN_BATCH_REASON))
                    {
                        item.IsAlertInTreatPresciption = true;
                        var alert = mediMatyTypeADOsAlertInTreatment.LastOrDefault(o => o.PrimaryKey == item.PrimaryKey);
                        if (alert == null)
                        {
                            mediMatyTypeADOsAlertInTreatment.Add(item);
                        }
                        else
                        {
                            mediMatyTypeADOsAlertInTreatment.Remove(alert);//Xóa thông tin cũ
                            mediMatyTypeADOsAlertInTreatment.Add(item);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
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
                                        && o.TUTORIAL == item.TUTORIAL
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
                this.UpdateAutoRoundUpByConvertUnitRatioDataMediMaty();
                bool IsRemoveMediMatyAmountAlert = false;
                if (this.ProcessValidMedicineTypeAge(true) && this.ProcessAmountInStockWarning(ref IsRemoveMediMatyAmountAlert))
                {
                    if (IsRemoveMediMatyAmountAlert)
                        mediMatyTypeADOs = mediMatyTypeADOs.Where(o => o.AmountAlert == null || o.AmountAlert == 0).ToList();
                    var mediMatyTypeADOsForTakeBeans__Other = (this.mediMatyTypeADOs.Where(o => o.DataType != HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC && o.DataType != HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.VATTU && o.DataType != HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.VATTU_TSD).ToList());
                    if (mediMatyTypeADOsForTakeBeans__Other != null && mediMatyTypeADOsForTakeBeans__Other.Count > 0)
                        mediMatyTypeADOsTemp.AddRange(mediMatyTypeADOsForTakeBeans__Other);

                    var mediMatyTypeADOsForTakeBeans__Medicine = (this.mediMatyTypeADOs.Where(o => o.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC && (o.AmountAlert ?? 0) == 0).ToList());

                    if (mediMatyTypeADOsForTakeBeans__Medicine != null && mediMatyTypeADOsForTakeBeans__Medicine.Count > 0)
                    {
                        if (!GlobalStore.IsTreatmentIn || GlobalStore.IsCabinet)
                        {
                            if (TakeOrReleaseBeanWorker.ProcessTakeListMedi(this.intructionTimeSelecteds, mediMatyTypeADOsForTakeBeans__Medicine, this.serviceReqMain, this.UseTimeSelecteds, this.lstOutPatientPres))
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
                            CommonParam Param = new CommonParam();
                            if (TakeOrReleaseBeanWorker.ProcessTakeListMaty(this.intructionTimeSelecteds, mediMatyTypeADOsForTakeBeans__Material, this.serviceReqMain, this.UseTimeSelecteds, this.lstOutPatientPres, ref Param))
                            {
                                mediMatyTypeADOsTemp.AddRange(mediMatyTypeADOsForTakeBeans__Material);
                            }
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
                            if (TakeOrReleaseBeanWorker.ProcessTakeListMatyTSD(this.intructionTimeSelecteds, mediMatyTypeADOsForTakeBeans__MaterialTSD, this.serviceReqMain, this.UseTimeSelecteds, this.lstOutPatientPres))
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

                List<MediMatyTypeADO> mediMatycheck = new List<MediMatyTypeADO>();
                foreach (var item in mediMatyTypeADOsTemp)
                {
                    FillDataOtherPaySourceDataRow(item);
                    UpdateExpMestReasonInDataRow(item);
                    if (ValidAcinInteractiveWorker.ValidGrade(item, mediMatycheck, ref txtInteractionReason, this))
                    {
                        mediMatycheck.Add(item);
                    }
                }

                mediMatyTypeADOsTemp = mediMatycheck;

                //Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => mediMatyTypeADOs), mediMatyTypeADOs));
                this.mediMatyTypeADOs = mediMatyTypeADOsTemp.OrderBy(o => o.NUM_ORDER).ToList();
                if (CheckMediMatyType(this.mediMatyTypeADOs) == false)
                {
                    return;
                }
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
        private bool ProcessAmountInStockWarning(ref bool IsRemoveMediMatyAmountAlert)
        {
            bool result = true;
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("ProcessAmountInStockWarning.1");
                string message = "";
                var mediMatyTypeInStockWarnings = this.mediMatyTypeADOs.Where(o => (o.AmountAlert ?? 0) > 0).ToList();
                if (mediMatyTypeInStockWarnings != null && mediMatyTypeInStockWarnings.Count > 0)
                {
                    foreach (var item in mediMatyTypeInStockWarnings)
                    {
                        message += (Inventec.Desktop.Common.HtmlString.ProcessorString.InsertColor(item.MEDICINE_TYPE_NAME, System.Drawing.Color.Red)
                            + " : " + Inventec.Desktop.Common.HtmlString.ProcessorString.InsertColor(Inventec.Common.Number.Convert.NumberToString((item.AmountAlert ?? 0), ConfigApplications.NumberSeperator), System.Drawing.Color.Maroon) + "; ");
                    }
                    MessageBoxButtons mesButton;
                    if (HisConfigCFG.IsExceedAvailableOutStock)
                    {
                        message += ResourceMessage.VuotQuaSoLuongKhaDungTrongKho;
                        mesButton = MessageBoxButtons.OK;
                    }
                    else
                    {
                        message += ResourceMessage.VuotQuaSoLuongKhaDungTrongKho__CoMuonTiepTuc;
                        mesButton = MessageBoxButtons.YesNoCancel;
                    }

                    DialogResult dialogResult = DevExpress.XtraEditors.XtraMessageBox.Show(message, Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaCanhBao), mesButton, MessageBoxIcon.Warning, DevExpress.Utils.DefaultBoolean.True);
                    Inventec.Common.Logging.LogSystem.Debug("ProcessAmountInStockWarning.2");
                    if (dialogResult == DialogResult.Yes)
                    {
                        //Chuyển các thuốc hết khả dụng sang thuốc ngoài kho
                        foreach (var item in mediMatyTypeInStockWarnings)
                        {
                            var medimatyWorker = this.mediMatyTypeADOs.FirstOrDefault(o => o.SERVICE_ID == item.SERVICE_ID);
                            medimatyWorker.PATIENT_TYPE_ID = null;
                            medimatyWorker.PATIENT_TYPE_CODE = null;
                            medimatyWorker.PATIENT_TYPE_NAME = null;
                            medimatyWorker.DataType = (item.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC ? HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC_DM : HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.VATTU_DM);
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
                        Inventec.Common.Logging.LogSystem.Debug("ProcessAmountInStockWarning.3");
                    }
                    else if (dialogResult == DialogResult.No || dialogResult == DialogResult.OK)
                    {
                        IsRemoveMediMatyAmountAlert = true;
                        //this.mediMatyTypeADOs = new List<MediMatyTypeADO>();
                        //this.gridControlServiceProcess.DataSource = null;
                        //result = false;
                        Inventec.Common.Logging.LogSystem.Debug("ProcessAmountInStockWarning.4");
                    }
                    else if (dialogResult == DialogResult.Cancel)
                    {
                        this.mediMatyTypeADOs = new List<MediMatyTypeADO>();
                        this.gridControlServiceProcess.DataSource = null;
                        result = false;
                        Inventec.Common.Logging.LogSystem.Debug("ProcessAmountInStockWarning.5");
                    }
                    else
                    {
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
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => currentMedicineTypeADOForEdit), currentMedicineTypeADOForEdit) + "____" + Inventec.Common.Logging.LogUtil.TraceData("IS_EXECUTE_KIDNEY_PRES", (this.oldServiceReq != null ? this.oldServiceReq.IS_EXECUTE_KIDNEY_PRES : null)));
                var selectedOpionGroup = GetSelectedOpionGroup();
                if (selectedOpionGroup == 1 && this.currentMedicineTypeADOForEdit != null && this.currentMedicineTypeADOForEdit.DataType != HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC_DM && this.currentMedicineTypeADOForEdit.DataType != HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.VATTU_DM
                    && this.currentMedicineTypeADOForEdit.DataType != HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.VATTU_TSD)
                {
                    dataType = (this.currentMedicineTypeADOForEdit.IS_KIDNEY == 1
                        && chkPreKidneyShift.Checked
                        && (this.oldServiceReq == null || (this.oldServiceReq != null && this.oldServiceReq.IS_EXECUTE_KIDNEY_PRES != GlobalVariables.CommonNumberTrue))
                        ) ? HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC_DM : (this.currentMedicineTypeADOForEdit.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC ?
                        HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC : HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.VATTU);
                }
                else if (selectedOpionGroup == 3)
                {
                    //TODO
                    dataType = HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.VATTU_TSD;
                }
                else
                {
                    //Thuốc - vật tư tự mua
                    if (!String.IsNullOrEmpty(this.txtMedicineTypeOther.Text.Trim()))
                        dataType = HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC_TUTUC;
                    //Thuốc - Vật tư DM
                    else if (currentMedicineTypeADOForEdit != null && this.currentMedicineTypeADOForEdit.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC)
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
                var selectedOpionGroup = GetSelectedOpionGroup();
                if (!String.IsNullOrEmpty(this.txtMedicineTypeOther.Text.Trim()) && selectedOpionGroup != 1)
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
                if (this.currentMedicineTypeADOForEdit != null)
                {
                    this.currentMedicineTypeADOForEdit.DataType = datatype;
                }

                HIS.Desktop.Plugins.AssignPrescriptionPK.Edit.IEdit iEdit = HIS.Desktop.Plugins.AssignPrescriptionPK.Edit.EditFactory.MakeIEdit(
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
                long serviceId = 0;
                long index = 0;
                if (currentMedicineTypeADOForEdit != null)
                {
                    serviceId = currentMedicineTypeADOForEdit.SERVICE_ID;
                    index = currentMedicineTypeADOForEdit.IdRow;
                }
                int datatype = GetDataTypeSelected();
                HIS.Desktop.Plugins.AssignPrescriptionPK.Add.IAdd iAdd = HIS.Desktop.Plugins.AssignPrescriptionPK.Add.AddFactory.MakeIAdd(
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
                    else
                    {
                        if (serviceId > 0)
                            GetServiceTick(serviceId, index);
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

        private void TickAllMediMateAssignPresed()
        {
            try
            {
                List<long> serviceIds = null;
                dicMediMateAssignPres = new Dictionary<string, long>();
                if (mediMatyTypeADOs != null && mediMatyTypeADOs.Count > 0)
                {
                    serviceIds = mediMatyTypeADOs.Select(o => o.SERVICE_ID).ToList();
                    if (mediMateTypeComboADOs != null && mediMateTypeComboADOs.Count > 0)
                    {
                        foreach (var item in mediMateTypeComboADOs)
                        {
                            var key = "MedicineMaterialTypeComboADO_" + item.IdRow;
                            if (serviceIds.Exists(o => o == item.SERVICE_ID))
                            {
                                item.IsExistAssignPres = true;
                                if (!dicMediMateAssignPres.ContainsKey(key))
                                {
                                    dicMediMateAssignPres[key] = item.SERVICE_ID;
                                }
                            }
                        }
                    }
                    if (mediStockD1ADOs != null && mediStockD1ADOs.Count > 0)
                    {
                        foreach (var item in mediStockD1ADOs)
                        {
                            var key = "DMediStock1ADO_" + item.IdRow;
                            if (serviceIds.Exists(o => o == item.SERVICE_ID))
                            {
                                item.IsExistAssignPres = true;
                                if (!dicMediMateAssignPres.ContainsKey(key))
                                {
                                    dicMediMateAssignPres[key] = item.SERVICE_ID ?? 0;
                                }
                            }
                        }
                    }
                    gridViewMediMaty.GridControl.RefreshDataSource();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void GetServiceTick(long serviceId, long index, bool IsRemove = false)
        {
            try
            {
                if (mediMateTypeComboADOs != null && mediMateTypeComboADOs.Count > 0)
                {
                    if (!dicMediMateAssignPres.ContainsKey("MedicineMaterialTypeComboADO_" + index) && !IsRemove)
                        dicMediMateAssignPres["MedicineMaterialTypeComboADO_" + index] = serviceId;
                    else if (IsRemove)
                        dicMediMateAssignPres.Remove("MedicineMaterialTypeComboADO_" + index);
                }
                if (mediStockD1ADOs != null && mediStockD1ADOs.Count > 0)
                {
                    if (!dicMediMateAssignPres.ContainsKey("DMediStock1ADO_" + index) && !IsRemove)
                        dicMediMateAssignPres["DMediStock1ADO_" + index] = serviceId;
                    else if (IsRemove)
                        dicMediMateAssignPres.Remove("DMediStock1ADO_" + index);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void popupControlContainerMediMaty_Popup(object sender, EventArgs e)
        {
            TickIsAssignPres();
        }

        private void TickIsAssignPres()
        {
            try
            {
                if (gridViewMediMaty.GridControl.DataSource is List<MedicineMaterialTypeComboADO> && mediMateTypeComboADOs != null && mediMateTypeComboADOs.Count > 0)
                {
                    var count = 0;
                    int total = dicMediMateAssignPres.Where(o => o.Key.Contains("MedicineMaterialTypeComboADO_")).GroupBy(o => o.Key).Count();
                    if (total > 0)
                    {
                        foreach (var item in mediMateTypeComboADOs)
                        {
                            var key = "MedicineMaterialTypeComboADO_" + item.IdRow;
                            if (dicMediMateAssignPres.ContainsKey(key) && dicMediMateAssignPres[key] == item.SERVICE_ID)
                            {
                                item.IsExistAssignPres = true;
                                count++;
                            }
                            if (count == total)
                                break;
                        }
                    }
                    else
                        mediMateTypeComboADOs.ForEach(o => o.IsExistAssignPres = false);

                }
                if (gridViewMediMaty.GridControl.DataSource is List<DMediStock1ADO> && mediStockD1ADOs != null && mediStockD1ADOs.Count > 0)
                {
                    var count = 0;
                    int total = dicMediMateAssignPres.Where(o => o.Key.Contains("DMediStock1ADO_")).GroupBy(o => o.Key).Count();
                    if (total > 0)
                    {
                        foreach (var item in mediStockD1ADOs)
                        {
                            var key = "DMediStock1ADO_" + item.IdRow;
                            if (dicMediMateAssignPres.ContainsKey(key) && dicMediMateAssignPres[key] == item.SERVICE_ID)
                            {
                                item.IsExistAssignPres = true;
                                count++;
                            }
                            if (count == total)
                                break;
                        }
                    }
                    else
                        mediStockD1ADOs.ForEach(o => o.IsExistAssignPres = false);
                }
                gridViewMediMaty.GridControl.RefreshDataSource();
            }
            catch (Exception ex)
            {
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
                var selectedOpionGroup = GetSelectedOpionGroup();

                if (selectedOpionGroup == 1)
                {
                    this.theRequiredWidth = 1030;
                    this.theRequiredHeight = 200;
                    this.RebuildMediMatyWithInControlContainer();
                }
                else if (selectedOpionGroup == 3)
                {
                    this.theRequiredWidth = 1030;
                    this.theRequiredHeight = 200;
                    this.RebuildMediMatyWithInControlContainer(true);
                }

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
                //if (this.intructionTimeSelecteds == null || this.intructionTimeSelecteds.Count == 0)
                //    this.intructionTimeSelecteds = this.ucDateProcessor.GetValue(this.ucDate);

                UC.DateEditor.ADO.DateInputADO dateInputADO = new UC.DateEditor.ADO.DateInputADO();
                dateInputADO.Time = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(this.intructionTimeSelecteds.First()) ?? new DateTime();
                dateInputADO.Dates = new List<DateTime?>();
                dateInputADO.Dates.Add(dateInputADO.Time);

                var selectedOpionGroup = GetSelectedOpionGroup();

                if (selectedOpionGroup == 1)
                {
                    spinAmount.Enabled =
                    txtMediMatyForPrescription.Enabled
                        = spinSoLuongNgay.Enabled
                        = spinSang.Enabled
                        = spinTrua.Enabled
                        = spinChieu.Enabled
                        = spinToi.Enabled
                        = spinTocDoTruyen.Enabled
                        = txtTutorial.Enabled
                        = cboHtu.Enabled
                        = txtPreviousUseDay.Enabled = true;

                    txtMedicineTypeOther.Enabled = spinPrice.Enabled = txtUnitOther.Enabled = false;

                    dateInputADO.IsVisibleMultiDate = true;
                }
                else if (selectedOpionGroup == 2)
                {
                    spinAmount.Enabled =
                    txtMediMatyForPrescription.Enabled
                        = spinSoLuongNgay.Enabled
                        = spinSang.Enabled
                        = spinTrua.Enabled
                        = spinChieu.Enabled
                        = spinToi.Enabled
                        = spinTocDoTruyen.Enabled
                        = txtTutorial.Enabled
                        = cboHtu.Enabled
                        = txtPreviousUseDay.Enabled = true;

                    txtMedicineTypeOther.Enabled = spinPrice.Enabled = txtUnitOther.Enabled = true;

                    dateInputADO.IsVisibleMultiDate = true;
                }
                else if (selectedOpionGroup == 3)
                {
                    txtMediMatyForPrescription.Enabled = true;
                    spinAmount.Enabled
                        = spinSoLuongNgay.Enabled
                        = spinSang.Enabled
                        = spinTrua.Enabled
                        = spinChieu.Enabled
                        = spinToi.Enabled
                        = spinTocDoTruyen.Enabled
                        = txtTutorial.Enabled
                        = cboHtu.Enabled
                        = txtPreviousUseDay.Enabled = false;
                    txtMedicineTypeOther.Enabled = spinPrice.Enabled = txtUnitOther.Enabled = false;

                    dateInputADO.IsVisibleMultiDate = false;
                }

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

        internal void ReSetChongCHiDinhInfoControl__MedicinePage()
        {
            this.txtThongTinChongChiDinhThuoc.Text = "";
        }

        internal void ReSetDataInputAfterAdd__MedicinePage()
        {
            try
            {
                this.cboMedicineUseForm.EditValue = null;
                this.DataHtuList.ForEach(o => o.IsChecked = false);
                this.cboHtu.Text = null;
                this.cboHtu.Properties.Buttons[1].Visible = false;
                this.spinSang.EditValue = null;
                this.spinTrua.EditValue = null;
                this.spinChieu.EditValue = null;
                this.spinToi.EditValue = null;
                this.spinAmount.Text = "";
                this.spinTocDoTruyen.EditValue = null;
                this.lciTocDoTruyen.Enabled = true;
                this.txtTocDoTho.Text = "";
                this.txtThoiGianTho.Text = "";
                this.txtMedicineTypeOther.Text = "";
                this.txtUnitOther.Text = "";
                this.spinPrice.EditValue = null;
                this.btnAddTutorial.Enabled = false;
                this.spinSoLuongNgay.Text = "";
                this.txtPreviousUseDay.Text = "";
                if (String.IsNullOrEmpty(txtLadder.Text))
                    this.txtTutorial.Text = "";
                this.memHtu.Text = "";
                this.VisibleInputControl(true);

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
                    //if (this.ucDateProcessor != null && this.ucDate != null)
                    //    this.ucDateProcessor.ReadOnly(this.ucDate, true);
                }
                else
                {
                    this.btnSave.Enabled = this.btnAdd.Enabled = btnSaveAndPrint.Enabled = false;
                    this.lciPrintAssignPrescription.Enabled = true;
                    if (this.assignPrescriptionEditADO == null)
                        this.btnNew.Enabled = true;
                    this.gridViewServiceProcess.OptionsBehavior.Editable = false;

                    //Nếu đã lưu đơn thuốc hoặc sửa đơn thuốc thì không cho chọn nhiều ngày
                    //if (this.ucDateProcessor != null && this.ucDate != null)
                    //    this.ucDateProcessor.ReadOnly(this.ucDate, true);

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

        private async Task InitDataByExpMestTemplate()
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

        private void ProcessAfterChooseformMediStockForServicePackage(List<MOS.EFMODEL.DataModels.V_HIS_MEST_ROOM> selectedMestRooms)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("ProcessAfterChooseformMediStockForServicePackage. 1");
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

                this.OpionGroupSelectedChanged();

                //Load data for material in package
                this.LoadPageMateMetyInServicePackage(this.serviceInPackages);
                this.ProcessInstructionTimeMediForEdit();
                if (this.ProcessCheckAllergenicByPatientAfterChoose()
                    && this.ProcessCheckContraindicaterWarningOptionAfterChoose())
                {
                    this.ProcessMergeDuplicateRowForListProcessing();
                    this.ProcessAddListRowDataIntoGridWithTakeBean();
                    this.ReloadDataAvaiableMediBeanInCombo();

                    //Kiểm tra trần hao phí của dịch vụ chính & các thuốc/vật tư đã kê, đưa ra cảnh báo nếu vượt trần
                    this.AlertOutofMaxExpend();
                }

                Inventec.Common.Logging.LogSystem.Debug("ProcessAfterChooseformMediStockForServicePackage. 2");
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

                this.OpionGroupSelectedChanged();

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

        private async Task InitDataByServiceMetyMaty()
        {
            try
            {
                if (GlobalStore.IsCabinet || GlobalStore.IsTreatmentIn)
                {

                    if (actionType == GlobalVariables.ActionAdd)
                    {
                        if (this.currentSereServ != null || this.currentSereServInEkip != null)
                        {
                            LogSystem.Debug("Loaded ProcessAfterChooseformMediStockForServiceMetyMaty. 1");
                            if (this.currentSereServ != null)
                            {
                                CreateThreadLoadDataByServiceMetyMaty(currentSereServ);
                            }

                            if (this.currentSereServInEkip != null)
                            {
                                CreateThreadLoadDataByServiceMetyMaty(currentSereServInEkip);
                            }
                            LogSystem.Debug("Loaded ProcessAfterChooseformMediStockForServiceMetyMaty. 2");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private async Task InitDataByServicePackage()
        {
            try
            {
                if (actionType == GlobalVariables.ActionAdd)
                {
                    if (this.currentSereServ != null || this.currentSereServInEkip != null)
                    {

                        var serviceLeaf__Medicine_Materials = (
                              from m in lstService
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
                    //TODO cần check lại
                    this.repositoryItemSpinAmount__MedicinePage.BeginUpdate();
                    if (HisConfigCFG.AmountDecimalNumber > 0)
                    {
                        numberDisplaySeperateFormatAmount = HisConfigCFG.AmountDecimalNumber;
                    }
                    else
                    {
                        numberDisplaySeperateFormatAmount = 2;
                    }

                    this.repositoryItemSpinAmount__MedicinePage.DisplayFormat.FormatString = "0.######";
                    this.repositoryItemSpinAmount__MedicinePage.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
                    this.repositoryItemSpinAmount__MedicinePage.EditFormat.FormatString = "0.######";
                    this.repositoryItemSpinAmount__MedicinePage.EditFormat.FormatType = DevExpress.Utils.FormatType.Numeric;

                    this.repositoryItemSpinAmount__MedicinePage.Properties.Mask.EditMask = "";
                    this.repositoryItemSpinAmount__MedicinePage.Properties.Mask.UseMaskAsDisplayFormat = false;
                }
                else
                {
                    numberDisplaySeperateFormatAmount = 0;

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
                spinSoNgay.EditValue = 1;
                spinSoLuongNgay.EditValue = 1;
                spinTocDoTruyen.EditValue = null;
                lciTocDoTruyen.Visibility = (GlobalStore.IsTreatmentIn && !GlobalStore.IsCabinet) ? DevExpress.XtraLayout.Utils.LayoutVisibility.Always : DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                grcTocDoTruyen.Visible = (GlobalStore.IsTreatmentIn && !GlobalStore.IsCabinet) ? true : false;
                lciHtu.TextSize = (GlobalStore.IsTreatmentIn && !GlobalStore.IsCabinet) ? new System.Drawing.Size(60, lciTocDoTruyen.Height) : new System.Drawing.Size(90, lciTocDoTruyen.Height);
                repositoryItemchkIsExpendType_Disable.Enabled = false;
                repositoryItemchkIsExpendType_Disable.ReadOnly = true;
                repositoryItemChkIsExpend__MedicinePage_Disable.Enabled = false;
                repositoryItemChkIsExpend__MedicinePage_Disable.ReadOnly = true;
                lciHomePres.Visibility = (GlobalStore.IsTreatmentIn && !GlobalStore.IsCabinet) ? DevExpress.XtraLayout.Utils.LayoutVisibility.Always : DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                lciForchkThongTinMat.Visibility = (GlobalStore.IsTreatmentIn && !GlobalStore.IsCabinet) ? DevExpress.XtraLayout.Utils.LayoutVisibility.Always : DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                lciForchkPreKidneyShift.Visibility = (GlobalStore.IsTreatmentIn && !GlobalStore.IsCabinet) ? DevExpress.XtraLayout.Utils.LayoutVisibility.Always : DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                lciForspinKidneyCount.Visibility = (GlobalStore.IsTreatmentIn && !GlobalStore.IsCabinet) ? DevExpress.XtraLayout.Utils.LayoutVisibility.Always : DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                lciForchkShowLo.Enabled = HisConfigCFG.IsAllowAssignPresByPackage ? true : false;

                lciForpnlUCDateForMedi.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                emptySpaceItem4.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                emptySpaceItem8.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                layoutControlItem16.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;

                gridColumnManyDate.Visible = (HisConfigCFG.ManyDayPrescriptionOption == 2 && (GlobalStore.IsTreatmentIn && !GlobalStore.IsCabinet));
                IsHandlerWhileOpionGroupSelectedIndexChanged = this.assignPrescriptionEditADO != null && this.assignPrescriptionEditADO.ServiceReq != null && this.assignPrescriptionEditADO.ServiceReq.ID > 0;

                dtExecuteTime.EditValue = DateTime.Now;
                spinBloodPressureMax.EditValue = null;
                spinBloodPressureMin.EditValue = null;
                spinBreathRate.EditValue = null;
                spinHeight.EditValue = null;
                spinChest.EditValue = null;
                spinBelly.EditValue = null;
                spinPulse.EditValue = null;
                spinTemperature.EditValue = null;
                spinWeight.EditValue = null;
                spinSPO2.EditValue = null;
                txtNote.Text = "";
                lblIsToCalculateEgfr.Text = "";
                lblBMI.Text = "";
                lblLeatherArea.Text = "";
                //lblBmiDisplayText.Text = "";
                txtThongTinChongChiDinhThuoc.AllowHtmlString = true;

                BackendDataWorker.Reset<HIS_ATC>();
                BackendDataWorker.Reset<HIS_CONTRAINDICATION>();
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
                        Inventec.Common.Logging.LogSystem.Debug("SetDefaultData => kiem tra co cau hình CONFIG_KEY__HIS_DESKTOP__ASSIGN_PRESCRIPTION__DEFAULT_NUM_OF_DAY= " + numOfDay + ", lay gan gia tri vao spinSoNgay ");
                    }
                }

                this.btnCreateVBA.Enabled = false;

                this.lblPhatSinh.Text = "";
                this.lblPhatSinh__BHYT.Text = "";
                this.lblPhatSinh__KhacBHYT.Text = "";
                this.lblPhatSinh__MuaNgoai.Text = "";
                this.idRow = 1;

                this.btnSave.Enabled = false;
                this.btnSaveAndPrint.Enabled = false;
                this.lciPrintAssignPrescription.Enabled = false;
                this.btnAdd.Enabled = false;
                this.btnAddTutorial.Enabled = false;

                this.txtLadder.Text = "";
                this.txtAdvise.Text = "";

                this.spinSoNgay.Enabled = true;
                this.spinTocDoTruyen.EditValue = null;
                this.chkHomePres.Checked = false;
                this.chkTemporayPres.Checked = false;
                this.spnPresPhaseNum.EditValue = null;
                this.txtProvisionalDiagnosis.Text = this.provisionalDiagnosis;
                this.txtPreviousUseDay.Text = "";

                this.actionType = (this.assignPrescriptionEditADO != null ? GlobalVariables.ActionEdit : GlobalVariables.ActionAdd);
                this.actionBosung = GlobalVariables.ActionAdd;

                this.gridControlServiceProcess.DataSource = null;
                this.mediMatyTypeADOs = new List<MediMatyTypeADO>();
                this.cboExpMestTemplate.EditValue = null;
                this.cboExpMestTemplate.Properties.Buttons[1].Visible = false;
                this.txtExpMestTemplateCode.Text = "";
                this.spinKidneyCount.EditValue = null;
                this.chkPreKidneyShift.Checked = false;
                txtInteractionReason.Enabled = false;

                this.ResetComboNhaThuoc();

                GlobalStore.ClientSessionKey = Guid.NewGuid().ToString();
                this.currentPatientTypes = BackendDataWorker.Get<HIS_PATIENT_TYPE>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
                this.currentMedicineTypes = BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>().Where(o => o.IS_LEAF == GlobalVariables.CommonNumberTrue).ToList();
                long isOnlyDisplayMediMateIsBusiness = Inventec.Common.TypeConvert.Parse.ToInt64(HisConfigs.Get<string>(HisConfigCFG.ONLY_DISPLAY_MEDIMATE_IS_BUSINESS));
                if (isOnlyDisplayMediMateIsBusiness == 1 && this.currentMedicineTypes != null && this.currentMedicineTypes.Count > 0)
                    this.currentMedicineTypes = this.currentMedicineTypes.Where(o => o.IS_BUSINESS.HasValue && o.IS_BUSINESS.Value == 1).ToList();

                this.currentMaterialTypes = BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>().Where(o => o.IS_LEAF == GlobalVariables.CommonNumberTrue).ToList();
                GlobalStore.HisMestMetyUnit = BackendDataWorker.Get<HIS_MEST_METY_UNIT>();

                this.InitServiceConditionData();

                Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(this.dxValidationProviderControl, dxErrorProvider1);

                this.serviceReqMetyInDay = null;
                this.serviceReqMatyInDay = null;
                this.ObeyContraindiSave = new List<HIS_OBEY_CONTRAINDI>();
                this.resultDataPrescription = null;

                //Ẩn số thang theo cấu hình
                long prescriptionIsVisiableRemedyCount = ConfigApplicationWorker.Get<long>(AppConfigKeys.CONFIG_KEY__HIS_DESKTOP__ASSIGN_PRESCRIPTION__ISVISIBLE_REMEDY_COUNT);
                if (prescriptionIsVisiableRemedyCount == 1)
                {
                    lciLadder.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                }

                this.layoutControlPrintAssignPrescriptionExt.Root.Clear();
                this.lciPrintAssignPrescriptionExt.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
                this.lciPrintAssignPrescriptionExt.MinSize = new System.Drawing.Size(2, 1);
                this.lciPrintAssignPrescriptionExt.MaxSize = new System.Drawing.Size(2, 40);
                this.lciPrintAssignPrescriptionExt.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                this.lciPrintAssignPrescription.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;

                this.cboPatientType.EditValue = null;
                this.cboPatientType.Properties.DataSource = null;

                this.lblChiPhiBNPhaiTra.Text = "";
                this.lblDaDong.Text = "";
                this.lblConThua.Text = "";
                this.lciForlblConThua.Text = Inventec.Common.Resource.Get.Value("frmAssignPrescription.lciForlblConThua.Text", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.lciForlblConThua.AppearanceItemCaption.ForeColor = System.Drawing.Color.Black;

                this.requestRoom = BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == this.currentModule.RoomId);

                this.lciExpMestReason.Visibility = actionType == GlobalVariables.ActionEdit ? DevExpress.XtraLayout.Utils.LayoutVisibility.Always : DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private async Task InitServiceConditionData()
        {
            try
            {
                workingServiceConditions = BackendDataWorker.Get<HIS_SERVICE_CONDITION>().Where(o => o.IS_ACTIVE == GlobalVariables.CommonNumberTrue).ToList();
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

        private void LoadDataByPackageService(V_HIS_SERE_SERV sereServ)
        {
            try
            {
                LogSystem.Debug("LoadDataByPackageService. 1");
                if (sereServ != null)
                {
                    this.serviceInPackages = null;
                    CommonParam param = new CommonParam();
                    //Lấy list service package
                    HisServicePackageViewFilter filter = new HisServicePackageViewFilter();
                    filter.SERVICE_ID = sereServ.SERVICE_ID;
                    this.servicePackageByServices = new BackendAdapter(param).Get<List<V_HIS_SERVICE_PACKAGE>>(HisRequestUriStore.HIS_SERVICE_PACKAGE_GETVIEW, ApiConsumers.MosConsumer, filter, ProcessLostToken, param);
                    if (this.servicePackageByServices != null && this.servicePackageByServices.Count > 0)
                    {
                        LogSystem.Debug("LoadDataByPackageService. 2");

                        List<long> serviceIds = this.servicePackageByServices.Select(o => o.SERVICE_ATTACH_ID).Distinct().ToList();
                        this.serviceInPackages = lstService.Where(o => serviceIds.Contains(o.ID)).ToList();
                        if (this.serviceInPackages == null || this.serviceInPackages.Count == 0)
                            throw new ArgumentNullException("serviceInPackages is null");


                        if (this.mediMatyTypeAvailables == null || this.mediMatyTypeAvailables.Count == 0)
                            this.InitDataMetyMatyTypeInStockD(this.currentMediStock);

                        //Release tat ca cac thuoc/ vat tu da duoc take bean truoc do nhung chua duoc luu
                        this.ReleaseAllMediByUser();

                        if (this.currentMediStock == null || this.currentMediStock.Count == 0)
                        {
                            Inventec.Common.Logging.LogSystem.Debug("Truong hop khoi tao goi vat tu khi load form ke don & danh sach kho duoc chon dang trong ==> tu dong mo form chon kho de nguoi dung chon ==> khoi tao san du lieu thuoc theo goi vat tu da chon");
                            frmWorkingMediStock frmWorkingMediStock = new frmWorkingMediStock(currentWorkingMestRooms, ProcessAfterChooseformMediStockForServicePackage);
                            frmWorkingMediStock.ShowDialog();
                        }
                        else
                        {
                            Inventec.Common.Logging.LogSystem.Debug("Truong hop khoi tao goi vat tu khi load form ke don & da co danh sach kho duoc chon ==> khoi tao san du lieu thuoc theo goi vat tu da chon");
                            ProcessAfterChooseformMediStockForServicePackage(this.currentMediStock);
                        }

                        LogSystem.Debug("LoadDataByPackageService. 3");

                        this.SetTotalPriceInPackage(sereServ);
                    }
                }
                LogSystem.Debug("LoadDataByPackageService. 4");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void CreateThreadLoadDataByServiceMetyMaty(object param)
        {
            Thread thread = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(LoadDataByServiceMetyMatyNewThread));
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

        void ProcessAfterChooseformMediStockForServiceMetyMaty(List<MOS.EFMODEL.DataModels.V_HIS_MEST_ROOM> selectedMestRooms)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("ProcessAfterChooseformMediStockForServiceMetyMaty. 1");
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

                this.OpionGroupSelectedChanged();

                Inventec.Common.Logging.LogSystem.Debug("ProcessAfterChooseformMediStockForServiceMetyMaty.2");
                //Release tat ca cac thuoc/ vat tu da duoc take bean truoc do nhung chua duoc luu
                this.ReleaseAllMediByUser();

                this.LoadPageMateMetyInServiceMety(this.serviceMetyByServices, this.serviceMatyByServices);
                //Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => mediMatyTypeADOs), mediMatyTypeADOs));
                this.ProcessInstructionTimeMediForEdit();
                Inventec.Common.Logging.LogSystem.Debug("ProcessAfterChooseformMediStockForServiceMetyMaty.3");
                if (this.ProcessCheckAllergenicByPatientAfterChoose()
                    && this.ProcessCheckContraindicaterWarningOptionAfterChoose())
                {
                    //this.CheckAmoutWarringInStock();
                    Inventec.Common.Logging.LogSystem.Debug("ProcessAfterChooseformMediStockForServiceMetyMaty.4");
                    this.ProcessMergeDuplicateRowForListProcessing();
                    this.ProcessAddListRowDataIntoGridWithTakeBean();
                    this.ReloadDataAvaiableMediBeanInCombo();

                    //Kiểm tra trần hao phí của dịch vụ chính & các thuốc/vật tư đã kê, đưa ra cảnh báo nếu vượt trần
                    this.AlertOutofMaxExpend();
                    Inventec.Common.Logging.LogSystem.Debug("ProcessAfterChooseformMediStockForServiceMetyMaty.5");
                }
                ProcessMediStock(this.mediMatyTypeADOs);
                //this.SetTotalPriceInPackage(sereServ);
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void LoadDataByServiceMetyMatyNewThread(object data)
        {
            try
            {
                if (this.InvokeRequired)
                {
                    this.Invoke(new MethodInvoker(delegate { LoadDataByServiceMetyMaty((V_HIS_SERE_SERV)data); }));
                }
                else
                {
                    LoadDataByServiceMetyMaty((V_HIS_SERE_SERV)data);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadDataByServiceMetyMaty(V_HIS_SERE_SERV sereServ)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("LoadDataByServiceMetyMaty.1");
                if (sereServ != null)
                {
                    CommonParam param = new CommonParam();
                    //Lấy list service package
                    HisServiceMetyViewFilter serviceMetyViewFilter = new HisServiceMetyViewFilter();
                    serviceMetyViewFilter.IS_ACTIVE = 1;
                    serviceMetyViewFilter.SERVICE_ID = sereServ.SERVICE_ID;
                    this.serviceMetyByServices = new BackendAdapter(param).Get<List<V_HIS_SERVICE_METY>>(RequestUriStore.HIS_SERVICE_METY__GET, ApiConsumers.MosConsumer, serviceMetyViewFilter, ProcessLostToken, param);

                    HisServiceMatyViewFilter serviceMatyViewFilter = new HisServiceMatyViewFilter();
                    serviceMatyViewFilter.SERVICE_ID = sereServ.SERVICE_ID;
                    serviceMatyViewFilter.IS_ACTIVE = 1;
                    this.serviceMatyByServices = new BackendAdapter(param).Get<List<V_HIS_SERVICE_MATY>>(RequestUriStore.HIS_SERVICE_MATY__GET, ApiConsumers.MosConsumer, serviceMatyViewFilter, ProcessLostToken, param);

                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => serviceMetyByServices), serviceMetyByServices)
                        + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => serviceMatyByServices), serviceMatyByServices));
                    Inventec.Common.Logging.LogSystem.Debug("LoadDataByServiceMetyMaty.2");
                    if ((serviceMetyByServices != null && serviceMetyByServices.Count > 0) || (serviceMatyByServices != null && serviceMatyByServices.Count > 0))
                    {

                        //Trường hợp chưa chọn kho thì tự động hiển thị kho cho người dùng chọn
                        if (this.currentMediStock == null || this.currentMediStock.Count == 0)
                        {
                            Inventec.Common.Logging.LogSystem.Debug("Truong hop khoi tao goi vat tu khi load form ke don & danh sach kho duoc chon dang trong ==> tu dong mo form chon kho de nguoi dung chon ==> khoi tao san du lieu thuoc theo goi vat tu da chon");
                            frmWorkingMediStock frmWorkingMediStock = new frmWorkingMediStock(currentWorkingMestRooms, ProcessAfterChooseformMediStockForServiceMetyMaty);
                            frmWorkingMediStock.ShowDialog();
                        }
                        else
                        {
                            Inventec.Common.Logging.LogSystem.Debug("Truong hop khoi tao goi vat tu khi load form ke don & da co danh sach kho duoc chon ==> khoi tao san du lieu thuoc theo goi vat tu da chon");
                            ProcessAfterChooseformMediStockForServiceMetyMaty(this.currentMediStock);
                        }

                    }
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

        private void LoadPageMateMetyInServiceMety(List<MOS.EFMODEL.DataModels.V_HIS_SERVICE_METY> serviceMetys, List<MOS.EFMODEL.DataModels.V_HIS_SERVICE_MATY> serviceMatys)
        {
            try
            {
                if ((serviceMetys == null || serviceMetys.Count == 0) && (serviceMatys == null || serviceMatys.Count == 0))
                    return;

                this.mediMatyTypeADOs = new List<MediMatyTypeADO>();
                if ((serviceMetys != null && serviceMetys.Count > 0))
                    foreach (var item in serviceMetys)
                    {
                        MediMatyTypeADO model = new MediMatyTypeADO();
                        var mety = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_MEDICINE_TYPE>().FirstOrDefault(o => o.ID == item.MEDICINE_TYPE_ID);
                        Inventec.Common.Mapper.DataObjectMapper.Map<MediMatyTypeADO>(model, mety);
                        model.DataType = HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC;
                        //Lay doi tuong mac dinh
                        MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE patientType = new MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE();
                        patientType = this.ChoosePatientTypeDefaultlServiceOther(this.currentHisPatientTypeAlter.PATIENT_TYPE_ID, model.SERVICE_ID, model.SERVICE_TYPE_ID);
                        if (patientType != null && patientType.ID > 0)
                        {
                            model.PATIENT_TYPE_ID = patientType.ID;
                            model.PATIENT_TYPE_CODE = patientType.PATIENT_TYPE_CODE;
                            model.PATIENT_TYPE_NAME = patientType.PATIENT_TYPE_NAME;
                        }

                        FillDataOtherPaySourceDataRow(model);

                        model.PRICE = item.EXPEND_PRICE;
                        model.AMOUNT = item.EXPEND_AMOUNT;

                        model.ID = item.MEDICINE_TYPE_ID;
                        model.PrimaryKey = (model.SERVICE_ID + "__" + Inventec.Common.DateTime.Get.Now() + "__" + Guid.NewGuid().ToString());

                        if ((AssignPrescriptionWorker.Instance.MediMatyCreateWorker.getIsAutoCheckExpend() == true
                    && HisConfigCFG.IsAutoTickExpendWithAssignPresPTTT)
                    || (mety != null && mety.IS_AUTO_EXPEND == GlobalVariables.CommonNumberTrue))
                        {
                            model.IsExpend = true;
                        }
                        model.IsAllowOdd = mety.IS_ALLOW_ODD == 1 ? true : false;
                        model.IsAllowOddAndExportOdd = (mety.IS_ALLOW_ODD == 1 && mety.IS_ALLOW_EXPORT_ODD == 1) ? true : false;

                        //Kiem tra xem thuoc do co trong kho??
                        AssignPrescriptionWorker.Instance.MediMatyCreateWorker.setDefaultMediStockForData(model);
                        MestMetyUnitWorker.UpdateUnit(model, GlobalStore.HisMestMetyUnit);

                        model.AMOUNT = ((model.IsUseOrginalUnitForPres ?? false) == false && model.CONVERT_RATIO.HasValue) ? (model.AMOUNT * model.CONVERT_RATIO.Value) : model.AMOUNT;
                        model.PRICE = ((model.IsUseOrginalUnitForPres ?? false) == false && model.CONVERT_RATIO.HasValue) ? (model.PRICE / model.CONVERT_RATIO.Value) : model.PRICE;
                        model.RAW_AMOUNT = (model.RAW_AMOUNT.HasValue && (model.IsUseOrginalUnitForPres ?? false) == false && model.CONVERT_RATIO.HasValue) ? (model.RAW_AMOUNT * model.CONVERT_RATIO.Value) : model.RAW_AMOUNT;

                        if (model.IS_ALLOW_ODD == 1 && model.IS_ALLOW_EXPORT_ODD != 1 && model.AMOUNT != (int)model.AMOUNT)
                        {
                            model.AMOUNT = (decimal)Inventec.Common.Number.Convert.RoundUpValue((double)model.AMOUNT, 0);
                        }

                        //Kiem tra thuoc trong kho, neu so luong trong goi lon hon 
                        //so luong kha dung trong kho thi gan vao so luong canh bao
                        decimal damount = (AmountOutOfStock(model, mety.SERVICE_ID, 0) ?? 0);
                        if (damount < 0 || model.AMOUNT > damount)
                        {
                            //model.AMOUNT = damount;
                            model.AmountAlert = model.AMOUNT;
                        }

                        model.NUM_ORDER = idRow;

                        this.mediMatyTypeADOs.Add(model);
                        this.idRow += stepRow;
                    }

                if ((serviceMatys != null && serviceMatys.Count > 0))
                    foreach (var item in serviceMatys)
                    {
                        MediMatyTypeADO model = new MediMatyTypeADO();
                        var maty = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_MATERIAL_TYPE>().FirstOrDefault(o => o.ID == item.MATERIAL_TYPE_ID);
                        Inventec.Common.Mapper.DataObjectMapper.Map<MediMatyTypeADO>(model, maty);
                        model.MEDICINE_TYPE_CODE = maty.MATERIAL_TYPE_CODE;
                        model.MEDICINE_TYPE_NAME = maty.MATERIAL_TYPE_NAME;

                        model.MEDICINE_USE_FORM_ID = null;
                        model.MEDICINE_USE_FORM_CODE = "";
                        model.MEDICINE_USE_FORM_NAME = "";
                        model.ErrorTypeMedicineUseForm = ErrorType.None;
                        model.ErrorMessageMedicineUseForm = "";
                        model.DataType = HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.VATTU;

                        //Lay doi tuong mac dinh
                        MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE patientType = new MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE();
                        patientType = this.ChoosePatientTypeDefaultlServiceOther(this.currentHisPatientTypeAlter.PATIENT_TYPE_ID, model.SERVICE_ID, model.SERVICE_TYPE_ID);

                        model.PRICE = item.EXPEND_PRICE;
                        model.AMOUNT = item.EXPEND_AMOUNT;
                        if (patientType != null && patientType.ID > 0)
                        {
                            model.PATIENT_TYPE_ID = patientType.ID;
                            model.PATIENT_TYPE_CODE = patientType.PATIENT_TYPE_CODE;
                            model.PATIENT_TYPE_NAME = patientType.PATIENT_TYPE_NAME;
                        }

                        FillDataOtherPaySourceDataRow(model);

                        model.PrimaryKey = (model.SERVICE_ID + "__" + Inventec.Common.DateTime.Get.Now() + "__" + Guid.NewGuid().ToString());
                        model.ID = item.MATERIAL_TYPE_ID;

                        //Kiem tra xem thuoc do co trong kho??
                        AssignPrescriptionWorker.Instance.MediMatyCreateWorker.setDefaultMediStockForData(model);
                        MestMetyUnitWorker.UpdateUnit(model, GlobalStore.HisMestMetyUnit);

                        model.AMOUNT = ((model.IsUseOrginalUnitForPres ?? false) == false && model.CONVERT_RATIO.HasValue) ? (model.AMOUNT * model.CONVERT_RATIO.Value) : model.AMOUNT;
                        model.PRICE = ((model.IsUseOrginalUnitForPres ?? false) == false && model.CONVERT_RATIO.HasValue) ? (model.PRICE / model.CONVERT_RATIO.Value) : model.PRICE;
                        model.RAW_AMOUNT = (model.RAW_AMOUNT.HasValue && (model.IsUseOrginalUnitForPres ?? false) == false && model.CONVERT_RATIO.HasValue) ? (model.RAW_AMOUNT * model.CONVERT_RATIO.Value) : model.RAW_AMOUNT;

                        if (model.IS_ALLOW_ODD == 1 && model.IS_ALLOW_EXPORT_ODD != 1 && model.AMOUNT != (int)model.AMOUNT)
                        {
                            model.AMOUNT = (decimal)Inventec.Common.Number.Convert.RoundUpValue((double)model.AMOUNT, 0);
                        }

                        if (((AssignPrescriptionWorker.Instance.MediMatyCreateWorker.getIsAutoCheckExpend() == true
                            //&& (model.HEIN_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__VT_TDM || model.HEIN_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__VT_NDM)
                            )
                   && HisConfigCFG.IsAutoTickExpendWithAssignPresPTTT)
                 || (maty != null && maty.IS_AUTO_EXPEND == GlobalVariables.CommonNumberTrue))
                        {
                            model.IsExpend = true;
                        }
                        model.IsStent = ((maty.IS_STENT ?? 0) == GlobalVariables.CommonNumberTrue ? true : false);
                        model.IsAllowOdd = maty.IS_ALLOW_ODD == 1 ? true : false;
                        model.IsAllowOddAndExportOdd = (maty.IS_ALLOW_ODD == 1 && maty.IS_ALLOW_EXPORT_ODD == 1) ? true : false;
                        //     if ((AssignPrescriptionWorker.Instance.MediMatyCreateWorker.getIsAutoCheckExpend() == true
                        //&& HisConfigCFG.IsAutoTickExpendWithAssignPresPTTT)
                        //|| (maty != null && maty.IS_AUTO_EXPEND == GlobalVariables.CommonNumberTrue))
                        //         model.IsExpend = true;

                        //Kiem tra thuoc trong kho, neu so luong trong goi lon hon 
                        //so luong kha dung trong kho thi gan vao so luong canh bao
                        decimal damount = (AmountOutOfStock(model, maty.SERVICE_ID, 0) ?? 0);
                        if (damount < 0 || model.AMOUNT > damount)
                        {
                            //model.AMOUNT = damount;
                            model.AmountAlert = model.AMOUNT;
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
                        model.DataType = HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC;
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
                    patientType = this.ChoosePatientTypeDefaultlServiceOther(this.currentHisPatientTypeAlter.PATIENT_TYPE_ID, model.SERVICE_ID, model.SERVICE_TYPE_ID);
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
                    //so luong kha dung trong kho thi gan vao so luong canh bao
                    decimal damount = (AmountOutOfStock(model, item.ID, 0) ?? 0);
                    if (damount < 0 || model.AMOUNT > damount)
                    {
                        //model.AMOUNT = damount;
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

        private async Task FillTreatmentInfo__PatientType()
        {
            try
            {
                this.lblPatientName.Text = this.currentTreatmentWithPatientType.TDL_PATIENT_NAME;
                if (this.currentTreatmentWithPatientType.TDL_PATIENT_DOB > 0)
                    this.lblDob.Text = Inventec.Common.DateTime.Convert.TimeNumberToDateString(this.currentTreatmentWithPatientType.TDL_PATIENT_DOB) + " (" + MPS.AgeUtil.CalculateFullAge(currentTreatmentWithPatientType.TDL_PATIENT_DOB) + ")";
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
                LogSystem.Debug("Begin FillDataToComboPriviousExpMest");
                if (this.periousExpMestListProcessor != null && this.ucPeriousExpMestList != null && treatment != null)
                {
                    this.currentPrescriptionFilter.TDL_PATIENT_ID = treatment.PATIENT_ID;
                    this.periousExpMestListProcessor.Load(this.ucPeriousExpMestList);
                }
                LogSystem.Debug("End FillDataToComboPriviousExpMest");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FillDataIntoPatientTypeCombo(MediMatyTypeADO data, GridLookUpEdit patientTypeCombo)
        {
            try
            {
                //var patientTypeIdAls = this.currentPatientTypeWithPatientTypeAlter.Select(o => o.ID).Distinct().ToList();
                if (patientTypeCombo != null
                    //&& patientTypeIdAls != null
                    //&& this.servicePatyAllows != null
                    )
                {
                    //var arrPatientTypeCode = this.servicePatyAllows[data.SERVICE_ID].Select(o => o.PATIENT_TYPE_CODE).Distinct().ToList();
                    if (this.currentPatientTypeWithPatientTypeAlter != null && this.currentPatientTypeWithPatientTypeAlter.Count > 0)
                    {
                        if (data.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC)
                        {
                            //1 thuốc được coi là "Có đủ thông tin BHYT" khi thỏa mãn:
                            //Khai báo đủ các thông tin: mã hoạt chất BHYT (ACTIVE_INGR_BHYT_CODE) và nhóm BHYT thuộc 1 trong các loại: "Thuốc trong danh mục", "Thuốc thanh toán theo tỷ lệ" hoặc "Thuốc ung thư, chống thải ghép"
                            //(bỏ, ko check "số đăng ký")
                            var sv = BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>().Where(o => o.SERVICE_ID == data.SERVICE_ID).FirstOrDefault();
                            if (sv != null)
                            {
                                data.SERVICE_ID = sv.SERVICE_ID;
                                data.SERVICE_TYPE_ID = sv.SERVICE_TYPE_ID;
                                data.ACTIVE_INGR_BHYT_CODE = sv.ACTIVE_INGR_BHYT_CODE;
                                data.HEIN_SERVICE_TYPE_ID = sv.HEIN_SERVICE_TYPE_ID;
                                data.HEIN_SERVICE_TYPE_CODE = sv.HEIN_SERVICE_TYPE_CODE;
                                data.HEIN_SERVICE_BHYT_CODE = sv.HEIN_SERVICE_BHYT_CODE;
                                data.HEIN_SERVICE_BHYT_NAME = sv.HEIN_SERVICE_BHYT_NAME;
                            }
                        }
                        else if (data.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__VT)
                        {
                            var sv = BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>().Where(o => o.SERVICE_ID == data.SERVICE_ID).FirstOrDefault();
                            if (sv != null)
                            {
                                data.SERVICE_ID = sv.SERVICE_ID;
                                data.SERVICE_TYPE_ID = sv.SERVICE_TYPE_ID;
                                data.HEIN_SERVICE_TYPE_ID = sv.HEIN_SERVICE_TYPE_ID;
                                data.HEIN_SERVICE_TYPE_CODE = sv.HEIN_SERVICE_TYPE_CODE;
                                data.HEIN_SERVICE_BHYT_CODE = sv.HEIN_SERVICE_BHYT_CODE;
                                data.HEIN_SERVICE_BHYT_NAME = sv.HEIN_SERVICE_BHYT_NAME;
                            }
                        }
                        var dt = IsFullHeinInfo(data);
                        List<HIS_PATIENT_TYPE> listSource = currentPatientTypeWithPatientTypeAlter;
                        if (!dt)
                            listSource = listSource.Where(o => o.ID != HisConfigCFG.PatientTypeId__BHYT).ToList();
                        //List<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE> dataCombo = this.currentPatientTypeWithPatientTypeAlter.Where(o => arrPatientTypeCode.Contains(o.PATIENT_TYPE_CODE) && mediStockInMestPatientTypeIds.Contains(o.ID)).ToList();

                        this.InitComboPatientType(patientTypeCombo, listSource);
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

                    var medicineInStock = this.mediMatyTypeAvailables.FirstOrDefault(o => o.SERVICE_ID == model.SERVICE_ID && ((isTSD && (o.IS_REUSABLE == 1 || o.IS_IDENTITY_MANAGEMENT == 1)) || (!isTSD && ((o.IS_REUSABLE == null || o.IS_REUSABLE != 1) && (o.IS_IDENTITY_MANAGEMENT == null || o.IS_IDENTITY_MANAGEMENT != 1)))));//Mac dinh lay kho dau tien
                    if (medicineInStock != null)
                    {
                        decimal amountAvailable = ((((model.IsUseOrginalUnitForPres ?? false) == false && model.CONVERT_RATIO.HasValue && model.CONVERT_RATIO > 0) ? medicineInStock.AMOUNT * model.CONVERT_RATIO : medicineInStock.AMOUNT) ?? 0);

                        //Trường hợp thuốc không đủ để xuất, số lượng khả dụng không đủ
                        //Set AmountAlert đánh dấu cảnh báo thuốc đã hết
                        if (amountAvailable < model.AMOUNT)
                        {
                            model.AmountAlert = model.AMOUNT;
                            if ((this.serviceReqMain != null && this.serviceReqMain.IS_MAIN_EXAM != 1) && (HisConfigCFG.IsUsingSubPrescriptionMechanism == "1") && !GlobalStore.IsCabinet)
                            {
                                model.IS_SUB_PRES = 1;
                            }
                        }

                        model.MEDI_STOCK_ID = medicineInStock.MEDI_STOCK_ID;
                        model.MEDI_STOCK_CODE = medicineInStock.MEDI_STOCK_CODE;
                        model.MEDI_STOCK_NAME = medicineInStock.MEDI_STOCK_NAME;
                    }
                    //Trường hợp thuốc không còn trong kho, Set AmountAlert đánh dấu cảnh báo thuốc đã hết
                    else
                    {
                        if ((this.serviceReqMain != null && this.serviceReqMain.IS_MAIN_EXAM != 1) && (HisConfigCFG.IsUsingSubPrescriptionMechanism == "1") && !GlobalStore.IsCabinet)
                        {
                            model.IS_SUB_PRES = 1;
                        }
                        model.AmountAlert = model.AMOUNT;
                        Inventec.Common.Logging.LogSystem.Debug("SetDefaultMediStockForData____truong hop thuoc/vat tu khong ton tai trong kho____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => medicineInStock), medicineInStock)
                            + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => model), model));
                    }
                }
                else
                {
                    model.AmountAlert = model.AMOUNT;

                    if ((this.serviceReqMain != null && this.serviceReqMain.IS_MAIN_EXAM != 1) && (HisConfigCFG.IsUsingSubPrescriptionMechanism == "1") && !GlobalStore.IsCabinet)
                    {
                        model.IS_SUB_PRES = 1;
                    }
                }
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
                    this.InstructionTime = intructionTimeSelecteds.OrderByDescending(o => o).First();
                    var q1 = (from m in listEmteMedcineType
                              select new MediMatyTypeADO(m, this.InstructionTime, isChayThan, licUseTime.Visible ? UseTime : 0)).ToList();
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

        //IsExpMestTemp : TRUE (Đơn mẫu), FALSE (Đơn cũ)
        private bool ProcessCheckOutMediStock(bool IsExpMestTemp)
        {
            bool isReturn = false;
            try
            {
                if ((currentMediStock == null || currentMediStock.Count == 0) && mediMatyTypeADOs != null && mediMatyTypeADOs.Count > 0 && ((IsExpMestTemp && mediMatyTypeADOs.FirstOrDefault(o => o.IS_OUT_MEDI_STOCK == null) != null) || !IsExpMestTemp))
                {
                    mediMatyTypeADOs = new List<MediMatyTypeADO>();
                    var myResult = DevExpress.XtraEditors.XtraMessageBox.Show(ResourceMessage.BanChuaChonKhoXuat, HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageBoxButtons.OK);
                    cboMediStockExport.Focus();
                    cboMediStockExport.SelectAll();
                    isReturn = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return isReturn;
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

                this.lstOutPatientPres = new List<OutPatientPresADO>();
                //Release tat ca cac thuoc/ vat tu da duoc take bean truoc do nhung chua duoc luu
                this.ReleaseAllMediByUser();

                if (chkShowLo.Checked)
                    chkShowLo.Checked = false;
                this.ProcessGetEmteMedcineType(this.GetEmteMedicineTypeByExpMestId(expTemplate.ID), false);
                this.ProcessGetEmteMaterialType(this.GetEmteMaterialTypeByExpMestId(expTemplate.ID), false);
                if (ProcessCheckOutMediStock(true))
                    return;
                //Check trong kho
                //Gắn biến tạm để không bị gấp đôi số lượng khi kiểm tra danh sách
                var dataSourceTmp = mediMatyTypeADOs;
                mediMatyTypeADOs = new List<MediMatyTypeADO>();
                this.ProcessDataMediStock(dataSourceTmp);
                if (!CheckMaterialReusableOrIdentityManager() || !CheckValidMaterial(true) || !CheckMedicineGroupTuberCulosis(true))
                    return;
                this.ProcessInstructionTimeMediForEdit();
                if (this.ProcessCheckAllergenicByPatientAfterChoose()
                    && this.ProcessCheckContraindicaterWarningOptionAfterChoose())
                {
                    this.ProcessMergeDuplicateRowForListProcessing();
                    if ((!(this.serviceReqMain != null && this.serviceReqMain.IS_MAIN_EXAM != 1) && (HisConfigCFG.IsUsingSubPrescriptionMechanism == "1") || (HisConfigCFG.IsUsingSubPrescriptionMechanism != "1") || GlobalStore.IsCabinet))
                    {
                        this.ProcessAddListRowDataIntoGridWithTakeBean();
                    }
                    else
                    {
                        List<MediMatyTypeADO> mediMatycheck = new List<MediMatyTypeADO>();
                        if (this.mediMatyTypeADOs != null && this.mediMatyTypeADOs.Count > 0)
                        {
                            foreach (var item in this.mediMatyTypeADOs)
                            {
                                if ((item.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC || item.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.VATTU || item.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.VATTU_TSD) && HisConfigCFG.IsUsingSubPrescriptionMechanism == "1" && !GlobalStore.IsCabinet)
                                {
                                    item.IS_SUB_PRES = 1;
                                }
                                FillDataOtherPaySourceDataRow(item);
                                UpdateExpMestReasonInDataRow(item);
                                if (ValidAcinInteractiveWorker.ValidGrade(item, mediMatycheck, ref txtInteractionReason, this))
                                {
                                    mediMatycheck.Add(item);
                                }
                            }
                        }
                        this.mediMatyTypeADOs = mediMatycheck;
                        this.RefeshResourceGridMedicine();
                    }
                    //this.VerifyWarningOverCeiling();
                    this.ReloadDataAvaiableMediBeanInCombo();
                }
                this.ResetFocusMediMaty(true);
                this.TickAllMediMateAssignPresed();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        public void ProcessMediStock(List<MediMatyTypeADO> mediMatyTypeADOs)
        {
            try
            {
                foreach (var item in mediMatyTypeADOs)
                {
                    if (!item.MEDI_STOCK_ID.HasValue)
                        continue;
                    var stock = BackendDataWorker.Get<V_HIS_MEDI_STOCK>().FirstOrDefault(o => o.ID == item.MEDI_STOCK_ID);
                    if (stock != null && stock.IS_EXPEND == 1)
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
                if (this.ProcessCheckAllergenicByPatientAfterChoose()
                    && this.ProcessCheckContraindicaterWarningOptionAfterChoose())
                {
                    this.ProcessMergeDuplicateRowForListProcessing();
                    this.ProcessAddListRowDataIntoGridWithTakeBean();
                    //this.VerifyWarningOverCeiling();
                    this.ReloadDataAvaiableMediBeanInCombo();
                }
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
                if (!CheckMaterialReusableOrIdentityManager() || !CheckValidMaterial(true) || !CheckMedicineGroupTuberCulosis(true))
                    return;
                this.ProcessInstructionTimeMediForEdit();
                if (this.ProcessCheckAllergenicByPatientAfterChoose()
                    && this.ProcessCheckContraindicaterWarningOptionAfterChoose())
                {
                    this.ProcessMergeDuplicateRowForListProcessing();
                    this.ProcessAddListRowDataIntoGridWithTakeBean();
                    //this.VerifyWarningOverCeiling();
                    this.ReloadDataAvaiableMediBeanInCombo();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private bool CheckMaterialReusableOrIdentityManager()
        {
            if (this.mediMatyTypeADOs != null && this.mediMatyTypeADOs.Count > 0)
            {
                var MaterialReusableOrIdentityManager = GetMaterialReusableOrIdentityManager();
                if (MaterialReusableOrIdentityManager != null && MaterialReusableOrIdentityManager.Count > 0)
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show(string.Format("Cần chọn lại vật tư đích danh - TSD: {0}.", string.Join(", ", MaterialReusableOrIdentityManager.Select(o => o.MEDICINE_TYPE_NAME))), HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageBoxButtons.OK);
                    mediMatyTypeADOs = mediMatyTypeADOs.Where(o => !MaterialReusableOrIdentityManager.Contains(o)).ToList();
                    return false;
                }
            }
            return true;
        }

        private List<MediMatyTypeADO> GetMaterialReusableOrIdentityManager()
        {
            if (this.mediMatyTypeADOs != null && this.mediMatyTypeADOs.Count > 0)
            {
                return this.mediMatyTypeADOs.Where(maty => (maty.IS_REUSABLE == 1 || maty.IS_IDENTITY_MANAGEMENT == 1)).ToList();
            }
            return new List<MediMatyTypeADO>();
        }

        /// <summary>
        /// Lấy giá trị cấu hình trần giới hạn BHYT thanh toán theo đúng tuyến & theo chuyển tuyến
        /// Nếu không có cấu hình thì return luôn -> không check giới hạn
        /// Chỉ thực hiện kiểm tra khi đối tượng bệnh nhân là "Khám" hoặc "Điều trị ngoại trú"
        /// (his_treatment có tdl_treatment_type_id = IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__KHAM hoặc IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTNGOAITRU)
        /// </summary>
        /// <param name="treatmentId"></param>
        /// <returns></returns>
        internal bool IsLimitHeinMedicinePrice(long treatmentId)
        {
            bool result = false;
            try
            {

                LogSystem.Debug("Begining  IsLimitHeinMedicinePrice ...");

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
                if ((this.currentHisPatientTypeAlter.TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__KHAM
                    || this.currentHisPatientTypeAlter.TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTNGOAITRU)
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
                if (this.totalPriceBHYT > 0
                    && (this.currentHisPatientTypeAlter.TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__KHAM
                    || this.currentHisPatientTypeAlter.TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTNGOAITRU))
                {
                    if (this.currentTreatmentWithPatientType != null
                        && this.currentTreatmentWithPatientType.IS_NOT_CHECK_LHMP.HasValue
                        && this.currentTreatmentWithPatientType.IS_NOT_CHECK_LHMP == GlobalVariables.CommonNumberTrue)
                    {
                        return false;
                    }

                    decimal limitHeinMedicinePrice__RightMediOrg = HisConfigCFG.LimitHeinMedicinePrice__RightMediOrg;
                    decimal limitHeinMedicinePrice__NotRightMediOrg = HisConfigCFG.LimitHeinMedicinePrice__NotRightMediOrg;

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
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => lstExpMestMedicine), lstExpMestMedicine));
                if (lstExpMestMedicine != null && lstExpMestMedicine.Count > 0)
                {
                    List<V_HIS_MEDICINE_BEAN_1> medicineBeans = null;
                    if (isEdit)
                    {
                        medicineBeans = GetMedicineBeanByExpMestMedicine(lstExpMestMedicine.Select(o => o.ID).ToList());
                    }
                    //var q1 = (from m in lstExpMestMedicine
                    //          select new MediMatyTypeADO(m, medicineBeans, isEdit)).ToList();

                    List<MediMatyTypeADO> mediMatyTypeADOAdds = new List<MediMatyTypeADO>();
                    foreach (var item in lstExpMestMedicine)
                    {
                        if ((item.IS_NOT_PRES ?? 0) != 1)
                        {
                            var itemNoPres = lstExpMestMedicine.FirstOrDefault(o =>
                                   o.IS_NOT_PRES.HasValue && o.IS_NOT_PRES == 1
                                   && o.MEDICINE_TYPE_ID == item.MEDICINE_TYPE_ID
                                   && ((o.MEDICINE_ID == null && item.MEDICINE_ID == null) || (o.MEDICINE_ID == item.MEDICINE_ID))
                                   && o.MEDI_STOCK_ID == item.MEDI_STOCK_ID
                                   && o.PATIENT_TYPE_ID == item.PATIENT_TYPE_ID
                                   && o.IS_EXPEND == item.IS_EXPEND
                                   && o.EXPEND_TYPE_ID == item.EXPEND_TYPE_ID
                                   && o.IS_OUT_PARENT_FEE == item.IS_OUT_PARENT_FEE
                                   && o.SERE_SERV_PARENT_ID == item.SERE_SERV_PARENT_ID
                                   && o.TUTORIAL == item.TUTORIAL
                                   );

                            MediMatyTypeADO mediMatyTypeADOAdd = null;
                            if (itemNoPres != null)
                            {
                                //Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => itemNoPres), itemNoPres)
                                //    + "____" + Inventec.Common.Logging.LogUtil.TraceData("item.AMOUNT truoc", item.AMOUNT)
                                //     + "____" + Inventec.Common.Logging.LogUtil.TraceData("item.AMOUNT sau", (item.AMOUNT + itemNoPres.AMOUNT)));

                                decimal RAW_AMOUNT = item.AMOUNT;
                                item.AMOUNT += itemNoPres.AMOUNT;

                                mediMatyTypeADOAdd = new MediMatyTypeADO(item, medicineBeans, isEdit);
                                mediMatyTypeADOAdd.RAW_AMOUNT = RAW_AMOUNT;

                                mediMatyTypeADOAdd.AMOUNT = (decimal)Inventec.Common.Number.Convert.RoundUpValue((double)mediMatyTypeADOAdd.AMOUNT, 0);
                            }
                            else
                            {
                                mediMatyTypeADOAdd = new MediMatyTypeADO(item, medicineBeans, isEdit);
                            }

                            Inventec.Common.Logging.LogSystem.Debug("Before:" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => mediMatyTypeADOAdd.SERVICE_CONDITION_ID), mediMatyTypeADOAdd.SERVICE_CONDITION_ID)
                                        + Inventec.Common.Logging.LogUtil.TraceData("ExpMestID", item.ID)
                                        + Inventec.Common.Logging.LogUtil.TraceData("SERVICE_ID", item.SERVICE_ID));
                            if (this.sereServWithTreatment != null && this.sereServWithTreatment.Count > 0)
                            {
                                var ssEXP = this.sereServWithTreatment.Where(o => o.EXP_MEST_MEDICINE_ID == item.ID).ToList();
                                if (ssEXP != null && ssEXP.Count > 0)
                                {
                                    var scdt = this.workingServiceConditions != null ? workingServiceConditions.FirstOrDefault(o => o.ID == ssEXP[0].SERVICE_CONDITION_ID) : null;
                                    if (scdt != null)
                                    {
                                        mediMatyTypeADOAdd.SERVICE_CONDITION_ID = ssEXP[0].SERVICE_CONDITION_ID;
                                        mediMatyTypeADOAdd.SERVICE_CONDITION_NAME = scdt.SERVICE_CONDITION_NAME;
                                    }
                                    Inventec.Common.Logging.LogSystem.Debug("After:" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => mediMatyTypeADOAdd.SERVICE_CONDITION_ID), mediMatyTypeADOAdd.SERVICE_CONDITION_ID)
                                        + Inventec.Common.Logging.LogUtil.TraceData("scdt", scdt)
                                        + Inventec.Common.Logging.LogUtil.TraceData("ExpMestID", item.ID)
                                        + Inventec.Common.Logging.LogUtil.TraceData("SERVICE_ID", item.SERVICE_ID));
                                }
                            }
                            if (mediMatyTypeADOAdd.ALERT_MAX_IN_TREATMENT.HasValue && !string.IsNullOrEmpty(mediMatyTypeADOAdd.EXCEED_LIMIT_IN_BATCH_REASON))
                            {
                                mediMatyTypeADOAdd.IsAlertInTreatPresciption = true;
                                mediMatyTypeADOAdd.PATIENT_NAME_BY_TREATMENT_CODE = currentTreatment.TDL_PATIENT_NAME + "_" + currentTreatment.TREATMENT_CODE;
                                mediMatyTypeADOsAlertInTreatment.Add(mediMatyTypeADOAdd);
                            }
                            mediMatyTypeADOAdds.Add(mediMatyTypeADOAdd);
                        }
                    }

                    if (mediMatyTypeADOAdds != null && mediMatyTypeADOAdds.Count > 0)
                        this.mediMatyTypeADOs.AddRange(mediMatyTypeADOAdds);
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
                if (lstExpMestMedicine != null && lstExpMestMedicine.Count > 0)
                {
                    List<HIS_MEDICINE_BEAN> medicineBeans = null;
                    List<MediMatyTypeADO> mediMatyTypeADOAdds = new List<MediMatyTypeADO>();
                    foreach (var item in lstExpMestMedicine)
                    {
                        if ((item.IS_NOT_PRES ?? 0) != 1)
                        {
                            var itemNoPres = lstExpMestMedicine.FirstOrDefault(o =>
                                    o.IS_NOT_PRES.HasValue && o.IS_NOT_PRES == 1
                                    && o.MEDICINE_TYPE_ID == item.MEDICINE_TYPE_ID
                                    && ((o.MEDICINE_ID == null && item.MEDICINE_ID == null) || (o.MEDICINE_ID == item.MEDICINE_ID))
                                    && o.MEDI_STOCK_ID == item.MEDI_STOCK_ID
                                    //&& o.PATIENT_TYPE_ID == item.PATIENT_TYPE_ID
                                    //&& o.IS_EXPEND == item.IS_EXPEND
                                    //&& o.EXPEND_TYPE_ID == item.EXPEND_TYPE_ID
                                    //&& o.IS_OUT_PARENT_FEE == item.IS_OUT_PARENT_FEE
                                    //&& o.SERE_SERV_PARENT_ID == item.SERE_SERV_PARENT_ID
                                    && o.TUTORIAL == item.TUTORIAL
                                    );

                            MediMatyTypeADO mediMatyTypeADOAdd = null;
                            if (itemNoPres != null)
                            {
                                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => itemNoPres), itemNoPres)
                                    + "____" + Inventec.Common.Logging.LogUtil.TraceData("item.AMOUNT truoc", item.AMOUNT)
                                     + "____" + Inventec.Common.Logging.LogUtil.TraceData("item.AMOUNT sau", (item.AMOUNT + itemNoPres.AMOUNT)));
                                item.PRES_AMOUNT += itemNoPres.PRES_AMOUNT;

                                decimal RAW_AMOUNT = item.AMOUNT;
                                item.AMOUNT += itemNoPres.AMOUNT;
                                item.AMOUNT = (decimal)Inventec.Common.Number.Convert.RoundUpValue((double)item.AMOUNT, 0);

                                mediMatyTypeADOAdd = new MediMatyTypeADO(item, currentInstructionTime, medicineBeans, serviceReq);
                                mediMatyTypeADOAdd.RAW_AMOUNT = ((mediMatyTypeADOAdd.IsUseOrginalUnitForPres ?? false) == false && mediMatyTypeADOAdd.CONVERT_RATIO.HasValue) ? (RAW_AMOUNT * mediMatyTypeADOAdd.CONVERT_RATIO.Value) : RAW_AMOUNT;
                                //mediMatyTypeADOAdd.RAW_AMOUNT = RAW_AMOUNT;


                            }
                            else
                            {
                                mediMatyTypeADOAdd = new MediMatyTypeADO(item, currentInstructionTime, medicineBeans, serviceReq);
                            }

                            mediMatyTypeADOAdds.Add(mediMatyTypeADOAdd);
                        }
                    }

                    //Check trong kho
                    this.ProcessDataMediStock(mediMatyTypeADOAdds);
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
                    var stock = BackendDataWorker.Get<V_HIS_MEDI_STOCK>();
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

                    List<MediMatyTypeADO> mediMatyTypeADOAdds = new List<MediMatyTypeADO>();
                    foreach (var item in lstExpMestMaterial)
                    {
                        if ((item.IS_NOT_PRES ?? 0) != 1)
                        {
                            var itemNoPres = lstExpMestMaterial.FirstOrDefault(o =>
                                o.IS_NOT_PRES.HasValue && o.IS_NOT_PRES == 1
                                && o.MATERIAL_TYPE_ID == item.MATERIAL_TYPE_ID
                                && ((o.MATERIAL_ID == null && item.MATERIAL_ID == null) || (o.MATERIAL_ID == item.MATERIAL_ID))
                                && o.MEDI_STOCK_ID == item.MEDI_STOCK_ID
                                //&& o.PATIENT_TYPE_ID == item.PATIENT_TYPE_ID
                                //&& o.IS_EXPEND == item.IS_EXPEND
                                //&& o.EXPEND_TYPE_ID == item.EXPEND_TYPE_ID
                                //&& o.IS_OUT_PARENT_FEE == item.IS_OUT_PARENT_FEE
                                );

                            MediMatyTypeADO mediMatyTypeADOAdd = null;
                            if (itemNoPres != null)
                            {
                                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => itemNoPres), itemNoPres)
                                    + "____" + Inventec.Common.Logging.LogUtil.TraceData("item.AMOUNT truoc", item.AMOUNT)
                                     + "____" + Inventec.Common.Logging.LogUtil.TraceData("item.AMOUNT sau", (item.AMOUNT + itemNoPres.AMOUNT)));

                                decimal RAW_AMOUNT = item.AMOUNT;
                                item.AMOUNT += itemNoPres.AMOUNT;

                                mediMatyTypeADOAdd = new MediMatyTypeADO(item, materialBeans, isEdit);
                                mediMatyTypeADOAdd.RAW_AMOUNT = RAW_AMOUNT;

                                mediMatyTypeADOAdd.AMOUNT = (decimal)Inventec.Common.Number.Convert.RoundUpValue((double)mediMatyTypeADOAdd.AMOUNT, 0);
                            }
                            else
                            {
                                mediMatyTypeADOAdd = new MediMatyTypeADO(item, materialBeans, isEdit);
                            }

                            if (this.sereServWithTreatment != null && this.sereServWithTreatment.Count > 0)
                            {
                                var ssEXP = this.sereServWithTreatment.Where(o => o.EXP_MEST_MATERIAL_ID == item.ID).ToList();
                                if (ssEXP != null && ssEXP.Count > 0)
                                {
                                    var scdt = this.workingServiceConditions != null ? workingServiceConditions.FirstOrDefault(o => o.ID == ssEXP[0].SERVICE_CONDITION_ID) : null;
                                    if (scdt != null)
                                    {
                                        mediMatyTypeADOAdd.SERVICE_CONDITION_ID = ssEXP[0].SERVICE_CONDITION_ID;
                                        mediMatyTypeADOAdd.SERVICE_CONDITION_NAME = scdt.SERVICE_CONDITION_NAME;
                                    }
                                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => mediMatyTypeADOAdd.SERVICE_CONDITION_ID), mediMatyTypeADOAdd.SERVICE_CONDITION_ID)
                                        + Inventec.Common.Logging.LogUtil.TraceData("scdt", scdt)
                                        + Inventec.Common.Logging.LogUtil.TraceData("ExpMestID", item.ID)
                                        + Inventec.Common.Logging.LogUtil.TraceData("SERVICE_ID", item.SERVICE_ID));
                                }
                            }

                            mediMatyTypeADOAdds.Add(mediMatyTypeADOAdd);
                        }
                    }

                    if (mediMatyTypeADOAdds != null && mediMatyTypeADOAdds.Count > 0)
                        this.mediMatyTypeADOs.AddRange(mediMatyTypeADOAdds);
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
                    if (isEdit && !((this.serviceReqMain != null && this.serviceReqMain.IS_MAIN_EXAM != 1) && (HisConfigCFG.IsUsingSubPrescriptionMechanism == "1")))
                    {
                        materialBeans = GetMaterialBeanByExpMestMedicine(lstExpMestMaterial.Select(o => o.ID).ToList());
                    }

                    List<MediMatyTypeADO> mediMatyTypeADOAdds = new List<MediMatyTypeADO>();
                    foreach (var item in lstExpMestMaterial)
                    {
                        if ((item.IS_NOT_PRES ?? 0) != 1)
                        {
                            var itemNoPres = lstExpMestMaterial.FirstOrDefault(o =>
                                   o.IS_NOT_PRES.HasValue && o.IS_NOT_PRES == 1
                                   && o.MATERIAL_TYPE_ID == item.MATERIAL_TYPE_ID
                                   && ((o.MATERIAL_ID == null && item.MATERIAL_ID == null) || (o.MATERIAL_ID == item.MATERIAL_ID))

                                   //&& o.MEDI_STOCK_ID == item.MEDI_STOCK_ID
                                   //&& o.PATIENT_TYPE_ID == item.PATIENT_TYPE_ID
                                   //&& o.IS_EXPEND == item.IS_EXPEND
                                   //&& o.EXPEND_TYPE_ID == item.EXPEND_TYPE_ID
                                   //&& o.IS_OUT_PARENT_FEE == item.IS_OUT_PARENT_FEE
                                   );


                            MediMatyTypeADO mediMatyTypeADOAdd = null;
                            if (itemNoPres != null)
                            {
                                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => itemNoPres), itemNoPres)
                                    + "____" + Inventec.Common.Logging.LogUtil.TraceData("item.AMOUNT truoc", item.AMOUNT)
                                     + "____" + Inventec.Common.Logging.LogUtil.TraceData("item.AMOUNT sau", (item.AMOUNT + itemNoPres.AMOUNT)));
                                item.PRES_AMOUNT += itemNoPres.PRES_AMOUNT;
                                decimal RAW_AMOUNT = item.AMOUNT;
                                item.AMOUNT += itemNoPres.AMOUNT;

                                mediMatyTypeADOAdd = new MediMatyTypeADO(item, materialBeans, isEdit);
                                mediMatyTypeADOAdd.RAW_AMOUNT = RAW_AMOUNT;

                                mediMatyTypeADOAdd.AMOUNT = (decimal)Inventec.Common.Number.Convert.RoundUpValue((double)mediMatyTypeADOAdd.AMOUNT, 0);
                            }
                            else
                            {
                                mediMatyTypeADOAdd = new MediMatyTypeADO(item, materialBeans, isEdit);
                            }

                            mediMatyTypeADOAdds.Add(mediMatyTypeADOAdd);
                        }
                    }

                    this.ProcessDataMediStock(mediMatyTypeADOAdds);
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
                    {
                        this.mediMatyTypeADOs.AddRange(q1);

                        if (q1.Exists(o => o.ALERT_MAX_IN_TREATMENT.HasValue && !string.IsNullOrEmpty(o.EXCEED_LIMIT_IN_BATCH_REASON)))
                        {
                            foreach (var mediMatyTypeADOAdd in q1.Where(o => o.ALERT_MAX_IN_TREATMENT.HasValue && !string.IsNullOrEmpty(o.EXCEED_LIMIT_IN_BATCH_REASON)))
                            {
                                mediMatyTypeADOAdd.IsAlertInTreatPresciption = true;
                                mediMatyTypeADOAdd.PATIENT_NAME_BY_TREATMENT_CODE = currentTreatment.TDL_PATIENT_NAME + "_" + currentTreatment.TREATMENT_CODE;
                                mediMatyTypeADOsAlertInTreatment.Add(mediMatyTypeADOAdd);
                            }
                        }
                    }
                    if (this.mediMatyTypeADOs != null && this.mediMatyTypeADOs.Count > 0)
                    {
                        foreach (var item in this.mediMatyTypeADOs)
                        {
                            if (item.MEDI_STOCK_ID == null && item.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC && (this.oldServiceReq != null && this.oldServiceReq.EXECUTE_ROOM_ID > 0))
                            {
                                var mediStock = BackendDataWorker.Get<V_HIS_MEDI_STOCK>().Where(o => o.ROOM_ID == this.oldServiceReq.EXECUTE_ROOM_ID).FirstOrDefault();

                                if (mediStock != null)
                                {
                                    item.MEDI_STOCK_ID = mediStock.ID;
                                    item.MEDI_STOCK_CODE = mediStock.MEDI_STOCK_CODE;
                                    item.MEDI_STOCK_NAME = mediStock.MEDI_STOCK_NAME;
                                    if (mediStock.IS_EXPEND == 1)
                                    {
                                        item.IsExpend = true;
                                        item.IsDisableExpend = true;
                                    }
                                }
                            }

                            if (item.PATIENT_TYPE_ID == null && item.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC && (this.oldServiceReq != null && this.oldServiceReq.TDL_PATIENT_TYPE_ID > 0))
                            {
                                item.PATIENT_TYPE_ID = this.oldServiceReq.TDL_PATIENT_TYPE_ID;

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

        /// <summary>
        /// đơn phụ
        /// </summary>
        /// <param name="lstExpMestMety"></param>
        /// <param name="serviceReq"></param>
        /// <param name="currentInstructionTime"></param>
        private void ProcessGetSubServiceReqMety(List<HIS_SERVICE_REQ_METY> lstExpMestMety, V_HIS_SERVICE_REQ_7 serviceReq, long currentInstructionTime)
        {
            try
            {
                if (lstExpMestMety != null)
                {
                    List<MediMatyTypeADO> mediMatyTypeADOAdds = new List<MediMatyTypeADO>();
                    var q1 = (from m in lstExpMestMety
                              where m.IS_SUB_PRES == 1
                              select new MediMatyTypeADO(m, currentInstructionTime, serviceReq)).ToList();
                    if (q1 != null && q1.Count > 0)
                        mediMatyTypeADOAdds.AddRange(q1);

                    //Check trong kho
                    this.ProcessDataMediStock(mediMatyTypeADOAdds);
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
                              where m.IS_SUB_PRES != 1
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
                    if (!isEdit)
                    {
                        List<MediMatyTypeADO> mediMatyTypeADOAdds = new List<MediMatyTypeADO>();

                        var q1 = (from m in lstExpMestMaty
                                  where m.IS_SUB_PRES == 1
                                  select new MediMatyTypeADO(m, isEdit)).ToList();
                        if (q1 != null && q1.Count > 0)
                            mediMatyTypeADOAdds.AddRange(q1);

                        //Check trong kho
                        this.ProcessDataMediStock(mediMatyTypeADOAdds);

                        var q2 = (from m in lstExpMestMaty
                                  where m.IS_SUB_PRES != 1
                                  select new MediMatyTypeADO(m, isEdit)).ToList();
                        if (q2 != null && q2.Count > 0)
                            this.mediMatyTypeADOs.AddRange(q2);
                    }
                    else
                    {
                        var q1 = (from m in lstExpMestMaty
                                  select new MediMatyTypeADO(m, isEdit)).ToList();
                        if (q1 != null && q1.Count > 0)
                            this.mediMatyTypeADOs.AddRange(q1);
                    }

                    if (this.mediMatyTypeADOs != null && this.mediMatyTypeADOs.Count > 0)
                    {
                        foreach (var item in this.mediMatyTypeADOs)
                        {
                            if (item.MEDI_STOCK_ID == null && item.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.VATTU && (this.oldServiceReq != null && this.oldServiceReq.EXECUTE_ROOM_ID > 0))
                            {
                                var mediStock = BackendDataWorker.Get<V_HIS_MEDI_STOCK>().Where(o => o.ROOM_ID == this.oldServiceReq.EXECUTE_ROOM_ID).FirstOrDefault();

                                if (mediStock != null)
                                {
                                    item.MEDI_STOCK_ID = mediStock.ID;
                                    item.MEDI_STOCK_CODE = mediStock.MEDI_STOCK_CODE;
                                    item.MEDI_STOCK_NAME = mediStock.MEDI_STOCK_NAME;
                                    if (mediStock.IS_EXPEND == 1)
                                    {
                                        item.IsExpend = true;
                                        item.IsDisableExpend = true;
                                    }
                                }
                            }

                            if (item.PATIENT_TYPE_ID == null && item.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.VATTU && (this.oldServiceReq != null && this.oldServiceReq.TDL_PATIENT_TYPE_ID > 0))
                            {
                                item.PATIENT_TYPE_ID = this.oldServiceReq.TDL_PATIENT_TYPE_ID;
                                item.PATIENT_TYPE_CODE = "";
                                item.PATIENT_TYPE_NAME = "";
                            }

                            if (this.oldServiceReq != null && this.serviceReqMain != null && this.serviceReqMain.IS_SUB_PRES == 1)
                            {
                                item.IS_SUB_PRES = 1;
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

        private void ProcessChoicePrescriptionPrevious(List<MOS.EFMODEL.DataModels.V_HIS_SERVICE_REQ_7> LstserviceReq)
        {
            try
            {
                if (LstserviceReq == null || LstserviceReq.Count <= 0) return;
                if (this.actionType == GlobalVariables.ActionView)
                {
                    LogSystem.Debug("ProcessChoicePrescriptionPrevious => thao tac khong hop le. actionType = " + this.actionType);
                    return;
                }
                if (this.mediMatyTypeAvailables == null || this.mediMatyTypeAvailables.Count == 0)
                    this.InitDataMetyMatyTypeInStockD(this.currentMediStock);
                this.PrescriptionPrevious = true;
                this.lstOutPatientPres = new List<OutPatientPresADO>();
                //Release tat ca cac thuoc/ vat tu da duoc take bean truoc do nhung chua duoc luu
                this.ReleaseAllMediByUser();
                if (chkShowLo.Checked)
                    chkShowLo.Checked = false;
                ServiceReqIdPrevios = LstserviceReq.First().ID;
                List<V_HIS_SERVICE_REQ_7> serviceReqTemps = new List<V_HIS_SERVICE_REQ_7>();
                foreach (var serviceReq in LstserviceReq)
                {
                    if (String.IsNullOrEmpty(serviceReq.SESSION_CODE))
                    {
                        serviceReqTemps.Add(serviceReq);
                    }
                    else
                    {
                        this.currentPrescriptions = this.periousExpMestListProcessor.GetServiceReqData(this.ucPeriousExpMestList);
                        serviceReqTemps.AddRange(this.currentPrescriptions != null ? this.currentPrescriptions.Where(o =>
                            o.SESSION_CODE == serviceReq.SESSION_CODE
                            && o.INTRUCTION_TIME == serviceReq.INTRUCTION_TIME
                            && o.REQUEST_LOGINNAME == serviceReq.REQUEST_LOGINNAME).ToList() : null);
                    }
                }

                serviceReqTemps = serviceReqTemps.Distinct().ToList();

                if (serviceReqTemps != null && serviceReqTemps.Count > 0)
                {
                    foreach (var item in serviceReqTemps)
                    {
                        if (item.EXP_MEST_ID.HasValue && item.EXP_MEST_TYPE_ID.HasValue)
                        {
                            this.ProcessGetExpMestMedicine_Prescription(this.GetExpMestMedicineByExpMestId(item.EXP_MEST_ID ?? 0), item, this.intructionTimeSelecteds.First());
                            this.ProcessGetExpMestMaterial_Prescription(this.GetExpMestMaterialByExpMestId(item.EXP_MEST_ID ?? 0), false);
                            if (ProcessCheckOutMediStock(false))
                                return;
                        }
                        //Đơn phụ
                        this.ProcessGetSubServiceReqMety(this.GetServiceReqMetyByServiceReqId(item.ID), item, this.intructionTimeSelecteds.First());

                        this.ProcessGetServiceReqMety(this.GetServiceReqMetyByServiceReqId(item.ID), item, this.intructionTimeSelecteds.First());
                        this.ProcessGetServiceReqMaty(this.GetServiceReqMatyByServiceReqId(item.ID), false);
                    }
                    this.mediMatyTypeADOs.ForEach(o => o.EXCEED_LIMIT_IN_PRES_REASON = null);
                    this.mediMatyTypeADOs.ForEach(o => o.EXCEED_LIMIT_IN_DAY_REASON = null);
                    this.mediMatyTypeADOs.ForEach(o => o.EXCEED_LIMIT_IN_BATCH_REASON = null);
                    this.mediMatyTypeADOs.ForEach(o => o.ODD_PRES_REASON = null);
                }
                //Check trong kho
                //Gắn biến tạm để không bị gấp đôi số lượng khi kiểm tra danh sách
                var dataSourceTmp = mediMatyTypeADOs;
                mediMatyTypeADOs = new List<MediMatyTypeADO>();
                this.ProcessDataMediStock(dataSourceTmp);
                if (!CheckMaterialReusableOrIdentityManager() || !CheckValidMaterial(true) || !CheckMedicineGroupTuberCulosis(true))
                    return;
                this.ProcessInstructionTimeMediForEdit();
                if (this.ProcessCheckAllergenicByPatientAfterChoose()
                    && this.ProcessCheckContraindicaterWarningOptionAfterChoose())
                {
                    this.ProcessMergeDuplicateRowForListProcessing();
                    if (!((this.serviceReqMain != null && this.serviceReqMain.IS_MAIN_EXAM != 1) && (HisConfigCFG.IsUsingSubPrescriptionMechanism == "1")) || GlobalStore.IsCabinet)
                    {
                        this.ProcessAddListRowDataIntoGridWithTakeBean();
                    }
                    else
                    {
                        List<MediMatyTypeADO> mediMatycheck = new List<MediMatyTypeADO>();
                        foreach (var item in this.mediMatyTypeADOs)
                        {
                            if (item.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC || item.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.VATTU || item.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.VATTU_TSD)
                            {
                                item.IS_SUB_PRES = 1;
                            }
                            UpdateExpMestReasonInDataRow(item);
                            if (ValidAcinInteractiveWorker.ValidGrade(item, mediMatycheck, ref txtInteractionReason, this))
                            {
                                mediMatycheck.Add(item);
                            }
                        }
                        this.mediMatyTypeADOs = mediMatycheck;

                        this.RefeshResourceGridMedicine();
                    }
                    this.ReloadDataAvaiableMediBeanInCombo();
                }
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => serviceReqTemps), serviceReqTemps));
                TickAllMediMateAssignPresed();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
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
                decimal totalPrice = 0, totalPriceBHYT = 0, totalPriceOther = 0;
                totalPriceNotBHYT = 0;
                if (medicineTypeADOs != null && medicineTypeADOs.Count > 0)
                {
                    foreach (var item in medicineTypeADOs)
                    {
                        totalPrice += item.TotalPrice;
                        if (item.PATIENT_TYPE_ID == HisConfigCFG.PatientTypeId__BHYT && item.MEDI_STOCK_ID.HasValue && item.MEDI_STOCK_ID.Value > 0)
                        {
                            totalPriceBHYT += item.TotalPrice;
                        }
                        else if (item.PATIENT_TYPE_ID != HisConfigCFG.PatientTypeId__BHYT && item.MEDI_STOCK_ID.HasValue && item.MEDI_STOCK_ID.Value > 0)
                        {
                            totalPriceNotBHYT += item.TotalPrice;
                        }
                        else
                        {
                            totalPriceOther += item.TotalPrice;
                        }
                    }
                }

                this.lblPhatSinh.Text = Inventec.Common.Number.Convert.NumberToStringRoundAuto(totalPrice, ConfigApplications.NumberSeperator);
                this.lblPhatSinh__BHYT.Text = Inventec.Common.Number.Convert.NumberToStringRoundAuto(totalPriceBHYT, ConfigApplications.NumberSeperator);
                this.lblPhatSinh__KhacBHYT.Text = Inventec.Common.Number.Convert.NumberToStringRoundAuto(totalPriceNotBHYT, ConfigApplications.NumberSeperator);
                this.lblPhatSinh__MuaNgoai.Text = Inventec.Common.Number.Convert.NumberToStringRoundAuto(totalPriceOther, ConfigApplications.NumberSeperator);
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

        private void CalculateAmount()
        {
            try
            {
                decimal vlAmount = CalculateValueAmount();
                if (vlAmount >= 0)
                    this.spinAmount.EditValue = vlAmount;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal decimal CalculateValueAmount()
        {
            decimal vl = -1;
            PresAmount = 0;
            try
            {
                bool is4Control = lciSang.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                double sang, trua, chieu, toi = 0, tocdotho, thoigiantho;
                sang = is4Control ? this.GetValueSpin(this.spinSang.Text) : 0;
                trua = is4Control ? this.GetValueSpin(this.spinTrua.Text) : 0;
                chieu = is4Control ? this.GetValueSpin(this.spinChieu.Text) : 0;
                toi = is4Control ? this.GetValueSpin(this.spinToi.Text) : 0;
                tocdotho = !is4Control ? this.GetValueSpin(this.txtTocDoTho.Text) : 0;
                thoigiantho = !is4Control ? this.GetValueSpin(this.txtThoiGianTho.Text) : 0;
                if (sang > 0
                    || trua > 0
                    || chieu > 0
                    || toi > 0
                    || tocdotho > 0
                    || thoigiantho > 0
                    )
                {
                    long roomTypeId = GetRoomTypeId();
                    double tongCong = is4Control ? (sang + trua + chieu + toi) : (tocdotho * thoigiantho * 60);
                    if (currentMedicineTypeADOForEdit != null && currentMedicineTypeADOForEdit.ID > 0)
                    {
                        bool isNotOdd = false;
                        if (
                            //(GlobalStore.IsTreatmentIn || GlobalStore.IsCabinet) && 
                            (currentMedicineTypeADOForEdit.IsAllowOdd.HasValue && currentMedicineTypeADOForEdit.IsAllowOdd.Value == true))
                        {

                        }
                        else
                        {
                            isNotOdd = true;
                        }
                        int plusSeperate = 1;
                        decimal amount = this.spinSoLuongNgay.Value * (decimal)tongCong;
                        PresAmount = amount;
                        if (HisConfigCFG.IsShowPresAmount && VHistreatment != null && VHistreatment.TDL_TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__KHAM)
                        {
                            return vl = (decimal)(Inventec.Common.Number.Convert.RoundUpValue((double)amount, 0) != (int)(amount) ? (int)(amount) + plusSeperate : amount);
                        }
                        if (isNotOdd)
                        {
                            vl = (decimal)(Inventec.Common.Number.Convert.RoundUpValue((double)amount, 0) != (int)(amount) ? (int)(amount) + plusSeperate : amount);
                            Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => amount), amount) + "____" + Inventec.Common.Logging.LogUtil.TraceData("(int)amount", (int)(amount)) + "____" + Inventec.Common.Logging.LogUtil.TraceData("spinSoLuongNgay.Value", spinSoLuongNgay.Value));
                        }
                        else
                        {
                            vl = Inventec.Common.Number.Convert.NumberToNumberRoundAuto((decimal)(amount), GetNumberDisplaySeperateFormat());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return vl;
        }

        internal decimal CalculateRawValueAmount()
        {
            Inventec.Common.Logging.LogSystem.Debug("CalculateRawValueAmount.1");
            decimal vl = -1;
            try
            {
                bool is4Control = lciSang.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                double sang, trua, chieu, toi = 0, tocdotho, thoigiantho;
                sang = is4Control ? this.GetValueSpin(this.spinSang.Text) : 0;
                trua = is4Control ? this.GetValueSpin(this.spinTrua.Text) : 0;
                chieu = is4Control ? this.GetValueSpin(this.spinChieu.Text) : 0;
                toi = is4Control ? this.GetValueSpin(this.spinToi.Text) : 0;
                tocdotho = !is4Control ? this.GetValueSpin(this.txtTocDoTho.Text) : 0;
                thoigiantho = !is4Control ? this.GetValueSpin(this.txtThoiGianTho.Text) : 0;
                if (sang > 0
                    || trua > 0
                    || chieu > 0
                    || toi > 0
                    || tocdotho > 0
                    || thoigiantho > 0
                    )
                {
                    Inventec.Common.Logging.LogSystem.Debug("CalculateRawValueAmount.2");
                    double tongCong = is4Control ? (sang + trua + chieu + toi) : (tocdotho * thoigiantho * 60);
                    if (currentMedicineTypeADOForEdit != null && currentMedicineTypeADOForEdit.ID > 0 && currentMedicineTypeADOForEdit.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC)//Check chỉ cho thuốc mới kiểm tra cấu hình và đánh dấu tách thuốc nếu không có cấu hình kê lẻ và số lượng kê lẻ, vật tư bỏ qua
                    {
                        decimal amount = Inventec.Common.Number.Convert.NumberToNumberRoundMax4((decimal)((this.spinSoLuongNgay.Value) * (decimal)(tongCong)));
                        if ((GlobalStore.IsTreatmentIn && !GlobalStore.IsCabinet)
                            && ((currentMedicineTypeADOForEdit.IsAllowOdd == null || currentMedicineTypeADOForEdit.IsAllowOdd.Value == false))
                            && amount != (int)amount)
                        {
                            vl = amount;
                            Inventec.Common.Logging.LogSystem.Debug("Ke don dieu tri____tinh toan so luong raw____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => amount), amount) + "____" +
                                Inventec.Common.Logging.LogUtil.TraceData("GetNumberDisplaySeperateFormat", GetNumberDisplaySeperateFormat()) + "____" +
                                Inventec.Common.Logging.LogUtil.TraceData("(int)amount", (int)(amount)) + "____" + Inventec.Common.Logging.LogUtil.TraceData("spinSoLuongNgay.Value", spinSoLuongNgay.Value));
                            Inventec.Common.Logging.LogSystem.Debug("CalculateRawValueAmount.3");
                        }
                    }
                    Inventec.Common.Logging.LogSystem.Debug("CalculateRawValueAmount.4");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            Inventec.Common.Logging.LogSystem.Debug("CalculateRawValueAmount.5");
            return vl;
        }

        int GetNumberDisplaySeperateFormat()
        {
            int numberDisplaySeperateFormatAmountTemp = 0;
            if (HisConfigCFG.AmountDecimalNumber > 0 && numberDisplaySeperateFormatAmount > 0)
            {
                numberDisplaySeperateFormatAmountTemp = HisConfigCFG.AmountDecimalNumber;
            }
            else
            {
                numberDisplaySeperateFormatAmountTemp = numberDisplaySeperateFormatAmount;
            }
            return numberDisplaySeperateFormatAmountTemp;
        }

        private void SetHuongDanFromSoLuongNgay()
        {
            try
            {
                if (HisConfigCFG.IsNotAutoGenerateTutorial)
                {
                    return;
                }

                string serviceUnitName = "";
                bool isUse = false;
                if (this.currentMedicineTypeADOForEdit != null)
                {
                    isUse = true;
                    serviceUnitName = !String.IsNullOrEmpty(this.currentMedicineTypeADOForEdit.CONVERT_UNIT_NAME) ? this.currentMedicineTypeADOForEdit.CONVERT_UNIT_NAME : (this.currentMedicineTypeADOForEdit.SERVICE_UNIT_NAME ?? "").ToLower();
                }
                if (isUse)
                {
                    int numberDisplaySeperateFormatAmountTemp = GetNumberDisplaySeperateFormat();

                    StringBuilder huongDan = new StringBuilder();
                    
                    string format__ThoVoiTocDoXTrongYGio = ResourceMessage.ThoVoiTocDoXTrongYGio;
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
                    string strSeperator = ", ";
                    int solan = 0;
                    double soLuongTrenlanMin = 0;
                    string buoiChon = "";
                    bool is4Control = lciSang.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    double sang, trua, chieu, toi = 0, tocdotho, thoigiantho;
                    double tongCong = 0;
                    sang = is4Control ? this.GetValueSpin(this.spinSang.Text) : 0;
                    trua = is4Control ? this.GetValueSpin(this.spinTrua.Text) : 0;
                    chieu = is4Control ? this.GetValueSpin(this.spinChieu.Text) : 0;
                    toi = is4Control ? this.GetValueSpin(this.spinToi.Text) : 0;
                    tocdotho = !is4Control ? this.GetValueSpin(this.txtTocDoTho.Text) : 0;
                    thoigiantho = !is4Control ? this.GetValueSpin(this.txtThoiGianTho.Text) : 0;
                    if (sang > 0
                        || trua > 0
                        || chieu > 0
                        || toi > 0
                        || tocdotho > 0
                        || thoigiantho > 0
                    )
                    {
                        if (sang > 0)
                        {
                            if (soLuongTrenlanMin == 0 || soLuongTrenlanMin > sang)
                                soLuongTrenlanMin = sang;
                            solan += 1;
                            buoiChon = ResourceMessage.BuoiSang;
                        }
                        if (trua > 0)
                        {
                            if (soLuongTrenlanMin == 0 || soLuongTrenlanMin > trua)
                                soLuongTrenlanMin = trua;
                            solan += 1;
                            buoiChon = ResourceMessage.BuoiTrua;
                        }
                        if (chieu > 0)
                        {
                            if (soLuongTrenlanMin == 0 || soLuongTrenlanMin > chieu)
                                soLuongTrenlanMin = chieu;
                            solan += 1;
                            buoiChon = ResourceMessage.BuoiChieu;
                        }
                        if (toi > 0)
                        {
                            if (soLuongTrenlanMin == 0 || soLuongTrenlanMin > toi)
                                soLuongTrenlanMin = toi;
                            solan += 1;
                            buoiChon = ResourceMessage.BuoiToi;
                        }
                        tongCong = is4Control ? (sang + trua + chieu + toi) : (tocdotho * thoigiantho * 60);

                        if (HisConfigCFG.TutorialFormat == 4)
                        {
                            //< Đường dùng> <Tổng số ngày> ngày. Ngày <đường dùng> <số lượng tổng số 1 ngày> <đơn vị> / số lần : thời điểm trong ngày : số lượng <Dạng dùng>      
                            //==> Uống {0} ngày. Ngày uống {1} lần {2} {3} 
                            //==>  Uống 4 ngày. Ngày uống 4 lần sáng 1, trưa 1, chiều 1, tối 1 sau ăn
                            huongDan.Append(String.Format(format__NgayUongTemp4, (String.IsNullOrEmpty(this.cboMedicineUseForm.Text) ? "" : (FirstCharToUpper(this.cboMedicineUseForm.Text) + " ")), (int)spinSoLuongNgay.Value, (String.IsNullOrEmpty(this.cboMedicineUseForm.Text) ? "" : this.cboMedicineUseForm.Text.ToLower() + " "), solan) + " ");
                            huongDan.Append(sang > 0 ? String.Format(format__Sang, Inventec.Common.Number.Convert.NumberToStringRoundAuto((decimal)sang, 4), "").Trim().ToLower() : "");//Sáng {0} {1}
                            huongDan.Append(trua > 0 ? ((String.IsNullOrEmpty(huongDan.ToString()) ? "" : strSeperator) + String.Format(format__Trua, Inventec.Common.Number.Convert.NumberToStringRoundAuto((decimal)trua, 4), "").Trim().ToLower()) : "");
                            huongDan.Append(chieu > 0 ? ((String.IsNullOrEmpty(huongDan.ToString()) ? "" : strSeperator) + String.Format(format__Chieu, Inventec.Common.Number.Convert.NumberToStringRoundAuto((decimal)chieu, 4), "").Trim().ToLower()) : "");
                            huongDan.Append(toi > 0 ? ((String.IsNullOrEmpty(huongDan.ToString()) ? "" : strSeperator) + String.Format(format__Toi, Inventec.Common.Number.Convert.NumberToStringRoundAuto((decimal)toi, 4), "").Trim().ToLower()) : "");
                            huongDan.Append((String.IsNullOrEmpty(this.cboHtu.Text) ? "" : " " + string.Join(", ", DataHtuList.Where(o => o.IsChecked).OrderByDescending(o => o.CHECK_ACIN_INTERACTIVE ?? Int64.MaxValue).ThenBy(o => o.NUM_ORDER).Select(o => o.HTU_NAME).ToList()).ToLower()));
                        }
                        else
                        {
                            if (solan == 1)
                            {
                                if (HisConfigCFG.TutorialFormat == 2)
                                {
                                    huongDan.Append(tongCong > 0 ? String.Format(format__NgayUongTemp2, (String.IsNullOrEmpty(this.cboMedicineUseForm.Text) ? "" : " " + this.cboMedicineUseForm.Text.ToLower() + " "), "", "", solan) : "");
                                }
                                else
                                {
                                    if ((int)tongCong == tongCong)
                                        huongDan.Append(!String.IsNullOrEmpty(this.spinAmount.Text) ? String.Format(format___NgayXVienBuoiYZ, (String.IsNullOrEmpty(this.cboMedicineUseForm.Text) ? "" : this.cboMedicineUseForm.Text + " "), "" + (int)tongCong, serviceUnitName, buoiChon) : "");
                                    else
                                        huongDan.Append(!String.IsNullOrEmpty(this.spinAmount.Text) ? String.Format(format___NgayXVienBuoiYZ, (String.IsNullOrEmpty(this.cboMedicineUseForm.Text) ? " " : this.cboMedicineUseForm.Text + " "), ConvertNumber.ConvertDecToFracByConfig(tongCong, 4), serviceUnitName, buoiChon) : "");
                                }
                            }
                            else
                            {
                                if (is4Control)
                                {
                                    if (HisConfigCFG.TutorialFormat == 2)
                                    {
                                        huongDan.Append(tongCong > 0 ? String.Format(format__NgayUongTemp2, (String.IsNullOrEmpty(this.cboMedicineUseForm.Text) ? "" : " " + this.cboMedicineUseForm.Text.ToLower() + " "), "", "", solan) : "");
                                    }
                                    else
                                    {
                                        if ((int)tongCong == tongCong)
                                            huongDan.Append(tongCong > 0 ? String.Format(format__NgayUong, (String.IsNullOrEmpty(this.cboMedicineUseForm.Text) ? "" : " " + this.cboMedicineUseForm.Text.ToLower() + " "), " " + (int)tongCong, serviceUnitName, solan) : "");
                                        else
                                            huongDan.Append(tongCong > 0 ? String.Format(format__NgayUong, (String.IsNullOrEmpty(this.cboMedicineUseForm.Text) ? "" : " " + this.cboMedicineUseForm.Text.ToLower() + " "), ConvertNumber.ConvertDecToFracByConfig(tongCong, 4), serviceUnitName, solan) : "");
                                    }

                                    huongDan.Append(sang > 0 ? (String.IsNullOrEmpty(huongDan.ToString()) ? "" : strSeperator) + String.Format(format__Sang, ConvertNumber.ConvertDecToFracByConfig(sang, 4), serviceUnitName) : "");
                                    huongDan.Append(trua > 0 ? ((String.IsNullOrEmpty(huongDan.ToString()) ? "" : strSeperator) + String.Format(format__Trua, ConvertNumber.ConvertDecToFracByConfig(trua, 4), serviceUnitName)) : "");
                                    huongDan.Append(chieu > 0 ? ((String.IsNullOrEmpty(huongDan.ToString()) ? "" : strSeperator) + String.Format(format__Chieu, ConvertNumber.ConvertDecToFracByConfig(chieu, 4), serviceUnitName)) : "");
                                    huongDan.Append(toi > 0 ? ((String.IsNullOrEmpty(huongDan.ToString()) ? "" : strSeperator) + String.Format(format__Toi, ConvertNumber.ConvertDecToFracByConfig(toi, 4), serviceUnitName)) : "");
                                    //}
                                }
                                else
                                {
                                    //- Khi nhập tốc độ thở và số giờ thì tự động set Số lượng = Tốc độ thở * giờ * 60.
                                    //- Khi nhập tốc độ thở và số giờ thì tự động set hướng dẫn: Thở với tốc độ x lít/phút trong vòng y giờ.
                                    huongDan.Append(tongCong > 0 ? String.Format(format__ThoVoiTocDoXTrongYGio, this.txtTocDoTho.Text, this.txtThoiGianTho.Text) : "");
                                }
                            }
                            if (is4Control)
                                huongDan.Append(!String.IsNullOrEmpty(this.cboHtu.Text) ? " (" + string.Join(", ", DataHtuList.Where(o => o.IsChecked).OrderByDescending(o => o.CHECK_ACIN_INTERACTIVE ?? Int64.MaxValue).ThenBy(o => o.NUM_ORDER).Select(o => o.HTU_NAME).ToList()).ToLower() + ")" : "");

                            huongDan = new StringBuilder().Append(FirstCharToUpper(huongDan.ToString()));
                        }

                        if (HisConfigCFG.TutorialFormat == 3)
                        {
                            string hdKey1 = huongDan.ToString();
                            //<Số lượng> <đơn vị>/lần * <Số lần>/ngày (<Định dạnh như cấu hình giá trị 1>)   {0} {1}/lần * {2} lần/ngày
                            //số lượng: số lượng thấp nhất trong 1 lần dùng/số ngày dùng làm tròn đến 2 chữ số sau phần thập phân
                            //Ví dụ: Thuốc A được kê là Ngày uống 3 viên chia 2 lần, sáng 1 viên, chiều 2 viên thì hiển thị: 1 viên/lần * 2 lần/ngày (Ngày uống 3 viên chia 2 lần, sáng 01 viên, chiều 02 viên)
                            huongDan = new StringBuilder();
                            huongDan.Append(soLuongTrenlanMin > 0 ? String.Format(format__NgayUongTemp3, Inventec.Common.Number.Convert.NumberToStringRoundAuto((decimal)soLuongTrenlanMin, 2), serviceUnitName, solan) : "");
                            huongDan.Append(" (");
                            huongDan.Append(hdKey1);
                            huongDan.Append(")");
                        }
                        if (HisConfigCFG.TutorialFormat == 5)
                        {
                            //<Đường dùng>, <Cách dùng>, Thời điểm trong ngày : Số lượng
                            //Ví dụ: Uống, Trước ăn, sáng 1 viên, chiều 1 viên.
                            huongDan = new StringBuilder();
                            huongDan.Append(soLuongTrenlanMin > 0 ? String.Format(format__NgayUongTemp5, (String.IsNullOrEmpty(this.cboMedicineUseForm.Text) ? "" : " " + this.cboMedicineUseForm.Text.ToLower() + " "), !String.IsNullOrEmpty(this.cboHtu.Text) ? string.Join(", ", DataHtuList.Where(o => o.IsChecked).OrderByDescending(o => o.CHECK_ACIN_INTERACTIVE ?? Int64.MaxValue).ThenBy(o => o.NUM_ORDER).Select(o => o.HTU_NAME).ToList()).ToLower() : "") : "");
                            huongDan.Append(sang > 0 ? ((String.IsNullOrEmpty(huongDan.ToString()) ? "" : strSeperator) + String.Format(format__Sang, ConvertNumber.ConvertDecToFracByConfig(sang, 4), serviceUnitName)) : "");
                            huongDan.Append(trua > 0 ? ((String.IsNullOrEmpty(huongDan.ToString()) ? "" : strSeperator) + String.Format(format__Trua, ConvertNumber.ConvertDecToFracByConfig(trua, 4), serviceUnitName)) : "");
                            huongDan.Append(chieu > 0 ? ((String.IsNullOrEmpty(huongDan.ToString()) ? "" : strSeperator) + String.Format(format__Chieu, ConvertNumber.ConvertDecToFracByConfig(chieu, 4), serviceUnitName)) : "");
                            huongDan.Append(toi > 0 ? ((String.IsNullOrEmpty(huongDan.ToString()) ? "" : strSeperator) + String.Format(format__Toi,
                              ConvertNumber.ConvertDecToFracByConfig(toi, 4), serviceUnitName)) : "");
                        }
                    }
                    if (HisConfigCFG.TutorialFormat == 6)
                    {
                        //Ví dụ: Liều dùng của thuốc A: 2 viên/lần, 2 lần/ngày, sử dụng trong 5 ngày thì được ghi như sau: 2 viên/lần * 2 lần/ngày * 5 ngày [4 viên/ngày].
                        //Liều dùng của thuốc A, sáng: 3 viên, chiều: 2 viên, tối: 1 viên.Như vậy, sẽ ghi như sau: Sáng: 3 viên, Chiều: 2 viên, Tối: 1 viên[6 viên / ngày].
                        huongDan = new StringBuilder();

                        if (sang > 0 || trua > 0 || chieu > 0 || toi > 0)
                        {
                            List<double> lst = new List<double>() { sang, trua, chieu, toi };
                            lst = lst.Where(o => o > 0).Distinct().ToList();
                            if (lst.Count == 1)
                            {
                                huongDan.Append(solan > 0 ? String.Format("{0} {1}/lần * {2} lần/ngày * {3} ngày", Inventec.Common.Number.Convert.NumberToStringRoundAuto((decimal)lst[0], 2), serviceUnitName, solan, (((int)spinSoLuongNgay.Value) >= 1 && ((int)spinSoLuongNgay.Value) < 10) ? "0" + ((int)spinSoLuongNgay.Value).ToString() : ((int)spinSoLuongNgay.Value).ToString()) : "");
                            }
                            else
                            {
                                huongDan.Append(sang > 0 ? (String.IsNullOrEmpty(huongDan.ToString()) ? "" : strSeperator) + String.Format(format__Sang, ConvertNumber.ConvertDecToFracByConfig(sang, 4), serviceUnitName) : "");
                                huongDan.Append(trua > 0 ? ((String.IsNullOrEmpty(huongDan.ToString()) ? "" : strSeperator) + String.Format(format__Trua, ConvertNumber.ConvertDecToFracByConfig(trua, 4), serviceUnitName)) : "");
                                huongDan.Append(chieu > 0 ? ((String.IsNullOrEmpty(huongDan.ToString()) ? "" : strSeperator) + String.Format(format__Chieu, ConvertNumber.ConvertDecToFracByConfig(chieu, 4), serviceUnitName)) : "");
                                huongDan.Append(toi > 0 ? ((String.IsNullOrEmpty(huongDan.ToString()) ? "" : strSeperator) + String.Format(format__Toi, ConvertNumber.ConvertDecToFracByConfig(toi, 4), serviceUnitName)) : "");
                            }
                            if ((int)tongCong == tongCong)
                                huongDan.Append(!String.IsNullOrEmpty(this.spinAmount.Text) ? string.Format(" [{0} {1}/ngày]", (int)tongCong, serviceUnitName) : "");
                            else
                                huongDan.Append(!String.IsNullOrEmpty(this.spinAmount.Text) ? String.Format(" [{0} {1}/ngày]", ConvertNumber.ConvertDecToFracByConfig(tongCong, 4), serviceUnitName) : "");
                        }
                        else
                        {
                            solan = 1;
                            List<double> lst = new List<double>() { (double)((double)this.GetValueSpin(this.spinAmount.Text) / ((int)spinSoLuongNgay.Value)) };
                            lst = lst.Where(o => o > 0).Distinct().ToList();
                            if (lst.Count == 1)
                            {
                                huongDan.Append(solan > 0 ? String.Format("{0} {1}/lần * {2} lần/ngày * {3} ngày", Inventec.Common.Number.Convert.NumberToStringRoundAuto((decimal)lst[0], 2), serviceUnitName, solan, (((int)spinSoLuongNgay.Value) >= 1 && ((int)spinSoLuongNgay.Value) < 10) ? "0" + ((int)spinSoLuongNgay.Value).ToString() : ((int)spinSoLuongNgay.Value).ToString()) : "");

                                if ((int)tongCong == tongCong)
                                    huongDan.Append(!String.IsNullOrEmpty(this.spinAmount.Text) ? string.Format(" [{0} {1}/ngày]", Inventec.Common.Number.Convert.NumberToStringRoundAuto((decimal)lst[0], 2), serviceUnitName) : "");
                                else
                                    huongDan.Append(!String.IsNullOrEmpty(this.spinAmount.Text) ? String.Format(" [{0} {1}/ngày]", ConvertNumber.ConvertDecToFracByConfig((double)lst[0], 4), serviceUnitName) : "");
                            }
                        }
                    }
                    this.txtTutorial.Text = string.IsNullOrEmpty(huongDan.ToString()) ? this.txtTutorial.Text : huongDan.ToString().Replace("  ", " ").Replace(", ,", ",");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        string FormatNumber(int data)
        {
            string result = data.ToString();
            try
            {
                result = (data >= 1 && data < 10 ? "0" + data : data.ToString());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
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

        private void SpinKeyPressNoSeperate(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
                {
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SpinAmountKeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                int numberDisplaySeperateFormatAmountTemp = GetNumberDisplaySeperateFormat();
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => numberDisplaySeperateFormatAmountTemp), numberDisplaySeperateFormatAmountTemp) + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => GlobalStore.IsCabinet), GlobalStore.IsCabinet));
                if (numberDisplaySeperateFormatAmountTemp == 0)
                {
                    if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != (char)System.Windows.Forms.Keys.Back)
                    {
                        e.Handled = true;
                    }
                }
                else if (numberDisplaySeperateFormatAmountTemp > 0)
                {
                    if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) &&
   (e.KeyChar != '.') && (e.KeyChar != ',') && (e.KeyChar != '/') && e.KeyChar != (char)System.Windows.Forms.Keys.Back)
                    {
                        e.Handled = true;
                    }
                }

                // only allow one decimal point                
                if ((e.KeyChar == '/') && ((sender as TextEdit).Text.IndexOf('/') > -1))
                {
                    e.Handled = true;
                }
                if (((e.KeyChar == '.') || (e.KeyChar == ',')) && (((sender as TextEdit).Text.IndexOf('.') > -1) || ((sender as TextEdit).Text.IndexOf(',') > -1)))
                {
                    e.Handled = true;
                }

                string oldText = (sender as TextEdit).Text;
                if ((oldText.IndexOf('.') > -1) || (oldText.IndexOf(',') > -1))
                {
                    var arrAmount = oldText.Split(new string[] { ",", "." }, StringSplitOptions.RemoveEmptyEntries);
                    if (arrAmount != null && arrAmount.Length > 1)
                    {
                        string afterSeperate = arrAmount[1];
                        if (afterSeperate.Length >= numberDisplaySeperateFormatAmountTemp && e.KeyChar != (char)System.Windows.Forms.Keys.Back)
                        {
                            e.Handled = true;
                        }
                    }
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
        private void spinAmount_Leave(object sender, EventArgs e)
        {
            try
            {
                TextEdit txt = sender as TextEdit;
                if (txt.OldEditValue.ToString() == txt.EditValue.ToString())
                    return;
                decimal vlTmp = 0;
                CultureInfo culture = new CultureInfo("en-US");
                spinAmount.Text = GetAmount().ToString();
                if (spinAmount.Text.Contains(","))  
                    culture = new CultureInfo("fr-FR");
                decimal amountTmp = Convert.ToDecimal(spinAmount.EditValue.ToString(), culture);
                if ((currentMedicineTypeADOForEdit != null && currentMedicineTypeADOForEdit.ID > 0 && amountTmp != (currentMedicineTypeADOForEdit.AMOUNT ?? 0)) || (currentMedicineTypeADOForEdit == null && txt.EditValue.ToString() != txt.OldEditValue.ToString()))
                    this.SetHuongDanFromSoLuongNgay();
                if (currentMedicineTypeADOForEdit != null && currentMedicineTypeADOForEdit.ID > 0 && (!GlobalStore.IsTreatmentIn || GlobalStore.IsCabinet))
                {
                    int plusSeperate = 1;
                    if (amountTmp > 0)
                    {
                        if ((HisConfigCFG.IsShowPresAmount && VHistreatment != null && VHistreatment.TDL_TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__KHAM)
                            || !(currentMedicineTypeADOForEdit.IsAllowOdd.HasValue && currentMedicineTypeADOForEdit.IsAllowOdd.Value == true))
                        {
                            vlTmp = (decimal)(Inventec.Common.Number.Convert.RoundUpValue((double)amountTmp, 0) != (int)(amountTmp) ? (int)(amountTmp) + plusSeperate : amountTmp);
                        }
                        else
                        {
                            vlTmp = Inventec.Common.Number.Convert.NumberToNumberRoundAuto((decimal)(amountTmp), GetNumberDisplaySeperateFormat());
                        }
                        if (vlTmp != 0)
                        {
                            spinAmount.EditValue = vlTmp.ToString();
                            PresAmount = amountTmp;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        internal double GetValueSpinHasAround(string strValue)
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

                    bool is4Control = lciSang.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    double sang, trua, chieu, toi = 0, tocdotho, thoigiantho;
                    sang = is4Control ? this.GetValueSpin(this.spinSang.Text) : 0;
                    trua = is4Control ? this.GetValueSpin(this.spinTrua.Text) : 0;
                    chieu = is4Control ? this.GetValueSpin(this.spinChieu.Text) : 0;
                    toi = is4Control ? this.GetValueSpin(this.spinToi.Text) : 0;
                    tocdotho = !is4Control ? this.GetValueSpin(this.txtTocDoTho.Text) : 0;
                    thoigiantho = !is4Control ? this.GetValueSpin(this.txtThoiGianTho.Text) : 0;
                    if (!(sang > 0
                        || trua > 0
                        || chieu > 0
                        || toi > 0
                        || tocdotho > 0
                        || thoigiantho > 0
                        ))
                    {
                        PresAmount = (decimal?)value;
                    }
                    //Xử lý tụ động làm tròn đến số chữ số được cấu hình sau phần thập phân(numberDisplaySeperateFormatAmount)//TODO
                    //if (value != (int)value)
                    //{
                    //Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("value before", value));
                    //value = (double)Inventec.Common.Number.Convert.NumberToNumberRoundAuto((decimal)value, GetNumberDisplaySeperateFormat());
                    //Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("numberDisplaySeperateFormatAmount", numberDisplaySeperateFormatAmount) + Inventec.Common.Logging.LogUtil.TraceData("HisConfigCFG.AmountDecimalNumber", HisConfigCFG.AmountDecimalNumber) + Inventec.Common.Logging.LogUtil.TraceData("value after", value));
                    //}
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return value;
        }

        /// <summary>
        /// Sửa cơ chế lưu dữ liệu vào các trường "sáng", "trưa", "chiều", "tối" trong chi tiết đơn thuốc (morning, noon, afternoon, evening trong his_exp_mest_medicine) để lưu theo định dạng như trên. Cụ thể:
        //- Nếu số nguyên thì ko hiển thị ,00 phía sau. Số nguyên nhỏ hơn 10 thì hiển thị 0 phía trước.
        //vd:
        //+ SL ghi 1,00 --> sửa thành 01.
        //+ SL 12,00 --> sửa thành 12
        //- Số lượng bằng 0 thì để trống ko ghi 0,00 như hiện tại
        //- Số lượng có phần thập phân thì ghi đủ phần thập phân phía sau
        /// </summary>
        /// <param name="strValue"></param>
        /// <returns></returns>
        internal string GetValueSpinNew(string strValue)
        {
            string value = "";
            double dValue = 0;
            try
            {
                if (!String.IsNullOrEmpty(strValue))
                {
                    //TODO
                    string vl = strValue;
                    vl = vl.Replace(".", System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator);
                    vl = vl.Replace(",", System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator);
                    if (vl.Contains("/"))
                    {
                        var arrNumber = vl.Split('/');
                        if (arrNumber != null && arrNumber.Count() > 1)
                        {
                            dValue = Convert.ToDouble(arrNumber[0]) / Convert.ToDouble(arrNumber[1]);
                        }
                    }
                    else
                    {
                        dValue = Convert.ToDouble(vl);
                    }

                    if (dValue == (int)dValue)
                    {
                        value = ConvertNumber.ProcessNumberInterger((decimal)dValue);
                    }
                    else if (vl.Contains("/"))
                    {
                        value = vl;
                    }
                    else
                    {
                        value = Inventec.Common.Number.Convert.NumberToStringRoundAuto((decimal)dValue, 6);
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

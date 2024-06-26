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
using HIS.Desktop.Plugins.AssignPrescriptionKidney.ADO;
using HIS.Desktop.Plugins.AssignPrescriptionKidney.Config;
using HIS.Desktop.Plugins.AssignPrescriptionKidney.Resources;
using HIS.Desktop.Plugins.AssignPrescriptionKidney.Worker;
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

namespace HIS.Desktop.Plugins.AssignPrescriptionKidney.AssignPrescription
{
    public partial class frmAssignPrescription : HIS.Desktop.Utility.FormBase
    {
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
                if (HisConfigCFG.ManyDayPrescriptionOption == 2 && (GlobalStore.IsTreatmentIn))
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
                this.ValidDataMediMaty();
                if (this.ProcessValidMedicineTypeAge(true) && this.ProcessAmountInStockWarning())
                {
                    //Nothing
                }

                this.gridControlServiceProcess.DataSource = null;
                this.gridControlServiceProcess.DataSource = this.mediMatyTypeADOs;
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
                if (this.mediMatyTypeADOs != null && this.mediMatyTypeADOs.Count > 0)
                {
                    this.idRow = 1;
                    this.gridControlServiceProcess.DataSource = null;
                    this.mediMatyTypeADOs = new List<MediMatyTypeADO>();
                }
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
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => currentMedicineTypeADOForEdit), currentMedicineTypeADOForEdit));
                var selectedOpionGroup = GetSelectedOpionGroup();
                if (selectedOpionGroup == 1 && this.currentMedicineTypeADOForEdit != null && this.currentMedicineTypeADOForEdit.DataType != HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC_DM && this.currentMedicineTypeADOForEdit.DataType != HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.VATTU_DM
                    && this.currentMedicineTypeADOForEdit.DataType != HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.VATTU_TSD)
                {
                    dataType = (this.currentMedicineTypeADOForEdit.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC ?
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
                HIS.Desktop.Plugins.AssignPrescriptionKidney.Edit.IEdit iEdit = HIS.Desktop.Plugins.AssignPrescriptionKidney.Edit.EditFactory.MakeIEdit(
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
                HIS.Desktop.Plugins.AssignPrescriptionKidney.Add.IAdd iAdd = HIS.Desktop.Plugins.AssignPrescriptionKidney.Add.AddFactory.MakeIAdd(
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
                var selectedOpionGroup = GetSelectedOpionGroup();

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
                if (this.intructionTimeSelecteds == null || this.intructionTimeSelecteds.Count == 0)
                    this.intructionTimeSelecteds = this.ucDateProcessor.GetValue(this.ucDate);

                UC.DateEditor.ADO.DateInputADO dateInputADO = new UC.DateEditor.ADO.DateInputADO();
                dateInputADO.Time = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(this.intructionTimeSelecteds.First()) ?? new DateTime();
                dateInputADO.Dates = new List<DateTime?>();
                dateInputADO.Dates.Add(dateInputADO.Time);

                spinAmount.Enabled =
                    txtMediMatyForPrescription.Enabled
                        = spinSoLuongNgay.Enabled
                        = spinSang.Enabled
                        = spinTrua.Enabled
                        = spinChieu.Enabled
                        = spinToi.Enabled
                        = spinTocDoTruyen.Enabled
                        = txtTutorial.Enabled
                        = cboHtu.Enabled = true;

                txtMedicineTypeOther.Enabled = spinPrice.Enabled = txtUnitOther.Enabled = false;
                dateInputADO.IsVisibleMultiDate = true;
                if (this.actionType != GlobalVariables.ActionEdit)
                {
                    this.ucDateProcessor.Reload(this.ucDate, dateInputADO);
                    this.isMultiDateState = this.ucDateProcessor.GetChkMultiDateState(this.ucDate);
                    this.intructionTimeSelecteds = this.ucDateProcessor.GetValue(this.ucDate);
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
                                        .Sum(o => ((o.AMOUNT ?? 0) - (o.PRE_AMOUNT ?? 0)));

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
                this.cboHtu.EditValue = null;
                this.cboHtu.Properties.Buttons[1].Visible = false;
                this.spinSang.EditValue = null;
                this.spinTrua.EditValue = null;
                this.spinChieu.EditValue = null;
                this.spinToi.EditValue = null;
                this.spinAmount.Text = "";
                this.spinTocDoTruyen.EditValue = null;
                this.lciTocDoTruyen.Enabled = true;
                this.txtMedicineTypeOther.Text = "";
                this.txtUnitOther.Text = "";
                this.spinPrice.EditValue = null;
                this.btnAddTutorial.Enabled = false;
                this.spinSoLuongNgay.Text = "";
                if (String.IsNullOrEmpty(txtLadder.Text))
                    this.txtTutorial.Text = "";

                Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(this.dxValidProviderBoXung, this.dxErrorProvider1);
                Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(this.dxValidationProviderMaterialTypeTSD, this.dxErrorProvider1);
                Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(this.dxValidProviderBoXung__DuongDung, this.dxErrorProvider1);

                gridControlTutorial.DataSource = null;
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
                    this.btnSave.Enabled = btnSaveAndPrint.Enabled = (serviceCheckeds__Send != null && serviceCheckeds__Send.Count > 0);
                    this.lciPrintAssignPrescription.Enabled = false;
                    this.btnAdd.Enabled = true;
                    this.gridViewServiceProcess.OptionsBehavior.Editable = true;
                }
                else if (this.actionType == GlobalVariables.ActionEdit)
                {
                    List<MediMatyTypeADO> serviceCheckeds__Send = this.mediMatyTypeADOs;
                    this.btnSave.Enabled = btnSaveAndPrint.Enabled = true;
                    this.lciPrintAssignPrescription.Enabled = true;
                    this.btnAdd.Enabled = true;
                    this.btnNew.Enabled = (oldServiceReq != null ? false : true);
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

        private void InitDataByExpMestTemplate()
        {
            try
            {
                if (actionType == GlobalVariables.ActionAdd)
                {
                    if (this.expMestTemplateId.HasValue && this.expMestTemplateId > 0)
                    {
                        MOS.EFMODEL.DataModels.HIS_EXP_MEST_TEMPLATE data = BackendDataWorker.Get<HIS_EXP_MEST_TEMPLATE>().Where(o => o.ID == this.expMestTemplateId).FirstOrDefault();
                        if (data != null)
                        {
                            this.txtExpMestTemplateCode.Text = data.EXP_MEST_TEMPLATE_CODE;
                            this.cboExpMestTemplate.EditValue = data.ID;
                            this.cboExpMestTemplate.Properties.Buttons[1].Visible = true;

                            //Trường hợp chưa chọn kho thì tự động hiển thị kho cho người dùng chọn
                            if (this.currentMediStock == null || this.currentMediStock.Count == 0)
                            {
                                //TODO
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
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitDataBySereServOrServiceReqOld()
        {
            try
            {
                if (actionType == GlobalVariables.ActionAdd)
                {
                    if ((this.ServiceReqMetyWorking != null && this.ServiceReqMetyWorking.ID > 0) || (this.serviceReqWorking != null && this.serviceReqWorking.ID > 0))
                    {
                        //Trường hợp chưa chọn kho thì tự động hiển thị kho cho người dùng chọn
                        if (this.currentMediStock == null || this.currentMediStock.Count == 0)
                        {
                            //TODO
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

                if (this.mediStockD1ADOs == null || this.mediStockD1ADOs.Count == 0)
                    this.InitDataMetyMatyTypeInStockD1(this.currentMediStock);

                if (this.expMestTemplateId > 0)
                {
                    Inventec.Common.Logging.LogSystem.Debug("ProcessAfterChooseformMediStock. 1");
                    MOS.EFMODEL.DataModels.HIS_EXP_MEST_TEMPLATE data = BackendDataWorker.Get<HIS_EXP_MEST_TEMPLATE>().Where(o => o.ID == this.expMestTemplateId).FirstOrDefault();
                    this.ProcessChoiceExpMestTemplate(data);
                }
                else if (this.ServiceReqMetyWorking != null && this.ServiceReqMetyWorking.ID > 0)
                {
                    Inventec.Common.Logging.LogSystem.Debug("ProcessAfterChooseformMediStock. 2");
                    this.ProcessChoiceService();
                }
                else if (this.serviceReqWorking != null && this.serviceReqWorking.ID > 0)
                {
                    Inventec.Common.Logging.LogSystem.Debug("ProcessAfterChooseformMediStock. 3");
                    this.ProcessChoicePrescriptionPrevious(this.serviceReqWorking);
                }
                this.ResetFocusMediMaty(true);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ProcessChoicePrescriptionPrevious(MOS.EFMODEL.DataModels.HIS_SERVICE_REQ serviceReq)
        {
            try
            {
                if (serviceReq == null) return;
                if (this.actionType == GlobalVariables.ActionView)
                {
                    LogSystem.Debug("ProcessChoicePrescriptionPrevious => thao tac khong hop le. actionType = " + this.actionType);
                    return;
                }

                //Release tat ca cac thuoc/ vat tu da duoc take bean truoc do nhung chua duoc luu
                this.ReleaseAllMediByUser();
                //List<V_HIS_SERVICE_REQ_7> serviceReqTemps = new List<V_HIS_SERVICE_REQ_7>();
                //serviceReqTemps.Add(serviceReq);

                //foreach (var item in serviceReqTemps)
                //{
                //if (item.EXP_MEST_ID.HasValue && item.EXP_MEST_TYPE_ID.HasValue)
                //{
                //    this.ProcessGetExpMestMedicine_Prescription(this.GetExpMestMedicineByExpMestId(item.EXP_MEST_ID ?? 0), item, this.intructionTimeSelecteds.First());
                //    this.ProcessGetExpMestMaterial_Prescription(this.GetExpMestMaterialByExpMestId(item.EXP_MEST_ID ?? 0), false);
                //}
                this.ProcessGetServiceReqMety(this.GetServiceReqMetyByServiceReqId(serviceReq.ID), serviceReq, this.intructionTimeSelecteds.First());
                //this.ProcessGetServiceReqMaty(this.GetServiceReqMatyByServiceReqId(item.ID), false);
                //}
                this.ProcessInstructionTimeMediForEdit();
                this.ProcessMergeDuplicateRowForListProcessing();
                this.ProcessAddListRowDataIntoGridWithTakeBean();
                this.ReloadDataAvaiableMediBeanInCombo();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
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
                lciTocDoTruyen.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                grcTocDoTruyen.Visible = true;
                lciHtu.TextSize = new System.Drawing.Size(60, lciTocDoTruyen.Height);
                repositoryItemchkIsExpendType_Disable.Enabled = false;
                repositoryItemchkIsExpendType_Disable.ReadOnly = true;
                repositoryItemChkIsExpend__MedicinePage_Disable.Enabled = false;
                repositoryItemChkIsExpend__MedicinePage_Disable.ReadOnly = true;
                lciHomePres.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
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
                this.lblTongTien.Text = "";
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

                this.actionType = GlobalVariables.ActionAdd;
                this.actionBosung = GlobalVariables.ActionAdd;

                this.gridControlServiceProcess.DataSource = null;
                this.mediMatyTypeADOs = new List<MediMatyTypeADO>();
                this.cboExpMestTemplate.EditValue = null;
                this.cboExpMestTemplate.Properties.Buttons[1].Visible = false;
                this.txtExpMestTemplateCode.Text = "";
                //this.spinKidneyCount.EditValue = null;
                //this.chkPreKidneyShift.Checked = false;

                GlobalStore.ClientSessionKey = Guid.NewGuid().ToString();

                this.currentMedicineTypes = BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>().Where(o => o.IS_LEAF == GlobalVariables.CommonNumberTrue).ToList();
                //long isOnlyDisplayMediMateIsBusiness = Inventec.Common.TypeConvert.Parse.ToInt64(HisConfigs.Get<string>(HisConfigCFG.ONLY_DISPLAY_MEDIMATE_IS_BUSINESS));
                //if (isOnlyDisplayMediMateIsBusiness == 1 && this.currentMedicineTypes != null && this.currentMedicineTypes.Count > 0)
               //     this.currentMedicineTypes = this.currentMedicineTypes.Where(o => o.IS_BUSINESS.HasValue && o.IS_BUSINESS.Value == 1).ToList();

                this.currentMaterialTypes = BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>().Where(o => o.IS_LEAF == GlobalVariables.CommonNumberTrue).ToList();

                Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(this.dxValidationProviderControl, dxErrorProvider1);

                //Ẩn số thang theo cấu hình
                long prescriptionIsVisiableRemedyCount = ConfigApplicationWorker.Get<long>(AppConfigKeys.CONFIG_KEY__HIS_DESKTOP__ASSIGN_PRESCRIPTION__ISVISIBLE_REMEDY_COUNT);
                if (prescriptionIsVisiableRemedyCount == 1)
                {
                    lciLadder.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                }

                cboPatientType.EditValue = null;
                cboPatientType.Properties.DataSource = null;
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
                        result = (medi1.AMOUNT ?? 0);
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
                if (this.mediStockD1ADOs == null || this.mediStockD1ADOs.Count == 0)
                    this.InitDataMetyMatyTypeInStockD1(this.currentMediStock);

                var result1 = this.mediStockD1ADOs.FirstOrDefault(o => o.SERVICE_ID == serviceId && (meidStockId == 0 || o.MEDI_STOCK_ID == meidStockId));
                if (result1 != null && (model.AMOUNT ?? 0) > (result1.AMOUNT ?? 0))
                {

                }
                result = result1;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private async Task FillTreatmentInfo__PatientType()
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
                if (this.mediStockD1ADOs == null || this.mediStockD1ADOs.Count == 0)
                    InitDataMetyMatyTypeInStockD1(this.currentMediStock);

                if (this.mediStockD1ADOs != null && this.mediStockD1ADOs.Count > 0)
                {
                    var medicineInStock = this.mediStockD1ADOs.FirstOrDefault(o => o.SERVICE_ID == model.SERVICE_ID);//Mac dinh lay kho dau tien
                    //Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => medicineInStock), medicineInStock));
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
                    Inventec.Common.Logging.LogSystem.Debug("this.InstructionTime " + this.InstructionTime);

                    this.intructionTimeSelecteds = this.ucDateProcessor.GetValue(this.ucDate);
                    this.isMultiDateState = this.ucDateProcessor.GetChkMultiDateState(this.ucDate);
                    this.InstructionTime = intructionTimeSelecteds.OrderByDescending(o => o).First();


                    var q1 = (from m in listEmteMedcineType
                              select new MediMatyTypeADO(m, this.InstructionTime)).ToList();
                    if (q1 != null && q1.Count > 0)
                        this.mediMatyTypeADOs.AddRange(q1);
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

        private void ProcessChoiceService()
        {
            try
            {
                //Release tat ca cac thuoc/ vat tu da duoc take bean truoc do nhung chua duoc luu
                this.ReleaseAllMediByUser();

                MediMatyTypeADO metyADO = new MediMatyTypeADO(this.ServiceReqMetyWorking, this.intructionTimeSelecteds.First(), this.serviceReqWorking);

                this.mediMatyTypeADOs.Add(metyADO);
                this.ProcessInstructionTimeMediForEdit();
                this.ProcessAddListRowDataIntoGridWithTakeBean();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
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

                this.ProcessGetEmteMedcineType(this.GetEmteMedicineTypeByExpMestId(expTemplate.ID));
                this.ProcessGetEmteMaterialType(this.GetEmteMaterialTypeByExpMestId(expTemplate.ID));
                this.ProcessInstructionTimeMediForEdit();
                this.ProcessMergeDuplicateRowForListProcessing();
                this.ProcessAddListRowDataIntoGridWithTakeBean();
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
                this.ReloadDataAvaiableMediBeanInCombo();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
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
                        if (this.mediStockD1ADOs != null && this.mediStockD1ADOs.Count > 0)
                        {
                            var dMediStock1ADOs = this.mediStockD1ADOs.Where(o => o.ID == item.ID && (item.MEDI_STOCK_ID == null || item.MEDI_STOCK_ID == 0 || o.MEDI_STOCK_ID == item.MEDI_STOCK_ID)).ToList();

                            if (dMediStock1ADOs == null || dMediStock1ADOs.Count == 0 || item.AMOUNT > dMediStock1ADOs[0].AMOUNT)
                            {
                                ado.AmountAlert = item.AMOUNT;
                                result.Add(ado);
                                continue;
                            }

                            if (item.AMOUNT > 0)
                            {
                                if (dMediStock1ADOs[0].AMOUNT < item.AMOUNT)
                                {
                                    ado.AMOUNT = dMediStock1ADOs[0].AMOUNT;
                                    item.AMOUNT = item.AMOUNT - dMediStock1ADOs[0].AMOUNT;
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
        private void ProcessGetServiceReqMety(List<HIS_SERVICE_REQ_METY> lstExpMestMety, HIS_SERVICE_REQ serviceReq, long currentInstructionTime)
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
                //if (this.currentSereServ != null)
                //    result = this.currentSereServ.ID;

                //if (this.currentSereServInEkip != null)
                //    result = this.currentSereServInEkip.ID;
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

        private void CalculateAmount()
        {
            try
            {
                double sang, trua, chieu, toi = 0;
                sang = this.GetValueSpin(this.spinSang.Text);
                trua = this.GetValueSpin(this.spinTrua.Text);
                chieu = this.GetValueSpin(this.spinChieu.Text);
                toi = this.GetValueSpin(this.spinToi.Text);
                if (sang > 0
                    || trua > 0
                    || chieu > 0
                    || toi > 0)
                {
                    long roomTypeId = GetRoomTypeId();
                    double tongCong = (sang + trua + chieu + toi);

                    if (currentMedicineTypeADOForEdit != null && currentMedicineTypeADOForEdit.ID > 0)
                    {
                        bool isNotOdd = false;
                        if (GlobalStore.IsTreatmentIn && ((currentMedicineTypeADOForEdit.IsAllowOdd.HasValue && currentMedicineTypeADOForEdit.IsAllowOdd.Value == true) || (currentMedicineTypeADOForEdit.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__VT)))
                        {

                        }
                        else
                        {
                            isNotOdd = true;
                        }

                        if (isNotOdd)
                        {
                            int plusSeperate = 1;
                            double amount = ((double)this.spinSoLuongNgay.Value * tongCong);


                            this.spinAmount.Text = (Inventec.Common.Number.Convert.RoundUpValue(amount, 0) != (int)(amount) ? (int)(amount) + plusSeperate : amount).ToString();
                            if (amount != (int)(amount))
                            {
                                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => amount), amount) + "____" + Inventec.Common.Logging.LogUtil.TraceData("(int)amount", (int)(amount)) + "____" + Inventec.Common.Logging.LogUtil.TraceData("spinSoLuongNgay.Value", spinSoLuongNgay.Value));
                            }
                        }
                        else
                        {
                            this.spinAmount.Text = ((double)(this.spinSoLuongNgay.Value) * (tongCong)) + "";
                        }
                    }
                }
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
                var mediMate = this.mediMatyTypeADOs != null ? this.mediMatyTypeADOs.FirstOrDefault(o => !String.IsNullOrEmpty(o.TUTORIAL)) : null;
                if (!String.IsNullOrEmpty(txtLadder.Text) && mediMate != null)
                    return;
                string serviceUnitName = "";
                bool isUse = false;
                if (this.currentMedicineTypeADOForEdit != null)
                {
                    isUse = true;
                    serviceUnitName = !String.IsNullOrEmpty(this.currentMedicineTypeADOForEdit.CONVERT_UNIT_NAME) ? this.currentMedicineTypeADOForEdit.CONVERT_UNIT_NAME : (this.currentMedicineTypeADOForEdit.SERVICE_UNIT_NAME ?? "").ToLower();
                }
                if (isUse)
                {
                    StringBuilder huongDan = new StringBuilder();
                    string format__NgayUong = ResourceMessage.NgayUong;
                    string format__NgayUongTemp2 = ResourceMessage.NgayUongTemp2;
                    string format___NgayXVienBuoiYZ = ResourceMessage._NgayXVienBuoiYZ;
                    string format__Sang = ResourceMessage.Sang;
                    string format__Trua = ResourceMessage.Trua;
                    string format__Chieu = ResourceMessage.Chieu;
                    string format__Toi = ResourceMessage.Toi;
                    string strSeperator = ", ";
                    int solan = 0;
                    string buoiChon = "";
                    double sang, trua, chieu, toi = 0;
                    sang = this.GetValueSpin(this.spinSang.Text);
                    trua = this.GetValueSpin(this.spinTrua.Text);
                    chieu = this.GetValueSpin(this.spinChieu.Text);
                    toi = this.GetValueSpin(this.spinToi.Text);
                    if (sang > 0
                    || trua > 0
                    || chieu > 0
                    || toi > 0)
                    {
                        if (sang > 0) { solan += 1; buoiChon = ResourceMessage.BuoiSang; }
                        if (trua > 0) { solan += 1; buoiChon = ResourceMessage.BuoiTrua; }
                        if (chieu > 0) { solan += 1; buoiChon = ResourceMessage.BuoiChieu; }
                        if (toi > 0) { solan += 1; buoiChon = ResourceMessage.BuoiToi; }
                        double tongCong = (sang + trua + chieu + toi);

                        if (solan == 1)
                        {
                            if ((int)tongCong == tongCong)
                                huongDan.Append(!String.IsNullOrEmpty(this.spinAmount.Text) ? String.Format(format___NgayXVienBuoiYZ, (String.IsNullOrEmpty(this.cboMedicineUseForm.Text) ? "" : this.cboMedicineUseForm.Text + " "), "" + (int)tongCong, serviceUnitName, buoiChon) : "");
                            else
                                huongDan.Append(!String.IsNullOrEmpty(this.spinAmount.Text) ? String.Format(format___NgayXVienBuoiYZ, (String.IsNullOrEmpty(this.cboMedicineUseForm.Text) ? "" : this.cboMedicineUseForm.Text + ""), this.ConvertDecToFracByConfig(tongCong), serviceUnitName, buoiChon) : "");
                        }
                        else
                        {
                            if (HisConfigCFG.TutorialFormat != 2)
                            {
                                if ((int)tongCong == tongCong)
                                    huongDan.Append(tongCong > 0 ? String.Format(format__NgayUong, (String.IsNullOrEmpty(this.cboMedicineUseForm.Text) ? "" : " " + this.cboMedicineUseForm.Text.ToLower() + " "), " " + (int)tongCong, serviceUnitName, solan) : "");
                                else
                                    huongDan.Append(tongCong > 0 ? String.Format(format__NgayUong, (String.IsNullOrEmpty(this.cboMedicineUseForm.Text) ? "" : " " + this.cboMedicineUseForm.Text.ToLower() + " "), this.ConvertDecToFracByConfig(tongCong), serviceUnitName, solan) : "");
                            }
                            else
                            {
                                huongDan.Append(tongCong > 0 ? String.Format(format__NgayUongTemp2, (String.IsNullOrEmpty(this.cboMedicineUseForm.Text) ? "" : " " + this.cboMedicineUseForm.Text.ToLower() + " "), "", "", solan) : "");
                            }

                            huongDan.Append(sang > 0 ? String.Format(format__Sang, this.ConvertDecToFracByConfig(sang), serviceUnitName) : "");
                            huongDan.Append(trua > 0 ? ((String.IsNullOrEmpty(huongDan.ToString()) ? "" : strSeperator) + String.Format(format__Trua, this.ConvertDecToFracByConfig(trua), serviceUnitName)) : "");
                            huongDan.Append(chieu > 0 ? ((String.IsNullOrEmpty(huongDan.ToString()) ? "" : strSeperator) + String.Format(format__Chieu, this.ConvertDecToFracByConfig(chieu), serviceUnitName)) : "");
                            huongDan.Append(toi > 0 ? ((String.IsNullOrEmpty(huongDan.ToString()) ? "" : strSeperator) + String.Format(format__Toi, this.ConvertDecToFracByConfig(toi), serviceUnitName)) : "");
                        }

                        huongDan.Append(!String.IsNullOrEmpty(this.cboHtu.Text) ? " (" + this.cboHtu.Text + ")" : "");
                        string hdTemp = huongDan.ToString().Replace("  ", " ").Replace(", ,", ",");
                        huongDan = new StringBuilder().Append(hdTemp.First().ToString().ToUpper() + String.Join("", hdTemp.Skip(1)).ToLower());
                    }
                    this.txtTutorial.Text = huongDan.ToString();
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

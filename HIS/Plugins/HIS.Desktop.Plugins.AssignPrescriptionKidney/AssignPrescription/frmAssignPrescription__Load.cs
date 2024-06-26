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
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.AssignPrescriptionKidney.ADO;
using HIS.Desktop.Plugins.AssignPrescriptionKidney.Config;
using HIS.Desktop.Plugins.AssignPrescriptionKidney.Resources;
using HIS.UC.Icd.ADO;
using HIS.UC.SecondaryIcd.ADO;
using Inventec.Common.Adapter;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.AssignPrescriptionKidney.AssignPrescription
{
    public partial class frmAssignPrescription : HIS.Desktop.Utility.FormBase
    {
        private async Task LoadAllergenic(long patientId)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("LoadAllergenic. 1");
                CommonParam param = new CommonParam();
                HisAllergenicFilter filter = new HisAllergenicFilter();
                filter.TDL_PATIENT_ID = patientId;
                this.allergenics = await new BackendAdapter(param)
                    .GetAsync<List<MOS.EFMODEL.DataModels.HIS_ALLERGENIC>>("api/HisAllergenic/Get", ApiConsumers.MosConsumer, filter, param);
                Inventec.Common.Logging.LogSystem.Debug("LoadAllergenic. 2");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private async Task ThreadLoadDonThuocCu()
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("ThreadLoadDonThuocCu. 1");
                //Neu la in gop don thuoc thi moi load
                string savePrintMpsDefault = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(HisConfigCFG.SAVE_PRINT_MPS_DEFAULT);
                if (savePrintMpsDefault != "Mps000234")
                    return;

                CommonParam param = new CommonParam();
                //Load đơn phòng khám
                HisServiceReqFilter serviceReqFilter = new HisServiceReqFilter();
                serviceReqFilter.TREATMENT_ID = this.treatmentId;
                serviceReqFilter.SERVICE_REQ_TYPE_IDs = new List<long> { IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONK };
                serviceReqPrints = await new BackendAdapter(param)
                      .GetAsync<List<MOS.EFMODEL.DataModels.HIS_SERVICE_REQ>>("api/HisServiceReq/Get", ApiConsumers.MosConsumer, serviceReqFilter, param);

                if (serviceReqPrints == null || serviceReqPrints.Count == 0)
                    return;
                //Load expmest
                serviceReqPrints = serviceReqPrints.Where(o => o.ID != this.serviceReqParentId).ToList();
                HisExpMestFilter expMestFilter = new HisExpMestFilter();
                expMestFilter.SERVICE_REQ_IDs = serviceReqPrints.Select(o => o.ID).ToList();
                expMestPrints = await new BackendAdapter(param)
                     .GetAsync<List<MOS.EFMODEL.DataModels.HIS_EXP_MEST>>("api/HisExpMest/Get", ApiConsumers.MosConsumer, expMestFilter, param);

                if (expMestPrints == null || expMestPrints.Count == 0)
                    return;

                //Laays thuoc va tu trong kho

                HisExpMestMedicineFilter expMestMedicineFilter = new HisExpMestMedicineFilter();
                expMestMedicineFilter.EXP_MEST_IDs = expMestPrints.Select(o => o.ID).ToList();
                expMestMedicinePrints = await new BackendAdapter(param)
                    .GetAsync<List<MOS.EFMODEL.DataModels.HIS_EXP_MEST_MEDICINE>>("api/HisExpMestMedicine/Get", ApiConsumers.MosConsumer, expMestMedicineFilter, param);

                HisExpMestMaterialFilter expMestMaterialFilter = new HisExpMestMaterialFilter();
                expMestMaterialFilter.EXP_MEST_IDs = expMestPrints.Select(o => o.ID).ToList();
                expMestMaterialPrints = await new BackendAdapter(param)
                    .GetAsync<List<MOS.EFMODEL.DataModels.HIS_EXP_MEST_MATERIAL>>("api/HisExpMestMaterial/Get", ApiConsumers.MosConsumer, expMestMaterialFilter, param);
                Inventec.Common.Logging.LogSystem.Debug("ThreadLoadDonThuocCu. 2");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitWorker()
        {
            try
            {
                AssignPrescriptionWorker.MediMatyCreateWorker = new MediMatyCreateWorker(GetDataAmountOutOfStock, SetDefaultMediStockForData, ChoosePatientTypeDefaultlService, GetPatientTypeId, GetNumRow, SetNumRow, GetMediMatyTypeADOs, GetIsAutoCheckExpend);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private List<MediMatyTypeADO> GetMediMatyTypeADOs()
        {
            try
            {
                return mediMatyTypeADOs;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return new List<MediMatyTypeADO>();
        }

        private bool GetIsAutoCheckExpend()
        {
            try
            {
                return isAutoCheckExpend;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return false;
        }

        private long GetPatientTypeId()
        {
            try
            {
                return currentHisPatientTypeAlter.PATIENT_TYPE_ID;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return 0;
        }

        private int GetNumRow()
        {
            try
            {
                return idRow;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return 1;
        }

        private void SetNumRow()
        {
            try
            {
                idRow += stepRow;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Nếu có thẻ cấu hình số ngày hẹn khám mặc định (EXE.HIS_TREATMENT_END.APPOINTMENT_TIME_DEFAULT) và kiểm tra xem hồ sơ điều trị hiện tại có phải đến theo diện hẹn khám hay không (trong his_treatment trường appointment_id có khác null hay ko).
        /// Nếu ko thì xử lý như cũ (ko tự động điền số ngày)
        /// Nếu có thì tiếp tục theo luồng sau:
        /// Kiểm tra thời gian hẹn khám trong thông tin hẹn khám (tương ứng với appointment_id trong treament) là ngày nào. So sánh ngày đó với ngày hiện tại. Lúc đó, số ngày điền sẵn vào đơn sẽ theo công thức:
        /// Số ngày trên đơn = Số ngày hẹn khám mặc định - MIN ((ngày hẹn khám - ngày hiện tại), 0)
        /// </summary>
        private async Task LoadDefaultSoNgayHoaDonFromAppointmentTimeDefault()
        {
            try
            {
                if (String.IsNullOrEmpty(HisConfigCFG.AppointmentTimeDefault)) return;
                if (String.IsNullOrEmpty(this.currentTreatmentWithPatientType.APPOINTMENT_CODE)) return;
                long songayHKmacDinh = Inventec.Common.TypeConvert.Parse.ToInt64(HisConfigCFG.AppointmentTimeDefault);
                if (songayHKmacDinh <= 0) return;

                CommonParam param = new CommonParam();
                HisTreatmentFilter treatmentFilter = new HisTreatmentFilter();
                treatmentFilter.TREATMENT_CODE__EXACT = this.currentTreatmentWithPatientType.APPOINTMENT_CODE;
                var appointmentTreatments = await new BackendAdapter(param).GetAsync<List<HIS_TREATMENT>>(RequestUriStore.HIS_TREATMENT_GET, ApiConsumers.MosConsumer, treatmentFilter, ProcessLostToken, param);
                if (appointmentTreatments != null && appointmentTreatments.Count > 0)
                {
                    //Kiểm tra thời gian hẹn khám trong thông tin hẹn khám (tương ứng với appointment_id trong treament) là ngày nào.
                    //So sánh ngày đó với ngày hiện tại. Lúc đó, số ngày điền sẵn vào đơn sẽ theo công thức:
                    //Số ngày trên đơn = Số ngày hẹn khám mặc định - MIN ((ngày hẹn khám - ngày hiện tại), 0)
                    System.DateTime dtAppointmentTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(appointmentTreatments[0].APPOINTMENT_TIME ?? 0).Value;
                    TimeSpan diff__hour = (dtAppointmentTime.Date - System.DateTime.Now.Date);
                    double totaldays = diff__hour.TotalDays;
                    long songaytrendon = 0;
                    if (totaldays > 0)
                    {
                        songaytrendon = (long)(songayHKmacDinh - (long)totaldays);
                        this.spinSoNgay.EditValue = songaytrendon;
                    }
                    else
                    {
                        songaytrendon = (long)songayHKmacDinh;
                        this.spinSoNgay.EditValue = songaytrendon;
                    }
                    Inventec.Common.Logging.LogSystem.Debug("Truong hop co cau hinh su dung so ngay hen kham mac dinh(EXE.HIS_TREATMENT_END.APPOINTMENT_TIME_DEFAULT) va ho so hien tai là den hen kham --> tinh so ngay tren don  gan vao spinSoNgay" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => totaldays), totaldays) + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => songaytrendon), songaytrendon) + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => songayHKmacDinh), songayHKmacDinh));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Nếu phòng đang làm việc là buồng bệnh -> cho phép nhập số lượng nguyên, thập phân, phân số
        /// ô số lượng nhập số: 1; 1.15; 1/2
        /// số lượng trên grid chi tiết nhập số lượng: nguyên, thập phân vd: 1; 1.15
        /// </summary>
        private void SetControlSoLuongNgayNhapChanLe(MediMatyTypeADO mediMatyADO)
        {
            try
            {
                if ((GlobalStore.IsTreatmentIn) && ((mediMatyADO.IsAllowOdd.HasValue && mediMatyADO.IsAllowOdd.Value == true) || (mediMatyADO.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__VT)))
                {
                    this.spinAmount.Properties.DisplayFormat.FormatString = "#,##0.00";
                    this.spinAmount.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.None;
                    this.spinAmount.Properties.EditFormat.FormatString = "#,##0.00";
                    this.spinAmount.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.None;
                    this.spinAmount.Properties.Mask.EditMask = "n";
                    this.spinAmount.Properties.Mask.UseMaskAsDisplayFormat = true;

                    //this.repositoryItemSpinAmount__MedicinePage.Properties.DisplayFormat.FormatString = "#,##0.00";
                    //this.repositoryItemSpinAmount__MedicinePage.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.None;
                    //this.repositoryItemSpinAmount__MedicinePage.Properties.EditFormat.FormatString = "#,##0.00";
                    //this.repositoryItemSpinAmount__MedicinePage.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.None;
                    //this.repositoryItemSpinAmount__MedicinePage.Properties.Mask.EditMask = "n";
                    //this.repositoryItemSpinAmount__MedicinePage.Properties.Mask.UseMaskAsDisplayFormat = true;
                }
                else
                {
                    this.spinAmount.Properties.Mask.EditMask = "d";
                    this.spinAmount.Properties.Mask.UseMaskAsDisplayFormat = true;

                    //this.repositoryItemSpinAmount__MedicinePage.Properties.Mask.EditMask = "d";
                    //this.repositoryItemSpinAmount__MedicinePage.Properties.Mask.UseMaskAsDisplayFormat = true;
                }
                this.spinAmount.Update();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Kiểm tra cấu hình ẩn hiện cột chi phí ngoài gói và hao phí trong grid danh sách thuốc/vật tư đã chọn
        /// </summary>
        private void VisibleColumnInGridControlService()
        {
            try
            {
                //An hien cot cp ngoai goi
                long isVisibleColumnCPNgoaiGoi = ConfigApplicationWorker.Get<long>(AppConfigKeys.CONFIG_KEY__MODULE_CHI_DINH_DICH_VU__AN_HIEN_CP_NGOAI_GOI);
                if (isVisibleColumnCPNgoaiGoi == 1)
                {
                    this.grcIsOutKtcFee__TabMedicine.Visible = false;
                }

                //An hien cot hao phi
                long isVisibleColumnHaoPhi = ConfigApplicationWorker.Get<long>(AppConfigKeys.CONFIG_KEY__MODULE_CHI_DINH_DICH_VU__AN_HIEN_HAO_PHI);
                if (isVisibleColumnHaoPhi == 1)
                {
                    this.grcExpend__TabMedicine.Visible = false;
                    this.grcIsExpendType.Visible = false;
                }

                if (HisConfigCFG.IsNotAllowingExpendWithoutHavingParent && this.GetSereServInKip() <= 0)
                {
                    this.grcExpend__TabMedicine.Visible = false;
                    this.grcIsExpendType.Visible = false;
                }
                if (GlobalStore.IsTreatmentIn)
                {
                    this.grcTotalPrice__TabMedicine.Visible = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private async Task FillDataToControlsForm()
        {
            try
            {
                InitComboRepositoryPatientType(repositoryItemcboPatientType_TabMedicine_GridLookUp, currentPatientTypeWithPatientTypeAlter);
                InitComboRepositoryPatientType(repositoryItemcboPatientType_TabMedicine_GridLookUp__Disable, BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE>());

                this.InitComboEquipment();
                this.InitComboHtu(null);
                this.InitComboUser();
                this.InitComboMedicineUseForm(cboMedicineUseForm, null);
                this.InitComboMedicineUseForm(repositoryItemcboMedicineUseForm, null);
                this.InitComboExpMestTemplate();
                this.InitComboMediStockAllow(0);
                this.InitMedicineTypeAcinInfo();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private async Task InitMedicineTypeAcinInfo()
        {
            try
            {
                CommonParam param = new CommonParam();
                HisMedicineTypeAcinFilter medicineTypeAcinFilter = new HisMedicineTypeAcinFilter();
                //medicineTypeAcinFilter.MEDICINE_TYPE_IDs = mediTypeIds;//TODO
                this.currentMedicineTypeAcins = await new BackendAdapter(param)
                .GetAsync<List<MOS.EFMODEL.DataModels.HIS_MEDICINE_TYPE_ACIN>>("api/HisMedicineTypeAcin/Get", ApiConsumers.MosConsumer, medicineTypeAcinFilter, param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FillAllPatientInfoSelectedInForm()
        {
            try
            {
                this.currentTreatmentWithPatientType = this.LoadDataToCurrentTreatmentData(this.treatmentId, this.intructionTimeSelecteds.OrderByDescending(o => o).First());

                this.PatientTypeWithTreatmentView7();
                Task.Run(() => CheckWarningOverTotalPatientPrice());
                this.LoadDataSereServWithTreatment(this.currentTreatmentWithPatientType, 0);

                this.LoadSereServTotalHeinPriceWithTreatment(treatmentId);

                this.FillTreatmentInfo__PatientType();//tinh toan va hien thi thong tin ve tong tien tat ca cac dich vu dang chi dinh                

                this.LoadIcdDefault();

                this.LoadDefaultSoNgayHoaDonFromAppointmentTimeDefault();

                this.InitWorker();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Kiem tra no vien phi
        /// Mức tiền cảnh báo nợ viện phí đối với BN nội trú và tủ trực
        /// </summary>
        private void CheckWarningOverTotalPatientPrice()
        {
            try
            {
                //Kiem tra cau hinh
                if (!HisConfigCFG.IsWarningOverTotalPatientPrice || this.currentHisPatientTypeAlter.TREATMENT_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTNOITRU || this.actionType != GlobalVariables.ActionAdd)
                    return;

                CommonParam param = new CommonParam();
                HisTreatmentFeeViewFilter filter = new HisTreatmentFeeViewFilter();
                filter.ID = treatmentId;
                var treatmentFees = new BackendAdapter(param)
                    .Get<List<MOS.EFMODEL.DataModels.V_HIS_TREATMENT_FEE>>(HisRequestUriStore.HIS_TREATMENT_GETFEEVIEW, ApiConsumers.MosConsumer, filter, param);

                //So tien benh nhan can thu
                if (treatmentFees == null || treatmentFees.Count == 0)
                    return;

                decimal totalPrice = 0;
                decimal totalHeinPrice = 0;
                decimal totalPatientPrice = 0;
                decimal totalDeposit = 0;
                decimal totalBill = 0;
                decimal totalBillTransferAmount = 0;
                decimal totalRepay = 0;
                decimal exemption = 0;
                decimal total_obtained_price = 0;
                totalPrice = treatmentFees[0].TOTAL_PRICE ?? 0; // tong tien
                totalHeinPrice = treatmentFees[0].TOTAL_HEIN_PRICE ?? 0;
                totalPatientPrice = treatmentFees[0].TOTAL_PATIENT_PRICE ?? 0; // bn tra
                totalDeposit = treatmentFees[0].TOTAL_DEPOSIT_AMOUNT ?? 0;
                totalBill = treatmentFees[0].TOTAL_BILL_AMOUNT ?? 0;
                totalBillTransferAmount = treatmentFees[0].TOTAL_BILL_TRANSFER_AMOUNT ?? 0;
                exemption = treatmentFees[0].TOTAL_BILL_EXEMPTION ?? 0;// HospitalFeeSum[0].TOTAL_EXEMPTION ?? 0;
                totalRepay = treatmentFees[0].TOTAL_REPAY_AMOUNT ?? 0;
                total_obtained_price = (totalDeposit + totalBill - totalBillTransferAmount - totalRepay + exemption);//Da thu benh nhan
                decimal transfer = totalPatientPrice - total_obtained_price;//Phai thu benh nhan

                decimal warningOverTotalPatientPrice = HisConfigCFG.WarningOverTotalPatientPrice;
                if (transfer > warningOverTotalPatientPrice)
                {
                    DialogResult myResult;
                    myResult = MessageBox.Show(this, String.Format("Bệnh nhân đang thiếu viện phí ({0} đồng). Bạn có muốn tiếp tục?", Inventec.Common.Number.Convert.NumberToString(transfer, ConfigApplications.NumberSeperator)), "Thông báo", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                    if (myResult != DialogResult.OK)
                    {
                        this.Close();
                    }

                    txtMediMatyForPrescription.Focus();
                    txtMediMatyForPrescription.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        int GetSelectedOpionGroup()
        {
            int selectedOpionGroup = 1;
            try
            {
                int iSelectedIndex = this.rdOpionGroup.SelectedIndex;
                iSelectedIndex = iSelectedIndex == -1 ? 0 : iSelectedIndex;
                selectedOpionGroup = (int)this.rdOpionGroup.Properties.Items[iSelectedIndex].Value;
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => selectedOpionGroup), selectedOpionGroup) + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => this.rdOpionGroup.SelectedIndex), this.rdOpionGroup.SelectedIndex) + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => this.rdOpionGroup.Properties.Items.Count), this.rdOpionGroup.Properties.Items.Count));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => rdOpionGroup.SelectedIndex), rdOpionGroup.SelectedIndex) + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => selectedOpionGroup), selectedOpionGroup) + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => this.rdOpionGroup.Properties.Items.Count), this.rdOpionGroup.Properties.Items.Count), ex);
            }

            return selectedOpionGroup;
        }

        internal void ReloadDataAvaiableMediBeanInCombo()
        {
            try
            {
                //var selectedOpionGroup = GetSelectedOpionGroup();
                //var extMediBean = this.mediMatyTypeADOs.Any(o =>
                //    o.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC
                //    || o.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.VATTU
                //    || o.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.VATTU_TSD);
                //if (extMediBean)
                //{
                //    this.RebuildMediMatyWithInControlContainer(selectedOpionGroup == 3);
                //}

                this.SetEnableButtonControl(this.actionType);
                this.SetTotalPrice__TrongDon();

                if (this.gridViewServiceProcess.DataRowCount == 0)
                    this.idRow = 1;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Lay Chan doan mac dinh: Lay chan doan cuoi cung trong cac xu ly dich vu Kham benh
        /// </summary>
        private void LoadIcdDefault()
        {
            try
            {
                LogSystem.Debug("Load LoadIcdDefault .1");
                if (this.currentTreatmentWithPatientType != null)
                {
                    IcdInputADO icd = new IcdInputADO();
                    icd.ICD_CODE = currentTreatmentWithPatientType.ICD_CODE;
                    icd.ICD_NAME = currentTreatmentWithPatientType.ICD_NAME;
                    if (ucIcd != null)
                    {
                        icdProcessor.Reload(ucIcd, icd);
                    }

                    IcdInputADO icdCause = new IcdInputADO();
                    icdCause.ICD_CODE = currentTreatmentWithPatientType.ICD_CAUSE_CODE;
                    icdCause.ICD_NAME = currentTreatmentWithPatientType.ICD_CAUSE_NAME;
                    if (ucIcdCause != null)
                    {
                        icdCauseProcessor.Reload(ucIcdCause, icdCause);
                    }

                    var icdCaus = BackendDataWorker.Get<HIS_ICD>().FirstOrDefault(o => o.ICD_CODE == currentTreatmentWithPatientType.ICD_CODE);
                    if (icdCaus != null)
                    {
                        this.icdCauseProcessor.SetRequired(this.ucIcdCause, (icdCaus.IS_REQUIRE_CAUSE == 1));
                    }

                    SecondaryIcdDataADO subIcd = new SecondaryIcdDataADO();
                    subIcd.ICD_SUB_CODE = currentTreatmentWithPatientType.ICD_SUB_CODE;
                    subIcd.ICD_TEXT = currentTreatmentWithPatientType.ICD_TEXT;
                    if (ucSecondaryIcd != null)
                    {
                        subIcdProcessor.Reload(ucSecondaryIcd, subIcd);
                    }
                }
                LogSystem.Debug("Load LoadIcdDefault .2");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Bổ sung: trong trường hợp đối tượng BN là BHYT và chưa đến ngày hiệu lực 
        /// hoặc đã hết hạn sử dụng (thời gian y lệnh ko nằm trong khoảng [từ ngày - đến ngày] của thẻ BHYT), 
        /// thì hiển thị đối tượng thanh toán mặc định là đối tượng viện phí
        /// Ngược lại xử lý như hiện tại: ưu tiên lấy theo đối tượng Bn trước, không có sẽ lấy mặc định theo đối tượng chấp nhận TT đầu tiên tìm thấy

        ///#22112 ----------- 
        ///- Ở màn hình tồn thuốc, bỏ điều kiện lọc chính sách giá dịch vụ khi hiển thị các thuốc/vật tư.
        ///- Khi chọn thuốc, hệ thống căn cứ vào đối tượng BN và "có đủ thông tin BHYT hay không" để tự động điền "Đối tượng thanh toán" mặc định. Cụ thể:
        ///+ Nếu đối tượng BN là BHYT, và loại thuốc/vật tư đó CÓ ĐỦ thông tin BHYT thì mặc định điền ĐTTT là BHYT
        ///+ Nếu đối tượng BN là BHYT, và loại thuốc/vật tư đó KHÔNG CÓ ĐỦ thông tin BHYT thì mặc định điền ĐTTT là Viện phí
        ///+ Nếu đối tượng BN ko phải là BHYT thì mặc định điền ĐTTT là đối tượng của BN

        ///1 thuốc được coi là "Có đủ thông tin BHYT" khi thỏa mãn:
        ///Khai báo đủ các thông tin: mã hoạt chất BHYT (ACTIVE_INGR_BHYT_CODE), số đăng ký (REGISTER_NUMBER), và nhóm BHYT thuộc 1 trong các loại: "Thuốc trong danh mục", "Thuốc thanh toán theo tỷ lệ" hoặc "Thuốc ung thư, chống thải ghép"
        ///1 vật tư được coi là "Có đủ thông tin BHYT" khi thỏa mãn:
        ///Khai báo đủ các thông tin: mã BHYT (HEIN_SERVICE_BHYT_CODE), tên BHYT (HEIN_SERVICE_BHYT_NAME), và nhóm BHYT thuộc 1 trong các loại: "Vật tư thay thế", "Vật tư trong danh mục", "Vật tư thanh toán theo tỷ lệ"
        ///------------
        /// </summary>
        /// <param name="patientTypeId"></param>
        /// <param name="serviceId"></param>
        /// <returns></returns>
        private MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE ChoosePatientTypeDefaultlServiceOther(long patientTypeId, MediMatyTypeADO medimaty)
        {
            MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE result = new MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE();
            try
            {
                bool isFullHeinInfoData = IsFullHeinInfo(medimaty);
                if (patientTypeId == HisConfigCFG.PatientTypeId__BHYT && isFullHeinInfoData)
                {
                    result = currentPatientTypeWithPatientTypeAlter.Where(o => o.ID == HisConfigCFG.PatientTypeId__BHYT).FirstOrDefault();
                }
                else if (patientTypeId == HisConfigCFG.PatientTypeId__BHYT && !isFullHeinInfoData)
                {
                    result = currentPatientTypeWithPatientTypeAlter.Where(o => o.ID == HisConfigCFG.PatientTypeId__VP).FirstOrDefault();
                }
                else
                {
                    result = currentPatientTypeWithPatientTypeAlter.Where(o => o.ID == patientTypeId).FirstOrDefault();
                }

                //var patientTypes = BackendDataWorker.Get<HIS_PATIENT_TYPE>();
                //if (patientTypes != null
                //    && patientTypes.Count > 0
                //    && currentPatientTypeWithPatientTypeAlter != null
                //    && currentPatientTypeWithPatientTypeAlter.Count > 0
                //    //&& this.servicePatyAllows.ContainsKey(serviceId)
                //    )
                //{
                //    var patientTypeIdInSePas = this.servicePatyAllows[serviceId].Select(o => o.PATIENT_TYPE_ID).ToList();
                //    var currentPatientTypeTemps = this.currentPatientTypeWithPatientTypeAlter
                //        .Where(o => patientTypeIdInSePas != null && patientTypeIdInSePas.Contains(o.ID)).ToList();
                //    if (currentPatientTypeTemps != null && currentPatientTypeTemps.Count > 0)
                //    {
                //        if (this.currentHisPatientTypeAlter.PATIENT_TYPE_ID == HisConfigCFG.PatientTypeId__BHYT
                //        && (Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(this.currentHisPatientTypeAlter.HEIN_CARD_FROM_TIME ?? 0).Value.Date > Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(intructionTimeSelecteds.OrderByDescending(o => o).First()).Value.Date
                //        || Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(this.currentHisPatientTypeAlter.HEIN_CARD_TO_TIME ?? 0).Value.Date < Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(intructionTimeSelecteds.OrderByDescending(o => o).First()).Value.Date
                //        ))
                //        {
                //            result = currentPatientTypeTemps.FirstOrDefault(o => o.ID == HisConfigCFG.PatientTypeId__VP);
                //        }
                //        else
                //        {
                //            result = (currentPatientTypeTemps != null ? (currentPatientTypeTemps.FirstOrDefault(o => o.ID == patientTypeId) ?? currentPatientTypeTemps[0]) : null);
                //        }
                //    }
                //}
                return (result ?? new HIS_PATIENT_TYPE());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE ChoosePatientTypeDefaultlService(long patientTypeId, long serviceId, long serviceTypeId)
        {
            MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE result = new MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE();
            try
            {
                MediMatyTypeADO mediMatyTypeADO = null;
                if (serviceTypeId == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC)
                {
                    var sv = BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>().Where(o => o.SERVICE_ID == serviceId).FirstOrDefault();
                    mediMatyTypeADO = new MediMatyTypeADO()
                    {
                        SERVICE_ID = sv.SERVICE_ID,
                        SERVICE_TYPE_ID = sv.SERVICE_TYPE_ID,
                        ACTIVE_INGR_BHYT_CODE = sv.ACTIVE_INGR_BHYT_CODE,
                        //REGISTER_NUMBER = sv.REGISTER_NUMBER,
                        HEIN_SERVICE_TYPE_ID = sv.HEIN_SERVICE_TYPE_ID,
                        HEIN_SERVICE_TYPE_CODE = sv.HEIN_SERVICE_TYPE_CODE,
                        HEIN_SERVICE_BHYT_CODE = sv.HEIN_SERVICE_BHYT_CODE,
                        HEIN_SERVICE_BHYT_NAME = sv.HEIN_SERVICE_BHYT_NAME,
                    };
                }
                else if (serviceTypeId == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__VT)
                {
                    var sv = BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>().Where(o => o.SERVICE_ID == serviceId).FirstOrDefault();
                    mediMatyTypeADO = new MediMatyTypeADO()
                    {
                        SERVICE_ID = sv.SERVICE_ID,
                        SERVICE_TYPE_ID = sv.SERVICE_TYPE_ID,
                        HEIN_SERVICE_TYPE_ID = sv.HEIN_SERVICE_TYPE_ID,
                        HEIN_SERVICE_TYPE_CODE = sv.HEIN_SERVICE_TYPE_CODE,
                        HEIN_SERVICE_BHYT_CODE = sv.HEIN_SERVICE_BHYT_CODE,
                        HEIN_SERVICE_BHYT_NAME = sv.HEIN_SERVICE_BHYT_NAME,
                    };
                }

                return this.ChoosePatientTypeDefaultlServiceOther(patientTypeId, mediMatyTypeADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        /// <summary>
        /// - 1 thuốc được coi là "Có đủ thông tin BHYT" khi thỏa mãn:
        /// Khai báo đủ các thông tin: mã hoạt chất BHYT (ACTIVE_INGR_BHYT_CODE), số đăng ký (REGISTER_NUMBER), và nhóm BHYT thuộc 1 trong các loại: "Thuốc trong danh mục", "Thuốc thanh toán theo tỷ lệ" hoặc "Thuốc ung thư, chống thải ghép"
        /// - 1 vật tư được coi là "Có đủ thông tin BHYT" khi thỏa mãn:
        /// Khai báo đủ các thông tin: mã BHYT (HEIN_SERVICE_BHYT_CODE), tên BHYT (HEIN_SERVICE_BHYT_NAME), và nhóm BHYT thuộc 1 trong các loại: "Vật tư thay thế", "Vật tư trong danh mục", "Vật tư thanh toán theo tỷ lệ"
        /// </summary>
        /// <returns></returns>
        private bool IsFullHeinInfo(MediMatyTypeADO medimaty)
        {
            bool valid = false;
            try
            {
                valid = (medimaty.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC &&
                            !String.IsNullOrEmpty(medimaty.ACTIVE_INGR_BHYT_CODE)
                            //&& !String.IsNullOrEmpty(medimaty.REGISTER_NUMBER)
                            && (medimaty.HEIN_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__TH_TDM
                            || medimaty.HEIN_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__TH_TL
                            || medimaty.HEIN_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__TH_UT))
                        ||
                        (medimaty.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__VT &&
                            !String.IsNullOrEmpty(medimaty.HEIN_SERVICE_BHYT_CODE)
                            && !String.IsNullOrEmpty(medimaty.HEIN_SERVICE_BHYT_NAME)
                            && (medimaty.HEIN_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__VT_TT
                            || medimaty.HEIN_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__VT_TL
                            || medimaty.HEIN_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__VT_TDM));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return valid;
        }
        
        private HisTreatmentWithPatientTypeInfoSDO LoadDataToCurrentTreatmentData(long treatmentId, long intructionTime)
        {
            HisTreatmentWithPatientTypeInfoSDO treatment = null;
            try
            {
                CommonParam param = new CommonParam();
                MOS.Filter.HisTreatmentWithPatientTypeInfoFilter filter = new MOS.Filter.HisTreatmentWithPatientTypeInfoFilter();
                filter.TREATMENT_ID = treatmentId;
                if (this.pnlUCDate.Enabled)
                    filter.INTRUCTION_TIME = intructionTime;
                treatment = new BackendAdapter(param).Get<List<HisTreatmentWithPatientTypeInfoSDO>>(RequestUriStore.HIS_TREATMENT_GET_TREATMENT_WITH_PATIENT_TYPE_INFO_SDO, ApiConsumers.MosConsumer, filter, ProcessLostToken, param).FirstOrDefault();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return treatment;
        }

        private void LoadCurrentPatientTypeAlter(long treatmentId, long intructionTime, ref MOS.EFMODEL.DataModels.V_HIS_PATIENT_TYPE_ALTER hisPatientTypeAlter)
        {
            try
            {
                CommonParam param = new CommonParam();
                HisPatientTypeAlterViewAppliedFilter filter = new HisPatientTypeAlterViewAppliedFilter();
                filter.TreatmentId = treatmentId;
                filter.InstructionTime = ((intructionTime > 0) ? intructionTime : Inventec.Common.DateTime.Get.Now() ?? 0);
                hisPatientTypeAlter = new BackendAdapter(param).Get<MOS.EFMODEL.DataModels.V_HIS_PATIENT_TYPE_ALTER>(HisRequestUriStore.HIS_PATIENT_TYPE_ALTER_GET_APPLIED, ApiConsumers.MosConsumer, filter, ProcessLostToken, param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void PatientTypeWithTreatmentView7()
        {
            try
            {
                var patientTypeAllows = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_PATIENT_TYPE_ALLOW>();
                var patientTypes = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE>();
                if (patientTypeAllows != null && patientTypeAllows.Count > 0 && patientTypes != null)
                {
                    if (this.currentTreatmentWithPatientType != null && !String.IsNullOrEmpty(this.currentTreatmentWithPatientType.PATIENT_TYPE_CODE))
                    {
                        var patientType = patientTypes.FirstOrDefault(o => o.PATIENT_TYPE_CODE == this.currentTreatmentWithPatientType.PATIENT_TYPE_CODE);
                        if (patientType == null) throw new AggregateException("Khong lay duoc thong tin PatientType theo ma doi tuong (PATIENT_TYPE trong HisTreatmentWithPatientTypeInfoSDO).");

                        this.currentHisPatientTypeAlter = new V_HIS_PATIENT_TYPE_ALTER();
                        this.currentHisPatientTypeAlter.PATIENT_TYPE_ID = patientType.ID;
                        this.currentHisPatientTypeAlter.PATIENT_TYPE_CODE = patientType.PATIENT_TYPE_CODE;
                        this.currentHisPatientTypeAlter.PATIENT_TYPE_NAME = patientType.PATIENT_TYPE_NAME;
                        this.currentHisPatientTypeAlter.TREATMENT_TYPE_CODE = this.currentTreatmentWithPatientType.TREATMENT_TYPE_CODE;
                        this.currentHisPatientTypeAlter.HEIN_MEDI_ORG_CODE = this.currentTreatmentWithPatientType.HEIN_MEDI_ORG_CODE;
                        this.currentHisPatientTypeAlter.HEIN_CARD_FROM_TIME = this.currentTreatmentWithPatientType.HEIN_CARD_FROM_TIME;
                        this.currentHisPatientTypeAlter.HEIN_CARD_TO_TIME = this.currentTreatmentWithPatientType.HEIN_CARD_TO_TIME;
                        this.currentHisPatientTypeAlter.HEIN_CARD_NUMBER = this.currentTreatmentWithPatientType.HEIN_CARD_NUMBER;
                        this.currentHisPatientTypeAlter.RIGHT_ROUTE_TYPE_CODE = this.currentTreatmentWithPatientType.RIGHT_ROUTE_TYPE_CODE;
                        this.currentHisPatientTypeAlter.RIGHT_ROUTE_CODE = this.currentTreatmentWithPatientType.RIGHT_ROUTE_CODE;
                        var tt = BackendDataWorker.Get<HIS_TREATMENT_TYPE>().FirstOrDefault(o => o.TREATMENT_TYPE_CODE == this.currentTreatmentWithPatientType.TREATMENT_TYPE_CODE);
                        this.currentHisPatientTypeAlter.TREATMENT_TYPE_ID = (tt != null ? tt.ID : 0);
                        this.currentHisPatientTypeAlter.TREATMENT_TYPE_NAME = (tt != null ? tt.TREATMENT_TYPE_NAME : "");

                        var patientTypeAllow = patientTypeAllows.Where(o => o.PATIENT_TYPE_ID == patientType.ID).Select(m => m.PATIENT_TYPE_ALLOW_ID).Distinct().ToList();
                        if (patientTypeAllow != null && patientTypeAllow.Count > 0)
                            this.currentPatientTypeWithPatientTypeAlter = patientTypes
                                .Where(o => patientTypeAllow.Contains(o.ID)).ToList();
                    }
                    else
                        throw new AggregateException("currentHisTreatment.PATIENT_TYPE_CODE is null");
                }
                else
                    throw new AggregateException("patientTypeAllows is null");

                var patientTypeIdAls = this.currentPatientTypeWithPatientTypeAlter.Select(o => o.ID).Distinct().ToList();
                //Lấy tất cả chính sách giá của tất cả các đối tượng tt và đối tượng chuyển đổi của bệnh nhân
                //var servicePatyTemps = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_SERVICE_PATY>()
                //    .Where(t => patientTypeIdAls.Contains(t.PATIENT_TYPE_ID))
               //     .ToList();
                //this.servicePatyAllows = servicePatyTemps.GroupBy(o => o.SERVICE_ID)
                //    .ToDictionary(o => o.Key, o => o.ToList());

                //Lọc các đối tượng thanh toán không có chính sách giá
                //var patientHasSetyIds = servicePatyTemps.Select(o => o.PATIENT_TYPE_ID).Distinct().ToList();
                //this.currentPatientTypeWithPatientTypeAlter = this.currentPatientTypeWithPatientTypeAlter.Where(o => patientHasSetyIds.Contains(o.ID)).ToList();
            }
            catch (AggregateException ex)
            {
                WaitingManager.Hide();
                this.currentHisPatientTypeAlter = new V_HIS_PATIENT_TYPE_ALTER();
                this.currentPatientTypeWithPatientTypeAlter = new List<HIS_PATIENT_TYPE>();
                MessageManager.Show(ResourceMessage.KhongTimThayDoiTuongThanhToanTrongThoiGianYLenh);
                LogSystem.Warn(LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => this.currentTreatmentWithPatientType), this.currentTreatmentWithPatientType));
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private async Task LoadTotalSereServByHeinWithTreatment()
        {
            try
            {
                CommonParam param = new CommonParam();
                HisSereServFilter hisSereServFilter = new HisSereServFilter();
                hisSereServFilter.TREATMENT_ID = this.treatmentId;
                hisSereServFilter.PATIENT_TYPE_ID = HisConfigCFG.PatientTypeId__BHYT;
                var sereServByHeinWithTreatments = await new BackendAdapter(param).GetAsync<List<MOS.EFMODEL.DataModels.HIS_SERE_SERV>>(HisRequestUriStore.HIS_SERE_SERV_GET, ApiConsumers.MosConsumer, hisSereServFilter, param);
                //Nếu sửa đơn thuốc thì lấy tổng tiền bảo hiểm hồ sơ trừ đi đơn đang sửa
                if (this.assignPrescriptionEditADO != null && this.assignPrescriptionEditADO.ServiceReq != null && this.assignPrescriptionEditADO.ServiceReq.ID > 0)
                {
                    this.totalHeinByTreatment = sereServByHeinWithTreatments.Where(o => o.SERVICE_REQ_ID != this.assignPrescriptionEditADO.ServiceReq.ID).Sum(o => o.VIR_TOTAL_PRICE_NO_ADD_PRICE ?? 0);
                }
                //Ngược lại lấy tất cả tổng tiền bảo hiểm trong hồ sơ
                else
                {
                    this.totalHeinByTreatment = sereServByHeinWithTreatments.Sum(o => o.VIR_TOTAL_PRICE_NO_ADD_PRICE ?? 0);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private async Task LoadSereServTotalHeinPriceWithTreatment(long treatmentId)
        {
            try
            {
                CommonParam param = new CommonParam();
                HisSereServView7Filter sereServFilter = new HisSereServView7Filter();
                sereServFilter.TDL_TREATMENT_ID = treatmentId;
                sereServFilter.SERVICE_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC;
                sereServFilter.TDL_SERVICE_REQ_TYPE_IDs = new List<long> { IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONK };
                sereServFilter.PATIENT_TYPE_ID = HisConfigCFG.PatientTypeId__BHYT;
                var sereServTotalHeinPriceWithTreatments = await new BackendAdapter(param).GetAsync<List<V_HIS_SERE_SERV_7>>(RequestUriStore.HIS_SERE_SERV_GETVIEW_7, ApiConsumers.MosConsumer, sereServFilter, ProcessLostToken, param);

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
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private async Task LoadDataSereServWithTreatment(HisTreatmentWithPatientTypeInfoSDO treatment, long intructionTime)
        {
            try
            {
                if (treatment != null)
                {
                    CommonParam param = new CommonParam();
                    HisSereServView1Filter hisSereServFilter = new HisSereServView1Filter();
                    hisSereServFilter.TREATMENT_ID = treatment.ID;                  
                    List<long> setyAllowsIds = new List<long>();
                    setyAllowsIds.Add(IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__VT);
                    setyAllowsIds.Add(IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC);
                    hisSereServFilter.SERVICE_TYPE_IDs = setyAllowsIds;
                    this.sereServWithTreatment = await new BackendAdapter(param).GetAsync<List<MOS.EFMODEL.DataModels.V_HIS_SERE_SERV_1>>(RequestUriStore.HIS_SERE_SERV_GETVIEW_1, ApiConsumers.MosConsumerNoStore, hisSereServFilter, ProcessLostToken, param);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private List<MOS.EFMODEL.DataModels.V_HIS_SERE_SERV_7> GetSereServ8ByTreatmentId(long treatmentId)
        {
            try
            {
                CommonParam param = new CommonParam();
                HisSereServView7Filter hisSereServFilter = new HisSereServView7Filter();
                hisSereServFilter.TDL_TREATMENT_ID = this.currentTreatmentWithPatientType.ID;
                return new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.V_HIS_SERE_SERV_7>>(RequestUriStore.HIS_SERE_SERV_GETVIEW_7, ApiConsumers.MosConsumer, hisSereServFilter, ProcessLostToken, param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return null;
        }

        private List<HIS_MEDICINE_BEAN> GetMedicineBeanByExpMestMedicine(List<long> expMestMedicineIds)
        {
            List<HIS_MEDICINE_BEAN> result = null;
            try
            {
                CommonParam param = new CommonParam();
                HisMedicineBeanFilter hismedicineBeanFilter = new HisMedicineBeanFilter();
                hismedicineBeanFilter.EXP_MEST_MEDICINE_IDs = expMestMedicineIds;
                result = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.HIS_MEDICINE_BEAN>>(RequestUriStore.HIS_MEDICINE_BEAN__GET, ApiConsumers.MosConsumer, hismedicineBeanFilter, ProcessLostToken, param);
            }
            catch (Exception ex)
            {
                result = null;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private List<HIS_MATERIAL_BEAN> GetMaterialBeanByExpMestMedicine(List<long> expMestMaterialIds)
        {
            List<HIS_MATERIAL_BEAN> result = null;
            try
            {
                CommonParam param = new CommonParam();
                HisMaterialBeanFilter hismaterialBeanFilter = new HisMaterialBeanFilter();
                hismaterialBeanFilter.EXP_MEST_MATERIAL_IDs = expMestMaterialIds;
                result = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.HIS_MATERIAL_BEAN>>(RequestUriStore.HIS_MATERIAL_BEAN__GET, ApiConsumers.MosConsumer, hismaterialBeanFilter, ProcessLostToken, param);
            }
            catch (Exception ex)
            {
                result = null;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private List<MOS.EFMODEL.DataModels.V_HIS_EXP_MEST_MEDICINE> GetExpMestMedicineByExpMestId(long expMestId)
        {
            try
            {
                CommonParam param = new CommonParam();
                HisExpMestMedicineViewFilter searchMedicineFilter = new HisExpMestMedicineViewFilter();
                searchMedicineFilter.ORDER_DIRECTION = "ASC";
                searchMedicineFilter.ORDER_FIELD = "ID";
                searchMedicineFilter.EXP_MEST_ID = expMestId;
                return new BackendAdapter(param).Get<List<V_HIS_EXP_MEST_MEDICINE>>(HisRequestUriStore.HIS_EXP_MEST_MEDICINE_GETVIEW, ApiConsumers.MosConsumer, searchMedicineFilter, ProcessLostToken, param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return null;
        }

        private List<MOS.EFMODEL.DataModels.V_HIS_EXP_MEST_MATERIAL> GetExpMestMaterialByExpMestId(long expMestId)
        {
            try
            {
                CommonParam param = new CommonParam();
                HisExpMestMaterialViewFilter searchMaterialFilter = new HisExpMestMaterialViewFilter();
                searchMaterialFilter.ORDER_DIRECTION = "ASC";
                searchMaterialFilter.ORDER_FIELD = "ID";
                searchMaterialFilter.EXP_MEST_ID = expMestId;
                return new BackendAdapter(param).Get<List<V_HIS_EXP_MEST_MATERIAL>>(HisRequestUriStore.HIS_EXP_MEST_MATERIAL_GETVIEW, ApiConsumers.MosConsumer, searchMaterialFilter, ProcessLostToken, param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return null;
        }

        private List<MOS.EFMODEL.DataModels.HIS_SERVICE_REQ_METY> GetServiceReqMetyByServiceReqId(long serviceReqId)
        {
            try
            {
                CommonParam param = new CommonParam();
                HisServiceReqMetyFilter expMestMetyFilter = new HisServiceReqMetyFilter();
                expMestMetyFilter.SERVICE_REQ_ID = serviceReqId;
                return new BackendAdapter(param).Get<List<HIS_SERVICE_REQ_METY>>(RequestUriStore.HIS_SERVICE_REQ_METY__GET, ApiConsumers.MosConsumer, expMestMetyFilter, ProcessLostToken, param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return null;
        }

        private List<MOS.EFMODEL.DataModels.HIS_EXP_MEST_METY_REQ> GetExpMestMetyReqByExpMestId(long expMestId)
        {
            try
            {
                CommonParam param = new CommonParam();
                MOS.Filter.HisExpMestMetyReqFilter filter = new HisExpMestMetyReqFilter();
                filter.EXP_MEST_ID = expMestId;
                return new BackendAdapter(param).Get<List<HIS_EXP_MEST_METY_REQ>>("api/HisExpMestMetyReq/Get", ApiConsumers.MosConsumer, filter, param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return null;
        }

        private List<MOS.EFMODEL.DataModels.HIS_EXP_MEST_MATY_REQ> GetExpMestMatyReqByExpMestId(long expMestId)
        {
            try
            {
                CommonParam param = new CommonParam();
                MOS.Filter.HisExpMestMatyReqFilter filter = new HisExpMestMatyReqFilter();
                filter.EXP_MEST_ID = expMestId;
                return new BackendAdapter(param).Get<List<HIS_EXP_MEST_MATY_REQ>>("api/HisExpMestMatyReq/Get", ApiConsumers.MosConsumer, filter, param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return null;
        }

        private List<MOS.EFMODEL.DataModels.HIS_SERVICE_REQ_MATY> GetServiceReqMatyByServiceReqId(long serviceReqId)
        {
            try
            {
                CommonParam param = new CommonParam();
                HisServiceReqMatyFilter expMestMatyFilter = new HisServiceReqMatyFilter();
                expMestMatyFilter.SERVICE_REQ_ID = serviceReqId;
                return new BackendAdapter(param).Get<List<HIS_SERVICE_REQ_MATY>>(RequestUriStore.HIS_SERVICE_REQ_MATY__GET, ApiConsumers.MosConsumer, expMestMatyFilter, ProcessLostToken, param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return null;
        }

        private MOS.EFMODEL.DataModels.V_HIS_ACIN_INTERACTIVE GetAcinInteractiveByCode(string activeIngrBhytCode, string activeIngrBhytConflicCode)
        {
            MOS.EFMODEL.DataModels.V_HIS_ACIN_INTERACTIVE result = null;
            try
            {
                result = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_ACIN_INTERACTIVE>()
                    .FirstOrDefault(o => o.ACTIVE_INGREDIENT_CODE == activeIngrBhytCode
                        && o.CONFLICT_CODE == activeIngrBhytConflicCode
                    );
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private List<MOS.EFMODEL.DataModels.V_HIS_MEDICINE_TYPE_ACIN> GetMedicineTypeAcinByMedicineType(List<long> medicineTypeIds)
        {
            List<MOS.EFMODEL.DataModels.V_HIS_MEDICINE_TYPE_ACIN> result = null;
            try
            {
                result = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_MEDICINE_TYPE_ACIN>()
                    .Where(o => medicineTypeIds.Contains(o.MEDICINE_TYPE_ID)
                    ).ToList();

                var medis = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_MEDICINE_TYPE_ACIN>();

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private List<MOS.EFMODEL.DataModels.V_HIS_EMTE_MEDICINE_TYPE> GetEmteMedicineTypeByExpMestId(long expMestTemplateId)
        {
            try
            {
                CommonParam param = new CommonParam();
                HisEmteMedicineTypeViewFilter filter = new HisEmteMedicineTypeViewFilter();
                filter.EXP_MEST_TEMPLATE_ID = expMestTemplateId;
                filter.ORDER_DIRECTION = "ASC";
                filter.ORDER_FIELD = "ID";
                return new BackendAdapter(param).Get<List<V_HIS_EMTE_MEDICINE_TYPE>>(HisRequestUriStore.HIS_EMTE_MEDICINE_TYPE_GETVIEW, ApiConsumers.MosConsumer, filter, ProcessLostToken, param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return null;
        }

        private List<MOS.EFMODEL.DataModels.V_HIS_EMTE_MATERIAL_TYPE> GetEmteMaterialTypeByExpMestId(long expMestTemplateId)
        {
            try
            {
                CommonParam param = new CommonParam();
                HisEmteMaterialTypeViewFilter filter = new HisEmteMaterialTypeViewFilter();
                filter.EXP_MEST_TEMPLATE_ID = expMestTemplateId;
                filter.ORDER_DIRECTION = "ASC";
                filter.ORDER_FIELD = "ID";
                return new BackendAdapter(param).Get<List<V_HIS_EMTE_MATERIAL_TYPE>>(HisRequestUriStore.HIS_EMTE_MATERIAL_TYPE_GETVIEW, ApiConsumers.MosConsumer, filter, ProcessLostToken, param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return null;
        }

        private List<MOS.EFMODEL.DataModels.V_HIS_EQUIPMENT_SET_MATY> GetMaterialTypeByEquipmentSetId(long equipmentSetId)
        {
            try
            {
                CommonParam param = new CommonParam();
                HisEquipmentSetMatyViewFilter filter = new HisEquipmentSetMatyViewFilter();
                filter.EQUIPMENT_SET_ID = equipmentSetId;
                filter.ORDER_DIRECTION = "ASC";
                filter.ORDER_FIELD = "ID";
                return new BackendAdapter(param).Get<List<V_HIS_EQUIPMENT_SET_MATY>>(RequestUriStore.HIS_EQUIPMENT_SET_MATY__GETVIEW, ApiConsumers.MosConsumer, filter, ProcessLostToken, param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return null;
        }

        private List<MOS.EFMODEL.DataModels.V_HIS_MEDICINE_BEAN_1> GetMedicineBeanByExpMestId(long expMestId)
        {
            try
            {
                CommonParam param = new CommonParam();
                MOS.Filter.HisMedicineBeanView1Filter medicineBeanFilter = new HisMedicineBeanView1Filter();
                //medicineBeanFilter.EXP_MEST_ID = expMestId;
                return new BackendAdapter(param).Get<List<V_HIS_MEDICINE_BEAN_1>>("api/HisMedicineBean/GetView1", ApiConsumers.MosConsumer, medicineBeanFilter, param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return null;
        }

        private List<MOS.EFMODEL.DataModels.V_HIS_MATERIAL_BEAN_1> GetMaterialBeanByExpMestId(long expMestId)
        {
            try
            {
                CommonParam param = new CommonParam();
                MOS.Filter.HisMaterialBeanView1Filter materialBeanFilter = new HisMaterialBeanView1Filter();
                //materialBeanFilter.EXP_MEST_ID = expMestId;
                return new BackendAdapter(param).Get<List<V_HIS_MATERIAL_BEAN_1>>("api/HisMaterialBean/GetView1", ApiConsumers.MosConsumer, materialBeanFilter, param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return null;
        }

        private async Task LoadDataTracking()
        {
            try
            {
                LogSystem.Debug("LoadDataTracking => 1");
                //Init Control
                CommonParam param = new CommonParam();

                //if (GlobalStore.IsCabinet)
                //{
                //    //Check đối tượng nội trú hoặc ngoại trú
                //    HisPatientTypeAlterViewAppliedFilter filterPatientTypeAlter = new HisPatientTypeAlterViewAppliedFilter();
                //    filterPatientTypeAlter.TreatmentId = this.treatmentId;
                //    filterPatientTypeAlter.InstructionTime = Inventec.Common.DateTime.Get.Now() ?? 0;
                //    V_HIS_PATIENT_TYPE_ALTER patientTypeAlter = await new BackendAdapter(param).GetAsync<MOS.EFMODEL.DataModels.V_HIS_PATIENT_TYPE_ALTER>(HisRequestUriStore.HIS_PATIENT_TYPE_ALTER_GET_APPLIED, ApiConsumers.MosConsumer, filterPatientTypeAlter, param);
                //    if (patientTypeAlter.TREATMENT_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTNOITRU
                //        && patientTypeAlter.TREATMENT_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTNGOAITRU)
                //    {
                //        lciPhieuDieuTri.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                //        return;
                //    }
                //}

                HisTrackingFilter filter = new HisTrackingFilter();
                filter.TREATMENT_ID = this.treatmentId;
                List<HIS_TRACKING> trackings = await new BackendAdapter(param)
                    .GetAsync<List<MOS.EFMODEL.DataModels.HIS_TRACKING>>("api/HisTracking/Get", ApiConsumers.MosConsumer, filter, param);
                if (trackings == null || trackings.Count == 0)
                    return;

                trackings = trackings.OrderByDescending(o => o.TRACKING_TIME).ToList();

                List<long> intructionTimeSelecteds = this.ucDateProcessor.GetValue(this.ucDate);
                if (intructionTimeSelecteds == null)
                    return;

                List<string> intructionDateSelecteds = new List<string>();
                foreach (var item in intructionTimeSelecteds)
                {
                    string intructionDate = item.ToString().Substring(0, 8);
                    intructionDateSelecteds.Add(intructionDate);
                }

                var trackingTemps = trackings.Where(o => intructionDateSelecteds.Contains(o.TRACKING_TIME.ToString().Substring(0, 8))
                    && o.DEPARTMENT_ID == HIS.Desktop.LocalStorage.LocalData.WorkPlace.GetDepartmentId(this.currentModule.RoomTypeId))
                    .OrderByDescending(o => o.TRACKING_TIME).ToList();

                if (trackingTemps == null || trackingTemps.Count == 0)
                {
                    return;
                }

                trackingADOs = new List<TrackingADO>();
                foreach (var item in trackings)
                {
                    TrackingADO tracking = new TrackingADO();
                    Inventec.Common.Mapper.DataObjectMapper.Map<TrackingADO>(tracking, item);
                    tracking.TrackingTimeStr = Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(tracking.TRACKING_TIME);
                    trackingADOs.Add(tracking);
                }
                trackingADOs = trackingADOs.OrderByDescending(o => o.TRACKING_TIME).ToList();
                if (HisConfigCFG.IsDefaultTracking)
                {
                    if (this.ucDateProcessor.GetChkMultiDateState(this.ucDate) == false)
                    {
                        cboPhieuDieuTri.EditValue = trackingTemps[0].ID;
                        cboPhieuDieuTri.Properties.Buttons[1].Visible = true;
                    }
                }

                this.InitComboTracking(cboPhieuDieuTri);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}

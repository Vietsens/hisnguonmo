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
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.AssignPrescriptionPK.ADO;
using HIS.Desktop.Plugins.AssignPrescriptionPK.AssignPrescription;
using HIS.Desktop.Plugins.AssignPrescriptionPK.Base;
using HIS.Desktop.Plugins.AssignPrescriptionPK.Config;
using HIS.Desktop.Plugins.AssignPrescriptionPK.MessageBoxForm;
using HIS.Desktop.Plugins.AssignPrescriptionPK.Resources;
using HIS.Desktop.Plugins.AssignPrescriptionPK.Worker;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.AssignPrescriptionPK.Add.MediStockD1SDO
{
    class MediStockD1SDORowAddBehavior : AddAbstract, IAdd
    {

        long expMestId;
        internal MediStockD1SDORowAddBehavior(CommonParam param,
            frmAssignPrescription frmAssignPrescription,
            ValidAddRow validAddRow,
            HIS.Desktop.Plugins.AssignPrescriptionPK.MediMatyCreateWorker.ChoosePatientTypeDefaultlService choosePatientTypeDefaultlService,
            HIS.Desktop.Plugins.AssignPrescriptionPK.MediMatyCreateWorker.ChoosePatientTypeDefaultlServiceOther choosePatientTypeDefaultlServiceOther,
            CalulateUseTimeTo calulateUseTimeTo,
            ExistsAssianInDay existsAssianInDay,
            object dataRow)
            : base(param,
             frmAssignPrescription,
             validAddRow,
             choosePatientTypeDefaultlService,
             choosePatientTypeDefaultlServiceOther,
             calulateUseTimeTo,
             existsAssianInDay,
             dataRow)
        {
            this.Id = frmAssignPrescription.currentMedicineTypeADOForEdit.ID;
            this.AmountAvaiable = (frmAssignPrescription.currentMedicineTypeADOForEdit.AMOUNT ?? 0);
            this.DataType = (frmAssignPrescription.currentMedicineTypeADOForEdit.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC ? HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC : HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.VATTU);
            this.Code = frmAssignPrescription.currentMedicineTypeADOForEdit.MEDICINE_TYPE_CODE;
            this.Name = frmAssignPrescription.currentMedicineTypeADOForEdit.MEDICINE_TYPE_NAME;
            this.ManuFacturerName = frmAssignPrescription.currentMedicineTypeADOForEdit.MANUFACTURER_NAME;
            this.ServiceUnitName = frmAssignPrescription.currentMedicineTypeADOForEdit.SERVICE_UNIT_NAME;
            this.NationalName = frmAssignPrescription.currentMedicineTypeADOForEdit.NATIONAL_NAME;
            this.ServiceId = frmAssignPrescription.currentMedicineTypeADOForEdit.SERVICE_ID;
            this.Concentra = frmAssignPrescription.currentMedicineTypeADOForEdit.CONCENTRA;
            this.MediStockId = frmAssignPrescription.currentMedicineTypeADOForEdit.MEDI_STOCK_ID;
            this.MediStockCode = frmAssignPrescription.currentMedicineTypeADOForEdit.MEDI_STOCK_CODE;
            this.MediStockName = frmAssignPrescription.currentMedicineTypeADOForEdit.MEDI_STOCK_NAME;
            this.HeinServiceTypeId = frmAssignPrescription.currentMedicineTypeADOForEdit.HEIN_SERVICE_TYPE_ID;
            this.ServiceTypeId = (long)(frmAssignPrescription.currentMedicineTypeADOForEdit.SERVICE_TYPE_ID);
            this.ActiveIngrBhytCode = frmAssignPrescription.currentMedicineTypeADOForEdit.ACTIVE_INGR_BHYT_CODE;
            this.ActiveIngrBhytName = frmAssignPrescription.currentMedicineTypeADOForEdit.ACTIVE_INGR_BHYT_NAME;
            this.IsStent = frmAssignPrescription.currentMedicineTypeADOForEdit.IsStent;
            this.IsAllowOdd = frmAssignPrescription.currentMedicineTypeADOForEdit.IsAllowOdd;
            this.IsAllowOddAndExportOdd = frmAssignPrescription.currentMedicineTypeADOForEdit.IsAllowOddAndExportOdd;
            this.IsOutKtcFee = ((frmAssignPrescription.currentMedicineTypeADOForEdit.IS_OUT_PARENT_FEE ?? -1) == 1);
            this.PackageNumber = frmAssignPrescription.currentMedicineTypeADOForEdit.TDL_PACKAGE_NUMBER;
            this.ExpiredDate = frmAssignPrescription.currentMedicineTypeADOForEdit.EXPIRED_DATE;
            this.IsAssignPackage = frmAssignPrescription.currentMedicineTypeADOForEdit.IsAssignPackage;
            this.IS_SPLIT_COMPENSATION = frmAssignPrescription.currentMedicineTypeADOForEdit.IS_SPLIT_COMPENSATION;
            this.MAME_ID = frmAssignPrescription.currentMedicineTypeADOForEdit.MAME_ID;
            this.ATC_CODES = frmAssignPrescription.currentMedicineTypeADOForEdit.ATC_CODES;
            this.ATC_GROUP_CODES = frmAssignPrescription.currentMedicineTypeADOForEdit.ATC_GROUP_CODES;
            this.CONTRAINDICATION = frmAssignPrescription.currentMedicineTypeADOForEdit.CONTRAINDICATION;
            this.DESCRIPTION = frmAssignPrescription.currentMedicineTypeADOForEdit.DESCRIPTION;
            this.CONTRAINDICATION_IDS = frmAssignPrescription.currentMedicineTypeADOForEdit.CONTRAINDICATION_IDS;
            this.IS_OUT_HOSPITAL = frmAssignPrescription.currentMedicineTypeADOForEdit.IS_OUT_HOSPITAL;
            this.SERVICE_CONDITION_ID = frmAssignPrescription.currentMedicineTypeADOForEdit.SERVICE_CONDITION_ID;
            this.SERVICE_CONDITION_NAME = frmAssignPrescription.currentMedicineTypeADOForEdit.SERVICE_CONDITION_NAME;
            if (!String.IsNullOrEmpty(frmAssignPrescription.txtPreviousUseDay.Text) && Inventec.Common.TypeConvert.Parse.ToInt64(frmAssignPrescription.txtPreviousUseDay.Text) > 0)
                this.PREVIOUS_USING_COUNT = Inventec.Common.TypeConvert.Parse.ToInt64(frmAssignPrescription.txtPreviousUseDay.Text);
            else
                this.PREVIOUS_USING_COUNT = null;
            var stock = BackendDataWorker.Get<V_HIS_MEDI_STOCK>().FirstOrDefault(o => o.ID == this.MediStockId);
            if (stock != null && stock.IS_EXPEND == 1)
            {
                this.IsExpend = true;
                this.IsDisableExpend = true;
            }
            else if (((frmAssignPrescription.currentMedicineTypeADOForEdit.IS_AUTO_EXPEND ?? -1) == 1)
                || (HisConfigCFG.IsAutoTickExpendWithAssignPresPTTT && frmAssignPrescription.isAutoCheckExpend == true
                    && (this.ServiceTypeId == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC
                        || (this.ServiceTypeId == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__VT
                            && frmAssignPrescription.currentMedicineTypeADOForEdit.HEIN_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__VT_TDM
                            || frmAssignPrescription.currentMedicineTypeADOForEdit.HEIN_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__VT_NDM))))
            {
                this.IsExpend = true;
            }
            this.expMestId = frmAssignPrescription.oldExpMestId;
            this.Speed = frmAssignPrescription.spinTocDoTruyen.EditValue != null ? (decimal?)frmAssignPrescription.spinTocDoTruyen.Value : null;
            Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => frmAssignPrescription.currentMedicineTypeADOForEdit.IS_AUTO_EXPEND), frmAssignPrescription.currentMedicineTypeADOForEdit.IS_AUTO_EXPEND) + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => HisConfigCFG.IsAutoTickExpendWithAssignPresPTTT), HisConfigCFG.IsAutoTickExpendWithAssignPresPTTT) + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => frmAssignPrescription.isAutoCheckExpend), frmAssignPrescription.isAutoCheckExpend) + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => frmAssignPrescription.currentMedicineTypeADOForEdit.HEIN_SERVICE_TYPE_ID), frmAssignPrescription.currentMedicineTypeADOForEdit.HEIN_SERVICE_TYPE_ID) + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => frmAssignPrescription.currentMedicineTypeADOForEdit.IntructionTimeSelecteds), frmAssignPrescription.currentMedicineTypeADOForEdit.IntructionTimeSelecteds));
        }

        bool IAdd.Run()
        {
            bool success = false;
            medicineTypeSDO__Category__SameMediAcin = null;
            try
            {
                if (this.ValidMetyMatyType__Add())
                {
                    List<MediMatyTypeADO> mediMatyTypeADOTemps = new List<MediMatyTypeADO>();
                    //Nếu thuốc đã kê không đủ khả dụng trong kho, người dùng chọn lấy thuốc ngoài kho thay thế
                    //==> Lấy các thuốc ngoài kho + các thông tin số lượng, đường dùng, cách dùng, hướng dẫn sử dụng,.. đã chọn => tự động bổ sung vào danh sách thuốc đã chọn luôn
                    if (medicineTypeSDO__Category__SameMediAcin != null)
                    {
                        SetValidAssianInDayError();
                        SetValidAmountError();
                        UpdateMediMatyByMedicineTypeCategory(medicineTypeSDO__Category__SameMediAcin);
                        this.medicineTypeSDO.IsMultiDateState = this.IsMultiDateState;//TODO
                        this.medicineTypeSDO.IntructionTimeSelecteds = this.IntructionTimeSelecteds;//TODO
                        mediMatyTypeADOTemps.Add(this.medicineTypeSDO);
                    }
                    //Nếu thuốc còn khả dụng trong kho
                    //==> Set các thông tin đối tượng mặc định, đường dùng, thời gian dùng, validate,...
                    else
                    {
                        this.CreateADO();
                        //119139 V+
                        if (frmAssignPrescription.intructionTimeSelected != null && frmAssignPrescription.intructionTimeSelected.Count < 2 && !frmAssignPrescription.IsSelectMultiPatient())
                        {
                            if (!frmAssignPrescription.GetOverReason(medicineTypeSDO))
                                return success;
                        }
                        this.UpdateMedicinePackageInfoInDataRow(this.medicineTypeSDO);
                        this.UpdatePatientTypeInDataRow(this.medicineTypeSDO);
                        this.UpdateExpMestReasonInDataRow(this.medicineTypeSDO);
                        this.UpdateMedicineUseFormInDataRow(this.medicineTypeSDO);
                        this.UpdateUseTimeInDataRow(this.medicineTypeSDO);
                        this.SetValidError();
                        MestMetyUnitWorker.UpdateUnit(this.medicineTypeSDO, GlobalStore.HisMestMetyUnit);
                        this.medicineTypeSDO.UpdateAmountAutoRoundUpByAllowOddInDataRow(this.medicineTypeSDO);
                        this.medicineTypeSDO.UpdateAutoRoundUpByConvertUnitRatioInDataRow(this.medicineTypeSDO, frmAssignPrescription.VHistreatment);
                        if (!WarningOddConvertWorker.CheckWarningOddConvertAmount(this.medicineTypeSDO, this.medicineTypeSDO.AMOUNT, Param))
                        {
                            throw new AggregateException("CheckWarningOddConvertAmount");
                        }

                        if (this.medicineTypeSDO != null && this.medicineTypeSDO.IsStent == true
                            && this.medicineTypeSDO.AMOUNT > 1)
                        {
                            mediMatyTypeADOTemps = MediMatyProcessor.MakeMaterialSingleStent(medicineTypeSDO);
                        }
                        else
                        {
                            mediMatyTypeADOTemps.Add(this.medicineTypeSDO);
                        }
                    }

                    foreach (var item in mediMatyTypeADOTemps)
                    {
                        //Nếu (kê đơn phòng khám hoặc kê tủ trực) và không phải trường hợp hết khả dụng chọn thuốc ngoài kho thay thế thì sẽ gọi hàm take bean
                        if (((!GlobalStore.IsTreatmentIn && ((frmAssignPrescription.serviceReqMain != null && frmAssignPrescription.serviceReqMain.IS_MAIN_EXAM == 1) || (frmAssignPrescription.serviceReqMain != null && frmAssignPrescription.serviceReqMain.IS_SUB_PRES != 1)) && (HisConfigCFG.IsUsingSubPrescriptionMechanism == "1")) || (!GlobalStore.IsTreatmentIn && HisConfigCFG.IsUsingSubPrescriptionMechanism != "1") || GlobalStore.IsCabinet) && medicineTypeSDO__Category__SameMediAcin == null)
                        {
                            item.PrimaryKey = (this.medicineTypeSDO.SERVICE_ID + "__" + Inventec.Common.DateTime.Get.Now() + "__" + Guid.NewGuid().ToString());
                            if (GlobalStore.IsTreatmentIn && !GlobalStore.IsCabinet ? frmAssignPrescription.CheckMaxInPrescriptionInBatchWithMultilPatient(item, item.AMOUNT) : frmAssignPrescription.CheckMaxInPrescriptionInBatch(item, item.AMOUNT))
                            {
                                if (!(GlobalStore.IsTreatmentIn && !GlobalStore.IsCabinet))
                                    medicineTypeSDO.EXCEED_LIMIT_IN_BATCH_REASON = frmAssignPrescription.reasonMaxPrescriptionBatch;
                                if (TakeOrReleaseBeanWorker.TakeForCreateBean(frmAssignPrescription.intructionTimeSelecteds, this.expMestId, item, false, Param, frmAssignPrescription.UseTimeSelecteds, frmAssignPrescription.lstOutPatientPres))
                                {
                                    success = true;
                                    this.SaveDataAndRefesh(item);
                                    frmAssignPrescription.ReloadDataAvaiableMediBeanInCombo();
                                    LogSystem.Debug("SaveDataAndRefesh => 4");
                                }
                                else
                                {
                                    //Release stent
                                    MessageManager.Show(Param, success);
                                    return success = false;
                                }
                            }
                        }
                        else
                        {
                            item.TotalPrice = frmAssignPrescription.CalculatePrice(item);
                            item.PrimaryKey = (item.SERVICE_ID + "__" + Inventec.Common.DateTime.Get.Now() + "__" + Guid.NewGuid().ToString());
                            //if (frmAssignPrescription.servicePatyAllows != null && frmAssignPrescription.servicePatyAllows.ContainsKey(item.SERVICE_ID))
                            //{
                            //    var data_ServicePrice = frmAssignPrescription.servicePatyAllows[item.SERVICE_ID].Where(o => o.PATIENT_TYPE_ID == item.PATIENT_TYPE_ID).OrderByDescending(m => m.MODIFY_TIME).ToList();
                            //    if (data_ServicePrice != null && data_ServicePrice.Count > 0)
                            //    {
                            //        item.TotalPrice = (data_ServicePrice[0].PRICE * item.AMOUNT) ?? 0;
                            //    }
                            //}
                            if (GlobalStore.IsTreatmentIn && !GlobalStore.IsCabinet ? frmAssignPrescription.CheckMaxInPrescriptionInBatchWithMultilPatient(item, item.AMOUNT) : frmAssignPrescription.CheckMaxInPrescriptionInBatch(item, item.AMOUNT))
                            {
                                if (!(GlobalStore.IsTreatmentIn && !GlobalStore.IsCabinet))
                                    medicineTypeSDO.EXCEED_LIMIT_IN_BATCH_REASON = frmAssignPrescription.reasonMaxPrescriptionBatch;
                                success = true;
                                this.SaveDataAndRefesh(item);
                                frmAssignPrescription.ReloadDataAvaiableMediBeanInCombo();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                this.medicineTypeSDO = null;
                success = false;
                MessageManager.Show(Param, success);
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return success;
        }

        private bool ValidMetyMatyType__Add()
        {
            bool valid = true;
            try
            {
                valid = valid && this.CheckValidPre();
                if (this.ServiceTypeId == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC)
                {
                    valid = valid && this.ValidTutorialAndUseForm();
                    valid = valid && ValidAcinInteractiveWorker.ValidGrade(this.DataRow, MediMatyTypeADOs, ref frmAssignPrescription.txtInteractionReason, frmAssignPrescription);
                    valid = valid && this.ValidThuocDaKeTrongNgay();
                    valid = valid && MedicineAgeWorker.ValidThuocCoGioiHanTuoi(this.ServiceId, frmAssignPrescription.patientDob);
                    valid = valid && ValidThuocWithContraindicaterWarningOption();
                    valid = valid && WarningOddConvertWorker.CheckWarningOddConvertAmount(frmAssignPrescription.currentMedicineTypeADOForEdit, this.Amount, frmAssignPrescription.ResetFocusMediMaty);
                    valid = valid && ValidAcinInteractiveWorker.ValidSameAcin(MediMatyTypeADOs, frmAssignPrescription.currentMedicineTypeADOForEdit);
                }
                else if (this.ServiceTypeId == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__VT)
                {
                    valid = valid && this.ValidThuocDaKeTrongNgay();
                }
                valid = valid && this.ValidKhaDungThuocTrongKho();
                valid = valid && this.CheckPatientTypeHasValue();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return valid;
        }

        /// <summary>
        ///Khi kê số lượng vượt quá số lượng tồn, mà trong kho có thuốc cùng hoạt chất bhyt đang còn tồn thì hiển thị 
        ///thông báo để người dùng chọn, thông báo dưới dạng option:
        ///"Thuốc trong kho không đủ để kê. Bạn muốn sử dụng thuốc thay thế:
        ///1. Thuốc cùng hoạt chất bhyt
        ///2. Thuốc ngoài kho"
        ///nếu người dùng chọn thuốc cùng hoạt chất, thì copy tên hoạt chất vào ô tìm kiếm ==> tìm ra các thuốc cùng hoạt chất khác để người dùng chọn
        ///Trong trường hợp, số lượng vượt quá tồn, mà trong kho cũng không có thuốc nào cùng hoạt chất đang còn tồn thì hiển thị thông báo kiêu 
        ///"Thuốc đã chọn và các thuốc cùng hoạt chất khác trong kho không đủ để kê. Bạn có muốn kê thuốc ngoài kho không". Nếu chọn ok thì lấy thuốc ngoài kho, nếu ko thì ko xử lý j cả
        ///--------------------------------------------------------------
        ///Nếu chọn vật tư
        /// a. Ở grid hiển thị thuốc/vật tư tồn kho bổ sung cột "Vật tư ánh xạ" (lấy theo material_type_map_id trong his_material_type)
        ///b. Bổ sung gợi ý chọn vật tư "tương đương" khi người dùng nhập số lượng thuốc ko đủ khả dụng tương tự như chức năng gợi ý thuốc cùng hoạt chất. Cụ thể:
        ///Khi kê đơn, nếu người dùng nhập vật tư không đủ khả dụng, thì kiểm tra:
        ///- Trong danh sách tồn kho, có vật tư tương đương (có thông tin "vật tư ánh xạ" giống với vật tư mà người dùng chọn) nào có đủ tồn không.
        ///+ Nếu có vật tư tương đương thì hiển thị thông báo: "Vật tư X không đủ để kê. Bạn có muốn sử dụng vật tư thay thế không?"
        ///Và hiển thị các nút lựa chọn:
        ///"Vật tư tương đương",
        ///"Vật tư mua ngoài",
        ///"Bỏ qua"
        ///Nếu người dùng chọn "Vật tư tương đương" thì tự động focus vào ô tìm kiếm và và điền nội dung chính là tên của vật tư ánh xạ
        ///Nếu người dùng chọn "vật tư mua ngoài" thì tự động bổ sung vật tư ngoài kho
        ///Nếu người dùng chọn "Bỏ qua" thì kết thúc nghiệp vụ
        ///+ Nếu không có vật tư tương đương nào đủ khả dụng, thì hiển thị thông báo "Vật tư X không đủ để kê. Bạn có muốn kê vật tư mua ngoài không".
        ///Nếu người dùng đồng ý thì tự động bổ sung vật tư ngoài kho
        ///Nếu người dùng không đồng ý thì kêt thúc nghiệp vụ.
        /// </summary>
        /// <returns>bool</returns>
        private bool ValidKhaDungThuocTrongKho()
        {
            bool valid = true;
            CommonParam param = new CommonParam();
            try
            {
                try
                {
                    //Lay thuoc trong kho va kiem tra thuoc co con trong kho khong
                    decimal damount = AmountOutOfStock(this.ServiceId, (this.MediStockId ?? 0));
                    if (damount <= 0)
                    {
                        param.Messages.Add(ResourceMessage.ThuocKhongCoTrongKho);
                        throw new ArgumentNullException("medicinetypeStockSDO is null");
                    }

                    Rectangle buttonBounds = new Rectangle(frmAssignPrescription.txtMediMatyForPrescription.Bounds.X, frmAssignPrescription.txtMediMatyForPrescription.Bounds.Y, frmAssignPrescription.txtMediMatyForPrescription.Bounds.Width, frmAssignPrescription.txtMediMatyForPrescription.Bounds.Height);
                    if (this.Amount > this.AmountAvaiable)
                    {
                        if (GlobalStore.IsCabinet)
                        {
                            MessageBox.Show("Thuốc vật tư trong kho không đủ khả dụng");
                            return false;
                        }

                        if (this.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC)
                        {
                            var medicineTypeAcin__SameAcinBhyt = GetDataByActiveIngrBhyt();
                            frmMessageBoxChooseAcinBhyt form = new frmMessageBoxChooseAcinBhyt(ChonThuocTrongKhoCungHoatChat);
                            form.ShowDialog();

                            switch (this.ChonThuocThayThe)
                            {
                                case OptionChonThuocThayThe.None:
                                    frmAssignPrescription.spinAmount.SelectAll();
                                    frmAssignPrescription.spinAmount.Focus();
                                    valid = false;
                                    break;
                                case OptionChonThuocThayThe.ThuocCungHoatChat:
                                    //thì copy tên hoạt chất vào ô tìm kiếm ==> tìm ra các thuốc cùng hoạt chất khác để người dùng chọn
                                    frmAssignPrescription.txtMediMatyForPrescription.Text = medicineTypeAcin__SameAcinBhyt;
                                    frmAssignPrescription.gridViewMediMaty.ActiveFilterString = " [ACTIVE_INGR_BHYT_NAME] Like '%" + frmAssignPrescription.txtMediMatyForPrescription.Text + "%'";
                                    //+ " OR [CONCENTRA] Like '%" + txtMediMatyForPrescription.Text + "%'"
                                    //+ " OR [MEDI_STOCK_NAME] Like '%" + txtMediMatyForPrescription.Text + "%'";
                                    frmAssignPrescription.gridViewMediMaty.OptionsFilter.FilterEditorUseMenuForOperandsAndOperators = false;
                                    frmAssignPrescription.gridViewMediMaty.OptionsFilter.ShowAllTableValuesInCheckedFilterPopup = false;
                                    frmAssignPrescription.gridViewMediMaty.OptionsFilter.ShowAllTableValuesInFilterPopup = false;
                                    frmAssignPrescription.gridViewMediMaty.FocusedRowHandle = 0;
                                    frmAssignPrescription.gridViewMediMaty.OptionsView.ShowFilterPanelMode = DevExpress.XtraGrid.Views.Base.ShowFilterPanelMode.Never;
                                    frmAssignPrescription.gridViewMediMaty.OptionsFind.HighlightFindResults = true;

                                    frmAssignPrescription.popupControlContainerMediMaty.ShowPopup(new Point(buttonBounds.X, buttonBounds.Bottom + 25));
                                    frmAssignPrescription.txtMediMatyForPrescription.Focus();
                                    frmAssignPrescription.txtMediMatyForPrescription.SelectAll();
                                    valid = false;
                                    break;
                                case OptionChonThuocThayThe.ThuocNgoaiKho:
                                    //Trong trường hợp, số lượng vượt quá tồn, mà trong kho cũng không có thuốc nào cùng hoạt chất đang còn tồn thì hiển thị thông báo kiêu 
                                    //"Thuốc đã chọn và các thuốc cùng hoạt chất khác trong kho không đủ để kê. Bạn có muốn kê thuốc ngoài kho không". 
                                    //Nếu chọn ok thì lấy thuốc ngoài kho, nếu ko thì ko xử lý j cả
                                    if (frmAssignPrescription.currentMedicineTypes == null || frmAssignPrescription.currentMedicineTypes.Count == 0)
                                    {
                                        frmAssignPrescription.currentMedicineTypes = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_MEDICINE_TYPE>();
                                        long isOnlyDisplayMediMateIsBusiness = Inventec.Common.TypeConvert.Parse.ToInt64(HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(HisConfigCFG.ONLY_DISPLAY_MEDIMATE_IS_BUSINESS));
                                        if (isOnlyDisplayMediMateIsBusiness == 1 && frmAssignPrescription.currentMedicineTypes != null && frmAssignPrescription.currentMedicineTypes.Count > 0)
                                            frmAssignPrescription.currentMedicineTypes = frmAssignPrescription.currentMedicineTypes.Where(o => o.IS_BUSINESS.HasValue && o.IS_BUSINESS.Value == 1).ToList();
                                    }

                                    V_HIS_MEDICINE_TYPE medicineType = null;
                                    medicineType = frmAssignPrescription.currentMedicineTypes.Where(o => o.MEDICINE_TYPE_NAME == this.Name)
                                        .OrderBy(o => Math.Abs(o.SERVICE_ID - this.ServiceId)).FirstOrDefault();
                                    //Nếu không tìm được thuốc ngoài kho nào thì tự động chuyển sang thuốc khác (tự mua)
                                    if (medicineType == null)
                                    {
                                        AddMedicineTypeCategoryByOtherMedi();
                                    }
                                    //Nếu tìm thấy thuốc ngoài kho thì lấy luôn thuốc ngoài kho đó
                                    else
                                    {
                                        AddMedicineTypeCategoryBySameMediAcin(medicineType);
                                    }
                                    break;
                            }
                        }
                        else if (this.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.VATTU)
                        {
                            if (frmAssignPrescription.currentMedicineTypeADOForEdit.MATERIAL_TYPE_MAP_ID.HasValue)
                            {
                                Inventec.Common.Logging.LogSystem.Info("Vat tu " + frmAssignPrescription.currentMedicineTypeADOForEdit.MEDICINE_TYPE_NAME + "co cau hinh vat tu tuong duong, MATERIAL_TYPE_MAP_ID = " + frmAssignPrescription.currentMedicineTypeADOForEdit.MATERIAL_TYPE_MAP_ID);
                                var materialTypeMap = GetDataByMaterialTypeMap();
                                if (materialTypeMap != null)
                                {
                                    frmMessageBoxChooseMaterialTypeMap form = new frmMessageBoxChooseMaterialTypeMap(ChonVatTuTrongKhoTuongDuong, this.Name);
                                    form.ShowDialog();

                                    switch (this.ChonVatTuTuongDuong)
                                    {
                                        case EnumOptionChonVatTuTuongDuong.None:
                                            frmAssignPrescription.spinAmount.SelectAll();
                                            frmAssignPrescription.spinAmount.Focus();
                                            valid = false;
                                            break;
                                        case EnumOptionChonVatTuTuongDuong.VatTuTUongDuong:
                                            //thì copy tên hoạt chất vào ô tìm kiếm ==> tìm ra các thuốc cùng hoạt chất khác để người dùng chọn
                                            frmAssignPrescription.txtMediMatyForPrescription.Text = materialTypeMap.MATERIAL_TYPE_MAP_NAME;
                                            frmAssignPrescription.gridViewMediMaty.ActiveFilterString = " [MATERIAL_TYPE_MAP_NAME] Like '%" + frmAssignPrescription.txtMediMatyForPrescription.Text + "%'";
                                            //+ " OR [CONCENTRA] Like '%" + txtMediMatyForPrescription.Text + "%'"
                                            //+ " OR [MEDI_STOCK_NAME] Like '%" + txtMediMatyForPrescription.Text + "%'";
                                            frmAssignPrescription.gridViewMediMaty.OptionsFilter.FilterEditorUseMenuForOperandsAndOperators = false;
                                            frmAssignPrescription.gridViewMediMaty.OptionsFilter.ShowAllTableValuesInCheckedFilterPopup = false;
                                            frmAssignPrescription.gridViewMediMaty.OptionsFilter.ShowAllTableValuesInFilterPopup = false;
                                            frmAssignPrescription.gridViewMediMaty.FocusedRowHandle = 0;
                                            frmAssignPrescription.gridViewMediMaty.OptionsView.ShowFilterPanelMode = DevExpress.XtraGrid.Views.Base.ShowFilterPanelMode.Never;
                                            frmAssignPrescription.gridViewMediMaty.OptionsFind.HighlightFindResults = true;

                                            frmAssignPrescription.popupControlContainerMediMaty.ShowPopup(new Point(buttonBounds.X, buttonBounds.Bottom + 25));
                                            frmAssignPrescription.txtMediMatyForPrescription.Focus();
                                            frmAssignPrescription.txtMediMatyForPrescription.SelectAll();
                                            valid = false;
                                            break;
                                        case EnumOptionChonVatTuTuongDuong.VatTuMuaNgoai:
                                            //Trong trường hợp, số lượng vượt quá tồn, mà trong kho cũng không có thuốc nào cùng hoạt chất đang còn tồn thì hiển thị thông báo kiêu 
                                            //"Thuốc đã chọn và các thuốc cùng hoạt chất khác trong kho không đủ để kê. Bạn có muốn kê thuốc ngoài kho không". 
                                            //Nếu chọn ok thì lấy thuốc ngoài kho, nếu ko thì ko xử lý j cả
                                            if (frmAssignPrescription.currentMaterialTypes == null || frmAssignPrescription.currentMaterialTypes.Count == 0)
                                            {
                                                frmAssignPrescription.currentMaterialTypes = BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>().Where(o => o.IS_LEAF == GlobalVariables.CommonNumberTrue).ToList();
                                            }

                                            V_HIS_MATERIAL_TYPE maTy = null;
                                            maTy = frmAssignPrescription.currentMaterialTypes.Where(o => o.SERVICE_ID == this.ServiceId).FirstOrDefault();
                                            //Nếu tìm thấy vật tư ngoài kho thì lấy luôn vật tư ngoài kho đó
                                            if (maTy != null)
                                            {
                                                AddMaterialTypeCategoryBySameMediAcin(maTy);
                                            }
                                            break;
                                    }
                                }
                                else
                                {
                                    DialogResult myResult = MessageBox.Show(String.Format(ResourceMessage.VatTuKhongDuKeBanCoMuonKeVatTuMuaNgoai, this.Name), HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                                    if (myResult == DialogResult.Yes)
                                    {
                                        if (frmAssignPrescription.currentMaterialTypes == null || frmAssignPrescription.currentMaterialTypes.Count == 0)
                                        {
                                            frmAssignPrescription.currentMaterialTypes = BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>().Where(o => o.IS_LEAF == GlobalVariables.CommonNumberTrue).ToList();
                                        }

                                        V_HIS_MATERIAL_TYPE maTy = null;
                                        maTy = frmAssignPrescription.currentMaterialTypes.Where(o => o.SERVICE_ID == this.ServiceId).FirstOrDefault();
                                        //Nếu tìm thấy vật tư ngoài kho thì lấy luôn vật tư ngoài kho đó
                                        if (maTy != null)
                                        {
                                            AddMaterialTypeCategoryBySameMediAcin(maTy);
                                        }
                                    }
                                    else
                                    {
                                        valid = false;
                                    }
                                }
                            }
                            else
                            {
                                frmMessageBoxChooseVT form = new frmMessageBoxChooseVT(ChonVatTu);
                                form.ShowDialog();

                                switch (this.ChonVTThayThe)
                                {
                                    case EnumOptionChonVatTuThayThe.None:
                                        frmAssignPrescription.spinAmount.SelectAll();
                                        frmAssignPrescription.spinAmount.Focus();
                                        valid = false;
                                        break;
                                    case EnumOptionChonVatTuThayThe.VatTuNgoaiKho:

                                        List<V_HIS_MATERIAL_TYPE> materialTypes = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_MATERIAL_TYPE>();

                                        var materialType = materialTypes.FirstOrDefault(o => o.SERVICE_ID == this.ServiceId);
                                        if (materialType == null)
                                            throw new ArgumentNullException("Khong tim thay medicineType SERVICE_ID = " + this.ServiceId + " tu danh muc thuoc.");

                                        AddMaterialTypeCategoryBySameMediAcin(materialType);
                                        break;
                                }
                            }
                        }
                    }
                }
                catch (ArgumentNullException ex)
                {
                    valid = false;
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                catch (Exception ex)
                {
                    valid = false;
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }

                if (!String.IsNullOrEmpty(param.GetMessage()))
                    MessageManager.Show(param.GetMessage());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

            return valid;
        }

    }
}

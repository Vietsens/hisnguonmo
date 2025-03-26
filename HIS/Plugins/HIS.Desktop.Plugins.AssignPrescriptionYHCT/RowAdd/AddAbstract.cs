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
using DevExpress.XtraEditors.DXErrorProvider;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Plugins.AssignPrescriptionYHCT.ADO;
using HIS.Desktop.Plugins.AssignPrescriptionYHCT.AssignPrescription;
using HIS.Desktop.Plugins.AssignPrescriptionYHCT.Config;
using HIS.Desktop.Plugins.AssignPrescriptionYHCT.MessageBoxForm;
using HIS.Desktop.Plugins.AssignPrescriptionYHCT.Resources;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HIS.Desktop.LocalStorage.LocalData;

namespace HIS.Desktop.Plugins.AssignPrescriptionYHCT.Add
{
    public abstract class AddAbstract : EntityBase
    {
        protected long Id { get; set; }
        protected decimal Amount { get; set; }
        protected decimal AmountAvaiable { get; set; }
        protected int DataType { get; set; }
        protected string Code { get; set; }
        protected string Name { get; set; }
        protected string ManuFacturerName { get; set; }
        protected string ServiceUnitName { get; set; }
        protected string NationalName { get; set; }
        protected long ServiceId { get; set; }
        protected string Concentra { get; set; }
        protected long? MediStockId { get; set; }
        protected string MediStockCode { get; set; }
        protected string MediStockName { get; set; }
        protected long? HeinServiceTypeId { get; set; }
        protected long ServiceTypeId { get; set; }
        protected long? NumOrder { get; set; }
        protected string ActiveIngrBhytCode { get; set; }
        protected string ActiveIngrBhytName { get; set; }
        protected bool? IsStent { get; set; }
        protected bool? IsAllowOdd { get; set; }
        protected string Tutorial { get; set; }
        protected string HtuText { get; set; }
        protected string AtcGroupCodes { get; set; }
        protected string AtcCodes { get; set; }
        protected ValidAddRow ValidAddRow { get; set; }
        protected GetPatientTypeBySeTy GetPatientTypeBySeTy { get; set; }
        protected CalulateUseTimeTo CalulateUseTimeTo { get; set; }
        protected ExistsAssianInDay ExistsAssianInDay { get; set; }

        protected List<DMediStock1ADO> MediStockD1SDOs { get; set; }
        protected bool? IsOutKtcFee { get; set; }
        protected long TreatmentId { get; set; }
        protected MOS.EFMODEL.DataModels.V_HIS_PATIENT_TYPE_ALTER PatientTypeAlter { get; set; }
        protected long PatientId { get; set; }
        protected long RequestRoomId { get; set; }

        protected List<MediMatyTypeADO> MediMatyTypeADOs { get; set; }
        protected HisTreatmentWithPatientTypeInfoSDO TreatmentWithPatientTypeInfoSDO { get; set; }
        protected frmAssignPrescription frmAssignPrescription { get; set; }

        protected CommonParam Param { get; set; }
        protected MediMatyTypeADO medicineTypeSDO { get; set; }

        protected long HtuId { get; set; }
        protected long MedicineUseFormId { get; set; }
        protected bool IsExpend { get; set; }
        protected bool IsDisableExpend { get; set; }
        protected decimal? UseDays { get; set; }
        protected object DataRow { get; set; }
        protected bool IsMultiDateState { get; set; }
        protected List<long> IntructionTimeSelecteds { get; set; }

        protected short DoNotRequiredUseForm { get; set; }
        public OptionChonThuocThayThe ChonThuocThayThe { get; set; }
        public EnumOptionChonVatTuThayThe ChonVTThayThe { get; set; }
        protected MediMatyTypeADO medicineTypeSDO__Category__SameMediAcin;
        protected List<V_HIS_EXP_MEST_MEDICINE> LstExpMestMedicine { get; set; }

        protected AddAbstract(CommonParam param,
            frmAssignPrescription frmAssignPrescription,
            ValidAddRow validAddRow,
            GetPatientTypeBySeTy getPatientTypeBySeTy,
            CalulateUseTimeTo calulateUseTimeTo,
            ExistsAssianInDay existsAssianInDay,
            object dataRow
            )
            : base()
        {
            if (param == null)
                this.Param = new CommonParam();
            else
                this.Param = param;
            this.TreatmentId = frmAssignPrescription.currentTreatmentWithPatientType.ID;
            this.PatientId = frmAssignPrescription.currentTreatmentWithPatientType.PATIENT_ID;
            this.TreatmentWithPatientTypeInfoSDO = frmAssignPrescription.currentTreatmentWithPatientType;
            this.frmAssignPrescription = frmAssignPrescription;
            this.PatientTypeAlter = frmAssignPrescription.currentHisPatientTypeAlter;
            this.MediMatyTypeADOs = frmAssignPrescription.mediMatyTypeADOs;
            this.MediStockD1SDOs = frmAssignPrescription.mediStockD1ADOs;
            this.MedicineUseFormId = Inventec.Common.TypeConvert.Parse.ToInt64((frmAssignPrescription.cboMedicineUseForm.EditValue ?? "0").ToString());
            this.UseDays = frmAssignPrescription.spinSoNgay.Value;
            this.Amount = frmAssignPrescription.GetAmount();
            this.NumOrder = frmAssignPrescription.idRow;

            this.ValidAddRow = validAddRow;
            this.GetPatientTypeBySeTy = getPatientTypeBySeTy;
            this.CalulateUseTimeTo = calulateUseTimeTo;
            this.ExistsAssianInDay = existsAssianInDay;
            if (HisConfigCFG.ManyDayPrescriptionOption == 2 && (GlobalStore.IsTreatmentIn && !GlobalStore.IsCabinet))
            {
                this.IsMultiDateState = frmAssignPrescription.isMultiDateState;
                this.IntructionTimeSelecteds = frmAssignPrescription.intructionTimeSelecteds;
            }
            this.Tutorial = frmAssignPrescription.txtHuongDan.Text.Trim();
            this.HtuText = frmAssignPrescription.memHtu.Text.Trim();
            this.DataRow = dataRow;
            this.LstExpMestMedicine = frmAssignPrescription.LstExpMestMedicine;
        }

        protected void CreateADO()
        {
            medicineTypeSDO = new MediMatyTypeADO();
            medicineTypeSDO.ID = this.Id;
            if (this.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC
                || this.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC_DM
                || this.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC_TUTUC)
            {
                long ladder = Inventec.Common.TypeConvert.Parse.ToInt64(frmAssignPrescription.txtLadder.Text);
                medicineTypeSDO.RemedyCount = ladder;
                medicineTypeSDO.AMOUNT = this.Amount * ladder;
                medicineTypeSDO.AmountOneRemedy = this.Amount;

            }
            else
            {
                medicineTypeSDO.AMOUNT = this.Amount;
            }

            medicineTypeSDO.BK_AMOUNT = medicineTypeSDO.AMOUNT;
            medicineTypeSDO.DataType = this.DataType;
            medicineTypeSDO.MEDICINE_TYPE_CODE = this.Code;
            medicineTypeSDO.MEDICINE_TYPE_NAME = this.Name;
            medicineTypeSDO.MANUFACTURER_NAME = this.ManuFacturerName;
            medicineTypeSDO.SERVICE_UNIT_NAME = this.ServiceUnitName;
            medicineTypeSDO.NATIONAL_NAME = this.NationalName;
            medicineTypeSDO.SERVICE_ID = this.ServiceId;
            medicineTypeSDO.CONCENTRA = this.Concentra;
            medicineTypeSDO.MEDI_STOCK_ID = this.MediStockId;
            medicineTypeSDO.MEDI_STOCK_CODE = this.MediStockCode;
            medicineTypeSDO.MEDI_STOCK_NAME = this.MediStockName;
            medicineTypeSDO.HEIN_SERVICE_TYPE_ID = this.HeinServiceTypeId;
            medicineTypeSDO.SERVICE_TYPE_ID = this.ServiceTypeId;
            medicineTypeSDO.NUM_ORDER = this.NumOrder;
            medicineTypeSDO.ACTIVE_INGR_BHYT_CODE = this.ActiveIngrBhytCode;
            medicineTypeSDO.ACTIVE_INGR_BHYT_NAME = this.ActiveIngrBhytName;
            medicineTypeSDO.IsOutKtcFee = this.IsOutKtcFee;
            medicineTypeSDO.IS_STAR_MARK = frmAssignPrescription.currentMedicineTypeADOForEdit != null ? frmAssignPrescription.currentMedicineTypeADOForEdit.IS_STAR_MARK : null;
            medicineTypeSDO.IsStent = this.IsStent;

            medicineTypeSDO.OTHER_PAY_SOURCE_ID = frmAssignPrescription.currentMedicineTypeADOForEdit != null ? frmAssignPrescription.currentMedicineTypeADOForEdit.OTHER_PAY_SOURCE_ID : null;
            medicineTypeSDO.OTHER_PAY_SOURCE_CODE = frmAssignPrescription.currentMedicineTypeADOForEdit != null ? frmAssignPrescription.currentMedicineTypeADOForEdit.OTHER_PAY_SOURCE_CODE : "";
            medicineTypeSDO.OTHER_PAY_SOURCE_NAME = frmAssignPrescription.currentMedicineTypeADOForEdit != null ? frmAssignPrescription.currentMedicineTypeADOForEdit.OTHER_PAY_SOURCE_NAME : "";
            medicineTypeSDO.IsExpend = this.IsExpend;
            medicineTypeSDO.IsDisableExpend = this.IsDisableExpend;
            if (this.MedicineUseFormId > 0)
                medicineTypeSDO.MEDICINE_USE_FORM_ID = this.MedicineUseFormId;
            if (this.HtuId > 0)
                medicineTypeSDO.HTU_ID = this.HtuId;
            medicineTypeSDO.IntructionTimeSelecteds = this.IntructionTimeSelecteds;
            medicineTypeSDO.IsMultiDateState = this.IsMultiDateState;
            medicineTypeSDO.TUTORIAL = this.Tutorial;
            medicineTypeSDO.ATC_CODES = this.AtcCodes;
            medicineTypeSDO.ATC_GROUP_CODES = this.AtcGroupCodes;
            if (frmAssignPrescription.currentMedicineTypeADOForEdit != null)
            {
                if (frmAssignPrescription.currentMedicineTypeADOForEdit.LAST_EXP_PRICE.HasValue || frmAssignPrescription.currentMedicineTypeADOForEdit.LAST_EXP_VAT_RATIO.HasValue)
                {
                    medicineTypeSDO.LAST_EXP_PRICE = frmAssignPrescription.currentMedicineTypeADOForEdit.LAST_EXP_PRICE;
                    medicineTypeSDO.LAST_EXP_VAT_RATIO = frmAssignPrescription.currentMedicineTypeADOForEdit.LAST_EXP_VAT_RATIO;
                    decimal? priceRaw = this.Amount * (frmAssignPrescription.currentMedicineTypeADOForEdit.LAST_EXP_PRICE ?? 0) * (1 + (frmAssignPrescription.currentMedicineTypeADOForEdit.LAST_EXP_VAT_RATIO ?? 0));
                    priceRaw = (frmAssignPrescription.currentMedicineTypeADOForEdit.CONVERT_RATIO.HasValue && frmAssignPrescription.currentMedicineTypeADOForEdit.CONVERT_RATIO > 0) ? priceRaw / frmAssignPrescription.currentMedicineTypeADOForEdit.CONVERT_RATIO.Value : priceRaw;

                    medicineTypeSDO.TotalPrice = priceRaw ?? 0;
                    medicineTypeSDO.PRICE = frmAssignPrescription.currentMedicineTypeADOForEdit.LAST_EXP_PRICE;
                }
                else
                {
                    medicineTypeSDO.PRICE = frmAssignPrescription.currentMedicineTypeADOForEdit.IMP_PRICE;
                    medicineTypeSDO.TotalPrice = (this.Amount * (frmAssignPrescription.currentMedicineTypeADOForEdit.IMP_PRICE ?? 0)) * (1 + (frmAssignPrescription.currentMedicineTypeADOForEdit.IMP_VAT_RATIO ?? 0));
                }

                medicineTypeSDO.DO_NOT_REQUIRED_USE_FORM = frmAssignPrescription.currentMedicineTypeADOForEdit.DO_NOT_REQUIRED_USE_FORM;
            }

            medicineTypeSDO.HTU_TEXT = this.HtuText;
        }

        protected void SaveDataAndRefesh(MediMatyTypeADO mediMatyADO)
        {
            frmAssignPrescription.mediMatyTypeADOs.Add(mediMatyADO);
            frmAssignPrescription.idRow += frmAssignPrescription.stepRow;

            frmAssignPrescription.gridViewServiceProcess.BeginUpdate();
            frmAssignPrescription.gridViewServiceProcess.GridControl.DataSource = frmAssignPrescription.mediMatyTypeADOs.OrderBy(o => o.NUM_ORDER).ToList();
            frmAssignPrescription.gridViewServiceProcess.EndUpdate();

            frmAssignPrescription.ReSetDataInputAfterAdd__MedicinePage();
            frmAssignPrescription.SetEnableButtonControl(frmAssignPrescription.actionType);
            frmAssignPrescription.ResetFocusMediMaty(true);
            frmAssignPrescription.SetTotalPrice__TrongDon();
            //if ((mediMatyADO.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC
            //    || mediMatyADO.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.VATTU)
            //    && mediMatyADO.PATIENT_TYPE_ID != null && mediMatyADO.PATIENT_TYPE_ID == HisConfigCFG.PatientTypeId__BHYT)
            //{
            //    frmAssignPrescription.VerifyWarningOverCeiling();
            //}
        }

        /// <summary>
        /// #20013
        /// Khi bật key cấu hình hệ thống tự động tạo phiếu xuất bán (MOS.HIS_SERVICE_REQ.IS_AUTO_CREATE_SALE_EXP_MEST) 
        /// thì ko cho bổ sung thuốc/vật tư vượt quá số lượng khả dụng của nhà thuốc (tương tự như kê thuốc/vật tư trong kho)
        /// </summary>
        /// <returns></returns>      
        protected bool ValidKhaDungThuocTrongNhaThuoc()
        {
            bool valid = true;
            CommonParam paramWarn = new CommonParam();
            try
            {
                try
                {
                    if (
                        //HisConfigCFG.IsAutoCreateSaleExpMest && 
                        //(!GlobalStore.IsTreatmentIn || GlobalStore.IsCabinet)&&
                         this.frmAssignPrescription.cboNhaThuoc.EditValue != null
                        //&& this.IS_OUT_HOSPITAL != 1
                        )
                    {
                        //Lay thuoc trong kho va kiem tra thuoc co con trong kho khong
                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => ServiceId), ServiceId)
                            + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => this.frmAssignPrescription.currentMedicineTypeADOForEdit.MEDI_STOCK_ID), this.frmAssignPrescription.currentMedicineTypeADOForEdit.MEDI_STOCK_ID));
                        decimal damount = AmountOutOfStock(this.ServiceId, (this.frmAssignPrescription.currentMedicineTypeADOForEdit.MEDI_STOCK_ID ?? 0));
                        if (damount <= 0)
                        {
                            paramWarn.Messages.Add(ResourceMessage.ThuocKhongCoTrongKho);
                            throw new ArgumentNullException("medicinetypeStockSDO is null");
                        }
                        decimal amountAdded = 0;
                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => this.Amount), this.Amount)
                            + "__" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => amountAdded), amountAdded)
                            + "__" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => damount), damount)
                             + "__" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => frmAssignPrescription.currentMedicineTypeADOForEdit.AMOUNT), frmAssignPrescription.currentMedicineTypeADOForEdit.AMOUNT)
                            );
                        Rectangle buttonBounds = new Rectangle(frmAssignPrescription.txtMediMatyForPrescription.Bounds.X, frmAssignPrescription.txtMediMatyForPrescription.Bounds.Y, frmAssignPrescription.txtMediMatyForPrescription.Bounds.Width, frmAssignPrescription.txtMediMatyForPrescription.Bounds.Height);
                        if ((this.Amount + amountAdded) > (frmAssignPrescription.currentMedicineTypeADOForEdit.AMOUNT ?? 0))
                        {
                            MessageBox.Show("Thuốc vật tư trong kho không đủ khả dụng");
                            return false;

                            if (this.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC_DM)
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
                            else if (this.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.VATTU_DM)
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

                if (!String.IsNullOrEmpty(paramWarn.GetMessage()))
                    MessageManager.Show(paramWarn.GetMessage());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

            return valid;
        }
        
        protected void AddMaterialTypeCategoryBySameMediAcin(V_HIS_MATERIAL_TYPE addMaterialTypeADO)
        {
            try
            {
                if (addMaterialTypeADO == null) throw new ArgumentNullException("currentMedicineTypeADO");

                medicineTypeSDO__Category__SameMediAcin = new MediMatyTypeADO();
                medicineTypeSDO__Category__SameMediAcin.MEDICINE_TYPE_NAME = addMaterialTypeADO.MATERIAL_TYPE_NAME;
                medicineTypeSDO__Category__SameMediAcin.MEDICINE_TYPE_CODE = addMaterialTypeADO.MATERIAL_TYPE_CODE;
                medicineTypeSDO__Category__SameMediAcin.ID = addMaterialTypeADO.ID;
                medicineTypeSDO__Category__SameMediAcin.DataType = HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.VATTU_DM;
                medicineTypeSDO__Category__SameMediAcin.AMOUNT = this.Amount;
                medicineTypeSDO__Category__SameMediAcin.BK_AMOUNT = this.Amount;
                medicineTypeSDO__Category__SameMediAcin.NUM_ORDER = this.NumOrder;               
                medicineTypeSDO__Category__SameMediAcin.TUTORIAL = this.Tutorial;
                UpdateUseTimeInDataRow(medicineTypeSDO__Category__SameMediAcin);
                medicineTypeSDO__Category__SameMediAcin.IsOutKtcFee = this.IsOutKtcFee;
                medicineTypeSDO__Category__SameMediAcin.IsStent = this.IsStent;
                medicineTypeSDO__Category__SameMediAcin.IsExpend = this.IsExpend;
                medicineTypeSDO__Category__SameMediAcin.IsDisableExpend = this.IsDisableExpend;
                medicineTypeSDO__Category__SameMediAcin.UseDays = this.UseDays;
                medicineTypeSDO__Category__SameMediAcin.PATIENT_TYPE_ID = null;
                medicineTypeSDO__Category__SameMediAcin.PATIENT_TYPE_CODE = null;
                medicineTypeSDO__Category__SameMediAcin.PATIENT_TYPE_NAME = null;
                medicineTypeSDO__Category__SameMediAcin.MEDI_STOCK_ID = null;
                medicineTypeSDO__Category__SameMediAcin.MEDI_STOCK_CODE = null;
                medicineTypeSDO__Category__SameMediAcin.MEDI_STOCK_NAME = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        protected void ChonThuocTrongKhoCungHoatChat(OptionChonThuocThayThe chonThuocThayThe)
        {
            try
            {
                this.ChonThuocThayThe = chonThuocThayThe;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        protected void ChonVatTu(EnumOptionChonVatTuThayThe chonVTThayThe)
        {
            try
            {
                this.ChonVTThayThe = chonVTThayThe;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        protected void UpdateMediMatyByMedicineTypeCategory(MediMatyTypeADO addMedicineTypeADO)
        {
            try
            {
                if (addMedicineTypeADO == null) throw new ArgumentNullException("currentMedicineTypeADO");

                this.medicineTypeSDO = new MediMatyTypeADO();
                Inventec.Common.Mapper.DataObjectMapper.Map<MediMatyTypeADO>(this.medicineTypeSDO, addMedicineTypeADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        protected void AddMedicineTypeCategoryBySameMediAcin(V_HIS_MEDICINE_TYPE addMedicineTypeADO)
        {
            try
            {
                if (addMedicineTypeADO == null) throw new ArgumentNullException("currentMedicineTypeADO");

                medicineTypeSDO__Category__SameMediAcin = new MediMatyTypeADO();
                Inventec.Common.Mapper.DataObjectMapper.Map<MediMatyTypeADO>(medicineTypeSDO__Category__SameMediAcin, addMedicineTypeADO);
                medicineTypeSDO__Category__SameMediAcin.DataType = HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC_DM;

                long ladder = Inventec.Common.TypeConvert.Parse.ToInt64(frmAssignPrescription.txtLadder.Text);
                medicineTypeSDO__Category__SameMediAcin.RemedyCount = ladder;
                medicineTypeSDO__Category__SameMediAcin.AMOUNT = this.Amount * ladder;
                medicineTypeSDO__Category__SameMediAcin.AmountOneRemedy = this.Amount;

                medicineTypeSDO__Category__SameMediAcin.NUM_ORDER = this.NumOrder;
                //medicineTypeSDO__Category__SameMediAcin.Sang = this.Sang;
                //medicineTypeSDO__Category__SameMediAcin.Trua = this.Trua;
                //medicineTypeSDO__Category__SameMediAcin.Chieu = this.Chieu;
                //medicineTypeSDO__Category__SameMediAcin.Toi = this.Toi;
                medicineTypeSDO__Category__SameMediAcin.TUTORIAL = this.Tutorial;
                UpdateUseTimeInDataRow(medicineTypeSDO__Category__SameMediAcin);
                medicineTypeSDO__Category__SameMediAcin.IsOutKtcFee = this.IsOutKtcFee;
                medicineTypeSDO__Category__SameMediAcin.IsExpend = this.IsExpend;
                medicineTypeSDO__Category__SameMediAcin.IsDisableExpend = this.IsDisableExpend;
                medicineTypeSDO__Category__SameMediAcin.UseDays = this.UseDays;
                medicineTypeSDO__Category__SameMediAcin.PATIENT_TYPE_ID = null;
                medicineTypeSDO__Category__SameMediAcin.PATIENT_TYPE_CODE = null;
                medicineTypeSDO__Category__SameMediAcin.PATIENT_TYPE_NAME = null;
                medicineTypeSDO__Category__SameMediAcin.MEDI_STOCK_ID = null;
                medicineTypeSDO__Category__SameMediAcin.MEDI_STOCK_CODE = null;
                medicineTypeSDO__Category__SameMediAcin.MEDI_STOCK_NAME = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        protected void AddMedicineTypeCategoryByOtherMedi()
        {
            try
            {
                medicineTypeSDO__Category__SameMediAcin = new MediMatyTypeADO();
                medicineTypeSDO__Category__SameMediAcin.DataType = HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC_TUTUC;

                long ladder = Inventec.Common.TypeConvert.Parse.ToInt64(frmAssignPrescription.txtLadder.Text);
                medicineTypeSDO__Category__SameMediAcin.RemedyCount = ladder;
                medicineTypeSDO__Category__SameMediAcin.AMOUNT = this.Amount * ladder;
                medicineTypeSDO__Category__SameMediAcin.AmountOneRemedy = this.Amount;

                medicineTypeSDO__Category__SameMediAcin.NUM_ORDER = this.NumOrder;
                medicineTypeSDO__Category__SameMediAcin.SERVICE_UNIT_NAME = this.ServiceUnitName;
                medicineTypeSDO__Category__SameMediAcin.MEDICINE_TYPE_NAME = this.Name;
                //medicineTypeSDO__Category__SameMediAcin.Sang = this.Sang;
                //medicineTypeSDO__Category__SameMediAcin.Trua = this.Trua;
                //medicineTypeSDO__Category__SameMediAcin.Chieu = this.Chieu;
                //medicineTypeSDO__Category__SameMediAcin.Toi = this.Toi;
                medicineTypeSDO__Category__SameMediAcin.TUTORIAL = this.Tutorial;
                //UpdateUseTimeInDataRow(medicineTypeSDO__Category__SameMediAcin);
                //medicineTypeSDO__Category__SameMediAcin.IsOutKtcFee = this.IsOutKtcFee;
                //medicineTypeSDO__Category__SameMediAcin.IsStent = this.IsStent;
                //medicineTypeSDO__Category__SameMediAcin.IsExpend = this.IsExpend;
                //medicineTypeSDO__Category__SameMediAcin.UseDays = this.UseDays;
                medicineTypeSDO__Category__SameMediAcin.PATIENT_TYPE_ID = null;
                medicineTypeSDO__Category__SameMediAcin.PATIENT_TYPE_CODE = null;
                medicineTypeSDO__Category__SameMediAcin.PATIENT_TYPE_NAME = null;
                medicineTypeSDO__Category__SameMediAcin.MEDI_STOCK_ID = null;
                medicineTypeSDO__Category__SameMediAcin.MEDI_STOCK_CODE = null;
                medicineTypeSDO__Category__SameMediAcin.MEDI_STOCK_NAME = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        
        protected void UpdatePatientTypeInDataRow(MediMatyTypeADO medicineTypeSDO)
        {
            try
            {
                //Lay doi tuong mac dinh
                var patientTypeSelected = new MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE();
                patientTypeSelected = this.ChoosePatientTypeDefaultlService(this.PatientTypeAlter.PATIENT_TYPE_ID, medicineTypeSDO.SERVICE_ID, medicineTypeSDO.SERVICE_TYPE_ID);

                if (patientTypeSelected != null && patientTypeSelected.ID > 0)
                {
                    medicineTypeSDO.PATIENT_TYPE_ID = patientTypeSelected.ID;
                    medicineTypeSDO.PATIENT_TYPE_CODE = patientTypeSelected.PATIENT_TYPE_CODE;
                    medicineTypeSDO.PATIENT_TYPE_NAME = patientTypeSelected.PATIENT_TYPE_NAME;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        protected void UpdateExpMestReasonInDataRow(MediMatyTypeADO medicineTypeSDO)
        {
            try
            {
                if (medicineTypeSDO != null && frmAssignPrescription.actionType == GlobalVariables.ActionAdd)
                {
                    medicineTypeSDO.EXP_MEST_REASON_ID = null;
                    medicineTypeSDO.EXP_MEST_REASON_CODE = "";
                    medicineTypeSDO.EXP_MEST_REASON_NAME = "";

                    var dataExmeReasons = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_EXME_REASON_CFG>().Where(o => o.IS_ACTIVE == GlobalVariables.CommonNumberTrue
                            && o.PATIENT_CLASSIFY_ID == frmAssignPrescription.Histreatment.TDL_PATIENT_CLASSIFY_ID && o.TREATMENT_TYPE_ID == frmAssignPrescription.Histreatment.TDL_TREATMENT_TYPE_ID && (o.PATIENT_TYPE_ID == null || o.PATIENT_TYPE_ID == medicineTypeSDO.PATIENT_TYPE_ID)
                            && (o.OTHER_PAY_SOURCE_ID == null || o.OTHER_PAY_SOURCE_ID == medicineTypeSDO.OTHER_PAY_SOURCE_ID)).ToList();

                    if (dataExmeReasons != null && dataExmeReasons.Count > 0)
                    {
                        var data = (frmAssignPrescription.lstExpMestReasons != null && frmAssignPrescription.lstExpMestReasons.Count > 0) ? frmAssignPrescription.lstExpMestReasons.Where(o => o.ID == dataExmeReasons[0].EXP_MEST_REASON_ID).ToList() : null;

                        if (data != null && data.Count > 0)
                        {
                            medicineTypeSDO.EXP_MEST_REASON_ID = data[0].ID;
                            medicineTypeSDO.EXP_MEST_REASON_CODE = data[0].EXP_MEST_REASON_CODE;
                            medicineTypeSDO.EXP_MEST_REASON_NAME = data[0].EXP_MEST_REASON_NAME;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        protected void UpdateMedicineUseFormInDataRow(MediMatyTypeADO medicineTypeSDO)
        {
            try
            {
                //Duong dung, HDSD:
                if (this.MedicineUseFormId > 0 && medicineTypeSDO.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC)
                {
                    MOS.EFMODEL.DataModels.HIS_MEDICINE_USE_FORM data_dd = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_MEDICINE_USE_FORM>().FirstOrDefault(o => o.ID == this.MedicineUseFormId);
                    if (data_dd != null)
                    {
                        medicineTypeSDO.MEDICINE_USE_FORM_ID = data_dd.ID;
                        medicineTypeSDO.MEDICINE_USE_FORM_CODE = data_dd.MEDICINE_USE_FORM_CODE;
                        medicineTypeSDO.MEDICINE_USE_FORM_NAME = data_dd.MEDICINE_USE_FORM_NAME;
                    }
                }
                else
                {
                    medicineTypeSDO.MEDICINE_USE_FORM_ID = null;
                    medicineTypeSDO.MEDICINE_USE_FORM_CODE = "";
                    medicineTypeSDO.MEDICINE_USE_FORM_NAME = "";
                    medicineTypeSDO.ErrorMessageMedicineUseForm = "";
                    medicineTypeSDO.ErrorTypeMedicineUseForm = ErrorType.None;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        protected void UpdateUseTimeInDataRow(MediMatyTypeADO medicineTypeSDO)
        {
            try
            {
                if (this.CalulateUseTimeTo != null)
                {
                    long? useTimeTo = this.CalulateUseTimeTo();
                    if ((useTimeTo ?? 0) > 0)
                    {
                        medicineTypeSDO.UseTimeTo = useTimeTo;
                        medicineTypeSDO.UseDays = this.UseDays;
                    }
                    else
                    {
                        medicineTypeSDO.UseTimeTo = null;
                        medicineTypeSDO.UseDays = null;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE ChoosePatientTypeDefaultlService(long patientTypeId, long serviceId, long serviceTypeId)
        {
            MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE result = new MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE();
            try
            {
                if (this.GetPatientTypeBySeTy != null)
                {
                    return this.GetPatientTypeBySeTy(patientTypeId, serviceId, serviceTypeId);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        protected bool CheckValidPre()
        {
            bool valid = true;
            try
            {
                if (this.ValidAddRow != null)
                {
                    valid = this.ValidAddRow(this.DataRow);
                }
            }
            catch (Exception ex)
            {
                valid = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return valid;
        }

        protected bool ValidThuocDaKeTrongNgay()
        {
            bool valid = true;
            try
            {
                var medicinetypeStockExists = this.MediMatyTypeADOs
                     .FirstOrDefault(o => o.SERVICE_ID == this.ServiceId);
                if (medicinetypeStockExists != null)
                {
                    if (DevExpress.XtraEditors.XtraMessageBox.Show(ResourceMessage.ThuocDaduocKe, HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageBoxButtons.YesNo, MessageBoxIcon.Question, DevExpress.Utils.DefaultBoolean.True) == DialogResult.No)
                    {
                        valid = false;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return valid;
        }

        private List<MOS.EFMODEL.DataModels.V_HIS_MEDICINE_TYPE_ACIN> GetMedicineTypeAcinByMedicineType(List<long> medicineTypeIds)
        {
            List<MOS.EFMODEL.DataModels.V_HIS_MEDICINE_TYPE_ACIN> result = null;
            try
            {
                result = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_MEDICINE_TYPE_ACIN>()
                    .Where(o => medicineTypeIds.Contains(o.MEDICINE_TYPE_ID)).ToList();

                var medis = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_MEDICINE_TYPE_ACIN>();

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        protected object GetDataMediMatyInStock()
        {
            object result = null;
            try
            {
                //if (GlobalStore.IsTreatmentIn && !GlobalStore.IsCabinet)
                //{
                //    result = this.MediStockD2SDOs;
                //}
                //else
                //{
                result = this.MediStockD1SDOs;
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        protected decimal AmountOutOfStock(long serviceId, long meidStockId)
        {
            decimal result = 0;
            try
            {
                var checkMatyInStock = GetDataAmountOutOfStock(serviceId, meidStockId);
                var medi1 = checkMatyInStock as DMediStock1ADO;
                if (medi1 != null)
                {
                    result = (medi1.AMOUNT ?? 0);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        protected object GetDataAmountOutOfStock(long serviceId, long meidStockId)
        {
            object result = null;
            try
            {
                var result1 = this.MediStockD1SDOs.FirstOrDefault(o => o.SERVICE_ID == serviceId && (meidStockId == 0 || o.MEDI_STOCK_ID == meidStockId));
                result = result1;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        protected string GetDataByActiveIngrBhyt()
        {
            string result = "";
            try
            {
                var rs = this.MediStockD1SDOs.FirstOrDefault(o =>
                    !String.IsNullOrEmpty(this.ActiveIngrBhytName)
                    && !String.IsNullOrEmpty(o.ACTIVE_INGR_BHYT_NAME)
                    && (o.ACTIVE_INGR_BHYT_NAME.Contains(this.ActiveIngrBhytName))
                    && o.AMOUNT >= this.Amount
                    && ((o.SERVICE_ID == this.ServiceId && o.MEDI_STOCK_ID != this.MediStockId) || (o.SERVICE_ID != this.ServiceId)));
                if (rs != null)
                {
                    result = this.ActiveIngrBhytName;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        protected object GetDataByActiveIngrBhytConstain()
        {
            object result = null;
            try
            {
                //if (GlobalStore.IsTreatmentIn && !GlobalStore.IsCabinet)
                //{
                //    result = this.MediStockD2SDOs.FirstOrDefault(o => !String.IsNullOrEmpty(this.ActiveIngrBhytName) && o.ACTIVE_INGR_BHYT_NAME.Contains(this.ActiveIngrBhytName) && o.AMOUNT >= this.Amount);
                //}
                //else
                //{
                result = this.MediStockD1SDOs.FirstOrDefault(o => !String.IsNullOrEmpty(this.ActiveIngrBhytName) && o.ACTIVE_INGR_BHYT_NAME.Contains(this.ActiveIngrBhytName) && o.AMOUNT >= this.Amount);
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        protected void SetValidError()
        {
            try
            {
                SetValidAssianInDayError();
                SetValidMedicineUseFormError();
                SetValidPatientTypeError();
                SetValidAmountError();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        protected void SetValidAssianInDayError()
        {
            try
            {
                if (this.ExistsAssianInDay != null && this.ExistsAssianInDay(this.ServiceId))
                {
                    if (medicineTypeSDO.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC
                                    || medicineTypeSDO.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC_DM
                                    || medicineTypeSDO.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC_TUTUC)
                    {
                        medicineTypeSDO.ErrorMessageIsAssignDay = ResourceMessage.CanhBaoThuocDaKeTrongNgay;
                        medicineTypeSDO.ErrorTypeIsAssignDay = ErrorType.Warning;
                    }
                }
                else
                {
                    medicineTypeSDO.ErrorMessageIsAssignDay = "";
                    medicineTypeSDO.ErrorTypeIsAssignDay = ErrorType.None;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Cap nhat 11/09/2018
        /// Bat buoc nhap lieu dung voi tat ca cac doi tuong
        /// </summary>
        /// <returns></returns>
        protected bool ValidUseForm()
        {
            bool valid = true;
            try
            { //frmAssignPrescription.currentHisPatientTypeAlter.PATIENT_TYPE_ID
                var patientTypeSelected = new MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE();
                patientTypeSelected = this.ChoosePatientTypeDefaultlService(this.PatientTypeAlter.PATIENT_TYPE_ID, this.ServiceId, this.ServiceTypeId);

                if (patientTypeSelected != null
                    //&& patientTypeSelected.ID == HisConfigCFG.PatientTypeId__BHYT
                    && this.ServiceTypeId == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC)
                {

                    if (this.MediMatyTypeADOs == null)
                        this.MediMatyTypeADOs = new List<MediMatyTypeADO>();

                    MediMatyTypeADO mediMatyTypeADO = this.MediMatyTypeADOs.Where(o => !String.IsNullOrEmpty(o.TUTORIAL)).LastOrDefault();

                    if (this.MedicineUseFormId <= 0 && (this.DoNotRequiredUseForm) != RequiredUseFormCFG.DO_NOT_REQUIRED)
                    {
                        MessageBox.Show(ResourceMessage.BenhNhanDoiTuongTTBhytBatBuocPhaiNhapDuongDung, HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao));
                        valid = false;
                        frmAssignPrescription.txtMediMatyForPrescription.Focus();
                    }
                }
                else
                {
                    throw new Exception("Khong lay duoc thong tin doi tuong thanh toan ,Service id" + this.ServiceId);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return valid;
        }

        public bool CheckPatientTypeHasValue()
        {
            bool valid = true;
            try
            {
                var patientTypeSelected = this.ChoosePatientTypeDefaultlService(this.PatientTypeAlter.PATIENT_TYPE_ID, this.ServiceId, this.ServiceTypeId);
                if (patientTypeSelected == null || patientTypeSelected.ID == 0)
                {
                    MessageBox.Show(ResourceMessage.KhongTimThayChinhSachGiaCuaDichVu, HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao));
                    valid = false;
                }
            }
            catch (Exception ex)
            {
                valid = false;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return valid;
        }

        protected void SetValidMedicineUseFormError()
        {
            try
            {
                if (medicineTypeSDO.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC
                    && medicineTypeSDO.PATIENT_TYPE_ID == HisConfigCFG.PatientTypeId__BHYT
                    && (medicineTypeSDO.MEDICINE_USE_FORM_ID ?? 0) <= 0
                    && (medicineTypeSDO.DO_NOT_REQUIRED_USE_FORM ?? -1) != RequiredUseFormCFG.DO_NOT_REQUIRED)
                {
                    medicineTypeSDO.ErrorMessageMedicineUseForm = ResourceMessage.BenhNhanDoiTuongTTBhytBatBuocPhaiNhapDuongDung;
                    medicineTypeSDO.ErrorTypeMedicineUseForm = ErrorType.Warning;
                }
                else
                {
                    medicineTypeSDO.ErrorMessageMedicineUseForm = "";
                    medicineTypeSDO.ErrorTypeMedicineUseForm = ErrorType.None;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        protected void SetValidPatientTypeError()
        {
            try
            {
                if (medicineTypeSDO.PATIENT_TYPE_ID <= 0)
                {
                    medicineTypeSDO.ErrorMessagePatientTypeId = Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.ThieuTruongDuLieuBatBuoc);
                    medicineTypeSDO.ErrorTypePatientTypeId = ErrorType.Warning;
                }
                else
                {
                    medicineTypeSDO.ErrorMessagePatientTypeId = "";
                    medicineTypeSDO.ErrorTypePatientTypeId = ErrorType.None;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        protected void SetValidAmountError()
        {
            try
            {
                if (medicineTypeSDO.AMOUNT <= 0)
                {
                    medicineTypeSDO.ErrorMessageAmount = Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.ThieuTruongDuLieuBatBuoc);
                    medicineTypeSDO.ErrorTypeAmount = ErrorType.Warning;
                }
                else
                {
                    medicineTypeSDO.ErrorMessageAmount = "";
                    medicineTypeSDO.ErrorTypeAmount = ErrorType.None;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

    }
}

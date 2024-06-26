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
using HIS.Desktop.Plugins.AssignPrescriptionKidney.ADO;
using HIS.Desktop.Plugins.AssignPrescriptionKidney.AssignPrescription;
using HIS.Desktop.Plugins.AssignPrescriptionKidney.Base;
using HIS.Desktop.Plugins.AssignPrescriptionKidney.Config;
using HIS.Desktop.Plugins.AssignPrescriptionKidney.MessageBoxForm;
using HIS.Desktop.Plugins.AssignPrescriptionKidney.Resources;
using HIS.Desktop.Plugins.AssignPrescriptionKidney.Worker;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.AssignPrescriptionKidney.Add.MediStockD1SDO
{
    class MediStockD1SDORowAddBehavior : AddAbstract, IAdd
    {
        MediMatyTypeADO medicineTypeSDO__Category__SameMediAcin;
        long expMestId;
        internal MediStockD1SDORowAddBehavior(CommonParam param,
            frmAssignPrescription frmAssignPrescription,
            ValidAddRow validAddRow,
            GetPatientTypeBySeTy getPatientTypeBySeTy,
            CalulateUseTimeTo calulateUseTimeTo,
            ExistsAssianInDay existsAssianInDay,
            object dataRow)
            : base(param,
             frmAssignPrescription,
             validAddRow,
             getPatientTypeBySeTy,
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
            this.IsOutKtcFee = ((frmAssignPrescription.currentMedicineTypeADOForEdit.IS_OUT_PARENT_FEE ?? -1) == 1);
            //Chi dinh tu man hinh phau thuat, thu thuat
            if (((frmAssignPrescription.currentMedicineTypeADOForEdit.IS_AUTO_EXPEND ?? -1) == 1)
                || (HisConfigCFG.IsAutoTickExpendWithAssignPresPTTT && frmAssignPrescription.isAutoCheckExpend == true && (frmAssignPrescription.currentMedicineTypeADOForEdit.HEIN_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__VT_TDM || frmAssignPrescription.currentMedicineTypeADOForEdit.HEIN_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__VT_NDM)))
            {
                this.IsExpend = true;
            }
            this.expMestId = frmAssignPrescription.oldExpMestId;
            this.Speed = frmAssignPrescription.spinTocDoTruyen.EditValue != null ? (decimal?)frmAssignPrescription.spinTocDoTruyen.Value : null;
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
                        this.UpdatePatientTypeInDataRow(this.medicineTypeSDO);
                        this.UpdateMedicineUseFormInDataRow(this.medicineTypeSDO);
                        this.UpdateUseTimeInDataRow(this.medicineTypeSDO);
                        this.SetValidError();

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
                        item.TotalPrice = frmAssignPrescription.CalculatePrice(item);
                        //if (frmAssignPrescription.servicePatyAllows != null && frmAssignPrescription.servicePatyAllows.ContainsKey(item.SERVICE_ID))
                        //{
                        //    var data_ServicePrice = frmAssignPrescription.servicePatyAllows[item.SERVICE_ID].Where(o => o.PATIENT_TYPE_ID == item.PATIENT_TYPE_ID).OrderByDescending(m => m.MODIFY_TIME).ToList();
                        //    if (data_ServicePrice != null && data_ServicePrice.Count > 0)
                        //    {
                        //        item.TotalPrice = (data_ServicePrice[0].PRICE * item.AMOUNT) ?? 0;
                        //    }
                        //}

                        success = true;
                        this.SaveDataAndRefesh(item);
                        frmAssignPrescription.ReloadDataAvaiableMediBeanInCombo();
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
                    valid = valid && HIS.Desktop.Plugins.AssignPrescriptionKidney.ValidAcinInteractiveWorker.Valid(this.DataRow, MediMatyTypeADOs);
                    valid = valid && this.ValidThuocDaKeTrongNgay();
                    valid = valid && MedicineAgeWorker.ValidThuocCoGioiHanTuoi(this.ServiceId, frmAssignPrescription.patientDob);
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
                        //if (GlobalStore.IsCabinet)
                        //{
                        //    MessageBox.Show("Thuốc vật tư trong kho không đủ khả dụng");
                        //    return false;
                        //}

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

        private void ChonVatTu(EnumOptionChonVatTuThayThe chonVTThayThe)
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

        private void AddMedicineTypeCategoryBySameMediAcin(V_HIS_MEDICINE_TYPE addMedicineTypeADO)
        {
            try
            {
                if (addMedicineTypeADO == null) throw new ArgumentNullException("currentMedicineTypeADO");

                medicineTypeSDO__Category__SameMediAcin = new MediMatyTypeADO();
                Inventec.Common.Mapper.DataObjectMapper.Map<MediMatyTypeADO>(medicineTypeSDO__Category__SameMediAcin, addMedicineTypeADO);
                medicineTypeSDO__Category__SameMediAcin.DataType = HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC_DM;
                medicineTypeSDO__Category__SameMediAcin.AMOUNT = this.Amount;
                medicineTypeSDO__Category__SameMediAcin.BK_AMOUNT = this.Amount;
                medicineTypeSDO__Category__SameMediAcin.NUM_ORDER = this.NumOrder;
                medicineTypeSDO__Category__SameMediAcin.Sang = this.Sang;
                medicineTypeSDO__Category__SameMediAcin.Trua = this.Trua;
                medicineTypeSDO__Category__SameMediAcin.Chieu = this.Chieu;
                medicineTypeSDO__Category__SameMediAcin.Toi = this.Toi;
                medicineTypeSDO__Category__SameMediAcin.TUTORIAL = this.Tutorial;
                UpdateUseTimeInDataRow(medicineTypeSDO__Category__SameMediAcin);
                medicineTypeSDO__Category__SameMediAcin.IsOutKtcFee = this.IsOutKtcFee;
                medicineTypeSDO__Category__SameMediAcin.IsStent = this.IsStent;
                medicineTypeSDO__Category__SameMediAcin.IsExpend = this.IsExpend;
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

        private void AddMedicineTypeCategoryByOtherMedi()
        {
            try
            {
                medicineTypeSDO__Category__SameMediAcin = new MediMatyTypeADO();
                medicineTypeSDO__Category__SameMediAcin.DataType = HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC_TUTUC;
                medicineTypeSDO__Category__SameMediAcin.AMOUNT = this.Amount;
                medicineTypeSDO__Category__SameMediAcin.BK_AMOUNT = this.Amount;
                medicineTypeSDO__Category__SameMediAcin.NUM_ORDER = this.NumOrder;
                medicineTypeSDO__Category__SameMediAcin.SERVICE_UNIT_NAME = this.ServiceUnitName;
                medicineTypeSDO__Category__SameMediAcin.MEDICINE_TYPE_NAME = this.Name;
                medicineTypeSDO__Category__SameMediAcin.Sang = this.Sang;
                medicineTypeSDO__Category__SameMediAcin.Trua = this.Trua;
                medicineTypeSDO__Category__SameMediAcin.Chieu = this.Chieu;
                medicineTypeSDO__Category__SameMediAcin.Toi = this.Toi;
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

        private void AddMaterialTypeCategoryBySameMediAcin(V_HIS_MATERIAL_TYPE addMaterialTypeADO)
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
                medicineTypeSDO__Category__SameMediAcin.Sang = this.Sang;
                medicineTypeSDO__Category__SameMediAcin.Trua = this.Trua;
                medicineTypeSDO__Category__SameMediAcin.Chieu = this.Chieu;
                medicineTypeSDO__Category__SameMediAcin.Toi = this.Toi;
                medicineTypeSDO__Category__SameMediAcin.TUTORIAL = this.Tutorial;
                UpdateUseTimeInDataRow(medicineTypeSDO__Category__SameMediAcin);
                medicineTypeSDO__Category__SameMediAcin.IsOutKtcFee = this.IsOutKtcFee;
                medicineTypeSDO__Category__SameMediAcin.IsStent = this.IsStent;
                medicineTypeSDO__Category__SameMediAcin.IsExpend = this.IsExpend;
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

        private void UpdateMediMatyByMedicineTypeCategory(MediMatyTypeADO addMedicineTypeADO)
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

        private void ChonThuocTrongKhoCungHoatChat(OptionChonThuocThayThe chonThuocThayThe)
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
    }
}

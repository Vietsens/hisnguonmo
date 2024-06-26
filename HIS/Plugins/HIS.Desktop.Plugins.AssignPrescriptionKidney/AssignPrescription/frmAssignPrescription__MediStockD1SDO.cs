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
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.AssignPrescriptionKidney.ADO;
using Inventec.Common.Adapter;
using Inventec.Common.Logging;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.AssignPrescriptionKidney.AssignPrescription
{
    public partial class frmAssignPrescription : HIS.Desktop.Utility.FormBase
    {
        internal async Task RebuildMediMatyWithInControlContainer()
        {
            try
            {
                this.InitDataMetyMatyTypeInStockD();

                //Tại màn hình kê đơn, nếu phòng mà người dùng đang làm việc có "Giới hạn thuốc được phép sử dụng" (IS_RESTRICT_MEDICINE_TYPE trong HIS_ROOM bằng true) thì danh sách thuốc khi kê thuốc trong kho chỉ hiển thị các thuốc được khai cấu hình tương ứng với phòng đấy (dữ liệu lưu trong bảng HIS_MEDICINE_TYPE_ROOM)
                List<DMediStock1ADO> dMediStock1s = new List<DMediStock1ADO>();
                dMediStock1s.AddRange(this.mediStockD1ADOs);

                gridViewMediMaty.BeginUpdate();
                gridViewMediMaty.Columns.Clear();
                popupControlContainerMediMaty.Width = theRequiredWidth;
                popupControlContainerMediMaty.Height = theRequiredHeight;

                DevExpress.XtraGrid.Columns.GridColumn col1 = new DevExpress.XtraGrid.Columns.GridColumn();
                col1.FieldName = "MEDICINE_TYPE_CODE";
                col1.Caption = Inventec.Common.Resource.Get.Value
                    ("IVT_LANGUAGE_KEY__UC_HIS_ASSIGN_PRESCRIPTION__GV_MEDICINE__GC_MEDICINE_TYPE_CODE",
                    Resources.ResourceLanguageManager.LanguagefrmAssignPrescription,
                    Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                col1.Width = 60;
                col1.VisibleIndex = 1;
                gridViewMediMaty.Columns.Add(col1);

                DevExpress.XtraGrid.Columns.GridColumn col2 = new DevExpress.XtraGrid.Columns.GridColumn();
                col2.FieldName = "MEDICINE_TYPE_NAME";
                col2.Caption = Inventec.Common.Resource.Get.Value
                    ("IVT_LANGUAGE_KEY__UC_HIS_ASSIGN_PRESCRIPTION__GV_MEDICINE__GC_MEDICINE_TYPE_NAME",
                    Resources.ResourceLanguageManager.LanguagefrmAssignPrescription,
                    Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                col2.Width = 250;
                col2.VisibleIndex = 2;
                gridViewMediMaty.Columns.Add(col2);


                //if (isTSD)
                //{
                //    DevExpress.XtraGrid.Columns.GridColumn col1a = new DevExpress.XtraGrid.Columns.GridColumn();
                //    col1a.FieldName = "SERIAL_NUMBER";
                //    col1a.Caption = Inventec.Common.Resource.Get.Value
                //        ("IVT_LANGUAGE_KEY__UC_HIS_ASSIGN_PRESCRIPTION__GV_MEDICINE__GC_SERIAL_NUMBER",
                //        Resources.ResourceLanguageManager.LanguagefrmAssignPrescription,
                //        Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                //    col1a.Width = 150;
                //    col1a.VisibleIndex = 3;
                //    gridViewMediMaty.Columns.Add(col1a);
                //}


                DevExpress.XtraGrid.Columns.GridColumn col3 = new DevExpress.XtraGrid.Columns.GridColumn();
                col3.FieldName = "SERVICE_UNIT_NAME_DISPLAY";
                col3.Caption = Inventec.Common.Resource.Get.Value
                    ("IVT_LANGUAGE_KEY__UC_HIS_ASSIGN_PRESCRIPTION__GC_SERVICE_UNIT_NAME",
                    Resources.ResourceLanguageManager.LanguagefrmAssignPrescription,
                    Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                col3.Width = 60;
                col3.VisibleIndex = 3;
                col3.UnboundType = DevExpress.Data.UnboundColumnType.Object;
                gridViewMediMaty.Columns.Add(col3);


                //if (isTSD)
                //{
                //    DevExpress.XtraGrid.Columns.GridColumn col4a = new DevExpress.XtraGrid.Columns.GridColumn();
                //    col4a.FieldName = "USE_COUNT_DISPLAY";
                //    col4a.Caption = Inventec.Common.Resource.Get.Value
                //        ("IVT_LANGUAGE_KEY__UC_HIS_ASSIGN_PRESCRIPTION__GC_USE_COUNT__USE_REMAIN_COUNT",
                //        Resources.ResourceLanguageManager.LanguagefrmAssignPrescription,
                //        Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                //    col4a.Width = 140;
                //    col4a.VisibleIndex = 4;
                //    gridViewMediMaty.Columns.Add(col4a);
                //}
                //else
                //{
                DevExpress.XtraGrid.Columns.GridColumn col4 = new DevExpress.XtraGrid.Columns.GridColumn();
                col4.FieldName = "AMOUNT";
                col4.Caption = Inventec.Common.Resource.Get.Value
                    ("IVT_LANGUAGE_KEY__UC_HIS_ASSIGN_PRESCRIPTION__GC_AVAILABLE_AMOUNT",
                    Resources.ResourceLanguageManager.LanguagefrmAssignPrescription,
                    Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                col4.Width = 70;
                col4.VisibleIndex = 4;
                col4.DisplayFormat.FormatString = "#,##0.00";
                col4.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
                gridViewMediMaty.Columns.Add(col4);
                //}

                DevExpress.XtraGrid.Columns.GridColumn col5 = new DevExpress.XtraGrid.Columns.GridColumn();
                col5.FieldName = "IMP_PRICE_DISPLAY";
                col5.Caption = "Giá nhập";
                col5.Width = 100;
                col5.VisibleIndex = 5;
                col5.DisplayFormat.FormatString = "#,##0.00";
                col5.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
                col5.UnboundType = DevExpress.Data.UnboundColumnType.Object;
                gridViewMediMaty.Columns.Add(col5);

                DevExpress.XtraGrid.Columns.GridColumn col6 = new DevExpress.XtraGrid.Columns.GridColumn();
                col6.FieldName = "IMP_VAT_RATIO_DISPLAY";
                col6.Caption = "VAT(%)";
                col6.Width = 100;
                col6.VisibleIndex = 6;
                col6.UnboundType = DevExpress.Data.UnboundColumnType.Object;
                gridViewMediMaty.Columns.Add(col6);

                //if (!isTSD)
                //{
                DevExpress.XtraGrid.Columns.GridColumn col7 = new DevExpress.XtraGrid.Columns.GridColumn();
                col7.FieldName = "ACTIVE_INGR_BHYT_NAME";
                col7.Caption = Inventec.Common.Resource.Get.Value
                    ("IVT_LANGUAGE_KEY__UC_HIS_ASSIGN_PRESCRIPTION__GC_ACTIVE_INGR_BHYT_NAME",
                    Resources.ResourceLanguageManager.LanguagefrmAssignPrescription,
                    Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                col7.Width = 160;
                col7.VisibleIndex = 7;
                gridViewMediMaty.Columns.Add(col7);

                DevExpress.XtraGrid.Columns.GridColumn col8 = new DevExpress.XtraGrid.Columns.GridColumn();
                col8.FieldName = "CONCENTRA";
                col8.Caption = Inventec.Common.Resource.Get.Value
                    ("IVT_LANGUAGE_KEY__UC_HIS_ASSIGN_PRESCRIPTION__GC_CONCENTRA",
                    Resources.ResourceLanguageManager.LanguagefrmAssignPrescription,
                    Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                col8.Width = 100;
                col8.VisibleIndex = 8;
                gridViewMediMaty.Columns.Add(col8);
                //}

                DevExpress.XtraGrid.Columns.GridColumn col9 = new DevExpress.XtraGrid.Columns.GridColumn();
                col9.FieldName = "MEDI_STOCK_NAME";
                col9.Caption = Inventec.Common.Resource.Get.Value
                    ("IVT_LANGUAGE_KEY__UC_HIS_ASSIGN_PRESCRIPTION__GC_MEDI_STOCK",
                    Resources.ResourceLanguageManager.LanguagefrmAssignPrescription,
                    Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                col9.Width = 100;
                col9.VisibleIndex = 9;
                gridViewMediMaty.Columns.Add(col9);

                DevExpress.XtraGrid.Columns.GridColumn col10 = new DevExpress.XtraGrid.Columns.GridColumn();
                col10.FieldName = "MANUFACTURER_NAME";
                col10.Caption = Inventec.Common.Resource.Get.Value
                    ("IVT_LANGUAGE_KEY__UC_HIS_ASSIGN_PRESCRIPTION__GC_MANUFACTURER_NAME",
                    Resources.ResourceLanguageManager.LanguagefrmAssignPrescription,
                    Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                col10.Width = 150;
                col10.VisibleIndex = 10;
                gridViewMediMaty.Columns.Add(col10);

                DevExpress.XtraGrid.Columns.GridColumn col11 = new DevExpress.XtraGrid.Columns.GridColumn();
                col11.FieldName = "NATIONAL_NAME";
                col11.Caption = Inventec.Common.Resource.Get.Value
                    ("IVT_LANGUAGE_KEY__UC_HIS_ASSIGN_PRESCRIPTION__GC_NATIONAL_NAME",
                    Resources.ResourceLanguageManager.LanguagefrmAssignPrescription,
                    Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                col11.Width = 80;
                col11.VisibleIndex = 11;
                gridViewMediMaty.Columns.Add(col11);


                //Phuc vu cho tim kiem khong dau

                DevExpress.XtraGrid.Columns.GridColumn col12 = new DevExpress.XtraGrid.Columns.GridColumn();
                col12.FieldName = "MEDICINE_TYPE_CODE__UNSIGN";
                col12.Width = 80;
                col12.VisibleIndex = -1;
                gridViewMediMaty.Columns.Add(col12);

                DevExpress.XtraGrid.Columns.GridColumn col13 = new DevExpress.XtraGrid.Columns.GridColumn();
                col13.FieldName = "MEDICINE_TYPE_NAME__UNSIGN";
                col13.Width = 80;
                col13.OptionsFilter.AutoFilterCondition = DevExpress.XtraGrid.Columns.AutoFilterCondition.Like;
                col13.VisibleIndex = -1;
                gridViewMediMaty.Columns.Add(col13);

                DevExpress.XtraGrid.Columns.GridColumn col14 = new DevExpress.XtraGrid.Columns.GridColumn();
                col14.FieldName = "ACTIVE_INGR_BHYT_NAME__UNSIGN";
                col14.VisibleIndex = -1;
                col14.OptionsFilter.AutoFilterCondition = DevExpress.XtraGrid.Columns.AutoFilterCondition.Like;
                gridViewMediMaty.Columns.Add(col14);

                gridViewMediMaty.GridControl.DataSource = dMediStock1s;
                gridViewMediMaty.EndUpdate();
            }
            catch (Exception ex)
            {
                gridViewMediMaty.EndUpdate();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void MetyMatyTypeInStock_RowClick(object data)
        {
            try
            {
                this.currentMedicineTypeADOForEdit = new MediMatyTypeADO();
                if (data != null)
                {
                    Inventec.Common.Mapper.DataObjectMapper.Map<MediMatyTypeADO>(this.currentMedicineTypeADOForEdit, data);
                    if (CheckExistMedicinePaymentLimit(this.currentMedicineTypeADOForEdit.MEDICINE_TYPE_CODE))
                    {
                        MessageBox.Show("Thuốc có giới hạn chỉ định thanh toán BHYT. Đề nghị BS xem xét trước khi kê", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }

                    if (this.currentMedicineTypeADOForEdit == null) throw new ArgumentNullException("currentMedicineTypeADOForEdit is null");
                    if (data.GetType() == typeof(DMediStock1ADO))
                    {
                        DMediStock1ADO dMediStock = data as DMediStock1ADO;
                        this.currentMedicineTypeADOForEdit.IsStent = ((dMediStock.IS_STENT ?? 0) == GlobalVariables.CommonNumberTrue ? true : false);
                        this.currentMedicineTypeADOForEdit.IsAllowOdd = this.GetIsAllowOdd(dMediStock != null && dMediStock.ID.HasValue ? dMediStock.ID.Value : 0, this.currentMedicineTypeADOForEdit.SERVICE_TYPE_ID);
                        this.currentMedicineTypeADOForEdit.TDL_GENDER_ID = dMediStock.GENDER_ID;
                        this.SetControlSoLuongNgayNhapChanLe(this.currentMedicineTypeADOForEdit);
                    }

                    this.actionBosung = GlobalVariables.ActionAdd;
                    this.VisibleButton(this.actionBosung);
                    this.ReSetDataInputAfterAdd__MedicinePage();
                    this.btnAdd.Enabled = true;
                    this.btnAddTutorial.Enabled = true;
                    this.txtMediMatyForPrescription.Text = this.currentMedicineTypeADOForEdit.MEDICINE_TYPE_NAME;
                    if (this.currentMedicineTypeADOForEdit.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC)
                    {
                        var mety = BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>().Where(o => o.SERVICE_ID == this.currentMedicineTypeADOForEdit.SERVICE_ID).FirstOrDefault();
                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => mety), mety) + " " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => this.currentMedicineTypeADOForEdit.IS_KIDNEY), this.currentMedicineTypeADOForEdit.IS_KIDNEY));
                        if (mety != null)
                        {
                            this.currentMedicineTypeADOForEdit.IS_KIDNEY = mety.IS_KIDNEY;
                        }
                        else
                        {
                            this.currentMedicineTypeADOForEdit.IS_KIDNEY = null;
                        }

                        if (lciTocDoTruyen.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Always)
                            this.lciTocDoTruyen.Enabled = true;

                        this.FillDataIntoMedicineUseFormAndTutorial(currentMedicineTypeADOForEdit.ID);

                        //nếu không có ngày theo HDSD thì gán theo số ngày đơn
                        if (this.spinSoNgay.Value > 0 && this.spinSoLuongNgay.Value <= 0)
                            this.spinSoLuongNgay.Value = this.spinSoNgay.Value;

                        //Neu la thuoc thi kiem tra co mẫu HDSD chưa, có thì focus vào nút "Bổ sung"
                        if (this.medicineTypeTutSelected != null && this.medicineTypeTutSelected.ID > 0)
                        {
                            spinAmount.Focus();
                            spinAmount.SelectAll();
                        }
                        //Ngược lại kiểm tra có cấu hình PM cho phép sau khi chọn thuốc thì nhảy vào ô số lượng hay ô ngày
                        else
                        {
                            long focusMedicineDefault = ConfigApplicationWorker.Get<long>(AppConfigKeys.CONFIG_KEY__HIS_DESKTOP__ASSIGN_PRESCRIPTION__FOCUS_MEDICINE_DEFAULT);
                            //Nếu có cấu hình thì mặc định focus vào ô số lượng
                            if (focusMedicineDefault == 1)
                            {
                                this.spinAmount.Focus();
                                this.spinAmount.SelectAll();
                            }
                            //Ngược lại thì mặc định focus vào ô số ngày
                            else
                            {
                                this.spinSoLuongNgay.Focus();
                                this.spinSoLuongNgay.SelectAll();
                            }
                        }
                    }
                    else
                    {
                        if (lciTocDoTruyen.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Always)
                            this.lciTocDoTruyen.Enabled = false;
                        this.spinTocDoTruyen.EditValue = null;

                        //Neu la vat tu thi mặc định focus vào ô số lượng
                        this.spinAmount.Focus();
                        this.spinAmount.SelectAll();
                    }

                    ///Khoi tao cbo PatientType va set gia tri mac dinh theo service
                    FillDataIntoPatientTypeCombo(this.currentMedicineTypeADOForEdit, cboPatientType);
                    HIS_PATIENT_TYPE patientTypeDefault = ChoosePatientTypeDefaultlService(currentHisPatientTypeAlter.PATIENT_TYPE_ID,
                        this.currentMedicineTypeADOForEdit.SERVICE_ID, this.currentMedicineTypeADOForEdit.SERVICE_TYPE_ID);
                    if (patientTypeDefault != null)
                    {
                        cboPatientType.EditValue = patientTypeDefault.ID;
                    }
                    else
                    {
                        Inventec.Common.Logging.LogSystem.Error("Khong tim thay doi tuong thanh toan mac dinh cho dinh vu");
                        cboPatientType.EditValue = null;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void MaterialTypeTSD_RowClick(object data)
        {
            try
            {
                this.currentMedicineTypeADOForEdit = new MediMatyTypeADO();
                if (data != null)
                {
                    Inventec.Common.Mapper.DataObjectMapper.Map<MediMatyTypeADO>(this.currentMedicineTypeADOForEdit, data);

                    this.actionBosung = GlobalVariables.ActionAdd;
                    this.VisibleButton(this.actionBosung);
                    this.btnAdd.Enabled = true;
                    //this.btnAddTutorial.Enabled = true;
                    if (lciTocDoTruyen.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Always)
                        this.lciTocDoTruyen.Enabled = false;

                    Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(this.dxValidProviderBoXung, this.dxErrorProvider1);
                    Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(this.dxValidProviderBoXung__DuongDung, this.dxErrorProvider1);
                    Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(this.dxValidationProviderMaterialTypeTSD, this.dxErrorProvider1);

                    this.txtMediMatyForPrescription.Text = this.currentMedicineTypeADOForEdit.MEDICINE_TYPE_NAME;

                    this.btnAdd.Focus();

                    //this.currentMedicineTypeADOForEdit.IsAllowOdd = this.GetIsAllowOdd(this.currentMedicineTypeADOForEdit.ID, this.currentMedicineTypeADOForEdit.SERVICE_TYPE_ID);

                    ///Khoi tao cbo PatientType va set gia tri mac dinh theo service
                    FillDataIntoPatientTypeCombo(this.currentMedicineTypeADOForEdit, cboPatientType);
                    HIS_PATIENT_TYPE patientTypeDefault = ChoosePatientTypeDefaultlService(currentHisPatientTypeAlter.PATIENT_TYPE_ID,
                        this.currentMedicineTypeADOForEdit.SERVICE_ID, this.currentMedicineTypeADOForEdit.SERVICE_TYPE_ID);
                    if (patientTypeDefault != null)
                    {
                        cboPatientType.EditValue = patientTypeDefault.ID;
                    }
                    else
                    {
                        Inventec.Common.Logging.LogSystem.Error("Khong tim thay doi tuong thanh toan mac dinh cho dinh vu");
                        cboPatientType.EditValue = null;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private bool GetIsAllowOdd(long id, long serviceTypeId)
        {
            bool result = false;
            try
            {
                if (id > 0)
                {
                    if (serviceTypeId == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC)
                    {
                        V_HIS_MEDICINE_TYPE medicineType = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>().FirstOrDefault(o => o.ID == id);
                        if (medicineType != null && medicineType.IS_ALLOW_ODD.HasValue)
                        {
                            result = medicineType.IS_ALLOW_ODD == 1 ? true : false;
                        }
                    }
                    else if (serviceTypeId == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__VT)
                    {
                        V_HIS_MATERIAL_TYPE maType = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>().FirstOrDefault(o => o.ID == id);
                        if (maType != null && maType.IS_ALLOW_ODD.HasValue)
                        {
                            result = maType.IS_ALLOW_ODD == 1 ? true : false;
                        }
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

        private void FillDataIntoMedicineUseFormAndTutorial(long medicineTypeId)
        {
            try
            {
                //Lấy dữ liệu cấu hình hướng dẫn sử dụng của thuốc (HIS_MEDICINE_TYPE_TUT) theo tài khoản đăng nhập và loại thuốc
                string loginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                var medicineTypeTuts = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_MEDICINE_TYPE_TUT>();
                if (medicineTypeTuts != null && medicineTypeTuts.Count > 0)
                {
                    List<HIS_MEDICINE_TYPE_TUT> medicineTypeTutFilters = medicineTypeTuts.OrderByDescending(o => o.MODIFY_TIME).Where(o => o.MEDICINE_TYPE_ID == medicineTypeId && o.LOGINNAME == loginName).ToList();

                    this.RebuildTutorialWithInControlContainer(medicineTypeTutFilters);
                    this.medicineTypeTutSelected = medicineTypeTutFilters.FirstOrDefault();
                    if (this.medicineTypeTutSelected != null)
                    {
                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => this.medicineTypeTutSelected), this.medicineTypeTutSelected));
                        //Nếu hướng dẫn sử dụng mẫu có đường dùng thì lấy ra
                        if (this.medicineTypeTutSelected.MEDICINE_USE_FORM_ID > 0)
                        {
                            this.cboMedicineUseForm.EditValue = this.medicineTypeTutSelected.MEDICINE_USE_FORM_ID;
                        }
                        //Nếu không có đường dùng thì lấy đường dùng từ danh mục loại thuốc
                        else
                        {
                            var medicineType = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_MEDICINE_TYPE>().FirstOrDefault(o => o.ID == medicineTypeId);
                            if (medicineType != null && (medicineType.MEDICINE_USE_FORM_ID ?? 0) > 0)
                            {
                                this.cboMedicineUseForm.EditValue = medicineType.MEDICINE_USE_FORM_ID;
                            }
                        }

                        this.cboHtu.EditValue = this.medicineTypeTutSelected.HTU_ID;
                        if (this.medicineTypeTutSelected.HTU_ID != null)
                            this.cboHtu.Properties.Buttons[1].Visible = true;
                        else
                            this.cboHtu.Properties.Buttons[1].Visible = false;

                        if (this.spinSoNgay.Value < (this.medicineTypeTutSelected.DAY_COUNT ?? 0))
                            this.spinSoNgay.EditValue = this.medicineTypeTutSelected.DAY_COUNT;
                        this.spinSoLuongNgay.EditValue = this.medicineTypeTutSelected.DAY_COUNT;

                        Inventec.Common.Logging.LogSystem.Debug("Truong hop co HDSD thuoc theo tai khoan cua loai thuoc (HIS_MEDICINE_TYPE_TUT)--> lay truong DAY_COUNT gan vao spinSoNgay" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => medicineTypeTutSelected), medicineTypeTutSelected));

                        this.spinSang.EditValue = this.medicineTypeTutSelected.MORNING;
                        this.spinTrua.EditValue = this.medicineTypeTutSelected.NOON;
                        this.spinChieu.EditValue = this.medicineTypeTutSelected.AFTERNOON;
                        this.spinToi.EditValue = this.medicineTypeTutSelected.EVENING;
                        if (String.IsNullOrEmpty(this.txtTutorial.Text)
                            || String.IsNullOrEmpty(txtLadder.Text))
                        {
                            //Nếu có trường hướng dẫn thì sử dụng luôn
                            if (!String.IsNullOrEmpty(this.medicineTypeTutSelected.TUTORIAL))
                            {
                                this.txtTutorial.Text = this.medicineTypeTutSelected.TUTORIAL;
                            }
                            //Nếu không có hướng dẫn sử dụng thì tự động set theo các trường như lúc nhập liệu
                            else
                            {
                                this.CalculateAmount();
                                this.SetHuongDanFromSoLuongNgay();
                            }
                        }
                    }
                }
                //Trường hợp thuốc không có cấu hình hướng dẫn sử dụng thì lấy hướng dẫn sử dụng ở danh mục loại thuốc fill vào
                else
                    this.medicineTypeTutSelected = null;

                if (this.medicineTypeTutSelected == null)
                {
                    var medicineType = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_MEDICINE_TYPE>().FirstOrDefault(o => o.ID == medicineTypeId);
                    if (medicineType != null)
                    {
                        if ((medicineType.MEDICINE_USE_FORM_ID ?? 0) > 0)
                        {
                            this.cboMedicineUseForm.EditValue = medicineType.MEDICINE_USE_FORM_ID;
                        }
                        if ((String.IsNullOrEmpty(this.txtTutorial.Text)
                            || String.IsNullOrEmpty(txtLadder.Text)))
                            this.txtTutorial.Text = medicineType.TUTORIAL;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private long? CalulateUseTimeTo()
        {
            long? result = null;
            try
            {
                if (this.intructionTimeSelecteds != null && this.intructionTimeSelecteds.OrderByDescending(o => o).First() > 0 && !string.IsNullOrEmpty(this.spinSoLuongNgay.Text))
                {
                    long useTime = this.intructionTimeSelecteds.OrderByDescending(o => o).First();
                    DateTime dtUseTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(useTime) ?? DateTime.MinValue;
                    DateTime dtUseTimeTo = dtUseTime.AddDays((double)this.spinSoLuongNgay.Value - 1);
                    long useTimeTo = Inventec.Common.TypeConvert.Parse.ToInt64(dtUseTimeTo.ToString("yyyyMMddHHmm") + "00");
                    result = useTimeTo;
                }
            }
            catch (Exception ex)
            {
                result = null;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private bool ExistsAssianInDay(long serviceId)
        {
            bool result = false;
            try
            {
                if (this.sereServWithTreatment != null && this.sereServWithTreatment.Count > 0)
                {
                    result = this.sereServWithTreatment.Where(o => o.SERVICE_ID == serviceId
                        && o.INTRUCTION_TIME.ToString().Substring(0, 8) == intructionTimeSelecteds.OrderByDescending(t => t).First().ToString().Substring(0, 8)).Any();
                }
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private void InitDataMetyMatyTypeInStockD()
        {
            try
            {
                InitDataMetyMatyTypeInStockD1(this.currentMediStock);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Gọi api lấy dữ liệu thuốc và vật tư gộp trong 1 danh sách, lọc theo các điều kiện và cấu hình khác => sử dụng trong phòng khám
        /// </summary>
        private void InitDataMetyMatyTypeInStockD1(List<MOS.EFMODEL.DataModels.V_HIS_MEST_ROOM> currentMediStock)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("InitDataMetyMatyTypeInStockD1 .1");
                this.mediStockD1ADOs = new List<DMediStock1ADO>();
                if (currentMediStock != null && currentMediStock.Count > 0)
                {
                    CommonParam param = new CommonParam();
                    MOS.Filter.DHisMediStock1Filter filter = new MOS.Filter.DHisMediStock1Filter();
                    List<long> mediStockIds = new List<long>();

                    mediStockIds = currentMediStock.Select(o => o.MEDI_STOCK_ID).ToList();
                    filter.MEDI_STOCK_IDs = mediStockIds;
                    filter.IS_REUSABLE = false;

                    this.mediMatyTypeAvailables = new BackendAdapter(param).Get<List<D_HIS_MEDI_STOCK_1>>(HisRequestUriStore.HIS_MEDISTOCKDISDO_GET1, ApiConsumers.MosConsumer, filter, ProcessLostToken, param);
                    LogSystem.Debug("Load du lieu kho theo dieu kien loc 1____ " + String.Join(",", mediStockIds) + "____ket qua tim thay " + (this.mediMatyTypeAvailables != null ? this.mediMatyTypeAvailables.Count : 0));

                    MediStockWorker.FilterByMediStockMeti(mediStockIds, ref this.mediMatyTypeAvailables);
                    MediStockWorker.FilterByMestMetyDepa(mediStockIds, this.currentWorkPlace, ref this.mediMatyTypeAvailables);

                    long roomId = GetRoomId();
                    V_HIS_ROOM room = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == roomId);
                    if (room != null && (room.IS_RESTRICT_MEDICINE_TYPE ?? 0) == 1)
                    {
                        List<HIS_MEDICINE_TYPE_ROOM> medicineTypeRooms = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_MEDICINE_TYPE_ROOM>()
                            .Where(o => o.ROOM_ID == roomId).ToList();
                        List<long> medicineTypeIdRooms = medicineTypeRooms != null ? medicineTypeRooms.Select(o => o.MEDICINE_TYPE_ID).ToList() : new List<long>();
                        this.mediMatyTypeAvailables = this.mediMatyTypeAvailables.Where(o => medicineTypeIdRooms.Contains(o.ID ?? 0)
                            || o.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__VT).ToList();
                    }

                    //Loc du lieu theo Rank
                    if (cboUser.EditValue != null)
                    {
                        string loginname = cboUser.EditValue.ToString();
                        List<HIS_EMPLOYEE> employees = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_EMPLOYEE>();
                        if (employees == null) employees = new List<HIS_EMPLOYEE>();
                        HIS_EMPLOYEE employee = employees.FirstOrDefault(o => o.LOGINNAME == loginname);
                        if (employee == null || !employee.MEDICINE_TYPE_RANK.HasValue)
                        {
                            this.mediMatyTypeAvailables = this.mediMatyTypeAvailables.Where(o => o.RANK == null).ToList();
                        }
                        else
                        {
                            this.mediMatyTypeAvailables = this.mediMatyTypeAvailables.Where(o => o.RANK == null || o.RANK <= employee.MEDICINE_TYPE_RANK).ToList();
                        }
                    }

                    //if (this.currentHisPatientTypeAlter != null && this.servicePatyAllows != null)
                    //    this.mediMatyTypeAvailables = this.mediMatyTypeAvailables.Where(o => this.servicePatyAllows.ContainsKey((o.SERVICE_ID ?? 0))).ToList();

                    this.mediMatyTypeAvailables = this.mediMatyTypeAvailables.Where(o => (o.AMOUNT ?? 0) > 0).ToList();

                    LogSystem.Debug("Du lieu thuoc/vat tu theo kho sau khi loc theo dieu kien loc khac____ " + "____ket qua tim thay " + (this.mediMatyTypeAvailables != null ? this.mediMatyTypeAvailables.Count : 0));

                    List<D_HIS_MEDI_STOCK_1> mediStockD1s = new List<D_HIS_MEDI_STOCK_1>();
                    mediStockD1s.AddRange(this.mediMatyTypeAvailables);
                    mediStockD1s = mediStockD1s.Where(o => (o.IS_REUSABLE == null || o.IS_REUSABLE != 1)).ToList();
                    this.mediStockD1ADOs = ConvertToDMediStock1(mediStockD1s);

                    Inventec.Common.Logging.LogSystem.Debug("InitDataMetyMatyTypeInStockD1 .3");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            LogSystem.Debug("Du lieu thuoc/vat tu theo kho sau khi loc____ " + "____ket qua tim thay " + (this.mediStockD1ADOs != null ? this.mediStockD1ADOs.Count : 0) + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => currentMediStock), currentMediStock));
        }

        private List<DMediStock1ADO> ConvertToDMediStock1(List<D_HIS_MEDI_STOCK_1> listMediStock)
        {
            List<DMediStock1ADO> result = new List<DMediStock1ADO>();
            try
            {
                if (listMediStock != null && listMediStock.Count > 0)
                {
                    foreach (var item in listMediStock)
                    {
                        DMediStock1ADO dMediStock1ADO = new DMediStock1ADO();
                        var mety = this.currentMedicineTypes.Where(o => o.SERVICE_ID == item.SERVICE_ID).FirstOrDefault();
                        if (mety != null && mety.IS_KIDNEY == 1)
                        {
                            dMediStock1ADO.IS_KIDNEY = mety.IS_KIDNEY;
                            dMediStock1ADO.ACTIVE_INGR_BHYT_CODE = item.ACTIVE_INGR_BHYT_CODE;
                            dMediStock1ADO.ACTIVE_INGR_BHYT_NAME = item.ACTIVE_INGR_BHYT_NAME;
                            dMediStock1ADO.ALERT_MAX_IN_PRESCRIPTION = item.ALERT_MAX_IN_PRESCRIPTION;
                            dMediStock1ADO.ALERT_MAX_IN_TREATMENT = item.ALERT_MAX_IN_TREATMENT;
                            dMediStock1ADO.ALERT_MIN_IN_STOCK = item.ALERT_MIN_IN_STOCK;
                            dMediStock1ADO.AMOUNT = item.CONVERT_RATIO.HasValue ? item.AMOUNT * item.CONVERT_RATIO : item.AMOUNT;
                            dMediStock1ADO.CONCENTRA = item.CONCENTRA;
                            dMediStock1ADO.GENDER_ID = item.GENDER_ID;
                            dMediStock1ADO.HEIN_SERVICE_TYPE_ID = item.HEIN_SERVICE_TYPE_ID;
                            dMediStock1ADO.ID = item.ID;
                            dMediStock1ADO.IMP_PRICE = item.IMP_PRICE;
                            dMediStock1ADO.IMP_VAT_RATIO = item.IMP_VAT_RATIO;
                            dMediStock1ADO.IS_ACTIVE = item.IS_ACTIVE;
                            dMediStock1ADO.IS_AUTO_EXPEND = item.IS_AUTO_EXPEND;
                            dMediStock1ADO.IS_CHEMICAL_SUBSTANCE = item.IS_CHEMICAL_SUBSTANCE;
                            dMediStock1ADO.IS_OUT_PARENT_FEE = item.IS_OUT_PARENT_FEE;
                            dMediStock1ADO.IS_STAR_MARK = item.IS_STAR_MARK;
                            dMediStock1ADO.IS_STENT = item.IS_STENT;
                            dMediStock1ADO.IS_VACCINE = item.IS_VACCINE;
                            dMediStock1ADO.MANUFACTURER_CODE = item.MANUFACTURER_CODE;
                            dMediStock1ADO.MANUFACTURER_ID = item.MANUFACTURER_ID;
                            dMediStock1ADO.MANUFACTURER_NAME = item.MANUFACTURER_NAME;
                            dMediStock1ADO.MEDI_STOCK_CODE = item.MEDI_STOCK_CODE;
                            dMediStock1ADO.MEDI_STOCK_ID = item.MEDI_STOCK_ID;
                            dMediStock1ADO.MEDI_STOCK_NAME = item.MEDI_STOCK_NAME;
                            dMediStock1ADO.MEDICINE_TYPE_CODE = item.MEDICINE_TYPE_CODE;
                            dMediStock1ADO.MEDICINE_TYPE_NAME = item.MEDICINE_TYPE_NAME;
                            dMediStock1ADO.MEDICINE_USE_FORM_ID = item.MEDICINE_USE_FORM_ID;
                            dMediStock1ADO.NATIONAL_NAME = item.NATIONAL_NAME;
                            dMediStock1ADO.RANK = item.RANK;
                            dMediStock1ADO.SERVICE_ID = item.SERVICE_ID;
                            dMediStock1ADO.SERVICE_TYPE_ID = item.SERVICE_TYPE_ID;
                            dMediStock1ADO.SERVICE_UNIT_CODE = item.SERVICE_UNIT_CODE;
                            dMediStock1ADO.SERVICE_UNIT_ID = item.SERVICE_UNIT_ID;
                            dMediStock1ADO.SERVICE_UNIT_NAME = item.SERVICE_UNIT_NAME;
                            dMediStock1ADO.TUTORIAL = item.TUTORIAL;
                            dMediStock1ADO.USE_ON_DAY = item.USE_ON_DAY;
                            dMediStock1ADO.CONVERT_RATIO = item.CONVERT_RATIO;
                            dMediStock1ADO.CONVERT_UNIT_CODE = item.CONVERT_UNIT_CODE;
                            dMediStock1ADO.CONVERT_UNIT_NAME = item.CONVERT_UNIT_NAME;

                            dMediStock1ADO.MEDICINE_TYPE_CODE__UNSIGN = StringUtil.convertToUnSign3(item.MEDICINE_TYPE_CODE) + item.MEDICINE_TYPE_CODE;
                            dMediStock1ADO.MEDICINE_TYPE_NAME__UNSIGN = StringUtil.convertToUnSign3(item.MEDICINE_TYPE_NAME) + item.MEDICINE_TYPE_NAME;
                            dMediStock1ADO.ACTIVE_INGR_BHYT_NAME__UNSIGN = StringUtil.convertToUnSign3(item.ACTIVE_INGR_BHYT_NAME) + item.ACTIVE_INGR_BHYT_NAME;

                            result.Add(dMediStock1ADO);
                        }
                        else
                        {
                            //Inventec.Common.Logging.LogSystem.Debug("Thuoc khong phai la thuoc chay than" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => mety), mety));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private void UpdateMedicineUseFormInDataRow(MediMatyTypeADO medicineTypeSDO)
        {
            try
            {
                bool hasUseForm = false;
                if (medicineTypeSDO.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC)
                {
                    V_HIS_MEDICINE_TYPE mety = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_MEDICINE_TYPE>().FirstOrDefault(o => o.SERVICE_ID == medicineTypeSDO.SERVICE_ID);
                    if (mety != null)
                    {
                        medicineTypeSDO.MEDICINE_USE_FORM_ID = mety.MEDICINE_USE_FORM_ID;
                        medicineTypeSDO.MEDICINE_USE_FORM_CODE = mety.MEDICINE_USE_FORM_CODE;
                        medicineTypeSDO.MEDICINE_USE_FORM_NAME = mety.MEDICINE_USE_FORM_NAME;
                        hasUseForm = true;
                    }
                }
                if (!hasUseForm)
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
    }
}

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
using DevExpress.XtraEditors.ViewInfo;
using HIS.Desktop.ADO;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.BackendData.ADO;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.HisConfig;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.AssignPrescriptionYHCT.ADO;
using HIS.Desktop.Plugins.AssignPrescriptionYHCT.Config;
using HIS.Desktop.Plugins.AssignPrescriptionYHCT.Resources;
using HIS.Desktop.Utilities.Extentions;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Core;
using Inventec.Desktop.Common.Controls.ValidationRule;
using Inventec.Desktop.Common.LibraryMessage;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.AssignPrescriptionYHCT.AssignPrescription
{
    public partial class frmAssignPrescription : HIS.Desktop.Utility.FormBase
    {
        internal void RebuildMedicineTypeWithInControlContainer()
        {
            try
            {
                gridViewMediMaty.BeginUpdate();
                gridViewMediMaty.Columns.Clear();
                popupControlContainerMediMaty.Width = theRequiredWidth;
                popupControlContainerMediMaty.Height = theRequiredHeight;

                DevExpress.XtraGrid.Columns.GridColumn col2 = new DevExpress.XtraGrid.Columns.GridColumn();
                col2.FieldName = "MEDICINE_TYPE_CODE";
                col2.Caption = Inventec.Common.Resource.Get.Value
                    ("IVT_LANGUAGE_KEY__UC_HIS_ASSIGN_PRESCRIPTION__GV_MEDICINE__GC_MEDICINE_TYPE_CODE",
                    Resources.ResourceLanguageManager.LanguagefrmAssignPrescription,
                    Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                col2.Width = 60;
                col2.VisibleIndex = 1;
                gridViewMediMaty.Columns.Add(col2);

                DevExpress.XtraGrid.Columns.GridColumn col1 = new DevExpress.XtraGrid.Columns.GridColumn();
                col1.FieldName = "MEDICINE_TYPE_NAME";
                col1.Caption = Inventec.Common.Resource.Get.Value
                    ("IVT_LANGUAGE_KEY__UC_HIS_ASSIGN_PRESCRIPTION__GV_MEDICINE__GC_MEDICINE_TYPE_NAME",
                    Resources.ResourceLanguageManager.LanguagefrmAssignPrescription,
                    Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                col1.Width = 250;
                col1.VisibleIndex = 2;
                gridViewMediMaty.Columns.Add(col1);

                DevExpress.XtraGrid.Columns.GridColumn col7 = new DevExpress.XtraGrid.Columns.GridColumn();
                col7.FieldName = "SERVICE_UNIT_NAME";
                col7.Caption = Inventec.Common.Resource.Get.Value
                    ("IVT_LANGUAGE_KEY__UC_HIS_ASSIGN_PRESCRIPTION__GC_SERVICE_UNIT_NAME",
                    Resources.ResourceLanguageManager.LanguagefrmAssignPrescription,
                    Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                col7.Width = 60;
                col7.VisibleIndex = 3;
                gridViewMediMaty.Columns.Add(col7);

                DevExpress.XtraGrid.Columns.GridColumn col4 = new DevExpress.XtraGrid.Columns.GridColumn();
                col4.FieldName = "ACTIVE_INGR_BHYT_NAME";
                col4.Caption = Inventec.Common.Resource.Get.Value
                    ("IVT_LANGUAGE_KEY__UC_HIS_ASSIGN_PRESCRIPTION__GC_ACTIVE_INGR_BHYT_NAME",
                    Resources.ResourceLanguageManager.LanguagefrmAssignPrescription,
                    Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                col4.Width = 160;
                col4.VisibleIndex = 5;
                gridViewMediMaty.Columns.Add(col4);

                DevExpress.XtraGrid.Columns.GridColumn col5 = new DevExpress.XtraGrid.Columns.GridColumn();
                col5.FieldName = "CONCENTRA";
                col5.Caption = Inventec.Common.Resource.Get.Value
                    ("IVT_LANGUAGE_KEY__UC_HIS_ASSIGN_PRESCRIPTION__GC_CONCENTRA",
                    Resources.ResourceLanguageManager.LanguagefrmAssignPrescription,
                    Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                col5.Width = 100;
                col5.VisibleIndex = 6;
                gridViewMediMaty.Columns.Add(col5);

                DevExpress.XtraGrid.Columns.GridColumn col20 = new DevExpress.XtraGrid.Columns.GridColumn();
                col20.FieldName = "PARENT_NAME";
                col20.Caption = Inventec.Common.Resource.Get.Value
                    ("IVT_LANGUAGE_KEY__UC_HIS_ASSIGN_PRESCRIPTION__GC_PARENT_NAME",
                    Resources.ResourceLanguageManager.LanguagefrmAssignPrescription,
                    Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                col20.Width = 100;
                col20.VisibleIndex = 7;
                gridViewMediMaty.Columns.Add(col20);

                DevExpress.XtraGrid.Columns.GridColumn col6 = new DevExpress.XtraGrid.Columns.GridColumn();
                col6.FieldName = "MEDI_STOCK_NAME";
                col6.Caption = Inventec.Common.Resource.Get.Value
                    ("IVT_LANGUAGE_KEY__UC_HIS_ASSIGN_PRESCRIPTION__GC_MEDI_STOCK",
                    Resources.ResourceLanguageManager.LanguagefrmAssignPrescription,
                    Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                col6.Width = 100;
                col6.VisibleIndex = 8;
                gridViewMediMaty.Columns.Add(col6);

                DevExpress.XtraGrid.Columns.GridColumn col8 = new DevExpress.XtraGrid.Columns.GridColumn();
                col8.FieldName = "MANUFACTURER_NAME";
                col8.Caption = Inventec.Common.Resource.Get.Value
                    ("IVT_LANGUAGE_KEY__UC_HIS_ASSIGN_PRESCRIPTION__GC_MANUFACTURER_NAME",
                    Resources.ResourceLanguageManager.LanguagefrmAssignPrescription,
                    Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                col8.Width = 150;
                col8.VisibleIndex = 9;
                gridViewMediMaty.Columns.Add(col8);

                DevExpress.XtraGrid.Columns.GridColumn col9 = new DevExpress.XtraGrid.Columns.GridColumn();
                col9.FieldName = "NATIONAL_NAME";
                col9.Caption = Inventec.Common.Resource.Get.Value
                    ("IVT_LANGUAGE_KEY__UC_HIS_ASSIGN_PRESCRIPTION__GC_NATIONAL_NAME",
                    Resources.ResourceLanguageManager.LanguagefrmAssignPrescription,
                    Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                col9.Width = 80;
                col9.VisibleIndex = 10;
                gridViewMediMaty.Columns.Add(col9);

                List<MedicineMaterialTypeComboADO> mediMateTypeComboADOs = BackendDataWorker.Get<MedicineMaterialTypeComboADO>(false, true, false, false);

                //Tại màn hình kê đơn, nếu phòng mà người dùng đang làm việc có "Giới hạn thuốc được phép sử dụng" (IS_RESTRICT_MEDICINE_TYPE trong HIS_ROOM bằng true) thì danh sách thuốc khi kê thuốc trong kho chỉ hiển thị các thuốc được khai cấu hình tương ứng với phòng đấy (dữ liệu lưu trong bảng HIS_MEDICINE_TYPE_ROOM)
                long roomId = GetRoomId();
                V_HIS_ROOM room = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == roomId);
                if (room != null && (room.IS_RESTRICT_MEDICINE_TYPE ?? 0) == 1)
                {
                    List<HIS_MEDICINE_TYPE_ROOM> medicineTypeRooms = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_MEDICINE_TYPE_ROOM>();
                    medicineTypeRooms = medicineTypeRooms.Where(o => o.ROOM_ID == roomId).ToList();
                    List<long> medicineTypeIdRooms = medicineTypeRooms != null ? medicineTypeRooms.Select(o => o.MEDICINE_TYPE_ID).ToList() : new List<long>();
                    mediMateTypeComboADOs = mediMateTypeComboADOs.Where(o => medicineTypeIdRooms.Contains(o.ID)
                        || o.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__VT).ToList();
                }

                long isOnlyDisplayMediMateIsBusiness = Inventec.Common.TypeConvert.Parse.ToInt64(HisConfigs.Get<string>(HisConfigCFG.ONLY_DISPLAY_MEDIMATE_IS_BUSINESS));
                if (isOnlyDisplayMediMateIsBusiness == 1 && mediMateTypeComboADOs != null && mediMateTypeComboADOs.Count > 0)
                    mediMateTypeComboADOs = mediMateTypeComboADOs.Where(o => o.IS_BUSINESS.HasValue && o.IS_BUSINESS.Value == 1).ToList();
                mediMateTypeComboADOs = mediMateTypeComboADOs != null ? mediMateTypeComboADOs.Where(o => o.MEDICINE_LINE_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_LINE.ID__VT_YHCT).ToList() : null;
                gridViewMediMaty.GridControl.DataSource = mediMateTypeComboADOs;
                gridViewMediMaty.EndUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        internal void MedicineType_RowClick(object data)
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
                    this.cboMedicineUseForm.EditValue = null;
                    this.spinAmount.Text = "";

                    Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(this.dxValidProviderBoXung, this.dxErrorProvider1);
                    Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(this.dxValidProviderBoXung__DuongDung, this.dxErrorProvider1);

                    this.txtMediMatyForPrescription.Text = this.currentMedicineTypeADOForEdit.MEDICINE_TYPE_NAME;
                    this.FillDataIntoMedicineUseForm(this.currentMedicineTypeADOForEdit.ID);
                    this.spinAmount.Focus();
                    this.spinAmount.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

    }
}

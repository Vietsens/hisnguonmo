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
using Inventec.Core;
//using MOS.Filter;
using System;
using DCV.APP.Report.CheckValid;
using His.UC.CreateReport.Design.CreateReport;
using His.UC.CreateReport.Design.CreateReport.CheckValid;

namespace DCV.APP.Report.CheckValid
{
    class CheckValidBehaviorFactory
    {
        internal static ICheckValid MakeIGetListV(CommonParam param, object dataItem)
        {
            ICheckValid result = null;
            try
            {
                if (dataItem is HIS.UC.FormType.DateMonthYear.UCDateMonthYear)
                {
                    result = new CheckValidDateMonthYearBehavior(param, (HIS.UC.FormType.DateMonthYear.UCDateMonthYear)dataItem);
                }
                if (dataItem is HIS.UC.FormType.FilterExtra.UCFilterExtra)
                {
                    result = new CheckValidFilterExtraBehavior(param, (HIS.UC.FormType.FilterExtra.UCFilterExtra)dataItem);
                }
                else if (dataItem is HIS.UC.FormType.Core.TrueOrFalse.UCTrueOrFalse)
                {
                    result = new CheckValidTrueOrFalseBehavior(param, (HIS.UC.FormType.Core.TrueOrFalse.UCTrueOrFalse)dataItem);
                }
                else if (dataItem is HIS.UC.FormType.Str.UCStr)
                {
                    result = new CheckValidStrBehavior(param, (HIS.UC.FormType.Str.UCStr)dataItem);
                }
                else if (dataItem is HIS.UC.FormType.HourFromTo.UCHourFromTo)
                {
                    result = new CheckValidHourFromToBehavior(param, (HIS.UC.FormType.HourFromTo.UCHourFromTo)dataItem);
                }
                else if (dataItem is HIS.UC.FormType.DepartmentCombo.UCDepartmentCombo)
                {
                    result = new CheckValidDepartmentComboBehavior(param, (HIS.UC.FormType.DepartmentCombo.UCDepartmentCombo)dataItem);
                }
                else if (dataItem is HIS.UC.FormType.MediStockSttFilterCheckBoxGroup.UCMediStockSttFilterCheckBoxGroup)
                {
                    result = new CheckValidMediStockSttFilterCheckBoxGroupBehavior(param, (HIS.UC.FormType.MediStockSttFilterCheckBoxGroup.UCMediStockSttFilterCheckBoxGroup)dataItem);
                }
                else if (dataItem is HIS.UC.FormType.MultipleRoomComboCheckFilterByDepartmentComboCheck.UCMultipleRoomComboCheckFilterByDepartmentComboCheck)
                {
                    result = new CheckValidMultipleRoomComboCheckFilterByDepartmentComboCheckBehavior(param, (HIS.UC.FormType.MultipleRoomComboCheckFilterByDepartmentComboCheck.UCMultipleRoomComboCheckFilterByDepartmentComboCheck)dataItem);
                }
                else if (dataItem is HIS.UC.FormType.F21.UCF21)
                {
                    result = new CheckValidF21Behavior(param, (HIS.UC.FormType.F21.UCF21)dataItem);
                }
                else if (dataItem is HIS.UC.FormType.MediStockComboFilterByDepartmentCombo.UCMediStockComboFilterByDepartmentCombo)
                {
                    result = new CheckValidMediStockComboFilterByDepartmentComboBehavior(param, (HIS.UC.FormType.MediStockComboFilterByDepartmentCombo.UCMediStockComboFilterByDepartmentCombo)dataItem);
                }
                else if (dataItem is HIS.UC.FormType.PatientTypeCombo.UCPatientTypeCombo)
                {
                    result = new CheckValidPatientTypeComboBehavior(param, (HIS.UC.FormType.PatientTypeCombo.UCPatientTypeCombo)dataItem);
                }
                else if (dataItem is HIS.UC.FormType.RoomCombo.UCRoomCombo)
                {
                    result = new CheckValidRoomComboBehavior(param, (HIS.UC.FormType.RoomCombo.UCRoomCombo)dataItem);
                }
                else if (dataItem is HIS.UC.FormType.ThuaThieuVienPhiRadio.UCThuaThieuVienPhiRadio)
                {
                    result = new CheckValidThuaThieuVienPhiRadioBehavior(param, (HIS.UC.FormType.ThuaThieuVienPhiRadio.UCThuaThieuVienPhiRadio)dataItem);
                }
                else if (dataItem is HIS.UC.FormType.TimeFromTo.UCTimeFromTo)
                {
                    result = new CheckValidTimeFromToBehavior(param, (HIS.UC.FormType.TimeFromTo.UCTimeFromTo)dataItem);
                }
                else if (dataItem is HIS.UC.FormType.TreatmentTypeComboCheck.UCTreatmentTypeComboCheck)
                {
                    result = new CheckValidTreatmentTypeComboCheckBehavior(param, (HIS.UC.FormType.TreatmentTypeComboCheck.UCTreatmentTypeComboCheck)dataItem);
                }
                else if (dataItem is HIS.UC.FormType.TreatmentTypeGridCheckBox.UCTreatmentTypeGridCheckBox)
                {
                    result = new CheckValidTreatmentTypeGridCheckBoxBehavior(param, (HIS.UC.FormType.TreatmentTypeGridCheckBox.UCTreatmentTypeGridCheckBox)dataItem);
                }
                else if (dataItem is HIS.UC.FormType.DateTime.UCDateTime)
                {
                    result = new CheckValidDateTimeBehavior(param, (HIS.UC.FormType.DateTime.UCDateTime)dataItem);
                }
                else if (dataItem is HIS.UC.FormType.Mounth.UCMounth)
                {
                    result = new CheckValidMounthBehavior(param, (HIS.UC.FormType.Mounth.UCMounth)dataItem);
                }
                else if (dataItem is HIS.UC.FormType.AccountBookCombo.UCAccountBookCombo)
                {
                    result = new CheckValidAccountBookComboBehavior(param, (HIS.UC.FormType.AccountBookCombo.UCAccountBookCombo)dataItem);
                }
                else if (dataItem is HIS.UC.FormType.TreatmentTypeCombo.UCTreatmentTypeCombo)
                {
                    result = new CheckValidTreatmentTypeComboBehavior(param, (HIS.UC.FormType.TreatmentTypeCombo.UCTreatmentTypeCombo)dataItem);
                }

                else if (dataItem is HIS.UC.FormType.MultiMedicineGridCheckFilterByMediStockPeriodByMediStockByDepartment.UCMultiMedicineGridCheckFilterByMediStockPeriodByMediStockByDepartment)
                {
                    result = new CheckValidMultiMedicineGridCheckFilterByMediStockPeriodByMediStockByDepartmentBehavior(param, (HIS.UC.FormType.MultiMedicineGridCheckFilterByMediStockPeriodByMediStockByDepartment.UCMultiMedicineGridCheckFilterByMediStockPeriodByMediStockByDepartment)dataItem);
                }

                else if (dataItem is HIS.UC.FormType.UserNameCombo.UCUserNameCombo)
                {
                    result = new CheckValidUserNameComboBehavior(param, (HIS.UC.FormType.UserNameCombo.UCUserNameCombo)dataItem);
                }
                else if (dataItem is HIS.UC.FormType.MaterialTypeCombo.UCMaterialTypeCombo)
                {
                    result = new CheckValidMaterialTypeComboBehavior(param, (HIS.UC.FormType.MaterialTypeCombo.UCMaterialTypeCombo)dataItem);
                }
                else if (dataItem is HIS.UC.FormType.MediStockPereiodByMediStock.UCMediStockPereiodByMediStock)
                {
                    result = new CheckValidMediStockPereiodByMediStockBehavior(param, (HIS.UC.FormType.MediStockPereiodByMediStock.UCMediStockPereiodByMediStock)dataItem);
                }
                else if (dataItem is HIS.UC.FormType.Medicin.UCMedicin)
                {
                    result = new CheckValidMedicinBehavior(param, (HIS.UC.FormType.Medicin.UCMedicin)dataItem);
                }
                else if (dataItem is HIS.UC.FormType.Numericft.UCNumeric)
                {
                    result = new CheckValidNumericBehavior(param, (HIS.UC.FormType.Numericft.UCNumeric)dataItem);
                }
                else if (dataItem is HIS.UC.FormType.Core.Checkbox.UCCheck)
                {
                    result = new CheckValidCheckBehavior(param, (HIS.UC.FormType.Core.Checkbox.UCCheck)dataItem);
                }
                else if (dataItem is HIS.UC.FormType.Core.ComboBoxAtComboBox_F4__.UCComboBoxAtComboBox)
                {
                    result = new CheckValidComboBoxAtComboBoxBehavior(param, (HIS.UC.FormType.Core.ComboBoxAtComboBox_F4__.UCComboBoxAtComboBox)dataItem);
                }
                else if (dataItem is HIS.UC.FormType.Core.HeinTreatmentTypeRadio.UCHeinTreatmentTypeRadio)
                {
                    result = new His.UC.CreateReport.Design.CreateReport.CheckValid.CheckValidHeinTreatmentTypeRadioBehavior(param, (HIS.UC.FormType.Core.HeinTreatmentTypeRadio.UCHeinTreatmentTypeRadio)dataItem);
                }
                else if (dataItem is HIS.UC.FormType.Core.ServiceGroupCombo_F32__.UCServiceGroupCombo)
                {
                    result = new DCV.APP.Report.CheckValid.CheckValidServiceGroupComboBehavior(param, (HIS.UC.FormType.Core.ServiceGroupCombo_F32__.UCServiceGroupCombo)dataItem);
                }
                else if (dataItem is HIS.UC.FormType.Core.F33.UCF33)
                {
                    result = new DCV.APP.Report.CheckValid.CheckValidF33Behavior(param, (HIS.UC.FormType.Core.F33.UCF33)dataItem);
                }
                else if (dataItem is HIS.UC.FormType.Core.SQLInput_F34__.UCSql)
                {
                    result = new CheckValidSqlBehavior(param, (HIS.UC.FormType.Core.SQLInput_F34__.UCSql)dataItem);
                }
                else if (dataItem is HIS.UC.FormType.Core.RoleUserGrid_F36__.UCRoleUser)
                {
                    result = new CheckValidRoleUserBehavior(param, (HIS.UC.FormType.Core.RoleUserGrid_F36__.UCRoleUser)dataItem);
                }
                else if (dataItem is HIS.UC.FormType.Core.F37.UCF37)
                {
                    result = new DCV.APP.Report.CheckValid.CheckValidF37Behavior(param, (HIS.UC.FormType.Core.F37.UCF37)dataItem);
                }
                else if (dataItem is HIS.UC.FormType.Core.F38.UCF38)
                {
                    result = new DCV.APP.Report.CheckValid.CheckValidF38Behavior(param, (HIS.UC.FormType.Core.F38.UCF38)dataItem);
                }
                else if (dataItem is HIS.UC.FormType.F20.UCF20)
                {
                    result = new DCV.APP.Report.CheckValid.CheckValidF20Behavior(param, (HIS.UC.FormType.F20.UCF20)dataItem);
                }

                if (result == null) throw new NullReferenceException();
            }
            catch (NullReferenceException ex)
            {
                Inventec.Common.Logging.LogSystem.Error("Factory khong khoi tao duoc doi tuong." + dataItem.GetType().ToString() + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => dataItem), dataItem), ex);
                result = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = null;
            }
            return result;
        }
    }
}

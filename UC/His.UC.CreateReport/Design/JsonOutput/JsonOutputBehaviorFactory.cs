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
using DCV.APP.Report.JsonOutput;
using His.UC.CreateReport.Design.CreateReport.JsonOutput;


namespace DCV.APP.Report.JsonOutput
{
    class JsonOutputBehaviorFactory
    {
        internal static IJsonOutput MakeIGetListV(CommonParam param, object dataItem)
        {
            IJsonOutput result = null;
            try
            {
                if (dataItem is HIS.UC.FormType.DateMonthYear.UCDateMonthYear)
                {
                    result = new JsonOutputDateMonthYearBehavior(param, (HIS.UC.FormType.DateMonthYear.UCDateMonthYear)dataItem);
                }
                else if (dataItem is HIS.UC.FormType.FilterExtra.UCFilterExtra)
                {
                    result = new JsonOutputFilterExtraBehavior(param, (HIS.UC.FormType.FilterExtra.UCFilterExtra)dataItem);
                }
                else if (dataItem is HIS.UC.FormType.Core.TrueOrFalse.UCTrueOrFalse)
                {
                    result = new JsonOutputTrueOrFalseBehavior(param, (HIS.UC.FormType.Core.TrueOrFalse.UCTrueOrFalse)dataItem);
                }
                else if (dataItem is HIS.UC.FormType.Str.UCStr)
                {
                    result = new JsonOutputStrBehavior(param, (HIS.UC.FormType.Str.UCStr)dataItem);
                }
                else if (dataItem is HIS.UC.FormType.HourFromTo.UCHourFromTo)
                {
                    result = new JsonOutputHourFromToBehavior(param, (HIS.UC.FormType.HourFromTo.UCHourFromTo)dataItem);
                }
                else if (dataItem is HIS.UC.FormType.DepartmentCombo.UCDepartmentCombo)
                {
                    result = new JsonOutputDepartmentComboBehavior(param, (HIS.UC.FormType.DepartmentCombo.UCDepartmentCombo)dataItem);
                }
                else if (dataItem is HIS.UC.FormType.MediStockSttFilterCheckBoxGroup.UCMediStockSttFilterCheckBoxGroup)
                {
                    result = new JsonOutputMediStockSttFilterCheckBoxGroupBehavior(param, (HIS.UC.FormType.MediStockSttFilterCheckBoxGroup.UCMediStockSttFilterCheckBoxGroup)dataItem);
                }
                else if (dataItem is HIS.UC.FormType.MultipleRoomComboCheckFilterByDepartmentComboCheck.UCMultipleRoomComboCheckFilterByDepartmentComboCheck)
                {
                    result = new JsonOutputMultipleRoomComboCheckFilterByDepartmentComboCheckBehavior(param, (HIS.UC.FormType.MultipleRoomComboCheckFilterByDepartmentComboCheck.UCMultipleRoomComboCheckFilterByDepartmentComboCheck)dataItem);
                }
                else if (dataItem is HIS.UC.FormType.F21.UCF21)
                {
                    result = new JsonOutputF21Behavior(param, (HIS.UC.FormType.F21.UCF21)dataItem);
                }
                else if (dataItem is HIS.UC.FormType.PatientTypeCombo.UCPatientTypeCombo)
                {
                    result = new JsonOutputPatientTypeComboBehavior(param, (HIS.UC.FormType.PatientTypeCombo.UCPatientTypeCombo)dataItem);
                }
                else if (dataItem is HIS.UC.FormType.RoomCombo.UCRoomCombo)
                {
                    result = new JsonOutputRoomComboBehavior(param, (HIS.UC.FormType.RoomCombo.UCRoomCombo)dataItem);
                }
                else if (dataItem is HIS.UC.FormType.ThuaThieuVienPhiRadio.UCThuaThieuVienPhiRadio)
                {
                    result = new JsonOutputThuaThieuVienPhiRadioBehavior(param, (HIS.UC.FormType.ThuaThieuVienPhiRadio.UCThuaThieuVienPhiRadio)dataItem);
                }
                else if (dataItem is HIS.UC.FormType.TimeFromTo.UCTimeFromTo)
                {
                    result = new JsonOutputTimeFromToBehavior(param, (HIS.UC.FormType.TimeFromTo.UCTimeFromTo)dataItem);
                }
                else if (dataItem is HIS.UC.FormType.TreatmentTypeComboCheck.UCTreatmentTypeComboCheck)
                {
                    result = new JsonOutputTreatmentTypeComboCheckBehavior(param, (HIS.UC.FormType.TreatmentTypeComboCheck.UCTreatmentTypeComboCheck)dataItem);
                }
                else if (dataItem is HIS.UC.FormType.TreatmentTypeGridCheckBox.UCTreatmentTypeGridCheckBox)
                {
                    result = new JsonOutputTreatmentTypeGridCheckBoxBehavior(param, (HIS.UC.FormType.TreatmentTypeGridCheckBox.UCTreatmentTypeGridCheckBox)dataItem);
                }
                else if (dataItem is HIS.UC.FormType.DateTime.UCDateTime)
                {
                    result = new JsonOutputDateTimeBehavior(param, (HIS.UC.FormType.DateTime.UCDateTime)dataItem);
                }
                else if (dataItem is HIS.UC.FormType.Mounth.UCMounth)
                {
                    result = new JsonOutputMounthBehavior(param, (HIS.UC.FormType.Mounth.UCMounth)dataItem);
                }
                else if (dataItem is HIS.UC.FormType.AccountBookCombo.UCAccountBookCombo)
                {
                    result = new JsonOutputAccountBookComboBehavior(param, (HIS.UC.FormType.AccountBookCombo.UCAccountBookCombo)dataItem);
                }
                else if (dataItem is HIS.UC.FormType.TreatmentTypeCombo.UCTreatmentTypeCombo)
                {
                    result = new JsonOutputTreatmentTypeComboBehavior(param, (HIS.UC.FormType.TreatmentTypeCombo.UCTreatmentTypeCombo)dataItem);
                }
                else if (dataItem is HIS.UC.FormType.Medicin.UCMedicin)
                {
                    result = new JsonOutputMedicinBehavior(param, (HIS.UC.FormType.Medicin.UCMedicin)dataItem);
                }
                else if (dataItem is HIS.UC.FormType.MediStockComboFilterByDepartmentCombo.UCMediStockComboFilterByDepartmentCombo)
                {
                    result = new JsonOutputMediStockComboFilterByDepartmentComboBehavior(param, (HIS.UC.FormType.MediStockComboFilterByDepartmentCombo.UCMediStockComboFilterByDepartmentCombo)dataItem);
                }
                else if (dataItem is HIS.UC.FormType.MultiMedicineGridCheckFilterByMediStockPeriodByMediStockByDepartment.UCMultiMedicineGridCheckFilterByMediStockPeriodByMediStockByDepartment)
                {
                    result = new JsonOutputMultiMedicineGridCheckFilterByMediStockPeriodByMediStockByDepartmentBehavior(param, (HIS.UC.FormType.MultiMedicineGridCheckFilterByMediStockPeriodByMediStockByDepartment.UCMultiMedicineGridCheckFilterByMediStockPeriodByMediStockByDepartment)dataItem);
                }
                else if (dataItem is HIS.UC.FormType.UserNameCombo.UCUserNameCombo)
                {
                    result = new JsonOutputUserNameComboBehavior(param, (HIS.UC.FormType.UserNameCombo.UCUserNameCombo)dataItem);
                }
                else if (dataItem is HIS.UC.FormType.MaterialTypeCombo.UCMaterialTypeCombo)
                {
                    result = new JsonOutputMaterialTypeComboBehavior(param, (HIS.UC.FormType.MaterialTypeCombo.UCMaterialTypeCombo)dataItem);
                }
                else if (dataItem is HIS.UC.FormType.MediStockPereiodByMediStock.UCMediStockPereiodByMediStock)
                {
                    result = new JsonOutputMediStockPereiodByMediStockBehavior(param, (HIS.UC.FormType.MediStockPereiodByMediStock.UCMediStockPereiodByMediStock)dataItem);
                }
                else if (dataItem is HIS.UC.FormType.Core.Checkbox.UCCheck)
                {
                    result = new JsonOutputCheckBehavior(param, (HIS.UC.FormType.Core.Checkbox.UCCheck)dataItem);
                }
                else if (dataItem is HIS.UC.FormType.Numericft.UCNumeric)
                {
                    result = new JsonOutputNumericBehavior(param, (HIS.UC.FormType.Numericft.UCNumeric)dataItem);
                }
                else if (dataItem is HIS.UC.FormType.Core.ComboBoxAtComboBox_F4__.UCComboBoxAtComboBox)
                {
                    result = new JsonOutputComboBoxAtComboBox_F4__Behavior(param, (HIS.UC.FormType.Core.ComboBoxAtComboBox_F4__.UCComboBoxAtComboBox)dataItem);
                }
                else if (dataItem is HIS.UC.FormType.Core.HeinTreatmentTypeRadio.UCHeinTreatmentTypeRadio)
                {
                    result = new JsonOutputHeinTreatmentTypeRadioBehavior(param, (HIS.UC.FormType.Core.HeinTreatmentTypeRadio.UCHeinTreatmentTypeRadio)dataItem);
                }
                else if (dataItem is HIS.UC.FormType.Core.ServiceGroupCombo_F32__.UCServiceGroupCombo)
                {
                    result = new JsonOutputServiceGroupComboBehavior(param, (HIS.UC.FormType.Core.ServiceGroupCombo_F32__.UCServiceGroupCombo)dataItem);
                }
                else if (dataItem is HIS.UC.FormType.Core.SQLInput_F34__.UCSql)
                {
                    result = new JsonOutputSqlBehavior(param, (HIS.UC.FormType.Core.SQLInput_F34__.UCSql)dataItem);
                }
                else if (dataItem is HIS.UC.FormType.Core.F33.UCF33)
                {
                    result = new JsonOutputF33Behavior(param, (HIS.UC.FormType.Core.F33.UCF33)dataItem);
                }
                else if (dataItem is HIS.UC.FormType.Core.RoleUserGrid_F36__.UCRoleUser)
                {
                    result = new JsonOutputRoleUserBehavior(param, (HIS.UC.FormType.Core.RoleUserGrid_F36__.UCRoleUser)dataItem);
                }
                else if (dataItem is HIS.UC.FormType.Core.F37.UCF37)
                {
                    result = new JsonOutputF37Behavior(param, (HIS.UC.FormType.Core.F37.UCF37)dataItem);
                }
                else if (dataItem is HIS.UC.FormType.Core.F38.UCF38)
                {
                    result = new JsonOutputF38Behavior(param, (HIS.UC.FormType.Core.F38.UCF38)dataItem);
                }
                else if (dataItem is HIS.UC.FormType.F20.UCF20)
                {
                    result = new JsonOutputF20Behavior(param, (HIS.UC.FormType.F20.UCF20)dataItem);
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

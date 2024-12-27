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
using SAR.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.UC.FormType
{
    public class ProcessorFactory
    {
        internal static IProcessorGenerate MakeProcessorBase(V_SAR_RETY_FOFI data, object generateRDO)
        {
            IProcessorGenerate result = null;
            try
            {
                try
                {
                    Base.ConfigSystemProcess.InitConfigSystem();
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Error(ex);
                }
                SAR_FORM_FIELD config = null;
                try
                {
                    config = FormTypeConfig.FormFields.Where(o => o.FORM_FIELD_CODE == data.FORM_FIELD_CODE).SingleOrDefault();
                    if (config == null) throw new Exception("Khong tim duoc config.");
                }
                catch (Exception ex)
                {
                    throw new Exception("Khong truy van duoc du lieu cau hinh in an theo cac thong tin truyen vao. Kiem tra lai frontend & SAR_FORM_FIELD. Khong the khoi tao truong du lieu man hinh tao bao cao." + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => data), data), ex);
                }

                try
                {
                    //Tat ca cac loai giao dien tao bao cao can phai khai bao o day
                    //Moi khi them mot giao dien hop den du lieu bao cao moi thi bo xung them 1 dong
                    switch (config.FORM_FIELD_CODE)
                    {
                        case "FTHIS000001":
                            result = new HIS.UC.FormType.TimeFromTo.TimeFromToProcessor(data, generateRDO);
                            break;
                        case "FTHIS000002":
                            result = new HIS.UC.FormType.MediStockComboFilterByDepartmentCombo.MediStockComboFilterByDepartmentComboProcessor(data, generateRDO);
                            break;
                        case "FTHIS000003":
                            result = new HIS.UC.FormType.RoomCombo.RoomComboProcessor(data, generateRDO);
                            break;
                        case "FTHIS000004":
                            result = new HIS.UC.FormType.ComboBoxAtComboBox.ComboBoxAtComboBoxProcessor(data, generateRDO);
                            break;
                        case "FTHIS000005":
                            result = new HIS.UC.FormType.HourFromTo.HourFromToProcessor(data, generateRDO);
                            break;
                        case "FTHIS000006":
                            result = new HIS.UC.FormType.Str.StrProcessor(data, generateRDO);
                            break;
                        case "FTHIS000007":
                            result = new HIS.UC.FormType.Core.TrueOrFalse.TrueOrFalseProcessor(data, generateRDO);
                            break;
                        case "FTHIS000008":
                            result = new HIS.UC.FormType.MultipleRoomComboCheckFilterByDepartmentComboCheck.MultipleRoomComboCheckFilterByDepartmentComboCheckProcessor(data, generateRDO);
                            break;
                        case "FTHIS000009":
                            result = new HIS.UC.FormType.DepartmentCombo.DepartmentComboProcessor(data, generateRDO);
                            break;
                        case "FTHIS000010":
                            result = new HIS.UC.FormType.DateMonthYear.DateMonthYearProcessor(data, generateRDO);
                            break;
                        case "FTHIS000011":
                            result = new HIS.UC.FormType.FilterExtra.FilterExtraProcessor(data, generateRDO);
                            break;
                        case "FTHIS000012":
                            result = new HIS.UC.FormType.MediStockSttFilterCheckBoxGroup.MediStockSttFilterCheckBoxGroupProcessor(data, generateRDO);
                            break;
                        case "FTHIS000013":
                            result = new HIS.UC.FormType.PatientTypeCombo.PatientTypeComboProcessor(data, generateRDO);
                            break;
                        case "FTHIS000014":
                            result = new HIS.UC.FormType.ThuaThieuVienPhiRadio.ThuaThieuVienPhiRadioProcessor(data, generateRDO);
                            break;
                        case "FTHIS000015":
                            result = new HIS.UC.FormType.TreatmentTypeComboCheck.TreatmentTypeComboCheckProcessor(data, generateRDO);
                            break;
                        case "FTHIS000016":
                            result = new HIS.UC.FormType.TreatmentTypeGridCheckBox.TreatmentTypeGridCheckBoxProcessor(data, generateRDO);
                            break;
                        case "FTHIS000017":
                            result = new HIS.UC.FormType.Mounth.MounthProcessor(data, generateRDO);
                            break;
                        case "FTHIS000018":
                            result = new HIS.UC.FormType.DateTime.DateTimeProcessor(data, generateRDO);
                            break;
                        case "FTHIS000019":
                            result = new HIS.UC.FormType.Core.Checkbox.CheckboxProcessor(data, generateRDO);
                            break;
                        case "FTHIS000020":
                            result = new HIS.UC.FormType.F20.F20Processor(data, generateRDO);
                            break;
                        case "FTHIS000021":
                            result = new HIS.UC.FormType.F21.F21Processor(data, generateRDO);
                            break;
                        case "FTHIS000022":
                            result = new HIS.UC.FormType.TreatmentTypeCombo.TreatmentTypeComboProcessor(data, generateRDO);
                            break;
                        case "FTHIS000023":
                            result = new HIS.UC.FormType.AccountBookCombo.AccountBookComboProcessor(data, generateRDO);
                            break;
                        case "FTHIS000024":
                            result = new HIS.UC.FormType.ExpMestTypeComboCheck.ExpMestTypeComboCheckProcessor(data, generateRDO);
                            break;
                        case "FTHIS000025":
                            result = new HIS.UC.FormType.MultiMedicineGridCheckFilterByMediStockPeriodByMediStockByDepartment.MultiMedicineGridCheckFilterByMediStockPeriodByMediStockByDepartmentProcessor(data, generateRDO);
                            break;
                        case "FTHIS000026":
                            result = new HIS.UC.FormType.UserNameCombo.UserNameComboProcessor(data, generateRDO);
                            break;
                        case "FTHIS000027":
                            result = new HIS.UC.FormType.MaterialTypeCombo.MaterialTypeComboProcessor(data, generateRDO);
                            break;
                        case "FTHIS000028":
                            result = new HIS.UC.FormType.MediStockPereiodByMediStock.MediStockPereiodByMediStockProcessor(data, generateRDO);
                            break;
                        case "FTHIS000029":
                            result = new HIS.UC.FormType.Medicin.MedicinProcessor(data, generateRDO);
                            break;
                        case "FTHIS000030":
                            result = new HIS.UC.FormType.Numeric.NumericProcessor(data, generateRDO);
                            break;
                        case "FTHIS000031":
                            result = new HIS.UC.FormType.Core.HeinTreatmentTypeRadio.HeinTreatmentTypeRadioProcessor(data, generateRDO);
                            break;
                        case "FTHIS000032":
                            result = new HIS.UC.FormType.Core.ServiceGroupCombo_F32__.ServiceGroupComboProcessor(data, generateRDO);
                            break;
                        case "FTHIS000033":
                            result = new HIS.UC.FormType.F33.F33Processor(data, generateRDO);
                            break;
                        case "FTHIS000034":
                            result = new HIS.UC.FormType.Core.SQLInput_F34__.SQLInputProcessor(data, generateRDO);
                            break;
                        case "FTHIS000035":
                            result = new HIS.UC.FormType.TimeFromTo.TimeFromToProcessor(data, generateRDO);
                            break;
                        case "FTHIS000036":
                            result = new HIS.UC.FormType.Core.RoleUserGrid_F36__.RoleUserProcessor(data, generateRDO);
                            break;
                        case "FTHIS000037":
                            result = new HIS.UC.FormType.F37.F37Processor(data, generateRDO);
                            break;
                        case "FTHIS000038":
                            result = new HIS.UC.FormType.F38.F38Processor(data, generateRDO);
                            break;
                        default:
                            break;
                    }
                }
                catch (Exception)
                {
                    throw new NullReferenceException();
                }

            }
            catch (NullReferenceException ex)
            {
                Inventec.Common.Logging.LogSystem.Error("Factory khong khoi tao duoc doi tuong." + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => data), data), ex);
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

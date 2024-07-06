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
using HIS.Desktop.Plugins.AssignPrescriptionPK.AssignPrescription;
using HIS.Desktop.Plugins.AssignPrescriptionPK.Edit.MaterialType;
using HIS.Desktop.Plugins.AssignPrescriptionPK.Edit.MaterialTypeTSD;
using HIS.Desktop.Plugins.AssignPrescriptionPK.Edit.MedicineType;
using HIS.Desktop.Plugins.AssignPrescriptionPK.Edit.MedicineTypeOther;
using HIS.Desktop.Plugins.AssignPrescriptionPK.Edit.MediStockD1SDO;
using Inventec.Core;
using System;

namespace HIS.Desktop.Plugins.AssignPrescriptionPK.Edit
{
    public class EditFactory
    {
        internal static IEdit MakeIEdit(CommonParam param,
            frmAssignPrescription frmAssignPrescription,
            ValidAddRow validAddRow,
            HIS.Desktop.Plugins.AssignPrescriptionPK.MediMatyCreateWorker.ChoosePatientTypeDefaultlService choosePatientTypeDefaultlService,
            HIS.Desktop.Plugins.AssignPrescriptionPK.MediMatyCreateWorker.ChoosePatientTypeDefaultlServiceOther choosePatientTypeDefaultlServiceOther,
            CalulateUseTimeTo calulateUseTimeTo,
            ExistsAssianInDay existsAssianInDay,
            object dataRow,
            int datatype)
        {
            IEdit result = null;
            try
            {
                switch (datatype)
                {
                    case HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC:
                        result = new MediStockD1SDORowEditBehavior(param, frmAssignPrescription, validAddRow, choosePatientTypeDefaultlService, choosePatientTypeDefaultlServiceOther, calulateUseTimeTo, existsAssianInDay, dataRow);
                        break;
                    case HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.VATTU:
                        result = new MediStockD1SDORowEditBehavior(param, frmAssignPrescription, validAddRow, choosePatientTypeDefaultlService, choosePatientTypeDefaultlServiceOther, calulateUseTimeTo, existsAssianInDay, dataRow);
                        break;
                    case HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC_DM:
                        result = new MedicineTypeRowEditBehavior(param, frmAssignPrescription, validAddRow, choosePatientTypeDefaultlService, choosePatientTypeDefaultlServiceOther, calulateUseTimeTo, existsAssianInDay, dataRow);
                        break;
                    case HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.VATTU_DM:
                        result = new MaterialTypeRowEditBehavior(param, frmAssignPrescription, validAddRow, choosePatientTypeDefaultlService, choosePatientTypeDefaultlServiceOther, calulateUseTimeTo, existsAssianInDay, dataRow);
                        break;
                    case HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC_TUTUC:
                        result = new MedicineTypeOtherRowEditBehavior(param, frmAssignPrescription, validAddRow, choosePatientTypeDefaultlService, choosePatientTypeDefaultlServiceOther, calulateUseTimeTo, existsAssianInDay, dataRow);
                        break;
                    case HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.VATTU_TSD:
                        result = new MaterialTypeTSDRowEditBehavior(param, frmAssignPrescription, validAddRow, choosePatientTypeDefaultlService, choosePatientTypeDefaultlServiceOther, calulateUseTimeTo, existsAssianInDay, dataRow);
                        break;
                    default:
                        break;
                }

                if (result == null) throw new NullReferenceException();
            }
            catch (NullReferenceException ex)
            {
                Inventec.Common.Logging.LogSystem.Error("Factory khong khoi tao duoc doi tuong." + dataRow.GetType().ToString() + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => dataRow), dataRow)
                   + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => param), param), ex);
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
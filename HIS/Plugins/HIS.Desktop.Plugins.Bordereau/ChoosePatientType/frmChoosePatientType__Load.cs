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
using HIS.Desktop.Common;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Plugins.Bordereau.ADO;
using HIS.Desktop.Plugins.Bordereau.Config;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.Bordereau.ChoosePatientType
{
    public partial class frmChoosePatientType : Form
    {

        private void JoinTwoList(ref List<long> lstResult, ref List<long> lstTemp)
        {
            try
            {
                if (lstResult == null)
                    lstResult = new List<long>();
                if (lstTemp == null)
                    lstTemp = new List<long>();

                if (lstResult.Count == 0)
                    lstResult.AddRange(lstTemp);
                else
                {
                    lstResult = (from a in lstResult
                            join b in lstTemp on a equals b
                            select a).ToList();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadComboPatientType()
        {
            try
            {
                var currentPatientType = this.patientTypeAlters.OrderByDescending(o => o.LOG_TIME).FirstOrDefault();

                //Lấy đối tượng thanh toán theo dịch vụ
                List<V_HIS_PATIENT_TYPE_ALTER> patientTypeAlterServices = this.GetPatientTypeAlterInstructionTime(sereServADOSelecteds);
                if (patientTypeAlterServices == null || patientTypeAlterServices.Count == 0)
                    return;

                List<long> patientTypeAlterServiceIds = patientTypeAlterServices.Select(o => o.PATIENT_TYPE_ID).ToList();

                List<HIS_PATIENT_TYPE_ALLOW> patientTypeAllows = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_PATIENT_TYPE_ALLOW>(false, true)
                    .Where(o => patientTypeAlterServiceIds.Contains(o.PATIENT_TYPE_ID) && o.IS_ACTIVE == 1).ToList();

                if (patientTypeAllows == null || patientTypeAllows.Count == 0)
                    return;

                foreach (var item in patientTypeAlterServices)
                {
                    List<HIS_PATIENT_TYPE_ALLOW> patientTypeAllowTemps = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_PATIENT_TYPE_ALLOW>(false, true)
                    .Where(o => o.PATIENT_TYPE_ID == item.PATIENT_TYPE_ID && o.IS_ACTIVE == 1).ToList();
                    if (patientTypeAllowTemps == null)
                        patientTypeAllowTemps = new List<HIS_PATIENT_TYPE_ALLOW>();

                    patientTypeAllows = patientTypeAllows.Where(o => patientTypeAllowTemps.Select(p => p.PATIENT_TYPE_ID).Contains(o.PATIENT_TYPE_ID)).ToList();

                }

                List<long> patientTypeAllowMetiTemps = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_PATIENT_TYPE_ALLOW>(false, true).Where(o => o.PATIENT_TYPE_ID == currentPatientType.PATIENT_TYPE_ID && o.IS_ACTIVE == 1).Select(p => p.PATIENT_TYPE_ALLOW_ID).ToList();

                List<long> patientTypeNotBHYTIds = patientTypeAllowMetiTemps != null && patientTypeAllowMetiTemps.Count > 0 ? patientTypeAllowMetiTemps.Where(o => o != HisPatientTypeCFG.PATIENT_TYPE_ID__BHYT).ToList() : new List<long>();

                List<long> patientTypeAllowIds = patientTypeAllows != null ? patientTypeAllows.Select(o => o.PATIENT_TYPE_ALLOW_ID).ToList() : new List<long>();

                var servicePatyInBranchs = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_SERVICE_PATY>()
                    .Where(o => o.BRANCH_ID == HIS.Desktop.LocalStorage.LocalData.WorkPlace.GetBranchId()).ToList();
                if (servicePatyInBranchs == null || servicePatyInBranchs.Count == 0)
                    return;

                List<long> patientTypeIds = new List<long>();
                List<long> patientTypeMediMatyIds = new List<long>();
                List<long> patientTypeServiceIds = new List<long>();

                List<HIS_MEDICINE> MedicineList = new List<HIS_MEDICINE>();
                List<HIS_MATERIAL> MaterialList = new List<HIS_MATERIAL>();

                var medicineIdList = sereServADOSelecteds
                    .Where(o => o.MEDICINE_ID.HasValue && o.MEDICINE_ID.Value > 0)
                    .Select(p => p.MEDICINE_ID.Value).Distinct().ToList();

                var materialIdList = sereServADOSelecteds
                    .Where(o => o.MATERIAL_ID.HasValue && o.MATERIAL_ID.Value > 0)
                    .Select(p => p.MATERIAL_ID.Value).Distinct().ToList();

                if (medicineIdList != null && medicineIdList.Count > 0)
                {
                    MOS.Filter.HisMedicineFilter medicineFilter = new HisMedicineFilter();
                    medicineFilter.IDs = medicineIdList;
                    MedicineList = new BackendAdapter(new CommonParam()).Get<List<HIS_MEDICINE>>("api/HisMedicine/Get", ApiConsumer.ApiConsumers.MosConsumer, medicineFilter, null);
                }

                if (materialIdList != null && materialIdList.Count > 0)
                {
                    MOS.Filter.HisMaterialFilter materialFilter = new HisMaterialFilter();
                    materialFilter.IDs = materialIdList;
                    MaterialList = new BackendAdapter(new CommonParam()).Get<List<HIS_MATERIAL>>("api/HisMaterial/Get", ApiConsumer.ApiConsumers.MosConsumer, materialFilter, null);
                }

                foreach (var item in sereServADOSelecteds)
                {
                    if (item.MEDICINE_ID.HasValue && item.MEDICINE_ID.Value > 0
                       && MedicineList != null && MedicineList.Count > 0)
                    {
                        if (currentPatientType.PATIENT_TYPE_ID == HisPatientTypeCFG.PATIENT_TYPE_ID__BHYT)
                        {
                            var checkMedicine = MedicineList.FirstOrDefault(o => o.ID == item.MEDICINE_ID.Value);
                            var medicineType = checkMedicine != null
                                ? BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>().FirstOrDefault(o => o.ID == checkMedicine.MEDICINE_TYPE_ID)
                                : null;
                            if (medicineType != null && (!String.IsNullOrEmpty(medicineType.ACTIVE_INGR_BHYT_CODE) || !string.IsNullOrEmpty(AllowAssignOffListMedicineMaterialHeinCardNumberPrefix) && !string.IsNullOrEmpty(currentTreatment.TDL_HEIN_CARD_NUMBER) && AllowAssignOffListMedicineMaterialHeinCardNumberPrefix.Split(',').ToList().Exists(o => currentTreatment.TDL_HEIN_CARD_NUMBER.StartsWith(o)))
                                && (medicineType.HEIN_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__TH_TDM || medicineType.HEIN_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__TH_TL || medicineType.HEIN_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__TH_UT || (!string.IsNullOrEmpty(AllowAssignOffListMedicineMaterialHeinCardNumberPrefix) && !string.IsNullOrEmpty(currentTreatment.TDL_HEIN_CARD_NUMBER) && AllowAssignOffListMedicineMaterialHeinCardNumberPrefix.Split(',').ToList().Exists(o => currentTreatment.TDL_HEIN_CARD_NUMBER.StartsWith(o)) && medicineType.HEIN_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__TH_NDM && !String.IsNullOrEmpty(medicineType.ACTIVE_INGR_BHYT_CODE))))
                            {
                                JoinTwoList(ref patientTypeMediMatyIds, ref patientTypeAllowMetiTemps);
                            }
                            else
                            {
                                JoinTwoList(ref patientTypeMediMatyIds, ref patientTypeNotBHYTIds);
                            }
                        }
                        else
                        {
                            JoinTwoList(ref patientTypeMediMatyIds, ref patientTypeNotBHYTIds);
                        }
                    }
                    else if (item.MATERIAL_ID.HasValue && item.MATERIAL_ID.Value > 0
                        && MaterialList != null && MaterialList.Count > 0)
                    {
                        if (currentPatientType.PATIENT_TYPE_ID == HisPatientTypeCFG.PATIENT_TYPE_ID__BHYT)
                        {
                            var checkMaterial = MaterialList.FirstOrDefault(o => o.ID == item.MATERIAL_ID.Value);
                            var materialType = checkMaterial != null
                                ? BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>().FirstOrDefault(o => o.ID == checkMaterial.MATERIAL_TYPE_ID)
                                : null;
                            if (materialType != null && !String.IsNullOrWhiteSpace(materialType.HEIN_SERVICE_BHYT_CODE)
                                && !String.IsNullOrWhiteSpace(materialType.HEIN_SERVICE_BHYT_NAME)
                                && (materialType.HEIN_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__VT_TT || materialType.HEIN_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__VT_TDM || materialType.HEIN_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__VT_TL || (!string.IsNullOrEmpty(AllowAssignOffListMedicineMaterialHeinCardNumberPrefix) && !string.IsNullOrEmpty(currentTreatment.TDL_HEIN_CARD_NUMBER) && AllowAssignOffListMedicineMaterialHeinCardNumberPrefix.Split(',').ToList().Exists(o => currentTreatment.TDL_HEIN_CARD_NUMBER.StartsWith(o)) && materialType.HEIN_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__VT_NDM)))
                            {
                                JoinTwoList(ref patientTypeMediMatyIds, ref patientTypeAllowMetiTemps);
                            }
                            else
                            {
                                JoinTwoList(ref patientTypeMediMatyIds, ref patientTypeNotBHYTIds);
                            }
                        }
                        else
                        {
                            JoinTwoList(ref patientTypeMediMatyIds, ref patientTypeNotBHYTIds);
                        }
                    }
                    else
                    {
                        List<long> patientTypeIdTemps = servicePatyInBranchs.Where(o => item.SERVICE_ID == o.SERVICE_ID && patientTypeAllowIds.Contains(o.PATIENT_TYPE_ID)).Select(o => o.PATIENT_TYPE_ID).Distinct().ToList();

                        if (patientTypeServiceIds.Count == 0)
                            patientTypeServiceIds.AddRange(patientTypeIdTemps);
                        else
                        {
                            patientTypeServiceIds = (from a in patientTypeServiceIds
                                                     join b in patientTypeIdTemps on a equals b
                                                     select a).ToList();
                        }
                    }
                }

                if (patientTypeServiceIds != null && patientTypeServiceIds.Count > 0 && patientTypeMediMatyIds != null && patientTypeMediMatyIds.Count > 0)
                {
                    patientTypeIds = (from a in patientTypeServiceIds
                                      join b in patientTypeMediMatyIds on a equals b
                                      select a).ToList();
                }
                else if (patientTypeServiceIds != null && patientTypeServiceIds.Count > 0)
                {
                    patientTypeIds = patientTypeServiceIds;
                }
                else
                {
                    patientTypeIds = patientTypeMediMatyIds;
                }

                if (patientTypeIds != null && patientTypeIds.Count > 0)
                {
                    patientTypeIds = patientTypeIds.Distinct().ToList();
                    List<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE> dataCombo = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_PATIENT_TYPE>(false, true).Where(o => patientTypeIds.Contains(o.ID)).ToList();

                    List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                    columnInfos.Add(new ColumnInfo("PATIENT_TYPE_NAME", "", 250, 2));
                    ControlEditorADO controlEditorADO = new ControlEditorADO("PATIENT_TYPE_NAME", "ID", columnInfos, false, 250);
                    ControlEditorLoader.Load(cboPatientType, dataCombo, controlEditorADO);
                }
                else
                {
                    btnSave.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private List<V_HIS_PATIENT_TYPE_ALTER> GetPatientTypeAlterInstructionTime(List<SereServADO> sereServADOs)
        {
            List<V_HIS_PATIENT_TYPE_ALTER> result = new List<V_HIS_PATIENT_TYPE_ALTER>();
            try
            {
                foreach (var item in sereServADOs)
                {
                    V_HIS_PATIENT_TYPE_ALTER patientTypeAlter = patientTypeAlters.FirstOrDefault(o => o.LOG_TIME <= item.TDL_INTRUCTION_TIME);
                    if (patientTypeAlter != null)
                    {
                        result.Add(patientTypeAlter);
                    }
                }
            }
            catch (Exception ex)
            {
                result = null;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

    }
}

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
using HIS.Desktop.ADO;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Plugins.ExamServiceReqExecute.Base;
using HIS.Desktop.Plugins.Library.PrintTreatmentEndTypeExt;
using HIS.Desktop.Plugins.Library.PrintTreatmentFinish;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Common.ThreadCustom;
using Inventec.Common.WordContent;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MPS.Processor.Mps000401.PDO;
using System;
using System.Collections.Generic;
using System.Linq;


namespace HIS.Desktop.Plugins.ExamServiceReqExecute
{
    public partial class ExamServiceReqExecuteControl : UserControlBase
    {
        private void LoadBieuMauGiayXetNghiemViKhuanLao(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("LoadBieuMauGiayXetNghiemViKhuanLao.1");
                //List<Action> methods = new List<Action>();
                //methods.Add(LoadPatient);
                //methods.Add(LoadPatientTypeAlter);
                //ThreadCustomManager.MultipleThreadWithJoin(methods);

                if (patientTypeAlter == null)
                    patientTypeAlter = new V_HIS_PATIENT_TYPE_ALTER();

                OtherFormAssTreatmentInputADO otherFormAssTreatmentInputADO = new Desktop.ADO.OtherFormAssTreatmentInputADO();
                otherFormAssTreatmentInputADO.TreatmentId = treatment.ID;
                otherFormAssTreatmentInputADO.PrintTypeCode = PrintTypeCodeWorker.PRINT_TYPE_CODE__PHIEU_XET_NGHIEM_VI_KHUAN_LAO__MPS000436;
                Dictionary<string, object> dicParamPlus = new Dictionary<string, object>();

                //TemplateKeyProcessor.AddKeyIntoDictionaryPrint<V_HIS_PATIENT>(patient, dicParamPlus);
                //TemplateKeyProcessor.AddKeyIntoDictionaryPrint<HIS_TREATMENT>(treatment, dicParamPlus);

                var depart = BackendDataWorker.Get<HIS_DEPARTMENT>().FirstOrDefault(o => o.ID == this.serviceReq.EXECUTE_DEPARTMENT_ID);

                //TemplateKeyProcessor.SetSingleKey(dicParamPlus, "AGE", MPS.AgeUtil.CalculateFullAge(patient.DOB));
                //TemplateKeyProcessor.SetSingleKey(dicParamPlus, "TDL_PATIENT_PHONE", patient.PHONE);
                //TemplateKeyProcessor.SetSingleKey(dicParamPlus, "DISTRICT_NAME", patient.DISTRICT_NAME);
                //TemplateKeyProcessor.SetSingleKey(dicParamPlus, "PROVINCE_NAME", patient.PROVINCE_NAME);
                TemplateKeyProcessor.SetSingleKey(dicParamPlus, "EXECUTE_DEPARTMENT_NAME", depart.DEPARTMENT_NAME);
                TemplateKeyProcessor.SetSingleKey(dicParamPlus, "EXECUTE_DEPARTMENT_CODE", depart.DEPARTMENT_CODE);
                
                //TemplateKeyProcessor.AddKeyIntoDictionaryPrint<V_HIS_PATIENT_TYPE_ALTER>(patientTypeAlter, dicParamPlus);

                otherFormAssTreatmentInputADO.DicParam = dicParamPlus;
                List<object> listObj = new List<object>();
                listObj.Add(otherFormAssTreatmentInputADO);

                HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.OtherFormAssTreatment", this.moduleData != null ? this.moduleData.RoomId : 0, this.moduleData != null ? this.moduleData.RoomTypeId : 0, listObj);

                Inventec.Common.Logging.LogSystem.Debug("LoadBieuMauGiayXetNghiemViKhuanLao.2");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}

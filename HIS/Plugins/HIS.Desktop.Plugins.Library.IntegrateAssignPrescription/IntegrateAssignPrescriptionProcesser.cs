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
using Inventec.Desktop.Common;
using Inventec.Desktop.Core;
using Inventec.Desktop.Common.Modules;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HIS.Desktop.Common;
using HIS.Desktop.Plugins.Library.IntegrateAssignPrescription;
using HIS.WCF.Service.AssignPrescriptionService;
using HIS.Desktop.LocalStorage.LocalData;
using MOS.EFMODEL.DataModels;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Utility;
using HIS.WCF.DCO;
using MOS.SDO;
using HIS.Desktop.LocalStorage.BackendData;


namespace HIS.Desktop.Plugins.Library.IntegrateAssignPrescription
{
    public class IntegrateAssignPrescriptionProcesser
    {
        string IntegrateAssignPrescriptionOption;
        WcfAssignPrescriptionResultDCO assignPrescriptionResultDCO = new WcfAssignPrescriptionResultDCO();

        public IntegrateAssignPrescriptionProcesser()
        {
            IntegrateAssignPrescriptionOption = HisConfigCFG.GetValue(HisConfigCFG.CONFIG_KEY__IntegrateAssignPrescription__Option);
            AssignPrescriptionServiceManager.SetDelegate(DelegateAssignPrescription);
        }

        public bool IsHostOpened()
        {
            bool success = false;
            try
            {
                if (IntegrateAssignPrescriptionOption == "1" && AssignPrescriptionServiceManager.IsOpen())
                {
                    success = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                success = false;
            }
            return success;
        }

        public bool CloseHost()
        {
            bool success = false;
            try
            {
                Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => IntegrateAssignPrescriptionOption), IntegrateAssignPrescriptionOption));
                if (IntegrateAssignPrescriptionOption == "1" && AssignPrescriptionServiceManager.CloseHost())
                {
                    Inventec.Common.Logging.LogSystem.Info("CloseHost service IntegrateAssignPrescriptionOption success");
                    success = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                success = false;
            }
            return success;
        }

        public bool OpenHost()
        {
            bool success = false;
            try
            {
                Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => IntegrateAssignPrescriptionOption), IntegrateAssignPrescriptionOption));
                if (IntegrateAssignPrescriptionOption == "1" && AssignPrescriptionServiceManager.OpenHost())
                {
                    Inventec.Common.Logging.LogSystem.Info("OpenHost service IntegrateAssignPrescriptionOption success");
                    success = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                success = false;
            }
            return success;
        }

        WcfAssignPrescriptionResultDCO DelegateAssignPrescription(HIS.WCF.DCO.WcfAssignPrescriptionDCO assignPrescriptionDCO)
        {
            this.assignPrescriptionResultDCO = new WcfAssignPrescriptionResultDCO();
            try
            {
                Inventec.Common.Logging.LogSystem.Info("DelegateAssignPrescription.1____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => assignPrescriptionDCO), assignPrescriptionDCO));
                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.AssignPrescriptionPK").FirstOrDefault();
                if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.AssignPrescriptionPK");
                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                {
                    List<object> listArgs = new List<object>();
                    HIS.Desktop.ADO.AssignPrescriptionADO assignServiceADO = new HIS.Desktop.ADO.AssignPrescriptionADO(0, 0, 0);

                    CommonParam paramCommon = new CommonParam();
                    MOS.Filter.HisTreatmentFilter treatFilter = new MOS.Filter.HisTreatmentFilter();
                    treatFilter.TREATMENT_CODE__EXACT = assignPrescriptionDCO.TreatmentCode;
                    List<HIS_TREATMENT> lstTreatments = new Inventec.Common.Adapter.BackendAdapter(paramCommon).Get<List<HIS_TREATMENT>>("/api/HisTreatment/Get", ApiConsumers.MosConsumer, treatFilter, paramCommon);

                    Inventec.Common.Logging.LogSystem.Info("DelegateAssignPrescription____2" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => lstTreatments), lstTreatments));

                    if (lstTreatments != null)
                    {
                        var treatment = lstTreatments.First();
                        assignServiceADO.TreatmentCode = treatment.TREATMENT_CODE;
                        assignServiceADO.TreatmentId = treatment.ID;
                        assignServiceADO.PatientDob = treatment.TDL_PATIENT_DOB;
                        assignServiceADO.PatientName = treatment.TDL_PATIENT_NAME;
                        assignServiceADO.GenderName = treatment.TDL_PATIENT_GENDER_NAME;
                        assignServiceADO.DgProcessDataResult = DelegateProcessDataResult;

                        listArgs.Add(assignServiceADO);
                        listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(moduleData, (assignPrescriptionDCO.RoomId ?? 0), (assignPrescriptionDCO.RoomTypeId ?? 0)));
                        var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, (assignPrescriptionDCO.RoomId ?? 0), (assignPrescriptionDCO.RoomTypeId ?? 0)), listArgs);
                        if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");
                        Form frmAssignPrescription = extenceInstance as Form;
                        frmAssignPrescription.ShowDialog();
                        frmAssignPrescription.ParentForm.Activate();
                        frmAssignPrescription.Activate();
                        ProcessFormOpened();
                        //frmAssignPrescription.TopMost = false;
                    }
                }

                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => assignPrescriptionResultDCO), assignPrescriptionResultDCO));
                Inventec.Common.Logging.LogSystem.Info("DelegateAssignPrescription____3");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return assignPrescriptionResultDCO;
        }

        private void ProcessFormOpened()
        {
            try
            {
                Form active = null;
                var a = Application.OpenForms.Cast<Form>().ToList();
                if (a != null && a.Count > 0)
                {
                    for (int i = (Application.OpenForms.Count - 1); i >= 0; i--)
                    {
                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("Application.OpenForms[i].Text", Application.OpenForms[i].Text));
                        if (Application.OpenForms[i].Name == "frmWaitForm" || String.IsNullOrEmpty(Application.OpenForms[i].Name))
                        {                           
                            active = Application.OpenForms[i];                           
                            active.Activate();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void DelegateProcessDataResult(object dataResult)
        {
            try
            {
                if (dataResult == null)
                    return;

                var currentMedicineTypes = BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>().Where(o => o.IS_LEAF == GlobalVariables.CommonNumberTrue).ToList();

                var currentMaterialTypes = BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>().Where(o => o.IS_LEAF == GlobalVariables.CommonNumberTrue).ToList();

                List<WcfAssignPrescriptionResultDetailDCO> assignPrescriptionResultDetailDCOs = new List<WcfAssignPrescriptionResultDetailDCO>();
                if (dataResult is OutPatientPresResultSDO)
                {
                    OutPatientPresResultSDO outPatientPresResultSDO = dataResult as OutPatientPresResultSDO;
                    if (outPatientPresResultSDO != null && outPatientPresResultSDO.Medicines != null && outPatientPresResultSDO.Medicines.Count > 0)
                    {
                        foreach (var medi in outPatientPresResultSDO.Medicines)
                        {
                            WcfAssignPrescriptionResultDetailDCO itemAdd = new WcfAssignPrescriptionResultDetailDCO();
                            var mety = currentMedicineTypes.Where(o => o.ID == medi.TDL_MEDICINE_TYPE_ID).FirstOrDefault();
                            if (mety != null)
                            {
                                itemAdd.MecicineTypeCode = mety.MEDICINE_TYPE_CODE;
                                itemAdd.MecicineTypeName = mety.MEDICINE_TYPE_NAME;
                                itemAdd.Tutorial = medi.TUTORIAL;

                                assignPrescriptionResultDetailDCOs.Add(itemAdd);
                            }
                        }
                    }
                    if (outPatientPresResultSDO != null && outPatientPresResultSDO.ServiceReqMeties != null && outPatientPresResultSDO.ServiceReqMeties.Count > 0)
                    {
                        foreach (var srqmedi in outPatientPresResultSDO.ServiceReqMeties)
                        {
                            WcfAssignPrescriptionResultDetailDCO itemAdd = new WcfAssignPrescriptionResultDetailDCO();
                            if (srqmedi.MEDICINE_TYPE_ID.HasValue && srqmedi.MEDICINE_TYPE_ID.Value > 0)
                            {
                                itemAdd.MecicineTypeCode = "";
                                itemAdd.MecicineTypeName = srqmedi.MEDICINE_TYPE_NAME;
                                itemAdd.Tutorial = srqmedi.TUTORIAL;

                                assignPrescriptionResultDetailDCOs.Add(itemAdd);
                            }
                            else
                            {
                                var mety = currentMedicineTypes.Where(o => o.ID == srqmedi.MEDICINE_TYPE_ID).FirstOrDefault();
                                if (mety != null)
                                {
                                    itemAdd.MecicineTypeCode = mety.MEDICINE_TYPE_CODE;
                                    itemAdd.MecicineTypeName = mety.MEDICINE_TYPE_NAME;
                                    itemAdd.Tutorial = srqmedi.TUTORIAL;

                                    assignPrescriptionResultDetailDCOs.Add(itemAdd);
                                }
                            }
                        }
                    }
                }
                else if (dataResult is InPatientPresResultSDO)
                {
                    InPatientPresResultSDO inPatientPresResultSDO = dataResult as InPatientPresResultSDO;
                    if (inPatientPresResultSDO != null && inPatientPresResultSDO.Medicines != null && inPatientPresResultSDO.Medicines.Count > 0)
                    {
                        foreach (var medi in inPatientPresResultSDO.Medicines)
                        {
                            WcfAssignPrescriptionResultDetailDCO itemAdd = new WcfAssignPrescriptionResultDetailDCO();
                            var mety = currentMedicineTypes.Where(o => o.ID == medi.TDL_MEDICINE_TYPE_ID).FirstOrDefault();
                            if (mety != null)
                            {
                                itemAdd.MecicineTypeCode = mety.MEDICINE_TYPE_CODE;
                                itemAdd.MecicineTypeName = mety.MEDICINE_TYPE_NAME;
                                itemAdd.Tutorial = medi.TUTORIAL;

                                assignPrescriptionResultDetailDCOs.Add(itemAdd);
                            }
                        }
                    }
                    if (inPatientPresResultSDO != null && inPatientPresResultSDO.ServiceReqMeties != null && inPatientPresResultSDO.ServiceReqMeties.Count > 0)
                    {
                        foreach (var srqmedi in inPatientPresResultSDO.ServiceReqMeties)
                        {
                            WcfAssignPrescriptionResultDetailDCO itemAdd = new WcfAssignPrescriptionResultDetailDCO();
                            if (srqmedi.MEDICINE_TYPE_ID.HasValue && srqmedi.MEDICINE_TYPE_ID.Value > 0)
                            {
                                itemAdd.MecicineTypeCode = "";
                                itemAdd.MecicineTypeName = srqmedi.MEDICINE_TYPE_NAME;
                                itemAdd.Tutorial = srqmedi.TUTORIAL;

                                assignPrescriptionResultDetailDCOs.Add(itemAdd);
                            }
                            else
                            {
                                var mety = currentMedicineTypes.Where(o => o.ID == srqmedi.MEDICINE_TYPE_ID).FirstOrDefault();
                                if (mety != null)
                                {
                                    itemAdd.MecicineTypeCode = EncodeData(mety.MEDICINE_TYPE_CODE);
                                    itemAdd.MecicineTypeName = EncodeData(mety.MEDICINE_TYPE_NAME);
                                    itemAdd.Tutorial = HIS.Desktop.LocalStorage.BackendData.EncryptUtil.EncodeData(srqmedi.TUTORIAL);

                                    assignPrescriptionResultDetailDCOs.Add(itemAdd);
                                }
                            }
                        }
                    }
                }

                if (assignPrescriptionResultDetailDCOs != null && assignPrescriptionResultDetailDCOs.Count > 0)
                {
                    this.assignPrescriptionResultDCO.Data = Inventec.Common.String.StringCompressor.CompressString(HIS.Desktop.LocalStorage.BackendData.EncryptUtil.EncodeData(assignPrescriptionResultDetailDCOs));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public static string EncodeData(string data)
        {
            string result = "";
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(data);
            result = System.Convert.ToBase64String(plainTextBytes);
            return result;
        }
    }
}

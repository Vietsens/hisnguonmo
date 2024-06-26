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
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Inventec.Core;
using HIS.Desktop.LocalStorage.BackendData;
using Inventec.Common.Adapter;
using MOS.SDO;
using HIS.Desktop.ApiConsumer;
using DevExpress.Data;
using System.Collections;
using DevExpress.XtraGrid.Views.Base;
using Inventec.Desktop.Common.Message;
using HIS.Desktop.Print;
using MOS.EFMODEL.DataModels;
using MPS.ADO;
using AutoMapper;
using HIS.Desktop.LocalStorage.LocalData;
using MOS.Filter;
using MPS.ADO.TrackingPrint;
using HIS.Desktop.Utility;

namespace HIS.Desktop.Plugins.BedRoomPartial
{
    public partial class UCBedRoomPartial : UserControlBase
    {
        internal enum PrintType
        {
            IN_GIAY_CHUNG_NHAN_NAM_VIEN,
            IN_PHIEU_CONG_KHAI_DV_KHAM_CHUA_BENH_NOI_TRU
        }

        void PrintProcessGiayChungNhan(PrintType printType)
        {
            try
            {
                Inventec.Common.RichEditor.RichEditorStore richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(HIS.Desktop.ApiConsumer.ApiConsumers.SarConsumer, HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), HIS.Desktop.LocalStorage.Location.PrintStoreLocation.PrintTemplatePath);

                switch (printType)
                {
                    case PrintType.IN_GIAY_CHUNG_NHAN_NAM_VIEN:
                        richEditorMain.RunPrintTemplate(PrintTypeCodeStorePlus.PRINT_TYPE_CODE__BIEUMAU__GIAY_CHUNG_NHAN_NAM_VIEN__MPS000179, DelegateRunPrinter);
                        break;
                    default:
                        break;
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        bool DelegateRunPrinter(string printTypeCode, string fileName)
        {
            bool result = false;
            try
            {
                switch (printTypeCode)
                {
                    case PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__PHIEU_YEU_CAU_IN_TO_DIEU_TRI__MPS000062:
                        break;
                    case PrintTypeCodeStorePlus.PRINT_TYPE_CODE__BIEUMAU__GIAY_CHUNG_NHAN_NAM_VIEN__MPS000179:
                        ProcessDataMps000179(printTypeCode, fileName, ref result);
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private void ProcessDataMps000179(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                if (this.treatmentBedRoomRow == null)
                    return;
                WaitingManager.Show();
                CommonParam paramGet = new CommonParam();
                V_HIS_PATIENT currentPatient = new V_HIS_PATIENT();
                MOS.Filter.HisPatientViewFilter patientFilter = new HisPatientViewFilter();
                patientFilter.ID = treatmentBedRoomRow.PATIENT_ID;
                var rsPatient = new BackendAdapter(paramGet).Get<List<V_HIS_PATIENT>>(HisRequestUriStore.HIS_PATIENT_GETVIEW, ApiConsumers.MosConsumer, patientFilter, paramGet);
                if (rsPatient != null && rsPatient.Count > 0)
                {
                    currentPatient = rsPatient.FirstOrDefault();
                }
                V_HIS_TREATMENT treatmentPrint = new V_HIS_TREATMENT();
                MOS.Filter.HisTreatmentViewFilter treatmentFilter = new HisTreatmentViewFilter();
                treatmentFilter.ID = treatmentBedRoomRow.TREATMENT_ID;
                var rsTreatment = new BackendAdapter(paramGet).Get<List<V_HIS_TREATMENT>>(HisRequestUriStore.HIS_TREATMENT_GETVIEW, ApiConsumers.MosConsumer, treatmentFilter, paramGet);
                if (rsTreatment != null && rsTreatment.Count > 0)
                {
                    treatmentPrint = rsTreatment.FirstOrDefault();
                }

                //BHYT
                MOS.Filter.HisPatientTypeAlterViewAppliedFilter hisPTAlterFilter = new HisPatientTypeAlterViewAppliedFilter();
                hisPTAlterFilter.TreatmentId = treatmentBedRoomRow.TREATMENT_ID;
                hisPTAlterFilter.InstructionTime = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(DateTime.Now).ToString("yyyyMMddHHmm") + "00");
                var currentHispatientTypeAlter = new BackendAdapter(paramGet).Get<V_HIS_PATIENT_TYPE_ALTER>(HisRequestUriStore.HIS_PATIENT_TYPE_ALTER_GET_APPLIED, ApiConsumers.MosConsumer, hisPTAlterFilter, paramGet);


                V_HIS_DEPARTMENT_TRAN depamentTran = new V_HIS_DEPARTMENT_TRAN();
                MOS.Filter.HisDepartmentTranLastFilter departmnetTran = new HisDepartmentTranLastFilter();
                departmnetTran.TREATMENT_ID = treatmentBedRoomRow.TREATMENT_ID;
                depamentTran = new BackendAdapter(paramGet).Get<V_HIS_DEPARTMENT_TRAN>("api/HisDepartmentTran/GetLastByTreatmentId", ApiConsumers.MosConsumer, departmnetTran, paramGet);

                Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((treatmentPrint != null ? treatmentPrint.TREATMENT_CODE : ""), printTypeCode, this.wkRoomId);
                MPS.Processor.Mps000179.PDO.Mps000179PDO mps000179PDO = new MPS.Processor.Mps000179.PDO.Mps000179PDO(
                    currentPatient,
                    treatmentPrint,
                    currentHispatientTypeAlter,
                    depamentTran
                    );

                WaitingManager.Hide();
                MPS.ProcessorBase.Core.PrintData PrintData = null;
                if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                {
                    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000179PDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, "") { EmrInputADO = inputADO };
                }
                else
                {
                    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000179PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "") { EmrInputADO = inputADO };
                }
                result = MPS.MpsPrinter.Run(PrintData);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessDataMps000225(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                if (this.treatmentBedRoomRow == null)
                    return;
                WaitingManager.Show();
                CommonParam paramGet = new CommonParam();
                V_HIS_PATIENT currentPatient = new V_HIS_PATIENT();
                MOS.Filter.HisPatientViewFilter patientFilter = new HisPatientViewFilter();
                patientFilter.ID = treatmentBedRoomRow.PATIENT_ID;
                var rsPatient = new BackendAdapter(paramGet).Get<List<V_HIS_PATIENT>>(HisRequestUriStore.HIS_PATIENT_GETVIEW, ApiConsumers.MosConsumer, patientFilter, paramGet);
                if (rsPatient != null && rsPatient.Count > 0)
                {
                    currentPatient = rsPatient.FirstOrDefault();
                }
                V_HIS_TREATMENT treatmentPrint = new V_HIS_TREATMENT();
                MOS.Filter.HisTreatmentViewFilter treatmentFilter = new HisTreatmentViewFilter();
                treatmentFilter.ID = treatmentBedRoomRow.TREATMENT_ID;
                var rsTreatment = new BackendAdapter(paramGet).Get<List<V_HIS_TREATMENT>>(HisRequestUriStore.HIS_TREATMENT_GETVIEW, ApiConsumers.MosConsumer, treatmentFilter, paramGet);
                if (rsTreatment != null && rsTreatment.Count > 0)
                {
                    treatmentPrint = rsTreatment.FirstOrDefault();
                }

                //BHYT
                MOS.Filter.HisPatientTypeAlterViewAppliedFilter hisPTAlterFilter = new HisPatientTypeAlterViewAppliedFilter();
                hisPTAlterFilter.TreatmentId = treatmentBedRoomRow.TREATMENT_ID;
                hisPTAlterFilter.InstructionTime = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(DateTime.Now).ToString("yyyyMMddHHmm") + "00");
                var currentHispatientTypeAlter = new BackendAdapter(paramGet).Get<V_HIS_PATIENT_TYPE_ALTER>(HisRequestUriStore.HIS_PATIENT_TYPE_ALTER_GET_APPLIED, ApiConsumers.MosConsumer, hisPTAlterFilter, paramGet);


                V_HIS_DEPARTMENT_TRAN depamentTran = new V_HIS_DEPARTMENT_TRAN();
                MOS.Filter.HisDepartmentTranLastFilter departmnetTran = new HisDepartmentTranLastFilter();
                departmnetTran.TREATMENT_ID = treatmentBedRoomRow.TREATMENT_ID;
                depamentTran = new BackendAdapter(paramGet).Get<V_HIS_DEPARTMENT_TRAN>("api/HisDepartmentTran/GetLastByTreatmentId", ApiConsumers.MosConsumer, departmnetTran, paramGet);


                MOS.Filter.HisServiceReqFilter serviceReqFilter = new HisServiceReqFilter();
                serviceReqFilter.TREATMENT_ID = treatmentBedRoomRow.TREATMENT_ID;
                serviceReqFilter.SERVICE_REQ_TYPE_IDs = new List<long>();
                serviceReqFilter.SERVICE_REQ_TYPE_IDs.Add(IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONDT);
                serviceReqFilter.SERVICE_REQ_TYPE_IDs.Add(IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONM);
                serviceReqFilter.REQUEST_DEPARTMENT_ID = WorkPlace.WorkPlaceSDO.FirstOrDefault(p => p.RoomId == this.currentModule.RoomId).DepartmentId; // filter theo khoa yeu cau(khoa cua nguoi dung dang dang nhap)
                var _ServiceReqs = new BackendAdapter(paramGet).Get<List<HIS_SERVICE_REQ>>(HisRequestUriStore.HIS_SERVICE_REQ_GET, ApiConsumers.MosConsumer, serviceReqFilter, paramGet);

                MOS.Filter.HisSereServViewFilter _ssFilter = new HisSereServViewFilter();
                _ssFilter.TREATMENT_ID = treatmentBedRoomRow.TREATMENT_ID;
                var _ListSereServs = new BackendAdapter(paramGet).Get<List<V_HIS_SERE_SERV>>(HisRequestUriStore.HIS_SERE_SERV_GETVIEW, ApiConsumers.MosConsumer, _ssFilter, paramGet);

                MPS.Processor.Mps000179.PDO.Mps000179PDO mps000179PDO = new MPS.Processor.Mps000179.PDO.Mps000179PDO(
                    currentPatient,
                    treatmentPrint,
                    currentHispatientTypeAlter,
                    depamentTran
                    );

                WaitingManager.Hide();
                MPS.ProcessorBase.Core.PrintData PrintData = null;
                if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                {
                    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000179PDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, "");
                }
                else
                {
                    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000179PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "");
                }
                result = MPS.MpsPrinter.Run(PrintData);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

    }
}

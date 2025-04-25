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
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.HisConfig;
using HIS.Desktop.LocalStorage.LocalData;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.Library.PrintTreatmentFinish
{
    class PrintMps000268
    {
        MPS.Processor.Mps000268.PDO.Mps000268PDO mps000268RDO { get; set; }
        bool printNow { get; set; }

        public PrintMps000268(string printTypeCode, string fileName, ref bool result, MOS.EFMODEL.DataModels.V_HIS_PATIENT HisPatient, MOS.EFMODEL.DataModels.HIS_TREATMENT HisTreatment, HIS_BRANCH currentBranch, bool _printNow, long? roomId)
        {
            try
            {
                if (HisTreatment == null || HisTreatment.ID <= 0)
                {
                    result = false;
                    return;
                }
                this.printNow = _printNow;

                V_HIS_TREATMENT vTreatment = new V_HIS_TREATMENT();

                CommonParam param = new CommonParam();
                HisTreatmentViewFilter treaViewFilter = new HisTreatmentViewFilter();
                treaViewFilter.ID = HisTreatment.ID;
                var apiResult = new BackendAdapter(param).Get<List<V_HIS_TREATMENT>>("api/HisTreatment/GetView", ApiConsumers.MosConsumer, treaViewFilter, param);
                if (apiResult != null && apiResult.Count > 0)
                {
                    vTreatment = apiResult.FirstOrDefault();
                }

                MPS.Processor.Mps000268.PDO.Mps000268ADO ado = new MPS.Processor.Mps000268.PDO.Mps000268ADO();
                if (HisTreatment.DEATH_CAUSE_ID != null)
                {
                    var deathCause = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_DEATH_CAUSE>().FirstOrDefault(o => o.ID == HisTreatment.DEATH_CAUSE_ID.Value);
                    if (deathCause != null)
                    {
                        ado.DEATH_CAUSE_CODE = deathCause.DEATH_CAUSE_CODE;
                        ado.DEATH_CAUSE_NAME = deathCause.DEATH_CAUSE_NAME;
                    }
                }
                if (HisTreatment.DEATH_WITHIN_ID != null)
                {
                    var deathWithin = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_DEATH_WITHIN>().FirstOrDefault(o => o.ID == HisTreatment.DEATH_WITHIN_ID.Value);
                    if (deathWithin != null)
                    {
                        ado.DEATH_WITHIN_CODE = deathWithin.DEATH_WITHIN_CODE;
                        ado.DEATH_WITHIN_NAME = deathWithin.DEATH_WITHIN_NAME;
                    }
                }

                if (HisTreatment.END_ROOM_ID.HasValue)
                {
                    var endRoom = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == HisTreatment.END_ROOM_ID.Value);
                    if (endRoom != null)
                    {
                        ado.END_DEPARTMENT_CODE = endRoom.DEPARTMENT_CODE;
                        ado.END_DEPARTMENT_NAME = endRoom.DEPARTMENT_NAME;
                        ado.END_ROOM_CODE = endRoom.ROOM_CODE;
                        ado.END_ROOM_NAME = endRoom.ROOM_NAME;
                    }
                }
                if (HisTreatment.FEE_LOCK_ROOM_ID.HasValue)
                {
                    var feelockRoom = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == HisTreatment.FEE_LOCK_ROOM_ID.Value);
                    if (feelockRoom != null)
                    {
                        ado.FEE_LOCK_DEPARTMENT_CODE = feelockRoom.DEPARTMENT_CODE;
                        ado.FEE_LOCK_DEPARTMENT_NAME = feelockRoom.DEPARTMENT_NAME;
                        ado.FEE_LOCK_ROOM_CODE = feelockRoom.ROOM_CODE;
                        ado.FEE_LOCK_ROOM_NAME = feelockRoom.ROOM_NAME;
                    }
                }
                if (HisTreatment.IN_ROOM_ID.HasValue)
                {
                    var inRoom = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == HisTreatment.IN_ROOM_ID.Value);
                    if (inRoom != null)
                    {
                        ado.IN_DEPARTMENT_CODE = inRoom.DEPARTMENT_CODE;
                        ado.IN_DEPARTMENT_NAME = inRoom.DEPARTMENT_NAME;
                        ado.IN_ROOM_CODE = inRoom.ROOM_CODE;
                        ado.IN_ROOM_NAME = inRoom.ROOM_NAME;
                    }
                }
                if (currentBranch != null)
                {
                    ado.BRANCH_ADDRESS = currentBranch.ADDRESS;
                }

                if (HisTreatment.TREATMENT_RESULT_ID.HasValue)
                {
                    var treatmentResult = BackendDataWorker.Get<HIS_TREATMENT_RESULT>().FirstOrDefault(o => o.ID == HisTreatment.TREATMENT_RESULT_ID.Value);
                    ado.TREATMENT_RESULT_CODE = treatmentResult != null ? treatmentResult.TREATMENT_RESULT_CODE : "";
                    ado.TREATMENT_RESULT_NAME = treatmentResult != null ? treatmentResult.TREATMENT_RESULT_NAME : "";
                }

                mps000268RDO = new MPS.Processor.Mps000268.PDO.Mps000268PDO(
                    HisPatient,
                   vTreatment,
                   ado
                   );

                mps000268RDO.TreatmentView = HisTreatment;

                result = Print.RunPrint(printTypeCode, fileName, mps000268RDO, (Inventec.Common.FlexCelPrint.DelegateEventLog)EventLogPrint, result, _printNow, roomId);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        //public PrintMps000268(string printTypeCode, string fileName, ref bool result, MOS.EFMODEL.DataModels.V_HIS_PATIENT HisPatient, MOS.EFMODEL.DataModels.HIS_TREATMENT HisTreatment, HIS_BRANCH currentBranch, MPS.ProcessorBase.PrintConfig.PreviewType? _previewType, long? roomId)
        //{
        //    try
        //    {
        //        if (HisTreatment == null || HisTreatment.ID <= 0)
        //        {
        //            result = false;
        //            return;
        //        }
        //        //this.printNow = _printNow;

        //        V_HIS_TREATMENT vTreatment = new V_HIS_TREATMENT();

        //        CommonParam param = new CommonParam();
        //        HisTreatmentViewFilter treaViewFilter = new HisTreatmentViewFilter();
        //        treaViewFilter.ID = HisTreatment.ID;
        //        var apiResult = new BackendAdapter(param).Get<List<V_HIS_TREATMENT>>("api/HisTreatment/GetView", ApiConsumers.MosConsumer, treaViewFilter, param);
        //        if (apiResult != null && apiResult.Count > 0)
        //        {
        //            vTreatment = apiResult.FirstOrDefault();
        //        }

        //        MPS.Processor.Mps000268.PDO.Mps000268ADO ado = new MPS.Processor.Mps000268.PDO.Mps000268ADO();
        //        if (HisTreatment.DEATH_CAUSE_ID != null)
        //        {
        //            var deathCause = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_DEATH_CAUSE>().FirstOrDefault(o => o.ID == HisTreatment.DEATH_CAUSE_ID.Value);
        //            if (deathCause != null)
        //            {
        //                ado.DEATH_CAUSE_CODE = deathCause.DEATH_CAUSE_CODE;
        //                ado.DEATH_CAUSE_NAME = deathCause.DEATH_CAUSE_NAME;
        //            }
        //        }
        //        if (HisTreatment.DEATH_WITHIN_ID != null)
        //        {
        //            var deathWithin = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_DEATH_WITHIN>().FirstOrDefault(o => o.ID == HisTreatment.DEATH_WITHIN_ID.Value);
        //            if (deathWithin != null)
        //            {
        //                ado.DEATH_WITHIN_CODE = deathWithin.DEATH_WITHIN_CODE;
        //                ado.DEATH_WITHIN_NAME = deathWithin.DEATH_WITHIN_NAME;
        //            }
        //        }

        //        if (HisTreatment.END_ROOM_ID.HasValue)
        //        {
        //            var endRoom = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == HisTreatment.END_ROOM_ID.Value);
        //            if (endRoom != null)
        //            {
        //                ado.END_DEPARTMENT_CODE = endRoom.DEPARTMENT_CODE;
        //                ado.END_DEPARTMENT_NAME = endRoom.DEPARTMENT_NAME;
        //                ado.END_ROOM_CODE = endRoom.ROOM_CODE;
        //                ado.END_ROOM_NAME = endRoom.ROOM_NAME;
        //            }
        //        }
        //        if (HisTreatment.FEE_LOCK_ROOM_ID.HasValue)
        //        {
        //            var feelockRoom = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == HisTreatment.FEE_LOCK_ROOM_ID.Value);
        //            if (feelockRoom != null)
        //            {
        //                ado.FEE_LOCK_DEPARTMENT_CODE = feelockRoom.DEPARTMENT_CODE;
        //                ado.FEE_LOCK_DEPARTMENT_NAME = feelockRoom.DEPARTMENT_NAME;
        //                ado.FEE_LOCK_ROOM_CODE = feelockRoom.ROOM_CODE;
        //                ado.FEE_LOCK_ROOM_NAME = feelockRoom.ROOM_NAME;
        //            }
        //        }
        //        if (HisTreatment.IN_ROOM_ID.HasValue)
        //        {
        //            var inRoom = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == HisTreatment.IN_ROOM_ID.Value);
        //            if (inRoom != null)
        //            {
        //                ado.IN_DEPARTMENT_CODE = inRoom.DEPARTMENT_CODE;
        //                ado.IN_DEPARTMENT_NAME = inRoom.DEPARTMENT_NAME;
        //                ado.IN_ROOM_CODE = inRoom.ROOM_CODE;
        //                ado.IN_ROOM_NAME = inRoom.ROOM_NAME;
        //            }
        //        }
        //        if (currentBranch != null)
        //        {
        //            ado.BRANCH_ADDRESS = currentBranch.ADDRESS;
        //        }

        //        if (HisTreatment.TREATMENT_RESULT_ID.HasValue)
        //        {
        //            var treatmentResult = BackendDataWorker.Get<HIS_TREATMENT_RESULT>().FirstOrDefault(o => o.ID == HisTreatment.TREATMENT_RESULT_ID.Value);
        //            ado.TREATMENT_RESULT_CODE = treatmentResult != null ? treatmentResult.TREATMENT_RESULT_CODE : "";
        //            ado.TREATMENT_RESULT_NAME = treatmentResult != null ? treatmentResult.TREATMENT_RESULT_NAME : "";
        //        }

        //        mps000268RDO = new MPS.Processor.Mps000268.PDO.Mps000268PDO(
        //            HisPatient,
        //           vTreatment,
        //           ado
        //           );

        //        mps000268RDO.TreatmentView = HisTreatment;

        //        result = Print.RunPrint(printTypeCode, fileName, mps000268RDO, (Inventec.Common.FlexCelPrint.DelegateEventLog)EventLogPrint, result, _previewType, roomId);
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Warn(ex);
        //    }
        //}

        private bool RunPrint(string printTypeCode, string fileName, bool result, bool _printNow)
        {
            try
            {
                string printerName = "";
                if (GlobalVariables.dicPrinter.ContainsKey(printTypeCode))
                {
                    printerName = GlobalVariables.dicPrinter[printTypeCode];
                }

                if (_printNow)
                {
                    result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000268RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName, (Inventec.Common.FlexCelPrint.DelegateEventLog)EventLogPrint));
                }
                else if (HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.CheDoInChoCacChucNangTrongPhanMem == 2)
                {
                    result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000268RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName, (Inventec.Common.FlexCelPrint.DelegateEventLog)EventLogPrint));
                }
                else
                {
                    result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000268RDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, printerName, (Inventec.Common.FlexCelPrint.DelegateEventLog)EventLogPrint));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return false;
            }
            return result;
        }

        private void EventLogPrint()
        {
            try
            {
                string message = "In phiếu giấy báo tử. Mã in : Mps000268" + "  TREATMENT_CODE: " + this.mps000268RDO.TreatmentView.TREATMENT_CODE + "  Thời gian in: " + Inventec.Common.DateTime.Convert.SystemDateTimeToTimeSeparateString(DateTime.Now) + "  Người in: " + Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                His.EventLog.Logger.Log(GlobalVariables.APPLICATION_CODE, Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName(), message, Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginAddress());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }
    }
}

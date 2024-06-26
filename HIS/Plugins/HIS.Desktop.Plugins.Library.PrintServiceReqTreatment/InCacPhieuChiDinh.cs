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
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.HisConfig;
using Inventec.Common.SignLibrary.DTO;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HIS.Desktop.Plugins.Library.PrintServiceReqTreatment
{
    class InCacPhieuChiDinh
    {

        public InCacPhieuChiDinh(string printTypeCode, string fileName, List<V_HIS_SERVICE_REQ> _lstServiceReqs, List<HIS_SERE_SERV> _lstSereServ, List<HIS_TREATMENT> _lstTreatment, List<V_HIS_PATIENT_TYPE_ALTER> _LstPatientTypeAlter, V_HIS_ROOM _vHisRoom, bool printNow, ref bool result,List<HIS_CONFIG> configs,HIS_TRANS_REQ tranReq, Action<DocumentSignedUpdateIGSysResultDTO> DlgSendResultSigned)
        {
            try
            {
                if (_lstTreatment != null && _lstTreatment.Count > 0)
                {
                    foreach (var _treatment in _lstTreatment)
                    {
                        EmrDataStore.treatmentCode = _treatment.TREATMENT_CODE;

                        var _vServiceReqs = _lstServiceReqs.Where(o => o.TREATMENT_ID == _treatment.ID).ToList();
                        var _patientTypeAlter = _LstPatientTypeAlter != null ? _LstPatientTypeAlter.FirstOrDefault(o => o.TREATMENT_ID == _treatment.ID) : null;
                        var _sereServs = _lstSereServ != null ? _lstSereServ.Where(o => o.TDL_TREATMENT_ID == _treatment.ID).ToList() : null;

                        MPS.Processor.Mps000276.PDO.Mps000276PDO mps000276RDO = new MPS.Processor.Mps000276.PDO.Mps000276PDO(
                            _vServiceReqs,
                            _treatment,
                            _patientTypeAlter,
                            _vHisRoom,
                            _sereServs,
                            BackendDataWorker.Get<V_HIS_SERVICE>(),
                            BackendDataWorker.Get<V_HIS_ROOM>(),
                            BackendDataWorker.Get<V_HIS_CASHIER_ROOM>(),
                            BackendDataWorker.Get<HIS_SERVICE_NUM_ORDER>(),
                            BackendDataWorker.Get<V_HIS_DESK>(),
                            configs,
                            tranReq
                            );
                        Print.PrintData(_vHisRoom.ID, printTypeCode, fileName, mps000276RDO, printNow, ref result, DlgSendResultSigned);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}

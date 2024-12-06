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
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace HIS.Desktop.Plugins.Library.PrintServiceReq
{
    class InPhieuYeuCauChiDinhTongHop
    {
        public InPhieuYeuCauChiDinhTongHop(string printTypeCode, string fileName, ADO.ChiDinhDichVuADO chiDinhDichVuADO,
            Dictionary<long, List<MOS.EFMODEL.DataModels.V_HIS_SERVICE_REQ>> dicServiceReqData,
            Dictionary<long, List<MOS.EFMODEL.DataModels.V_HIS_SERE_SERV>> dicSereServData,
            Dictionary<long, HIS_SERE_SERV_EXT> dicSereServExtData, bool printNow,
            ref bool result, long? roomId, bool isView, MPS.ProcessorBase.PrintConfig.PreviewType? PreviewType,
            Action<int, Inventec.Common.FlexCelPrint.Ado.PrintMergeAdo> savedData,
            Action<string> cancelPrint, Action<Inventec.Common.SignLibrary.DTO.DocumentSignedUpdateIGSysResultDTO> DlgSendResultSigned)
        {
            try
            {
                var treatment = new HIS_TREATMENT();
                Inventec.Common.Mapper.DataObjectMapper.Map<HIS_TREATMENT>(treatment, chiDinhDichVuADO.treament);
                List<V_HIS_SERE_SERV> lstSereServ = new List<V_HIS_SERE_SERV>();
                foreach (var item in dicSereServData)
                {
                    lstSereServ.AddRange(item.Value);
                }

                MPS.Processor.Mps000037.PDO.Mps000037ADO Mps000037ADO = new MPS.Processor.Mps000037.PDO.Mps000037ADO();
                Mps000037ADO.bebRoomName = chiDinhDichVuADO.BedRoomName;
                Mps000037ADO.departmentName = chiDinhDichVuADO.DepartmentName;
                Mps000037ADO.ratio = chiDinhDichVuADO.Ratio;

                List<V_HIS_SERVICE_REQ> req = new List<V_HIS_SERVICE_REQ>();
                foreach (var item in dicServiceReqData)
                {
                    req.AddRange(item.Value);
                }

                var acsUser = BackendDataWorker.Get<ACS.EFMODEL.DataModels.ACS_USER>().FirstOrDefault(o => o.LOGINNAME == req.First().REQUEST_LOGINNAME);
                Mps000037ADO.REQUEST_USER_MOBILE = acsUser != null ? acsUser.MOBILE : "";

                List<HIS_SERE_SERV_EXT> lstExt = new List<HIS_SERE_SERV_EXT>();
                foreach (var item in dicSereServExtData)
                {
                    lstExt.Add(item.Value);
                }

                Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => lstSereServ), lstSereServ));

                MPS.Processor.Mps000037.PDO.Mps000037PDO mps000037PDO = new MPS.Processor.Mps000037.PDO.Mps000037PDO(
                    lstSereServ,
                    dicServiceReqData.FirstOrDefault().Value.FirstOrDefault(),
                    chiDinhDichVuADO.patientTypeAlter,
                    treatment,
                    Mps000037ADO,
                    req,
                    lstExt,
                    BackendDataWorker.Get<V_HIS_SERVICE>(),
                    BackendDataWorker.Get<HIS_SERVICE_REQ_TYPE>());

                Print.PrintData(printTypeCode, fileName, mps000037PDO, printNow, ref result, roomId, isView, PreviewType, lstSereServ.Count, savedData, dicServiceReqData.FirstOrDefault().Value.FirstOrDefault().TREATMENT_CODE, DlgSendResultSigned);
            }
            catch (Exception ex)
            {
                cancelPrint(printTypeCode);
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}

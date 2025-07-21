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
using HIS.Desktop.LocalStorage.LocalData;
using Inventec.Common.Adapter;
using Inventec.Common.SignLibrary.DTO;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using MPS.Processor.Mps000432.PDO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.Library.PrintServiceReq
{
    class InDichVuNgoaiKCB
    {
        List<V_HIS_SERVICE_REQ> ServiceReq = new List<V_HIS_SERVICE_REQ>();
        Dictionary<long, List<HIS_SERE_NMSE>> dicSereServ = new Dictionary<long, List<HIS_SERE_NMSE>>();

        //in gộp xét nghiệp sẽ tăng số lượng dịch vụ
        //cần xử lý để gộp file đủ 
        public InDichVuNgoaiKCB(string printTypeCode, string fileName, ADO.ChiDinhDichVuADO chiDinhDichVuADO,
            Dictionary<long, List<V_HIS_SERVICE_REQ>> dicServiceReqData,
            Dictionary<long, List<HIS_SERE_NMSE>> dicSereServData,
            bool printNow, ref bool result, long? roomId, bool isView,
            MPS.ProcessorBase.PrintConfig.PreviewType? PreviewType,
            bool isMethodSaveNPrint, Action<int, Inventec.Common.FlexCelPrint.Ado.PrintMergeAdo> savedData,
            Action<string> cancelPrint, Action<Inventec.Common.SignLibrary.DTO.DocumentSignedUpdateIGSysResultDTO> DlgSendResultSigned)
        {
            try
            {
                ServiceReq = dicServiceReqData[IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__OT].ToList();
                if (ServiceReq != null && ServiceReq.Count > 0)
                {
                    foreach (var item in dicSereServData)
                    {
                        dicSereServ.Add(item.Key, item.Value);
                    }

                    foreach (var item in ServiceReq)
                    {
                        if (dicSereServ.ContainsKey(item.ID))
                        {
                            var listSereNmse = dicSereServ[item.ID].ToList();
                            var vtreatment = new V_HIS_TREATMENT();
                            Inventec.Common.Mapper.DataObjectMapper.Map<V_HIS_TREATMENT>(vtreatment, chiDinhDichVuADO.treament);
                            MPS.Processor.Mps000502.PDO.Mps000502PDO pdo = new MPS.Processor.Mps000502.PDO.Mps000502PDO(vtreatment,
                        item,
                        listSereNmse
                        );

                            Print.PrintData(printTypeCode, fileName, pdo, printNow, ref result, roomId, isView, PreviewType, listSereNmse.Count, savedData, chiDinhDichVuADO.treament.TREATMENT_CODE, DlgSendResultSigned);
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                cancelPrint(printTypeCode);
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}

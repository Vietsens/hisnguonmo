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
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.Library.EmrGenerate;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.Library.PrintTestTotal
{
    public class PrintTestTotalProcessor
    {
        private long TreatmentId { get; set; }
        private long? RoomId { get; set; }
        private V_HIS_TREATMENT VHisTreatment { get; set; }
        private HIS_SERVICE_REQ HisServiceReq { get; set; }
        private MPS.ProcessorBase.PrintConfig.PreviewType? previewType;

        public PrintTestTotalProcessor(long roomId, V_HIS_TREATMENT hisTreatment)
        {
            if (CheckRoom(roomId))
            {
                this.RoomId = roomId;
            }
            this.VHisTreatment = hisTreatment;
            this.TreatmentId = hisTreatment.ID;
        }

        public PrintTestTotalProcessor(long roomId, long treatmentId)
        {
            if (CheckRoom(roomId))
            {
                this.RoomId = roomId;
            }
            this.TreatmentId = treatmentId;
        }

        public PrintTestTotalProcessor(HIS_SERVICE_REQ serviceReq)
        {
            this.HisServiceReq = serviceReq;
            this.RoomId = serviceReq.EXECUTE_ROOM_ID;
            this.TreatmentId = serviceReq.TREATMENT_ID;
        }

        public void Print()
        {
            this.Print(MPS.Processor.Mps000222.PDO.Mps000222PDO.PrintTypeCode);
        }

        public void Print(string prinTypeCode)
        {
            try
            {
                Inventec.Common.RichEditor.RichEditorStore richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumer.ApiConsumers.SarConsumer, HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), HIS.Desktop.LocalStorage.Location.PrintStoreLocation.ROOT_PATH);
                richEditorMain.RunPrintTemplate(prinTypeCode, DelegateRunPrinter);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void Print(string prinTypeCode, MPS.ProcessorBase.PrintConfig.PreviewType? previewType)
        {
            try
            {
                Inventec.Common.RichEditor.RichEditorStore richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumer.ApiConsumers.SarConsumer, HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), HIS.Desktop.LocalStorage.Location.PrintStoreLocation.ROOT_PATH);
                this.previewType = previewType;
                richEditorMain.RunPrintTemplate(prinTypeCode, DelegateRunPrinter);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        //là phòng xử lý thì lấy yêu cầu khám của phòng và các chỉ định phòng tạo ra. không phải sẽ lấy hết
        private bool CheckRoom(long roomId)
        {
            bool result = false;
            try
            {
                var room = BackendDataWorker.Get<V_HIS_EXECUTE_ROOM>().FirstOrDefault(o => o.ROOM_ID == roomId);
                if (room != null)
                {
                    result = true;
                }
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private bool DelegateRunPrinter(string printCode, string fileName)
        {
            bool result = true;
            try
            {
                if (this.VHisTreatment == null)
                {
                    this.VHisTreatment = new V_HIS_TREATMENT();
                    MOS.Filter.HisTreatmentViewFilter filter = new MOS.Filter.HisTreatmentViewFilter();
                    filter.ID = this.TreatmentId;
                    var treatment = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_TREATMENT>>("api/HisTreatment/GetView", ApiConsumer.ApiConsumers.MosConsumer, filter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, null);
                    if (treatment != null && treatment.Count > 0)
                    {
                        this.VHisTreatment = treatment.FirstOrDefault();
                    }
                }
                switch (printCode)
                {
                    case MPS.Processor.Mps000222.PDO.Mps000222PDO.PrintTypeCode:
                        new Mps000222Processor(printCode, fileName, ref result, this.RoomId, VHisTreatment);
                        break;
                    case "Mps000316":
                        if (HisServiceReq == null)
                        {
                            ProcessGetServiceReq();
                        }
                        //đảm bảo hồ sơ ko có yêu cầu khám cũng in ra đc chi tiết.
                        if (HisServiceReq == null)
                        {
                            HisServiceReq = new HIS_SERVICE_REQ();
                            HisServiceReq.TREATMENT_ID = VHisTreatment.ID;
                        }
                        if (this.previewType != null)
                        {
                            new Mps000316Processor(printCode, fileName, ref result, HisServiceReq, VHisTreatment, this.RoomId, this.previewType);
                        }
                        else
                        {
                            new Mps000316Processor(printCode, fileName, ref result, HisServiceReq, VHisTreatment, this.RoomId, null);
                        }
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
            return result;
        }

        private void ProcessGetServiceReq()
        {
            try
            {
                MOS.Filter.HisServiceReqFilter filter = new MOS.Filter.HisServiceReqFilter();
                filter.TREATMENT_ID = this.TreatmentId;
                filter.SERVICE_REQ_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__KH;
                var serviceReq = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<HIS_SERVICE_REQ>>("api/HisServiceReq/Get", ApiConsumer.ApiConsumers.MosConsumer, filter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, null);
                if (serviceReq != null && serviceReq.Count > 0)
                {
                    this.HisServiceReq = serviceReq.FirstOrDefault(o => !o.PARENT_ID.HasValue) ?? serviceReq.First();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}

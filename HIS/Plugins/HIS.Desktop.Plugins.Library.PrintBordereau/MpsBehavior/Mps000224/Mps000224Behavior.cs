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
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.Plugins.Library.PrintBordereau.Base;
using HIS.Desktop.Plugins.Library.PrintBordereau.ChooseService;
using HIS.Desktop.Plugins.Library.PrintBordereau.Config;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MPS.Processor.Mps000120.PDO;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.Library.PrintBordereau.Mps000120
{


    public class Mps000224Behavior : MpsDataBase, ILoad
    {
        internal HIS_SERE_SERV sereServKTC;

        public Mps000224Behavior(long? roomId,
            List<HIS_SERE_SERV> _sereServs,
            List<V_HIS_DEPARTMENT_TRAN> _departmentTrans,
            List<V_HIS_TREATMENT_FEE> _treamentFees,
            V_HIS_TREATMENT _treatment,
            List<V_HIS_ROOM> _rooms,
            List<V_HIS_SERVICE> _services,
            List<HIS_HEIN_SERVICE_TYPE> _heinServiceTypes,
            long _totalDayTreatment,
            string _statusTreatmentOut,
            string _departmentName,
            string _userNameReturnResult)
            : base(roomId, _treatment)
        {
            this.SereServs = _sereServs;
            this.DepartmentTrans = _departmentTrans;
            this.TreatmentFees = _treamentFees;
            this.Treatment = _treatment;
            this.Rooms = _rooms;
            this.Services = _services;
            this.HeinServiceTypes = _heinServiceTypes;
            this.TotalDayTreatment = _totalDayTreatment;
            this.StatusTreatmentOut = _statusTreatmentOut;
            this.DepartmentName = _departmentName;
            this.UserNameReturnResult = _userNameReturnResult;
        }

        bool ILoad.Load(string printTypeCode, string fileName, Inventec.Common.FlexCelPrint.DelegateReturnEventPrint returnEventPrint)
        {
            bool result = false;
            try
            {
                MPS.Processor.Mps000224.PDO.Config.ServiceTypeCFG serviceTypeCFG = new MPS.Processor.Mps000224.PDO.Config.ServiceTypeCFG();
                serviceTypeCFG.SERVICE_TYPE_ID__MEDI = IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC;
                serviceTypeCFG.SERVICE_TYPE_ID__MATE = IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__VT;

                Dictionary<string, List<HIS_SERE_SERV>> dicSereServ = new Dictionary<string, List<HIS_SERE_SERV>>();
                List<HIS_PATIENT_TYPE_ALTER> patyBhyts = new List<HIS_PATIENT_TYPE_ALTER>();
                dicSereServ = SereServProcessor.GroupSereServByPatyAlterBhyt(this.SereServs);
                foreach (var item in dicSereServ)
                {
                    if (item.Value == null || item.Value.Count == 0)
                    {
                        continue;
                    }
                    HIS_PATIENT_TYPE_ALTER patyBhyt = JsonConvert.DeserializeObject<HIS_PATIENT_TYPE_ALTER>(item.Value.FirstOrDefault().JSON_PATIENT_TYPE_ALTER);
                    patyBhyts.Add(patyBhyt);
                }
                var sereServIsPackages = new List<HIS_SERE_SERV>();
                int PrintBordereau = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<int>("HIS.Desktop.Plugins.PrintBordereau.Mps000224.PtttNoParentOption");
                List<long> sereServHasChildIds = this.SereServs.Where(o => o.PARENT_ID.HasValue).Select(p => p.PARENT_ID.Value).ToList();
                sereServIsPackages = this.SereServs.Where(o => sereServHasChildIds.Contains(o.ID)).Distinct().ToList();
                if (PrintBordereau == 1)
                {
                    List<V_HIS_SERVICE> hisService = BackendDataWorker.Get<V_HIS_SERVICE>().Where(o => o.PTTT_GROUP_ID != null).ToList();
                    foreach (var item in SereServs)
                    {
                        if (!sereServIsPackages.Exists(o => o.ID == item.ID))
                        {
                            //thêm dịch vụ có loại là pttt
                            if (item.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__PT || item.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__TT)
                            {
                                sereServIsPackages.Add(item);
                            }
                            else
                            {
                                //thêm dịch vụ được thiết lập nhóm pttt
                                var service = hisService.FirstOrDefault(o => o.ID == item.SERVICE_ID);
                                if (service != null)
                                {
                                    sereServIsPackages.Add(item);
                                }
                            }
                        }
                    }
                }

                frmChonDichVuPhauThuat frmChoose = new frmChonDichVuPhauThuat(sereServIsPackages, SereServKTCChoosed);
                frmChoose.ShowDialog();

                if (sereServKTC == null)
                {
                    //MessageBox.Show("Không tìm thấy dịch vụ phẫu thuật thủ thuật");
                    return false;
                }
                WaitingManager.Hide();

                MPS.Processor.Mps000224.PDO.SingleKeyValue singleKeyValue = new MPS.Processor.Mps000224.PDO.SingleKeyValue();

                List<HIS_TREATMENT_TYPE> treatmentTypes = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_TREATMENT_TYPE>();
                List<HIS_HEIN_SERVICE_TYPE> heinServiceTypes = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_HEIN_SERVICE_TYPE>();
                HIS_BRANCH branch = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_BRANCH>().FirstOrDefault(o => o.ID == HIS.Desktop.LocalStorage.LocalData.WorkPlace.GetBranchId());

                CommonParam param = new CommonParam();
                HisServiceReqFilter serviceReqFilter = new HisServiceReqFilter();
                serviceReqFilter.ID = sereServKTC.SERVICE_REQ_ID;
                HIS_SERVICE_REQ serviceReq = new BackendAdapter(param)
                    .Get<List<MOS.EFMODEL.DataModels.HIS_SERVICE_REQ>>("api/HisServiceReq/Get", ApiConsumers.MosConsumer, serviceReqFilter, param).FirstOrDefault();
                singleKeyValue.executeTimeStr = (serviceReq != null && serviceReq.START_TIME.HasValue) ? Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(serviceReq.START_TIME.Value) : "";
                singleKeyValue.departmentName = DepartmentName;

                MPS.Processor.Mps000224.PDO.Mps000224PDO rdo = new MPS.Processor.Mps000224.PDO.Mps000224PDO(patyBhyts, sereServKTC.ID, this.SereServs, Treatment, treatmentTypes, heinServiceTypes, branch, this.Rooms, this.Services, serviceTypeCFG, singleKeyValue);

                Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode(Treatment.TREATMENT_CODE, printTypeCode, this.RoomId);
                inputADO.DlgSendResultSigned = PrintBordereauProcessor.DlgSendResultSigned;
                result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "") { EmrInputADO = inputADO });
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }
        private void SereServKTCChoosed(object data)
        {
            try
            {
                if (data != null)
                    sereServKTC = data as HIS_SERE_SERV;
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}

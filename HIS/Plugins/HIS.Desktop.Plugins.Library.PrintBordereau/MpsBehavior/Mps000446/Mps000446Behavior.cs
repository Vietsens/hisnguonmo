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
using DevExpress.XtraEditors;
using HIS.Desktop.LocalStorage.ConfigApplication;
//using HIS.Desktop.Plugins.Library.PrintBordereau.BankQrCode;
using HIS.Desktop.Plugins.Library.PrintBordereau.Base;
using HIS.Desktop.Plugins.Library.PrintBordereau.Config;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MPS.Processor.Mps000446.PDO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.Library.PrintBordereau.MpsBehavior.Mps000446
{
    class Mps000446Behavior : MpsDataBase, ILoad
    {
        public Mps000446Behavior(long? roomId, V_HIS_PATIENT_TYPE_ALTER currentPatientTypeAlter, List<HIS_SERE_SERV> _sereServs,
            List<V_HIS_DEPARTMENT_TRAN> _departmentTrans, List<V_HIS_TREATMENT_FEE> _treamentFees, V_HIS_TREATMENT _treatment,
            V_HIS_PATIENT _patient, List<V_HIS_ROOM> _rooms, List<V_HIS_SERVICE> _services, List<HIS_HEIN_SERVICE_TYPE> _heinServiceTypes,
            long _totalDayTreatment, string _statusTreatmentOut, string _departmentName, string _roomName, string _userNameReturnResult,
            List<HIS_SERE_SERV_BILL> listSsBill, HIS_TRANS_REQ _transReq, List<HIS_CONFIG> _lstConfig, bool IsActionButtonPrintBill)
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
            this.CurrentPatientTypeAlter = currentPatientTypeAlter;
            this.RoomName = _roomName;
            this.Patient = _patient;
            this.SereServBills = listSsBill;
            this.transReq2 = _transReq;
            this.lstConfig = _lstConfig;
            this.IsActionButtonPrintBill = IsActionButtonPrintBill;
        }

        bool ILoad.Load(string printTypeCode, string fileName, Inventec.Common.FlexCelPrint.DelegateReturnEventPrint returnEventPrint)
        {
            bool result = false;
            try
            {
                //if (HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(SdaConfigKey.CreateQRCodeBillCFG) == "1")
                //{
                //    CreateBankQrCodeProcessor qr = new CreateBankQrCodeProcessor(this.RoomId ?? 0, lstSs, this.Treatment);
                //    transReqData = qr.Create();

                //    if (transReqData == null || String.IsNullOrWhiteSpace(transReqData.BANK_QR_CODE))
                //    {
                //        if (XtraMessageBox.Show("Hệ thống tạo mã QR thanh toán thất bại bạn có muốn tiếp tục in hay không?", "Thông báo", MessageBoxButtons.OKCancel) != DialogResult.OK)
                //            return result;
                //    }
                //}

                //V_HIS_TREATMENT_FEE treatment = this.TreatmentFees.FirstOrDefault(o => o.ID == this.Treatment.ID);
                //List<V_HIS_ROOM> listRoom = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_ROOM>();
                //List<V_HIS_SERVICE> listService = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_SERVICE>();
                //List<HIS_SERVICE_TYPE> listServiceType = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_SERVICE_TYPE>();

                //MPS.Processor.Mps000446.PDO.Mps000446PDO rdo = new MPS.Processor.Mps000446.PDO.Mps000446PDO(treatment, transReqData, lstSs, listRoom, listService, listServiceType);
                CommonParam param = new CommonParam();
                List<V_HIS_SERE_SERV> VHisSereServs = new List<V_HIS_SERE_SERV>();
                List<HIS_SESE_TRANS_REQ> HisSeseTransReqs = new List<HIS_SESE_TRANS_REQ>();
                if (transReq2 != null)
                {
                    HisSeseTransReqFilter seseTransReqFilter = new HisSeseTransReqFilter();
                    seseTransReqFilter.TRANS_REQ_ID = transReq2.ID;
                    HisSeseTransReqs = new Inventec.Common.Adapter.BackendAdapter(param)
                      .Get<List<MOS.EFMODEL.DataModels.HIS_SESE_TRANS_REQ>>("api/HisSeseTransReq/Get", ApiConsumer.ApiConsumers.MosConsumer, seseTransReqFilter, param);
                    if (HisSeseTransReqs != null && HisSeseTransReqs.Count > 0)
                    {
                        HisSereServViewFilter ssViewFilter = new HisSereServViewFilter();
                        ssViewFilter.IDs = HisSeseTransReqs.Select(o => o.SERE_SERV_ID).ToList();
                        VHisSereServs = new Inventec.Common.Adapter.BackendAdapter(param)
                          .Get<List<MOS.EFMODEL.DataModels.V_HIS_SERE_SERV>>("api/HisSereServ/GetView", ApiConsumer.ApiConsumers.MosConsumer, ssViewFilter, param);
                    }

                    if (VHisSereServs == null || VHisSereServs.Count <= 0)
                    {
                        if (IsActionButtonPrintBill)
                            XtraMessageBox.Show("Không có dịch vụ cần thanh toán!", "Thông báo");
                        return result;
                    }

                    MPS.Processor.Mps000446.PDO.Mps000446PDO rdo = new MPS.Processor.Mps000446.PDO.Mps000446PDO(HisSeseTransReqs, VHisSereServs, Treatment, transReq2, lstConfig);

                    #region Run Print

                    PrintCustomShow<Mps000446PDO> printShow = new PrintCustomShow<Mps000446PDO>(printTypeCode, fileName, rdo, returnEventPrint, this.isPreview);
                    result = printShow.SignRun(Treatment.TREATMENT_CODE, this.RoomId);
                    #endregion
                }else
                {
                    if (IsActionButtonPrintBill)
                        XtraMessageBox.Show("Không có dịch vụ cần thanh toán!", "Thông báo");
                }
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }
    }
}

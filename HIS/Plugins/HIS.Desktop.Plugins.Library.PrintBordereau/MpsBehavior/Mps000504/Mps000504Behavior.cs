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
using MPS.Processor.Mps000504.PDO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.Library.PrintBordereau.MpsBehavior.Mps000504
{
    class Mps000504Behavior : MpsDataBase, ILoad
    {
        public Mps000504Behavior(long? roomId, V_HIS_PATIENT_TYPE_ALTER currentPatientTypeAlter, List<HIS_SERE_SERV> _sereServs,
            List<V_HIS_DEPARTMENT_TRAN> _departmentTrans, List<V_HIS_TREATMENT_FEE> _treamentFees, V_HIS_TREATMENT _treatment,
            V_HIS_PATIENT _patient, List<V_HIS_ROOM> _rooms, List<V_HIS_SERVICE> _services, List<HIS_HEIN_SERVICE_TYPE> _heinServiceTypes,
            long _totalDayTreatment, string _statusTreatmentOut, string _departmentName, string _roomName, string _userNameReturnResult,
            List<HIS_SERE_SERV_BILL> listSsBill, HIS_TRANS_REQ _transReq, List<HIS_CONFIG> _lstConfig, bool IsActionButtonPrintBill, long fromDateReq, long toDateReq)
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
            this.fromDateReq = fromDateReq;
            this.toDateReq = toDateReq;
        }

        bool ILoad.Load(string printTypeCode, string fileName, Inventec.Common.FlexCelPrint.DelegateReturnEventPrint returnEventPrint)
        {
            bool result = false;
            try
            {
                V_HIS_TREATMENT_FEE treatment = this.TreatmentFees.FirstOrDefault(o => o.ID == this.Treatment.ID);

                CommonParam param = new CommonParam();
                List<V_HIS_SERE_SERV> VHisSereServs = new List<V_HIS_SERE_SERV>();

                // LẤY THEO TREATMENT
                if (this.Treatment != null)
                {
                    HisSereServViewFilter ssViewFilter = new HisSereServViewFilter();
                    ssViewFilter.TREATMENT_ID = this.Treatment.ID;

                    // Nếu class filter có sẵn field FROM/TO thì dùng, còn không thì lọc bằng LINQ bên dưới.
                    // Ví dụ (nếu có):
                    // ssViewFilter.TDL_INTRUCTION_TIME_FROM = fromDateReq;
                    // ssViewFilter.TDL_INTRUCTION_TIME_TO   = toDateReq;

                    VHisSereServs = new Inventec.Common.Adapter.BackendAdapter(param)
                        .Get<List<MOS.EFMODEL.DataModels.V_HIS_SERE_SERV>>(
                            "api/HisSereServ/GetView",
                            ApiConsumer.ApiConsumers.MosConsumer,
                            ssViewFilter,
                            param
                        );
                }

                if (VHisSereServs == null || VHisSereServs.Count == 0)
                {
                    if (IsActionButtonPrintBill)
                        XtraMessageBox.Show("Không có dịch vụ cần thanh toán!", "Thông báo");
                    return result;
                }

                // LỌC THEO KHOẢNG THỜI GIAN TDL_INTRUCTION_TIME
                VHisSereServs = VHisSereServs
                    .Where(o => o.TDL_INTRUCTION_TIME >= fromDateReq
                             && o.TDL_INTRUCTION_TIME <= toDateReq)
                    .ToList();

                if (VHisSereServs == null || VHisSereServs.Count == 0)
                {
                    if (IsActionButtonPrintBill)
                        XtraMessageBox.Show("Không có dịch vụ trong khoảng thời gian đã chọn!", "Thông báo");
                    return result;
                }

                // Tạo PDO với danh sách đã lọc + khoảng thời gian
                Mps000504PDO rdo = new Mps000504PDO(treatment, VHisSereServs, fromDateReq, toDateReq);

                #region Run Print
                PrintCustomShow<Mps000504PDO> printShow = new PrintCustomShow<Mps000504PDO>(
                    printTypeCode,
                    fileName,
                    rdo,
                    returnEventPrint,
                    this.isPreview
                );
                result = printShow.SignRun(Treatment.TREATMENT_CODE, this.RoomId);
                #endregion
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

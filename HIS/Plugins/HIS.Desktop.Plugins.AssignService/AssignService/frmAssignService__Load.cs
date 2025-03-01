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
using DevExpress.Data;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.DXErrorProvider;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.BackendData.ADO;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.AssignService.ADO;
using HIS.Desktop.Plugins.AssignService.Config;
using HIS.Desktop.Plugins.AssignService.Resources;
using HIS.Desktop.Plugins.Library.AlertWarningFee;
using HIS.Desktop.Utilities.Extensions;
using HIS.Desktop.Utility;
using HIS.UC.DateEditor.ADO;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using System.Threading.Tasks;
using System.Windows.Forms;
using HIS.Desktop.IsAdmin;
using DevExpress.XtraPrinting.Native;
using HIS.Desktop.Controls.Session;
using HIS.UC.Icd.ADO;
using HIS.UC.SecondaryIcd.ADO;
using System.Drawing;

namespace HIS.Desktop.Plugins.AssignService.AssignService
{
    public partial class frmAssignService : HIS.Desktop.Utility.FormBase
    {
        private void LoadDataServiceReqById(long id)
        {
            try
            {
                CommonParam paramCommon = new CommonParam();
                HisServiceReqFilter filter = new HisServiceReqFilter();
                filter.ID = id;
                filter.ColumnParams = new List<string>()
                    {
                        "ID",
                        "IS_EMERGENCY",
                        "IS_NOT_USE_BHYT",
                        "IS_NOT_REQUIRE_FEE",
                        "SERVICE_REQ_CODE",
                        "IS_MAIN_EXAM",
                        "IS_ANTIBIOTIC_RESISTANCE",
                        "TDL_PATIENT_TYPE_ID"
                    };
                filter.ColumnParams = filter.ColumnParams.Distinct().ToList();
                var serviceReqs = new Inventec.Common.Adapter.BackendAdapter(paramCommon).Get<List<HIS_SERVICE_REQ>>((RequestUriStore.HIS_SERVICE_REQ_GET_DYNAMIC), ApiConsumers.MosConsumer, filter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, paramCommon);
                if (!this.workingAssignServiceADO.IsNotUseBhyt.HasValue)
                {
                    if (serviceReqs != null && serviceReqs.Count > 0)
                    {
                        this.serviceReqMain = serviceReqs.FirstOrDefault();

                        this.isNotUseBhyt = this.serviceReqMain != null ? (this.serviceReqMain.IS_NOT_USE_BHYT.HasValue && this.serviceReqMain.IS_NOT_USE_BHYT == 1) : false;

                    }
                }
                Inventec.Common.Logging.LogSystem.Debug("LoadDataServiceReqById. 2");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void LoadTotalSereServByHeinWithTreatment(long treatmentId)
        {
            try
            {
                CommonParam param = new CommonParam();
                HisSereServFilter hisSereServFilter = new HisSereServFilter();
                hisSereServFilter.TREATMENT_ID = treatmentId;
                //hisSereServFilter.PATIENT_TYPE_ID = HisConfigCFG.PatientTypeId__BHYT;
                this.sereServsInTreatmentRaw = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.HIS_SERE_SERV>>(HisRequestUriStore.HIS_SERE_SERV_GET, ApiConsumers.MosConsumer, hisSereServFilter, param);
                this.sereServsInTreatment = this.sereServsInTreatmentRaw != null ? this.sereServsInTreatmentRaw.Where(o => o.PATIENT_TYPE_ID == HisConfigCFG.PatientTypeId__BHYT).ToList() : null;

                this.totalHeinByTreatment = this.sereServsInTreatment != null && this.sereServsInTreatment.Count > 0 ? this.sereServsInTreatment.Sum(o => o.VIR_TOTAL_PRICE_NO_ADD_PRICE ?? 0) : 0;
                this.totalHeinPriceByTreatment = this.sereServsInTreatment.Sum(o => o.VIR_TOTAL_HEIN_PRICE ?? 0);
                if (this.totalHeinPriceByTreatment > 0 && this.totalHeinPriceByTreatmentBK != this.totalHeinPriceByTreatment)
                {
                    string messageErr = "";

                    AlertWarningFeeManager alertWarningFeeManager = new AlertWarningFeeManager();
                    if (!alertWarningFeeManager.RunOption(treatmentId, currentHisPatientTypeAlter.PATIENT_TYPE_ID, currentHisPatientTypeAlter.TREATMENT_TYPE_ID, currentHisPatientTypeAlter.HEIN_MEDI_ORG_CODE, HisConfigCFG.PatientTypeId__BHYT, totalHeinPriceByTreatment, HisConfigCFG.IsUsingWarningHeinFee, 0, ref messageErr, true))
                    {
                        this.Close();
                    }
                }
                this.totalHeinPriceByTreatmentBK = this.totalHeinPriceByTreatment;
                this.LoadIcdDefault();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private async Task LoadTotalSereServByHeinWithTreatmentAsync(long treatmentId)
        {
            try
            {
                DateTime intructTime = (Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(this.intructionTimeSelecteds.First()) ?? DateTime.Now);

                CommonParam param = new CommonParam();
                HisSereServFilter hisSereServFilter = new HisSereServFilter();
                hisSereServFilter.TREATMENT_ID = treatmentId;
                //hisSereServFilter.PATIENT_TYPE_ID = HisConfigCFG.PatientTypeId__BHYT;
                this.sereServsInTreatmentRaw = await new BackendAdapter(param).GetAsync<List<MOS.EFMODEL.DataModels.HIS_SERE_SERV>>(HisRequestUriStore.HIS_SERE_SERV_GET, ApiConsumers.MosConsumer, hisSereServFilter, param);
                this.sereServsInTreatment = this.sereServsInTreatmentRaw != null ? this.sereServsInTreatmentRaw.Where(o => o.PATIENT_TYPE_ID == HisConfigCFG.PatientTypeId__BHYT).ToList() : null;

                this.totalHeinByTreatment = this.sereServsInTreatment != null && this.sereServsInTreatment.Count > 0 ? this.sereServsInTreatment.Sum(o => o.VIR_TOTAL_PRICE_NO_ADD_PRICE ?? 0) : 0;
                this.totalHeinPriceByTreatment = this.sereServsInTreatment.Sum(o => o.VIR_TOTAL_HEIN_PRICE ?? 0);

                this.LoadDataSereServWithTreatment(this.currentHisTreatment, intructTime);
                this.LoadIcdDefault();

                if (this.totalHeinPriceByTreatment > 0)
                {
                    string messageErr = "";
                    AlertWarningFeeManager alertWarningFeeManager = new AlertWarningFeeManager();
                    if (!alertWarningFeeManager.RunOption(treatmentId, currentHisPatientTypeAlter.PATIENT_TYPE_ID, currentHisPatientTypeAlter.TREATMENT_TYPE_ID, currentHisPatientTypeAlter.HEIN_MEDI_ORG_CODE, HisConfigCFG.PatientTypeId__BHYT, totalHeinPriceByTreatment, HisConfigCFG.IsUsingWarningHeinFee, 0, ref messageErr, true))
                    {
                        this.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Mong muốn có cấu hình cài đặt mức trần BHYT cho 3 loại đối tượng BN này (Khám bệnh, Ngoại trú, Nội trú), mức trần này là tổng chi phí BHYT của bệnh nhân.
        ///- Với BN khám bệnh sẽ áp dụng với mức trần Khám bệnh (các BN được chỉ định trực tiếp tại tiếp đón cũng được áp dụng trong trường hợp này)
        ///- Với BN điều trị ngoại trú sẽ áp dụng mức trần Ngoại trú (tính cả chi phí BHYT từ PK)
        ///- Với BN điều trị nội trú sẽ áp dụng mức trần Nội trú (tính cả chi phí BHYT từ PK và cả điều trị ngoại trú trong trường hợp điều trị kết hợp).
        ///Chú ý:
        ///+ Với BN đang điều trị kết hợp giữa nội trú và ngoại trú, khi được chỉ định tại khoa điều trị Ngoại trú thì cũng phải áp dụng mức trần đã cài đặt đối với Nội trú.
        ///+ Với Bn được chỉ định tại các bộ phận CLS thì áp dụng với loại bệnh án tương ứng (với trường hợp BN đang điều trị kết hợp Nội trú và ngoại trú --> khoa ngoại trú chỉ định CLS --> phòng CLS chỉ định --> áp dụng mức trần của Nội trú).
        ///+ Chức năng cảnh báo áp dụng cả với lúc chuyển đối tượng dịch vụ và chuyển đối tượng BN từ đối tượng thu phí sang BHYT.
        /// </summary>
        private void GetSereServForBill()
        {
            try
            {

            }
            catch (Exception)
            {

                throw;
            }
        }
        private async Task LoadSampleType()
        {
            try
            {
                dataListTestSampleType = BackendDataWorker.Get<HIS_TEST_SAMPLE_TYPE>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
                InitComboRepositoryTestSampleType(dataListTestSampleType);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private async Task LoadDataToCashierRoom()
        {
            try
            {
                List<V_HIS_CASHIER_ROOM> cashierRooms;
                if (WorkPlace.GetRoomIds() != null && WorkPlace.GetRoomIds().Count > 0)
                {
                    if (!BackendDataWorker.IsExistsKey<V_HIS_CASHIER_ROOM>())
                    {
                        CommonParam paramCommon = new CommonParam();
                        MOS.Filter.HisPatientTypeFilter filter = new MOS.Filter.HisPatientTypeFilter();
                        cashierRooms = await new Inventec.Common.Adapter.BackendAdapter(paramCommon).GetAsync<List<MOS.EFMODEL.DataModels.V_HIS_CASHIER_ROOM>>("api/HisCashierRoom/GetView", ApiConsumers.MosConsumer, filter, paramCommon);

                        if (cashierRooms != null) BackendDataWorker.UpdateToRam(typeof(MOS.EFMODEL.DataModels.V_HIS_CASHIER_ROOM), cashierRooms, long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss")));
                    }

                    cashierRooms = BackendDataWorker.Get<V_HIS_CASHIER_ROOM>().Where(o => WorkPlace.GetRoomIds().Contains(o.ROOM_ID)).ToList();
                }
                else
                {
                    cashierRooms = new List<V_HIS_CASHIER_ROOM>();
                }
                cboCashierRoom.Properties.DataSource = cashierRooms;
                cboCashierRoom.Properties.DisplayMember = "CASHIER_ROOM_NAME";
                cboCashierRoom.Properties.ValueMember = "ID";
                cboCashierRoom.Properties.ForceInitialize();
                cboCashierRoom.Properties.Columns.Clear();
                cboCashierRoom.Properties.Columns.Add(new LookUpColumnInfo("CASHIER_ROOM_CODE", "", 50));
                cboCashierRoom.Properties.Columns.Add(new LookUpColumnInfo("CASHIER_ROOM_NAME", "", 200));
                cboCashierRoom.Properties.ShowHeader = false;
                cboCashierRoom.Properties.ImmediatePopup = true;
                cboCashierRoom.Properties.DropDownRows = 10;
                cboCashierRoom.Properties.PopupWidth = 250;
                // đặt giá trị mặc định cho phòng thu ngân
                if (cashierRooms != null && cashierRooms.Count > 0)
                {
                    cboCashierRoom.EditValue = cashierRooms.FirstOrDefault().ID;
                }
                else
                {
                    cboCashierRoom.EditValue = null;
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Bổ sung cấu hình hệ thống: "HIS.Desktop.Plugins.AssignService.SetRequestRoomByBedRoomWhenBeingInSurgery": "1: Tự động điền phòng chỉ định theo buồng bệnh nhân đang nằm nếu chỉ định DVKT ở màn hình xử lý PTTT"
        ///  Giá trị mặc định hiển thị:
        ///+ Theo buồng của BN đang nằm (Lấy theo bản ghi his_treatment_bed_room tương ứng với hồ sơ điều trị và có remove_time null và có add_time lớn nhất)
        ///+ Nếu ko có bản ghi thông tin buồng BN đang nằm thì hiển thị mặc định theo phòng mà người dùng đang làm việc.
        /// </summary>
        /// <returns></returns>
        private async Task LoadDataToAssignRoom()
        {
            try
            {
                bool isDefaultByWorkingRoom = true;
                List<V_HIS_ROOM> assRooms = BackendDataWorker.Get<V_HIS_ROOM>();
                this.assRoomsWorks = assRooms != null ? assRooms.Where(o => o.ROOM_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_ROOM_TYPE.ID__BUONG || o.ROOM_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_ROOM_TYPE.ID__XL || o.ROOM_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_ROOM_TYPE.ID__TD).ToList() : null;
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("ROOM_CODE", "", 150, 1));
                columnInfos.Add(new ColumnInfo("ROOM_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("ROOM_NAME", "ID", columnInfos, false, 400);
                ControlEditorLoader.Load(this.cboAssignRoom, assRoomsWorks, controlEditorADO);

                if (this.currentHisTreatment.TDL_TREATMENT_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__KHAM
                    && this.currentHisTreatment.TDL_TREATMENT_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__NHANTHUOC)
                {
                    CommonParam param = new CommonParam();
                    MOS.Filter.HisTreatmentBedRoomFilter treatmentBedroomFilter = new HisTreatmentBedRoomFilter();
                    treatmentBedroomFilter.TREATMENT_ID = treatmentId;
                    this.TreatmentBedRooms = new BackendAdapter(param).Get<List<V_HIS_TREATMENT_BED_ROOM>>("api/HisTreatmentBedRoom/GetView", ApiConsumer.ApiConsumers.MosConsumer, treatmentBedroomFilter, null);
                    if (this.TreatmentBedRooms != null && this.TreatmentBedRooms.Count > 0)
                    {
                        this.IsTreatmentInBedRoom = true;
                    }
                }

                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => isAssignInPttt), isAssignInPttt) + Inventec.Common.Logging.LogUtil.TraceData("HisConfigCFG.SetRequestRoomByBedRoomWhenBeingInSurgery", HisConfigCFG.SetRequestRoomByBedRoomWhenBeingInSurgery));
                if (isAssignInPttt && HisConfigCFG.SetRequestRoomByBedRoomWhenBeingInSurgery == "1")
                {
                    cboAssignRoom.Enabled = txtAssignRoomCode.Enabled = true;

                    var treatmentBedRooms = this.TreatmentBedRooms != null ? this.TreatmentBedRooms.Where(o => o.REMOVE_TIME == null).ToList() : null;
                    V_HIS_TREATMENT_BED_ROOM treatmentBedRoom = (treatmentBedRooms != null && treatmentBedRooms.Count() > 0) ? treatmentBedRooms.OrderByDescending(o => o.ADD_TIME).FirstOrDefault() : null;
                    if (treatmentBedRoom != null)
                    {
                        isDefaultByWorkingRoom = false;
                        var bedrooms = BackendDataWorker.Get<V_HIS_BED_ROOM>().Where(o => o.ID == treatmentBedRoom.BED_ROOM_ID).ToList();
                        if (bedrooms != null && bedrooms.Count > 0 && assRoomsWorks != null && assRoomsWorks.Count > 0)
                        {
                            var assRoom = assRoomsWorks.FirstOrDefault(o => o.ID == bedrooms.First().ROOM_ID);
                            cboAssignRoom.EditValue = assRoom != null ? (long?)assRoom.ID : null;
                            txtAssignRoomCode.Text = assRoom != null ? assRoom.ROOM_CODE : "";
                        }
                    }
                }
                else if (this.examRegisterRoomId.HasValue && this.examRegisterRoomId > 0)
                {
                    cboAssignRoom.Enabled = txtAssignRoomCode.Enabled = true;//TODO
                    isDefaultByWorkingRoom = false;
                    var assRoom = assRoomsWorks.FirstOrDefault(o => o.ID == this.examRegisterRoomId);
                    cboAssignRoom.EditValue = assRoom != null ? (long?)assRoom.ID : null;
                    txtAssignRoomCode.Text = assRoom != null ? assRoom.ROOM_CODE : "";
                }

                if (isDefaultByWorkingRoom)
                {
                    // đặt giá trị mặc định
                    if (assRoomsWorks != null && assRoomsWorks.Count > 0)
                    {
                        var assRoom = assRoomsWorks.FirstOrDefault(o => o.ID == this.currentModule.RoomId);
                        cboAssignRoom.EditValue = assRoom != null ? (long?)assRoom.ID : null;
                        txtAssignRoomCode.Text = assRoom != null ? assRoom.ROOM_CODE : "";
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void VerifyWarningOverCeiling()
        {

            try
            {
                decimal totalPriceSum = totalHeinByTreatment + GetTotalPriceServiceSelected(HisConfigCFG.PatientTypeId__BHYT);

                decimal warningOverCeiling = (this.currentHisPatientTypeAlter.TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__KHAM ? HisConfigCFG.WarningOverCeiling__Exam : (this.currentHisPatientTypeAlter.TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTNGOAITRU ? HisConfigCFG.WarningOverCeiling__Out : HisConfigCFG.WarningOverCeiling__In));

                bool inValid = (warningOverCeiling > 0 && totalPriceSum > warningOverCeiling);
                if (inValid)
                {
                    MessageManager.Show(String.Format(ResourceMessage.TongTienTheoDoiTuongDieuTriChoBHYTDaVuotquaMucGioiHan, GetTreatmentTypeNameByCode(this.currentHisPatientTypeAlter.TREATMENT_TYPE_CODE), Inventec.Common.Number.Convert.NumberToString(totalPriceSum, 0), Inventec.Common.Number.Convert.NumberToString(warningOverCeiling, 0)));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private bool VerifyCheckFeeWhileAssign(List<ServiceReqDetailSDO> serviceReqDetails = null)
        {
            bool valid = true;
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("VerifyCheckFeeWhileAssign.1");
                this.patientTypeByPT = (currentHisPatientTypeAlter != null && currentHisPatientTypeAlter.PATIENT_TYPE_ID > 0) ? BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE>().Where(o => o.ID == currentHisPatientTypeAlter.PATIENT_TYPE_ID).FirstOrDefault() : null;
                if (this.patientTypeByPT != null && this.patientTypeByPT.IS_CHECK_FEE_WHEN_ASSIGN == 1
                        && this.currentHisPatientTypeAlter.TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__KHAM)
                {
                    decimal totalPriceServiceSelected = GetFullTotalPriceServiceSelected();
                    if (serviceReqDetails != null && serviceReqDetails.Count > 0)
                        foreach (var item in serviceReqDetails)
                        {
                            if (item.ServiceId > 0 && item.PatientTypeId > 0)
                            {
                                if (BranchDataWorker.DicServicePatyInBranch.ContainsKey(item.ServiceId))
                                {
                                    var data_ServicePrice = BranchDataWorker.ServicePatyWithPatientType(item.ServiceId, item.PatientTypeId).OrderByDescending(m => m.PRIORITY).ThenByDescending(m => m.ID).ToList();
                                    if (data_ServicePrice != null && data_ServicePrice.Count > 0)
                                    {
                                        totalPriceServiceSelected += item.Amount * (data_ServicePrice[0].PRICE * (1 + data_ServicePrice[0].VAT_RATIO));
                                    }
                                }
                            }
                        }

                    if (this.isMultiDateState && intructionTimeSelecteds.Count() > 1)
                    {
                        totalPriceServiceSelected = totalPriceServiceSelected * intructionTimeSelecteds.Count();
                    }

                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => transferTreatmentFee), transferTreatmentFee));


                    // - Trong trường hợp ĐỐI TƯỢNG BỆNH NHÂN được check "Không cho phép chỉ định dịch vụ nếu thiếu tiền" (HIS_PATIENT_TYPE có IS_CHECK_FEE_WHEN_ASSIGN = 1) và hồ sơ là "Khám" (HIS_TREATMENT có TDL_TREATMENT_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__KHAM) thì kiểm tra:
                    //+ Nếu hồ sơ đang không thừa tiền "Còn thừa" = 0 hoặc hiển thị "Còn thiếu" thì hiển thị thông báo "Bệnh nhân đang nợ tiền, không cho phép chỉ định dịch vụ", người dùng nhấn "Đồng ý" thì tắt form chỉ định.
                    //+ Nếu hồ sơ đang thừa tiền ("Còn thừa" > 0), thì khi người dùng check chọn dịch vụ, nếu số tiền "Phát sinh" > "Còn thừa" thì hiển thị cảnh báo: "Không cho phép chỉ định dịch vụ vượt quá số tiền còn thừa" và không cho phép người dùng check chọn dịch vụ đó.
                    if (this.patientTypeByPT != null && this.patientTypeByPT.IS_CHECK_FEE_WHEN_ASSIGN == 1
                            && this.currentHisPatientTypeAlter.TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__KHAM
                            && (
                            this.transferTreatmentFee >= 0 ||
                            (this.transferTreatmentFee < 0 && totalPriceServiceSelected > Math.Abs(this.transferTreatmentFee))
                            )
                        && this.currentModule.RoomTypeId != IMSys.DbConfig.HIS_RS.HIS_ROOM_TYPE.ID__TD
                        )
                    {
                        //DialogResult myResult = MessageBox.Show(this, String.Format(ResourceMessage.BenhNhanDangNoTienKhogChoPhepChiDinhDV, Inventec.Common.Number.Convert.NumberToString(this.transferTreatmentFee, ConfigApplications.NumberSeperator)), HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        MessageBox.Show(this, String.Format(ResourceMessage.KhongChoPhepChiDInhDVVuotQuaSoTienCOnThua), HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao));
                        Inventec.Common.Logging.LogSystem.Warn("co cau hinh IS_CHECK_FEE_WHEN_ASSIGN va ke don phong kham ==>" + ResourceMessage.KhongChoPhepChiDInhDVVuotQuaSoTienCOnThua);


                        //if (myResult == DialogResult.Yes)
                        //{

                        valid = false;
                        //}
                        Inventec.Common.Logging.LogSystem.Debug("VerifyCheckFeeWhileAssign.2");
                    }
                    Inventec.Common.Logging.LogSystem.Debug("VerifyCheckFeeWhileAssign.3");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return valid;
        }

        private decimal GetFullTotalPriceServiceSelected()
        {
            decimal totalPrice = 0;
            try
            {
                List<SereServADO> serviceCheckeds__Send = this.ServiceIsleafADOs.FindAll(o => o.IsChecked);
                foreach (var item in serviceCheckeds__Send)
                {
                    if (item.IsChecked
                        && (item.IsExpend ?? false) == false)
                    {
                        if (BranchDataWorker.DicServicePatyInBranch.ContainsKey(item.SERVICE_ID))
                        {
                            var data_ServicePrice = BranchDataWorker.ServicePatyWithPatientType(item.SERVICE_ID, item.PRIMARY_PATIENT_TYPE_ID ?? item.PATIENT_TYPE_ID).OrderByDescending(m => m.PRIORITY).ThenByDescending(m => m.ID).ToList();
                            if (data_ServicePrice != null && data_ServicePrice.Count > 0)
                            {

                                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => data_ServicePrice), data_ServicePrice));
                                totalPrice += item.AMOUNT * (data_ServicePrice[0].PRICE * (1 + data_ServicePrice[0].VAT_RATIO));
                            }
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return totalPrice;
        }

        private string GetTreatmentTypeNameByCode(string code)
        {
            string name = "";
            try
            {
                name = BackendDataWorker.Get<HIS_TREATMENT_TYPE>().FirstOrDefault(o => o.TREATMENT_TYPE_CODE == code).TREATMENT_TYPE_NAME;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return name;
        }

        private void FillDataOtherPaySourceDataRow(SereServADO currentRowSereServADO)
        {
            try
            {
                if (currentRowSereServADO.IsChecked && currentRowSereServADO.PATIENT_TYPE_ID > 0)
                {
                    var dataOtherPaySources = BackendDataWorker.Get<HIS_OTHER_PAY_SOURCE>();
                    List<HIS_OTHER_PAY_SOURCE> dataOtherPaySourceTmps = new List<HIS_OTHER_PAY_SOURCE>();
                    dataOtherPaySources = (dataOtherPaySources != null && dataOtherPaySources.Count > 0) ? dataOtherPaySources.Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList() : null;
                    if (dataOtherPaySources != null && dataOtherPaySources.Count > 0)
                    {
                        var workingPatientType = currentPatientTypes.Where(t => t.ID == currentRowSereServADO.PATIENT_TYPE_ID).FirstOrDefault();

                        if (workingPatientType != null && !String.IsNullOrEmpty(workingPatientType.OTHER_PAY_SOURCE_IDS))
                        {
                            dataOtherPaySourceTmps = dataOtherPaySources.Where(o => ("," + workingPatientType.OTHER_PAY_SOURCE_IDS + ",").Contains("," + o.ID + ",")).ToList();

                            if (currentRowSereServADO.OTHER_PAY_SOURCE_ID == null && dataOtherPaySourceTmps != null && dataOtherPaySourceTmps.Count == 1)
                            {
                                currentRowSereServADO.OTHER_PAY_SOURCE_ID = dataOtherPaySourceTmps[0].ID;
                                currentRowSereServADO.OTHER_PAY_SOURCE_CODE = dataOtherPaySourceTmps[0].OTHER_PAY_SOURCE_CODE;
                                currentRowSereServADO.OTHER_PAY_SOURCE_NAME = dataOtherPaySourceTmps[0].OTHER_PAY_SOURCE_NAME;
                            }
                        }
                        else
                        {
                            dataOtherPaySourceTmps.AddRange(dataOtherPaySources);
                        }

                        if (currentRowSereServADO.OTHER_PAY_SOURCE_ID == null
                            && currentHisTreatment != null && currentHisTreatment.OTHER_PAY_SOURCE_ID.HasValue && currentHisTreatment.OTHER_PAY_SOURCE_ID.Value > 0
                            && dataOtherPaySourceTmps != null && dataOtherPaySourceTmps.Exists(k => k.ID == currentHisTreatment.OTHER_PAY_SOURCE_ID.Value))
                        {
                            var otherPaysourceByTreatment = dataOtherPaySourceTmps.Where(k => k.ID == currentHisTreatment.OTHER_PAY_SOURCE_ID.Value).FirstOrDefault();
                            if (otherPaysourceByTreatment != null)
                            {
                                currentRowSereServADO.OTHER_PAY_SOURCE_ID = otherPaysourceByTreatment.ID;
                                currentRowSereServADO.OTHER_PAY_SOURCE_CODE = otherPaysourceByTreatment.OTHER_PAY_SOURCE_CODE;
                                currentRowSereServADO.OTHER_PAY_SOURCE_NAME = otherPaysourceByTreatment.OTHER_PAY_SOURCE_NAME;
                            }
                        }
                        else if (currentRowSereServADO.OTHER_PAY_SOURCE_ID == null)
                        {
                            HIS.UC.Icd.ADO.IcdInputADO icdData = UcIcdGetValue() as HIS.UC.Icd.ADO.IcdInputADO;
                            var serviceTemp = lstService.Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE && o.ID == currentRowSereServADO.SERVICE_ID).FirstOrDefault();
                            if (serviceTemp != null && serviceTemp.OTHER_PAY_SOURCE_ID.HasValue && dataOtherPaySourceTmps.Exists(k =>
                                k.ID == serviceTemp.OTHER_PAY_SOURCE_ID.Value)
                                && (String.IsNullOrEmpty(serviceTemp.OTHER_PAY_SOURCE_ICDS) || (icdData != null && !String.IsNullOrEmpty(serviceTemp.OTHER_PAY_SOURCE_ICDS) && !String.IsNullOrEmpty(icdData.ICD_CODE) && ("," + serviceTemp.OTHER_PAY_SOURCE_ICDS.ToLower() + ",").Contains("," + icdData.ICD_CODE.ToLower() + ","))))
                            {
                                var otherPaysourceByService = dataOtherPaySourceTmps.Where(k => k.ID == serviceTemp.OTHER_PAY_SOURCE_ID.Value).FirstOrDefault();
                                if (otherPaysourceByService != null)
                                {
                                    currentRowSereServADO.OTHER_PAY_SOURCE_ID = otherPaysourceByService.ID;
                                    currentRowSereServADO.OTHER_PAY_SOURCE_CODE = otherPaysourceByService.OTHER_PAY_SOURCE_CODE;
                                    currentRowSereServADO.OTHER_PAY_SOURCE_NAME = otherPaysourceByService.OTHER_PAY_SOURCE_NAME;
                                }
                            }
                            //Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => serviceTemp), serviceTemp)
                            //    + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => icdData), icdData));
                        }

                        //Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => workingPatientType), workingPatientType)
                        //    + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => dataOtherPaySourceTmps), dataOtherPaySourceTmps)
                        //    + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => currentRowSereServADO.OTHER_PAY_SOURCE_ID), currentRowSereServADO.OTHER_PAY_SOURCE_ID)
                        //    + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => currentRowSereServADO.OTHER_PAY_SOURCE_NAME), currentRowSereServADO.OTHER_PAY_SOURCE_NAME));
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FillDataOtherPaySourceDataRowPatientSelect(SereServADO currentRowSereServADO, long? OtherPaySource, string icdCode)
        {
            try
            {
                if (currentRowSereServADO.IsChecked && currentRowSereServADO.PATIENT_TYPE_ID > 0)
                {
                    var dataOtherPaySources = BackendDataWorker.Get<HIS_OTHER_PAY_SOURCE>();
                    List<HIS_OTHER_PAY_SOURCE> dataOtherPaySourceTmps = new List<HIS_OTHER_PAY_SOURCE>();
                    dataOtherPaySources = (dataOtherPaySources != null && dataOtherPaySources.Count > 0) ? dataOtherPaySources.Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList() : null;
                    if (dataOtherPaySources != null && dataOtherPaySources.Count > 0)
                    {
                        var workingPatientType = currentPatientTypes.Where(t => t.ID == currentRowSereServADO.PATIENT_TYPE_ID).FirstOrDefault();

                        if (workingPatientType != null && !String.IsNullOrEmpty(workingPatientType.OTHER_PAY_SOURCE_IDS))
                        {
                            dataOtherPaySourceTmps = dataOtherPaySources.Where(o => ("," + workingPatientType.OTHER_PAY_SOURCE_IDS + ",").Contains("," + o.ID + ",")).ToList();

                            if (currentRowSereServADO.OTHER_PAY_SOURCE_ID == null && dataOtherPaySourceTmps != null && dataOtherPaySourceTmps.Count == 1)
                            {
                                currentRowSereServADO.OTHER_PAY_SOURCE_ID = dataOtherPaySourceTmps[0].ID;
                                currentRowSereServADO.OTHER_PAY_SOURCE_CODE = dataOtherPaySourceTmps[0].OTHER_PAY_SOURCE_CODE;
                                currentRowSereServADO.OTHER_PAY_SOURCE_NAME = dataOtherPaySourceTmps[0].OTHER_PAY_SOURCE_NAME;
                            }
                        }
                        else
                        {
                            dataOtherPaySourceTmps.AddRange(dataOtherPaySources);
                        }

                        if (currentRowSereServADO.OTHER_PAY_SOURCE_ID == null
                            && OtherPaySource != null
                            && dataOtherPaySourceTmps != null && dataOtherPaySourceTmps.Exists(k => k.ID == OtherPaySource))
                        {
                            var otherPaysourceByTreatment = dataOtherPaySourceTmps.Where(k => k.ID == OtherPaySource).FirstOrDefault();
                            if (otherPaysourceByTreatment != null)
                            {
                                currentRowSereServADO.OTHER_PAY_SOURCE_ID = otherPaysourceByTreatment.ID;
                                currentRowSereServADO.OTHER_PAY_SOURCE_CODE = otherPaysourceByTreatment.OTHER_PAY_SOURCE_CODE;
                                currentRowSereServADO.OTHER_PAY_SOURCE_NAME = otherPaysourceByTreatment.OTHER_PAY_SOURCE_NAME;
                            }
                        }
                        else if (currentRowSereServADO.OTHER_PAY_SOURCE_ID == null)
                        {
                            var serviceTemp = lstService.Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE && o.ID == currentRowSereServADO.SERVICE_ID).FirstOrDefault();
                            if (serviceTemp != null && serviceTemp.OTHER_PAY_SOURCE_ID.HasValue && dataOtherPaySourceTmps.Exists(k =>
                                k.ID == serviceTemp.OTHER_PAY_SOURCE_ID.Value)
                                && (String.IsNullOrEmpty(serviceTemp.OTHER_PAY_SOURCE_ICDS) || (!String.IsNullOrEmpty(icdCode) && !String.IsNullOrEmpty(serviceTemp.OTHER_PAY_SOURCE_ICDS) && ("," + serviceTemp.OTHER_PAY_SOURCE_ICDS.ToLower() + ",").Contains("," + icdCode.ToLower() + ","))))
                            {
                                var otherPaysourceByService = dataOtherPaySourceTmps.Where(k => k.ID == serviceTemp.OTHER_PAY_SOURCE_ID.Value).FirstOrDefault();
                                if (otherPaysourceByService != null)
                                {
                                    currentRowSereServADO.OTHER_PAY_SOURCE_ID = otherPaysourceByService.ID;
                                    currentRowSereServADO.OTHER_PAY_SOURCE_CODE = otherPaysourceByService.OTHER_PAY_SOURCE_CODE;
                                    currentRowSereServADO.OTHER_PAY_SOURCE_NAME = otherPaysourceByService.OTHER_PAY_SOURCE_NAME;
                                }
                            }
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }


        private decimal GetTotalPriceServiceSelected(long patientTypeId)
        {
            decimal totalPrice = 0;
            try
            {
                List<SereServADO> serviceCheckeds__Send = this.ServiceIsleafADOs.FindAll(o => o.IsChecked);
                foreach (var item in serviceCheckeds__Send)
                {
                    if (item.IsChecked
                        && ((patientTypeId > 0 && item.PATIENT_TYPE_ID == patientTypeId) || (patientTypeId <= 0 && item.PATIENT_TYPE_ID > 0))
                        && (item.IsExpend ?? false) == false)
                    {
                        if (BranchDataWorker.DicServicePatyInBranch.ContainsKey(item.SERVICE_ID))
                        {
                            var data_ServicePrice = BranchDataWorker.ServicePatyWithPatientType(item.SERVICE_ID, item.PATIENT_TYPE_ID).OrderByDescending(m => m.PRIORITY).ThenByDescending(m => m.ID).ToList();
                            if (data_ServicePrice != null && data_ServicePrice.Count > 0)
                            {
                                totalPrice += item.AMOUNT * (data_ServicePrice[0].PRICE * (1 + data_ServicePrice[0].VAT_RATIO));
                            }
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return totalPrice;
        }

        private decimal? GetPriceBypackage(SereServADO sereServADOOld)
        {
            decimal? resultData = null;
            try
            {
                if (sereServADOOld.PATIENT_TYPE_ID != 0 && BranchDataWorker.HasServicePatyWithListPatientType(sereServADOOld.SERVICE_ID, this.patientTypeIdAls))
                {
                    if (this.dicServices.ContainsKey(sereServADOOld.SERVICE_ID))
                    {
                        var oneServicePrice = this.dicServices[sereServADOOld.SERVICE_ID];
                        if (oneServicePrice != null)
                        {
                            resultData = (oneServicePrice.PACKAGE_PRICE);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return resultData;
        }

        private decimal? GetPriceBySurg(SereServADO sereServADOOld)
        {
            decimal? resultData = null;
            decimal? heinLimitPrice = null;
            decimal? heinLimitRatio = null;
            try
            {
                long instructionTime = this.intructionTimeSelecteds != null && this.intructionTimeSelecteds.Count > 0 ? this.intructionTimeSelecteds.FirstOrDefault() : 0;
                if (sereServADOOld.PATIENT_TYPE_ID != 0 && BranchDataWorker.DicServicePatyInBranch.ContainsKey(sereServADOOld.SERVICE_ID) && instructionTime > 0)
                {
                    List<MOS.EFMODEL.DataModels.V_HIS_EXECUTE_ROOM> dataCombo = new List<V_HIS_EXECUTE_ROOM>();
                    var serviceRoomViews = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_SERVICE_ROOM>();
                    List<MOS.EFMODEL.DataModels.V_HIS_SERVICE_ROOM> arrExcuteRoomCode = new List<V_HIS_SERVICE_ROOM>();
                    if (sereServADOOld.TDL_EXECUTE_ROOM_ID > 0)
                    {
                        dataCombo = this.allDataExecuteRooms.Where(o => sereServADOOld.TDL_EXECUTE_ROOM_ID == o.ROOM_ID).ToList();
                    }
                    else if (this.allDataExecuteRooms != null && this.allDataExecuteRooms.Count > 0 && serviceRoomViews != null && serviceRoomViews.Count > 0)
                    {
                        arrExcuteRoomCode = serviceRoomViews.Where(o => sereServADOOld != null && o.SERVICE_ID == sereServADOOld.SERVICE_ID).ToList();
                        if (HisConfigCFG.IsAssignRoomByPatientType && PatientTypeRooms != null && PatientTypeRooms.Count > 0 && PatientTypeRooms.Exists(o => o.PATIENT_TYPE_ID == sereServADOOld.PATIENT_TYPE_ID))
                        {
                            var RoomIds = PatientTypeRooms.Where(o => o.PATIENT_TYPE_ID == sereServADOOld.PATIENT_TYPE_ID).Select(o => o.ROOM_ID).ToList();
                            arrExcuteRoomCode = arrExcuteRoomCode.Where(o => RoomIds.Contains(o.ROOM_ID)).ToList();
                        }
                        dataCombo = ((arrExcuteRoomCode != null && arrExcuteRoomCode.Count > 0 && this.allDataExecuteRooms != null) ?
                            this.allDataExecuteRooms.Where(o => arrExcuteRoomCode.Select(p => p.ROOM_ID).Contains(o.ROOM_ID) && o.BRANCH_ID == this.requestRoom.BRANCH_ID).ToList()
                            : null);
                    }

                    var checkExecuteRoom = dataCombo != null && dataCombo.Count > 0 ? dataCombo.FirstOrDefault() : null;
                    if (checkExecuteRoom != null)
                    {
                        sereServADOOld.TDL_EXECUTE_BRANCH_ID = checkExecuteRoom.BRANCH_ID;
                    }
                    else
                    {
                        sereServADOOld.TDL_EXECUTE_BRANCH_ID = dataCombo != null && dataCombo.Count > 0 ? dataCombo.FirstOrDefault().BRANCH_ID : HIS.Desktop.LocalStorage.BackendData.BranchDataWorker.GetCurrentBranchId();
                    }
                    long? intructionNumByType = null;

                    List<HIS_SERE_SERV> sameServiceType = this.sereServWithTreatment != null ? this.sereServWithTreatment.Where(o => o.TDL_SERVICE_TYPE_ID == sereServADOOld.SERVICE_TYPE_ID).ToList() : null;
                    List<HIS_SERE_SERV> sameService = this.sereServWithTreatment != null ? this.sereServWithTreatment.Where(o => o.SERVICE_ID == sereServADOOld.SERVICE_ID).ToList() : null;
                    intructionNumByType = sameServiceType != null ? (long)sameServiceType.Count() + 1 : 1;
                    var intructionNum = sameService != null ? (long)sameService.Count() + 1 : 1;

                    List<V_HIS_SERVICE_PATY> servicePaties = BranchDataWorker.ServicePatyWithListPatientType(sereServADOOld.SERVICE_ID, this.patientTypeIdAls);

                    V_HIS_SERVICE_PATY oneServicePatyPrice = MOS.ServicePaty.ServicePatyUtil.GetApplied(servicePaties, sereServADOOld.TDL_EXECUTE_BRANCH_ID, (sereServADOOld.TDL_EXECUTE_ROOM_ID > 0 ? (long?)sereServADOOld.TDL_EXECUTE_ROOM_ID : null), this.requestRoom.ID, this.requestRoom.DEPARTMENT_ID, instructionTime, this.currentHisTreatment.IN_TIME, sereServADOOld.SERVICE_ID, sereServADOOld.PATIENT_TYPE_ID, intructionNum, intructionNumByType, sereServADOOld.PackagePriceId, sereServADOOld.SERVICE_CONDITION_ID, this.currentHisTreatment.TDL_PATIENT_CLASSIFY_ID, null);

                    if (sereServADOOld.PRIMARY_PATIENT_TYPE_ID.HasValue)
                    {
                        V_HIS_SERVICE_PATY primary = MOS.ServicePaty.ServicePatyUtil.GetApplied(servicePaties, sereServADOOld.TDL_EXECUTE_BRANCH_ID, (sereServADOOld.TDL_EXECUTE_ROOM_ID > 0 ? (long?)sereServADOOld.TDL_EXECUTE_ROOM_ID : null), this.requestRoom.ID, this.requestRoom.DEPARTMENT_ID, instructionTime, this.currentHisTreatment.IN_TIME, sereServADOOld.SERVICE_ID, sereServADOOld.PRIMARY_PATIENT_TYPE_ID.Value, intructionNum, intructionNumByType, sereServADOOld.PackagePriceId, sereServADOOld.SERVICE_CONDITION_ID, this.currentHisTreatment.TDL_PATIENT_CLASSIFY_ID, null);
                        if (oneServicePatyPrice == null || primary == null || (oneServicePatyPrice.PRICE * (1 + oneServicePatyPrice.VAT_RATIO)) >= (primary.PRICE * (1 + primary.VAT_RATIO)))
                        {
                            if (HisConfigCFG.IsSetPrimaryPatientType != "2")
                            {
                                sereServADOOld.PRIMARY_PATIENT_TYPE_ID = null;//TODO
                                sereServADOOld.IsNotChangePrimaryPaty = false;
                            }
                        }
                        oneServicePatyPrice = primary;
                    }

                    if (sereServADOOld.PATIENT_TYPE_ID == HisConfigCFG.PatientTypeId__BHYT
                        && sereServADOOld.IsNoDifference.HasValue
                        && sereServADOOld.IsNoDifference.Value)
                    {
                        this.GetHeinLimitPrice(sereServADOOld, instructionTime, this.currentHisTreatment.IN_TIME, ref heinLimitPrice, ref heinLimitRatio);

                        if (heinLimitPrice.HasValue && heinLimitPrice.Value > 0)
                        {
                            resultData = heinLimitPrice;
                        }
                        else if (heinLimitRatio.HasValue && heinLimitRatio.Value > 0 && oneServicePatyPrice != null)
                        {
                            resultData = (oneServicePatyPrice.PRICE * (1 + oneServicePatyPrice.VAT_RATIO) * heinLimitRatio.Value);
                        }
                        else if (oneServicePatyPrice != null)
                        {
                            resultData = (oneServicePatyPrice.PRICE * (1 + oneServicePatyPrice.VAT_RATIO));
                        }
                    }
                    else if (oneServicePatyPrice != null)
                    {
                        resultData = (oneServicePatyPrice.PRICE * (1 + oneServicePatyPrice.VAT_RATIO));
                    }
                }
                else
                {
                    resultData = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return resultData;
        }

        private decimal? GetHeinLimitPriceByDataRow(SereServADO sereServADOOld)
        {
            decimal? resultData = null;
            decimal? heinLimitPrice = null;
            decimal? heinLimitRatio = null;
            try
            {
                long instructionTime = this.intructionTimeSelecteds != null && this.intructionTimeSelecteds.Count > 0 ? this.intructionTimeSelecteds.FirstOrDefault() : 0;

                this.GetHeinLimitPrice(sereServADOOld, instructionTime, this.currentHisTreatment.IN_TIME, ref heinLimitPrice, ref heinLimitRatio);

                if (heinLimitPrice.HasValue && heinLimitPrice.Value > 0)
                {
                    resultData = heinLimitPrice;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return resultData;
        }

        private void GetHeinLimitPrice(SereServADO hisService, long instructionTime, long inTime, ref decimal? heinLimitPrice, ref decimal? heinLimitRatio)
        {
            //neu dich vu khai bao gia tran
            if (hisService.HEIN_LIMIT_PRICE.HasValue || hisService.HEIN_LIMIT_PRICE_OLD.HasValue)
            {
                //neu gia ap dung theo ngay vao vien, thi cac benh nhan vao vien truoc ngay ap dung se lay gia cu
                if (hisService.HEIN_LIMIT_PRICE_IN_TIME.HasValue)
                {
                    heinLimitPrice = inTime < hisService.HEIN_LIMIT_PRICE_IN_TIME.Value ? hisService.HEIN_LIMIT_PRICE_OLD : hisService.HEIN_LIMIT_PRICE;
                }
                //neu ap dung theo ngay chi dinh, thi cac chi dinh truoc ngay ap dung se tinh gia cu
                else if (hisService.HEIN_LIMIT_PRICE_INTR_TIME.HasValue)
                {
                    heinLimitPrice = instructionTime < hisService.HEIN_LIMIT_PRICE_INTR_TIME.Value ? hisService.HEIN_LIMIT_PRICE_OLD : hisService.HEIN_LIMIT_PRICE;
                }
                //neu ca 2 truong ko co gia tri thi luon lay theo gia moi
                else
                {
                    heinLimitPrice = hisService.HEIN_LIMIT_PRICE;
                }
            }
        }

        private void UpdateSurgPrice(SereServADO data)
        {
            try
            {
                if (this.gridViewServiceProcess.IsEditing)
                    this.gridViewServiceProcess.CloseEditor();

                if (this.gridViewServiceProcess.FocusedRowModified)
                    this.gridViewServiceProcess.UpdateCurrentRow();

                this.gridControlServiceProcess.RefreshDataSource();

                this.SetDefaultSerServTotalPrice();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void UpdatePackagePrice(SereServADO data)
        {
            try
            {
                if (this.gridViewServiceProcess.IsEditing)
                    this.gridViewServiceProcess.CloseEditor();

                if (this.gridViewServiceProcess.FocusedRowModified)
                    this.gridViewServiceProcess.UpdateCurrentRow();

                this.gridControlServiceProcess.RefreshDataSource();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private bool IsAllowShowEditPrice(int rowHandle)
        {
            bool valid = true;
            try
            {
                long patientTypeId = Inventec.Common.TypeConvert.Parse.ToInt64((gridViewServiceProcess.GetRowCellValue(rowHandle, "PATIENT_TYPE_ID") ?? "0").ToString());
                long primaryPatientTypeId = Inventec.Common.TypeConvert.Parse.ToInt64((gridViewServiceProcess.GetRowCellValue(rowHandle, "PRIMARY_PATIENT_TYPE_ID") ?? "0").ToString());
                if (patientTypeId == HisConfigCFG.PatientTypeId__BHYT || primaryPatientTypeId == HisConfigCFG.PatientTypeId__BHYT)
                {
                    valid = false;
                }
            }
            catch (Exception ex)
            {
                valid = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return valid;
        }

        private bool IsAllowEditSurgeryPrice(int rowHandle)
        {
            bool valid = false;
            try
            {
                string strIsChecked = (gridViewServiceProcess.GetRowCellValue(rowHandle, "IsChecked") ?? "0").ToString();
                bool IsChecked = (strIsChecked == "1" || strIsChecked.ToLower() == "true");
                long packageId = Inventec.Common.TypeConvert.Parse.ToInt64((gridViewServiceProcess.GetRowCellValue(rowHandle, "PACKAGE_ID") ?? "0").ToString());
                if (IsChecked && packageId > 0 && this.currentDepartment != null && this.currentDepartment.ALLOW_ASSIGN_PACKAGE_PRICE == 1)
                {
                    valid = false;
                }
                else
                {
                    long serviceTypeId = Inventec.Common.TypeConvert.Parse.ToInt64((gridViewServiceProcess.GetRowCellValue(rowHandle, "TDL_SERVICE_TYPE_ID") ?? "0").ToString());
                    if (IsChecked && serviceTypeId == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__PT && this.currentDepartment != null && this.currentDepartment.ALLOW_ASSIGN_SURGERY_PRICE == 1)
                    {
                        valid = true;
                    }
                }
            }
            catch (Exception ex)
            {
                valid = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return valid;
        }

        private bool IsAllowEditPackagePrice(int rowHandle)
        {
            bool valid = false;
            try
            {
                string strIsChecked = (gridViewServiceProcess.GetRowCellValue(rowHandle, "IsChecked") ?? "0").ToString();
                bool IsChecked = (strIsChecked == "1" || strIsChecked.ToLower() == "true");
                long packageId = Inventec.Common.TypeConvert.Parse.ToInt64((gridViewServiceProcess.GetRowCellValue(rowHandle, "PACKAGE_ID") ?? "0").ToString());

                if (IsChecked && packageId > 0 && this.currentDepartment != null && this.currentDepartment.ALLOW_ASSIGN_PACKAGE_PRICE == 1)
                {
                    valid = true;
                }
            }
            catch (Exception ex)
            {
                valid = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return valid;
        }

        private async Task LoadDataDhst()
        {
            try
            {
                if (this.currentDhst == null)
                {
                    CommonParam param = new CommonParam();
                    HisDhstFilter dhstFilter = new HisDhstFilter();
                    dhstFilter.TREATMENT_ID = this.treatmentId;
                    dhstFilter.ORDER_FIELD = "EXECUTE_TIME";
                    dhstFilter.ORDER_DIRECTION = "DESC";
                    currentDhst = new HIS_DHST();
                    var listDHST = await new BackendAdapter(param)
                                    .GetAsync<List<MOS.EFMODEL.DataModels.HIS_DHST>>("api/HisDHST/Get", ApiConsumers.MosConsumer, dhstFilter, param);
                    currentDhst = listDHST != null ? listDHST.FirstOrDefault() : null;
                }

                lblWeight.Text = currentDhst != null && currentDhst.WEIGHT.HasValue ? currentDhst.WEIGHT + "" : "";
                lblHeight.Text = currentDhst != null && currentDhst.HEIGHT.HasValue ? currentDhst.HEIGHT + "" : "";
                lblBMI.Text = currentDhst != null ? FillDataToBmiArea(currentDhst) : "";
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private string FillDataToBmiArea(HIS_DHST currentDhst)
        {
            string rs = "";
            try
            {
                if (currentDhst != null && currentDhst.WEIGHT.HasValue && currentDhst.HEIGHT.HasValue)
                {
                    string s = "", bmiDisplay = "";
                    decimal bmi = 0;
                    if (currentDhst.WEIGHT != null && currentDhst.HEIGHT != 0)
                    {
                        bmi = (currentDhst.WEIGHT.Value) / ((currentDhst.HEIGHT.Value / 100) * (currentDhst.HEIGHT.Value / 100));
                    }
                    //double leatherArea = 0.007184 * Math.Pow((double)currentDhst.HEIGHT.Value, 0.725) * Math.Pow((double)currentDhst.WEIGHT.Value, 0.425);
                    s = Math.Round(bmi, 2) + "";
                    //lblLeatherArea.Text = Math.Round(leatherArea, 2) + "";
                    if (bmi < 16)
                    {
                        bmiDisplay = Inventec.Common.Resource.Get.Value("UCDHST.SKINNY.III", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                    }
                    else if (16 <= bmi && bmi < 17)
                    {
                        bmiDisplay = Inventec.Common.Resource.Get.Value("UCDHST.SKINNY.II", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                    }
                    else if (17 <= bmi && bmi < (decimal)18.5)
                    {
                        bmiDisplay = Inventec.Common.Resource.Get.Value("UCDHST.SKINNY.I", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                    }
                    else if ((decimal)18.5 <= bmi && bmi < 25)
                    {
                        bmiDisplay = Inventec.Common.Resource.Get.Value("UCDHST.BMIDISPLAY.NORMAL", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                    }
                    else if (25 <= bmi && bmi < 30)
                    {
                        bmiDisplay = Inventec.Common.Resource.Get.Value("UCDHST.BMIDISPLAY.OVERWEIGHT", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                    }
                    else if (30 <= bmi && bmi < 35)
                    {
                        bmiDisplay = Inventec.Common.Resource.Get.Value("UCDHST.BMIDISPLAY.OBESITY.I", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                    }
                    else if (35 <= bmi && bmi < 40)
                    {
                        bmiDisplay = Inventec.Common.Resource.Get.Value("UCDHST.BMIDISPLAY.OBESITY.II", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                    }
                    else if (40 < bmi)
                    {
                        bmiDisplay = Inventec.Common.Resource.Get.Value("UCDHST.BMIDISPLAY.OBESITY.III", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                    }
                    rs = s + "  " + bmiDisplay;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return rs;
        }

        private long GetSereServInKip()
        {
            long result = 0;
            try
            {
                if (this.currentSereServ != null)
                    result = this.currentSereServ.ID;

                if (this.currentSereServInEkip != null)
                    result = this.currentSereServInEkip.ID;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private void ValidServiceDetailProcessing(SereServADO sereServADO, bool isValidExecuteRoom)
        {
            try
            {
                if (sereServADO != null)
                {
                    if (HisConfigCFG.IsSetPrimaryPatientType != "2" || sereServADO.ErrorTypePatientTypeId == ErrorType.None)
                    {
                        bool vlPatientTypeId = (sereServADO.IsChecked && sereServADO.PATIENT_TYPE_ID <= 0);
                        sereServADO.ErrorMessagePatientTypeId = (vlPatientTypeId ? Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.ThieuTruongDuLieuBatBuoc) : "");
                        sereServADO.ErrorTypePatientTypeId = (vlPatientTypeId ? ErrorType.Warning : ErrorType.None);
                    }

                    bool vlAmount = (sereServADO.IsChecked && sereServADO.AMOUNT <= 0);
                    sereServADO.ErrorMessageAmount = (vlAmount ? Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.ThieuTruongDuLieuBatBuoc) : "");
                    sereServADO.ErrorTypeAmount = (vlAmount ? ErrorType.Warning : ErrorType.None);

                    List<HIS_SERE_SERV> serviceSames = null;
                    List<SereServADO> serviceSameResult = null;
                    CheckServiceSameByServiceId(sereServADO, this.currentServiceSames, ref serviceSames, ref serviceSameResult);
                    var existsSereServInDate = this.sereServWithTreatment.Any(o => o.SERVICE_ID == sereServADO.SERVICE_ID && o.TDL_INTRUCTION_TIME.ToString().Substring(0, 8) == intructionTimeSelecteds.First().ToString().Substring(0, 8));

                    if (existsSereServInDate && (serviceSames != null && serviceSames.Count > 0))
                    {
                        sereServADO.ErrorMessageIsAssignDay = String.Format(ResourceMessage.CanhBaoDichVuVaDichVuCungCoCheDaChiDinhTrongNgay, string.Join("; ", serviceSames.Select(o => o.TDL_SERVICE_NAME).ToArray()));
                        sereServADO.ErrorTypeIsAssignDay = ErrorType.Warning;
                    }
                    else if (existsSereServInDate)
                    {
                        sereServADO.ErrorMessageIsAssignDay = (existsSereServInDate ? ResourceMessage.CanhBaoDichVuDaChiDinhTrongNgay : "");
                        sereServADO.ErrorTypeIsAssignDay = (existsSereServInDate ? ErrorType.Warning : ErrorType.None);
                    }
                    else if (serviceSames != null && serviceSames.Count > 0)
                    {
                        sereServADO.ErrorMessageIsAssignDay = String.Format(ResourceMessage.CanhBaoDichVuCungCoCheDaChiDinhTrongNgay, string.Join("; ", serviceSames.Select(o => o.TDL_SERVICE_NAME).ToArray()));
                        sereServADO.ErrorTypeIsAssignDay = ErrorType.Warning;
                    }
                    else if (serviceSameResult != null && serviceSameResult.Count > 0)
                    {
                        sereServADO.ErrorMessageIsAssignDay = String.Format(ResourceMessage.CanhBaoDichVuCungCoChe, string.Join("; ", serviceSameResult.Select(o => o.TDL_SERVICE_NAME).ToArray()));
                        sereServADO.ErrorTypeIsAssignDay = ErrorType.Warning;
                    }
                    else
                    {
                        sereServADO.ErrorMessageIsAssignDay = "";
                        sereServADO.ErrorTypeIsAssignDay = ErrorType.None;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void CheckServiceSameByServiceId(SereServADO sereServADO, List<V_HIS_SERVICE_SAME> serviceSameAll, ref List<HIS_SERE_SERV> result, ref List<SereServADO> resultSelect)
        {
            try
            {
                result = null;
                resultSelect = null;
                if (sereServADO != null && serviceSameAll != null && serviceSameAll.Count > 0)
                {
                    //Lay ra cac dich vu cung co che voi dich vu dang duoc chon

                    //Lay cac dich vu cung co che voi no
                    List<long> serviceSameId1s = serviceSameAll
                        .Where(o => o.SERVICE_ID == sereServADO.SERVICE_ID && o.SAME_ID != sereServADO.SERVICE_ID)
                        .Select(o => o.SAME_ID).ToList();
                    //Hoac cac dich vu ma no cung co che
                    List<long> serviceSameId2s = serviceSameAll
                        .Where(o => o.SAME_ID == sereServADO.SERVICE_ID && o.SERVICE_ID != sereServADO.SERVICE_ID)
                        .Select(o => o.SERVICE_ID).ToList();

                    List<long> serviceSameIds = new List<long>();

                    if (serviceSameId1s != null)
                    {
                        serviceSameIds.AddRange(serviceSameId1s);
                    }
                    if (serviceSameId2s != null)
                    {
                        serviceSameIds.AddRange(serviceSameId2s);
                    }
                    result = new List<HIS_SERE_SERV>();

                    if (this.sereServWithTreatment != null && this.sereServWithTreatment.Count() > 0)
                    {

                        long intructionTimeFrom = 0, intructionTimeTo = 0;
                        DateTime itime = (Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(this.intructionTimeSelecteds.First()) ?? DateTime.Now);

                        if (itime != null && itime != DateTime.MinValue)
                        {
                            intructionTimeFrom = Inventec.Common.TypeConvert.Parse.ToInt64(itime.ToString("yyyyMMdd") + "000000");
                            intructionTimeTo = Inventec.Common.TypeConvert.Parse.ToInt64(itime.ToString("yyyyMMdd") + "235959");
                        }
                        else
                        {
                            intructionTimeFrom = (Inventec.Common.DateTime.Get.StartDay() ?? 0);
                            intructionTimeTo = (Inventec.Common.DateTime.Get.EndDay() ?? 0);
                        }

                        var checkServiceSame = this.sereServWithTreatment.Where(o => (intructionTimeFrom <= o.TDL_INTRUCTION_TIME && o.TDL_INTRUCTION_TIME <= intructionTimeTo) && serviceSameIds.Contains(o.SERVICE_ID));

                        if (checkServiceSame != null && checkServiceSame.Count() > 0)
                        {
                            var groupServiceSame = checkServiceSame.GroupBy(o => o.SERVICE_ID).ToList();
                            foreach (var serviceSameItems in groupServiceSame)
                            {
                                result.Add(serviceSameItems.FirstOrDefault());
                            }
                        }
                        else
                        {
                            result = null;
                        }
                    }

                    List<SereServADO> serviceCheckeds__Send = this.ServiceIsleafADOs.FindAll(o => o.IsChecked);
                    if (serviceCheckeds__Send != null && serviceCheckeds__Send.Count > 0)
                    {
                        var checkServiceSame = serviceCheckeds__Send.Where(o => serviceSameIds.Contains(o.SERVICE_ID));
                        resultSelect = new List<SereServADO>();
                        if (checkServiceSame != null && checkServiceSame.Count() > 0)
                        {
                            var groupServiceSame = checkServiceSame.GroupBy(o => o.SERVICE_ID).ToList();
                            foreach (var serviceSameItems in groupServiceSame)
                            {
                                resultSelect.Add(serviceSameItems.FirstOrDefault());
                            }
                        }
                        else
                        {
                            resultSelect = null;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidServiceDetailProcessing(SereServADO sereServADO)
        {
            try
            {
                this.ValidServiceDetailProcessing(sereServADO, false);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private decimal GetDefaultSerServTotalPrice(ref decimal totalPrimaryPatientType, long? patientTypeId = null)
        {
            decimal totalPrice = 0;
            try
            {
                long instructionTime = this.intructionTimeSelecteds != null && this.intructionTimeSelecteds.Count > 0 ? this.intructionTimeSelecteds.FirstOrDefault() : 0;

                if (ServiceIsleafADOs != null && ServiceIsleafADOs.Count > 0)
                {
                    var dataCheckeds = ServiceIsleafADOs.Where(o => o.IsChecked).ToList();
                    if (patientTypeId.HasValue && patientTypeId.Value > 0)
                        dataCheckeds = dataCheckeds.Where(o => o.PATIENT_TYPE_ID == patientTypeId.Value).ToList();
                    if (dataCheckeds != null && dataCheckeds.Count > 0)
                    {
                        var serviceRoomViews = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_SERVICE_ROOM>();
                        foreach (var item in dataCheckeds)
                        {
                            if (item.IsChecked && item.PATIENT_TYPE_ID != 0 && (item.IsExpend ?? false) == false)
                            {
                                var servicePaties = HIS.Desktop.LocalStorage.BackendData.BranchDataWorker.ServicePatyWithListPatientType(item.SERVICE_ID, this.patientTypeIdAls);
                                V_HIS_SERVICE_PATY data_ServicePrice = null;
                                if (servicePaties != null && servicePaties.Count > 0 && this.requestRoom != null)
                                {
                                    List<MOS.EFMODEL.DataModels.V_HIS_EXECUTE_ROOM> dataCombo = new List<V_HIS_EXECUTE_ROOM>();

                                    if (this.allDataExecuteRooms != null && this.allDataExecuteRooms.Count > 0 && serviceRoomViews != null && serviceRoomViews.Count > 0)
                                    {
                                        var arrExcuteRoom = serviceRoomViews.Where(o => item != null && o.SERVICE_ID == item.SERVICE_ID);

                                        if (HisConfigCFG.IsAssignRoomByPatientType && PatientTypeRooms != null && PatientTypeRooms.Count > 0 && PatientTypeRooms.Exists(o => o.PATIENT_TYPE_ID == item.PATIENT_TYPE_ID))
                                        {
                                            var RoomIds = PatientTypeRooms.Where(o => o.PATIENT_TYPE_ID == item.PATIENT_TYPE_ID).Select(o => o.ROOM_ID).ToList();
                                            arrExcuteRoom = arrExcuteRoom.Where(o => RoomIds.Contains(o.ROOM_ID)).ToList();
                                        }
                                        var arrExcuteRoomCode = arrExcuteRoom.Select(o => o.ROOM_ID).ToArray(); 
                                        dataCombo = ((arrExcuteRoomCode != null && arrExcuteRoomCode.Count() > 0 && this.allDataExecuteRooms != null) ? this.allDataExecuteRooms.Where(o => arrExcuteRoomCode.Contains(o.ROOM_ID)).ToList() : null);
                                    }
                                    var checkExecuteRoom = dataCombo != null && dataCombo.Count > 0 ? dataCombo.FirstOrDefault(o => o.BRANCH_ID == this.requestRoom.BRANCH_ID) : null;
                                    if (checkExecuteRoom != null)
                                    {
                                        item.TDL_EXECUTE_BRANCH_ID = checkExecuteRoom.BRANCH_ID;
                                    }
                                    else
                                    {
                                        item.TDL_EXECUTE_BRANCH_ID = dataCombo != null && dataCombo.Count > 0 ? dataCombo.FirstOrDefault().BRANCH_ID : 0;
                                        item.TDL_EXECUTE_BRANCH_ID = item.TDL_EXECUTE_BRANCH_ID == 0 ? HIS.Desktop.LocalStorage.BackendData.BranchDataWorker.GetCurrentBranchId() : item.TDL_EXECUTE_BRANCH_ID;
                                    }

                                    List<HIS_SERE_SERV> sameServiceType = this.sereServWithTreatment != null ? this.sereServWithTreatment.Where(o => o.TDL_SERVICE_TYPE_ID == item.SERVICE_TYPE_ID).ToList() : null;
                                    List<HIS_SERE_SERV> sameService = this.sereServWithTreatment != null ? this.sereServWithTreatment.Where(o => o.SERVICE_ID == item.SERVICE_ID).ToList() : null;
                                    var intructionNumByType = sameServiceType != null ? (long)sameServiceType.Count() + 1 : 1;
                                    var intructionNum = sameService != null ? (long)sameService.Count() + 1 : 1;
                                    if (HisConfigCFG.IsSetPrimaryPatientType != "0"
                                        && item.PRIMARY_PATIENT_TYPE_ID.HasValue && !patientTypeId.HasValue)
                                    {
                                        data_ServicePrice = MOS.ServicePaty.ServicePatyUtil.GetApplied(servicePaties, item.TDL_EXECUTE_BRANCH_ID, null, this.requestRoom.ID, this.requestRoom.DEPARTMENT_ID, instructionTime, this.currentHisTreatment.IN_TIME, item.SERVICE_ID, item.PRIMARY_PATIENT_TYPE_ID.Value, intructionNum, intructionNumByType, item.PackagePriceId, item.SERVICE_CONDITION_ID, this.currentHisTreatment.TDL_PATIENT_CLASSIFY_ID, null);
                                        if (item.HEIN_LIMIT_RATIO.HasValue
                                            && item.HEIN_LIMIT_RATIO.Value > 0
                                            && data_ServicePrice != null)
                                        {
                                            totalPrimaryPatientType += (item.AMOUNT * data_ServicePrice.PRICE * (1 + data_ServicePrice.VAT_RATIO) * item.HEIN_LIMIT_RATIO.Value);
                                        }
                                        else if (data_ServicePrice != null)
                                        {
                                            totalPrimaryPatientType += (item.AMOUNT * data_ServicePrice.PRICE * (1 + data_ServicePrice.VAT_RATIO));
                                        }
                                    }
                                    else
                                    {
                                        data_ServicePrice = MOS.ServicePaty.ServicePatyUtil.GetApplied(servicePaties, item.TDL_EXECUTE_BRANCH_ID, null, this.requestRoom.ID, this.requestRoom.DEPARTMENT_ID, instructionTime, this.currentHisTreatment.IN_TIME, item.SERVICE_ID, item.PATIENT_TYPE_ID, intructionNum, intructionNumByType, item.PackagePriceId, item.SERVICE_CONDITION_ID, this.currentHisTreatment.TDL_PATIENT_CLASSIFY_ID, null);

                                    }
                                }

                                if (item.AssignSurgPriceEdit.HasValue && item.AssignSurgPriceEdit > 0)
                                {
                                    totalPrice += item.AssignSurgPriceEdit.Value;
                                }
                                else
                                {
                                    if (item.PATIENT_TYPE_ID == HisConfigCFG.PatientTypeId__BHYT
                                            && item.IsNoDifference.HasValue
                                            && item.IsNoDifference.Value)
                                    {
                                        if (item.HEIN_LIMIT_PRICE.HasValue
                                            && item.HEIN_LIMIT_PRICE.Value > 0)
                                        {
                                            totalPrice += item.AMOUNT * item.HEIN_LIMIT_PRICE.Value;
                                        }
                                        else if (item.HEIN_LIMIT_RATIO.HasValue
                                            && item.HEIN_LIMIT_RATIO.Value > 0
                                            && data_ServicePrice != null)
                                        {
                                            totalPrice += (item.AMOUNT * data_ServicePrice.PRICE * (1 + data_ServicePrice.VAT_RATIO) * item.HEIN_LIMIT_RATIO.Value);
                                        }
                                        else if (data_ServicePrice != null)
                                        {
                                            totalPrice += (item.AMOUNT * data_ServicePrice.PRICE * (1 + data_ServicePrice.VAT_RATIO));
                                        }
                                    }
                                    else if (data_ServicePrice != null)
                                    {

                                        totalPrice += (item.AMOUNT * data_ServicePrice.PRICE * (1 + data_ServicePrice.VAT_RATIO));
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return totalPrice;
        }

        private void SetDefaultSerServTotalPrice()
        {
            try
            {
                decimal totalPrimary = 0, tmp = 0;
                decimal totalPrice = GetDefaultSerServTotalPrice(ref totalPrimary);
                this.lblTotalServicePrice.Text = Inventec.Common.Number.Convert.NumberToString(totalPrice, ConfigApplications.NumberSeperator);

                decimal totalPriceBhyt = GetDefaultSerServTotalPrice(ref tmp, HisConfigCFG.PatientTypeId__BHYT);
                decimal totalChecnhBhyt = 0;
                if (totalPrimary > 0 && totalPriceBhyt > 0)
                {
                    totalChecnhBhyt = totalPrimary - totalPriceBhyt;
                }
                this.lblChenhBHYT.Text = Inventec.Common.Number.Convert.NumberToString(totalChecnhBhyt, ConfigApplications.NumberSeperator);
                decimal totalPriceOther = totalPrice - totalPriceBhyt - totalChecnhBhyt;

                this.lblTotalServicePriceBhyt.Text = Inventec.Common.Number.Convert.NumberToString(totalPriceBhyt, ConfigApplications.NumberSeperator);
                this.lblTotalServicePriceOther.Text = Inventec.Common.Number.Convert.NumberToString(totalPriceOther, ConfigApplications.NumberSeperator);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadServicePaty(bool IsPatientSelect = false)
        {
            try
            {
                long[] serviceTypeIdAllows = null;

                serviceTypeIdAllows = new long[12]{IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__CDHA,
                                                            IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__G,
                                                            IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__KH,
                                                            IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__KHAC,
                                                            IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__NS,
                                                            IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__PHCN,
                                                            IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__PT,
                                                            IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__SA,
                                                            IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__TDCN,
                                                            IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__TT,
                                                            IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__XN,
                                                            IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__GPBL
                        };



                var patientTypeAll = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE>().Where(o => o.IS_ACTIVE == 1).ToList();
                List<long> patientTypeIds = new List<long>();
                if (IsPatientSelect)
                {
                    foreach (var item in this.dicPatientType.Values.Select(o => o.Select(p => p.ID).ToList()).ToList())
                    {
                        patientTypeIds.AddRange(item);

                    }
                    patientTypeIds = patientTypeIds.Distinct().ToList();
                }
                else
                {
                    patientTypeIds = this.currentPatientTypeWithPatientTypeAlter.Select(o => o.ID).ToList();
                }


                //var patientTypeIdPlus = patientTypeAll.Where(k => k.BASE_PATIENT_TYPE_ID != null && patientTypeIds.Contains(k.BASE_PATIENT_TYPE_ID.Value)).ToList();
                //if (patientTypeIdPlus != null && patientTypeIdPlus.Count > 0)
                //{
                //    patientTypeIds.AddRange(patientTypeIdPlus.Select(o => o.ID));
                //}

                long intructionTime = this.intructionTimeSelecteds.FirstOrDefault();
                long treatmentTime = this.currentHisTreatment.IN_TIME;

                //Lọc các đối tượng thanh toán không có chính sách giá
                var sety = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_SERVICE_PATY>()
                                        .Where(t => (patientTypeIds.Contains(t.PATIENT_TYPE_ID) || BranchDataWorker.CheckPatientTypeInherit(t.INHERIT_PATIENT_TYPE_IDS, patientTypeIds.ToList()))
                                            && t.IS_ACTIVE == HIS.Desktop.LocalStorage.LocalData.GlobalVariables.CommonNumberTrue
                                            && t.BRANCH_ID == BranchDataWorker.GetCurrentBranchId()
                                            && serviceTypeIdAllows.Contains(t.SERVICE_TYPE_ID)
                                            && ((!t.TREATMENT_TO_TIME.HasValue || t.TREATMENT_TO_TIME.Value >= treatmentTime) || (!t.TO_TIME.HasValue || t.TO_TIME.Value >= intructionTime))).ToList();


                //Ds cũ: List<HIS_PATIENT_TYPE> X
                //Ds mới = X + Y
                //Trong đó:
                //Y = all (his_patient_Type).Where(o => X.Exists(t => t.ID == o.BASE_PATIENT_TYPE_ID)).ToList(); (edited)

                this.patientTypeIdAls = sety != null ? sety.Select(o => o.PATIENT_TYPE_ID).Distinct().ToList() : null;//TODO
                var patientTypeIdPlusAfterFilter = patientTypeAll.Where(k => k.BASE_PATIENT_TYPE_ID != null && this.patientTypeIdAls.Contains(k.BASE_PATIENT_TYPE_ID.Value)).ToList();
                if (patientTypeIdPlusAfterFilter != null && patientTypeIdPlusAfterFilter.Count > 0)
                {
                    patientTypeIdAls.AddRange(patientTypeIdPlusAfterFilter.Select(o => o.ID));
                }
                if (patientTypeIdAls != null)
                    patientTypeIdAls = patientTypeIdAls.Distinct().ToList();

                this.servicePatyInBranchs = sety
                            .GroupBy(o => o.SERVICE_ID)
                            .ToDictionary(o => o.Key, o => o.ToList());

                //Inventec.Common.Logging.LogSystem.Debug("LoadServicePaty____1:" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => currentPatientTypeWithPatientTypeAlter), currentPatientTypeWithPatientTypeAlter)
                //    + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => patientTypeIdAls), patientTypeIdAls));

                //this.currentPatientTypeWithPatientTypeAlter = patientTypeAll.Where(o => this.patientTypeIdAls.Contains(o.ID)).OrderBy(o => o.PRIORITY).ToList();

                //Inventec.Common.Logging.LogSystem.Debug("LoadServicePaty____2:" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => currentPatientTypeWithPatientTypeAlter), currentPatientTypeWithPatientTypeAlter)
                //    );

                this.dicServices = lstService
                    .ToDictionary(o => o.ID);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadDataToGrid(bool isResetSearchtext)
        {
            try
            {
                this.gridViewServiceProcess.ClearGrouping();
                var allDatas = this.ServiceIsleafADOs != null && this.ServiceIsleafADOs.Count > 0 ? this.ServiceIsleafADOs.AsQueryable() : null;
                List<SereServADO> listSereServADO = null;
                List<long> serviceTypeIds = new List<long>();
                if (isResetSearchtext)
                {
                    this.notSearch = true;
                    this.notSearch = false;
                }

                if (this.toggleSwitchDataChecked.EditValue != null && (this.toggleSwitchDataChecked.EditValue ?? "").ToString().ToLower() == "true" && allDatas != null && allDatas.Count() > 0)
                {
                    serviceTypeIds.AddRange(ServiceParentADOs.Select(o => o.SERVICE_TYPE_ID).Distinct());
                    listSereServADO = allDatas.Where(o => o.IsChecked).ToList();
                    this.ChangeStateGroupInGrid(groupType__ServiceTypeName);
                }
                else
                {
                    //Lay tat ca cac node duoc check tren tree
                    this.treeService.CloseEditor();
                    this.treeService.EndCurrentEdit();

                    List<ServiceADO> parentNodes = GetParentSelectedIntree();
                    var nodeCheckeds = this.treeService.GetAllCheckedNodes();

                    var parentNodeHasParentServices = ((this.isNotEventByChangeServiceParent && parentNodes != null && parentNodes.Count > 0) ? parentNodes.Where(o => o.IsParentServiceId == true).ToList() : null);
                    if (parentNodeHasParentServices != null && parentNodeHasParentServices.Count > 0)
                    {
                        WaitingManager.Show();
                        var sgPSIds = parentNodeHasParentServices.Select(o => o.ID).ToArray();
                        this.selectedSeviceGroups = this.workingServiceGroupADOs.Where(o => sgPSIds.Contains(o.ID)).ToList();
                        SelectOneServiceGroupProcess(this.selectedSeviceGroups, false);
                        WaitingManager.Hide();
                        return;
                    }
                    parentNodes = parentNodes.Where(o => (o.IsParentServiceId ?? false) == false).ToList();
                    if (parentNodes != null && parentNodes.Count > 0 && !HisConfigCFG.IsSearchAll)
                    {
                        listSereServADO = new List<SereServADO>();
                        List<long?> parentIds = new List<long?>();
                        serviceTypeIds.AddRange(parentNodes.Select(o => o.SERVICE_TYPE_ID).Distinct());
                        var checkPtttSelected = parentNodes.Any(o => o.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__TT || o.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__PT);
                        if (checkPtttSelected)
                        {
                            this.ChangeStateGroupInGrid(groupType__PtttGroupName);
                        }
                        else
                        {
                            this.ChangeStateGroupInGrid(groupType__ServiceTypeName);
                        }
                        var parentIdAllows = parentNodes.Select(o => o.ID).ToArray();

                        //Lay tat ca cac dich vụ khong co cha cua tat ca cac loai dich vụ duoc check tren tree
                        var parentRootSetys = parentNodes.Where(o => String.IsNullOrEmpty(o.PARENT_ID__IN_SETY)).ToList();
                        if (parentRootSetys != null && parentRootSetys.Count > 0)
                        {
                            foreach (var item in parentRootSetys)
                            {
                                if (item != null)
                                {
                                    var childOfParentNodeNoParents = allDatas.Where(o =>
                                    (o.PARENT_ID == null || item.ID == o.PARENT_ID)
                                    && o.SERVICE_TYPE_ID == item.SERVICE_TYPE_ID
                                    && parentIdAllows.Contains(o.PARENT_ID ?? 0)
                                    ).ToList();
                                    if (childOfParentNodeNoParents != null && childOfParentNodeNoParents.Count > 0)
                                    {
                                        listSereServADO.AddRange(childOfParentNodeNoParents);
                                    }
                                }
                            }
                        }

                        //Lay ra tat ca cac dich vụ con cua dich vu cha va cac dich vu con cua con cua no neu co -> duyet de quy cho den het cay dich vu,..
                        var parentRoots = parentNodes.Where(o => !String.IsNullOrEmpty(o.PARENT_ID__IN_SETY)).ToList();
                        if (parentRoots != null && parentRoots.Count > 0)
                        {
                            foreach (var item in parentRoots)
                            {
                                var childs = GetChilds(item);
                                if (childs != null && childs.Count > 0)
                                {
                                    listSereServADO.AddRange(childs);
                                }
                            }
                        }
                        listSereServADO = listSereServADO.Distinct().ToList();

                    }
                    else
                    {
                        this.ChangeStateGroupInGrid(groupType__ServiceTypeName);
                        listSereServADO = allDatas != null && allDatas.Count() > 0 ? allDatas.ToList() : null;
                        serviceTypeIds.AddRange(ServiceParentADOs.Select(p => p.SERVICE_TYPE_ID).Distinct());
                    }
                }
                this.gridControlServiceProcess.DataSource = null;
                //if (!String.IsNullOrWhiteSpace(txtServiceName_Search.Text) && listSereServADO != null && listSereServADO.Count() > 0)
                //{
                //    listSereServADO = listSereServADO.Where(o => o.SERVICE_NAME_HIDDEN.ToLower().Contains(txtServiceName_Search.Text.ToLower().Trim())).ToList();
                //}
                //if (!String.IsNullOrWhiteSpace(txtServiceCode_Search.Text) && listSereServADO != null && listSereServADO.Count() > 0)
                //{
                //    listSereServADO = listSereServADO.Where(o => o.SERVICE_CODE_HIDDEN.ToLower().Contains(txtServiceCode_Search.Text.ToLower().Trim())).ToList();
                //}
                //if (!String.IsNullOrWhiteSpace(txtServiceBhytCode_Search.Text) && listSereServADO != null && listSereServADO.Count() > 0)
                //{
                //    listSereServADO = listSereServADO.Where(o => o.TDL_HEIN_SERVICE_BHYT_CODE != null && o.TDL_HEIN_SERVICE_BHYT_CODE.ToLower().Contains(txtServiceBhytCode_Search.Text.ToLower().Trim())).ToList();
                //}
                grcSampleType.VisibleIndex = -1;
                serviceTypeIdSplitReq = new List<long>();
                serviceTypeIdRequired = new List<long>();
                if (serviceTypeIds != null && serviceTypeIds.Count > 0)
                {
                    var serviceTypeSereS = BackendDataWorker.Get<HIS_SERVICE_TYPE>().Where(o => serviceTypeIds.Exists(p => p == o.ID)).ToList();
                    bool IsExistSplitReqBySampleType = serviceTypeSereS.Exists(o => o.IS_SPLIT_REQ_BY_SAMPLE_TYPE == 1);
                    if (((HisConfigCFG.IntegrationVersionValue == "1" && HisConfigCFG.IntegrationOptionValue != "1") || (HisConfigCFG.IntegrationVersionValue == "2" && HisConfigCFG.IntegrationTypeValue != "1")) && IsExistSplitReqBySampleType)
                    {
                        grcSampleType.VisibleIndex = 21;
                        serviceTypeIdSplitReq.AddRange(serviceTypeSereS.Where(o => o.IS_SPLIT_REQ_BY_SAMPLE_TYPE == 1).Select(o => o.ID));
                    }
                    foreach (var item in serviceTypeIdSplitReq)
                    {
                        var serviceType = BackendDataWorker.Get<HIS_SERVICE_TYPE>().FirstOrDefault(o => o.ID == item);
                        if (serviceType != null && serviceType.IS_REQUIRED_SAMPLE_TYPE == 1)
                        {
                            serviceTypeIdRequired.Add(serviceType.ID);
                        }
                    }
                }
                this.gridControlServiceProcess.DataSource =
                    listSereServADO != null && listSereServADO.Count > 0 ?
                    listSereServADO
                        .OrderBy(o => o.SERVICE_TYPE_ID)
                        .ThenByDescending(o => o.SERVICE_NUM_ORDER)
                        .ThenBy(o => o.TDL_SERVICE_NAME).ToList()
                    : null;
                this.SetEnableButtonControl(this.actionType);
            }
            catch (Exception ex)
            {
                this.gridViewServiceProcess.EndUpdate();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private List<ServiceADO> GetParentSelectedIntree()
        {
            List<ServiceADO> parentNodes = new List<ServiceADO>();
            var nodeCheckeds = this.treeService.GetAllCheckedNodes();
            if (nodeCheckeds != null && nodeCheckeds.Count > 0)
            {
                //lay data cua cac dong tuong ung voi cac node duoc check
                foreach (var node in nodeCheckeds)
                {
                    var data = this.treeService.GetDataRecordByNode(node) as ServiceADO;
                    if (data != null)
                    {
                        parentNodes.Add(data);
                    }
                }
            }
            return parentNodes;
        }

        private void ChangeStateGroupInGrid(int type)
        {
            try
            {
                if (type == groupType__ServiceTypeName)
                {
                    gridViewServiceProcess.Columns["SERVICE_TYPE_NAME"].GroupIndex = 0;
                    gridViewServiceProcess.Columns["SERVICE_TYPE_NAME"].SortOrder = ColumnSortOrder.Ascending;
                    gridViewServiceProcess.Columns["SERVICE_TYPE_NAME"].Visible = true;

                    gridViewServiceProcess.Columns["PTTT_GROUP_NAME"].GroupIndex = -1;
                    gridViewServiceProcess.Columns["PTTT_GROUP_NAME"].SortOrder = ColumnSortOrder.None;
                    gridViewServiceProcess.Columns["PTTT_GROUP_NAME"].Visible = false;
                }
                else if (type == groupType__PtttGroupName)
                {
                    gridViewServiceProcess.Columns["SERVICE_TYPE_NAME"].GroupIndex = -1;
                    gridViewServiceProcess.Columns["SERVICE_TYPE_NAME"].SortOrder = ColumnSortOrder.None;
                    gridViewServiceProcess.Columns["SERVICE_TYPE_NAME"].Visible = false;

                    gridViewServiceProcess.Columns["PTTT_GROUP_NAME"].GroupIndex = 0;
                    gridViewServiceProcess.Columns["PTTT_GROUP_NAME"].SortOrder = ColumnSortOrder.Ascending;
                    gridViewServiceProcess.Columns["PTTT_GROUP_NAME"].Visible = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private List<SereServADO> GetChilds(ServiceADO parentNode)
        {
            List<SereServADO> result = new List<SereServADO>();
            try
            {
                if (parentNode != null)
                {
                    var childs = ServiceIsleafADOs.Where(o => o.PARENT_ID == parentNode.ID && o.SERVICE_TYPE_ID == parentNode.SERVICE_TYPE_ID).ToList();
                    if (childs != null && childs.Count > 0)
                    {
                        result.AddRange(childs);
                    }

                    var childOfParents = ServiceParentADOs.Where(o => o.PARENT_ID == parentNode.ID && o.SERVICE_TYPE_ID == parentNode.SERVICE_TYPE_ID).ToList();
                    if (childOfParents != null && childOfParents.Count > 0)
                    {
                        foreach (var item in childOfParents)
                        {
                            var childOfChilds = GetChilds(item);
                            if (childOfChilds != null && childOfChilds.Count > 0)
                            {
                                result.AddRange(childOfChilds);
                            }
                        }
                    }
                    if (parentNode.IsParentServiceId == true)
                    {
                        result.AddRange(ServiceIsleafADOs.Where(o => o.IsChecked).ToList());
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private void LoadAppliedPatientType(long patientTypeId, long serviceId, ref SereServADO sereServADO)
        {
            try
            {
                if (serviceId > 0)
                {
                    var checkService = lstService.Find(o => o.ID == serviceId);
                    if (checkService != null && (string.IsNullOrEmpty(checkService.APPLIED_PATIENT_TYPE_IDS) || IsContainString(checkService.APPLIED_PATIENT_TYPE_IDS, patientTypeId.ToString())) && (string.IsNullOrEmpty(checkService.APPLIED_PATIENT_CLASSIFY_IDS) || IsContainString(checkService.APPLIED_PATIENT_CLASSIFY_IDS, currentPatient.PATIENT_CLASSIFY_ID != null ? currentPatient.PATIENT_CLASSIFY_ID.ToString() : "-1")))
                    {
                        sereServADO.IsContainAppliedPatientType = true;
                    }
                    else
                    {
                        sereServADO.IsContainAppliedPatientType = false;
                    }

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetDefaultPrimaryPatientSelect(long patientTypeId, long serviceId, SereServADO sereServADO, long treatmentTime, ref bool IsReturn)
        {
            try
            {
                MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE result = new MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE();
                if (this.currentPatientTypes != null && this.currentPatientTypes.Count > 0 && dicPatientType != null && dicPatientType.Count > 0)
                {
                    long intructionTime = this.intructionTimeSelecteds.FirstOrDefault();
                    var patientTypeIdInSePas = BranchDataWorker.ServicePatyWithListPatientType(serviceId, this.patientTypeIdAls).Where(o => ((!o.TREATMENT_TO_TIME.HasValue || o.TREATMENT_TO_TIME.Value >= treatmentTime) || (!o.TO_TIME.HasValue || o.TO_TIME.Value >= intructionTime)) && ((!sereServADO.PackagePriceId.HasValue && !o.PACKAGE_ID.HasValue) || (sereServADO.PackagePriceId.HasValue && sereServADO.PackagePriceId.Value == o.PACKAGE_ID))).Select(o => o.PATIENT_TYPE_ID).ToList();
                    var patientTypeAll = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE>().Where(o => o.IS_ACTIVE == 1).ToList();
                    if (patientTypeIdInSePas == null) patientTypeIdInSePas = new List<long>();
                    var patientTypeIdPlus = patientTypeAll.Where(k => k.BASE_PATIENT_TYPE_ID != null && patientTypeIdInSePas.Contains(k.BASE_PATIENT_TYPE_ID.Value)).ToList();
                    if (patientTypeIdPlus != null && patientTypeIdPlus.Count > 0)
                    {
                        patientTypeIdInSePas.AddRange(patientTypeIdPlus.Select(o => o.ID));
                    }
                    patientTypeIdInSePas = patientTypeIdInSePas.Distinct().ToList();
                    if (dicPatientType.ContainsKey(sereServADO.PATIENT_TYPE_ID))
                    {
                        var currentPatientTypeTemps = patientTypeIdInSePas != null ? this.dicPatientType[sereServADO.PATIENT_TYPE_ID].Where(o => patientTypeIdInSePas.Contains(o.ID)).OrderBy(o => o.PRIORITY).ToList() : null;
                        var primaryPatientTypeTemps = patientTypeIdInSePas != null ? this.dicPatientType[sereServADO.PATIENT_TYPE_ID].Where(o => o.IS_ADDITION == (short)1 && patientTypeIdInSePas.Contains(o.ID)).OrderBy(o => o.PRIORITY).ToList() : null;
                        if (currentPatientTypeTemps != null && currentPatientTypeTemps.Count > 0)
                        {
                            if (HisConfigCFG.IsSetPrimaryPatientType != commonString__true
                               && this.currentDepartment.DEFAULT_INSTR_PATIENT_TYPE_ID.HasValue
                               && this.currentDepartment.DEFAULT_INSTR_PATIENT_TYPE_ID.Value != HisConfigCFG.PatientTypeId__BHYT
                               && currentPatientTypeTemps.Exists(e => e.ID == this.currentDepartment.DEFAULT_INSTR_PATIENT_TYPE_ID.Value))
                            {
                                Inventec.Common.Logging.LogSystem.Debug("ChoosePatientTypeDefaultlService.3");
                                result = currentPatientTypeTemps.Where(o => (!this.isNotUseBhyt || (this.isNotUseBhyt && o.ID != HisConfigCFG.PatientTypeId__BHYT)) && o.ID == this.currentDepartment.DEFAULT_INSTR_PATIENT_TYPE_ID.Value).FirstOrDefault();
                            }
                            if (sereServADO != null && sereServADO.DEFAULT_PATIENT_TYPE_ID != null && currentPatientTypeTemps.Exists(e => e.ID == sereServADO.DEFAULT_PATIENT_TYPE_ID.Value))
                            {
                                result = currentPatientTypeTemps.Where(o => (!this.isNotUseBhyt || (this.isNotUseBhyt && o.ID != HisConfigCFG.PatientTypeId__BHYT)) && o.ID == sereServADO.DEFAULT_PATIENT_TYPE_ID.Value).FirstOrDefault();
                            }

                            if (HisConfigCFG.IsSetPrimaryPatientType == "2")
                            {
                                if (this.currentHisTreatment.PRIMARY_PATIENT_TYPE_ID <= 0)
                                {
                                    //sereServADO.PRIMARY_PATIENT_TYPE_ID = null;//TODO
                                }
                                else
                                {
                                    if (sereServADO.PATIENT_TYPE_ID == this.currentHisTreatment.PRIMARY_PATIENT_TYPE_ID)
                                    {
                                        sereServADO.PRIMARY_PATIENT_TYPE_ID = null;
                                    }
                                    else
                                    {
                                        sereServADO.PRIMARY_PATIENT_TYPE_ID = this.currentHisTreatment.PRIMARY_PATIENT_TYPE_ID;
                                        if (primaryPatientTypeTemps.Exists(e => e.ID == this.currentHisTreatment.PRIMARY_PATIENT_TYPE_ID))
                                        {
                                            var priPaty = primaryPatientTypeTemps.FirstOrDefault(o => o.ID == this.currentHisTreatment.PRIMARY_PATIENT_TYPE_ID);
                                            sereServADO.PRIMARY_PATIENT_TYPE_ID = priPaty.ID;
                                        }
                                        else
                                        {
                                            try
                                            {
                                                var billPaty = this.currentPatientTypes.FirstOrDefault(o => o.ID == this.currentHisTreatment.PRIMARY_PATIENT_TYPE_ID);
                                                string patyName = billPaty != null ? billPaty.PATIENT_TYPE_NAME : "";
                                                DevExpress.XtraEditors.XtraMessageBox.Show(String.Format(ResourceMessage.DichVuCoDTPTBatBuocNhungKhongCoChinhSachGia, patyName), HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageBoxButtons.OK);
                                                IsReturn = true;
                                                return;
                                            }
                                            catch (Exception ex)
                                            {
                                                Inventec.Common.Logging.LogSystem.Error(ex);
                                            }
                                        }
                                    }
                                }
                            }
                            else if (HisConfigCFG.IsSetPrimaryPatientType == commonString__true
                                && sereServADO.BILL_PATIENT_TYPE_ID.HasValue
                                && sereServADO.PATIENT_TYPE_ID != sereServADO.BILL_PATIENT_TYPE_ID.Value
                                && BackendDataWorker.Get<HIS_PATIENT_TYPE>().FirstOrDefault(o => o.ID == sereServADO.PATIENT_TYPE_ID).BASE_PATIENT_TYPE_ID != sereServADO.BILL_PATIENT_TYPE_ID.Value
                                && primaryPatientTypeTemps.Exists(e => e.ID == sereServADO.BILL_PATIENT_TYPE_ID.Value)
                                && sereServADO.IsContainAppliedPatientType)
                            {
                                var priPaty = primaryPatientTypeTemps.FirstOrDefault(o => o.ID == sereServADO.BILL_PATIENT_TYPE_ID.Value);
                                sereServADO.PRIMARY_PATIENT_TYPE_ID = priPaty.ID;
                                sereServADO.IsNotChangePrimaryPaty = (sereServADO.IS_NOT_CHANGE_BILL_PATY == (short)1);
                            }
                            else if (HisConfigCFG.IsSetPrimaryPatientType == commonString__true
                                && this.currentDepartment.DEFAULT_INSTR_PATIENT_TYPE_ID.HasValue
                                && this.currentDepartment.DEFAULT_INSTR_PATIENT_TYPE_ID.Value != HisConfigCFG.PatientTypeId__BHYT
                                && primaryPatientTypeTemps.Exists(e => e.ID == this.currentDepartment.DEFAULT_INSTR_PATIENT_TYPE_ID.Value)
                                && result.ID != this.currentDepartment.DEFAULT_INSTR_PATIENT_TYPE_ID.Value)
                            {
                                var priPaty = primaryPatientTypeTemps.FirstOrDefault(o => o.ID == this.currentDepartment.DEFAULT_INSTR_PATIENT_TYPE_ID.Value);
                                sereServADO.PRIMARY_PATIENT_TYPE_ID = priPaty.ID;
                            }
                            else
                            {
                                sereServADO.PRIMARY_PATIENT_TYPE_ID = null;//TODO
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Bổ sung: trong trường hợp đối tượng BN là BHYT và chưa đến ngày hiệu lực 
        /// hoặc đã hết hạn sử dụng (thời gian y lệnh ko nằm trong khoảng [từ ngày - đến ngày] của thẻ BHYT), 
        /// thì hiển thị đối tượng thanh toán mặc định là đối tượng viện phí
        /// Ngược lại xử lý như hiện tại: ưu tiên lấy theo đối tượng Bn trước, không có sẽ lấy mặc định theo đối tượng chấp nhận TT đầu tiên tìm thấy
        /// </summary>
        /// <param name="patientTypeId"></param>
        /// <param name="serviceId"></param>
        /// <returns></returns>
        private HIS_PATIENT_TYPE ChoosePatientTypeDefaultlService(long patientTypeId, long serviceId, SereServADO sereServADO, bool notChangePrimary = false, long? patientTypeAppointmentId = null, bool isChangingPatientType = false)
        {
            MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE result = new MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE();
            try
            {
                List<HIS_PATIENT_TYPE> listResult = new List<HIS_PATIENT_TYPE>();
                Inventec.Common.Logging.LogSystem.Debug("ChoosePatientTypeDefaultlService.1");
                if (this.currentPatientTypes != null && this.currentPatientTypes.Count > 0 && currentPatientTypeWithPatientTypeAlter != null && currentPatientTypeWithPatientTypeAlter.Count > 0)
                {
                    this.LoadAppliedPatientType(patientTypeId, sereServADO.SERVICE_ID, ref sereServADO);
                    bool LastOption = false;
                    long intructionTime = this.intructionTimeSelecteds.FirstOrDefault();
                    long treatmentTime = this.currentHisTreatment.IN_TIME;
                    var patientTypeIdInSePas = BranchDataWorker.ServicePatyWithListPatientType(serviceId, this.patientTypeIdAls).Where(o => ((!o.TREATMENT_TO_TIME.HasValue || o.TREATMENT_TO_TIME.Value >= treatmentTime) || (!o.TO_TIME.HasValue || o.TO_TIME.Value >= intructionTime)) && ((!sereServADO.PackagePriceId.HasValue && !o.PACKAGE_ID.HasValue) || (sereServADO.PackagePriceId.HasValue && sereServADO.PackagePriceId.Value == o.PACKAGE_ID))).Select(o => o.PATIENT_TYPE_ID).ToList();
                    var patientTypeIdInSePasWithServices = BranchDataWorker.ServicePatyWithListPatientType(serviceId, this.patientTypeIdAls);
                    //Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => patientTypeIdInSePasWithServices), patientTypeIdInSePasWithServices));
                    //    + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => sereServADO.PackagePriceId), sereServADO.PackagePriceId));
                    var patientTypeAll = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE>().Where(o => o.IS_ACTIVE == 1).ToList();
                    if (patientTypeIdInSePas == null) patientTypeIdInSePas = new List<long>();
                    var patientTypeIdPlus = patientTypeAll.Where(k => k.BASE_PATIENT_TYPE_ID != null && patientTypeIdInSePas.Contains(k.BASE_PATIENT_TYPE_ID.Value)).ToList();
                    if (patientTypeIdPlus != null && patientTypeIdPlus.Count > 0)
                    {
                        patientTypeIdInSePas.AddRange(patientTypeIdPlus.Select(o => o.ID));
                    }
                    patientTypeIdInSePas = patientTypeIdInSePas.Distinct().ToList();
                    var currentPatientTypeTemps = patientTypeIdInSePas != null ? this.currentPatientTypeWithPatientTypeAlter.Where(o => patientTypeIdInSePas.Contains(o.ID)).OrderBy(o => o.PRIORITY).ToList() : null;
                    var primaryPatientTypeTemps = patientTypeIdInSePas != null ? this.currentPatientTypeWithPatientTypeAlter.Where(o => o.IS_ADDITION == (short)1 && patientTypeIdInSePas.Contains(o.ID)).OrderBy(o => o.PRIORITY).ToList() : null;

                    //Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => patientTypeIdInSePas), patientTypeIdInSePas)
                    //    + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => currentPatientTypes), currentPatientTypes)
                    //    + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => currentPatientTypeWithPatientTypeAlter), currentPatientTypeWithPatientTypeAlter));
                    if (currentPatientTypeTemps != null && currentPatientTypeTemps.Count > 0)
                    {
                        if (isChangingPatientType)
                        {
                            Inventec.Common.Logging.LogSystem.Debug("ChoosePatientTypeDefaultlService.6");
                            listResult = currentPatientTypeTemps.Exists(t => t.ID == patientTypeId && (!this.isNotUseBhyt || (this.isNotUseBhyt && t.ID != HisConfigCFG.PatientTypeId__BHYT))) ? currentPatientTypeTemps.Where(o => o.ID == patientTypeId && (!this.isNotUseBhyt || (this.isNotUseBhyt && o.ID != HisConfigCFG.PatientTypeId__BHYT))).ToList() : null;
                        }
                        else
                        {
                            if (patientTypeAppointmentId.HasValue
                                && patientTypeAppointmentId.Value > 0
                                && currentPatientTypeTemps.Exists(e => e.ID == patientTypeAppointmentId.Value))
                            {
                                Inventec.Common.Logging.LogSystem.Debug("ChoosePatientTypeDefaultlService.2");
                                listResult = currentPatientTypeTemps.Where(o => (!this.isNotUseBhyt || (this.isNotUseBhyt && o.ID != HisConfigCFG.PatientTypeId__BHYT)) && o.ID == patientTypeAppointmentId.Value).ToList();
                            }else if (HisConfigCFG.IsSetPrimaryPatientType != commonString__true
                                && this.requestRoom.DEFAULT_INSTR_PATIENT_TYPE_ID.HasValue
                                && this.requestRoom.DEFAULT_INSTR_PATIENT_TYPE_ID.Value != HisConfigCFG.PatientTypeId__BHYT
                                && currentPatientTypeTemps.Exists(e => e.ID == this.requestRoom.DEFAULT_INSTR_PATIENT_TYPE_ID.Value))
                            {
                                Inventec.Common.Logging.LogSystem.Debug("ChoosePatientTypeDefaultlService. 6 currentRoom Has default instr patient type");
                                listResult = currentPatientTypeTemps.Where(o => (!this.isNotUseBhyt || (this.isNotUseBhyt && o.ID != HisConfigCFG.PatientTypeId__BHYT)) && o.ID == this.requestRoom.DEFAULT_INSTR_PATIENT_TYPE_ID.Value).ToList();
                            }
                            else if (HisConfigCFG.IsSetPrimaryPatientType != commonString__true
                                && this.currentDepartment.DEFAULT_INSTR_PATIENT_TYPE_ID.HasValue
                                && this.currentDepartment.DEFAULT_INSTR_PATIENT_TYPE_ID.Value != HisConfigCFG.PatientTypeId__BHYT
                                && currentPatientTypeTemps.Exists(e => e.ID == this.currentDepartment.DEFAULT_INSTR_PATIENT_TYPE_ID.Value))
                            {
                                Inventec.Common.Logging.LogSystem.Debug("ChoosePatientTypeDefaultlService.3");
                                listResult = currentPatientTypeTemps.Where(o => (!this.isNotUseBhyt || (this.isNotUseBhyt && o.ID != HisConfigCFG.PatientTypeId__BHYT)) && o.ID == this.currentDepartment.DEFAULT_INSTR_PATIENT_TYPE_ID.Value).ToList();
                            }
                            else if (!IsValidBhytExceedDayAllowForInPatient())
                            {
                                Inventec.Common.Logging.LogSystem.Debug("ChoosePatientTypeDefaultlService.4");
                                listResult = currentPatientTypeTemps.Where(o => (!this.isNotUseBhyt || (this.isNotUseBhyt && o.ID != HisConfigCFG.PatientTypeId__BHYT)) && o.ID == HisConfigCFG.PatientTypeId__VP).ToList();
                            }
                            else
                            {
                                Inventec.Common.Logging.LogSystem.Debug("ChoosePatientTypeDefaultlService.5");
                                listResult = ((currentPatientTypeTemps.Exists(t => t.ID == patientTypeId && (!this.isNotUseBhyt || (this.isNotUseBhyt && t.ID != HisConfigCFG.PatientTypeId__BHYT)))) ? (currentPatientTypeTemps.Where(o => o.ID == patientTypeId && (!this.isNotUseBhyt || (this.isNotUseBhyt && o.ID != HisConfigCFG.PatientTypeId__BHYT))).ToList() ?? currentPatientTypeTemps.Where(o => (!this.isNotUseBhyt || (this.isNotUseBhyt && o.ID != HisConfigCFG.PatientTypeId__BHYT))).ToList()) : currentPatientTypeTemps.Where(o => (!this.isNotUseBhyt || (this.isNotUseBhyt && o.ID != HisConfigCFG.PatientTypeId__BHYT))).ToList());
                                LastOption = true;
                            }
                        }

                        if (sereServADO != null && sereServADO.DEFAULT_PATIENT_TYPE_ID != null && currentPatientTypeTemps.Exists(e => e.ID == sereServADO.DEFAULT_PATIENT_TYPE_ID.Value) && !sereServADO.IsNotLoadDefaultPatientType)
                        {
                            listResult = currentPatientTypeTemps.Where(o => (!this.isNotUseBhyt || (this.isNotUseBhyt && o.ID != HisConfigCFG.PatientTypeId__BHYT)) && o.ID == sereServADO.DEFAULT_PATIENT_TYPE_ID.Value).ToList();
                            LastOption = false;
                        }
                        if (listResult != null && listResult.Count > 0 && sereServADO.DO_NOT_USE_BHYT == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE && !CheckLoginAdmin.IsAdmin(Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName()))
                        {
                            if (LastOption)
                                listResult = currentPatientTypeTemps.Where(o => o.ID != HisConfigCFG.PatientTypeId__BHYT).ToList();
                            else
                                listResult = listResult.Where(o => o.ID != HisConfigCFG.PatientTypeId__BHYT).ToList();
                        }
                        result = (listResult != null && listResult.Count > 0) ? listResult.FirstOrDefault() : null;

                        #region ĐTTT
                        if (HisConfigCFG.DefaultPatientTypeOption && this.serviceReqParentId != null && this.hisSereServForGetPatientType != null && !sereServADO.IsNotLoadDefaultPatientType)
                        {
                            var lstPatientTypeIdInSePasWithServices = patientTypeIdInSePasWithServices.Where(o => o.PATIENT_TYPE_ID == this.hisSereServForGetPatientType.PATIENT_TYPE_ID).ToList();
                            if (lstPatientTypeIdInSePasWithServices != null && lstPatientTypeIdInSePasWithServices.Count > 0)
                            {
                                if (sereServADO.DO_NOT_USE_BHYT == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE && !CheckLoginAdmin.IsAdmin(Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName()) && this.hisSereServForGetPatientType.PATIENT_TYPE_ID == HisConfigCFG.PatientTypeId__BHYT)
                                {
                                    var ptNotBhyt = patientTypeIdInSePasWithServices.FirstOrDefault(o => o.PATIENT_TYPE_ID != HisConfigCFG.PatientTypeId__BHYT);
                                    if (ptNotBhyt != null)
                                    {
                                        sereServADO.PATIENT_TYPE_ID = ptNotBhyt.PATIENT_TYPE_ID;
                                        sereServADO.PATIENT_TYPE_CODE = currentPatientTypes.First(o => o.ID == ptNotBhyt.PATIENT_TYPE_ID).PATIENT_TYPE_CODE;
                                        sereServADO.PATIENT_TYPE_NAME = currentPatientTypes.First(o => o.ID == ptNotBhyt.PATIENT_TYPE_ID).PATIENT_TYPE_NAME;
                                    }
                                }
                                else
                                {
                                    sereServADO.PATIENT_TYPE_ID = this.hisSereServForGetPatientType.PATIENT_TYPE_ID;
                                    sereServADO.PATIENT_TYPE_CODE = currentPatientTypes.First(o => o.ID == this.hisSereServForGetPatientType.PATIENT_TYPE_ID).PATIENT_TYPE_CODE;
                                    sereServADO.PATIENT_TYPE_NAME = currentPatientTypes.First(o => o.ID == this.hisSereServForGetPatientType.PATIENT_TYPE_ID).PATIENT_TYPE_NAME;
                                }
                            }
                            else if (patientTypeIdInSePasWithServices != null && patientTypeIdInSePasWithServices.Count > 0)
                            {
                                sereServADO.PATIENT_TYPE_ID = patientTypeIdInSePasWithServices.OrderBy(o => o.ID).ToList()[0].PATIENT_TYPE_ID;
                                sereServADO.PATIENT_TYPE_CODE = currentPatientTypes.First(o => o.ID == patientTypeIdInSePasWithServices.OrderBy(p => p.PATIENT_TYPE_ID).ToList()[0].PATIENT_TYPE_ID).PATIENT_TYPE_CODE;
                                sereServADO.PATIENT_TYPE_NAME = currentPatientTypes.First(o => o.ID == patientTypeIdInSePasWithServices.OrderBy(p => p.PATIENT_TYPE_ID).ToList()[0].PATIENT_TYPE_ID).PATIENT_TYPE_NAME;
                            }
                        }
                        else if (result != null && sereServADO != null)
                        {
                            sereServADO.PATIENT_TYPE_ID = result.ID;
                            sereServADO.PATIENT_TYPE_CODE = result.PATIENT_TYPE_CODE;
                            sereServADO.PATIENT_TYPE_NAME = result.PATIENT_TYPE_NAME;
                        }
                        #endregion
                        #region Phụ thu
                        if (HisConfigCFG.IsSetPrimaryPatientType == "2")
                        {
                            if (this.currentHisTreatment.PRIMARY_PATIENT_TYPE_ID <= 0 || notChangePrimary)
                            {
                                //sereServADO.PRIMARY_PATIENT_TYPE_ID = null;//TODO
                            }
                            else
                            {
                                if (sereServADO.PATIENT_TYPE_ID == this.currentHisTreatment.PRIMARY_PATIENT_TYPE_ID)
                                {
                                    sereServADO.PRIMARY_PATIENT_TYPE_ID = null;
                                }
                                else
                                {
                                    sereServADO.PRIMARY_PATIENT_TYPE_ID = this.currentHisTreatment.PRIMARY_PATIENT_TYPE_ID;
                                    if (primaryPatientTypeTemps.Exists(e => e.ID == this.currentHisTreatment.PRIMARY_PATIENT_TYPE_ID))
                                    {
                                        var priPaty = primaryPatientTypeTemps.FirstOrDefault(o => o.ID == this.currentHisTreatment.PRIMARY_PATIENT_TYPE_ID);
                                        sereServADO.PRIMARY_PATIENT_TYPE_ID = priPaty.ID;
                                    }
                                    else
                                    {
                                        try
                                        {
                                            var billPaty = this.currentPatientTypes.FirstOrDefault(o => o.ID == this.currentHisTreatment.PRIMARY_PATIENT_TYPE_ID);
                                            string patyName = billPaty != null ? billPaty.PATIENT_TYPE_NAME : "";
                                            sereServADO.ErrorMessagePatientTypeId = String.Format(ResourceMessage.DichVuCoDTPTBatBuocNhungKhongCoChinhSachGia, patyName);
                                            sereServADO.ErrorTypePatientTypeId = ErrorType.Warning;
                                        }
                                        catch (Exception ex)
                                        {
                                            Inventec.Common.Logging.LogSystem.Error(ex);
                                        }
                                    }
                                }
                            }
                        }
                        else if (!notChangePrimary
                            && HisConfigCFG.IsSetPrimaryPatientType == commonString__true
                            && sereServADO.BILL_PATIENT_TYPE_ID.HasValue
                            && sereServADO.PATIENT_TYPE_ID != sereServADO.BILL_PATIENT_TYPE_ID.Value
                            && BackendDataWorker.Get<HIS_PATIENT_TYPE>().FirstOrDefault(o => o.ID == sereServADO.PATIENT_TYPE_ID).BASE_PATIENT_TYPE_ID != sereServADO.BILL_PATIENT_TYPE_ID.Value
                            && primaryPatientTypeTemps.Exists(e => e.ID == sereServADO.BILL_PATIENT_TYPE_ID.Value)
                            && sereServADO.IsContainAppliedPatientType)
                        {
                            //if (primaryPatientTypeTemps.Exists(e => e.ID == sereServADO.BILL_PATIENT_TYPE_ID.Value))
                            //{
                            var priPaty = primaryPatientTypeTemps.FirstOrDefault(o => o.ID == sereServADO.BILL_PATIENT_TYPE_ID.Value);
                            sereServADO.PRIMARY_PATIENT_TYPE_ID = priPaty.ID;
                            sereServADO.IsNotChangePrimaryPaty = (sereServADO.IS_NOT_CHANGE_BILL_PATY == (short)1);
                            //LogSystem.Debug("sereServADO.IsNotChangePrimaryPaty: " + sereServADO.IsNotChangePrimaryPaty);
                            //}
                            //else
                            //{
                            //    try
                            //    {
                            //        var billPaty = this.currentPatientTypes.FirstOrDefault(o => o.ID == sereServADO.BILL_PATIENT_TYPE_ID.Value);
                            //        string patyName = billPaty != null ? billPaty.PATIENT_TYPE_NAME : "";
                            //        sereServADO.ErrorMessagePatientTypeId = String.Format(ResourceMessage.DichVuCoDTTTBatBuocNhungKhongCoChinhSachGia, patyName);
                            //        sereServADO.ErrorTypePatientTypeId = ErrorType.Warning;
                            //    }
                            //    catch (Exception ex)
                            //    {
                            //        Inventec.Common.Logging.LogSystem.Error(ex);
                            //    }
                            //}
                        }
                        else if (!notChangePrimary
                           && HisConfigCFG.IsSetPrimaryPatientType == commonString__true
                           && this.requestRoom.DEFAULT_INSTR_PATIENT_TYPE_ID.HasValue
                           && this.requestRoom.DEFAULT_INSTR_PATIENT_TYPE_ID.Value != HisConfigCFG.PatientTypeId__BHYT
                           && primaryPatientTypeTemps.Exists(e => e.ID == this.requestRoom.DEFAULT_INSTR_PATIENT_TYPE_ID.Value)
                           && result.ID != this.requestRoom.DEFAULT_INSTR_PATIENT_TYPE_ID.Value)
                        {
                            var priPaty = primaryPatientTypeTemps.FirstOrDefault(o => o.ID == this.requestRoom.DEFAULT_INSTR_PATIENT_TYPE_ID.Value);
                            sereServADO.PRIMARY_PATIENT_TYPE_ID = priPaty.ID;
                        }
                        else if (!notChangePrimary
                            && HisConfigCFG.IsSetPrimaryPatientType == commonString__true
                            && this.currentDepartment.DEFAULT_INSTR_PATIENT_TYPE_ID.HasValue
                            && this.currentDepartment.DEFAULT_INSTR_PATIENT_TYPE_ID.Value != HisConfigCFG.PatientTypeId__BHYT
                            && primaryPatientTypeTemps.Exists(e => e.ID == this.currentDepartment.DEFAULT_INSTR_PATIENT_TYPE_ID.Value)
                            && result.ID != this.currentDepartment.DEFAULT_INSTR_PATIENT_TYPE_ID.Value)
                        {
                            var priPaty = primaryPatientTypeTemps.FirstOrDefault(o => o.ID == this.currentDepartment.DEFAULT_INSTR_PATIENT_TYPE_ID.Value);
                            sereServADO.PRIMARY_PATIENT_TYPE_ID = priPaty.ID;
                        }
                        else if (!notChangePrimary)
                        {
                            sereServADO.PRIMARY_PATIENT_TYPE_ID = null;//TODO
                        }
                        #endregion
                    }
                    else
                    {
                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => currentPatientTypeTemps), currentPatientTypeTemps));
                    }
                }
                return (result ?? new HIS_PATIENT_TYPE());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        bool IsValidBhytExceedDayAllowForInPatient()
        {
            bool result = true;
            try
            {
                if ((this.currentHisPatientTypeAlter.HEIN_CARD_FROM_TIME ?? 0) == 0 && (this.currentHisPatientTypeAlter.HEIN_CARD_TO_TIME ?? 0) == 0)
                    return result;
                DateTime dtHeinCardFromTimePlus = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(this.currentHisPatientTypeAlter.HEIN_CARD_FROM_TIME ?? 0).Value.Date;
                DateTime dtHeinCardToTimePlus = HisConfigCFG.BhytExceedDayAllowForInPatient > 0 ? Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(this.currentHisPatientTypeAlter.HEIN_CARD_TO_TIME ?? 0).Value.AddDays(HisConfigCFG.BhytExceedDayAllowForInPatient).Date : Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(this.currentHisPatientTypeAlter.HEIN_CARD_TO_TIME ?? 0).Value.Date;

                if (this.currentHisPatientTypeAlter.PATIENT_TYPE_ID == HisConfigCFG.PatientTypeId__BHYT
                        && (
                                ((dtHeinCardFromTimePlus > Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(intructionTimeSelecteds.OrderByDescending(o => o).First()).Value.Date
                                || dtHeinCardToTimePlus.Date < Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(intructionTimeSelecteds.OrderByDescending(o => o).First()).Value.Date
                                ))
                            )
                )
                {
                    Inventec.Common.Logging.LogSystem.Warn(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => HisConfigCFG.BhytExceedDayAllowForInPatient), HisConfigCFG.BhytExceedDayAllowForInPatient)
                        + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => this.currentHisPatientTypeAlter.HEIN_CARD_FROM_TIME), this.currentHisPatientTypeAlter.HEIN_CARD_FROM_TIME)
                        + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => this.currentHisPatientTypeAlter.HEIN_CARD_TO_TIME), this.currentHisPatientTypeAlter.HEIN_CARD_TO_TIME)
                        + Inventec.Common.Logging.LogUtil.TraceData("intructionTimeSelecteds.OrderByDescending(o => o).First()", intructionTimeSelecteds.OrderByDescending(o => o).First())
                        + Inventec.Common.Logging.LogUtil.TraceData("dtHeinCardToTimePlus", dtHeinCardToTimePlus)
                        + Inventec.Common.Logging.LogUtil.TraceData("dtHeinCardFromTimePlus", dtHeinCardFromTimePlus)
                        );
                    result = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private bool IsBhytOrVp(long patientTypeId)
        {
            try
            {
                HIS_PATIENT_TYPE paty = BackendDataWorker.Get<HIS_PATIENT_TYPE>().FirstOrDefault(o => o.ID == patientTypeId);
                long basePatientTypeId = paty != null ? (paty.BASE_PATIENT_TYPE_ID ?? 0) : 0;
                if (patientTypeId == HisConfigCFG.PatientTypeId__BHYT
                    || patientTypeId == HisConfigCFG.PatientTypeId__VP
                    || basePatientTypeId == HisConfigCFG.PatientTypeId__BHYT
                    || basePatientTypeId == HisConfigCFG.PatientTypeId__VP)
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return false;
        }

        private long GetRoomId()
        {
            long roomId = 0;
            try
            {
                if (cboAssignRoom.EditValue != null)
                {
                    roomId = (long)cboAssignRoom.EditValue;
                    Inventec.Common.Logging.LogSystem.Debug("Combo nguoi chi dinh co gia tri ==> se lay gia tri combo gan vao RequestRoomId:" + Inventec.Common.Logging.LogUtil.TraceData("roomId", roomId) + Inventec.Common.Logging.LogUtil.TraceData("isAssignInPttt", isAssignInPttt) + Inventec.Common.Logging.LogUtil.TraceData("HisConfigCFG.SetRequestRoomByBedRoomWhenBeingInSurgery", HisConfigCFG.SetRequestRoomByBedRoomWhenBeingInSurgery));
                }
                else
                {
                    roomId = (this.currentModule != null ? this.currentModule.RoomId : 0);
                    Inventec.Common.Logging.LogSystem.Debug("Combo nguoi chi dinh khong gia tri ==> se lay phong dang lam viec gan vao RequestRoomId:" + Inventec.Common.Logging.LogUtil.TraceData("roomId", roomId) + Inventec.Common.Logging.LogUtil.TraceData("isAssignInPttt", isAssignInPttt) + Inventec.Common.Logging.LogUtil.TraceData("HisConfigCFG.SetRequestRoomByBedRoomWhenBeingInSurgery", HisConfigCFG.SetRequestRoomByBedRoomWhenBeingInSurgery));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return roomId;
        }

        private bool GetManualRequestRoom()
        {
            bool isManualRequestRoom = false;
            try
            {
                isManualRequestRoom = (this.examRegisterRoomId > 0 || cboAssignRoom.Enabled);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return isManualRequestRoom;
        }

        /// 0 (hoặc ko khai báo): Không thay đổi gì, giữ nguyên nghiệp vụ như hiện tại
        ///- 1: Có kiểm tra dịch vụ đã kê có nằm trong danh sách đã được cấu hình tương ứng với ICD (căn cứ theo ICD_CODE và ICD_SUB_CODE) của bệnh nhân hay không. Nếu tồn tại dịch vụ không được cấu hình thì hiển thị thông báo và không cho lưu.
        ///- 2: Có kiểm tra, nhưng chỉ hiển thị cảnh báo, và hỏi "Bạn có muốn tiếp tục không". Nếu người dùng chọn "OK" thì vẫn cho phép lưu
        List<HIS_ICD> GetIcdCodeListFromUcIcd()
        {
            List<HIS_ICD> icdList = new List<HIS_ICD>();
            try
            {
                var icdValue = UcIcdGetValue() as HIS.UC.Icd.ADO.IcdInputADO;
                if (icdValue != null && !string.IsNullOrEmpty(icdValue.ICD_CODE))
                {
                    HIS_ICD icdMain = new HIS_ICD();

                    var icd = this.currentIcds.Where(o => o.ICD_CODE == icdValue.ICD_CODE).FirstOrDefault();
                    if (icd != null)
                    {
                        icdMain.ID = icd != null ? icd.ID : 0;
                        icdMain.ICD_NAME = icd != null ? icd.ICD_NAME : "";
                        icdMain.ICD_CODE = icd != null ? icd.ICD_CODE : "";
                        icdList.Add(icdMain);
                    }
                }

                var subIcd = UcSecondaryIcdGetValue() as HIS.UC.SecondaryIcd.ADO.SecondaryIcdDataADO;
                if (subIcd != null)
                {
                    string icd_sub_code = subIcd.ICD_SUB_CODE;
                    if (!string.IsNullOrEmpty(icd_sub_code))
                    {
                        String[] icdCodes = icd_sub_code.Split(';');
                        foreach (var item in icdCodes)
                        {
                            var icd = this.currentIcds.Where(o => o.IS_TRADITIONAL != 1).ToList().FirstOrDefault(o => o.ICD_CODE == item);
                            if (icd != null)
                            {
                                HIS_ICD icdSub = new HIS_ICD();
                                icdSub.ID = icd != null ? icd.ID : 0;
                                icdSub.ICD_NAME = icd != null ? icd.ICD_NAME : "";
                                icdSub.ICD_CODE = icd != null ? icd.ICD_CODE : "";
                                icdList.Add(icdSub);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                icdList = new List<HIS_ICD>();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return icdList;
        }

        private bool CheckIcdServiceForIcd(List<HIS_ICD_SERVICE> IcdServices, ref string messageErr, List<SereServADO> ServiceCheckeds_Send, ref List<SereServADO> ServiceNotConfigResult)
        {
            bool valid = true;
            try
            {
                ServiceNotConfigResult = new List<SereServADO>();
                if (HisConfigCFG.IcdServiceHasRequireCheck || (!HisConfigCFG.IcdServiceHasRequireCheck && IcdServices != null && IcdServices.Count > 0) && ServiceCheckeds_Send != null && ServiceCheckeds_Send.Count > 0)
                {
                    foreach (var item in IcdServices)
                    {
                        var sv = this.ServiceIsleafADOs.Where(o => o.SERVICE_ID == item.SERVICE_ID).FirstOrDefault();

                        if (sv == null || sv.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__G || sv.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__KHAC)
                            continue;

                        var checkIcdService = ServiceCheckeds_Send.Any(o => o.SERVICE_ID == item.SERVICE_ID);
                        if (!checkIcdService)
                        {
                            valid = false;
                            ServiceNotConfigResult.Add(sv);
                            messageErr += sv.TDL_SERVICE_CODE + " - " + sv.TDL_SERVICE_NAME + "; ";
                            Inventec.Common.Logging.LogSystem.Debug("Dich vu (" + sv.TDL_SERVICE_CODE + "-" + sv.TDL_SERVICE_NAME + " chua duoc cau hinh ICD - Dich vu.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                valid = true;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return valid;
        }

        private bool CheckIcdServiceForService(List<HIS_ICD_SERVICE> icdServices, ref string messageErr, List<SereServADO> serviceCheckeds_Send, ref List<SereServADO> serviceNotConfigResult)
        {
            bool valid = true;
            try
            {
                serviceNotConfigResult = new List<SereServADO>();
                // kiểm tra dịch vụ theo cấu hình ICD - Dịch vụ                             

                List<long> serviceIdChecks = serviceCheckeds_Send.Select(o => o.SERVICE_ID).Distinct().ToList();

                if (HisConfigCFG.IcdServiceHasRequireCheck || (!HisConfigCFG.IcdServiceHasRequireCheck && icdServices != null && icdServices.Count > 0) && serviceCheckeds_Send != null && serviceCheckeds_Send.Count > 0)
                {
                    var icdServiceChecks = icdServices.Where(o => serviceIdChecks.Contains(o.SERVICE_ID ?? -1)).ToList();
                    foreach (var item in serviceCheckeds_Send)
                    {
                        if (item.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__G || item.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__KHAC)
                            continue;

                        var checkIcdService = icdServiceChecks.FirstOrDefault(o => o.SERVICE_ID == item.SERVICE_ID);
                        if (checkIcdService == null)
                        {
                            valid = false;
                            serviceNotConfigResult.Add(item);
                            messageErr += item.TDL_SERVICE_CODE + " - " + item.TDL_SERVICE_NAME + "; ";
                            Inventec.Common.Logging.LogSystem.Debug("Dich vu (" + item.TDL_SERVICE_CODE + "-" + item.TDL_SERVICE_NAME + " chua duoc cau hinh ICD - Dich vu.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                valid = true;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return valid;
        }

        private bool CheckValidDataInGridService(CommonParam param, List<SereServADO> serviceCheckeds__Send)
        {
            bool valid = true;
            try
            {
                if (serviceCheckeds__Send != null && serviceCheckeds__Send.Count > 0)
                {
                    foreach (var item in serviceCheckeds__Send)
                    {
                        string messageErr = "";
                        messageErr = String.Format(ResourceMessage.CanhBaoDichVu, item.TDL_SERVICE_NAME);

                        if (item.PATIENT_TYPE_ID <= 0)
                        {
                            valid = false;
                            messageErr += ResourceMessage.KhongCoDoiTuongThanhToan;
                            Inventec.Common.Logging.LogSystem.Debug("Dich vu (" + item.TDL_SERVICE_CODE + "-" + item.TDL_SERVICE_NAME + " khong co doi tuong thanh toan.");
                        }
                        if (item.AMOUNT <= 0)
                        {
                            valid = false;
                            messageErr += ResourceMessage.KhongNhapSoLuong;
                            Inventec.Common.Logging.LogSystem.Debug("Dich vu (" + item.TDL_SERVICE_CODE + "-" + item.TDL_SERVICE_NAME + " khong co so luong.");
                        }

                        if (!valid)
                        {
                            param.Messages.Add(messageErr + ";");
                        }
                    }
                }
                else
                {
                    HIS.Desktop.LibraryMessage.MessageUtil.SetParam(param, HIS.Desktop.LibraryMessage.Message.Enum.ThongBaoDuLieuTrong);
                    valid = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return valid;
        }

        private bool Valid(List<SereServADO> serviceCheckeds__Send)
        {
            CommonParam param = new CommonParam();
            bool valid = true;
            try
            {
                string warning = "";
                this.txtIcdCode.ErrorText = "";
                this.dxValidationProviderControl.RemoveControlError(txtIcdCode);

                this.positionHandleControl = -1;
                valid = (this.dxValidationProviderControl.Validate()) && valid;
                Inventec.Common.Logging.LogSystem.Debug("Valid1:" + valid);
                valid = valid && this.CheckValidDataInGridService(param, serviceCheckeds__Send);
                Inventec.Common.Logging.LogSystem.Debug("Valid2:" + valid);
                if (!valid)
                {
                    if (this.ModuleControls == null || this.ModuleControls.Count == 0)
                    {
                        ModuleControlProcess controlProcess = new ModuleControlProcess(true);
                        this.ModuleControls = controlProcess.GetControls(this);
                    }

                    GetMessageErrorControlInvalidProcess getMessageErrorControlInvalidProcess = new Utility.GetMessageErrorControlInvalidProcess();
                    getMessageErrorControlInvalidProcess.Run(this, this.dxValidationProviderControl, this.ModuleControls, param);

                    warning = param.GetMessage();
                }

                if (!String.IsNullOrEmpty(warning))
                {
                    MessageBox.Show(warning, Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }

                string WaringContinued = "";
                foreach (var item in serviceCheckeds__Send)
                {
                    if (item.ErrorTypeAmount == ErrorType.Warning)
                    {
                        WaringContinued += item.TDL_SERVICE_NAME + " " + item.ErrorMessageAmount + "; ";
                    }
                    if (item.ErrorTypeIsAssignDay == ErrorType.Warning)
                    {
                        WaringContinued += item.TDL_SERVICE_NAME + " " + item.ErrorMessageIsAssignDay + "; ";
                    }
                    if (item.ErrorTypePatientTypeId == ErrorType.Warning)
                    {
                        WaringContinued += item.TDL_SERVICE_NAME + " " + item.ErrorMessagePatientTypeId + "; ";
                    }
                }

                if (!String.IsNullOrEmpty(WaringContinued))
                {
                    WaringContinued += "\n" + ResourceMessage.BanCoMuonTiepTuc;
                    if (MessageBox.Show(WaringContinued, Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.No)
                    {
                        valid = false;
                    }
                }

                Inventec.Common.Logging.LogSystem.Debug("Chi dinh dich vụ -> luu: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => valid), valid) + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => warning), warning));
            }
            catch (Exception ex)
            {
                valid = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return valid;
        }

        private bool CheckIcdByRoom()
        {
            bool valid = true;
            if (this.requestRoom.IS_ALLOW_NO_ICD != 1)
                return valid;

            try
            {
                var icdValue = UcIcdGetValue() as HIS.UC.Icd.ADO.IcdInputADO;
                if ((icdValue != null && String.IsNullOrEmpty(icdValue.ICD_CODE)) && String.IsNullOrEmpty(txtProvisionalDiagnosis.Text))
                {
                    if (MessageBox.Show(ResourceMessage.ChuaNhapChanDoanChinhVaChanDoanSoBo, Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.No)
                    {
                        valid = false;
                        if (String.IsNullOrEmpty(icdValue.ICD_CODE))
                        {
                            txtIcdCode.Focus();
                            txtIcdCode.SelectAll();
                        }
                        else
                        {
                            txtProvisionalDiagnosis.Focus();
                            txtProvisionalDiagnosis.SelectAll();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                valid = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return valid;
        }

        List<HIS_SERE_SERV> lstSereServExist = new List<HIS_SERE_SERV>();
        private void ProcessChoiceIcdPhacDo(List<HIS_ICD_SERVICE> serviceIcds)
        {
            try
            {
                lstSereServExist = new List<HIS_SERE_SERV>();
                ServicePDDTIds = new List<long>();
                var allDatas = this.ServiceIsleafADOs.AsQueryable();
                var serviceChecked = allDatas.Where(o => o.IsChecked).ToList();
                var icdServiceIds = serviceIcds.Where(o => o.IS_CONTRAINDICATION == 1 || o.IS_WARNING == 1).Select(o => o.SERVICE_ID).ToList();
                serviceIcds = serviceIcds.Where(o => !icdServiceIds.Exists(p => p == o.SERVICE_ID)).ToList();
                if (serviceIcds != null && serviceIcds.Count > 0)
                {
                    List<SereServADO> SereServICD = null;
                    List<HIS_SERE_SERV> sereServMinDurations = null;
                    var serviceIds = serviceIcds.Select(o => o.SERVICE_ID).Distinct().ToArray();
                    SereServICD = allDatas.Where(o => serviceIds.Contains(o.SERVICE_ID)).ToList();
                    if (SereServICD != null && SereServICD.Count > 0)
                    {
                        #region GET MIN_DURATION
                        var icdServiceMinDuration = serviceIcds.Where(p => p.MIN_DURATION > 0).ToList();
                        var SereServICDMinDuration = allDatas.Where(o => icdServiceMinDuration.Select(p => p.SERVICE_ID).Distinct().ToArray().Contains(o.SERVICE_ID)).ToList();
                        if (SereServICDMinDuration != null && SereServICDMinDuration.Count > 0)
                        {
                            sereServMinDurations = getSereServWithMinDuration(SereServICDMinDuration, this.currentTreatment.PATIENT_ID, icdServiceMinDuration);
                        }
                        #endregion


                        List<SereServADO> lstSereServResult = new List<SereServADO>();
                        if (sereServMinDurations != null && sereServMinDurations.Count > 0)
                        {
                            var serviceIcdIds = SereServICD.Select(o => o.SERVICE_ID).Distinct().ToArray();
                            var serviceMinDurationIds = sereServMinDurations.Select(p => p.SERVICE_ID).ToArray();
                            var svNotExist = serviceIcdIds.Where(o => !serviceMinDurationIds.ToList().Exists(p => p == o)).ToList();
                            if (svNotExist != null && svNotExist.Count > 0)
                                lstSereServResult = allDatas.Where(o => svNotExist.Contains(o.SERVICE_ID)).ToList();


                            var svExist = serviceIcdIds.Where(o => serviceMinDurationIds.ToList().Exists(p => p == o)).ToList();
                            if (svExist != null && svExist.Count > 0)
                            {
                                lstSereServExist = sereServMinDurations.Where(o => svExist.Contains(o.SERVICE_ID)).ToList();
                            }

                        }
                        else
                        {
                            lstSereServResult = SereServICD;
                        }

                        if (lstSereServResult != null && lstSereServResult.Count > 0)
                        {
                            foreach (var sereServADO in lstSereServResult)
                            {
                                var ssData = this.ServiceIsleafADOs.Where(o => o.SERVICE_ID == sereServADO.SERVICE_ID).FirstOrDefault();
                                if (ssData != null)
                                {
                                    if (!chkAutoCheckPDDT.Checked)
                                    {
                                        if (!serviceChecked.Exists(o => o.SERVICE_ID == ssData.SERVICE_ID))
                                        {
                                            this.ChoosePatientTypeDefaultlService(this.currentHisPatientTypeAlter.PATIENT_TYPE_ID, ssData.SERVICE_ID, ssData);
                                            this.FillDataOtherPaySourceDataRow(ssData);
                                            this.ValidServiceDetailProcessing(ssData);
                                        }
                                        ssData.IsChecked = true;
                                        ServicePDDTIds.Add(ssData.SERVICE_ID);
                                    }
                                    else
                                    {
                                        ssData.IsChecked = false;
                                    }
                                }
                            }
                            foreach (var item in ServiceIsleafADOs)
                            {
                                if (!ServicePDDTIds.Exists(o => o == item.SERVICE_ID))
                                    item.IsChecked = false;
                            }
                        }
                        else
                        {
                            this.ResetDefaultGridData();
                        }

                        this.gridControlServiceProcess.DataSource = null;
                        List<SereServADO> gData = new List<SereServADO>();
                        if (chkAutoCheckPDDT.Checked)
                        {
                            this.ResetDefaultGridData();
                            gData = this.ServiceIsleafADOs.OrderBy(o => o.SERVICE_TYPE_ID).ThenByDescending(o => o.SERVICE_NUM_ORDER).ThenBy(o => o.TDL_SERVICE_NAME).ToList();
                        }
                        else
                        {
                            this.toggleSwitchDataChecked.EditValue = true;
                            gData = this.ServiceIsleafADOs.Where(o => o.IsChecked).OrderBy(o => o.SERVICE_TYPE_ID).ThenByDescending(o => o.SERVICE_NUM_ORDER).ThenBy(o => o.TDL_SERVICE_NAME).ToList();
                        }
                        this.gridControlServiceProcess.DataSource = gData;
                        this.SetEnableButtonControl(this.actionType);
                        VerifyWarningOverCeiling();
                        this.SetDefaultSerServTotalPrice();
                    }
                    else
                    {
                        this.ResetDefaultGridData();
                    }
                }
                else
                {
                    this.ResetDefaultGridData();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ProcessChoiceServiceReqPrevious(MOS.EFMODEL.DataModels.V_HIS_SERVICE_REQ_6 serviceReq)
        {
            try
            {
                if (serviceReq != null)
                {
                    txtDescription.Text = serviceReq.DESCRIPTION ?? "";
                    var allDatas = this.ServiceIsleafADOs.AsQueryable();
                    List<MOS.EFMODEL.DataModels.HIS_SERE_SERV> sereServs = SereServGet.GetByServiceReqId(serviceReq.ID);
                    List<HIS_SERE_SERV_EXT> sereServExts = GetSereServExtByReqId(serviceReq.ID);
                    if (sereServs != null && sereServs.Count > 0)
                    {
                        this.gridViewServiceProcess.BeginUpdate();
                        treeService.UncheckAll();
                        if (sereServs != null && sereServs.Count > 0)
                        {
                            var serviceIds = sereServs.Select(o => o.SERVICE_ID).Distinct().ToArray();
                            allDatas = allDatas.Where(o => serviceIds.Contains(o.SERVICE_ID));
                        }
                        var resultData = allDatas.ToList();

                        if (resultData != null && resultData.Count > 0)
                        {
                            foreach (var sereServADO in resultData)
                            {
                                sereServADO.IsChecked = true;

                                if (sereServExts != null && sereServExts.Count > 0)
                                {
                                    var ss = sereServs.FirstOrDefault(o => o.SERVICE_ID == sereServADO.SERVICE_ID);
                                    var ssExt = ss != null ? sereServExts.FirstOrDefault(o => o.SERE_SERV_ID == ss.ID) : null;
                                    if (ssExt != null)
                                    {
                                        sereServADO.InstructionNote = ssExt.INSTRUCTION_NOTE;
                                    }
                                }
                                this.ChoosePatientTypeDefaultlService(this.currentHisPatientTypeAlter.PATIENT_TYPE_ID, sereServADO.SERVICE_ID, sereServADO);

                                if (!VerifyCheckFeeWhileAssign())
                                {
                                    this.ResetOneService(sereServADO);
                                    sereServADO.IsChecked = false;
                                    break;
                                }
                                this.SetAssignNumOrder(sereServADO);
                                this.FillDataOtherPaySourceDataRow(sereServADO);

                                List<V_HIS_EXECUTE_ROOM> executeRoomList = null;
                                FilterExecuteRoom(sereServADO, ref executeRoomList);

                                long executeRoomId = this.SetPriorityRequired(executeRoomList);

                                if (executeRoomId <= 0)
                                    executeRoomId = this.SetDefaultExcuteRoom(executeRoomList);

                                //data.TDL_EXECUTE_ROOM_ID = executeRoomDefault;
                                if (sereServADO.TDL_EXECUTE_ROOM_ID <= 0 && executeRoomId > 0)
                                {
                                    sereServADO.TDL_EXECUTE_ROOM_ID = executeRoomId;
                                }
                                this.ValidServiceDetailProcessing(sereServADO);
                            }

                            this.toggleSwitchDataChecked.EditValue = true;
                            //this.ValidOnlyShowNoticeService(resultData);
                        }
                        this.gridViewServiceProcess.GridControl.DataSource = resultData.OrderByDescending(o => o.SERVICE_NUM_ORDER).ToList();
                        this.gridViewServiceProcess.EndUpdate();

                        this.SetEnableButtonControl(this.actionType);
                        VerifyWarningOverCeiling();
                        this.SetDefaultSerServTotalPrice();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private List<HIS_SERE_SERV_EXT> GetSereServExtByReqId(long serviceReqId)
        {
            List<HIS_SERE_SERV_EXT> rs = null;
            try
            {
                HisSereServExtFilter ssExtFilter = new HisSereServExtFilter();
                ssExtFilter.TDL_SERVICE_REQ_ID = serviceReqId;
                rs = new BackendAdapter(new CommonParam()).Get<List<HIS_SERE_SERV_EXT>>("api/HisSereServExt/Get", ApiConsumers.MosConsumer, ssExtFilter, null);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                rs = null;
            }
            return rs;
        }
        int dfEndServiceReq = 10;
        int rowCount = 0;
        int dataTotal = 0;
        int startPage = 0;
        private void FillDataToComboPriviousServiceReq()
        {
            try
            {
                LoadPaging(new CommonParam(0, dfEndServiceReq));
                CommonParam param = new CommonParam();
                param.Limit = rowCount;
                param.Count = dataTotal;
                ucPaging1.Init(LoadPaging, param, dfEndServiceReq, this.gridControl3);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadPaging(object commonParam)
        {
            try
            {
                startPage = ((CommonParam)commonParam).Start ?? 0;
                int limit = ((CommonParam)commonParam).Limit ?? 0;

                CommonParam param = new CommonParam(startPage, limit);
                MOS.Filter.HisServiceReqView6Filter serviceReqFilter = new MOS.Filter.HisServiceReqView6Filter();
                serviceReqFilter.TDL_PATIENT_ID = this.currentHisTreatment.PATIENT_ID;
                serviceReqFilter.ORDER_DIRECTION = "DESC";
                serviceReqFilter.ORDER_FIELD = "CREATE_TIME";
                serviceReqFilter.SERVICE_REQ_TYPE_IDs = new List<long>();
                //Nếu thêm một loại yêu cầu dv khác thì phải vào đây bổ sung
                if (indexServiceType == 0)
                {
                    serviceReqFilter.SERVICE_REQ_TYPE_IDs.Add(IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__G);
                    serviceReqFilter.SERVICE_REQ_TYPE_IDs.Add(IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__CDHA);
                    serviceReqFilter.SERVICE_REQ_TYPE_IDs.Add(IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__NS);
                    serviceReqFilter.SERVICE_REQ_TYPE_IDs.Add(IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__SA);
                    serviceReqFilter.SERVICE_REQ_TYPE_IDs.Add(IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__TDCN);
                    serviceReqFilter.SERVICE_REQ_TYPE_IDs.Add(IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__PT);
                    serviceReqFilter.SERVICE_REQ_TYPE_IDs.Add(IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__KHAC);
                    serviceReqFilter.SERVICE_REQ_TYPE_IDs.Add(IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__PHCN);
                    serviceReqFilter.SERVICE_REQ_TYPE_IDs.Add(IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__KH);
                    serviceReqFilter.SERVICE_REQ_TYPE_IDs.Add(IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__TT);
                    serviceReqFilter.SERVICE_REQ_TYPE_IDs.Add(IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__XN);
                    serviceReqFilter.SERVICE_REQ_TYPE_IDs.Add(IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__GPBL);
                }
                else if (indexServiceType == 1)
                {
                    serviceReqFilter.SERVICE_REQ_TYPE_IDs.Add(IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__KH);
                }
                else if (indexServiceType == 2)
                {
                    serviceReqFilter.SERVICE_REQ_TYPE_IDs.Add(IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__XN);
                }
                else if (indexServiceType == 3)
                {
                    serviceReqFilter.SERVICE_REQ_TYPE_IDs.Add(IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__CDHA);
                }
                else if (indexServiceType == 4)
                {
                    serviceReqFilter.SERVICE_REQ_TYPE_IDs.Add(IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__TT);
                }
                else if (indexServiceType == 5)
                {
                    serviceReqFilter.SERVICE_REQ_TYPE_IDs.Add(IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__TDCN);
                }
                else if (indexServiceType == 6)
                {
                    serviceReqFilter.SERVICE_REQ_TYPE_IDs.Add(IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__G);
                }
                else if (indexServiceType == 7)
                {
                    serviceReqFilter.SERVICE_REQ_TYPE_IDs.Add(IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__NS);
                }
                else if (indexServiceType == 8)
                {
                    serviceReqFilter.SERVICE_REQ_TYPE_IDs.Add(IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__SA);
                }
                else if (indexServiceType == 9)
                {
                    serviceReqFilter.SERVICE_REQ_TYPE_IDs.Add(IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__PT);
                }
                else if (indexServiceType == 10)
                {
                    serviceReqFilter.SERVICE_REQ_TYPE_IDs.Add(IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__KHAC);
                }
                else if (indexServiceType == 11)
                {
                    serviceReqFilter.SERVICE_REQ_TYPE_IDs.Add(IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__PHCN);
                }
                else if (indexServiceType == 12)
                {
                    serviceReqFilter.SERVICE_REQ_TYPE_IDs.Add(IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__GPBL);
                }
                Inventec.Common.Logging.LogSystem.Debug("begin call HisServiceReq/GetView6");
                var apiResult = new BackendAdapter(param).GetRO<List<MOS.EFMODEL.DataModels.V_HIS_SERVICE_REQ_6>>(RequestUriStore.HIS_SERVICE_REQ_GETVIEW_6, ApiConsumers.MosConsumer, serviceReqFilter, ProcessLostToken, param);
                Inventec.Common.Logging.LogSystem.Debug("end call HisServiceReq/GetView6");
                //List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                //columnInfos.Add(new ColumnInfo("SERVICE_REQ_TYPE_NAME", "", 250, 1));
                //columnInfos.Add(new ColumnInfo("RENDERER_INTRUCTION_TIME", "", 150, 2));
                //columnInfos.Add(new ColumnInfo("REQUEST_USERNAME ", "", 200, 3));
                //ControlEditorADO controlEditorADO = new ControlEditorADO("RENDERER_INTRUCTION_TIME", "ID", columnInfos, false, 700);
                //ControlEditorLoader.Load(this.cboPriviousServiceReq, this.currentPreServiceReqs, controlEditorADO);
                //cboPriviousServiceReq.Properties.ImmediatePopup = true;
                //cboPriviousServiceReq.Properties.PopupWidth = 700;
                WaitingManager.Hide();
                gridControl3.DataSource = null;
                if (apiResult != null)
                {
                    currentPreServiceReqs = (List<MOS.EFMODEL.DataModels.V_HIS_SERVICE_REQ_6>)apiResult.Data;
                    if (currentPreServiceReqs != null)
                    {
                        gridControl3.DataSource = currentPreServiceReqs;
                        rowCount = (currentPreServiceReqs == null ? 0 : currentPreServiceReqs.Count);
                        dataTotal = (apiResult.Param == null ? 0 : apiResult.Param.Count ?? 0);
                    }
                }

                #region Process has exception
                SessionManager.ProcessTokenLost(param);
                #endregion
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Gan gia trị mac dinh cho cac control can khoi tao san gia tri
        /// </summary>
        private void SetDefaultData(bool isInit = false)
        {
            try
            {
                lstSereServExist = new List<HIS_SERE_SERV>();
                this.gridViewServiceProcess.ActiveFilter.Clear();
                this.gridViewServiceProcess.ClearColumnsFilter();
                this.dicServiceReqList = new Dictionary<long, HisServiceReqListResultSDO>();
                this.serviceReqComboResultSDO = null;
                this.repositoryItemchkIsCheckedDisable.ReadOnly = true;
                this.repositoryItemchkIsCheckedDisable.Enabled = false;
                this.ButtonEdit_IsExpenDisable.ReadOnly = true;
                this.ButtonEdit_IsExpenDisable.Enabled = false;
                this.repositoryItemSpinAmount__Disable_TabService.ReadOnly = true;
                this.repositoryItemSpinAmount__Disable_TabService.Enabled = false;
                this.btnCreateServiceGroup.Enabled = false;
                this.btnSave.Enabled = false;
                this.btnSaveAndPrint.Enabled = false;
                this.btnShowDetail.Enabled = false;
                this.btnCreateBill.Enabled = false;
                this.btnDepositService.Enabled = false;
                this.btnPrintPhieuHuongDanBN.Enabled = false;
                this.pnlPrintAssignService.Enabled = false;
                this.chkPriority.Checked = false;
                this.chkIsNotRequireFee.Checked = false;
                this.selectedSeviceGroups = null;
                if (this.workingServiceGroupADOs != null && this.workingServiceGroupADOs.Count > 0)
                    this.workingServiceGroupADOs.ForEach(o => o.IsChecked = false);
                this.beditRoom.EditValue = null;
                this.beditRoom.Properties.Buttons[1].Visible = false;

                this.cboPackage.EditValue = null;
                this.txtDescription.Text = "";
                this.cboExecuteGroup.EditValue = null;
                this.cboExecuteGroup.Properties.Buttons[1].Visible = false;
                this.chkExpendAll.Checked = false;
                this.lblTotalServicePrice.Text = "0";
                this.lblTotalServicePriceBhyt.Text = "0";
                this.lblTotalServicePriceOther.Text = "0";
                this.lblChenhBHYT.Text = "0";
                this.actionType = GlobalVariables.ActionAdd;
                this.btnBoSungPhacDo.Enabled = (HisConfigCFG.IcdServiceAllowUpdate == GlobalVariables.CommonStringTrue);
                this.chkIsInformResultBySms.CheckState = CheckState.Unchecked;
                this.chkIsEmergency.CheckState = CheckState.Unchecked;
                this.chkIsNotRequireFee.Enabled = false;
                this.chkIsNotRequireFee.CheckState = CheckState.Unchecked;
                this.txtProvisionalDiagnosis.Text = this.provisionalDiagnosis;
                this.dSignedList = new Dictionary<long, List<Inventec.Common.SignLibrary.DTO.DocumentSignedUpdateIGSysResultDTO>>();
                //this.txtAssignRoomCode.Text = "";
                //this.cboAssignRoom.EditValue = null;

                //this.lblChiPhiBNPhaiTra.Text = "";
                //this.lblDaDong.Text = "";
                //this.lciForlblConThua.AppearanceItemCaption.ForeColor = System.Drawing.Color.Black;
                //this.lblConThua.Text = "";
                //this.lciForlblConThua.Text = Inventec.Common.Resource.Get.Value("frmAssignService.lciForlblConThua.Text", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());

                if (isInit || HisConfigCFG.IsUsingServerTime != commonString__true)
                {
                    UC.DateEditor.ADO.DateInputADO dateInputADO = new UC.DateEditor.ADO.DateInputADO();
                    if (HisConfigCFG.IsShowServerTimeByDefault)
                    {
                        dateInputADO.Time = dteCommonParam;
                        dateInputADO.Dates = new List<DateTime?>();
                        dateInputADO.Dates.Add(dateInputADO.Time);
                    }
                    dateInputADO.IsVisibleMultiDate = true;
                    UcDateReload(dateInputADO);
                    //ucDateProcessor.Reload(ucDate, dateInputADO);
                    //this.intructionTimeSelecteds = ucDateProcessor.GetValue(ucDate);
                }
                if (!isInit)
                {
                    string loginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                    var data = BackendDataWorker.Get<ACS.EFMODEL.DataModels.ACS_USER>().Where(o => o.LOGINNAME.ToUpper().Equals(loginName.ToUpper())).FirstOrDefault();
                    if (data != null)
                    {
                        this.cboConsultantUser.EditValue = data.LOGINNAME;
                        this.txtConsultantLoginname.Text = data.LOGINNAME;
                    }
                }
                //this.isMultiDateState = false;

                //GridCheckMarksSelection gridCheckMark = cboServiceGroup.Properties.Tag as GridCheckMarksSelection;
                //if (gridCheckMark != null)
                //    gridCheckMark.ClearSelection(cboServiceGroup.Properties.View);

                //if (HisConfigCFG.SetRequestRoomByBedRoomWhenBeingInSurgery == "1")
                //{
                //    txtAssignRoomCode.Enabled = cboAssignRoom.Enabled = true;
                //}

                //this.beditRoom.Text = "";
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private async void InitConfig()
        {
            try
            {
                this.DisablecheckEmergencyPriorityByConfig();
                this.VisibleExecuteGroupByConfig();
                this.VisibleColumnInGridControlService();
                this.EnableCboTracking();

                if (HisConfigCFG.IsUsingExecuteRoomPayment)
                {
                    lciForlblSoDuTaiKhoan.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    LoadUsingExecuteRoomPaymentProcess();
                }
                else
                {
                    lciForlblSoDuTaiKhoan.Text = "  ";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private async Task LoadUsingExecuteRoomPaymentProcess()
        {
            CommonParam param = new CommonParam();
            Inventec.Common.Logging.LogSystem.Debug("begin call HisPatient/GetCardBalance");
            var balance = await new BackendAdapter(param).GetAsync<decimal?>(RequestUriStore.HIS_PATIENT__GET_CARD_BALANCE, ApiConsumers.MosConsumer, this.currentHisTreatment.PATIENT_ID, ProcessLostToken, param);
            Inventec.Common.Logging.LogSystem.Debug("end call HisPatient/GetCardBalance");
            lblSoDuTaiKhoan.Text = (balance.HasValue ? Inventec.Common.Number.Convert.NumberToString(balance.Value, ConfigApplications.NumberSeperator) : "0");
        }

        /// <summary>
        /// Kiểm tra nếu có cấu hình ẩn hiện ô check cấp cứu & ô check ưu tiên
        /// </summary>
        private void DisablecheckEmergencyPriorityByConfig()
        {
            try
            {
                if (this.isAutoEnableEmergency)
                {
                    this.lciEmergency.Enabled = true;
                }

                var isExistsExecuteRoom = this.allDataExecuteRooms
                     .Any(o =>
                     o.IS_EMERGENCY == 1
                     && o.IS_ACTIVE == 1
                     && o.ROOM_ID == currentModule.RoomId);
                if (isExistsExecuteRoom)
                {
                    this.lciEmergency.Enabled = true;
                    this.chkIsNotRequireFee.CheckState = CheckState.Checked;
                    this.chkPriority.CheckState = CheckState.Checked;
                }

                if (HisConfigCFG.IsAutoCheckPriorityForPrioritizedExam && this.isPriority)
                {
                    this.chkPriority.CheckState = CheckState.Checked;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Kiểm tra nếu thiết lập cấu hình phần mềm (key = HIS.Desktop.Plugins.Assign.IsExecuteGroup = 1) 
        /// và (loại phòng làm việc của người dùng là phòng xử lý dv và là phòng phẫu thuật) hoặc (loại phòng làm việc của người dùng là buồng bệnh là phòng phẫu thuật)
        /// thì hiển thị control nhóm xử lý. Mặc định ẩn control nhóm xử lý
        /// </summary>
        private void VisibleExecuteGroupByConfig()
        {
            try
            {
                if (HisConfigCFG.IsVisibleExecuteGroup == commonString__true && this.currentModule != null && this.currentModule.RoomId > 0)
                {
                    var executeRoom = this.allDataExecuteRooms.FirstOrDefault(o => o.ROOM_ID == currentModule.RoomId && o.IS_ACTIVE == 1);
                    var bedRoom = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_BED_ROOM>().FirstOrDefault(o => o.ROOM_ID == currentModule.RoomId);

                    this.lciExecuteGroup.Visibility = ((executeRoom != null && executeRoom.IS_SURGERY == 1)
                                            || (bedRoom != null && bedRoom.IS_SURGERY == 1))
                    ? DevExpress.XtraLayout.Utils.LayoutVisibility.Always : DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Kiểm tra cấu hình phần mềm ẩn hiện cột chi phí ngoài gói, cột hao phí, cột giá gói, cột không tính chênh lệch
        /// </summary>
        private void VisibleColumnInGridControlService()
        {
            try
            {
                //An hien cot cp ngoai goi
                long isVisibleColumnCPNgoaiGoi = ConfigApplicationWorker.Get<long>(AppConfigKeys.CONFIG_KEY__MODULE_CHI_DINH_DICH_VU__AN_HIEN_CP_NGOAI_GOI);
                this.gridColumnChiPhiNgoaiGoi_TabService.Visible = (isVisibleColumnCPNgoaiGoi != 1);

                //An hien cot hao phi
                long isVisibleColumnHaoPhi = ConfigApplicationWorker.Get<long>(AppConfigKeys.CONFIG_KEY__MODULE_CHI_DINH_DICH_VU__AN_HIEN_HAO_PHI);
                this.grcExpend_TabService.Visible = (isVisibleColumnHaoPhi != 1);

                if (HisConfigCFG.IsNotAllowingExpendWithoutHavingParent && this.GetSereServInKip() > 0)
                {
                    this.grcExpend_TabService.Visible = false;
                }

                //An hien cot gia goi
                long isVisibleColumnGiaGoi = ConfigApplicationWorker.Get<long>(AppConfigKeys.CONFIG_KEY__MODULE_CHI_DINH_DICH_VU__AN_HIEN_GIA_GOI);
                this.grcPrice_ServicePatyPrpo.Visible = (isVisibleColumnGiaGoi != 1);

                //Ẩn hiện cột "Không tính chênh lệch" & cột "Giá trần BHYT" theo cấu hình trên ccc             
                this.gridColumnNoDifference.Visible = (HisConfigCFG.NoDifference == commonString__true);
                this.gridColumnHeadCardNumberNoDifference.Visible = (HisConfigCFG.NoDifference == commonString__true);
                if (HisConfigCFG.IsSetPrimaryPatientType == commonString__true)
                {
                    this.gridColumn_Service_PrimaryPatientType.Visible = true;
                    this.gridColumn_Service_PrimaryPatientType.OptionsColumn.AllowEdit = true;
                }
                else if (HisConfigCFG.IsSetPrimaryPatientType == "2")
                {
                    this.gridColumn_Service_PrimaryPatientType.Visible = true;
                    this.gridColumn_Service_PrimaryPatientType.OptionsColumn.AllowEdit = false;
                }
                else
                {
                    this.gridColumn_Service_PrimaryPatientType.Visible = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetDefautTrackingCombo(List<TrackingAdo> result, string isDefaultTracking)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("SetDefautTrackingCombo.1");
                if (isDefaultTracking != "1")
                {
                    cboTracking.EditValue = null;
                    return;
                }
                Inventec.Common.Logging.LogSystem.Debug("SetDefautTrackingCombo.2");
                if (this.intructionTimeSelecteds != null && this.intructionTimeSelecteds.Count > 0 && result != null && result.Count > 0)
                {
                    Inventec.Common.Logging.LogSystem.Debug("SetDefautTrackingCombo.3");
                    long instructionTime = this.intructionTimeSelecteds.FirstOrDefault();//20181216223040

                    string instructionDateStr = instructionTime.ToString().Substring(0, 8);
                    var trackingDefaults = result.Where(o => instructionDateStr.Contains(o.TRACKING_TIME.ToString().Substring(0, 8))
                     && o.DEPARTMENT_ID == requestRoom.DEPARTMENT_ID)
                     .OrderByDescending(o => o.TRACKING_TIME).ToList();
                    if (trackingDefaults != null && trackingDefaults.Count > 0 && isDefaultTracking == "1")
                    {
                        Inventec.Common.Logging.LogSystem.Debug("SetDefautTrackingCombo.4");
                        cboTracking.EditValue = trackingDefaults.FirstOrDefault().ID;
                    }
                    else
                    {
                        Inventec.Common.Logging.LogSystem.Debug("SetDefautTrackingCombo.5");
                        cboTracking.EditValue = null;
                    }
                }
                else
                {
                    Inventec.Common.Logging.LogSystem.Debug("SetDefautTrackingCombo.6");
                    cboTracking.EditValue = null;
                }
                Inventec.Common.Logging.LogSystem.Debug("SetDefautTrackingCombo.7");
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        async Task LoadServiceSameToRAM()
        {
            try
            {
                if (!BackendDataWorker.IsExistsKey<V_HIS_SERVICE_SAME>())
                {
                    MOS.Filter.HisServiceSameViewFilter serviceSameViewFilter = new HisServiceSameViewFilter();
                    serviceSameViewFilter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    this.currentServiceSames = await new BackendAdapter(new CommonParam()).GetAsync<List<V_HIS_SERVICE_SAME>>("api/HisServiceSame/GetView", ApiConsumer.ApiConsumers.MosConsumer, serviceSameViewFilter, null);

                    if (this.currentServiceSames != null) BackendDataWorker.UpdateToRam(typeof(MOS.EFMODEL.DataModels.V_HIS_SERVICE_SAME), this.currentServiceSames, long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss")));
                }
                else
                {
                    this.currentServiceSames = BackendDataWorker.Get<V_HIS_SERVICE_SAME>();
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        async Task LoadDataToTrackingCombo()
        {
            Inventec.Common.Logging.LogSystem.Debug("LoadDataToTrackingCombo.1");
            List<TrackingAdo> result = new List<TrackingAdo>();
            try
            {
                CommonParam param = new CommonParam();
                MOS.Filter.HisTrackingFilter trackingFilter = new HisTrackingFilter();

                //trackingFilter.TRACKING_TIME_FROM = Inventec.Common.TypeConvert.Parse.ToInt64(instructionDateStr + "000000");
                //trackingFilter.TRACKING_TIME_TO = Inventec.Common.TypeConvert.Parse.ToInt64(instructionDateStr + "235959");
                trackingFilter.TREATMENT_ID = this.treatmentId;
                trackingFilter.DEPARTMENT_ID = this.requestRoom != null ? (long?)this.requestRoom.DEPARTMENT_ID : null;
                this.trackings = await new BackendAdapter(param).GetAsync<List<HIS_TRACKING>>("api/HisTracking/Get", ApiConsumer.ApiConsumers.MosConsumer, trackingFilter, param);
                this.trackings = this.trackings != null && this.trackings.Count > 0 ? this.trackings.OrderByDescending(o => o.TRACKING_TIME).ToList() : trackings;
                foreach (var tracking in this.trackings)
                {
                    var trackingAdo = new TrackingAdo(tracking);
                    result.Add(trackingAdo);
                }

                this.trackingAdos = result;

                List<string> intructionDateSelectedProcess = new List<string>();
                foreach (var item in this.intructionTimeSelecteds)
                {
                    string intructionDate = item.ToString().Substring(0, 8);
                    intructionDateSelectedProcess.Add(intructionDate);
                }

                bool btnEn = cboTracking.Enabled;
                cboTracking.Enabled = true;
                if (!chkMultiIntructionTime.Checked)
                {
                    this.isInitTracking = false;
                    cboTracking.Properties.View.OptionsSelection.MultiSelect = false;
                    List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                    columnInfos.Add(new ColumnInfo("TrackingTimeStr", "", 150, 1));
                    columnInfos.Add(new ColumnInfo("CREATOR", "", 150, 2));
                    ControlEditorADO controlEditorADO = new ControlEditorADO("TrackingTimeStr", "ID", columnInfos, false, 350);
                    ControlEditorLoader.Load(this.cboTracking, result, controlEditorADO);
                    cboTracking.EditValue = null;
                    //SetDefautTrackingCombo(result, HisConfigCFG.IsDefaultTracking);


                    long trackIdSet = 0;
                    if (this.tracking != null && !isUseTrackingInputWhileChangeTrackingTime)
                    {
                        trackIdSet = this.tracking.ID;
                    }
                    else
                    {
                        var trackingTemps = trackings.Where(o => intructionDateSelectedProcess.Contains(o.TRACKING_TIME.ToString().Substring(0, 8))
                            && o.DEPARTMENT_ID == HIS.Desktop.LocalStorage.LocalData.WorkPlace.GetDepartmentId(this.currentModule.RoomTypeId))
                            .OrderByDescending(o => o.TRACKING_TIME).ToList();

                        if (trackingTemps != null
                            && trackingTemps.Count > 0
                            && HisConfigCFG.IsDefaultTracking == "1"
                            )
                        {
                            trackIdSet = trackingTemps[0].ID;
                            this.tracking = trackingTemps[0];
                        }
                    }

                    if (trackIdSet > 0)
                    {
                        cboTracking.EditValue = trackIdSet;
                        cboTracking.Properties.Buttons[1].Visible = true;
                        if (HisConfigCFG.IsServiceReqIcdOption)
                        {
                            this.LoadIcdToControl(this.tracking.ICD_CODE, this.tracking.ICD_NAME);
                            isNotProcessWhileChangedTextSubIcd = true;
                            this.LoadDataToIcdSub(this.tracking.ICD_SUB_CODE, this.tracking.ICD_TEXT);
                            isNotProcessWhileChangedTextSubIcd = false;
                        }
                        else
                        {
                            if (HisConfigCFG.TrackingCreate__UpdateTreatmentIcd == "1")
                                this.LoadIcdDefault();//TODO
                        }
                    }

                    this.isUseTrackingInputWhileChangeTrackingTime = false;
                }
                else
                {
                    this.isInitTracking = true;
                    InitCheck(cboTracking, SelectionGrid__ToDieuTri);
                    InitCombo(cboTracking, trackingAdos, "TrackingTimeStr", "CREATOR", "ID");

                    cboTracking.ShowPopup();
                    cboTracking.ClosePopup();

                    if (HisConfigCFG.IsDefaultTracking == "1")
                    {
                        var trackingTemps = result.Where(o => intructionDateSelectedProcess.Contains(o.TRACKING_TIME.ToString().Substring(0, 8))
                            && o.CREATOR.ToUpper() == this.txtLoginName.Text.ToUpper())
                            .GroupBy(o => o.TRACKING_TIME.ToString().Substring(0, 8));

                        List<TrackingAdo> LstTrackingADOs = new List<TrackingAdo>();

                        foreach (var itemG in trackingTemps)
                        {
                            LstTrackingADOs.Add(itemG.OrderByDescending(o => o.TRACKING_TIME).FirstOrDefault());
                        }

                        GridCheckMarksSelection gridCheckMark = cboTracking.Properties.Tag as GridCheckMarksSelection;
                        if (gridCheckMark != null)
                        {
                            gridCheckMark.SelectAll(LstTrackingADOs);
                        }

                    }
                }
                if (!btnEn) cboTracking.Enabled = false;
                isInitTracking = false;
            }
            catch (Exception ex)
            {
                result = new List<TrackingAdo>();
                LogSystem.Error(ex);
            }


        }

        private void InitCombo(GridLookUpEdit cbo, object data, string DisplayValue1, string DisplayValue2, string ValueMember)
        {
            try
            {
                cbo.Properties.DataSource = data;
                cbo.Properties.DisplayMember = DisplayValue1;
                cbo.Properties.ValueMember = ValueMember;
                DevExpress.XtraGrid.Columns.GridColumn col2 = cbo.Properties.View.Columns.AddField(DisplayValue1);
                col2.VisibleIndex = 1;
                col2.Width = 250;
                col2.Caption = "Thời gian";

                DevExpress.XtraGrid.Columns.GridColumn col3 = cbo.Properties.View.Columns.AddField(DisplayValue2);
                col3.VisibleIndex = 2;
                col3.Width = 100;
                col3.Caption = "Người tạo";

                cbo.Properties.PopupFormWidth = 350;
                cbo.Properties.View.OptionsView.ShowColumnHeaders = true;

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void SelectionGrid__ToDieuTri(object sender, EventArgs e)
        {
            try
            {
                string typeName = "";
                this.Listtrackings = new List<HIS_TRACKING>();
                foreach (TrackingAdo rv in (sender as GridCheckMarksSelection).Selection)
                {
                    if (rv != null)
                    {
                        HIS_TRACKING tracking = new HIS_TRACKING();
                        Inventec.Common.Mapper.DataObjectMapper.Map<HIS_TRACKING>(tracking, rv);
                        this.Listtrackings.Add(tracking);
                        typeName += Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(tracking.TRACKING_TIME) + ", ";
                    }
                }
                cboTracking.Text = typeName;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitCheck(GridLookUpEdit cbo, GridCheckMarksSelection.SelectionChangedEventHandler eventSelect)
        {
            try
            {
                cbo.Properties.View.Columns.Clear();
                GridCheckMarksSelection gridCheck = new GridCheckMarksSelection(cbo.Properties);
                gridCheck.SelectionChanged += new GridCheckMarksSelection.SelectionChangedEventHandler(eventSelect);
                cbo.Properties.Tag = gridCheck;
                cbo.Properties.View.OptionsSelection.CheckBoxSelectorColumnWidth = 30;
                cbo.Properties.View.OptionsSelection.MultiSelect = true;
                GridCheckMarksSelection gridCheckMark = cbo.Properties.Tag as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    gridCheckMark.ClearSelection(cbo.Properties.View);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private async Task LoadDataByPackageService(V_HIS_SERE_SERV sereServ)
        {
            try
            {
                if (sereServ != null)
                {
                    LogSystem.Debug("LoadDataByPackageService .1");
                    CommonParam param = new CommonParam();
                    //Lấy list service package
                    HisServicePackageViewFilter filter = new HisServicePackageViewFilter();
                    filter.SERVICE_ID = sereServ.SERVICE_ID;
                    var servicePackageByServices = await new BackendAdapter(param).GetAsync<List<V_HIS_SERVICE_PACKAGE>>(HisRequestUriStore.HIS_SERVICE_PACKAGE_GETVIEW, ApiConsumers.MosConsumer, filter, ProcessLostToken, param);
                    if (servicePackageByServices != null && servicePackageByServices.Count > 0)
                    {
                        List<long> serviceIds = servicePackageByServices.Select(o => o.SERVICE_ATTACH_ID).Distinct().ToList();

                        MOS.Filter.HisServiceViewFilter filterMedicine = new HisServiceViewFilter();
                        filterMedicine.IDs = serviceIds;
                        Inventec.Common.Logging.LogSystem.Debug("begin call HisService/GetView");
                        List<MOS.EFMODEL.DataModels.V_HIS_SERVICE> serviceInPackages = await new BackendAdapter(param).GetAsync<List<V_HIS_SERVICE>>("api/HisService/GetView", ApiConsumer.ApiConsumers.MosConsumer, filterMedicine, param);
                        Inventec.Common.Logging.LogSystem.Debug("end call HisService/GetView");
                        //Load data for service package
                        this.LoadPageServiceInServicePackage(serviceInPackages);

                        //Tính lại tổng số tiền đã thanh toán là hao phí trong gói
                        await this.SetTotalPriceInPackage(sereServ);
                    }
                    else
                    {
                        this.isNotHandlerWhileChangeToggetSwith = true;
                        this.toggleSwitchDataChecked.EditValue = false;
                        this.LoadDataToGrid(true);
                        this.isNotHandlerWhileChangeToggetSwith = false;
                    }
                    LogSystem.Debug("LoadDataByPackageService .2");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private async Task SetTotalPriceInPackage(V_HIS_SERE_SERV sereServ)
        {
            try
            {
                CommonParam param = new CommonParam();
                //Lấy list service package
                HisSereServFilter sereServFilter = new HisSereServFilter();
                sereServFilter.IS_EXPEND = true;
                sereServFilter.PARENT_ID = sereServ.ID;
                var serviceInPackage__Expends = await new BackendAdapter(param).GetAsync<List<HIS_SERE_SERV>>(HisRequestUriStore.HIS_SERE_SERV_GET, ApiConsumers.MosConsumer, sereServFilter, ProcessLostToken, param);
                if (serviceInPackage__Expends != null && serviceInPackage__Expends.Count > 0)
                {
                    this.currentExpendInServicePackage = serviceInPackage__Expends.Sum(o => (o.AMOUNT * o.PRICE));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private async Task LoadPageServiceInAppointmentServices(long treatmentId)
        {
            try
            {
                this.gridViewServiceProcess.BeginUpdate();
                var allDatas = this.ServiceIsleafADOs.AsQueryable();
                treeService.UncheckAll();
                if (treatmentId > 0)
                {
                    // get appointmentServices
                    CommonParam param = new CommonParam();
                    MOS.Filter.HisAppointmentServFilter appointmentServiceFilter = new HisAppointmentServFilter();
                    appointmentServiceFilter.TREATMENT_ID = treatmentId;
                    var AppointmentServices = await new BackendAdapter(param).GetAsync<List<HIS_APPOINTMENT_SERV>>("api/HisAppointmentServ/Get", ApiConsumer.ApiConsumers.MosConsumer, appointmentServiceFilter, param);
                    if (AppointmentServices != null && AppointmentServices.Count > 0)
                    {
                        cboUser.EditValue = AppointmentServices[0].CREATOR;
                        txtLoginName.Text = AppointmentServices[0].CREATOR;
                        CheckAssignServiceSimultaneityOption();
                        var serviceIds = AppointmentServices.Select(o => o.SERVICE_ID).Distinct().ToArray();
                        allDatas = allDatas.Where(o => serviceIds.Contains(o.ID));
                        var resultData = allDatas.ToList();
                        if (resultData != null && resultData.Count > 0)
                        {
                            foreach (var sereServADO in resultData)
                            {
                                var appointmentService = AppointmentServices.FirstOrDefault(o => o.SERVICE_ID == sereServADO.SERVICE_ID);
                                long? appointmentPatyId = null;
                                if (appointmentService != null)
                                {
                                    sereServADO.AMOUNT = appointmentService.AMOUNT;
                                    appointmentPatyId = appointmentService.PATIENT_TYPE_ID;
                                }
                                sereServADO.IsChecked = true;
                                this.ChoosePatientTypeDefaultlService(this.currentHisPatientTypeAlter.PATIENT_TYPE_ID, sereServADO.SERVICE_ID, sereServADO, false, appointmentPatyId);
                                this.FillDataOtherPaySourceDataRow(sereServADO);

                                List<V_HIS_EXECUTE_ROOM> executeRoomList = null;
                                FilterExecuteRoom(sereServADO, ref executeRoomList);

                                long executeRoomId = this.SetPriorityRequired(executeRoomList);

                                if (executeRoomId <= 0)
                                    executeRoomId = this.SetDefaultExcuteRoom(executeRoomList);

                                //data.TDL_EXECUTE_ROOM_ID = executeRoomDefault;
                                if (sereServADO.TDL_EXECUTE_ROOM_ID <= 0 && executeRoomId > 0)
                                {
                                    sereServADO.TDL_EXECUTE_ROOM_ID = executeRoomId;
                                }
                                this.ValidServiceDetailProcessing(sereServADO);
                                this.SetAssignNumOrder(sereServADO);
                            }
                            this.toggleSwitchDataChecked.EditValue = true;
                        }
                        this.gridViewServiceProcess.GridControl.DataSource = resultData.OrderByDescending(o => o.SERVICE_NUM_ORDER).ToList();

                        this.SetEnableButtonControl(this.actionType);
                        this.SetDefaultSerServTotalPrice();
                    }
                    else
                    {
                        this.isNotHandlerWhileChangeToggetSwith = true;
                        this.toggleSwitchDataChecked.EditValue = false;
                        this.LoadDataToGrid(true);
                        this.isNotHandlerWhileChangeToggetSwith = false;
                    }
                }
                this.gridViewServiceProcess.EndUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private async void LoadPageServiceInServicePackage(List<MOS.EFMODEL.DataModels.V_HIS_SERVICE> serviceInPackages)
        {
            try
            {
                this.gridViewServiceProcess.BeginUpdate();
                var allDatas = this.ServiceIsleafADOs.AsQueryable();
                treeService.UncheckAll();
                if (serviceInPackages != null && serviceInPackages.Count > 0)
                {
                    var serviceIds = serviceInPackages.Select(o => o.ID).Distinct().ToArray();
                    allDatas = allDatas.Where(o => serviceIds.Contains(o.ID));
                }
                var resultData = allDatas.ToList();
                if (resultData != null && resultData.Count > 0)
                {
                    foreach (var sereServADO in resultData)
                    {
                        sereServADO.IsChecked = true;

                        this.ChoosePatientTypeDefaultlService(this.currentHisPatientTypeAlter.PATIENT_TYPE_ID, sereServADO.SERVICE_ID, sereServADO);

                        if (!VerifyCheckFeeWhileAssign())
                        {
                            this.ResetOneService(sereServADO);
                            sereServADO.IsChecked = false;
                            break;
                        }
                        this.SetAssignNumOrder(sereServADO);
                        this.FillDataOtherPaySourceDataRow(sereServADO);
                        this.ValidServiceDetailProcessing(sereServADO);
                    }
                    this.toggleSwitchDataChecked.EditValue = true;

                    this.gridViewServiceProcess.GridControl.DataSource = resultData.OrderByDescending(o => o.SERVICE_NUM_ORDER).ToList();
                    this.gridViewServiceProcess.EndUpdate();

                    this.SetEnableButtonControl(this.actionType);
                    this.SetDefaultSerServTotalPrice();
                }
                else
                {
                    Inventec.Common.Logging.LogSystem.Debug("Truong hop co dich vu duoc cau hinh trong goi dich vu nhung dich vu do khong co trong danh sach dich vu hop le duoc phep hien thi__ hien thi tat cả dich vụ");
                    this.isNotHandlerWhileChangeToggetSwith = true;
                    if ((bool)this.toggleSwitchDataChecked.EditValue == true)
                        this.toggleSwitchDataChecked.EditValue = false;
                    else
                        this.LoadDataToGrid(true);
                    this.isNotHandlerWhileChangeToggetSwith = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadDataToCurrentTreatmentData(long treatmentId, long intructionTime)
        {
            try
            {
                CommonParam param = new CommonParam();
                MOS.Filter.HisTreatmentWithPatientTypeInfoFilter filter = new MOS.Filter.HisTreatmentWithPatientTypeInfoFilter();
                filter.TREATMENT_ID = treatmentId;
                if (HisConfigCFG.IsUsingServerTime == commonString__true)
                {
                    filter.INTRUCTION_TIME = null;
                }
                else
                {
                    filter.INTRUCTION_TIME = intructionTime;
                }
                var hisTreatments = new BackendAdapter(param).Get<List<HisTreatmentWithPatientTypeInfoSDO>>(RequestUriStore.HIS_TREATMENT_GET_TREATMENT_WITH_PATIENT_TYPE_INFO_SDO, ApiConsumers.MosConsumer, filter, ProcessLostToken, param);
                this.currentHisTreatment = hisTreatments != null && hisTreatments.Count > 0 ? hisTreatments.FirstOrDefault() : null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetDateUc()
        {
            try
            {
                if (this.currentHisTreatment != null && HisConfigCFG.IsUsingServerTime == commonString__true
                   && this.currentHisTreatment.SERVER_TIME > 0)
                {
                    DateInputADO ip = new DateInputADO();
                    ip.Time = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(this.currentHisTreatment.SERVER_TIME).Value;
                    ip.Dates = new List<DateTime?>() { ip.Time.Date };
                    UcDateSetValue(ip);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ProcessPatientSelecttWithPatientTypeInfo()
        {
            try
            {
                dicPatientType = new Dictionary<long, List<HIS_PATIENT_TYPE>>();
                var lstPatientSelect = this.patientSelectProcessor.GetSelectedRows(this.ucPatientSelect);
                var lstPatientType = lstPatientSelect.Select(o => new { o.TDL_PATIENT_TYPE_ID, o.PATIENT_TYPE_CODE }).Distinct().ToList();
                if (this.currentPatientTypeAllows != null && this.currentPatientTypes != null)
                {
                    foreach (var item in lstPatientType)
                    {
                        if (dicPatientType.ContainsKey(item.TDL_PATIENT_TYPE_ID ?? 0))
                            continue;
                        var patientType = this.currentPatientTypes.FirstOrDefault(o => o.PATIENT_TYPE_CODE == item.PATIENT_TYPE_CODE);
                        if (patientType == null) throw new AggregateException("Khong lay duoc thong tin PatientType theo ma doi tuong (PATIENT_TYPE trong HisTreatmentWithPatientTypeInfoSDO).");
                        var patientTypeAllow = this.currentPatientTypeAllows.Where(o => o.PATIENT_TYPE_ID == patientType.ID).Select(m => m.PATIENT_TYPE_ALLOW_ID).Distinct().ToList();
                        var dt = ((patientTypeAllow != null && patientTypeAllow.Count > 0) ? currentPatientTypes.Where(o => patientTypeAllow.Contains(o.ID)).OrderBy(o => o.PRIORITY).ToList() : new List<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE>());
                        dicPatientType[item.TDL_PATIENT_TYPE_ID ?? 0] = dt;
                    }
                }
                else
                    throw new AggregateException("patientTypeAllows is null");
            }
            catch (AggregateException ex)
            {
                WaitingManager.Hide();
                MessageManager.Show(ResourceMessage.KhongTimThayDoiTuongThanhToanTrongThoiGianYLenh);
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ProcessDataWithTreatmentWithPatientTypeInfo()
        {
            try
            {
                if (this.currentPatientTypeAllows != null && this.currentPatientTypes != null)
                {
                    if (this.currentHisTreatment != null && !String.IsNullOrEmpty(this.currentHisTreatment.PATIENT_TYPE_CODE))
                    {
                        var patientType = this.currentPatientTypes.FirstOrDefault(o => o.PATIENT_TYPE_CODE == this.currentHisTreatment.PATIENT_TYPE_CODE);
                        if (patientType == null) throw new AggregateException("Khong lay duoc thong tin PatientType theo ma doi tuong (PATIENT_TYPE trong HisTreatmentWithPatientTypeInfoSDO).");

                        this.currentHisPatientTypeAlter = new V_HIS_PATIENT_TYPE_ALTER();
                        this.currentHisPatientTypeAlter.PATIENT_TYPE_ID = patientType.ID;
                        this.currentHisPatientTypeAlter.PATIENT_TYPE_CODE = patientType.PATIENT_TYPE_CODE;
                        this.currentHisPatientTypeAlter.PATIENT_TYPE_NAME = patientType.PATIENT_TYPE_NAME;
                        this.currentHisPatientTypeAlter.TREATMENT_TYPE_CODE = this.currentHisTreatment.TREATMENT_TYPE_CODE;
                        this.currentHisPatientTypeAlter.HEIN_MEDI_ORG_CODE = this.currentHisTreatment.HEIN_MEDI_ORG_CODE;
                        this.currentHisPatientTypeAlter.HEIN_CARD_FROM_TIME = this.currentHisTreatment.HEIN_CARD_FROM_TIME;
                        this.currentHisPatientTypeAlter.HEIN_CARD_TO_TIME = this.currentHisTreatment.HEIN_CARD_TO_TIME;
                        this.currentHisPatientTypeAlter.HEIN_CARD_NUMBER = this.currentHisTreatment.HEIN_CARD_NUMBER;
                        this.currentHisPatientTypeAlter.RIGHT_ROUTE_TYPE_CODE = this.currentHisTreatment.RIGHT_ROUTE_TYPE_CODE;
                        this.currentHisPatientTypeAlter.LEVEL_CODE = this.currentHisTreatment.LEVEL_CODE;
                        this.currentHisPatientTypeAlter.RIGHT_ROUTE_CODE = this.currentHisTreatment.RIGHT_ROUTE_CODE;
                        var tt = BackendDataWorker.Get<HIS_TREATMENT_TYPE>().FirstOrDefault(o => o.TREATMENT_TYPE_CODE == this.currentHisTreatment.TREATMENT_TYPE_CODE);
                        this.currentHisPatientTypeAlter.TREATMENT_TYPE_ID = (tt != null ? tt.ID : 0);
                        this.currentHisPatientTypeAlter.TREATMENT_TYPE_NAME = (tt != null ? tt.TREATMENT_TYPE_NAME : "");

                        var patientTypeAllow = this.currentPatientTypeAllows.Where(o => o.PATIENT_TYPE_ID == patientType.ID).Select(m => m.PATIENT_TYPE_ALLOW_ID).Distinct().ToList();

                        this.currentPatientTypeWithPatientTypeAlter = ((patientTypeAllow != null && patientTypeAllow.Count > 0) ? currentPatientTypes.Where(o => patientTypeAllow.Contains(o.ID)).OrderBy(o => o.PRIORITY).ToList() : new List<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE>());
                        if (HisConfigCFG.IsAssignRoomByPatientType && currentPatientTypeWithPatientTypeAlter != null && currentPatientTypeWithPatientTypeAlter.Count > 0)
                        {
                            MOS.Filter.HisPatientTypeRoomFilter _patienttypeRoomFIlter = new MOS.Filter.HisPatientTypeRoomFilter();
                            _patienttypeRoomFIlter.PATIENT_TYPE_IDs = currentPatientTypeWithPatientTypeAlter.Select(o => o.ID).ToList();
                            _patienttypeRoomFIlter.IS_ACTIVE = (short)1;
                            PatientTypeRooms = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<HIS_PATIENT_TYPE_ROOM>>("api/HisPatientTypeRoom/Get", ApiConsumers.MosConsumer, _patienttypeRoomFIlter, null);
                        }
                    }
                    else
                        throw new AggregateException("currentHisTreatment.PATIENT_TYPE_CODE is null");
                }
                else
                    throw new AggregateException("patientTypeAllows is null");
            }
            catch (AggregateException ex)
            {
                this.currentHisPatientTypeAlter = new V_HIS_PATIENT_TYPE_ALTER();
                this.currentPatientTypeWithPatientTypeAlter = new List<HIS_PATIENT_TYPE>();
                WaitingManager.Hide();
                MessageManager.Show(ResourceMessage.KhongTimThayDoiTuongThanhToanTrongThoiGianYLenh);
                Inventec.Common.Logging.LogSystem.Warn("LoadDataToCurrentTreatmentData => khong lay duoc doi tuong benh nhan. Dau vao____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => treatmentId), treatmentId) + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => intructionTimeSelecteds), intructionTimeSelecteds) + "____Dau ra____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => currentHisTreatment), currentHisTreatment));
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void PatientTypeWithPatientTypeAlter()
        {
            try
            {
                if (this.currentPatientTypeAllows != null && this.currentPatientTypeAllows.Count > 0 && this.currentPatientTypes != null)
                {
                    if (this.currentHisPatientTypeAlter != null)
                    {
                        var patientTypeAllow = this.currentPatientTypeAllows.Where(o => o.PATIENT_TYPE_ID == this.currentHisPatientTypeAlter.PATIENT_TYPE_ID).Select(m => m.PATIENT_TYPE_ALLOW_ID).Distinct().ToList();
                        this.currentPatientTypeWithPatientTypeAlter = (patientTypeAllow != null && patientTypeAllow.Count > 0) ? this.currentPatientTypes.Where(o => patientTypeAllow.Contains(o.ID)).OrderBy(o => o.PRIORITY).ToList() : null;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private async Task FillDataToControlsForm()
        {
            try
            {
                this.InitComboRepositoryPatientType(this.currentPatientTypeWithPatientTypeAlter);
                this.InitDefaultDataByPatientType();
                this.LoadShareCount();
                this.ValidateForm();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadShareCount()
        {
            try
            {
                List<ShareCountADO> shareCounts = new List<ShareCountADO>();
                shareCounts.Add(new ShareCountADO(2));
                shareCounts.Add(new ShareCountADO(3));
                shareCounts.Add(new ShareCountADO(4));
                shareCounts.Add(new ShareCountADO(5));
                shareCounts.Add(new ShareCountADO(6));
                shareCounts.Add(new ShareCountADO(7));
                shareCounts.Add(new ShareCountADO(8));
                shareCounts.Add(new ShareCountADO(9));
                shareCounts.Add(new ShareCountADO(10));

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("ShareCount", "", 100, 1));
                ControlEditorADO controlEditorADO = new ControlEditorADO("ShareCount", "ShareCount", columnInfos, false, 100);
                ControlEditorLoader.Load(this.repositoryItemcboShareCount, shareCounts, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void LoadIcdTranditionalToControl(string icdCode, string icdName)
        {
            try
            {
                if (icdYhctProcessor != null)
                {
                    UC.Icd.ADO.IcdInputADO icdYhct = new UC.Icd.ADO.IcdInputADO();
                    icdYhct.ICD_CODE = icdCode;
                    icdYhct.ICD_NAME = icdName;
                    if (ucIcdYhct != null)
                    {
                        this.icdYhctProcessor.Reload(ucIcdYhct, icdYhct);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void LoadIcdSubTranditionalToControl(string icdCode, string icdName)
        {
            try
            {
                SecondaryIcdDataADO subYhctIcd = new SecondaryIcdDataADO();
                subYhctIcd.ICD_SUB_CODE = icdCode;
                subYhctIcd.ICD_TEXT = icdName;
                if (ucSecondaryIcdYhct != null)
                {
                    subIcdYhctProcessor.Reload(ucSecondaryIcdYhct, subYhctIcd);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        /// <summary>
        /// Lay Chan doan mac dinh: Lay chan doan cuoi cung trong cac xu ly dich vu Kham benh
        /// </summary>
        HIS_ICD icdMain = null; 
        private void LoadIcdDefault()
        {
            try
            {
                this.isNotProcessWhileChangedTextSubIcd = true;
                Inventec.Common.Logging.LogSystem.Debug("LoadIcdDefault. 1");
                if (tracking != null && !String.IsNullOrEmpty(tracking.TRADITIONAL_ICD_CODE) && HisConfigCFG.TrackingCreate__UpdateTreatmentIcd == "1")
                {
                    this.LoadIcdTranditionalToControl(tracking.TRADITIONAL_ICD_CODE, tracking.TRADITIONAL_ICD_NAME);
                    this.LoadIcdSubTranditionalToControl(tracking.TRADITIONAL_ICD_SUB_CODE, tracking.TRADITIONAL_ICD_TEXT);
                }
                else if ((HisConfigCFG.IsloadIcdFromExamServiceExecute || (currentHisTreatment != null && String.IsNullOrEmpty(currentHisTreatment.TRADITIONAL_ICD_CODE))) && this.icdExam != null)
                {
                    this.LoadIcdTranditionalToControl(icdExam.TRADITIONAL_ICD_CODE, icdExam.TRADITIONAL_ICD_NAME);
                    this.LoadIcdSubTranditionalToControl(icdExam.TRADITIONAL_ICD_SUB_CODE, icdExam.TRADITIONAL_ICD_TEXT);
                }
                else if (this.currentHisTreatment != null)
                {
                    this.LoadIcdTranditionalToControl(currentHisTreatment.TRADITIONAL_ICD_CODE, currentHisTreatment.TRADITIONAL_ICD_NAME);
                    this.LoadIcdSubTranditionalToControl(currentHisTreatment.TRADITIONAL_ICD_SUB_CODE, currentHisTreatment.TRADITIONAL_ICD_TEXT);
                }


                if (this.tracking != null && !String.IsNullOrEmpty(this.tracking.ICD_CODE) && HisConfigCFG.TrackingCreate__UpdateTreatmentIcd == "1")
                {
                    this.LoadIcdToControl(this.tracking.ICD_CODE, this.tracking.ICD_NAME);

                    if ((HisConfigCFG.IsloadIcdFromExamServiceExecute || (currentHisTreatment != null && String.IsNullOrEmpty(currentHisTreatment.ICD_CODE))) && this.icdExam != null && !String.IsNullOrEmpty(this.icdExam.ICD_CODE))
                    {
                        this.LoadIcdCauseToControl(this.icdExam.ICD_CAUSE_CODE, this.icdExam.ICD_CAUSE_NAME);
                    }
                    else if (this.currentHisTreatment != null)
                    {
                        //Nếu hồ sơ chưa có thông tin ICD, và là hồ sơ đến khám theo loại là hẹn khám thì khi chỉ định dịch vụ, tự động hiển thị ICD của đợt điều trị trước, tương ứng với mã hẹn khám
                        if (string.IsNullOrEmpty(this.currentHisTreatment.ICD_CODE)
                            && !String.IsNullOrEmpty(this.currentHisTreatment.PREVIOUS_ICD_CODE))
                        {

                        }
                        else
                        {
                            LoadIcdCauseToControl(currentHisTreatment.ICD_CAUSE_CODE, this.currentHisTreatment.ICD_CAUSE_NAME);
                        }
                    }

                    icdMain = this.currentIcds.FirstOrDefault(o => o.ICD_CODE == this.tracking.ICD_CODE);
                    if (icdMain != null)
                    {
                        LoadRequiredCause((icdMain.IS_REQUIRE_CAUSE == 1));
                    }

                    this.LoadDataToIcdSub(this.tracking.ICD_SUB_CODE, this.tracking.ICD_TEXT);

                    Inventec.Common.Logging.LogSystem.Debug("LoadIcdDefault. 2");
                }
                else if ((HisConfigCFG.IsloadIcdFromExamServiceExecute || (currentHisTreatment != null && String.IsNullOrEmpty(currentHisTreatment.ICD_CODE))) && this.icdExam != null && !String.IsNullOrEmpty(this.icdExam.ICD_CODE))
                {
                    this.LoadIcdToControl(this.icdExam.ICD_CODE, this.icdExam.ICD_NAME);
                    this.LoadIcdCauseToControl(this.icdExam.ICD_CAUSE_CODE, this.icdExam.ICD_CAUSE_NAME);

                    icdMain = this.currentIcds.FirstOrDefault(o => o.ICD_CODE == this.icdExam.ICD_CODE);
                    if (icdMain != null)
                    {
                        LoadRequiredCause((icdMain.IS_REQUIRE_CAUSE == 1));
                    }

                    this.LoadDataToIcdSub(this.icdExam.ICD_SUB_CODE, this.icdExam.ICD_TEXT);

                    Inventec.Common.Logging.LogSystem.Debug("LoadIcdDefault. 3");
                }
                else if (this.currentHisTreatment != null)
                {
                    //Nếu hồ sơ chưa có thông tin ICD, và là hồ sơ đến khám theo loại là hẹn khám thì khi chỉ định dịch vụ, tự động hiển thị ICD của đợt điều trị trước, tương ứng với mã hẹn khám
                    if (string.IsNullOrEmpty(this.currentHisTreatment.ICD_CODE)
                        && !String.IsNullOrEmpty(this.currentHisTreatment.PREVIOUS_ICD_CODE))
                    {
                        HIS.UC.Icd.ADO.IcdInputADO icd = new HIS.UC.Icd.ADO.IcdInputADO();
                        icd.ICD_CODE = currentHisTreatment.PREVIOUS_ICD_CODE;
                        icd.ICD_NAME = this.currentHisTreatment.PREVIOUS_ICD_NAME;
                        icdMain = this.currentIcds.FirstOrDefault(o => o.ICD_CODE == currentHisTreatment.PREVIOUS_ICD_CODE);

                        LoadIcdToControl(currentHisTreatment.PREVIOUS_ICD_CODE, this.currentHisTreatment.PREVIOUS_ICD_NAME);
                        if (icdMain != null)
                        {
                            LoadRequiredCause((icdMain.IS_REQUIRE_CAUSE == 1));
                        }

                        LoadIcdToControlIcdSub(this.currentHisTreatment.PREVIOUS_ICD_SUB_CODE, this.currentHisTreatment.PREVIOUS_ICD_TEXT);
                    }
                    else
                    {
                        HIS.UC.Icd.ADO.IcdInputADO icd = new HIS.UC.Icd.ADO.IcdInputADO();
                        icd.ICD_CODE = currentHisTreatment.ICD_CODE;
                        icd.ICD_NAME = this.currentHisTreatment.ICD_NAME;
                        icdMain = this.currentIcds.FirstOrDefault(o => o.ICD_CODE == currentHisTreatment.ICD_CODE);
                        LoadIcdToControl(currentHisTreatment.ICD_CODE, this.currentHisTreatment.ICD_NAME);
                        if (icdMain != null)
                        {
                            LoadRequiredCause((icdMain.IS_REQUIRE_CAUSE == 1));
                        }
                        LoadIcdCauseToControl(currentHisTreatment.ICD_CAUSE_CODE, this.currentHisTreatment.ICD_CAUSE_NAME);
                        LoadIcdToControlIcdSub(this.currentHisTreatment.ICD_SUB_CODE, this.currentHisTreatment.ICD_TEXT);
                    }
                    Inventec.Common.Logging.LogSystem.Debug("LoadIcdDefault. 4");
                }

                if (icdMain != null)
                {
                    Inventec.Common.Logging.LogSystem.Debug("LoadIcdDefault. 5");
                    DelegateSelectedIcd(icdMain);
                }

                string[] codes = this.txtIcdSubCode.Text.Split(IcdUtil.seperator.ToCharArray());
                this.icdSubcodeAdoChecks = (from m in this.currentIcds.Where(o => o.IS_TRADITIONAL != 1).ToList() select new ADO.IcdADO(m, codes)).ToList();

                customGridViewSubIcdName.BeginUpdate();
                customGridViewSubIcdName.GridControl.DataSource = this.icdSubcodeAdoChecks;
                customGridViewSubIcdName.EndUpdate();
                this.isNotProcessWhileChangedTextSubIcd = false;
                Inventec.Common.Logging.LogSystem.Debug("LoadIcdDefault. 6");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void EnableCboTracking()
        {
            try
            {
                if (this.currentHisPatientTypeAlter != null && (this.currentHisPatientTypeAlter.TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTNOITRU || this.currentHisPatientTypeAlter.TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTNGOAITRU))
                {
                    cboTracking.Enabled = true;
                }
                else
                {
                    cboTracking.Enabled = false;
                    cboTracking.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private async Task LoadTreatmentInfo__PatientType()
        {
            try
            {
                //decimal totalPrice = 0;
                //if (this.dSereServ1WithTreatment != null && this.dSereServ1WithTreatment.Count > 0)
                //{
                //    totalPrice = this.dSereServ1WithTreatment.Sum(o => o.VIR_TOTAL_HEIN_PRICE ?? 0);              
                //}
                string patientInfo = "";
                patientInfo += this.currentHisTreatment.TDL_PATIENT_NAME;
                if (this.patientDob > 0)
                    patientInfo += "    -    " + Inventec.Common.DateTime.Convert.TimeNumberToDateString(this.currentHisTreatment.TDL_PATIENT_DOB) + " (" + MPS.AgeUtil.CalculateFullAge(currentHisTreatment.TDL_PATIENT_DOB) + ") ";
                patientInfo += "    -    " + this.currentHisTreatment.TDL_PATIENT_GENDER_NAME;

                if (this.currentHisPatientTypeAlter != null)
                {
                    patientInfo += "    -    " + this.currentHisPatientTypeAlter.PATIENT_TYPE_NAME;
                    patientInfo += "    -    " + this.currentHisPatientTypeAlter.TREATMENT_TYPE_NAME;
                }
                this.lblPatientName.Text = patientInfo;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private async Task LoadDataSereServWithTreatment(HisTreatmentWithPatientTypeInfoSDO treatment, DateTime? intructionTime)
        {
            try
            {
                if (treatment != null)
                {
                    this.RefeshSereServInTreatmentData();
                    this.isNotHandlerWhileChangeToggetSwith = true;
                    if (!HisConfigCFG.IsNotAutoLoadServiceOpenAssignService)
                    {
                        this.LoadDataToGrid(true);//TODO
                    }

                    this.isNotHandlerWhileChangeToggetSwith = false;
                    this.FillDataToControlsForm();
                    this.InitDefaultDataService();
                    this.LoadDataToTrackingCombo();
                    this.CheckOverTotalPatientPrice();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        //private void CreateThreadLoadDataServiceToGrid(object param)
        //{
        //    Thread thread = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(LoadDataServiceToGridNewThread));
        //    thread.Priority = ThreadPriority.Normal;
        //    try
        //    {
        //        thread.Start(param);
        //    }
        //    catch (Exception ex)
        //    {
        //        LogSystem.Error(ex);
        //        thread.Abort();
        //    }
        //}

        private void LoadDataServiceToGridNewThread(object param)
        {
            try
            {
                if (this.InvokeRequired)
                {
                    this.Invoke(new MethodInvoker(delegate { this.LoadDataToGrid(false); }));
                }
                else
                {
                    this.LoadDataToGrid(false);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitDefaultDataByPatientType()
        {
            try
            {
                if (this.currentSereServ == null)
                {
                    //this.gridViewServiceProcess.Columns["IsOutKtcFee"].Visible = false;
                }

                this.SetPatientInfoToControl(); //thong tin ve BN                
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetPatientInfoToControl()
        {
            try
            {
                if (this.currentHisTreatment != null)
                {
                    //this.lblTreatmentCode_TabBlood.Text = currentHisTreatment.TREATMENT_CODE;
                    //this.lblPatientName_TabBlood.Text = currentHisTreatment.VIR_PATIENT_NAME;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void FillSampleType(SereServADO data, GridLookUpEdit sampleTypeCombo)
        {
            try
            {
                if (((HisConfigCFG.IntegrationVersionValue == "1" && HisConfigCFG.IntegrationOptionValue != "1") || (HisConfigCFG.IntegrationVersionValue == "2" && HisConfigCFG.IntegrationTypeValue != "1")) && data.SERVICE_TYPE_ID > 0 && serviceTypeIdSplitReq != null && serviceTypeIdSplitReq.Count > 0 && serviceTypeIdSplitReq.Exists(o => o == data.SERVICE_TYPE_ID))
                {
                    InitComboSampleType(sampleTypeCombo);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FillDataIntoPatientTypeCombo(SereServADO data, GridLookUpEdit patientTypeCombo)
        {
            try
            {
                if (patientTypeCombo != null)
                {
                    long intructionTime = this.intructionTimeSelecteds.FirstOrDefault();
                    long treatmentTime = this.currentHisTreatment.IN_TIME;
                    List<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE> dataCombo = null;
                    if (BranchDataWorker.HasServicePatyWithListPatientType(data.SERVICE_ID, this.patientTypeIdAls))
                    {
                        var patientTypeIdInSePas = BranchDataWorker.ServicePatyWithListPatientType(data.SERVICE_ID, this.patientTypeIdAls).Where(o => ((!o.TREATMENT_TO_TIME.HasValue || o.TREATMENT_TO_TIME.Value >= treatmentTime) || (!o.TO_TIME.HasValue || o.TO_TIME.Value >= intructionTime))).Select(s => s.PATIENT_TYPE_ID).Distinct().ToList();
                        var patientTypeAll = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE>().Where(o => o.IS_ACTIVE == 1).ToList();
                        if (patientTypeIdInSePas == null) patientTypeIdInSePas = new List<long>();
                        var patientTypeIdPlus = patientTypeAll.Where(k => k.BASE_PATIENT_TYPE_ID != null && patientTypeIdInSePas.Contains(k.BASE_PATIENT_TYPE_ID.Value)).ToList();
                        if (patientTypeIdPlus != null && patientTypeIdPlus.Count > 0)
                        {
                            patientTypeIdInSePas.AddRange(patientTypeIdPlus.Select(o => o.ID));
                        }
                        patientTypeIdInSePas = patientTypeIdInSePas.Distinct().ToList();

                        dataCombo = (patientTypeIdInSePas != null && patientTypeIdInSePas.Count > 0 ? currentPatientTypeWithPatientTypeAlter.Where(o =>
                                    patientTypeIdInSePas.Contains(o.ID) && (!this.isNotUseBhyt || (isNotUseBhyt && o.ID != HisConfigCFG.PatientTypeId__BHYT))).ToList() : null);
                    }
                    if (dataCombo != null && dataCombo.Count > 0 && data.DO_NOT_USE_BHYT == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE && !CheckLoginAdmin.IsAdmin(Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName()))
                        dataCombo = dataCombo.Where(o => o.ID != HisConfigCFG.PatientTypeId__BHYT).ToList();
                    this.InitComboPatientType(patientTypeCombo, dataCombo);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FillDataIntoPrimaryPatientTypeCombo(SereServADO data, GridLookUpEdit patientTypeCombo)
        {
            try
            {
                List<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE> dataCombo = new List<HIS_PATIENT_TYPE>();
                if (BranchDataWorker.HasServicePatyWithListPatientType(data.SERVICE_ID, this.patientTypeIdAls))
                {
                    long instructionTime = this.intructionTimeSelecteds != null && this.intructionTimeSelecteds.Count > 0 ? this.intructionTimeSelecteds.FirstOrDefault() : 0;
                    List<HIS_SERE_SERV> sameServiceType = this.sereServWithTreatment != null ? this.sereServWithTreatment.Where(o => o.TDL_SERVICE_TYPE_ID == data.SERVICE_TYPE_ID).ToList() : null;
                    long? intructionNumByType = sameServiceType != null ? (long)sameServiceType.Count() + 1 : 1;
                    List<V_HIS_SERVICE_PATY> servicePaties = BranchDataWorker.ServicePatyWithListPatientType(data.SERVICE_ID, this.patientTypeIdAls);

                    List<HIS_SERE_SERV> sameService = this.sereServWithTreatment != null ? this.sereServWithTreatment.Where(o => o.SERVICE_ID == data.SERVICE_ID).ToList() : null;
                    var intructionNum = sameService != null ? (long)sameService.Count() + 1 : 1;
                    var currentPaty = MOS.ServicePaty.ServicePatyUtil.GetApplied(servicePaties, data.TDL_EXECUTE_BRANCH_ID, null, this.requestRoom.ID, this.requestRoom.DEPARTMENT_ID, instructionTime, this.currentHisTreatment.IN_TIME, data.SERVICE_ID, data.PATIENT_TYPE_ID, intructionNum, intructionNumByType, data.PackagePriceId, data.SERVICE_CONDITION_ID, this.currentHisTreatment.TDL_PATIENT_CLASSIFY_ID, null);

                    var patientTypePrimatyList = this.currentPatientTypeWithPatientTypeAlter.Where(o => o.IS_ADDITION == (short)1).ToList();

                    var patyIds = servicePaties.Select(o => o.PATIENT_TYPE_ID).Distinct().ToList();
                    patyIds = patientTypePrimatyList != null && patientTypePrimatyList.Count > 0 ? patyIds.Where(o => patientTypePrimatyList.Select(p => p.ID).Contains(o)).ToList() : null;
                    if (patyIds != null)
                    {
                        foreach (var item in patyIds)
                        {
                            if (item == data.PATIENT_TYPE_ID)
                                continue;
                            var itemPaty = MOS.ServicePaty.ServicePatyUtil.GetApplied(servicePaties, data.TDL_EXECUTE_BRANCH_ID, null, this.requestRoom.ID, this.requestRoom.DEPARTMENT_ID, instructionTime, this.currentHisTreatment.IN_TIME, data.SERVICE_ID, item, intructionNum, intructionNumByType, data.PackagePriceId, data.SERVICE_CONDITION_ID, this.currentHisTreatment.TDL_PATIENT_CLASSIFY_ID, null);
                            if (itemPaty == null || currentPaty == null || (currentPaty.PRICE * (1 + currentPaty.VAT_RATIO)) >= (itemPaty.PRICE * (1 + itemPaty.VAT_RATIO)))
                                continue;
                            dataCombo.Add(this.currentPatientTypeWithPatientTypeAlter.FirstOrDefault(o => o.ID == item));
                        }
                    }
                }

                this.InitComboPatientType(patientTypeCombo, dataCombo);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FillDataIntoExcuteRoomCombo(SereServADO data, GridLookUpEdit excuteRoomCombo)
        {
            try
            {
                var serviceRoomViews = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_SERVICE_ROOM>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToArray();
                if (excuteRoomCombo != null && this.allDataExecuteRooms != null && serviceRoomViews != null && serviceRoomViews.Count() > 0)
                {
                    var arrExcuteRoom = serviceRoomViews.Where(o => data != null && o.SERVICE_ID == data.SERVICE_ID).ToList();
                    if (HisConfigCFG.IsAssignRoomByPatientType && PatientTypeRooms != null && PatientTypeRooms.Count > 0 && PatientTypeRooms.Exists(o => o.PATIENT_TYPE_ID == data.PATIENT_TYPE_ID))
                    {
                        var RoomIds = PatientTypeRooms.Where(o => o.PATIENT_TYPE_ID == data.PATIENT_TYPE_ID).Select(o => o.ROOM_ID).ToList();
                        arrExcuteRoom = arrExcuteRoom.Where(o => RoomIds.Contains(o.ROOM_ID)).ToList();
                    }
                    var arrExcuteRoomIds = arrExcuteRoom.Select(o => o.ROOM_ID).ToArray();
                    var dataComboExcuteRooms = ((arrExcuteRoomIds != null && arrExcuteRoomIds.Count() > 0 && this.allDataExecuteRooms != null) ? this.allDataExecuteRooms.Where(o => arrExcuteRoomIds.Contains(o.ROOM_ID)).ToList() : null);
                    if (this.IsTreatmentInBedRoom)
                    {
                        ProcessAddBedRoomToExecuteRoom(arrExcuteRoomIds.ToList(), ref dataComboExcuteRooms);
                    }

                    this.InitComboExecuteRoom(excuteRoomCombo, dataComboExcuteRooms);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FilterExecuteRoom(SereServADO data, ref List<V_HIS_EXECUTE_ROOM> executeRoomList)
        {
            try
            {
                var serviceRoomViews = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_SERVICE_ROOM>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToArray();
                if (this.allDataExecuteRooms != null && serviceRoomViews != null && serviceRoomViews.Count() > 0)
                {
                    var arrExcuteRoom = serviceRoomViews.Where(o => data != null && o.SERVICE_ID == data.SERVICE_ID).ToList();
                    if (HisConfigCFG.IsAssignRoomByPatientType && PatientTypeRooms != null && PatientTypeRooms.Count > 0 && PatientTypeRooms.Exists(o => o.PATIENT_TYPE_ID == data.PATIENT_TYPE_ID))
                    {
                        var RoomIds = PatientTypeRooms.Where(o => o.PATIENT_TYPE_ID == data.PATIENT_TYPE_ID).Select(o => o.ROOM_ID).ToList();
                        arrExcuteRoom = arrExcuteRoom.Where(o => RoomIds.Contains(o.ROOM_ID)).ToList();
                    }
                    var arrExcuteRoomIds = arrExcuteRoom.Select(o => o.ROOM_ID).ToArray();
                    executeRoomList = ((arrExcuteRoomIds != null && arrExcuteRoomIds.Count() > 0 && this.allDataExecuteRooms != null) ? this.allDataExecuteRooms.Where(o => arrExcuteRoomIds.Contains(o.ROOM_ID)).ToList() : null);
                    List<MOS.EFMODEL.DataModels.V_HIS_EXECUTE_ROOM> executeRoomFilters = ProcessExecuteRoom();
                    executeRoomList = (executeRoomFilters != null && executeRoomFilters.Count > 0 && executeRoomList != null && executeRoomList.Count > 0) ? executeRoomList.Where(p => executeRoomFilters.Select(o => o.ID).Distinct().Contains(p.ID)).ToList() : null;
                    if (this.IsTreatmentInBedRoom)
                    {
                        ProcessAddBedRoomToExecuteRoom(arrExcuteRoomIds.ToList(), ref executeRoomList);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private List<L_HIS_ROOM_COUNTER_1> GetLCounter1()
        {
            try
            {
                HisRoomCounterLView1Filter exetuteFilter = new HisRoomCounterLView1Filter();
                exetuteFilter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                exetuteFilter.BRANCH_ID = WorkPlace.GetBranchId();
                return new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<L_HIS_ROOM_COUNTER_1>>("api/HisRoom/GetCounterLView1", ApiConsumers.MosConsumer, exetuteFilter, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return null;
        }

        private async Task GetLCounter1Async()
        {
            try
            {
                HisRoomCounterLView1Filter exetuteFilter = new HisRoomCounterLView1Filter();
                exetuteFilter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                exetuteFilter.BRANCH_ID = WorkPlace.GetBranchId();
                this.hisRoomCounters = await new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).GetAsync<List<L_HIS_ROOM_COUNTER_1>>("api/HisRoom/GetCounterLView1", ApiConsumers.MosConsumer, exetuteFilter, null);
            }
            catch (Exception ex)
            {

            }
        }

        private void TimerGetDataGetLCounter1()
        {
            try
            {
                if (HisConfigCFG.MaxPatientByDay == 1)
                {
                    var startTimeSpan = TimeSpan.Zero;
                    var periodTimeSpan = TimeSpan.FromSeconds(30);
                    //var Timer = new System.Threading.Timer(GetLCounter1Async() , null, TimeSpan.Zero, TimeSpan.FromMinutes(5));
                    var timer = new System.Threading.Timer((e) =>
                    {
                        GetLCounter1Async();
                    }, null, startTimeSpan, periodTimeSpan);

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }
    }
}

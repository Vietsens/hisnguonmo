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
using HIS.Desktop.Plugins.AssignNoneMediService.ADO;
using HIS.Desktop.Plugins.AssignNoneMediService.Config;
using HIS.Desktop.Plugins.AssignNoneMediService.Resources;
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
using DevExpress.XtraExport;

namespace HIS.Desktop.Plugins.AssignNoneMediService.AssignService
{
    public partial class frmAssignService : HIS.Desktop.Utility.FormBase
    {
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


        /// <summary>
        /// Bổ sung cấu hình hệ thống: "HIS.Desktop.Plugins.AssignNoneMediService.SetRequestRoomByBedRoomWhenBeingInSurgery": "1: Tự động điền phòng chỉ định theo buồng bệnh nhân đang nằm nếu chỉ định DVKT ở màn hình xử lý PTTT"
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

        private void LoadDataToGrid(bool isResetSearchtext)
        {
            try
            {
                this.gridViewServiceProcess.ClearGrouping();
                var allDatas = this.ServiceIsleafADOs != null && this.ServiceIsleafADOs.Count > 0 ? this.ServiceIsleafADOs.AsQueryable() : null;
                List<SereServADO> listSereServADO = allDatas != null ? allDatas.ToList() : new List<SereServADO>();
                if (isResetSearchtext)
                {
                    this.notSearch = true;
                    this.notSearch = false;
                }

                if (this.toggleSwitchDataChecked.EditValue != null && (this.toggleSwitchDataChecked.EditValue ?? "").ToString().ToLower() == "true" && allDatas != null && allDatas.Count() > 0)
                {
                    listSereServADO = allDatas.Where(o => o.IsChecked).ToList();
                }
                this.gridControlServiceProcess.DataSource = null;
                this.gridControlServiceProcess.DataSource = listSereServADO;
                this.SetEnableButtonControl(this.actionType);
            }
            catch (Exception ex)
            {
                this.gridViewServiceProcess.EndUpdate();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
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


        List<HIS_SERE_SERV> lstSereServExist = new List<HIS_SERE_SERV>();


        int dfEndServiceReq = 10;
        int rowCount = 0;
        int dataTotal = 0;
        int startPage = 0;

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
                this.btnSave.Enabled = false;
                this.btnSaveAndPrint.Enabled = false;
                this.btnShowDetail.Enabled = false;
                this.actionType = GlobalVariables.ActionAdd;

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


                    icdMain = this.currentIcds.FirstOrDefault(o => o.ICD_CODE == this.tracking.ICD_CODE);
                    this.LoadDataToIcdSub(this.tracking.ICD_SUB_CODE, this.tracking.ICD_TEXT);

                    Inventec.Common.Logging.LogSystem.Debug("LoadIcdDefault. 2");
                }
                else if ((HisConfigCFG.IsloadIcdFromExamServiceExecute || (currentHisTreatment != null && String.IsNullOrEmpty(currentHisTreatment.ICD_CODE))) && this.icdExam != null && !String.IsNullOrEmpty(this.icdExam.ICD_CODE))
                {
                    this.LoadIcdToControl(this.icdExam.ICD_CODE, this.icdExam.ICD_NAME);

                    icdMain = this.currentIcds.FirstOrDefault(o => o.ICD_CODE == this.icdExam.ICD_CODE);

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

                        LoadIcdToControlIcdSub(this.currentHisTreatment.PREVIOUS_ICD_SUB_CODE, this.currentHisTreatment.PREVIOUS_ICD_TEXT);
                    }
                    else
                    {
                        HIS.UC.Icd.ADO.IcdInputADO icd = new HIS.UC.Icd.ADO.IcdInputADO();
                        icd.ICD_CODE = currentHisTreatment.ICD_CODE;
                        icd.ICD_NAME = this.currentHisTreatment.ICD_NAME;
                        icdMain = this.currentIcds.FirstOrDefault(o => o.ICD_CODE == currentHisTreatment.ICD_CODE);
                        LoadIcdToControl(currentHisTreatment.ICD_CODE, this.currentHisTreatment.ICD_NAME);
                        LoadIcdToControlIcdSub(this.currentHisTreatment.ICD_SUB_CODE, this.currentHisTreatment.ICD_TEXT);
                    }
                    Inventec.Common.Logging.LogSystem.Debug("LoadIcdDefault. 4");
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

        private async Task LoadTreatmentInfo__PatientType()
        {
            try
            {
                string patientInfo = "";
                patientInfo += this.currentHisTreatment.TDL_PATIENT_NAME;
                if (this.currentHisTreatment.TDL_PATIENT_DOB > 0)
                    patientInfo += "    -    " + Inventec.Common.DateTime.Convert.TimeNumberToDateString(this.currentHisTreatment.TDL_PATIENT_DOB) + " (" + MPS.AgeUtil.CalculateFullAge(currentHisTreatment.TDL_PATIENT_DOB) + ") ";
                patientInfo += "    -    " + this.currentHisTreatment.TDL_PATIENT_GENDER_NAME;

                patientInfo += "    -    " + BackendDataWorker.Get<HIS_PATIENT_TYPE>().FirstOrDefault(o=>o.ID == this.currentHisTreatment.TDL_PATIENT_TYPE_ID).PATIENT_TYPE_NAME;
                patientInfo += "    -    " + BackendDataWorker.Get<HIS_TREATMENT_TYPE>().FirstOrDefault(o => o.ID == this.currentHisTreatment.TDL_TREATMENT_TYPE_ID).TREATMENT_TYPE_NAME;
                this.lblPatientName.Text = patientInfo;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}

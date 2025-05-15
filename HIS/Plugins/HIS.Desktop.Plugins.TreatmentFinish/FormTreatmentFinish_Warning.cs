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
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Plugins.TreatmentFinish.ADO;
using HIS.Desktop.Plugins.TreatmentFinish.Config;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using MOS.UTILITY;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.TreatmentFinish
{
    public partial class FormTreatmentFinish : HIS.Desktop.Utility.FormBase
    {
        enum ValidationDataType
        {
            PopupMessage,
            GetListMessage
        }
        private bool CheckAssignServiceBed_ForSave(ValidationDataType validationDataType,ref List<WarningADO> listWarningADO)
        {
            bool valid = true;
            try
            {
                if (validationDataType == ValidationDataType.PopupMessage && this._isSkipWarningForSave == true)
                {
                    return valid;
                }
                if (HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("HIS.DESKTOP.TREATMENT_FINISH.CHECK_ASSIGN_SERVICE_BED") == "2")
                {
                    decimal amountBed = 0;
                    decimal amountTreat = Inventec.Common.TypeConvert.Parse.ToDecimal(txtDaysBedTreatment.Text);
                    List<HIS_SERE_SERV> listSereServBed = null;
                    if (this.SereServCheck != null && this.SereServCheck.Count > 0)
                    {
                        listSereServBed = this.SereServCheck.Where(o => o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__G).ToList();
                    }
                    if (listSereServBed != null && listSereServBed.Count > 0)
                    {
                        amountBed = listSereServBed.Sum(s => s.AMOUNT);
                    }
                    if (amountBed > amountTreat)
                    {
                        if (validationDataType == ValidationDataType.PopupMessage)
                        {
                            DevExpress.XtraEditors.XtraMessageBox.Show(String.Format(ResourceMessage.SoNgayGiuongLonHonSoNgayGiuongToiDa, amountBed, amountTreat), ResourceMessage.ThongBao);
                            return false;
                        }
                        else if (validationDataType == ValidationDataType.GetListMessage && listWarningADO != null)
                        {
                            WarningADO warning = new WarningADO();
                            warning.IsSkippable = false;
                            warning.Description = String.Format(ResourceMessage.SoNgayGiuongLonHonSoNgayGiuongToiDa, amountBed, amountTreat);
                            listWarningADO.Add(warning);
                        }
                    }
                    else if (amountBed < amountTreat)
                    {
                        if (validationDataType == ValidationDataType.PopupMessage)
                        {
                            DevExpress.XtraEditors.XtraMessageBox.Show(String.Format(ResourceMessage.SoNgayGiuongNhoHonSoNgayGiuongToiDa, amountBed, amountTreat), ResourceMessage.ThongBao);
                            return false;
                        }
                        else if (validationDataType == ValidationDataType.GetListMessage && listWarningADO != null)
                        {
                            WarningADO warning = new WarningADO();
                            warning.IsSkippable = false;
                            warning.Description = String.Format(ResourceMessage.SoNgayGiuongNhoHonSoNgayGiuongToiDa, amountBed, amountTreat);
                            listWarningADO.Add(warning);
                        }
                        
                    }
                }

                if (Config.CheckFinishTimeCFG.isCheckBedService)
                {
                    GetPatientTypeAlter();
                    if (patientTypeAlter != null && patientTypeAlter.TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTNOITRU)
                    {
                        List<HIS_SERE_SERV> listSereServBed = null;

                        if (this.SereServCheck != null && this.SereServCheck.Count > 0)
                        {
                            listSereServBed = this.SereServCheck.Where(o => o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__G).ToList();
                        }

                        if (listSereServBed == null || listSereServBed.Count <= 0)
                        {
                            if (validationDataType == ValidationDataType.PopupMessage)
                            {
                                if (DevExpress.XtraEditors.XtraMessageBox.Show(string.Format(ResourceMessage.BanCoMuonTiepTuc, ResourceMessage.BenhNhanNoiTruChuaDuocChiDinhDichVuGiuong), ResourceMessage.ThongBao, MessageBoxButtons.YesNo) == DialogResult.No)
                                {
                                    return false;
                                }
                            }
                            else if (validationDataType == ValidationDataType.GetListMessage && listWarningADO != null)
                            {
                                WarningADO warning = new WarningADO();
                                warning.IsSkippable = true;
                                warning.Description = ResourceMessage.BenhNhanNoiTruChuaDuocChiDinhDichVuGiuong;
                                listWarningADO.Add(warning);
                            }
                        }
                        else
                        {
                            var amountBed = listSereServBed.Sum(s => s.AMOUNT);
                            decimal amountTreat = Inventec.Common.TypeConvert.Parse.ToDecimal(txtDaysBedTreatment.Text);
                            if (amountBed < amountTreat)
                            {
                                if (validationDataType == ValidationDataType.PopupMessage)
                                {
                                    if (DevExpress.XtraEditors.XtraMessageBox.Show(String.Format(ResourceMessage.BanCoMuonTiepTuc, String.Format(ResourceMessage.SoNgayGiuongNhoHonSoNgayGiuongToiDa, amountBed, amountTreat)), ResourceMessage.ThongBao, MessageBoxButtons.YesNo) != System.Windows.Forms.DialogResult.Yes)
                                    {
                                        return false;
                                    }
                                }
                                else if (validationDataType == ValidationDataType.GetListMessage && listWarningADO != null)
                                {
                                    WarningADO warning = new WarningADO();
                                    warning.IsSkippable = true;
                                    warning.Description = String.Format(ResourceMessage.SoNgayGiuongNhoHonSoNgayGiuongToiDa, amountBed, amountTreat);
                                    listWarningADO.Add(warning);
                                }
                            }
                            else if (amountBed > amountTreat)
                            {
                                if (validationDataType == ValidationDataType.PopupMessage)
                                {
                                    if (DevExpress.XtraEditors.XtraMessageBox.Show(String.Format(ResourceMessage.BanCoMuonTiepTuc, String.Format(ResourceMessage.SoNgayGiuongLonHonSoNgayGiuongToiDa, amountBed, amountTreat)), ResourceMessage.ThongBao, MessageBoxButtons.YesNo) != System.Windows.Forms.DialogResult.Yes)
                                    {
                                        return false;
                                    }
                                }
                                else if (validationDataType == ValidationDataType.GetListMessage && listWarningADO != null)
                                {
                                    WarningADO warning = new WarningADO();
                                    warning.IsSkippable = true;
                                    warning.Description = String.Format(ResourceMessage.SoNgayGiuongLonHonSoNgayGiuongToiDa, amountBed, amountTreat);
                                    listWarningADO.Add(warning);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                valid = false;
            }
            return valid;
        }

        private bool Check_INTRUCTION_TIME_and_DEPARTMENT_IN_TIME_ForSave(ValidationDataType validationDataType, ref List<WarningADO> listWarningADO)
        {
            bool valid = true;
            try
            {
                if (validationDataType == ValidationDataType.PopupMessage && this._isSkipWarningForSave == true)
                {
                    return valid;
                }
                if (this.SereServCheck != null && this.SereServCheck.Count > 0 && this.ListDepartmentTran != null && this.ListDepartmentTran.Count > 0)
                {
                    //duyệt danh sách chuyển khoa theo thứ tự thời gian tăng dần
                    //lấy danh sách dịch vụ có khoa chỉ định ứng với khoa đó và có thời gian chỉ định lớn hơn thời gian ra khoa.
                    List<HIS_SERE_SERV> lstSereServOutTime = new List<HIS_SERE_SERV>();
                    ListDepartmentTran = ListDepartmentTran.OrderBy(o => o.DEPARTMENT_IN_TIME ?? 99999999999999).ThenBy(o => o.ID).ToList();
                    foreach (var item in ListDepartmentTran)
                    {
                        long checkTime = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtEndTime.DateTime) ?? 0;
                        var nextDepa = ListDepartmentTran.FirstOrDefault(o => o.PREVIOUS_ID == item.ID);
                        if (nextDepa != null && nextDepa.DEPARTMENT_IN_TIME.HasValue)
                        {
                            checkTime = nextDepa.DEPARTMENT_IN_TIME.Value;
                        }

                        //lấy các dịch vụ có thời gian chỉ định lớn hơn thời gian ra khoa và có thời gian chỉ định trong khoa.
                        var ssOutTime = this.SereServCheck.Where(o => o.AMOUNT != 0 && o.TDL_REQUEST_DEPARTMENT_ID == item.DEPARTMENT_ID && o.TDL_INTRUCTION_TIME > checkTime).ToList();
                        if (ssOutTime != null && ssOutTime.Count > 0)
                        {
                            lstSereServOutTime.AddRange(ssOutTime);
                        }

                        //bỏ các dịch vụ do khoa hiện tại chỉ định và có thời gian chỉ định lớn hơn thời gian vào khoa và nhỏ hơn thời gian ra khoa
                        if (lstSereServOutTime.Count > 0)
                        {
                            var lstRemove = lstSereServOutTime.Where(o => o.TDL_REQUEST_DEPARTMENT_ID == item.DEPARTMENT_ID && o.TDL_INTRUCTION_TIME > (item.DEPARTMENT_IN_TIME ?? 0) && o.TDL_INTRUCTION_TIME < checkTime).ToList();
                            if (lstRemove != null && lstRemove.Count > 0)
                            {
                                lstSereServOutTime = lstSereServOutTime.Except(lstRemove).ToList();
                            }
                        }
                    }

                    //tồn tại dịch vụ có thời gian chỉ định lớn hơn thời gian ra khoa
                    if (lstSereServOutTime.Count > 0)
                    {
                        string codes = string.Join(", ", lstSereServOutTime.Select(s => s.TDL_SERVICE_REQ_CODE).Distinct().OrderBy(o => o));
                        
                        if (validationDataType == ValidationDataType.PopupMessage)
                        {
                            if (DevExpress.XtraEditors.XtraMessageBox.Show(string.Format(ResourceMessage.YLenhCoThoiGianChiDinhLonHonThoiGianRaKhoa, codes) + " Bạn có muốn kết thúc điều trị không?", ResourceMessage.ThongBao, MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == DialogResult.No)
                            {
                                return false;
                            }
                        }
                        else if (validationDataType == ValidationDataType.GetListMessage && listWarningADO != null)
                        {
                            WarningADO warning = new WarningADO();
                            warning.IsSkippable = true;
                            warning.Description = String.Format(ResourceMessage.YLenhCoThoiGianChiDinhLonHonThoiGianRaKhoa, codes);
                            listWarningADO.Add(warning);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                valid = false;
            }
            return valid;
        }

        private bool CheckSameHein_ForSave(ValidationDataType validationDataType, ref List<WarningADO> listWarningADO)
        {
            bool valid = true;
            try
            {
                if (validationDataType == ValidationDataType.PopupMessage && this._isSkipWarningForSave == true)
                {
                    return valid;
                }
                if (HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(Config.CheckFinishTimeCFG.CHECK_SAME_HEIN) == "1" || HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(Config.CheckFinishTimeCFG.CHECK_SAME_HEIN) == "2")
                {
                    bool checkSameHein = false;
                    CommonParam param = new CommonParam();

                    HisPatientTypeAlterViewFilter patientTypeAlterFilter = new HisPatientTypeAlterViewFilter();
                    patientTypeAlterFilter.TREATMENT_ID = currentHisTreatment.ID;

                    var patientTypeAlter = new BackendAdapter(param).Get<List<V_HIS_PATIENT_TYPE_ALTER>>("api/HisPatientTypeAlter/GetView", ApiConsumers.MosConsumer, patientTypeAlterFilter, param);

                    if (patientTypeAlter != null && patientTypeAlter.Count >= 2)
                    {
                        foreach (var item in patientTypeAlter)
                        {
                            var sameHein = patientTypeAlter.Where(o => o.HEIN_CARD_NUMBER == item.HEIN_CARD_NUMBER).ToList();
                            if (sameHein != null && sameHein.Count >= 2)
                            {
                                var checkHeinOrg = sameHein.Select(o => o.HEIN_MEDI_ORG_CODE).Distinct().ToList();
                                if (checkHeinOrg.Count > 1)
                                {
                                    //Mã cskcb khác nhau
                                    checkSameHein = true;
                                    break;
                                }
                                else
                                {
                                    var checkRightRoute = sameHein.Select(o => o.RIGHT_ROUTE_CODE).Distinct().ToList();
                                    if (checkRightRoute.Count == 1)
                                    {
                                        //Đúng tuyến và lý do đúng tuyến khác nhau
                                        if (checkRightRoute.FirstOrDefault() == MOS.LibraryHein.Bhyt.HeinRightRoute.HeinRightRouteCode.TRUE)
                                        {
                                            var checkRightRouteType = sameHein.Select(o => o.RIGHT_ROUTE_TYPE_CODE).Distinct().ToList();
                                            if (checkRightRouteType.Count > 1)
                                            {
                                                checkSameHein = true;
                                                break;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        //Trái tuyến
                                        checkSameHein = true;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    Inventec.Common.Logging.LogSystem.Info("Save treatmentFinish 4");

                    //issue 13722
                    if (checkSameHein && HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(Config.CheckFinishTimeCFG.CHECK_SAME_HEIN) == "1")
                    {
                        if (validationDataType == ValidationDataType.PopupMessage)
                        {
                            DevExpress.XtraEditors.XtraMessageBox.Show(ResourceMessage.BenhNhanCoThongTinBhytChuaDung, ResourceMessage.ThongBao);
                            return false;
                        }
                        else if (validationDataType == ValidationDataType.GetListMessage && listWarningADO != null)
                        {
                            WarningADO warning = new WarningADO();
                            warning.IsSkippable = false;
                            warning.Description = ResourceMessage.BenhNhanCoThongTinBhytChuaDung;
                            listWarningADO.Add(warning);
                        }
                    }
                    else if (checkSameHein && HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(Config.CheckFinishTimeCFG.CHECK_SAME_HEIN) == "2")
                    {
                        if (validationDataType == ValidationDataType.PopupMessage)
                        {
                            if (DevExpress.XtraEditors.XtraMessageBox.Show(string.Format(ResourceMessage.BanCoMuonTiepTuc, ResourceMessage.BenhNhanCoThongTinBhytChuaDung), ResourceMessage.ThongBao, MessageBoxButtons.YesNo) != DialogResult.Yes)
                            {
                                return false;
                            }
                        }
                        else if (validationDataType == ValidationDataType.GetListMessage && listWarningADO != null)
                        {
                            WarningADO warning = new WarningADO();
                            warning.IsSkippable = true;
                            warning.Description = ResourceMessage.BenhNhanCoThongTinBhytChuaDung;
                            listWarningADO.Add(warning);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                valid = false;
            }
            return valid;
        }

        private bool CheckRation_ForSave(ValidationDataType validationDataType, ref List<WarningADO> listWarningADO)
        {
            bool valid = true;
            try
            {
                if (validationDataType == ValidationDataType.PopupMessage && this._isSkipWarningForSave == true)
                {
                    return valid;
                }
                if (!CheckFinishTimeCFG.isWarningApproveRation || this.currentHisTreatment.TDL_TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__KHAM)
                {
                    return true;
                }

                CommonParam param = new CommonParam();
                HisTreatmentRationNotApproveFilter rationFilter = new HisTreatmentRationNotApproveFilter();
                rationFilter.TREATMENT_ID = this.currentHisTreatment.ID;
                List<HisTreatmentRationNotApproveSDO> notApproves = new BackendAdapter(param).Get<List<HisTreatmentRationNotApproveSDO>>("api/HisTreatment/GetRationNotApprove", ApiConsumers.MosConsumer, rationFilter, param);

                if (notApproves != null && notApproves.Count > 0)
                {
                    var Groups = notApproves.GroupBy(g => g.RationSumCode).ToList();
                    List<string> msgs = new List<string>();

                    string notHasRationSum = "";

                    foreach (var item in Groups)
                    {
                        if (!String.IsNullOrWhiteSpace(item.Key))
                        {
                            string maYLenhs = string.Join(",", item.Select(s => s.ServiceReqCode).ToList());
                            msgs.Add(String.Format(ResourceMessage.MaPhieuTongHopSuatAnMaYLenh, item.Key, maYLenhs));
                        }
                        else
                        {
                            notHasRationSum = string.Join(",", item.Select(s => s.ServiceReqCode).ToList());
                        }
                    }

                    //De cho cau thong bao chu tong hop nam cuoi cung
                    if (!String.IsNullOrWhiteSpace(notHasRationSum))
                    {
                        msgs.Add(String.Format(ResourceMessage.ChuaTongHopSuatAnMaYLenh, notHasRationSum));
                    }

                    string messages = String.Join(".\n", msgs);

                    if (validationDataType == ValidationDataType.PopupMessage)
                    {
                        if (XtraMessageBox.Show(String.Format("\r\n" + ResourceMessage.BanCoMuonTiepTuc, String.Format(ResourceMessage.YLenhChuaTongHopHoacDuyetSuatAn, messages)), ResourceMessage.ThongBao, MessageBoxButtons.YesNo, DevExpress.Utils.DefaultBoolean.True) != System.Windows.Forms.DialogResult.Yes)
                        {
                            return false;
                        }
                    }
                    else if (validationDataType == ValidationDataType.GetListMessage && listWarningADO != null)
                    {
                        WarningADO warning = new WarningADO();
                        warning.IsSkippable = true;
                        warning.Description = String.Format(ResourceMessage.YLenhChuaTongHopHoacDuyetSuatAn, messages);
                        listWarningADO.Add(warning);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                valid = false;
            }
            return valid;
        }

        private bool Check_UNSIGN_DOC_FINISH_OPTION_ForSave(ValidationDataType validationDataType, ref List<WarningADO> listWarningADO)
        {
            bool valid = true;
            try
            {
                if (validationDataType == ValidationDataType.PopupMessage && this._isSkipWarningForSave == true)
                {
                    return valid;
                }
                if (this.currentHisTreatment != null)
                {
                    var treatmentType = this.hisTreatmentTypes.FirstOrDefault(o => o.ID == this.currentHisTreatment.TDL_TREATMENT_TYPE_ID);
                    //Hồ sơ có diện điều trị được khai báo cảnh báo(UNSIGN_DOC_FINISH_OPTION = 1) hoặc chặn(UNSIGN_DOC_FINISH_OPTION = 2) khi có văn bản chưa hoàn thiện ký
                    if (treatmentType != null && (treatmentType.UNSIGN_DOC_FINISH_OPTION == 1
                                                || treatmentType.UNSIGN_DOC_FINISH_OPTION == 2))
                    {
                        if (CheckEmrDocumentData(treatmentType, validationDataType, ref listWarningADO))
                        {
                            return false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                valid = false;
            }
            return valid;
        }

        private bool CheckDHST_ForSave(ValidationDataType validationDataType, ref List<WarningADO> listWarningADO)
        {
            bool valid = true;
            try
            {
                if (validationDataType == ValidationDataType.PopupMessage && this._isSkipWarningForSave == true)
                {
                    return valid;
                }
                saveDHST = null;

                if (TinhTuoi(this.currentHisTreatment.TDL_PATIENT_DOB, dtEndTime.DateTime) <= 1 && !CheckDHST(this.currentHisTreatment.ID, ref saveDHST))
                {
                    WaitingManager.Hide();
                    if (validationDataType == ValidationDataType.PopupMessage)
                    {
                        DevExpress.XtraEditors.XtraMessageBox.Show(ResourceMessage.BenhNhanChuaCoDHST, ResourceMessage.ThongBao);
                        btnDHST.Enabled = true;
                        return false;
                    }
                    else if (validationDataType == ValidationDataType.GetListMessage && listWarningADO != null)
                    {
                        btnDHST.Enabled = true;
                        WarningADO warning = new WarningADO();
                        warning.IsSkippable = false;
                        warning.Description = ResourceMessage.BenhNhanChuaCoDHST;
                        listWarningADO.Add(warning);
                    }
                }

                if (TinhTuoi(this.currentHisTreatment.TDL_PATIENT_DOB, dtEndTime.DateTime) <= 1 && CheckDHST(this.currentHisTreatment.ID, ref saveDHST) && saveDHST != null && !saveDHST.WEIGHT.HasValue)
                {
                    WaitingManager.Hide();
                    if (validationDataType == ValidationDataType.PopupMessage)
                    {
                        DevExpress.XtraEditors.XtraMessageBox.Show(ResourceMessage.BenhNhanThieuCanNang, ResourceMessage.ThongBao);
                        btnDHST.Enabled = true;
                        return false;
                    }
                    else if (validationDataType == ValidationDataType.GetListMessage && listWarningADO != null)
                    {
                        btnDHST.Enabled = true;
                        WarningADO warning = new WarningADO();
                        warning.IsSkippable = false;
                        warning.Description = ResourceMessage.BenhNhanThieuCanNang;
                        listWarningADO.Add(warning);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                valid = false;
            }
            return valid;
        }

        private bool CheckWarnNotRequiredCompleteHasNoSample(ValidationDataType validationDataType, ref List<WarningADO> listWarningADO)
        {
            bool valid = true;
            try
            {
                if (validationDataType == ValidationDataType.PopupMessage && this._isSkipWarningForSave == true)
                {
                    return valid;
                }
                string serviceReqCode = "";

                if (ConfigKey.WarnNotRequiredCompleteHasNoSample == "1")
                {
                    HisServiceReqFilter srFilter = new HisServiceReqFilter();

                    srFilter.TREATMENT_ID = currentHisTreatment.ID;
                    srFilter.SERVICE_REQ_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__XN;
                    srFilter.IS_NOT_REQUIRED_COMPLETE = true;
                    srFilter.ORDER_DIRECTION = "DESC";
                    srFilter.ORDER_FIELD = "CREATE_TIME";

                    examServiceReqs = new BackendAdapter(new CommonParam()).Get<List<HIS_SERVICE_REQ>>("api/HisServiceReq/Get", ApiConsumers.MosConsumer, srFilter, null);

                }

                if (examServiceReqs != null && examServiceReqs.Count > 0)
                {
                    var serviceReqs = examServiceReqs.Where(o => o.SAMPLE_TIME == null).ToList();
                    if (serviceReqs != null && serviceReqs.Count > 0)
                    {
                        serviceReqCode = string.Join(", ", serviceReqs.Select(o => o.SERVICE_REQ_CODE));

                        if (validationDataType == ValidationDataType.PopupMessage)
                        {
                            if (DevExpress.XtraEditors.XtraMessageBox.Show(String.Format("Y lệnh {0} chưa có thông tin lấy mẫu.Bạn có muốn tiếp tục?", serviceReqCode),
                           "Thông báo",
                          MessageBoxButtons.YesNo) == DialogResult.No)
                            {
                                return false;
                            }
                        }
                        else if (validationDataType == ValidationDataType.GetListMessage && listWarningADO != null)
                        {
                            WarningADO warning = new WarningADO();
                            warning.IsSkippable = true;
                            warning.Description = String.Format(ResourceMessage.YLenhChuaCoThongTinLayMau, serviceReqCode);
                            listWarningADO.Add(warning);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                valid = false;
            }
            return valid;
        }

        private bool CheckUnassignTrackingServiceReq_ForSave(ValidationDataType validationDataType, ref List<WarningADO> listWarningADO)
        {
            bool valid = true;
            try
            {
                if (validationDataType == ValidationDataType.PopupMessage && this._isSkipWarningForSave == true)
                {
                    return valid;
                }
                if (Config.CheckFinishTimeCFG.WarningOptionInCaseOfUnassignTrackingServiceReq == "1"
                    || Config.CheckFinishTimeCFG.WarningOptionInCaseOfUnassignTrackingServiceReq == "2" 
                    || Config.CheckFinishTimeCFG.WarningOptionInCaseOfUnassignTrackingServiceReq == "3")
                {
                    List<V_HIS_SERVICE_REQ_12> listServiceReq = new List<V_HIS_SERVICE_REQ_12>();

                    CommonParam param = new CommonParam();
                    MOS.Filter.HisServiceReqView12Filter filter = new MOS.Filter.HisServiceReqView12Filter();
                    filter.TREATMENT_ID = this.treatmentId;
                    filter.ORDER_DIRECTION = "DESC";
                    filter.ORDER_FIELD = "CREATE_TIME";
                    var apiResult = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.V_HIS_SERVICE_REQ_12>>("api/HisServiceReq/GetView12", ApiConsumers.MosConsumer, filter, param);

                    if ((Config.CheckFinishTimeCFG.IsNotShowOutMediAndMate == "1" || Config.CheckFinishTimeCFG.WarningOptionInCaseOfUnassignTrackingServiceReq == "3") && apiResult != null)
                    {
                        listServiceReq = apiResult.Where(o => o.TRACKING_ID == null
                                                    && o.TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTNOITRU
                                                    && o.SERVICE_REQ_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__AN
                                                    && o.SERVICE_REQ_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__G).ToList();
                        if (Config.CheckFinishTimeCFG.WarningOptionInCaseOfUnassignTrackingServiceReq == "3")
                        {
                            List<V_HIS_SERVICE_REQ_12> lstTmp = new List<V_HIS_SERVICE_REQ_12>();
                            if (SereServTreatment != null && SereServTreatment.Count > 0)
                            {
                                foreach (var item in listServiceReq)
                                {
                                    if (item.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONK || item.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONTT || item.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONDT)
                                    {
                                        if (SereServTreatment.Exists(o => o.SERVICE_REQ_ID == item.ID && o.EXP_MEST_MEDICINE_ID != null))
                                        {
                                            lstTmp.Add(item);
                                        }
                                    }
                                    else
                                    {
                                        lstTmp.Add(item);
                                    }
                                }
                            }
                            listServiceReq = lstTmp;
                        }
                    }
                    else if (apiResult != null)
                    {
                        listServiceReq = apiResult.Where(o => o.TRACKING_ID == null
                                                    && o.TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTNOITRU
                                                    && o.SERVICE_REQ_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__AN
                                                    && o.SERVICE_REQ_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__G
                                                    && ((o.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONDT && o.EXP_MEST_ID != null) || o.SERVICE_REQ_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONDT)).ToList();
                    }
                    if (listServiceReq != null && listServiceReq.Count > 0)
                    {
                        string message = "";
                        var groupServiceReq = listServiceReq.GroupBy(o => o.REQUEST_DEPARTMENT_NAME).ToList();
                        foreach (var group in groupServiceReq)
                        {
                            string str = "";
                            foreach (var item in group)
                            {
                                str += item.SERVICE_REQ_CODE + "; ";
                            }
                            if (!String.IsNullOrEmpty(str) && str.Length > 2)
                                str = str.Remove(str.Length - 2, 2);
                            str += " (khoa chỉ định: " + group.Key + ")";
                            str += ",";
                            message += str;
                        }
                        if (!String.IsNullOrEmpty(message) && message.Length > 1)
                        {
                            message = message.Remove(message.Length - 1, 1);
                            message = "Y lệnh " + message + " chưa gắn tờ điều trị.";
                        }
                        if (validationDataType == ValidationDataType.PopupMessage)
                        {
                            if (Config.CheckFinishTimeCFG.WarningOptionInCaseOfUnassignTrackingServiceReq == "1")
                            {
                                XtraMessageBox.Show(message, ResourceMessage.ThongBao);
                                return false;
                            }
                            else if (Config.CheckFinishTimeCFG.WarningOptionInCaseOfUnassignTrackingServiceReq == "2" || Config.CheckFinishTimeCFG.WarningOptionInCaseOfUnassignTrackingServiceReq == "3")
                            {
                                if (XtraMessageBox.Show(String.Format(ResourceMessage.BanCoMuonTiepTuc, message), ResourceMessage.ThongBao, MessageBoxButtons.YesNo, DevExpress.Utils.DefaultBoolean.True) != System.Windows.Forms.DialogResult.Yes)
                                {
                                    return false;
                                }
                            }
                        }
                        else if (validationDataType == ValidationDataType.GetListMessage && listWarningADO != null)
                        {
                            if (Config.CheckFinishTimeCFG.WarningOptionInCaseOfUnassignTrackingServiceReq == "1")
                            {
                                btnDHST.Enabled = true;
                                WarningADO warning = new WarningADO();
                                warning.IsSkippable = false;
                                warning.Description = message;
                                listWarningADO.Add(warning);
                            }
                            else if (Config.CheckFinishTimeCFG.WarningOptionInCaseOfUnassignTrackingServiceReq == "2"
                                || Config.CheckFinishTimeCFG.WarningOptionInCaseOfUnassignTrackingServiceReq == "3")
                            {
                                btnDHST.Enabled = true;
                                WarningADO warning = new WarningADO();
                                warning.IsSkippable = true;
                                warning.Description = message;
                                listWarningADO.Add(warning);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                valid = false;
            }
            return valid;
        }

        private bool Check_IsAllowTreatmentFinishDepartmentIsActiveFee_ForSave()
        {
            bool valid = true;
            try
            {
                if (this.currentHisTreatment != null)
                {
                    if (Config.ConfigKey.IsAllowTreatmentFinishDepartmentIsActiveFee == "1")
                    {
                        CommonParam param = new CommonParam();
                        var departments = BackendDataWorker.Get<HIS_DEPARTMENT>().Where(o => o.IS_CLINICAL == Constant.IS_TRUE).ToList();
                        if (departments != null && departments.Count > 0)
                        {
                            List<long> departmentIds = departments.Select(o => o.ID).ToList();
                            HisDepartmentTranViewFilter filter = new HisDepartmentTranViewFilter();
                            filter.TREATMENT_ID = this.currentHisTreatment.ID;
                            filter.DEPARTMENT_IDs = departmentIds;
                            filter.IS_ACTIVE = Constant.IS_TRUE;
                            var departmentTrans = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.V_HIS_DEPARTMENT_TRAN>>("api/HisDepartmentTran/GetView", ApiConsumers.MosConsumer, filter, param);
                            if (departmentTrans != null && departmentTrans.Count > 0)
                            {
                                List<string> departmentNames = departmentTrans.Select(o => o.DEPARTMENT_NAME).Distinct().ToList();
                                XtraMessageBox.Show(String.Format("Hồ sơ {0} có khoa {1} chưa được khóa chi phí. Không cho phép kết thúc điều trị", this.currentHisTreatment.TREATMENT_CODE, string.Join(",", departmentNames), ResourceMessage.ThongBao));
                                if (departmentNames != null && departmentNames.Count > 0)
                                {
                                    return false;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                valid = false;
            }
            return valid;
        }
    }
}

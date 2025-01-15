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
using EMR.EFMODEL.DataModels;
using EMR.Filter;
using EMR_MAIN;
using EMR_MAIN.DATABASE.BenhAn;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.HisConfig;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.Library.FormMedicalRecord.Base;
using HIS.Desktop.Plugins.Library.FormMedicalRecord.ConfigKeys;
using Inventec.Common.Adapter;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.Library.FormMedicalRecord.Process
{
    internal class MediRecordProcessor
    {
        internal void LoadDataEmr(LoaiBenhAnEMR _TYpe, EmrInputADO ado, string _MaPhieu = "")
        {
            try
            {
                if (ado == null)
                    return;
                WaitingManager.Show();
                string caption__Tuoi = "tuổi";
                string caption__ThangTuoi = "tháng tuổi";
                string caption__NgayTuoi = "ngày tuổi";
                string caption__GioTuoi = "giờ tuổi";

                LogSystem.Debug("LoadDataEmr. 1");

                List<ThuocKhangSinh> KhangSinh_HISs = new List<ThuocKhangSinh>();

                #region --- Load
                CommonParam param = new CommonParam();
                long? DepartmentID = null;
                long? RoomTypeId = null;
                string RoomCode = "";
                string RoomTypeCode = null;
                long? TuyChonCanhBaoVanBanDaKy = null;
                string SoLuuTru = "";
                string MaYTe = "";
                if (ado.roomId != null && ado.roomId > 0)
                {
                    DepartmentID = HIS.Desktop.LocalStorage.LocalData.WorkPlace.WorkPlaceSDO.FirstOrDefault(o => o.RoomId == ado.roomId).DepartmentId;
                    RoomTypeId = HIS.Desktop.LocalStorage.LocalData.WorkPlace.WorkPlaceSDO.FirstOrDefault(o => o.RoomId == ado.roomId).RoomTypeId;
                    RoomCode = HIS.Desktop.LocalStorage.LocalData.WorkPlace.WorkPlaceSDO.FirstOrDefault(o => o.RoomId == ado.roomId).RoomCode;

                    if (RoomTypeId != null && RoomTypeId > 0)
                    {
                        RoomTypeCode = BackendDataWorker.Get<HIS_ROOM_TYPE>().FirstOrDefault(o => o.ID == RoomTypeId).ROOM_TYPE_CODE;
                    }
                }

                V_HIS_TREATMENT _Treatment = null;
                V_HIS_PATIENT _Patient = null;
                V_HIS_PATIENT_TYPE_ALTER _PatientTypeAlter = null;
                HIS_DHST _DHST = new HIS_DHST();
                HIS_DHST _DHSTMOI = new HIS_DHST();
                V_HIS_BABY _Baby = new V_HIS_BABY();
                List<V_HIS_DEPARTMENT_TRAN> _DepartmentTrans = null;
                V_HIS_DEPARTMENT_TRAN _DepartmentTranYc = new V_HIS_DEPARTMENT_TRAN();
                V_HIS_SERVICE_REQ _ExamServiceReq = new V_HIS_SERVICE_REQ();
                List<V_HIS_SERVICE_REQ> currentServiceReqs = null;
                long TreatmentIcdCount = 1;
                List<V_HIS_SERE_SERV_PTTT> sereServPttts = null;
                List<HIS_SERE_SERV> sereServAlls = null;
                List<HIS_SERE_SERV> sereServCLSs = null;
                V_HIS_TREATMENT_BED_ROOM treatmentBedRoom = null;
                List<HIS_EXP_MEST_MEDICINE> _ExpMestMedicine = null;
                List<EMR_CONFIG> _EmrConfigs = null;

                HisTreatmentForEmrSDO treatmentForEmrSDO = new MOS.SDO.HisTreatmentForEmrSDO();
                treatmentForEmrSDO.TreatmentId = ado.TreatmentId;
                treatmentForEmrSDO.PatientId = ado.PatientId;
                treatmentForEmrSDO.IntructionTime = Inventec.Common.DateTime.Get.Now() ?? 0;
                treatmentForEmrSDO.WorkingDepartmentId = DepartmentID != null ? DepartmentID : null;

                Inventec.Common.Logging.LogSystem.Info("treatmentForEmrSDO: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => treatmentForEmrSDO), treatmentForEmrSDO));

                var _TreatmentForEmr = new BackendAdapter(param).Get<HisTreatmentForEmrSDO>("api/HisTreatment/GetForEmr", ApiConsumers.MosConsumer, treatmentForEmrSDO, param);
                if (_TreatmentForEmr != null)
                {
                    _Patient = _TreatmentForEmr.Patient;
                    _PatientTypeAlter = _TreatmentForEmr.PatientTypeAlter;
                    _DHST = _TreatmentForEmr.Dhst;
                    _Baby = _TreatmentForEmr.Baby;
                    _DepartmentTrans = _TreatmentForEmr.DepartmentTrans;
                    currentServiceReqs = _TreatmentForEmr.ServiceReqs;
                    sereServPttts = _TreatmentForEmr.SereServPttts;
                    sereServAlls = _TreatmentForEmr.SereServs;
                    treatmentBedRoom = _TreatmentForEmr.TreatmentBedRoom;
                    _DHSTMOI = _TreatmentForEmr.RecentDhst;
                    _Treatment = _TreatmentForEmr.Treatment;
                    _ExpMestMedicine = _TreatmentForEmr.ExpMestMedicines;
                    TreatmentIcdCount = _TreatmentForEmr.TreatmentIcdCount;
                }

                if (_DepartmentTrans != null && _DepartmentTrans.Count > 0)
                {
                    _DepartmentTrans = _DepartmentTrans.Where(p => p.DEPARTMENT_IN_TIME != null).ToList();

                }

                //Lấy config EMR
                CommonParam paramEmr = new CommonParam();
                _EmrConfigs = new List<EMR_CONFIG>();
                EmrConfigFilter filter = new EmrConfigFilter();

                _EmrConfigs = new BackendAdapter(paramEmr).Get<List<EMR_CONFIG>>("api/EmrConfig/Get", ApiConsumers.EmrConsumer, filter, paramEmr);
                if (_EmrConfigs != null && _EmrConfigs.Count > 0)
                {
                    var cfgSignDisplayOptions = _EmrConfigs.FirstOrDefault(o => o.KEY == SdaConfigKeys.EMR__EMR_DOCUMENT__DULICATE_HIS_CODE__WARNING_OPTION);
                    if (cfgSignDisplayOptions != null)
                    {
                        TuyChonCanhBaoVanBanDaKy = Inventec.Common.TypeConvert.Parse.ToInt64(!String.IsNullOrEmpty(cfgSignDisplayOptions.VALUE) ? cfgSignDisplayOptions.VALUE : cfgSignDisplayOptions.DEFAULT_VALUE);
                    }
                }

                if (currentServiceReqs != null && currentServiceReqs.Count > 0)
                {
                    //#39489 2. Sửa lại phần tích hợp "Vỏ bệnh án":
                    //Thông tin khám trên vỏ bệnh án (phần "khám bệnh" và "Hỏi bệnh") cần được lấy từ công khám xử lý cho BN nhập viện hoặc công khám chính (gọi là X). Cụ thể:

                    //a. Kiểm tra trong các công khám, lấy ra công khám có loại kết thúc là "Nhập viện":
                    //X = HIS_SERVICE_REQ có SERVICE_REQ_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__KH
                    //và EXAM_END_TYPE = IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ.EXAM_END_TYPE__NHAP_VIEN
                    //Nếu có, thì kết thúc, nếu ko thì kiểm tra điều kiện (b):
                    _ExamServiceReq = currentServiceReqs.Where(p => p.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__KH && p.EXAM_END_TYPE == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ.EXAM_END_TYPE__NHAP_VIEN).OrderBy(o => o.MODIFY_TIME).FirstOrDefault();
                    if (_ExamServiceReq == null || _ExamServiceReq.ID == 0)
                    {
                        //b. Kiểm tra trong các công khám, lấy ra công khám có phòng xử lý tương ứng với phòng xử lý nhập viện:
                        //X = HIS_SERVICE_REQ có SERVICE_REQ_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__KH
                        //và EXECUTE_ROOM_ID = IN_ROOM_ID trong HIS_TREATMENT
                        //Nếu có, thì kết thúc, nếu ko thì kiểm tra điều kiện (c)
                        _ExamServiceReq = currentServiceReqs.Where(p => p.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__KH && p.EXECUTE_ROOM_ID == _Treatment.IN_ROOM_ID).OrderBy(o => o.MODIFY_TIME).FirstOrDefault();

                        if (_ExamServiceReq == null || _ExamServiceReq.ID == 0)
                        {
                            //c. Kiểm tra trong các công khám, lấy ra công khám chính:
                            //X = HIS_SERVICE_REQ có SERVICE_REQ_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__KH
                            //và IS_MAIN_EXAM = 1
                            //Nếu có, thì kết thúc, nếu ko thì kiểm tra điều kiện (d)
                            _ExamServiceReq = currentServiceReqs.Where(p => p.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__KH && p.IS_MAIN_EXAM == GlobalVariables.CommonNumberTrue).OrderBy(o => o.MODIFY_TIME).FirstOrDefault();

                            if (_ExamServiceReq == null || _ExamServiceReq.ID == 0)
                            {
                                //d. Kiểm tra trong các công khám, lấy ra công khám có thời gian y lệnh bé nhất:
                                //X = HIS_SERVICE_REQ có SERVICE_REQ_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__KH
                                //và INTRUCTION_TIME bé nhất (nếu có 2 công khám có INTRUCTION_TIME bằng nhau, thì lấy bản ghi có ID nhỏ hơn)
                                //Nếu có, thì kết thúc, nếu ko thì kiểm tra điều kiện (d)
                                _ExamServiceReq = currentServiceReqs.Where(p => p.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__KH).OrderBy(o => o.INTRUCTION_TIME).ThenBy(o => o.ID).FirstOrDefault();
                            }
                        }
                    }
                    if (_ExamServiceReq != null && _ExamServiceReq.DHST_ID > 0 && _ExamServiceReq.DHST_ID != (_DHST != null ? _DHST.ID : 0))
                    {
                        HisDhstFilter dhstFilter = new HisDhstFilter();
                        dhstFilter.ID = _ExamServiceReq.DHST_ID;
                        param = new CommonParam();
                        var dhsts = new BackendAdapter(param).Get<List<HIS_DHST>>("api/HisDhst/Get", ApiConsumer.ApiConsumers.MosConsumer, dhstFilter, param);
                        if (dhsts != null && dhsts.Count() > 0)
                        {
                            _DHST = dhsts.First();
                        }
                    }
                }
                if (sereServAlls != null && sereServAlls.Count > 0)
                {
                    sereServCLSs = (sereServAlls != null && sereServAlls.Count > 0) ? sereServAlls.Where(o => (o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__XN || o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__CDHA || o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__SA || o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__NS || o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__GPBL)).ToList() : null;
                }

                #endregion
                LogSystem.Debug("LoadDataEmr. 2");
                #region ------- HanhChinhBenhNhan
                HanhChinhBenhNhan _HanhChinhBenhNhan = new HanhChinhBenhNhan();
                HIS_BRANCH HisBranch_WorkPlace = new HIS_BRANCH();
                HisBranch_WorkPlace = BackendDataWorker.Get<HIS_BRANCH>().FirstOrDefault(p => p.ID == WorkPlace.WorkPlaceSDO.FirstOrDefault().BranchId);
                _HanhChinhBenhNhan.SoYTe = HisBranch_WorkPlace != null ? HisBranch_WorkPlace.PARENT_ORGANIZATION_NAME : "";
                _HanhChinhBenhNhan.BenhVien = HisBranch_WorkPlace != null ? HisBranch_WorkPlace.BRANCH_NAME : "";

                //Xử lý lấy Mã Y tế dựa vào 3 trường IN_TIME, IN_CODE và IN_CODE_SEED_CODE (Nếu không có dữ liệu của 3 trường này thì bỏ qua không truyền sang): IN_CODE sẽ có định dạng xxx/IN_CODE_SEED_CODE.
                //+ Từ IN_CODE và IN_CODE_SEED_CODE => xử lý để lấy được xxx.
                //+ Nếu xxx < 6 ký tự -> tự động thêm 0 trước vào để đủ 6 ký tự.
                //+ Từ IN_DEPARTMENT_ID => mã bệnh viện (MEDI_ORG_CODE trong HIS_BRANCH tương ứng với khoa IN_DEPARTMENT_ID ) gọi là mm.
                //+ Lấy 2 ký tự 2,3 của IN_TIME gọi là yy.
                //+ Nối mm+yy+xx truyền vào trường Mã Y tế (trường MaYTe trong HanhChinhBenhNhan).

                string maYTe = "";
                if (_Treatment != null && _Treatment.IN_TIME > 0 && !string.IsNullOrEmpty(_Treatment.IN_CODE) && !string.IsNullOrEmpty(_Treatment.IN_CODE_SEED_CODE))
                {
                    string xxx = _Treatment.IN_CODE.Replace("/" + _Treatment.IN_CODE_SEED_CODE, "");
                    Inventec.Common.Logging.LogSystem.Debug("IN_CODE: " + _Treatment.IN_CODE + " xxx: " + xxx + " IN_CODE_SEED_CODE: " + _Treatment.IN_CODE_SEED_CODE);
                    if (xxx.Length < 6)
                    {
                        xxx = string.Format("{0:000000}", Convert.ToInt64(xxx));
                    }
                    string mm = HisBranch_WorkPlace != null ? HisBranch_WorkPlace.HEIN_MEDI_ORG_CODE : "";
                    string yy = _Treatment.IN_TIME.ToString().Substring(2, 2);
                    maYTe = mm + yy + xxx;

                    Inventec.Common.Logging.LogSystem.Debug("xxx: " + xxx + " mm: " + mm + " yy " + yy + " maYTe: " + maYTe);
                }

                _HanhChinhBenhNhan.MaYTe = maYTe;

                _HanhChinhBenhNhan.MaBenhNhan = _Patient.PATIENT_CODE;
                _HanhChinhBenhNhan.TenBenhNhan = _Treatment.TDL_PATIENT_NAME;
                _HanhChinhBenhNhan.NgaySinh = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTimeUTC(_Treatment.TDL_PATIENT_DOB) ?? DateTime.Now;
                _HanhChinhBenhNhan.Tuoi = Inventec.Common.DateTime.Calculation.AgeString(_Treatment.TDL_PATIENT_DOB, caption__Tuoi, caption__ThangTuoi, caption__NgayTuoi, caption__GioTuoi, _Treatment.IN_TIME); //MPS.AgeUtil.CalculateFullAge(_Treatment.TDL_PATIENT_DOB);
                _HanhChinhBenhNhan.GioiTinh = _Treatment.TDL_PATIENT_GENDER_ID == IMSys.DbConfig.HIS_RS.HIS_GENDER.ID__MALE ? GioiTinh.Nam : _Treatment.TDL_PATIENT_GENDER_ID == IMSys.DbConfig.HIS_RS.HIS_GENDER.ID__FEMALE ? GioiTinh.Nu : GioiTinh.ChuaXacDinh;
                _HanhChinhBenhNhan.DienThoaiLienLac = _Patient.PHONE;
                _HanhChinhBenhNhan.NgheNghiep = _Patient.CAREER_NAME;
                _HanhChinhBenhNhan.MaNgheNghiep = _Patient.CAREER_CODE;
                _HanhChinhBenhNhan.DanToc = _Patient.ETHNIC_NAME;
                _HanhChinhBenhNhan.MaDanhToc = _Patient.ETHNIC_CODE;
                _HanhChinhBenhNhan.NgoaiKieu = _Patient.NATIONAL_NAME;
                _HanhChinhBenhNhan.MaNgoaiKieu = _Patient.NATIONAL_CODE;
                _HanhChinhBenhNhan.HoVaTenBo = _Patient.FATHER_NAME;
                _HanhChinhBenhNhan.NgheNghiepBo = _Patient.FATHER_CAREER;
                _HanhChinhBenhNhan.TrinhDoVanHoaBo = _Patient.FATHER_EDUCATIIONAL_LEVEL;
                _HanhChinhBenhNhan.HoVaTenMe = _Patient.MOTHER_NAME;
                _HanhChinhBenhNhan.NgheNghiepMe = _Patient.MOTHER_CAREER;
                _HanhChinhBenhNhan.TrinhDoVanHoaMe = _Patient.MOTHER_EDUCATIIONAL_LEVEL;
                _HanhChinhBenhNhan.SoDienThoaiNguoiNha = _Patient.RELATIVE_PHONE;
                _HanhChinhBenhNhan.NoiLamViec = _Patient.WORK_PLACE;
                var mitiRank = _Patient.MILITARY_RANK_ID > 0 ? BackendDataWorker.Get<HIS_MILITARY_RANK>().Where(o => o.ID == _Patient.MILITARY_RANK_ID).FirstOrDefault() : null;
                _HanhChinhBenhNhan.CapBac = mitiRank != null ? mitiRank.MILITARY_RANK_NAME : "";
                var branchPatient = _Patient.BRANCH_ID > 0 ? BackendDataWorker.Get<HIS_BRANCH>().Where(o => o.ID == _Patient.BRANCH_ID).FirstOrDefault() : null;
                _HanhChinhBenhNhan.DonVi = branchPatient != null ? branchPatient.BRANCH_NAME : "";

                if (String.IsNullOrEmpty(_Patient.COMMUNE_NAME) && String.IsNullOrEmpty(_Patient.DISTRICT_NAME) && String.IsNullOrEmpty(_Patient.PROVINCE_NAME))
                {
                    _HanhChinhBenhNhan.SoNha = _Patient.VIR_ADDRESS;
                }
                else
                {
                    _HanhChinhBenhNhan.SoNha = _Patient.ADDRESS;
                    _HanhChinhBenhNhan.ThonPho = "";
                    _HanhChinhBenhNhan.XaPhuong = _Patient.COMMUNE_NAME;
                    _HanhChinhBenhNhan.HuyenQuan = _Patient.DISTRICT_NAME;
                    _HanhChinhBenhNhan.MaHuyenQuan = _Patient.DISTRICT_CODE;
                    _HanhChinhBenhNhan.TinhThanhPho = _Patient.PROVINCE_NAME;
                    _HanhChinhBenhNhan.MaTinhThanhPho = _Patient.PROVINCE_CODE;
                }

                LogSystem.Debug("LoadDataEmr. 2.1.4");

                if (_PatientTypeAlter != null)
                {
                    _HanhChinhBenhNhan.DoiTuong = _PatientTypeAlter.PATIENT_TYPE_CODE == HisConfigCFG.PatientTypeCode__BHYT ? DoiTuong.BHYT : _PatientTypeAlter.PATIENT_TYPE_CODE == HisConfigCFG.PatientTypeCode__VP ? DoiTuong.ThuPhi : DoiTuong.Khac;
                    if (_PatientTypeAlter.HEIN_CARD_FROM_TIME > 0)
                        _HanhChinhBenhNhan.NgayDangKyBHYT = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTimeUTC(_PatientTypeAlter.HEIN_CARD_FROM_TIME ?? 0) ?? null;
                    if (_PatientTypeAlter.HEIN_CARD_TO_TIME > 0)
                        _HanhChinhBenhNhan.NgayHetHanBHYT = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTimeUTC(_PatientTypeAlter.HEIN_CARD_TO_TIME ?? 0) ?? null;

                    _HanhChinhBenhNhan.SoTheBHYT = _PatientTypeAlter.HEIN_CARD_NUMBER;
                    _HanhChinhBenhNhan.TenNoiDangKyBHYT = _PatientTypeAlter.HEIN_MEDI_ORG_NAME;
                    _HanhChinhBenhNhan.MaNoiDangKyBHYT = _PatientTypeAlter.HEIN_MEDI_ORG_CODE;
                    _HanhChinhBenhNhan.NgayDuocHuong5Nam = _PatientTypeAlter.JOIN_5_YEAR == MOS.LibraryHein.Bhyt.HeinJoin5Year.HeinJoin5YearCode.TRUE ? Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTimeUTC(_PatientTypeAlter.JOIN_5_YEAR_TIME ?? 0) : null;
                }

                LogSystem.Debug("LoadDataEmr. 2.1.4.8");

                _HanhChinhBenhNhan.HoTenDiaChiNguoiNha = _Patient.RELATIVE_NAME + " - " + _Patient.RELATIVE_ADDRESS;
                _HanhChinhBenhNhan.CMND = !String.IsNullOrEmpty(_Patient.CMND_NUMBER) ? _Patient.CMND_NUMBER : _Patient.CCCD_NUMBER;
                _HanhChinhBenhNhan.NoiCap_CMND = !String.IsNullOrEmpty(_Patient.CMND_PLACE) ? _Patient.CMND_PLACE : _Patient.CCCD_PLACE;
                _HanhChinhBenhNhan.NgayCap_CMND = _Patient.CMND_DATE != null ? Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTimeUTC(_Patient.CMND_DATE.Value) : _Patient.CCCD_DATE != null ? Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTimeUTC(_Patient.CCCD_DATE.Value) : null;
                #endregion

                LogSystem.Debug("LoadDataEmr. 2.1.5");

                #region ------- ThongTinDieuTri
                ThongTinDieuTri _ThongTinDieuTri = new ThongTinDieuTri();
                if (_Baby != null)
                {
                    _ThongTinDieuTri.LucVaoDe = "";
                    _ThongTinDieuTri.NgayDe = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTimeUTC(_Baby.BORN_TIME ?? 0);
                    _ThongTinDieuTri.NgoiThai = _Baby.BORN_POSITION_NAME;
                    _ThongTinDieuTri.CachThucDe = _Baby.BORN_TYPE_NAME;
                    _ThongTinDieuTri.KiemSoatTuCung = false;
                    _ThongTinDieuTri.KiemSoatTuCung_Text = "";
                    _ThongTinDieuTri.TreSoSinh_LoaiThai = -1;
                    _ThongTinDieuTri.TreSoSinh_GioiTinh = _Baby.GENDER_ID == IMSys.DbConfig.HIS_RS.HIS_GENDER.ID__MALE ? 1 : 0;
                    _ThongTinDieuTri.TreSoSinh_DiTat = 0;
                    _ThongTinDieuTri.TreSoSinh_DiTat_Text = "";
                    _ThongTinDieuTri.TreSoSinh_CanNang = (int?)_Baby.WEIGHT;
                    _ThongTinDieuTri.TreSoSinh_SongChet = (String.IsNullOrEmpty(_Baby.BORN_RESULT_CODE) || _Baby.BORN_RESULT_CODE == "01") ? 1 : 0;
                }
                else
                {
                    _ThongTinDieuTri.LucVaoDe = "";
                    _ThongTinDieuTri.NgayDe = null;
                    _ThongTinDieuTri.NgoiThai = "";
                    _ThongTinDieuTri.CachThucDe = "";
                    _ThongTinDieuTri.KiemSoatTuCung = false;
                    _ThongTinDieuTri.KiemSoatTuCung_Text = "";
                    _ThongTinDieuTri.TreSoSinh_LoaiThai = -1;
                    _ThongTinDieuTri.TreSoSinh_GioiTinh = -1;
                    _ThongTinDieuTri.TreSoSinh_DiTat = -1;
                    _ThongTinDieuTri.TreSoSinh_DiTat_Text = "";
                    _ThongTinDieuTri.TreSoSinh_CanNang = null;
                    _ThongTinDieuTri.TreSoSinh_SongChet = -1;
                }

                LogSystem.Debug("LoadDataEmr. 2.1.6");

                if (_Treatment.END_DEPARTMENT_ID > 0)
                {
                    var dpEnd = BackendDataWorker.Get<HIS_DEPARTMENT>().Where(p => p.ID == _Treatment.END_DEPARTMENT_ID).FirstOrDefault();
                    _ThongTinDieuTri.KhoaRaVien = dpEnd != null ? dpEnd.DEPARTMENT_NAME : "";
                }

                _ThongTinDieuTri.MaBenhAn = _Treatment.TREATMENT_CODE;
                _ThongTinDieuTri.GiuongRaVien = "";
                _ThongTinDieuTri.SoLuuTru = _Treatment.STORE_CODE;
                _ThongTinDieuTri.MaQuanLy = Inventec.Common.TypeConvert.Parse.ToDecimal(_Treatment.TREATMENT_CODE);//Mỗi lần vào điều trị có cái mã
                _ThongTinDieuTri.MaBenhNhan = _Patient.PATIENT_CODE;

                ///Lựa chọn thời gian hiển thị trên mục 12.Vào viện của "Vỏ bệnh án":
                ///+1: Hiển thị thời gian vào viện
                ///+Khác 1:Hiển thị thời gian nhập viện
                long HOSPITALIZE_TIME_OPTION = Inventec.Common.TypeConvert.Parse.ToInt64(HisConfigs.Get<string>(SdaConfigKeys.HOSPITALIZE_TIME_OPTION));

                if (HOSPITALIZE_TIME_OPTION == 1)
                {
                    _ThongTinDieuTri.NgayVaoVien = _Treatment.IN_TIME != null ? Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTimeUTC((long)_Treatment.IN_TIME) : null;
                }
                else
                {
                    if (_Treatment != null && _Treatment.CLINICAL_IN_TIME != null)
                    {
                        _ThongTinDieuTri.NgayVaoVien = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTimeUTC((long)_Treatment.CLINICAL_IN_TIME);
                    }
                    else
                    {
                        _DepartmentTranYc = Get_DepartmentTranYc(_Treatment.ID);
                        if (_DepartmentTranYc != null)
                        {
                            _ThongTinDieuTri.NgayVaoVien = _DepartmentTranYc.REQUEST_TIME != null ? Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTimeUTC((long)_DepartmentTranYc.REQUEST_TIME) : null;
                        }
                    }
                }

                //1. Sửa cách truyền "ThongTinDieuTri.TrucTiepVao":
                //- Nếu hồ sơ điều trị là cấp cứu (his_treatment có is_emergency = 1), thì truyền vào là CapCuu.
                //- Nếu hồ sơ điều trị ko phải là cấp cứu thì kiểm tra khoa tiếp đón BN (khoa đầu tiên trong dòng thời gian, tính theo department_in_time, nếu time giống nhau thì lấy ID nhỏ hơn):
                //+ Nếu khoa được tick là "khoa khám bệnh" (his_department có is_exam = 1) thì truyền vào là "KKB"
                //+ Còn lại, thì truyền vào là "KhoaDieuTri"
                var departmentTimeFirst = _DepartmentTrans != null ? _DepartmentTrans.OrderBy(o => o.DEPARTMENT_IN_TIME).ThenBy(o => o.ID).FirstOrDefault() : null;
                var depa = departmentTimeFirst != null ? BackendDataWorker.Get<HIS_DEPARTMENT>().Where(p => p.ID == departmentTimeFirst.DEPARTMENT_ID).FirstOrDefault() : null;
                _ThongTinDieuTri.TrucTiepVao = (_Treatment.IS_EMERGENCY == 1) ? TrucTiepVao.CapCuu :
                    (depa != null && depa.IS_EXAM == 1 ? TrucTiepVao.KKB : TrucTiepVao.KhoaDieuTri)
                ;

                _ThongTinDieuTri.NoiGioiThieu = (!String.IsNullOrEmpty(_Treatment.TRANSFER_IN_MEDI_ORG_NAME) && !String.IsNullOrEmpty(_Treatment.TRANSFER_IN_MEDI_ORG_CODE)) ? NoiGioiThieu.CoQuanYTe : NoiGioiThieu.TuDen;

                //2. Sửa cách truyền TenKhoaVao, NgayVaoKhoa,SoNgayDieuTriTaiKhoa của ThongTinDieuTri:
                //Lấy theo tên khoa, số ngày điều trị tại khoa, ngày vào lấy theo thông tin nhập viện (department_tran đầu tiên có patient_type_alter có diện điều trị là "điều trị nội trú")
                V_HIS_DEPARTMENT_TRAN departmentTranFirst = new V_HIS_DEPARTMENT_TRAN();
                List<V_HIS_DEPARTMENT_TRAN> _DepartmentTranCKs = new List<V_HIS_DEPARTMENT_TRAN>();
                V_HIS_DEPARTMENT_TRAN departmentTranSecond = new V_HIS_DEPARTMENT_TRAN();

                HIS.Common.Treatment.PatientTypeEnum.TYPE ptyType = (_Treatment.TDL_PATIENT_TYPE_ID == HisConfigCFG.PatientTypeId__BHYT ? HIS.Common.Treatment.PatientTypeEnum.TYPE.BHYT : HIS.Common.Treatment.PatientTypeEnum.TYPE.THU_PHI);

                MOS.Filter.HisPatientTypeAlterFilter patientTypeAlterFilter = new HisPatientTypeAlterFilter();
                patientTypeAlterFilter.TREATMENT_ID = _Treatment.ID;
                patientTypeAlterFilter.ORDER_FIELD = "LOG_TIME";
                patientTypeAlterFilter.ORDER_DIRECTION = "ASC";
                param = new CommonParam();
                var patientTypeAlters = new BackendAdapter(param).Get<List<HIS_PATIENT_TYPE_ALTER>>("api/HisPatientTypeAlter/Get", ApiConsumer.ApiConsumers.MosConsumer, patientTypeAlterFilter, param);

                if (patientTypeAlters != null && patientTypeAlters.Count() > 0)
                {
                    patientTypeAlters = patientTypeAlters.Where(o => o.TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTNOITRU || o.TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTBANNGAY || o.TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTNGOAITRU).ToList();
                }

                if (patientTypeAlters != null && patientTypeAlters.Count() > 0)
                {
                    long? timeIn = 0, timeOut = 0;
                    var depaTranFirstId = patientTypeAlters.First().DEPARTMENT_TRAN_ID;
                    var depaTranFirst = patientTypeAlters.First();
                    if (_DepartmentTrans != null && _DepartmentTrans.Count() > 0)
                    {
                        departmentTranFirst = _DepartmentTrans.FirstOrDefault(o => o.ID == depaTranFirstId);
                        _DepartmentTranCKs = _DepartmentTrans.Where(o => o.DEPARTMENT_IN_TIME >= departmentTranFirst.DEPARTMENT_IN_TIME && o.ID > departmentTranFirst.ID).OrderBy(o => o.DEPARTMENT_IN_TIME).ToList();
                        departmentTranSecond = _DepartmentTranCKs != null ? _DepartmentTranCKs.FirstOrDefault() : null;
                        timeIn = departmentTranFirst.DEPARTMENT_IN_TIME;
                        timeOut = (departmentTranSecond == null) ? _Treatment.OUT_TIME : departmentTranSecond.DEPARTMENT_IN_TIME;

                        _ThongTinDieuTri.NgayVaoKhoa = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTimeUTC(depaTranFirst.LOG_TIME) ?? null;
                        _ThongTinDieuTri.TenKhoaVao = departmentTranFirst.DEPARTMENT_NAME;
                        long? songay = null;

                        songay = DayOfTreatmentDepartment(timeIn ?? 0, timeOut, _Treatment.TDL_TREATMENT_TYPE_ID ?? 0);
                        _ThongTinDieuTri.SoNgayDieuTriTaiKhoa = Inventec.Common.TypeConvert.Parse.ToInt32(songay.ToString());
                    }
                }

                //3. Sửa thông tin "Chuyển khoa":
                //Chỉ lấy các bản ghi sau bản ghi được lấy ở mục 2 (theo department_in_time và Id)
                _ThongTinDieuTri.lstChuyenKhoaHis = new List<LichSuChuyenKhoa>();
                if (_DepartmentTranCKs != null && _DepartmentTranCKs.Count > 0)
                {
                    for (int rowNum = 0; rowNum < _DepartmentTranCKs.Count; rowNum++)
                    {
                        int? songay1a = null, songay1b = null;

                        songay1a = (int?)DayOfTreatmentDepartment(_DepartmentTranCKs[rowNum].DEPARTMENT_IN_TIME ?? 0, _DepartmentTranCKs.Count > rowNum + 1 ? _DepartmentTranCKs[rowNum + 1].DEPARTMENT_IN_TIME : 0, _Treatment.TDL_TREATMENT_TYPE_ID ?? 0);
                        songay1b = (int?)DayOfTreatmentDepartment(_DepartmentTranCKs[rowNum].DEPARTMENT_IN_TIME ?? 0, _Treatment.OUT_TIME, _Treatment.TDL_TREATMENT_TYPE_ID ?? 0);

                        int? songay1 = _DepartmentTranCKs.Count > rowNum + 1 ?
                          songay1a : songay1b;

                        _ThongTinDieuTri.lstChuyenKhoaHis.Add(new LichSuChuyenKhoa()
                        {
                            ChuyenKhoa = _DepartmentTranCKs[rowNum].DEPARTMENT_NAME,
                            DateNgayChuyenKhoa = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTimeUTC(_DepartmentTranCKs[rowNum].REQUEST_TIME ?? 0) ?? null,
                            DateNgayGioVaoKhoa = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTimeUTC(_DepartmentTranCKs[rowNum].DEPARTMENT_IN_TIME ?? 0) ?? null,
                            NgayChuyenKhoa = Inventec.Common.DateTime.Convert.TimeNumberToDateString(_DepartmentTranCKs[rowNum].DEPARTMENT_IN_TIME ?? 0),
                            SoNgayDieuTriKhoa = songay1
                        });
                    }

                    _ThongTinDieuTri.ChuyenKhoa1 = _DepartmentTranCKs[0].DEPARTMENT_NAME;
                    _ThongTinDieuTri.NgayChuyenKhoa1 = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTimeUTC(_DepartmentTranCKs[0].DEPARTMENT_IN_TIME ?? 0) ?? null;
                    if (_DepartmentTranCKs.Count > 1)
                    {
                        long? songay1 = null;

                        songay1 = DayOfTreatmentDepartment(_DepartmentTranCKs[0].DEPARTMENT_IN_TIME ?? 0, _DepartmentTranCKs[1].DEPARTMENT_IN_TIME, _Treatment.TDL_TREATMENT_TYPE_ID ?? 0);

                        _ThongTinDieuTri.SoNgayDieuTriKhoa1 = Inventec.Common.TypeConvert.Parse.ToInt32(songay1.ToString());

                        _ThongTinDieuTri.ChuyenKhoa2 = _DepartmentTranCKs[1].DEPARTMENT_NAME;
                        _ThongTinDieuTri.NgayChuyenKhoa2 = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTimeUTC(_DepartmentTranCKs[1].DEPARTMENT_IN_TIME ?? 0) ?? null;
                        if (_DepartmentTranCKs.Count > 2)
                        {
                            long? songay2 = null;

                            songay2 = DayOfTreatmentDepartment(_DepartmentTranCKs[1].DEPARTMENT_IN_TIME ?? 0, _DepartmentTranCKs[2].DEPARTMENT_IN_TIME, _Treatment.TDL_TREATMENT_TYPE_ID ?? 0);

                            _ThongTinDieuTri.SoNgayDieuTriKhoa2 = Inventec.Common.TypeConvert.Parse.ToInt32(songay2.ToString());
                            _ThongTinDieuTri.ChuyenKhoa3 = _DepartmentTranCKs[2].DEPARTMENT_NAME;
                            _ThongTinDieuTri.NgayChuyenKhoa3 = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTimeUTC(_DepartmentTranCKs[2].DEPARTMENT_IN_TIME ?? 0) ?? null;

                            if (_DepartmentTranCKs.Count > 3)
                            {
                                long? songay3 = null;

                                songay3 = DayOfTreatmentDepartment(_DepartmentTranCKs[2].DEPARTMENT_IN_TIME ?? 0, _DepartmentTranCKs[3].DEPARTMENT_IN_TIME, _Treatment.TDL_TREATMENT_TYPE_ID ?? 0);

                                _ThongTinDieuTri.SoNgayDieuTriKhoa3 = Inventec.Common.TypeConvert.Parse.ToInt32(songay3.ToString());
                            }
                        }
                    }
                }

                var patientCodelv1 = BackendDataWorker.Get<HIS_MEDI_ORG>().Where(p => p.MEDI_ORG_CODE == _Treatment.MEDI_ORG_CODE).FirstOrDefault();
                var hisBranch = BackendDataWorker.Get<HIS_BRANCH>().Where(p => p.ID == _Treatment.BRANCH_ID).FirstOrDefault();
                var patientCodelv2 = hisBranch != null ? BackendDataWorker.Get<HIS_MEDI_ORG>().Where(p => p.MEDI_ORG_CODE == hisBranch.HEIN_MEDI_ORG_CODE).FirstOrDefault() : null;
                if (patientCodelv1 != null && patientCodelv2 != null)
                {
                    _ThongTinDieuTri.ChuyenVien = GetChuyenVienFromTranspatiForm(patientCodelv1.LEVEL_CODE, patientCodelv2.LEVEL_CODE);
                }

                _ThongTinDieuTri.TenVienChuyenBenhNhanDen = _Treatment.MEDI_ORG_NAME;
                _ThongTinDieuTri.MaVienChuyenBenhNhanDen = _Treatment.MEDI_ORG_CODE;
                if (_Treatment.OUT_TIME > 0)
                    _ThongTinDieuTri.NgayRaVien = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTimeUTC(_Treatment.OUT_TIME ?? 0) ?? null;
                if (_Treatment.TREATMENT_END_TYPE_ID > 0)
                    _ThongTinDieuTri.TinhTrangRaVien = _Treatment.TREATMENT_END_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__HEN ? TinhTrangRaVien.HenKham : _Treatment.TREATMENT_END_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__RAVIEN ? TinhTrangRaVien.RaVien : _Treatment.TREATMENT_END_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__TRON ? TinhTrangRaVien.BoVe : _Treatment.TREATMENT_END_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__XINRAVIEN ? TinhTrangRaVien.XinVe : _Treatment.TREATMENT_END_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__CHUYEN ? TinhTrangRaVien.ChuyenVien : TinhTrangRaVien.DuaVe;
                long _snDieuTri = 0;
                if (HisConfigCFG.IsTreatmentDayCount6556)
                {
                    _snDieuTri = _snDieuTri = HIS.Common.Treatment.Calculation.DayOfTreatment6556(_Treatment.CLINICAL_IN_TIME ?? 0, _Treatment.CLINICAL_IN_TIME, _Treatment.OUT_TIME, _Treatment.TDL_TREATMENT_TYPE_ID ?? 0) ?? 0;
                }
                else
                {
                    _snDieuTri = _snDieuTri = HIS.Common.Treatment.Calculation.DayOfTreatment(_Treatment.CLINICAL_IN_TIME, _Treatment.OUT_TIME, _Treatment.TREATMENT_END_TYPE_ID, _Treatment.TREATMENT_RESULT_ID, ptyType) ?? 0;
                }

                _ThongTinDieuTri.TongSoNgayDieuTri1 = _snDieuTri.ToString();
                _ThongTinDieuTri.TongSoNgayDieuTri2 = _snDieuTri.ToString();

                if (_Treatment.IS_TRANSFER_IN == 1)
                {
                    _ThongTinDieuTri.ChanDoan_NoiChuyenDen = _Treatment.TRANSFER_IN_ICD_NAME;
                    _ThongTinDieuTri.MaICD_NoiChuyenDen = _Treatment.TRANSFER_IN_ICD_CODE;
                }

                if (_Treatment.OUT_TIME.HasValue && _Treatment.OUT_TIME > 0)
                {
                    _ThongTinDieuTri.BenhChinh_RaVien = _Treatment.ICD_NAME;
                    _ThongTinDieuTri.MaICD_BenhChinh_RaVien = _Treatment.ICD_CODE;//"41";
                    _ThongTinDieuTri.BenhKemTheo_RaVien = _Treatment.ICD_TEXT;
                    _ThongTinDieuTri.NguyenNhan_BenhChinh_RaVien = _Treatment.ICD_CAUSE_NAME;
                    _ThongTinDieuTri.MaICD_NguyenNhan_BenhChinh_RV = _Treatment.ICD_CAUSE_CODE;
                    _ThongTinDieuTri.MaICD_BenhKemTheo_RaVien = _Treatment.ICD_SUB_CODE; //"42";
                    _ThongTinDieuTri.YHCT_ChuanDoanaRaVien = _Treatment.TRADITIONAL_ICD_NAME;//"40";
                }

                var KhoaNhapVien = _DepartmentTrans.Where(o => o.IS_HOSPITALIZED == 1).OrderBy(p => p.DEPARTMENT_IN_TIME).FirstOrDefault();
                V_HIS_DEPARTMENT_TRAN departmentTranNT = null;
                if (KhoaNhapVien != null)
                {
                    _ThongTinDieuTri.ChanDoan_KhiVaoKhoaDieuTri = KhoaNhapVien.ICD_NAME;
                    _ThongTinDieuTri.MaICD_KhiVaoKhoaDieuTri = KhoaNhapVien.ICD_CODE;
                }
                else
                {
                    var PatientTypeAlterNT = patientTypeAlters != null && patientTypeAlters.Count() > 0 ? patientTypeAlters.Where(o => o.TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTNOITRU).OrderBy(o => o.LOG_TIME).FirstOrDefault() : null;
                    if (PatientTypeAlterNT != null)
                        departmentTranNT = _DepartmentTrans.FirstOrDefault(o => o.ID == PatientTypeAlterNT.DEPARTMENT_TRAN_ID);
                    if (departmentTranNT != null)
                    {
                        _ThongTinDieuTri.ChanDoan_KhiVaoKhoaDieuTri = departmentTranNT.ICD_NAME;
                        _ThongTinDieuTri.MaICD_KhiVaoKhoaDieuTri = departmentTranNT.ICD_CODE;
                    }
                    else
                    {
                        _ThongTinDieuTri.ChanDoan_KhiVaoKhoaDieuTri = _Treatment.ICD_NAME;
                        _ThongTinDieuTri.MaICD_KhiVaoKhoaDieuTri = _Treatment.ICD_CODE;
                    }
                }

                _ThongTinDieuTri.VaoVienDoBenhNayLanThu = (int)TreatmentIcdCount;
                if (sereServPttts != null && sereServPttts.Count > 0)
                {
                    LogSystem.Debug("LoadDataEmr. 2.2");

                    int? dem = null;
                    var phauthuat = sereServAlls.Where(o => o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__PT).ToList();
                    List<V_HIS_SERE_SERV_PTTT> sereServPt = new List<V_HIS_SERE_SERV_PTTT>();
                    foreach (var item in phauthuat)
                    {
                        var check = sereServPttts.Where(o => o.SERE_SERV_ID == item.ID).ToList();
                        if (check != null && check.Count > 0)
                        {
                            sereServPt.AddRange(check);
                        }
                    }
                    _ThongTinDieuTri.TongSoLanPhauThuat = sereServPt.Count;

                    string BeforeAfterSurgeryICDOption = HisConfigs.Get<string>(SdaConfigKeys.BeforeAfterSurgeryICDOption);
                    sereServPttts = sereServPttts.OrderBy(o => o.CREATE_TIME).ToList();
                    sereServPt = sereServPt.OrderBy(o => o.CREATE_TIME).ToList();
                    LogSystem.Debug("LoadDataEmr. 2.4");
                    _ThongTinDieuTri.TongSoNgayDieuTriSauPT = null;//TODO
                    _ThongTinDieuTri.LyDoTaiBienBienChung = null;//TODO
                    _ThongTinDieuTri.MaICD_NguyenNhan_BenhChinh_RV = sereServPttts[0].ICD_CODE;

                    if (BeforeAfterSurgeryICDOption == "1")
                    {
                        if (sereServPt.Count > 0)
                        {
                            _ThongTinDieuTri.MaICD_ChanDoanSauPhauThuat = sereServPt[0].AFTER_PTTT_ICD_CODE;
                            _ThongTinDieuTri.MaICD_ChanDoanTruocPhauThuat = sereServPt[0].BEFORE_PTTT_ICD_CODE;
                            _ThongTinDieuTri.ChanDoanSauPhauThuat = sereServPt[0].AFTER_PTTT_ICD_NAME;
                            _ThongTinDieuTri.ChanDoanTruocPhauThuat = sereServPt[0].BEFORE_PTTT_ICD_NAME;
                        }
                    }
                    else
                    {
                        _ThongTinDieuTri.MaICD_ChanDoanSauPhauThuat = sereServPttts[0].AFTER_PTTT_ICD_CODE;
                        _ThongTinDieuTri.MaICD_ChanDoanTruocPhauThuat = sereServPttts[0].BEFORE_PTTT_ICD_CODE;
                        _ThongTinDieuTri.ChanDoanSauPhauThuat = sereServPttts[0].AFTER_PTTT_ICD_NAME;
                        _ThongTinDieuTri.ChanDoanTruocPhauThuat = sereServPttts[0].BEFORE_PTTT_ICD_NAME;
                    }

                    _ThongTinDieuTri.TaiBien = sereServPttts.Any(o => o.PTTT_CATASTROPHE_ID > 0);
                }

                LogSystem.Debug("LoadDataEmr. 2.5");
                if (_Treatment.TREATMENT_RESULT_ID > 0)
                    _ThongTinDieuTri.KetQuaDieuTri = _Treatment.TREATMENT_RESULT_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_RESULT.ID__CHET ? KetQuaDieuTri.TuVong : _Treatment.TREATMENT_RESULT_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_RESULT.ID__DO ? KetQuaDieuTri.GiamDo : _Treatment.TREATMENT_RESULT_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_RESULT.ID__KHOI ? KetQuaDieuTri.Khoi : _Treatment.TREATMENT_RESULT_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_RESULT.ID__NANG ? KetQuaDieuTri.NangHon : KetQuaDieuTri.KhongThayDoi;
                _ThongTinDieuTri.GiaiPhauBenh = 0;
                if (_Treatment.DEATH_TIME > 0)
                    _ThongTinDieuTri.NgayTuVong = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTimeUTC(_Treatment.DEATH_TIME ?? 0);
                if (_Treatment.DEATH_CAUSE_ID > 0)
                {
                    var deathCause = BackendDataWorker.Get<HIS_DEATH_CAUSE>().Where(o => o.ID == _Treatment.DEATH_CAUSE_ID).FirstOrDefault();
                    if (deathCause != null)
                    {
                        _ThongTinDieuTri.LyDoTuVong = deathCause.DEATH_CAUSE_CODE == "01" ? LyDoTuVong.DoBenh : deathCause.DEATH_CAUSE_CODE == "02" ? LyDoTuVong.DoTaiBienDieuTri : LyDoTuVong.Khac;
                        _ThongTinDieuTri.NguyenNhanChinhTuVong = deathCause.DEATH_CAUSE_NAME;
                        _ThongTinDieuTri.MaICD_NguyenNhanChinhTuVong = _Treatment.ICD_CODE;
                    }
                }
                if (_Treatment.DEATH_WITHIN_ID > 0)
                {
                    _ThongTinDieuTri.ThoiGianTuVong = _Treatment.DEATH_WITHIN_ID == 1 ? ThoiGianTuVong.Trong24hVaoVien : ThoiGianTuVong.Sau24hvaoVien;

                    _ThongTinDieuTri.KhamNghiemTuThi = false;
                    _ThongTinDieuTri.ChanDoanGiaiPhauTuThi = "";
                    _ThongTinDieuTri.MaICD_ChanDoanGiaiPhauTuThi = "";
                }

                _ThongTinDieuTri.SoNhapVien = _Treatment.IN_CODE;

                if (treatmentBedRoom != null)
                {
                    _ThongTinDieuTri.Buong = treatmentBedRoom.BED_ROOM_NAME;
                    _ThongTinDieuTri.Giuong = treatmentBedRoom.BED_CODE;
                    _ThongTinDieuTri.TenGiuong = treatmentBedRoom.BED_NAME;

                    var bedroom = BackendDataWorker.Get<V_HIS_BED_ROOM>().Where(o => o.ID == treatmentBedRoom.BED_ROOM_ID).FirstOrDefault();

                    if (bedroom != null)
                    {
                        _ThongTinDieuTri.MaKhoa = bedroom.DEPARTMENT_CODE;
                        _ThongTinDieuTri.Khoa = bedroom.DEPARTMENT_NAME;
                    }
                }
                else
                {
                    V_HIS_DEPARTMENT_TRAN departmentTran1 = (_PatientTypeAlter != null && _DepartmentTrans != null && _DepartmentTrans.Count > 0) ? _DepartmentTrans.Where(p => p.ID == _PatientTypeAlter.DEPARTMENT_TRAN_ID && p.DEPARTMENT_IN_TIME != null).First() : null;
                    _ThongTinDieuTri.MaKhoa = departmentTran1 != null ? departmentTran1.DEPARTMENT_CODE : "";
                    _ThongTinDieuTri.Khoa = departmentTran1 != null ? departmentTran1.DEPARTMENT_NAME : "";
                }

                _ThongTinDieuTri.MaGiamDocBenhVien = "";
                _ThongTinDieuTri.NgayThangNamTrangBia = DateTime.Now;
                _ThongTinDieuTri.MaTruongKhoa = "";
                _ThongTinDieuTri.BacSiKhamBenh = _Treatment.IN_LOGINNAME + " - " + _Treatment.IN_USERNAME;

                LogSystem.Debug("LoadDataEmr. 2.5.1");
                string Ten_KKB_CapCuu = "", Ma_KKB_CapCuu = "";

                if (currentServiceReqs != null && currentServiceReqs.Count > 0)
                {
                    V_HIS_SERVICE_REQ VServiceReqmain = currentServiceReqs.Where(p => p.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__KH && p.IS_MAIN_EXAM == 1).FirstOrDefault();
                    if (VServiceReqmain != null)
                    {
                        Ten_KKB_CapCuu = VServiceReqmain.ICD_NAME;
                        Ma_KKB_CapCuu = VServiceReqmain.ICD_CODE;
                    }
                    else
                    {
                        V_HIS_SERVICE_REQ VServiceReq = currentServiceReqs.Where(p => p.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__KH).OrderBy(o => o.INTRUCTION_TIME).ThenBy(o => o.ID).FirstOrDefault();
                        Ten_KKB_CapCuu = VServiceReq != null ? VServiceReq.ICD_NAME : null;
                        Ma_KKB_CapCuu = VServiceReq != null ? VServiceReq.ICD_CODE : null;
                    }
                }

                if (!string.IsNullOrEmpty(_Treatment.ICD_TEXT))
                {
                    _ThongTinDieuTri.BenhKemTheo = _Treatment.ICD_TEXT;
                }

                _ThongTinDieuTri.ChanDoan_KKB_CapCuu = Ten_KKB_CapCuu;
                _ThongTinDieuTri.MaICD_KKB_CapCuu = Ma_KKB_CapCuu;

                _ThongTinDieuTri.ChanDoan_YHCT = _Treatment.TRADITIONAL_ICD_NAME;
                _ThongTinDieuTri.LyDoVaoVien = _Treatment.HOSPITALIZATION_REASON;

                _ThongTinDieuTri.ChanDoanVaoVien = _Treatment.IN_ICD_NAME;
                _ThongTinDieuTri.DienDieuTri = int.Parse((_Treatment.TDL_TREATMENT_TYPE_ID ?? 0).ToString());

                #endregion
                LogSystem.Debug("LoadDataEmr. 2.6");

                #region ------ Kháng sinh
                if (_ExpMestMedicine != null && _ExpMestMedicine.Count > 0)
                {
                    KhangSinh_HISs = new List<ThuocKhangSinh>();

                    List<V_HIS_MEDICINE_TYPE> lstHisMedicineType = new List<V_HIS_MEDICINE_TYPE>();
                    List<HIS_EXP_MEST_MEDICINE> listExpMestMedicine = new List<HIS_EXP_MEST_MEDICINE>();
                    lstHisMedicineType = BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>().Where(o => o.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__KS).ToList();
                    foreach (var item in lstHisMedicineType)
                    {
                        var lstcheckMedicine = _ExpMestMedicine.Where(o => o.TDL_MEDICINE_TYPE_ID == item.ID).ToList();
                        if (lstcheckMedicine != null && lstcheckMedicine.Count > 0)
                        {
                            lstcheckMedicine = lstcheckMedicine.OrderBy(o => o.TDL_MEDICINE_TYPE_ID).ToList();
                            ThuocKhangSinh khangsinh = new ThuocKhangSinh();

                            khangsinh.MaKhangSinh = item.MEDICINE_TYPE_CODE;
                            khangsinh.TenKhangSinh = item.MEDICINE_TYPE_NAME;
                            khangsinh.SoLuong = lstcheckMedicine.Sum(o => o.AMOUNT);
                            khangsinh.DonVi = item.SERVICE_UNIT_NAME;
                            khangsinh.HuongDanSuDung = lstcheckMedicine.Where(x => x.USE_TIME_TO == lstcheckMedicine.Max(p => p.USE_TIME_TO)).FirstOrDefault().TUTORIAL;

                            KhangSinh_HISs.Add(khangsinh);
                        }
                    }
                }
                #endregion
                LogSystem.Debug("LoadDataEmr. 2.6.1");

                #region SoLuuTru, MaYTe
                if (_Treatment != null)
                {
                    SoLuuTru = this.GenerateStorageNumber(_Treatment.END_CODE, _Treatment.DEATH_TIME, _Treatment.ICD_CAUSE_CODE);

                    if (HisBranch_WorkPlace != null)
                    {
                        string Year = DateTime.Now.Year.ToString().Substring(2);

                        if (!String.IsNullOrEmpty(HisBranch_WorkPlace.HEIN_PROVINCE_CODE) && HisBranch_WorkPlace.HEIN_PROVINCE_CODE.Length < 3)
                        {
                            MaYTe = HisBranch_WorkPlace.HEIN_PROVINCE_CODE.PadLeft(3, '0');
                        }

                        if (!String.IsNullOrEmpty(HisBranch_WorkPlace.BRANCH_CODE) && HisBranch_WorkPlace.BRANCH_CODE.Length < 3)
                        {
                            MaYTe += "/" + HisBranch_WorkPlace.BRANCH_CODE.PadLeft(3, '0');
                        }

                        MaYTe += "/" + Year;

                        if (!String.IsNullOrEmpty(_Treatment.TREATMENT_CODE))
                        {
                            MaYTe += "/" + _Treatment.TREATMENT_CODE.Substring(_Treatment.TREATMENT_CODE.Length - 6);
                        }
                    }
                }
                #endregion
                LogSystem.Debug("LoadDataEmr. 2.6.2");

                #region ------ DHST
                _ThongTinDieuTri.DauSinhTon = new DauSinhTon();
                DauSinhTon _DauSinhTon = new DauSinhTon();
                if (_DHST != null)
                {
                    if (_DHST.WEIGHT != null)
                    {
                        _DauSinhTon.CanNang = (double)_DHST.WEIGHT;
                    }
                    if (_DHST.BLOOD_PRESSURE_MAX != null && _DHST.BLOOD_PRESSURE_MIN != null)
                    {
                        _DauSinhTon.HuyetAp = _DHST.BLOOD_PRESSURE_MAX + "/" + _DHST.BLOOD_PRESSURE_MIN;
                    }
                    if (_DHST.PULSE != null)
                    {
                        _DauSinhTon.Mach = (int)_DHST.PULSE;
                    }
                    if (_DHST.TEMPERATURE != null)
                    {
                        _DauSinhTon.NhietDo = (double)_DHST.TEMPERATURE;
                    }
                    if (_DHST.BREATH_RATE != null)
                    {
                        _DauSinhTon.NhipTho = (int)_DHST.BREATH_RATE;
                    }
                    if (_DHST.HEIGHT != null)
                    {
                        _DauSinhTon.ChieuCao = (double)_DHST.HEIGHT;
                    }
                    if (_DHST.VIR_BMI != null)
                    {
                        _DauSinhTon.BMI = (double)_DHST.VIR_BMI;
                    }
                }
                _ThongTinDieuTri.DauSinhTon = _DauSinhTon;
                #endregion

                #region ------ DauHieuSinhTonMoi
                DauSinhTon _DauHieuSinhTonMoi = new DauSinhTon();
                if (_DHSTMOI != null)
                {
                    if (_DHSTMOI.WEIGHT != null)
                    {
                        _DauHieuSinhTonMoi.CanNang = (double)_DHSTMOI.WEIGHT;
                    }
                    if (_DHSTMOI.BLOOD_PRESSURE_MAX != null || _DHSTMOI.BLOOD_PRESSURE_MIN != null)
                    {
                        _DauHieuSinhTonMoi.HuyetAp = _DHSTMOI.BLOOD_PRESSURE_MAX + "/" + _DHSTMOI.BLOOD_PRESSURE_MIN;
                    }
                    if (_DHSTMOI.PULSE != null)
                    {
                        _DauHieuSinhTonMoi.Mach = (int)_DHSTMOI.PULSE;
                    }
                    if (_DHSTMOI.TEMPERATURE != null)
                    {
                        _DauHieuSinhTonMoi.NhietDo = (double)_DHSTMOI.TEMPERATURE;
                    }
                    if (_DHSTMOI.BREATH_RATE != null)
                    {
                        _DauHieuSinhTonMoi.NhipTho = (int)_DHSTMOI.BREATH_RATE;
                    }
                    if (_DHSTMOI.HEIGHT != null)
                    {
                        _DauHieuSinhTonMoi.ChieuCao = (double)_DHSTMOI.HEIGHT;
                    }
                    if (_DHSTMOI.VIR_BMI != null)
                    {
                        _DauHieuSinhTonMoi.BMI = (double)_DHSTMOI.VIR_BMI;
                    }
                }
                #endregion

                #region ------ HoSo
                _ThongTinDieuTri.HoSo = new HoSo();
                //- Số tờ X-Quang: Tổng số lượng các dịch vụ có loại dịch vụ là CĐHA và có loại CĐHA là XQ (DIIM_TYPE_ID trong HIS_SERVICE).
                //- Số tờ Scanner: Tổng số lượng các dịch vụ có loại dịch vụ là CĐHA và có loại CĐHA là CT (DIIM_TYPE_ID trong HIS_SERVICE).
                //- Số tờ Siêu âm: Tổng số lượng các dịch vụ có loại dịch vụ là Siêu Âm.
                //- Số tờ Xét nghiệm: Tổng số lượng các dịch vụ có loại dịch vụ là Xét Nghiệm.
                //- Khác:
                //+ Số tơ khác: Tổng số lượng các dịch vụ cận lâm sàn khác các dịch vụ trên.
                //+ Tên các loại dịch vụ: Tên các loại dịch vụ cận lâm sàn khác các dịch vụ trên cách nhau bởi dấu ,: Nội soi, Thủ thuật, ...
                //- Chú y không tính các dịch vụ được đánh dấu "Không thực hiện".
                HoSo _HoSo = new HoSo();

                if (sereServAlls != null && sereServAlls.Count > 0 && sereServAlls.Any(o => o.IS_NO_EXECUTE == null || o.IS_NO_EXECUTE != 1))
                {
                    List<long> serviceclsType = new List<long> { IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__CDHA,
                    IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__NS,
                    IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__SA,
                    IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__XN,
                    IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__GPBL,
                    IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__TDCN};
                    var sereServByTypeAlls = sereServAlls.Where(o => (o.IS_NO_EXECUTE == null || o.IS_NO_EXECUTE != 1) && serviceclsType.Contains(o.TDL_SERVICE_TYPE_ID)).ToList();
                    _HoSo.XQuang = sereServByTypeAlls.Count(o => o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__CDHA
                        && CheckDiimTypeService(o.SERVICE_ID, IMSys.DbConfig.HIS_RS.HIS_DIIM_TYPE.ID__XQ));
                    _HoSo.CTScanner = sereServByTypeAlls.Count(o => o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__CDHA
                        && CheckDiimTypeService(o.SERVICE_ID, IMSys.DbConfig.HIS_RS.HIS_DIIM_TYPE.ID__CT));
                    _HoSo.SieuAm = sereServByTypeAlls.Count(o => o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__SA);
                    _HoSo.XetNghiem = sereServByTypeAlls.Count(o => o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__XN);
                    _HoSo.Khac = (sereServByTypeAlls.Count - _HoSo.XQuang - _HoSo.CTScanner - _HoSo.SieuAm - _HoSo.XetNghiem);
                    var serviceOrthers = sereServByTypeAlls.Where(o => !((o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__CDHA
                        && (CheckDiimTypeService(o.SERVICE_ID, IMSys.DbConfig.HIS_RS.HIS_DIIM_TYPE.ID__XQ) || CheckDiimTypeService(o.SERVICE_ID, IMSys.DbConfig.HIS_RS.HIS_DIIM_TYPE.ID__CT))
                        ) || (o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__SA)
                        || (o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__XN))
                        ).ToList();
                    if (serviceOrthers != null && serviceOrthers.Count > 0)
                    {
                        var serviceTypeIds = serviceOrthers.Select(o => o.TDL_SERVICE_TYPE_ID).ToList();
                        var serviceTypes = BackendDataWorker.Get<HIS_SERVICE_TYPE>();
                        _HoSo.Khac_Text = (serviceTypes != null && serviceTypes.Count > 0 ? String.Join(", ", serviceTypes.Where(o => serviceTypeIds.Contains(o.ID)).Select(o => o.SERVICE_TYPE_NAME)) : "");
                    }
                    _HoSo.ToanBoHoSo = sereServByTypeAlls.Count;
                }
                _ThongTinDieuTri.HoSo = _HoSo;
                #endregion
                LogSystem.Debug("LoadDataEmr. 3");
                #region Data Benh an
                BenhAnCommonADO _BenhAnCommonADO = new BenhAnCommonADO();
                string json = "";

                _BenhAnCommonADO.DauSinhTon = new DauSinhTon();
                _BenhAnCommonADO.HoSo = new HoSo();
                _BenhAnCommonADO.DacDiemLienQuanBenh = new EMR_MAIN.DacDiemLienQuanBenh()
                {
                    DiUng = false,
                    DiUng_Text = "",
                    Khac_DacDiemLienQuanBenh = false,
                    Khac_DacDiemLienQuanBenh_Text = "",
                    MaTuy = false,
                    MaTuy_Text = "",
                    RuouBia = false,
                    RuouBia_Text = "",
                    ThuocLa = false,
                    ThuocLao = false,
                    ThuocLao_Text = "",
                    ThuocLa_Text = ""
                };

                _BenhAnCommonADO.BenhChinh = _Treatment.ICD_NAME;//"36";
                if (!string.IsNullOrEmpty(_Treatment.ICD_TEXT))
                {
                    _BenhAnCommonADO.BenhKemTheo = _Treatment.ICD_TEXT;
                }
                _BenhAnCommonADO.PhuongPhapDieuTri = _Treatment.TREATMENT_METHOD;
                _BenhAnCommonADO.QuaTrinhBenhLyVaDienBien = _Treatment.CLINICAL_NOTE;
                _BenhAnCommonADO.TomTatKetQuaXetNghiem = _Treatment.SUBCLINICAL_RESULT;
                _BenhAnCommonADO.CacXetNghiemCanLamSangCanLam = sereServCLSs != null ? String.Join(";", sereServCLSs.Select(o => o.TDL_SERVICE_NAME).Distinct().ToArray()) : "";
                _BenhAnCommonADO.CacXetNghiemCanLamSangCanLam = _BenhAnCommonADO.CacXetNghiemCanLamSangCanLam.Length > 2048 ? _BenhAnCommonADO.CacXetNghiemCanLamSangCanLam.Substring(0, 2048) : _BenhAnCommonADO.CacXetNghiemCanLamSangCanLam;

                if (_ExamServiceReq != null)
                {
                    _BenhAnCommonADO.BacSyDieuTri = _ExamServiceReq.EXECUTE_USERNAME;
                    _BenhAnCommonADO.BacSyLamBenhAn = _ExamServiceReq.REQUEST_USERNAME;
                    _BenhAnCommonADO.CoXuongKhop = _ExamServiceReq.PART_EXAM_MUSCLE_BONE;
                    _BenhAnCommonADO.HoHap = _ExamServiceReq.PART_EXAM_RESPIRATORY;
                    _BenhAnCommonADO.HuongDieuTri = _ExamServiceReq.NEXT_TREATMENT_INSTRUCTION;
                    _BenhAnCommonADO.HuongDieuTriVaCacCheDoTiepTheo = _ExamServiceReq.NEXT_TREATMENT_INSTRUCTION;
                    _BenhAnCommonADO.Khac_CacCoQuan = _ExamServiceReq.PART_EXAM;
                    _BenhAnCommonADO.LyDoVaoVien = _ExamServiceReq.HOSPITALIZATION_REASON;
                    _BenhAnCommonADO.MaQuanLy = Inventec.Common.TypeConvert.Parse.ToDecimal(_ExamServiceReq.TREATMENT_CODE);
                    _BenhAnCommonADO.Mat = _ExamServiceReq.PART_EXAM_EYE;
                    _BenhAnCommonADO.NgayKhamBenh = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTimeUTC(_ExamServiceReq.INTRUCTION_DATE) ?? DateTime.Now;
                    _BenhAnCommonADO.NgayTongKet = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTimeUTC(_ExamServiceReq.FINISH_TIME ?? 0) ?? DateTime.Now;
                    _BenhAnCommonADO.NguoiGiaoHoSo = _ExamServiceReq.REQUEST_USERNAME;
                    _BenhAnCommonADO.NguoiNhanHoSo = _ExamServiceReq.EXECUTE_USERNAME;
                    _BenhAnCommonADO.PhanBiet = "";
                    _BenhAnCommonADO.QuaTrinhBenhLy = _ExamServiceReq.PATHOLOGICAL_PROCESS;
                    _BenhAnCommonADO.RangHamMat = _ExamServiceReq.PART_EXAM_STOMATOLOGY;
                    _BenhAnCommonADO.TaiMuiHong = String.IsNullOrEmpty(_ExamServiceReq.PART_EXAM_ENT) ? _ExamServiceReq.PART_EXAM_EAR + " " + _ExamServiceReq.PART_EXAM_NOSE + " " + _ExamServiceReq.PART_EXAM_THROAT : _ExamServiceReq.PART_EXAM_ENT;
                    _BenhAnCommonADO.TienLuong = "";
                    _BenhAnCommonADO.TienSuBenhBanThan = _ExamServiceReq.PATHOLOGICAL_HISTORY;
                    _BenhAnCommonADO.TienSuBenhGiaDinh = _ExamServiceReq.PATHOLOGICAL_HISTORY_FAMILY;
                    _BenhAnCommonADO.TieuHoa = _ExamServiceReq.PART_EXAM_DIGESTION;
                    _BenhAnCommonADO.TinhTrangNguoiBenhRaVien = _ExamServiceReq.ADVISE;
                    _BenhAnCommonADO.ToanThan = _ExamServiceReq.FULL_EXAM;
                    _BenhAnCommonADO.TomTatBenhAn = _ExamServiceReq.SUBCLINICAL;
                    _BenhAnCommonADO.TuanHoan = _ExamServiceReq.PART_EXAM_CIRCULATION;
                    _BenhAnCommonADO.ThanKinh = _ExamServiceReq.PART_EXAM_NEUROLOGICAL;
                    _BenhAnCommonADO.ThanTietNieuSinhDuc = _ExamServiceReq.PART_EXAM_KIDNEY_UROLOGY;
                    _BenhAnCommonADO.VaoNgayThu = Inventec.Common.TypeConvert.Parse.ToInt32(_ExamServiceReq.SICK_DAY > 0 ? _ExamServiceReq.SICK_DAY.ToString() : "0");
                }
                if (_TYpe == LoaiBenhAnEMR.NoiKhoa)
                {
                    #region ----NoiKhoa
                    BenhAnNoiKhoa _BenhAnNoiKhoa = new BenhAnNoiKhoa();
                    Inventec.Common.Mapper.DataObjectMapper.Map<BenhAnNoiKhoa>(_BenhAnNoiKhoa, _BenhAnCommonADO);
                    _BenhAnNoiKhoa.PhuongPhapDieuTri = _BenhAnCommonADO.PhuongPhapDieuTri;
                    _BenhAnNoiKhoa.QuaTrinhBenhLyVaDienBien = _BenhAnCommonADO.QuaTrinhBenhLyVaDienBien;
                    json = Newtonsoft.Json.JsonConvert.SerializeObject(_BenhAnNoiKhoa);
                    #endregion
                }
                else if (_TYpe == LoaiBenhAnEMR.NgoaiKhoa)
                {
                    #region ----NgoaiKhoa
                    BenhAnNgoaiKhoa _BenhAnNgoaiKhoa = new BenhAnNgoaiKhoa();
                    Inventec.Common.Mapper.DataObjectMapper.Map<BenhAnNgoaiKhoa>(_BenhAnNgoaiKhoa, _BenhAnCommonADO);
                    json = Newtonsoft.Json.JsonConvert.SerializeObject(_BenhAnNgoaiKhoa);
                    #endregion
                }
                else if (_TYpe == LoaiBenhAnEMR.DaLieu)
                {
                    #region ----DaLieu
                    BenhAnDaLieu _BenhAnDaLieu = new BenhAnDaLieu();
                    Inventec.Common.Mapper.DataObjectMapper.Map<BenhAnDaLieu>(_BenhAnDaLieu, _BenhAnCommonADO);
                    json = Newtonsoft.Json.JsonConvert.SerializeObject(_BenhAnDaLieu);
                    #endregion
                }
                else if (_TYpe == LoaiBenhAnEMR.Bong)
                {
                    #region ----Bong
                    BenhAnBong _BenhAnBong = new BenhAnBong();
                    Inventec.Common.Mapper.DataObjectMapper.Map<BenhAnBong>(_BenhAnBong, _BenhAnCommonADO);
                    _BenhAnBong.HinhAnhHoacVe = "";
                    _BenhAnBong.PhauThuat = false;
                    _BenhAnBong.SinhDuc = "";
                    _BenhAnBong.ThuThuat = false;
                    _BenhAnBong.TonThuongBong = "";
                    json = Newtonsoft.Json.JsonConvert.SerializeObject(_BenhAnBong);
                    #endregion
                }
                else if (_TYpe == LoaiBenhAnEMR.HuyetHocTruyenMau)
                {
                    #region ----Huyết học truyền máu
                    BenhAnHuyetHocTruyenMau _BenhAnHuyetHocTruyenMau = new BenhAnHuyetHocTruyenMau();
                    Inventec.Common.Mapper.DataObjectMapper.Map<BenhAnHuyetHocTruyenMau>(_BenhAnHuyetHocTruyenMau, _BenhAnCommonADO);
                    _BenhAnHuyetHocTruyenMau.Bo_Gan = "";
                    _BenhAnHuyetHocTruyenMau.Bo_Lach = "";
                    _BenhAnHuyetHocTruyenMau.CacPhanUngKhiTruyenMau = 0;
                    _BenhAnHuyetHocTruyenMau.CoombsTest = false;
                    _BenhAnHuyetHocTruyenMau.DaNiemMac = "";
                    _BenhAnHuyetHocTruyenMau.Dau_Gan = "";
                    _BenhAnHuyetHocTruyenMau.Dau_Hach = "";
                    _BenhAnHuyetHocTruyenMau.Dau_Lach = "";
                    _BenhAnHuyetHocTruyenMau.DienDiHST = false;
                    _BenhAnHuyetHocTruyenMau.DinhLuongYeuToMauDong = false;
                    _BenhAnHuyetHocTruyenMau.DoDiDong_Hach = "";
                    _BenhAnHuyetHocTruyenMau.DongMauToanBo = false;
                    _BenhAnHuyetHocTruyenMau.GPB = false;
                    _BenhAnHuyetHocTruyenMau.HeThongLong_Toc_Mong = "";
                    _BenhAnHuyetHocTruyenMau.HinhDangTuThe = "";
                    _BenhAnHuyetHocTruyenMau.HongCauRua = 0;
                    _BenhAnHuyetHocTruyenMau.HuyetDo = false;
                    _BenhAnHuyetHocTruyenMau.HuyetTuongDongLanh = 0;
                    _BenhAnHuyetHocTruyenMau.HuyetTuuong = 0;
                    _BenhAnHuyetHocTruyenMau.KhangTheBatThuong = false;
                    _BenhAnHuyetHocTruyenMau.KhoiBachCauHat = 0;
                    _BenhAnHuyetHocTruyenMau.KhoiHongCau = 0;
                    _BenhAnHuyetHocTruyenMau.KhoiTieuCau = 0;
                    _BenhAnHuyetHocTruyenMau.KichThuoc_Gan = "";
                    _BenhAnHuyetHocTruyenMau.KichThuoc_Hach = "";
                    _BenhAnHuyetHocTruyenMau.KichThuoc_Lach = "";
                    _BenhAnHuyetHocTruyenMau.MatDo_Gan = "";
                    _BenhAnHuyetHocTruyenMau.MatDo_Lach = "";
                    _BenhAnHuyetHocTruyenMau.MatGan_Gan = "";
                    _BenhAnHuyetHocTruyenMau.MatGan_Lach = "";
                    _BenhAnHuyetHocTruyenMau.MatHach_Hach = "";
                    _BenhAnHuyetHocTruyenMau.NhiemSacThe = false;
                    _BenhAnHuyetHocTruyenMau.NhomMau = false;
                    _BenhAnHuyetHocTruyenMau.SinhHoa = false;
                    _BenhAnHuyetHocTruyenMau.SinhThietHach = false;
                    _BenhAnHuyetHocTruyenMau.SinhThietTuy = false;
                    _BenhAnHuyetHocTruyenMau.SoLuong_Hach = "";
                    _BenhAnHuyetHocTruyenMau.TinhThanCuaNguoiBenh = "";
                    _BenhAnHuyetHocTruyenMau.TrieuChungPhu = "";
                    _BenhAnHuyetHocTruyenMau.TrieuChungXuatHuyet = "";
                    _BenhAnHuyetHocTruyenMau.TruyenMauToanPhan = 0;
                    _BenhAnHuyetHocTruyenMau.TuaVIII = 0;
                    _BenhAnHuyetHocTruyenMau.TuyDo = false;
                    _BenhAnHuyetHocTruyenMau.TuyenGiap = "";
                    _BenhAnHuyetHocTruyenMau.ViSinh = false;
                    _BenhAnHuyetHocTruyenMau.ViTri_Hach = "";
                    _BenhAnHuyetHocTruyenMau.XQuang = false;
                    json = Newtonsoft.Json.JsonConvert.SerializeObject(_BenhAnHuyetHocTruyenMau);
                    #endregion
                }
                else if (_TYpe == LoaiBenhAnEMR.MatBanPhanTruoc)
                {
                    #region ----Mắt_bán phần trước
                    BenhAnMatBanPhanTruoc _BenhAnMatBanPhanTruoc = new BenhAnMatBanPhanTruoc();
                    Inventec.Common.Mapper.DataObjectMapper.Map<BenhAnMatBanPhanTruoc>(_BenhAnMatBanPhanTruoc, _BenhAnCommonADO);
                    json = Newtonsoft.Json.JsonConvert.SerializeObject(_BenhAnMatBanPhanTruoc);
                    #endregion
                }
                else if (_TYpe == LoaiBenhAnEMR.MatChanThuong)
                {
                    #region ----Mắt_chấn thương
                    BenhAnMatChanThuong _BenhAnMatChanThuong = new BenhAnMatChanThuong();
                    Inventec.Common.Mapper.DataObjectMapper.Map<BenhAnMatChanThuong>(_BenhAnMatChanThuong, _BenhAnCommonADO);
                    json = Newtonsoft.Json.JsonConvert.SerializeObject(_BenhAnMatChanThuong);
                    #endregion
                }
                else if (_TYpe == LoaiBenhAnEMR.MatDayMat)
                {
                    #region Mắt đáy mắt
                    BenhAnMatDayMat _BenhAnMatDayMat = new BenhAnMatDayMat();
                    Inventec.Common.Mapper.DataObjectMapper.Map<BenhAnMatDayMat>(_BenhAnMatDayMat, _BenhAnCommonADO);
                    json = Newtonsoft.Json.JsonConvert.SerializeObject(_BenhAnMatDayMat);
                    #endregion
                }
                else if (_TYpe == LoaiBenhAnEMR.MatGloCom)
                {
                    #region mắt glocom
                    BenhAnMatGlocom _BenhAnMatGlocom = new BenhAnMatGlocom();
                    Inventec.Common.Mapper.DataObjectMapper.Map<BenhAnMatGlocom>(_BenhAnMatGlocom, _BenhAnCommonADO);
                    json = Newtonsoft.Json.JsonConvert.SerializeObject(_BenhAnMatGlocom);
                    #endregion
                }
                else if (_TYpe == LoaiBenhAnEMR.MatLac)
                {
                    #region MatLac
                    BenhAnMatLac _BenhAnMatLac = new BenhAnMatLac();
                    Inventec.Common.Mapper.DataObjectMapper.Map<BenhAnMatLac>(_BenhAnMatLac, _BenhAnCommonADO);
                    json = Newtonsoft.Json.JsonConvert.SerializeObject(_BenhAnMatLac);
                    #endregion
                }
                else if (_TYpe == LoaiBenhAnEMR.MatTreEm)
                {
                    #region MatTreEm
                    BenhAnMatTreEm _BenhAnMatTreEm = new BenhAnMatTreEm();
                    Inventec.Common.Mapper.DataObjectMapper.Map<BenhAnMatTreEm>(_BenhAnMatTreEm, _BenhAnCommonADO);
                    json = Newtonsoft.Json.JsonConvert.SerializeObject(_BenhAnMatTreEm);
                    #endregion
                }
                else if (_TYpe == LoaiBenhAnEMR.NgoaiTru)
                {
                    #region NgoaiTru
                    BenhAnNgoaiTru _BenhAnNgoaiTru = new BenhAnNgoaiTru();
                    Inventec.Common.Mapper.DataObjectMapper.Map<BenhAnNgoaiTru>(_BenhAnNgoaiTru, _BenhAnCommonADO);

                    // thong tin rieng
                    _BenhAnNgoaiTru.CacBoPhan = _ExamServiceReq != null ? _ExamServiceReq.PART_EXAM : "";
                    _BenhAnNgoaiTru.ChanDoanBanDau = "";
                    _BenhAnNgoaiTru.ChanDoanKhiRaVien = "";
                    _BenhAnNgoaiTru.DaXuLy = "";
                    _BenhAnNgoaiTru.DieuTriNgoaiTru_DenNgay = new DateTime();
                    _BenhAnNgoaiTru.DieuTriNgoaiTru_TuNgay = new DateTime();
                    _BenhAnNgoaiTru.MaICD_BenhChinh = "";
                    _BenhAnNgoaiTru.MaICD_BenhKemTheo = "";
                    _BenhAnNgoaiTru.MAIDC_ChanDoanKhiRaVien = "";
                    _BenhAnNgoaiTru.TenKhoa = "";
                    _BenhAnNgoaiTru.TomTatKetQuaCanLamSang = "";

                    json = Newtonsoft.Json.JsonConvert.SerializeObject(_BenhAnNgoaiTru);
                    #endregion
                }
                else if (_TYpe == LoaiBenhAnEMR.NgoaiTruRangHamMat)
                {
                    #region NgoaiTruRangHamMat
                    BenhAnNgoaiTruRangHamMat _BenhAnNgoaiTruRangHamMat = new BenhAnNgoaiTruRangHamMat();
                    Inventec.Common.Mapper.DataObjectMapper.Map<BenhAnRangHamMat>(_BenhAnNgoaiTruRangHamMat, _BenhAnCommonADO);

                    //thong tin rieng
                    _BenhAnNgoaiTruRangHamMat.MaICD_BenhChinh = "";
                    _BenhAnNgoaiTruRangHamMat.MaICD_BenhKemTheo = "";
                    _BenhAnNgoaiTruRangHamMat.BenhChuyenKhoa = "";
                    _BenhAnNgoaiTruRangHamMat.Phai_HinhVe = "";
                    _BenhAnNgoaiTruRangHamMat.Thang_HinhVe = "";
                    _BenhAnNgoaiTruRangHamMat.HamTrenVaHong_HinhVe = "";
                    _BenhAnNgoaiTruRangHamMat.HamDuoi_HinhVe = "";
                    _BenhAnNgoaiTruRangHamMat.PhanLoai_HinhVe = "";
                    _BenhAnNgoaiTruRangHamMat.ChuanDoanCuaKhoaKhamBenh = "";
                    _BenhAnNgoaiTruRangHamMat.DaXuLyCuaTuyenDuoi = "";
                    _BenhAnNgoaiTruRangHamMat.ChuanDoanCuaKhoaKhamBenh = "";
                    _BenhAnNgoaiTruRangHamMat.DieuTriNgoaiTru_DenNgay = new DateTime();
                    _BenhAnNgoaiTruRangHamMat.DieuTriNgoaiTru_TuNgay = new DateTime();

                    json = Newtonsoft.Json.JsonConvert.SerializeObject(_BenhAnNgoaiTruRangHamMat);
                    #endregion
                }
                else if (_TYpe == LoaiBenhAnEMR.NgoaiTruTaiMuiHong)
                {
                    #region NgoaiTruTaiMuiHong
                    BenhAnNgoaiTruTaiMuiHong _BenhAnNgoaiTruTaiMuiHong = new BenhAnNgoaiTruTaiMuiHong();
                    Inventec.Common.Mapper.DataObjectMapper.Map<BenhAnNgoaiTruTaiMuiHong>(_BenhAnNgoaiTruTaiMuiHong, _BenhAnCommonADO);

                    // thong tin rieng                                                      
                    _BenhAnNgoaiTruTaiMuiHong.MaICD_BenhChinh = "";
                    _BenhAnNgoaiTruTaiMuiHong.MaICD_BenhKemTheo = "";
                    _BenhAnNgoaiTruTaiMuiHong.BenhChuyenKhoa = "";
                    _BenhAnNgoaiTruTaiMuiHong.DieuTriNgoaiTru_DenNgay = new DateTime();
                    _BenhAnNgoaiTruTaiMuiHong.DieuTriNgoaiTru_TuNgay = new DateTime();
                    _BenhAnNgoaiTruTaiMuiHong.ManNhiPhai_HinhAnh = "";
                    _BenhAnNgoaiTruTaiMuiHong.ManNhiTrai_HinhAnh = "";
                    _BenhAnNgoaiTruTaiMuiHong.MuiTruoc_HinhAnh = "";
                    _BenhAnNgoaiTruTaiMuiHong.MuiSau_HinhAnh = "";
                    _BenhAnNgoaiTruTaiMuiHong.ThanhQuan_HinhAnh = "";
                    _BenhAnNgoaiTruTaiMuiHong.Hong_HinhAnh = "";
                    _BenhAnNgoaiTruTaiMuiHong.CoNghiengPhai_HinhAnh = "";
                    _BenhAnNgoaiTruTaiMuiHong.CoNghiengTrai_HinhAnh = "";
                    _BenhAnNgoaiTruTaiMuiHong.ChuanDoanPhongKham = "";
                    _BenhAnNgoaiTruTaiMuiHong.DaXuLy = "";
                    _BenhAnNgoaiTruTaiMuiHong.TaiCho = "";
                    json = Newtonsoft.Json.JsonConvert.SerializeObject(_BenhAnNgoaiTruTaiMuiHong);
                    #endregion
                }
                else if (_TYpe == LoaiBenhAnEMR.NgoaiTruYHCT)
                {
                    #region NgoaiTruYHCT
                    BenhAnNgoaiTruYHCT _BenhAnNgoaiTruYHCT = new BenhAnNgoaiTruYHCT();
                    Inventec.Common.Mapper.DataObjectMapper.Map<BenhAnNgoaiTruYHCT>(_BenhAnNgoaiTruYHCT, _BenhAnCommonADO);

                    if (KhoaNhapVien != null)
                    {
                        _ThongTinDieuTri.ChanDoan_KhiVaoKhoaDieuTri = KhoaNhapVien.ICD_NAME;// "dt1";
                        _ThongTinDieuTri.MaICD_KhiVaoKhoaDieuTri = KhoaNhapVien.ICD_CODE;// "dt20";
                    }
                    else
                    {
                        if (departmentTranNT != null)
                        {
                            _ThongTinDieuTri.ChanDoan_KhiVaoKhoaDieuTri = departmentTranNT.ICD_NAME;
                            _ThongTinDieuTri.MaICD_KhiVaoKhoaDieuTri = departmentTranNT.ICD_CODE;
                        }
                        else
                        {
                            _ThongTinDieuTri.ChanDoan_KhiVaoKhoaDieuTri = _Treatment.TRADITIONAL_ICD_NAME;
                            _ThongTinDieuTri.MaICD_KhiVaoKhoaDieuTri = _Treatment.TRADITIONAL_ICD_CODE;
                        }
                    }

                    _ThongTinDieuTri.ChanDoanKKBYHCT = _Treatment.TRADITIONAL_ICD_NAME;
                    _ThongTinDieuTri.YHHD_BenhKemTheo = _Treatment.IN_ICD_TEXT; //"dt29";
                    _BenhAnNgoaiTruYHCT.MaICD_BenhKemTheo_YHHD = _Treatment.IN_ICD_SUB_CODE;//"16";//TODO
                    _BenhAnNgoaiTruYHCT.MaICD_BenhChinh_YHCT = _Treatment.TRADITIONAL_IN_ICD_CODE;//"28";
                    _BenhAnNgoaiTruYHCT.BenhKemTheo_YHCT = _Treatment.TRADITIONAL_IN_ICD_TEXT;//"4";
                    _BenhAnNgoaiTruYHCT.MaICD_BenhKemTheo_YHCT = _Treatment.TRADITIONAL_IN_ICD_SUB_CODE;//"31";

                    if (_Treatment.OUT_TIME.HasValue && _Treatment.OUT_TIME > 0)
                    {
                        _BenhAnNgoaiTruYHCT.BenhKemTheo_RV_YHCT = _Treatment.TRADITIONAL_ICD_TEXT;//"1";
                        _BenhAnNgoaiTruYHCT.MaICD_BenhKemTheo_RV_YHCT = _Treatment.TRADITIONAL_ICD_SUB_CODE;//"14";
                        _BenhAnNgoaiTruYHCT.MaICD_BenhChinh_RV_YHCT = _Treatment.TRADITIONAL_ICD_CODE;// "12";
                        _BenhAnNgoaiTruYHCT.ChanDoanRaVienTheoYHCT = _Treatment.TRADITIONAL_ICD_NAME;//"8";
                        _BenhAnNgoaiTruYHCT.MICDBenhChinhRVYHCT = _Treatment.TRADITIONAL_ICD_CODE;//"22";
                        _BenhAnNgoaiTruYHCT.CDBenhKemTheoRaVienTheoYHCT = _Treatment.TRADITIONAL_ICD_TEXT;//"3";
                        _BenhAnNgoaiTruYHCT.MICDbenhKemTheoRaVienYHCT = _Treatment.TRADITIONAL_ICD_SUB_CODE;//"24";
                        _BenhAnNgoaiTruYHCT.ChanDoanRaVienTheoYHHD = _Treatment.ICD_NAME;//"9";
                        _BenhAnNgoaiTruYHCT.MICDRaVienTheoYHHD = _Treatment.ICD_CODE;//"28";
                        _BenhAnNgoaiTruYHCT.CDBenhKemTheoRVYHHD = _Treatment.ICD_TEXT;//"4";
                        _BenhAnNgoaiTruYHCT.MICDbenhKemTheoRaVienYHHD = _Treatment.ICD_SUB_CODE;//"25";
                    }

                    _BenhAnNgoaiTruYHCT.ChanDoanVaoVienTheoYHCT = _Treatment.TRADITIONAL_IN_ICD_NAME;//"10";
                    _BenhAnNgoaiTruYHCT.MICDBenhChinhVVYHCT = _Treatment.TRADITIONAL_IN_ICD_CODE;//"23";
                    _BenhAnNgoaiTruYHCT.CDBenhKemtheoVVYHCT = _Treatment.TRADITIONAL_IN_ICD_TEXT;//"5";
                    _BenhAnNgoaiTruYHCT.MICDBenhKemTheoVVYHCT = _Treatment.TRADITIONAL_IN_ICD_SUB_CODE;//"26";
                    _BenhAnNgoaiTruYHCT.ChanDoanVaoVienTheoYHHD = _Treatment.IN_ICD_NAME;//"11";
                    _BenhAnNgoaiTruYHCT.MICDVaoVienTheoYHHD = _Treatment.IN_ICD_CODE;//"29";
                    _BenhAnNgoaiTruYHCT.CDBenhKemtheoVVYHHD = _Treatment.IN_ICD_TEXT;//"6";
                    _BenhAnNgoaiTruYHCT.MICDBenhKemtheoVVYHHD = _Treatment.IN_ICD_SUB_CODE;//"27";
                    _BenhAnNgoaiTruYHCT.MaICD_NoiChuyenDen = _Treatment.IN_ICD_CODE;
                    _BenhAnNgoaiTruYHCT.MaICD_NoiChuyenDen_YHCT = _Treatment.TRADITIONAL_IN_ICD_CODE;//"19";
                    _BenhAnNgoaiTruYHCT.BenhChinh = _Treatment.ICD_NAME;//"36";
                    _BenhAnNgoaiTruYHCT.BenhKemTheo = _Treatment.ICD_TEXT;//"37"; 
                    _BenhAnNgoaiTruYHCT.MICD_BenhKemTheo = _Treatment.ICD_SUB_CODE;//"21";
                    _BenhAnNgoaiTruYHCT.MICD_BenhChinh = _Treatment.ICD_CODE;//"";
                    _BenhAnNgoaiTruYHCT.MaICD_NoiChuyenDen_YHCT = _Treatment.TRADITIONAL_TRANS_IN_ICD_CODE;

                    // thong tin rieng    
                    _BenhAnNgoaiTruYHCT.ThoiGianDieuTri = (int)(_Treatment.TREATMENT_DAY_COUNT ?? 0);
                    _BenhAnNgoaiTruYHCT.TinhTrangNguoiBenhKhiRavien = _Treatment.PATIENT_CONDITION;
                    _BenhAnNgoaiTruYHCT.KetQuaDieuTriID = _ThongTinDieuTri.KetQuaDieuTri.HasValue ? (int)_ThongTinDieuTri.KetQuaDieuTri : 0;
                    _BenhAnNgoaiTruYHCT.ThoiGianDieuTriTuNgay = new DateTime();
                    _BenhAnNgoaiTruYHCT.ThoiGianDieuTriDenNgay = new DateTime();
                    _BenhAnNgoaiTruYHCT.PhuongPhapDieuTriTheoYHCT = _Treatment.TREATMENT_METHOD;

                    json = Newtonsoft.Json.JsonConvert.SerializeObject(_BenhAnNgoaiTruYHCT);
                    #endregion
                }
                else if (_TYpe == LoaiBenhAnEMR.DieuTriBanNgay)
                {
                    #region Điều trị ban ngày
                    BenhAnDieuTriBanNgay _BenhAnDieuTriBanNgay = new BenhAnDieuTriBanNgay();
                    Inventec.Common.Mapper.DataObjectMapper.Map<BenhAnDieuTriBanNgay>(_BenhAnDieuTriBanNgay, _BenhAnCommonADO);
                    _BenhAnDieuTriBanNgay.TruongKhoa = "";
                    json = Newtonsoft.Json.JsonConvert.SerializeObject(_BenhAnDieuTriBanNgay);
                    #endregion
                }
                else if (_TYpe == LoaiBenhAnEMR.BANTBanNgayYHCT)
                {
                    #region Nội trú ban ngày - YHCT
                    BenhAnNoiTruBanNgayYHCT _BenhAnNoiTruBanNgayYHCT = new BenhAnNoiTruBanNgayYHCT();
                    Inventec.Common.Mapper.DataObjectMapper.Map<BenhAnNoiTruBanNgayYHCT>(_BenhAnNoiTruBanNgayYHCT, _BenhAnCommonADO);
                    json = Newtonsoft.Json.JsonConvert.SerializeObject(_BenhAnNoiTruBanNgayYHCT);
                    #endregion
                }
                else if (_TYpe == LoaiBenhAnEMR.SanKhoa)
                {
                    #region Sản khoa
                    BenhAnSanKhoa _BenhAnSanKhoa = new BenhAnSanKhoa();
                    Inventec.Common.Mapper.DataObjectMapper.Map<BenhAnSanKhoa>(_BenhAnSanKhoa, _BenhAnCommonADO);
                    json = Newtonsoft.Json.JsonConvert.SerializeObject(_BenhAnSanKhoa);
                    #endregion
                }
                else if (_TYpe == LoaiBenhAnEMR.RangHamMat)
                {
                    #region Răng - Hàm - Mặt
                    BenhAnRangHamMat _BenhAnRangHamMat = new BenhAnRangHamMat();
                    Inventec.Common.Mapper.DataObjectMapper.Map<BenhAnRangHamMat>(_BenhAnRangHamMat, _BenhAnCommonADO);
                    json = Newtonsoft.Json.JsonConvert.SerializeObject(_BenhAnRangHamMat);
                    #endregion
                }
                else if (_TYpe == LoaiBenhAnEMR.PhuKhoa)
                {
                    #region Phụ khoa
                    BenhAnPhuKhoa _BenhAnPhuKhoa = new BenhAnPhuKhoa();
                    Inventec.Common.Mapper.DataObjectMapper.Map<BenhAnPhuKhoa>(_BenhAnPhuKhoa, _BenhAnCommonADO);
                    json = Newtonsoft.Json.JsonConvert.SerializeObject(_BenhAnPhuKhoa);
                    #endregion
                }
                else if (_TYpe == LoaiBenhAnEMR.NoiTruYHCT)
                {
                    #region NoiTruYHCT
                    BenhAnNoiTruYHCT _BenhAnNoiTruYHCT = new BenhAnNoiTruYHCT();
                    Inventec.Common.Mapper.DataObjectMapper.Map<BenhAnNoiTruYHCT>(_BenhAnNoiTruYHCT, _BenhAnCommonADO);
                    _BenhAnNoiTruYHCT.MachTayTrai_Thon1 = -1;
                    _BenhAnNoiTruYHCT.MachTayTrai_Thon2 = -1;
                    _BenhAnNoiTruYHCT.MachTayTrai_Thon3 = -1;
                    _BenhAnNoiTruYHCT.MachTayTrai_Quan1 = -1;
                    _BenhAnNoiTruYHCT.MachTayTrai_Quan2 = -1;
                    _BenhAnNoiTruYHCT.MachTayTrai_Quan3 = -1;
                    _BenhAnNoiTruYHCT.MachTayTrai_Xich1 = -1;
                    _BenhAnNoiTruYHCT.MachTayTrai_Xich2 = -1;
                    _BenhAnNoiTruYHCT.MachTayTrai_Xich3 = -1;
                    _BenhAnNoiTruYHCT.MachTayPhai_Thon1 = -1;
                    _BenhAnNoiTruYHCT.MachTayPhai_Thon2 = -1;
                    _BenhAnNoiTruYHCT.MachTayPhai_Thon3 = -1;
                    _BenhAnNoiTruYHCT.MachTayPhai_Quan1 = -1;
                    _BenhAnNoiTruYHCT.MachTayPhai_Quan2 = -1;
                    _BenhAnNoiTruYHCT.MachTayPhai_Quan3 = -1;
                    _BenhAnNoiTruYHCT.MachTayPhai_Xich1 = -1;
                    _BenhAnNoiTruYHCT.MachTayPhai_Xich2 = -1;
                    _BenhAnNoiTruYHCT.MachTayPhai_Xich3 = -1;

                    if (_Treatment.OUT_TIME.HasValue && _Treatment.OUT_TIME > 0)
                    {
                        _BenhAnNoiTruYHCT.BenhChinh_RV_YHCT = _Treatment.TRADITIONAL_ICD_NAME;//"1";
                        _BenhAnNoiTruYHCT.MaICD_BenhChinh_RV_YHCT = _Treatment.TRADITIONAL_ICD_CODE;//"19";
                        _BenhAnNoiTruYHCT.BenhKemTheo_RV_YHCT = _Treatment.TRADITIONAL_ICD_TEXT;//"3";
                        _BenhAnNoiTruYHCT.MaICD_BenhKemTheo_RV_YHCT = _Treatment.TRADITIONAL_ICD_SUB_CODE;//"22";
                        _BenhAnNoiTruYHCT.ChanDoanRVYHCT_BenhChinh = _Treatment.TRADITIONAL_ICD_NAME;//"9";
                        _BenhAnNoiTruYHCT.MaICDChanDoanRVYHCT_BenhChinh = _Treatment.TRADITIONAL_ICD_CODE;//"28";
                        _BenhAnNoiTruYHCT.ChanDoanRVYHCT_KemTheo = _Treatment.TRADITIONAL_ICD_TEXT;//"10";
                        _BenhAnNoiTruYHCT.MaICDChanDoanRVYHCT_KemTheo = _Treatment.TRADITIONAL_ICD_SUB_CODE;//"29";
                        _BenhAnNoiTruYHCT.ChanDoanRVYHD_KemTheo = _Treatment.ICD_TEXT;//"11";
                        _BenhAnNoiTruYHCT.MaICDChanDoanRVYHD_KemTheo = _Treatment.ICD_SUB_CODE;//"30";
                        _BenhAnNoiTruYHCT.ChanDoanRVYHHD_BenhChinh = _Treatment.ICD_NAME;//"12";
                        _BenhAnNoiTruYHCT.MaICDRVYHD_BenhChinh = _Treatment.ICD_CODE;//"34";
                        _BenhAnNoiTruYHCT.ChanDoanRaVienTheoYHCT = _Treatment.TRADITIONAL_ICD_NAME; //"7";
                        _BenhAnNoiTruYHCT.ChanDoanRaVienTheoYHHD = _Treatment.ICD_NAME;//"8";
                    }

                    _BenhAnNoiTruYHCT.ChanDoanVVYHCT_BenhChinh = _Treatment.TRADITIONAL_IN_ICD_NAME;//"15";
                    _BenhAnNoiTruYHCT.MaICDChanDoanVVYHCT_BenhChinh = _Treatment.TRADITIONAL_IN_ICD_CODE;//"31";
                    _BenhAnNoiTruYHCT.ChanDoanVVYHCT_KemTheo = _Treatment.TRADITIONAL_IN_ICD_TEXT;//"16";
                    _BenhAnNoiTruYHCT.MaICDChanDoanVVYHCT_KemTheo = _Treatment.TRADITIONAL_IN_ICD_SUB_CODE;//"32";
                    _BenhAnNoiTruYHCT.MaICD_BenhChinh_YHHD_CD = _Treatment.ICD_CODE;//"21";
                    _BenhAnNoiTruYHCT.MaICD_BenhKemTheo_YHHD_CD = _Treatment.ICD_SUB_CODE;//"25";            
                    _BenhAnNoiTruYHCT.ChanDoanVVYHD_KemTheo = _Treatment.IN_ICD_TEXT;//"17";
                    _BenhAnNoiTruYHCT.MaICDChanDoanVVYHD_KemTheo = _Treatment.IN_ICD_SUB_CODE;//"33";
                    _BenhAnNoiTruYHCT.MaICDVVYHD_BenhChinh = _Treatment.IN_ICD_CODE;//"35";
                    _BenhAnNoiTruYHCT.ChanDoanVVYHHD_BenhChinh = _Treatment.IN_ICD_NAME;//"18";
                    _BenhAnNoiTruYHCT.ChanDoan_NoiChuyenDen_YHCT = _Treatment.TRADITIONAL_TRANS_IN_ICD_NAME;//"6"
                    _BenhAnNoiTruYHCT.MaICD_NoiChuyenDen_YHCT = _Treatment.TRADITIONAL_TRANS_IN_ICD_CODE;//"27";

                    if (KhoaNhapVien != null)
                    {
                        _ThongTinDieuTri.ChanDoan_KhiVaoKhoaDieuTri = KhoaNhapVien.ICD_NAME;// "dt1";
                        _ThongTinDieuTri.MaICD_KhiVaoKhoaDieuTri = KhoaNhapVien.ICD_CODE;// "dt20";
                    }
                    else
                    {
                        if (departmentTranNT != null)
                        {
                            _ThongTinDieuTri.ChanDoan_KhiVaoKhoaDieuTri = departmentTranNT.ICD_NAME;
                            _ThongTinDieuTri.MaICD_KhiVaoKhoaDieuTri = departmentTranNT.ICD_CODE;
                        }
                        else
                        {
                            _ThongTinDieuTri.ChanDoan_KhiVaoKhoaDieuTri = _Treatment.TRADITIONAL_ICD_NAME;
                            _ThongTinDieuTri.MaICD_KhiVaoKhoaDieuTri = _Treatment.TRADITIONAL_ICD_CODE;
                        }
                    }

                    _ThongTinDieuTri.YHHD_BenhKemTheo = _Treatment.IN_ICD_TEXT; //"dt29";
                    _BenhAnNoiTruYHCT.MaICD_BenhKemTheo_YHHD = _Treatment.IN_ICD_SUB_CODE;//"32";
                    _BenhAnNoiTruYHCT.BenhChinh_YHCT = _Treatment.TRADITIONAL_IN_ICD_NAME ?? _Treatment.TRADITIONAL_ICD_NAME;// "2";
                    _BenhAnNoiTruYHCT.MaICD_BenhChinh_YHCT = _Treatment.TRADITIONAL_IN_ICD_CODE ?? _Treatment.TRADITIONAL_ICD_CODE;//"28";
                    _BenhAnNoiTruYHCT.BenhKemTheo_YHCT = _Treatment.TRADITIONAL_IN_ICD_TEXT ?? _Treatment.TRADITIONAL_ICD_TEXT;//"4";
                    _BenhAnNoiTruYHCT.MaICD_BenhKemTheo_YHCT = _Treatment.TRADITIONAL_IN_ICD_SUB_CODE ?? _Treatment.TRADITIONAL_ICD_SUB_CODE;//"31";
                    _BenhAnNoiTruYHCT.PhuongPhapDieuTriTheoYHCT = "";// this._Treatment.TREATMENT_METHOD;
                    _BenhAnNoiTruYHCT.PhuongPhapDieuTriTheoYHHD = _Treatment.TREATMENT_METHOD;
                    _BenhAnNoiTruYHCT.ChanDoanVaoVienTheoYHCT = _Treatment.TRADITIONAL_IN_ICD_NAME; //"13";
                    _BenhAnNoiTruYHCT.ChanDoanVaoVienTheoYHHD = _Treatment.IN_ICD_NAME; //"14";
                    _BenhAnNoiTruYHCT.ChanDoanVVYHHD_BenhChinh = _Treatment.IN_ICD_NAME;
                    _BenhAnNoiTruYHCT.ChanDoanVVYHD_KemTheo = _Treatment.IN_ICD_TEXT;

                    json = Newtonsoft.Json.JsonConvert.SerializeObject(_BenhAnNoiTruYHCT);
                    #endregion
                }
                else if (_TYpe == LoaiBenhAnEMR.NoiTruNhiYHCT)
                {
                    #region Nội trú nhi _ YHCT
                    BenhAnNoiTruNhiYHCT _BenhAnNoiTruNhiYHCT = new BenhAnNoiTruNhiYHCT();
                    Inventec.Common.Mapper.DataObjectMapper.Map<BenhAnNoiTruNhiYHCT>(_BenhAnNoiTruNhiYHCT, _BenhAnCommonADO);

                    if (KhoaNhapVien != null)
                    {
                        _ThongTinDieuTri.ChanDoan_KhiVaoKhoaDieuTri = KhoaNhapVien.ICD_NAME; ;
                        _ThongTinDieuTri.MaICD_KhiVaoKhoaDieuTri = KhoaNhapVien.ICD_CODE; ;
                    }
                    else
                    {
                        if (departmentTranNT != null)
                        {
                            _ThongTinDieuTri.ChanDoan_KhiVaoKhoaDieuTri = departmentTranNT.ICD_NAME;
                            _ThongTinDieuTri.MaICD_KhiVaoKhoaDieuTri = departmentTranNT.ICD_CODE;
                        }
                        else
                        {
                            _ThongTinDieuTri.ChanDoan_KhiVaoKhoaDieuTri = _Treatment.TRADITIONAL_ICD_NAME;
                            _ThongTinDieuTri.MaICD_KhiVaoKhoaDieuTri = _Treatment.TRADITIONAL_ICD_CODE;
                        }
                    }
                    _BenhAnNoiTruNhiYHCT.MaICD_BenhChinh_YHCT = _Treatment.TRADITIONAL_ICD_CODE;
                    _ThongTinDieuTri.YHHD_BenhKemTheo = _Treatment.IN_ICD_TEXT;
                    _BenhAnNoiTruNhiYHCT.MaICD_BenhKemTheo_YHHD = _Treatment.IN_ICD_SUB_CODE;//"32";
                    _BenhAnNoiTruNhiYHCT.BenhKemTheo_YHCT = _Treatment.TRADITIONAL_IN_ICD_TEXT ?? _Treatment.TRADITIONAL_ICD_TEXT;//"4";
                    _BenhAnNoiTruNhiYHCT.MaICD_BenhKemTheo_YHCT = _Treatment.TRADITIONAL_IN_ICD_SUB_CODE ?? _Treatment.TRADITIONAL_ICD_SUB_CODE;//"31";

                    if (_Treatment.OUT_TIME.HasValue && _Treatment.OUT_TIME > 0)
                    {
                        _BenhAnNoiTruNhiYHCT.BenhChinh_RV_YHCT = _Treatment.TRADITIONAL_ICD_NAME;//"1";
                        _BenhAnNoiTruNhiYHCT.MaICD_BenhChinh_RV_YHCT = _Treatment.TRADITIONAL_ICD_CODE;//"19";

                        _BenhAnNoiTruNhiYHCT.BenhKemTheo_RV_YHCT = _Treatment.TRADITIONAL_ICD_TEXT;//"3";
                        _BenhAnNoiTruNhiYHCT.MaICD_BenhKemTheo_RV_YHCT = _Treatment.TRADITIONAL_ICD_SUB_CODE;//"22";
                    }

                    _BenhAnNoiTruNhiYHCT.ChanDoan_NoiChuyenDen_YHCT = _Treatment.TRADITIONAL_TRANS_IN_ICD_NAME;
                    _BenhAnNoiTruNhiYHCT.MaICD_NoiChuyenDen_YHCT = _Treatment.TRADITIONAL_TRANS_IN_ICD_CODE;

                    json = Newtonsoft.Json.JsonConvert.SerializeObject(_BenhAnNoiTruNhiYHCT);
                    #endregion
                }
                else if (_TYpe == LoaiBenhAnEMR.PhucHoiChucNang)
                {
                    #region Phục hồi chức năng
                    BenhAnPhucHoiChucNang _BenhAnPhucHoiChucNang = new BenhAnPhucHoiChucNang();
                    Inventec.Common.Mapper.DataObjectMapper.Map<BenhAnPhucHoiChucNang>(_BenhAnPhucHoiChucNang, _BenhAnCommonADO);
                    json = Newtonsoft.Json.JsonConvert.SerializeObject(_BenhAnPhucHoiChucNang);
                    #endregion
                }
                else if (_TYpe == LoaiBenhAnEMR.TruyenNhiem)
                {
                    #region Truyền nhiễm
                    BenhAnTruyenNhiem _BenhAnTruyenNhiem = new BenhAnTruyenNhiem();
                    Inventec.Common.Mapper.DataObjectMapper.Map<BenhAnTruyenNhiem>(_BenhAnTruyenNhiem, _BenhAnCommonADO);
                    _BenhAnTruyenNhiem.BenhCapTinhDangLuuHanh = "";
                    _BenhAnTruyenNhiem.DaNoiONoiNao = "";
                    _BenhAnTruyenNhiem.DichTe = "";
                    _BenhAnTruyenNhiem.MoiSinh = "";
                    json = Newtonsoft.Json.JsonConvert.SerializeObject(_BenhAnTruyenNhiem);
                    #endregion
                }
                else if (_TYpe == LoaiBenhAnEMR.TaiMuiHong)
                {
                    #region Tai - Mũi - Họng
                    BenhAnTaiMuiHong _BenhAnTaiMuiHong = new BenhAnTaiMuiHong();
                    Inventec.Common.Mapper.DataObjectMapper.Map<BenhAnTaiMuiHong>(_BenhAnTaiMuiHong, _BenhAnCommonADO);
                    _BenhAnTaiMuiHong.BenhChuyenKhoa = "";
                    _BenhAnTaiMuiHong.CoNghiengPhai_HinhAnh = "";
                    _BenhAnTaiMuiHong.CoNghiengTrai_HinhAnh = "";
                    _BenhAnTaiMuiHong.DaVaMoDuoiDa = "";
                    _BenhAnTaiMuiHong.Hong_HinhAnh = "";
                    _BenhAnTaiMuiHong.ManNhiPhai_HinhAnh = "";
                    _BenhAnTaiMuiHong.ManNhiTrai_HinhAnh = "";
                    _BenhAnTaiMuiHong.MuiSau_HinhAnh = "";
                    _BenhAnTaiMuiHong.MuiTruoc_HinhAnh = "";
                    _BenhAnTaiMuiHong.ThanhQuan_HinhAnh = "";
                    json = Newtonsoft.Json.JsonConvert.SerializeObject(_BenhAnTaiMuiHong);
                    #endregion
                }
                else if (_TYpe == LoaiBenhAnEMR.NhiKhoa)
                {
                    #region Nhi khoa
                    BenhAnNhiKhoa _BenhAnNhiKhoa = new BenhAnNhiKhoa();
                    Inventec.Common.Mapper.DataObjectMapper.Map<BenhAnNhiKhoa>(_BenhAnNhiKhoa, _BenhAnCommonADO);
                    json = Newtonsoft.Json.JsonConvert.SerializeObject(_BenhAnNhiKhoa);
                    #endregion
                }
                else if (_TYpe == LoaiBenhAnEMR.Tim)
                {
                    #region Tim
                    BenhAnTim _BenhAnTim = new BenhAnTim();
                    Inventec.Common.Mapper.DataObjectMapper.Map<BenhAnTim>(_BenhAnTim, _BenhAnCommonADO);
                    json = Newtonsoft.Json.JsonConvert.SerializeObject(_BenhAnTim);
                    #endregion
                }
                else if (_TYpe == LoaiBenhAnEMR.LuuCapCuu)
                {
                    #region Lưu cấp cứu
                    BenhAnLuuCapCuu _BenhAnLuuCapCuu = new BenhAnLuuCapCuu();
                    Inventec.Common.Mapper.DataObjectMapper.Map<BenhAnLuuCapCuu>(_BenhAnLuuCapCuu, _BenhAnCommonADO);
                    json = Newtonsoft.Json.JsonConvert.SerializeObject(_BenhAnLuuCapCuu);
                    #endregion
                }
                else if (_TYpe == LoaiBenhAnEMR.Mat)
                {
                    #region Mắt
                    BenhAnMat _BenhAnMat = new BenhAnMat();
                    Inventec.Common.Mapper.DataObjectMapper.Map<BenhAnMat>(_BenhAnMat, _BenhAnCommonADO);
                    json = Newtonsoft.Json.JsonConvert.SerializeObject(_BenhAnMat);
                    #endregion
                }
                else if (_TYpe == LoaiBenhAnEMR.PhaThaiI)
                {
                    #region Phá thai I
                    BenhAnPhaThaiI _BenhAnPhaThaiI = new BenhAnPhaThaiI();
                    Inventec.Common.Mapper.DataObjectMapper.Map<BenhAnPhaThaiI>(_BenhAnPhaThaiI, _BenhAnCommonADO);
                    json = Newtonsoft.Json.JsonConvert.SerializeObject(_BenhAnPhaThaiI);
                    #endregion
                }
                else if (_TYpe == LoaiBenhAnEMR.PhaThaiII)
                {
                    #region Phá thai II
                    BenhAnPhaThaiII _BenhAnPhaThaiII = new BenhAnPhaThaiII();
                    Inventec.Common.Mapper.DataObjectMapper.Map<BenhAnPhaThaiII>(_BenhAnPhaThaiII, _BenhAnCommonADO);
                    json = Newtonsoft.Json.JsonConvert.SerializeObject(_BenhAnPhaThaiII);
                    #endregion
                }
                else if (_TYpe == LoaiBenhAnEMR.PhaThaiIII)
                {
                    #region Phá thai III
                    BenhAnPhaThaiIII _BenhAnPhaThaiIII = new BenhAnPhaThaiIII();
                    Inventec.Common.Mapper.DataObjectMapper.Map<BenhAnPhaThaiIII>(_BenhAnPhaThaiIII, _BenhAnCommonADO);
                    json = Newtonsoft.Json.JsonConvert.SerializeObject(_BenhAnPhaThaiIII);
                    #endregion
                }
                else if (_TYpe == LoaiBenhAnEMR.PhucHoiChucNangYHCT)
                {
                    #region YHCT-Phục hồi chức năng
                    BenhAnPhucHoiChucNangYHCT _BenhAnPhucHoiChucNangYHCT = new BenhAnPhucHoiChucNangYHCT();
                    Inventec.Common.Mapper.DataObjectMapper.Map<BenhAnPhucHoiChucNangYHCT>(_BenhAnPhucHoiChucNangYHCT, _BenhAnCommonADO);
                    json = Newtonsoft.Json.JsonConvert.SerializeObject(_BenhAnPhucHoiChucNangYHCT);
                    #endregion
                }
                else if (_TYpe == LoaiBenhAnEMR.ThanNhanTao)
                {
                    #region Thận nhân tạo
                    BenhAnThanNhanTao _BenhAnThanNhanTao = new BenhAnThanNhanTao();
                    Inventec.Common.Mapper.DataObjectMapper.Map<BenhAnThanNhanTao>(_BenhAnThanNhanTao, _BenhAnCommonADO);
                    json = Newtonsoft.Json.JsonConvert.SerializeObject(_BenhAnThanNhanTao);
                    #endregion
                }
                else if (_TYpe == LoaiBenhAnEMR.CMU)
                {
                    #region CMU
                    BenhAnCMU _BenhAnCMU = new BenhAnCMU();
                    Inventec.Common.Mapper.DataObjectMapper.Map<BenhAnCMU>(_BenhAnCMU, _BenhAnCommonADO);
                    json = Newtonsoft.Json.JsonConvert.SerializeObject(_BenhAnCMU);
                    #endregion
                }
                else if (_TYpe == LoaiBenhAnEMR.BANgoaTruPK)
                {
                    #region Bệnh án ngoại trú phòng khám
                    BANgoaiTruPK _BANgoaiTruPK = new BANgoaiTruPK();
                    Inventec.Common.Mapper.DataObjectMapper.Map<BANgoaiTruPK>(_BANgoaiTruPK, _BenhAnCommonADO);
                    json = Newtonsoft.Json.JsonConvert.SerializeObject(_BANgoaiTruPK);
                    #endregion
                }
                else if (_TYpe == LoaiBenhAnEMR.TayChanMieng)
                {
                    #region Tay Chân Miệng
                    BenhAnTayChanMieng _BenhAnTayChanMieng = new BenhAnTayChanMieng();
                    Inventec.Common.Mapper.DataObjectMapper.Map<BenhAnTayChanMieng>(_BenhAnTayChanMieng, _BenhAnCommonADO);
                    json = Newtonsoft.Json.JsonConvert.SerializeObject(_BenhAnTayChanMieng);
                    #endregion
                }
                else if (_TYpe == LoaiBenhAnEMR.NgoaiTruMat)
                {
                    #region Ngoại trú mắt
                    BenhAnNgoaiTruMat _BenhAnNgoaiTruMat = new BenhAnNgoaiTruMat();
                    Inventec.Common.Mapper.DataObjectMapper.Map<BenhAnNgoaiTruMat>(_BenhAnNgoaiTruMat, _BenhAnCommonADO);
                    json = Newtonsoft.Json.JsonConvert.SerializeObject(_BenhAnNgoaiTruMat);
                    #endregion
                }
                else if (_TYpe == LoaiBenhAnEMR.NgoaiTruPhucHoiChucNang)
                {
                    #region Ngoại Trú Phục Hồi Chức Năng
                    BenhAnNgoaiTruPHCN _BenhAnNgoaiTruPHCN = new BenhAnNgoaiTruPHCN();
                    Inventec.Common.Mapper.DataObjectMapper.Map<BenhAnNgoaiTruPHCN>(_BenhAnNgoaiTruPHCN, _BenhAnCommonADO);
                    json = Newtonsoft.Json.JsonConvert.SerializeObject(_BenhAnNgoaiTruPHCN);
                    #endregion
                }
                else if (_TYpe == LoaiBenhAnEMR.DaLieuTW)
                {
                    #region Da liễu trung ương
                    BenhAnDaLieuTW _BenhAnDaLieuTW = new BenhAnDaLieuTW();
                    Inventec.Common.Mapper.DataObjectMapper.Map<BenhAnDaLieuTW>(_BenhAnDaLieuTW, _BenhAnCommonADO);
                    if (_Treatment != null)
                    {
                        _BenhAnDaLieuTW.MaCSKCB = _Treatment.TRANSFER_IN_MEDI_ORG_CODE;
                        _BenhAnDaLieuTW.CSKCB = _Treatment.TRANSFER_IN_MEDI_ORG_NAME;
                        _BenhAnDaLieuTW.BacSyChuyenVaoVien = _Treatment.IN_LOGINNAME;
                    }
                    json = Newtonsoft.Json.JsonConvert.SerializeObject(_BenhAnDaLieuTW);
                    #endregion
                }
                else if (_TYpe == LoaiBenhAnEMR.PhucHoiChucNangNhi)
                {
                    #region Phục Hồi Chức Năng Nhi
                    BenhAnPHCNNhi _BenhAnPHCNNhi = new BenhAnPHCNNhi();
                    Inventec.Common.Mapper.DataObjectMapper.Map<BenhAnPHCNNhi>(_BenhAnPHCNNhi, _BenhAnCommonADO);
                    json = Newtonsoft.Json.JsonConvert.SerializeObject(_BenhAnPHCNNhi);
                    #endregion
                }
                else if (_TYpe == LoaiBenhAnEMR.PHCNII)
                {
                    #region Phục hồi chức năng
                    BenhAnPHCNII _BenhAnPHCNII = new BenhAnPHCNII();
                    Inventec.Common.Mapper.DataObjectMapper.Map<BenhAnPHCNII>(_BenhAnPHCNII, _BenhAnCommonADO);
                    json = Newtonsoft.Json.JsonConvert.SerializeObject(_BenhAnPHCNII);
                    #endregion
                }
                else if (_TYpe == LoaiBenhAnEMR.TruyenNhiemII)
                {
                    #region truyền nhiềm II
                    BenhAnTruyenNhiemII _BenhAnTruyenNhiemII = new BenhAnTruyenNhiemII();
                    Inventec.Common.Mapper.DataObjectMapper.Map<BenhAnTruyenNhiemII>(_BenhAnTruyenNhiemII, _BenhAnCommonADO);
                    json = Newtonsoft.Json.JsonConvert.SerializeObject(_BenhAnTruyenNhiemII);
                    #endregion
                }
                else if (_TYpe == LoaiBenhAnEMR.NgoaiTru_VayNenThuongThuong)
                {
                    #region bệnh án vẩy nến thông thường
                    BenhAnNgoaiTru_BenhVayNenThongThuong _BenhAnNgoaiTru_BenhVayNenThongThuong = new BenhAnNgoaiTru_BenhVayNenThongThuong();
                    Inventec.Common.Mapper.DataObjectMapper.Map<BenhAnNgoaiTru_BenhVayNenThongThuong>(_BenhAnNgoaiTru_BenhVayNenThongThuong, _BenhAnCommonADO);
                    json = Newtonsoft.Json.JsonConvert.SerializeObject(_BenhAnNgoaiTru_BenhVayNenThongThuong);
                    #endregion
                }
                else if (_TYpe == LoaiBenhAnEMR.NgoaiTru_AVayNen)
                {
                    #region bệnh án ngoại trú á vảy nến
                    BenhAnNgoaiTruAVayNen _BenhAnNgoaiTruAVayNen = new BenhAnNgoaiTruAVayNen();
                    Inventec.Common.Mapper.DataObjectMapper.Map<BenhAnNgoaiTruAVayNen>(_BenhAnNgoaiTruAVayNen, _BenhAnCommonADO);
                    json = Newtonsoft.Json.JsonConvert.SerializeObject(_BenhAnNgoaiTruAVayNen);
                    #endregion
                }
                else if (_TYpe == LoaiBenhAnEMR.NgoaiTru_HoTroSinhSan)
                {
                    #region bệnh án ngoại trú trung tâm hỗ trợ sinh sản
                    BenhAnNgoaiTru_HoTroSinhSan _BenhAnNgoaiTru_HoTroSinhSan = new BenhAnNgoaiTru_HoTroSinhSan();
                    Inventec.Common.Mapper.DataObjectMapper.Map<BenhAnNgoaiTru_HoTroSinhSan>(_BenhAnNgoaiTru_HoTroSinhSan, _BenhAnCommonADO);
                    json = Newtonsoft.Json.JsonConvert.SerializeObject(_BenhAnNgoaiTru_HoTroSinhSan);
                    #endregion
                }
                else if (_TYpe == LoaiBenhAnEMR.NgoaiTru_PEMPHIGOID)
                {
                    #region bệnh án ngoại trú PEMPHIGOID
                    BenhAnNgoaiTruPemphigoid _BenhAnNgoaiTruPemphigoid = new BenhAnNgoaiTruPemphigoid();
                    Inventec.Common.Mapper.DataObjectMapper.Map<BenhAnNgoaiTruPemphigoid>(_BenhAnNgoaiTruPemphigoid, _BenhAnCommonADO);
                    #endregion
                }
                else if (_TYpe == LoaiBenhAnEMR.NgoaiTru_VayPhanDoNangLong)
                {
                    #region bệnh án ngoại trú bệnh vảy phấn đỏ nang lông
                    BenhAnNgoaiTru_VayPhanDoNangLong _BenhAnNgoaiTru_VayPhanDoNangLong = new BenhAnNgoaiTru_VayPhanDoNangLong();
                    Inventec.Common.Mapper.DataObjectMapper.Map<BenhAnNgoaiTru_VayPhanDoNangLong>(_BenhAnNgoaiTru_VayPhanDoNangLong, _BenhAnCommonADO);
                    json = Newtonsoft.Json.JsonConvert.SerializeObject(_BenhAnNgoaiTru_VayPhanDoNangLong);
                    #endregion
                }
                else if (_TYpe == LoaiBenhAnEMR.NgoaiTru_LuPusBanDoManTinh)
                {
                    #region bệnh án ngoại trú lupus ban đỏ mạn tính
                    BenhAnNgoaiTru_LuPusBanDoManTinh _BenhAnNgoaiTru_LuPusBanDoManTinh = new BenhAnNgoaiTru_LuPusBanDoManTinh();
                    Inventec.Common.Mapper.DataObjectMapper.Map<BenhAnNgoaiTru_LuPusBanDoManTinh>(_BenhAnNgoaiTru_LuPusBanDoManTinh, _BenhAnCommonADO);
                    json = Newtonsoft.Json.JsonConvert.SerializeObject(_BenhAnNgoaiTru_LuPusBanDoManTinh);
                    #endregion
                }
                else if (_TYpe == LoaiBenhAnEMR.NgoaiTru_BenhAnLupusBanDoHeThong)
                {
                    #region điều trị ngoại trú bệnh lupus ban đỏ hệ thống
                    BenhAnNgoaiTru_LupusBanDoHeThong _BenhAnNgoaiTru_LupusBanDoHeThong = new BenhAnNgoaiTru_LupusBanDoHeThong();
                    Inventec.Common.Mapper.DataObjectMapper.Map<BenhAnNgoaiTru_LupusBanDoHeThong>(_BenhAnNgoaiTru_LupusBanDoHeThong, _BenhAnCommonADO);
                    json = Newtonsoft.Json.JsonConvert.SerializeObject(_BenhAnNgoaiTru_LupusBanDoHeThong);
                    #endregion
                }
                else if (_TYpe == LoaiBenhAnEMR.NgoaiTru_BenhAnViemBiCo)
                {
                    #region điều trị ngoại trú bệnh viêm bì cơ
                    BenhAnNgoaiTru_ViemBiCo _BenhAnNgoaiTru_ViemBiCo = new BenhAnNgoaiTru_ViemBiCo();
                    Inventec.Common.Mapper.DataObjectMapper.Map<BenhAnNgoaiTru_ViemBiCo>(_BenhAnNgoaiTru_ViemBiCo, _BenhAnCommonADO);
                    json = Newtonsoft.Json.JsonConvert.SerializeObject(_BenhAnNgoaiTru_ViemBiCo);
                    #endregion
                }
                else if (_TYpe == LoaiBenhAnEMR.NgoaiTru_VayNenTheMu)
                {
                    #region ngoại trú bệnh vảy nến thể mủ
                    BenhAnNgoaiTru_BenhVayNenTheMu _BenhAnNgoaiTru_BenhVayNenTheMu = new BenhAnNgoaiTru_BenhVayNenTheMu();
                    Inventec.Common.Mapper.DataObjectMapper.Map<BenhAnNgoaiTru_BenhVayNenTheMu>(_BenhAnNgoaiTru_BenhVayNenTheMu, _BenhAnCommonADO);
                    json = Newtonsoft.Json.JsonConvert.SerializeObject(_BenhAnNgoaiTru_BenhVayNenTheMu);
                    #endregion
                }
                else if (_TYpe == LoaiBenhAnEMR.NgoaiTru_DuhringBrocq)
                {
                    #region điều trị ngoại trú bệnh duhring brocq
                    BenhAnNgoaiTruDuhringBrocq _BenhAnNgoaiTruDuhringBrocq = new BenhAnNgoaiTruDuhringBrocq();
                    Inventec.Common.Mapper.DataObjectMapper.Map<BenhAnNgoaiTruDuhringBrocq>(_BenhAnNgoaiTruDuhringBrocq, _BenhAnCommonADO);
                    json = Newtonsoft.Json.JsonConvert.SerializeObject(_BenhAnNgoaiTruDuhringBrocq);
                    #endregion
                }
                else if (_TYpe == LoaiBenhAnEMR.DaiThaoDuong)
                {
                    #region đái tháo đường
                    BenhAnDaiThaoDuong _BenhAnDaiThaoDuong = new BenhAnDaiThaoDuong();
                    Inventec.Common.Mapper.DataObjectMapper.Map<BenhAnDaiThaoDuong>(_BenhAnDaiThaoDuong, _BenhAnCommonADO);
                    json = Newtonsoft.Json.JsonConvert.SerializeObject(_BenhAnDaiThaoDuong);
                    #endregion
                }
                else if (_TYpe == LoaiBenhAnEMR.NgoaiTru_UngThuDaHacTo)
                {
                    #region điều trị ngoại trú bệnh ung thư hắc tố
                    BenhAnUngThuHacTo _BenhAnUngThuHacTo = new BenhAnUngThuHacTo();
                    Inventec.Common.Mapper.DataObjectMapper.Map<BenhAnUngThuHacTo>(_BenhAnUngThuHacTo, _BenhAnCommonADO);
                    json = Newtonsoft.Json.JsonConvert.SerializeObject(_BenhAnUngThuHacTo);
                    #endregion
                }
                else if (_TYpe == LoaiBenhAnEMR.NgoaiTru_UngThuDaKhongHacTo)
                {
                    #region điều trị ngoại trú bệnh ung thư không hắc tố
                    BenhAnUngThuKhongHacTo _BenhAnUngThuKhongHacTo = new BenhAnUngThuKhongHacTo();
                    Inventec.Common.Mapper.DataObjectMapper.Map<BenhAnUngThuKhongHacTo>(_BenhAnUngThuKhongHacTo, _BenhAnCommonADO);
                    json = Newtonsoft.Json.JsonConvert.SerializeObject(_BenhAnUngThuKhongHacTo);
                    #endregion
                }
                else if (_TYpe == LoaiBenhAnEMR.NgoaiTru_Pemphigus)
                {
                    #region điều trị ngoại trú bệnh pemphigus
                    BenhAnNgoaiTruPemphigus _BenhAnNgoaiTruPemphigus = new BenhAnNgoaiTruPemphigus();
                    Inventec.Common.Mapper.DataObjectMapper.Map<BenhAnNgoaiTruPemphigus>(_BenhAnNgoaiTruPemphigus, _BenhAnCommonADO);
                    json = Newtonsoft.Json.JsonConvert.SerializeObject(_BenhAnNgoaiTruPemphigus);
                    #endregion
                }
                else if (_TYpe == LoaiBenhAnEMR.NgoaiTru_VayNenTheKhop)
                {
                    #region điều trị ngoại trú bệnh vảy nến thể khớp
                    BenhAnVayNenTheKhop _BenhAnVayNenTheKhop = new BenhAnVayNenTheKhop();
                    Inventec.Common.Mapper.DataObjectMapper.Map<BenhAnVayNenTheKhop>(_BenhAnVayNenTheKhop, _BenhAnCommonADO);
                    json = Newtonsoft.Json.JsonConvert.SerializeObject(_BenhAnVayNenTheKhop);
                    #endregion
                }
                else if (_TYpe == LoaiBenhAnEMR.NgoaiTru_HoiChungTrungLap)
                {
                    #region điều trị ngoại trú bệnh hội chứng trùng lắp
                    BenhAnNgoaiTru_HoiChungTrungLap _BenhAnNgoaiTru_HoiChungTrungLap = new BenhAnNgoaiTru_HoiChungTrungLap();
                    Inventec.Common.Mapper.DataObjectMapper.Map<BenhAnNgoaiTru_HoiChungTrungLap>(_BenhAnNgoaiTru_HoiChungTrungLap, _BenhAnCommonADO);
                    json = Newtonsoft.Json.JsonConvert.SerializeObject(_BenhAnNgoaiTru_HoiChungTrungLap);
                    #endregion
                }
                else if (_TYpe == LoaiBenhAnEMR.StentDongMachVanh)
                {
                    #region Theo dõi và điều trị có kiểm soát stent động mạnh vành
                    BenhAnStentDongMachVanh _BenhAnStentDongMachVanh = new BenhAnStentDongMachVanh();
                    Inventec.Common.Mapper.DataObjectMapper.Map<BenhAnStentDongMachVanh>(_BenhAnStentDongMachVanh, _BenhAnCommonADO);
                    json = Newtonsoft.Json.JsonConvert.SerializeObject(_BenhAnStentDongMachVanh);
                    #endregion
                }
                else if (_TYpe == LoaiBenhAnEMR.ThieuMauCoTimCucBo)
                {
                    #region Theo dõi quản lý điều trị có kiểm soát thiếu máu cơ tim cục bộ
                    BenhAnThieuMauCoTimCucBo _BenhAnThieuMauCoTimCucBo = new BenhAnThieuMauCoTimCucBo();
                    Inventec.Common.Mapper.DataObjectMapper.Map<BenhAnThieuMauCoTimCucBo>(_BenhAnThieuMauCoTimCucBo, _BenhAnCommonADO);
                    json = Newtonsoft.Json.JsonConvert.SerializeObject(_BenhAnThieuMauCoTimCucBo);
                    #endregion
                }
                else if (_TYpe == LoaiBenhAnEMR.NgoaiTru_BenhBasedow)
                {
                    #region Theo dõi và điều trị bệnh Basedow
                    BenhAnBenhBaseDow _BenhAnBenhBaseDow = new BenhAnBenhBaseDow();
                    Inventec.Common.Mapper.DataObjectMapper.Map<BenhAnBenhBaseDow>(_BenhAnBenhBaseDow, _BenhAnCommonADO);
                    json = Newtonsoft.Json.JsonConvert.SerializeObject(_BenhAnBenhBaseDow);
                    #endregion
                }
                else if (_TYpe == LoaiBenhAnEMR.ViemGanBManTinh)
                {
                    #region Theo dõi điều trị bệnh viêm gan siêu vi B
                    BenhAnViemGanBManTinh _BenhAnViemGanBManTinh = new BenhAnViemGanBManTinh();
                    Inventec.Common.Mapper.DataObjectMapper.Map<BenhAnViemGanBManTinh>(_BenhAnViemGanBManTinh, _BenhAnCommonADO);
                    json = Newtonsoft.Json.JsonConvert.SerializeObject(_BenhAnViemGanBManTinh);
                    #endregion
                }
                else if (_TYpe == LoaiBenhAnEMR.PhoiTacNghenManTinh)
                {
                    #region Theo dõi quản lý bệnh phổi tắc nghẽn mãn tính
                    BenhAnPhoiTacNghenManTinh _BenhAnPhoiTacNghenManTinh = new BenhAnPhoiTacNghenManTinh();
                    Inventec.Common.Mapper.DataObjectMapper.Map<BenhAnPhoiTacNghenManTinh>(_BenhAnPhoiTacNghenManTinh, _BenhAnCommonADO);
                    json = Newtonsoft.Json.JsonConvert.SerializeObject(_BenhAnPhoiTacNghenManTinh);
                    #endregion
                }
                else if (_TYpe == LoaiBenhAnEMR.BenhTangHuyetAp)
                {
                    #region Tăng huyết áp
                    BenhAnTangHuyetAp _BenhAnTangHuyetAp = new BenhAnTangHuyetAp();
                    Inventec.Common.Mapper.DataObjectMapper.Map<BenhAnTangHuyetAp>(_BenhAnTangHuyetAp, _BenhAnCommonADO);
                    json = Newtonsoft.Json.JsonConvert.SerializeObject(_BenhAnTangHuyetAp);
                    #endregion
                }
                else if (_TYpe == LoaiBenhAnEMR.BenhAnNgoaiTruHIV)
                {
                    #region Ngoại trú _ HIV
                    BenhAnNgoaiTruHIV _BenhAnNgoaiTruHIV = new BenhAnNgoaiTruHIV();
                    Inventec.Common.Mapper.DataObjectMapper.Map<BenhAnNgoaiTruHIV>(_BenhAnNgoaiTruHIV, _BenhAnCommonADO);
                    json = Newtonsoft.Json.JsonConvert.SerializeObject(_BenhAnNgoaiTruHIV);
                    #endregion
                }
                else if (_TYpe == LoaiBenhAnEMR.BenhAnHenPheQuan)
                {
                    #region Ngoại trú _ Hen phế quản
                    BenhAnHenPheQuan _BenhAnHenPheQuan = new BenhAnHenPheQuan();
                    Inventec.Common.Mapper.DataObjectMapper.Map<BenhAnHenPheQuan>(_BenhAnHenPheQuan, _BenhAnCommonADO);
                    json = Newtonsoft.Json.JsonConvert.SerializeObject(_BenhAnHenPheQuan);
                    #endregion
                }
                else if (_TYpe == LoaiBenhAnEMR.BenhAnSuyTim)
                {
                    #region Hồ sơ bệnh án suy tim
                    BenhAnSuyTim _BenhAnSuyTim = new BenhAnSuyTim();
                    Inventec.Common.Mapper.DataObjectMapper.Map<BenhAnSuyTim>(_BenhAnSuyTim, _BenhAnCommonADO);
                    json = Newtonsoft.Json.JsonConvert.SerializeObject(_BenhAnSuyTim);
                    #endregion
                }
                else if (_TYpe == LoaiBenhAnEMR.NgoaiTru_XoCungBiHeThong)
                {
                    #region bệnh án Xơ cứng bì hệ thống
                    BenhAnNgoaiTru_BenhXoCungBiHeThong _BenhXoCungBiHeThong = new BenhAnNgoaiTru_BenhXoCungBiHeThong();
                    Inventec.Common.Mapper.DataObjectMapper.Map<BenhAnNgoaiTru_BenhXoCungBiHeThong>(_BenhXoCungBiHeThong, _BenhAnCommonADO);
                    json = Newtonsoft.Json.JsonConvert.SerializeObject(_BenhXoCungBiHeThong);
                    #endregion
                }
                else if (_TYpe == LoaiBenhAnEMR.NgoaiTru_XoCungBiKhuTru)
                {
                    #region bệnh án Xơ cứng bì khu trú
                    BenhAnNgoaiTru_BenhXoCungBiKhuTru _BenhXoCungBiKhuTru = new BenhAnNgoaiTru_BenhXoCungBiKhuTru();
                    Inventec.Common.Mapper.DataObjectMapper.Map<BenhAnNgoaiTru_BenhXoCungBiKhuTru>(_BenhXoCungBiKhuTru, _BenhAnCommonADO);
                    json = Newtonsoft.Json.JsonConvert.SerializeObject(_BenhXoCungBiKhuTru);
                    #endregion
                }
                else if (_TYpe == LoaiBenhAnEMR.NgoaiTru_DaLieuTW)
                {
                    #region bệnh án ngoại trú phẫu thuật
                    BenhAnNgoaiTruDaLieuTW _BenhAnNgoaiTru_DaLieuTW = new BenhAnNgoaiTruDaLieuTW();
                    Inventec.Common.Mapper.DataObjectMapper.Map<BenhAnNgoaiTruDaLieuTW>(_BenhAnNgoaiTru_DaLieuTW, _BenhAnCommonADO);
                    json = Newtonsoft.Json.JsonConvert.SerializeObject(_BenhAnNgoaiTru_DaLieuTW);
                    #endregion
                }
                else if (_TYpe == LoaiBenhAnEMR.NoiKhoaThan)
                {
                    #region bệnh án nội khoa thận
                    BenhAnNoiKhoaThan _BenhAnNoiKhoaThan = new BenhAnNoiKhoaThan();
                    Inventec.Common.Mapper.DataObjectMapper.Map<BenhAnNoiKhoaThan>(_BenhAnNoiKhoaThan, _BenhAnCommonADO);
                    json = Newtonsoft.Json.JsonConvert.SerializeObject(_BenhAnNoiKhoaThan);
                    #endregion
                }
                else if (_TYpe == LoaiBenhAnEMR.SoSinh)
                {
                    #region bệnh án sơ sinh
                    BenhAnSoSinh _BenhAnSoSinh = new BenhAnSoSinh();
                    Inventec.Common.Mapper.DataObjectMapper.Map<BenhAnSoSinh>(_BenhAnSoSinh, _BenhAnCommonADO);
                    json = Newtonsoft.Json.JsonConvert.SerializeObject(_BenhAnSoSinh);
                    #endregion
                }
                else if (_TYpe == LoaiBenhAnEMR.CapCuu)
                {
                    #region bệnh án cấp cứu
                    BenhAnCapCuu _BenhAnCapCuu = new BenhAnCapCuu();
                    Inventec.Common.Mapper.DataObjectMapper.Map<BenhAnCapCuu>(_BenhAnCapCuu, _BenhAnCommonADO);
                    json = Newtonsoft.Json.JsonConvert.SerializeObject(_BenhAnCapCuu);
                    #endregion
                }
                else if (_TYpe == LoaiBenhAnEMR.NgoaiTru_TNT)
                {
                    #region bệnh án NGOẠI TRÚ THẬN NHÂN TẠO
                    BenhAnNgoaiTru_TNT _BenhAnNgoaiTru_TNT = new BenhAnNgoaiTru_TNT();
                    Inventec.Common.Mapper.DataObjectMapper.Map<BenhAnNgoaiTru_TNT>(_BenhAnNgoaiTru_TNT, _BenhAnCommonADO);
                    json = Newtonsoft.Json.JsonConvert.SerializeObject(_BenhAnNgoaiTru_TNT);
                    #endregion
                }
                else if (_TYpe > 0)
                {
                    json = Newtonsoft.Json.JsonConvert.SerializeObject(_BenhAnCommonADO);
                }
                #endregion

                LogSystem.Debug("LoadDataEmr. 4");
                #region Call Show ERM.Dll
                EMR_ADO _ERMADO = new EMR_ADO();
                _ERMADO.KyDienTu_ApplicationCode = GlobalVariables.APPLICATION_CODE;
                _ERMADO.KyDienTu_DiaChiACS = HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_ACS;
                _ERMADO.KyDienTu_DiaChiEMR = HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_EMR;
                _ERMADO.KyDienTu_DiaChiThuVienKy = HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_FSS;
                _ERMADO.KyDienTu_DiaChiMOS = HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_MOS;
                _ERMADO.KyDienTu_TREATMENT_CODE = _Treatment.TREATMENT_CODE;
                _ERMADO.TreatmentId = _Treatment.ID;
                _ERMADO.UserCodeLogin = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                _ERMADO.MaPhieu = _MaPhieu;
                _ERMADO.TuyChonCanhBaoVanBanDaKy = TuyChonCanhBaoVanBanDaKy;
                _ERMADO.IdPhong = ado.roomId;
                _ERMADO.IdLoaiPhong = RoomTypeId;
                _ERMADO.MaPhong = RoomCode;
                _ERMADO.MaLoaiPhong = RoomTypeCode;
                _ERMADO.SoLuuTru = SoLuuTru;
                _ERMADO.MaYTe = MaYTe;

                Inventec.Token.Core.TokenData token = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetTokenData();
                if (token != null)
                {
                    _ERMADO.KyDienTu_TokenCode = token.TokenCode;
                }

                _ERMADO._HanhChinhBenhNhan_s = new HanhChinhBenhNhan();
                _ERMADO._ThongTinDieuTri_s = new ThongTinDieuTri();
                _ERMADO._LoaiBenhAnEMR_s = new LoaiBenhAnEMR();
                if (_TYpe != 0)
                {
                    _ERMADO._LoaiBenhAnEMR_s = _TYpe;
                }

                // gán thông tin hành chính
                _ERMADO._HanhChinhBenhNhan_s = _HanhChinhBenhNhan;
                _ERMADO._ThongTinDieuTri_s = _ThongTinDieuTri;
                _ERMADO.jsonbenhan = json;
                _ERMADO._DauHieuSinhTonMoi_s = new DauSinhTon();
                _ERMADO._DauHieuSinhTonMoi_s = _DauHieuSinhTonMoi;
                _ERMADO._KhangSinh_s = KhangSinh_HISs;
                _ERMADO.IsDongBenhAn_s = _Treatment.IS_PAUSE == 1 ? true : false;

                string cmdLn = EncodeData(_ERMADO);
                LogSystem.Debug("LoadDataEmr. end");
                System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
                startInfo.FileName = Application.StartupPath + @"\Integrate\\EMR\\ConnectToEMR.exe";
                startInfo.Arguments = cmdLn;

                Thread threadOpen = new Thread(() =>
                    {
                        System.Diagnostics.Process.Start(startInfo);
                    });
                //cập nhật EMR_COVER_TYPE_ID                
                threadOpen.Start();

                if (ado.EmrCoverTypeId != (long)_TYpe && _TYpe > 0)
                {
                    CommonParam param1 = new CommonParam();
                    HisTreatmentEmrCoverSDO TreatmentEmrCover = new HisTreatmentEmrCoverSDO();
                    TreatmentEmrCover.TreatmentId = ado.TreatmentId;
                    TreatmentEmrCover.EmrCoverTypeId = (long)_TYpe;

                    var resultData = new BackendAdapter(param1).Post<bool>("api/HisTreatment/UpdateEmrCover", ApiConsumers.MosConsumer, TreatmentEmrCover, param1);

                    Inventec.Common.Logging.LogSystem.Debug("UpdateEmrCover: " + resultData + " TreatmentEmrCover: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => TreatmentEmrCover), TreatmentEmrCover));
                }

                WaitingManager.Hide();
                #endregion
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                LogSystem.Error(ex);
            }
        }

        bool CheckDiimTypeService(long serviceId, long diimTypeId)
        {
            bool valid = false;
            try
            {
                valid = BackendDataWorker.Get<V_HIS_SERVICE>().Any(o => o.DIIM_TYPE_ID == diimTypeId && o.ID == serviceId);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return valid;
        }

        ChuyenVien GetChuyenVienFromTranspatiForm(string levelCode1, string levelCode2)
        {
            ChuyenVien cv;
            var level1 = (!String.IsNullOrEmpty(levelCode1)) ? Convert.ToInt32(levelCode1) : 0;
            var level2 = (!String.IsNullOrEmpty(levelCode2)) ? Convert.ToInt32(levelCode2) : 0;
            if (level1 > level2)
            {
                cv = ChuyenVien.TuyenDuoi;
            }
            else if (level1 < level2)
            {
                cv = ChuyenVien.TuyenTren;
            }
            else { cv = ChuyenVien.Khac; }
            return cv;
        }

        string EncodeData(object data)
        {
            string result = "", dataString = "";
            System.Web.Script.Serialization.JavaScriptSerializer js = new System.Web.Script.Serialization.JavaScriptSerializer();
            dataString = js.Serialize(data);

            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(dataString);

            result = System.Convert.ToBase64String(plainTextBytes);

            return Inventec.Common.String.StringCompressor.CompressString(result);
        }

        V_HIS_DEPARTMENT_TRAN Get_DepartmentTranYc(long _TreatmentId)
        {
            try
            {
                V_HIS_DEPARTMENT_TRAN result = new V_HIS_DEPARTMENT_TRAN();

                CommonParam param = new CommonParam();
                HisDepartmentTranViewFilter DepartmentTranFilter = new HisDepartmentTranViewFilter();
                DepartmentTranFilter.TREATMENT_ID = _TreatmentId;

                var resultData = new BackendAdapter(param).Get<List<V_HIS_DEPARTMENT_TRAN>>("api/HisDepartmentTran/GetView", ApiConsumers.MosConsumer, DepartmentTranFilter, param);

                if (resultData != null && resultData.Count > 0)
                {
                    result = resultData.FirstOrDefault(o => o.IS_HOSPITALIZED == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE);
                }

                return result;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return null;
            }
        }

        private static long? DayOfTreatmentDepartment(long in_time, long? out_time, long treatment_type_id)
        {
            long? result = null;
            try
            {
                if (out_time.HasValue)
                {
                    System.DateTime? dateBefore = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTimeUTC(in_time);
                    System.DateTime? dateAfter = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTimeUTC(out_time.Value);
                    if (dateBefore != null && dateAfter != null)
                    {
                        TimeSpan difference = dateAfter.Value - dateBefore.Value;

                        //Lớn hơn 24h thì ngày ra - ngày vào + 1;
                        if ((difference.Days > 1 || (difference.Days == 1 && (difference.Hours >= 1 || difference.Minutes >= 1 || difference.Seconds >= 1))) && treatment_type_id != IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__KHAM)
                        {
                            result = (int)((TimeSpan)(dateAfter.Value.Date - dateBefore.Value.Date)).TotalDays + 1;
                        }
                        else if (treatment_type_id != IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__KHAM)
                        {
                            result = 1;
                        }
                    }
                }
            }
            catch (Exception)
            {
                result = null;
            }
            return result;
        }

        public string GenerateStorageNumber(string endCode, long? deathTime, string icdCauseCode)
        {
            string result = "";
            try
            {
                string cx15 = "", db15 = "", di14 = "";
                if (!String.IsNullOrEmpty(endCode))
                {
                    int count = endCode.Length - endCode.Replace("/", "").Length + 1;
                    cx15 = (count == 3) ? endCode.Substring(endCode.Length - (endCode.Length - 3)) : endCode;
                    if (endCode.Length >= 10)
                    {
                        db15 = endCode.Substring(4, 6);
                    }
                }

                if (deathTime != null)
                {
                    di14 = "/TV";
                }
                else if (!string.IsNullOrEmpty(icdCauseCode))
                {
                    di14 = "/TN";
                }

                result = cx15 + db15 + di14;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return result;
            }
            return result;
        }
    }
}

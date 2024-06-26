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
using EMR_MAIN;
using EMR_MAIN.DATABASE.BenhAn;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.TreatmentList.ADO;
using HIS.Desktop.Plugins.TreatmentList.Config;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.TreatmentList
{
    public partial class UCTreatmentList : UserControlBase
    {
        private void LoadData_ERMv3(LoaiBenhAnEMR _TYpe)
        {
            WaitingManager.Show();
            if (this.currentTreatment == null)
                return;
            Inventec.Common.Logging.LogSystem.Debug("LoadData_ERMv3. 1");

            List<PhauThuatThuThuat_HIS> PhauThuatThuThuat_HISs = new List<PhauThuatThuThuat_HIS>();

            #region --- Load
            CommonParam param = new CommonParam();
            MOS.Filter.HisPatientViewFilter _patientFilter = new MOS.Filter.HisPatientViewFilter();
            _patientFilter.ID = this.currentTreatment.PATIENT_ID;
            var currentPatient = new BackendAdapter(param).Get<List<V_HIS_PATIENT>>("api/HisPatient/getView", ApiConsumers.MosConsumer, _patientFilter, param);
            if (currentPatient == null || currentPatient.Count == 0)
                throw new NullReferenceException("Khong lay duoc V_HIS_PATIENT bang ID" + this.currentTreatment.PATIENT_ID);
            V_HIS_PATIENT _Patient = currentPatient.FirstOrDefault();

            HisPatientTypeAlterViewAppliedFilter filter = new HisPatientTypeAlterViewAppliedFilter();
            filter.TreatmentId = this.currentTreatment.ID;
            filter.InstructionTime = Inventec.Common.DateTime.Get.Now() ?? 0;
            V_HIS_PATIENT_TYPE_ALTER _PatientTypeAlter = new BackendAdapter(param).Get<V_HIS_PATIENT_TYPE_ALTER>("/api/HisPatientTypeAlter/GetApplied", ApiConsumers.MosConsumer, filter, param);

            HisDhstFilter _dhstFilter = new HisDhstFilter();
            _dhstFilter.TREATMENT_ID = this.currentTreatment.ID;
            _dhstFilter.ORDER_DIRECTION = "DESC";
            _dhstFilter.ORDER_FIELD = "EXECUTE_TIME";
            var currentDhst = new BackendAdapter(param).Get<List<HIS_DHST>>("/api/HisDhst/Get", ApiConsumers.MosConsumer, _dhstFilter, param);
            HIS_DHST _DHST = new HIS_DHST();
            currentDhst = (currentDhst != null && currentDhst.Count > 0) ? currentDhst.Where(o => GetRoomTypeByRoomId(o.EXECUTE_ROOM_ID) == IMSys.DbConfig.HIS_RS.HIS_ROOM_TYPE.ID__XL).ToList() : null;
            if (currentDhst != null && currentDhst.Count > 0)
            {
                _DHST = currentDhst.FirstOrDefault();
            }
            MOS.Filter.HisBabyViewFilter _babyFIlter = new HisBabyViewFilter();
            _babyFIlter.TREATMENT_ID = this.currentTreatment.ID;
            var currentBaby = new BackendAdapter(param).Get<List<V_HIS_BABY>>("/api/HisBaby/GetView", ApiConsumers.MosConsumer, _babyFIlter, param);
            V_HIS_BABY _Baby = new V_HIS_BABY();
            if (currentBaby != null && currentBaby.Count > 0)
            {
                _Baby = currentBaby.FirstOrDefault();
            }

            MOS.Filter.HisDepartmentTranViewFilter _departmentTranFilter = new HisDepartmentTranViewFilter();
            _departmentTranFilter.TREATMENT_ID = this.currentTreatment.ID;
            _departmentTranFilter.ORDER_DIRECTION = "ASC";
            _departmentTranFilter.ORDER_FIELD = "CREATE_TIME";
            var _DepartmentTrans = new BackendAdapter(param).Get<List<V_HIS_DEPARTMENT_TRAN>>("/api/HisDepartmentTran/GetView", ApiConsumers.MosConsumer, _departmentTranFilter, param);
            if (_DepartmentTrans != null && _DepartmentTrans.Count > 0)
            {
                _DepartmentTrans = _DepartmentTrans.Where(p => p.DEPARTMENT_IN_TIME != null).ToList();
            }

            MOS.Filter.HisServiceReqViewFilter _reqFilter = new HisServiceReqViewFilter();
            _reqFilter.TREATMENT_ID = this.currentTreatment.ID;
            _reqFilter.ORDER_DIRECTION = "DESC";
            _reqFilter.ORDER_FIELD = "MODIFY_TIME";
            var currentServiceReqs = new BackendAdapter(param).Get<List<V_HIS_SERVICE_REQ>>("/api/HisServiceReq/GetView", ApiConsumers.MosConsumer, _reqFilter, param);
            V_HIS_SERVICE_REQ _ExamServiceReq = new V_HIS_SERVICE_REQ();
            if (currentServiceReqs != null && currentServiceReqs.Count > 0)
            {
                _ExamServiceReq = currentServiceReqs.FirstOrDefault(p => p.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__KH);
            }

            int treatmentCount = 1;
            MOS.Filter.HisTreatmentFilter treatmentCountFilter = new HisTreatmentFilter();
            treatmentCountFilter.PATIENT_ID = _Patient.ID;
            var treatmentPatients = new BackendAdapter(param).Get<List<HIS_TREATMENT>>("/api/HisTreatment/Get", ApiConsumers.MosConsumer, treatmentCountFilter, param);
            if (treatmentPatients != null && treatmentPatients.Count > 0)
            {
                treatmentCount = treatmentPatients.Count;
            }

            MOS.Filter.HisSereServPtttViewFilter sereServPtttFilter = new HisSereServPtttViewFilter();
            sereServPtttFilter.TDL_TREATMENT_ID = this.currentTreatment.ID;
            sereServPtttFilter.ORDER_DIRECTION = "DESC";
            sereServPtttFilter.ORDER_FIELD = "MODIFY_TIME";
            var sereServPttts = new BackendAdapter(param).Get<List<V_HIS_SERE_SERV_PTTT>>("/api/HisSereServPttt/GetView", ApiConsumers.MosConsumer, sereServPtttFilter, param);

            var currentServiceReqIdCls = currentServiceReqs != null ? currentServiceReqs.Where(o => o.SERVICE_REQ_STT_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__CXL).Select(o => o.ID).ToList() : new List<long>();

            MOS.Filter.HisSereServFilter sereServFilter = new HisSereServFilter();
            sereServFilter.TREATMENT_ID = this.currentTreatment.ID;
            sereServFilter.ORDER_DIRECTION = "DESC";
            sereServFilter.ORDER_FIELD = "MODIFY_TIME";
            //sereServFilter.SERVICE_REQ_IDs = currentServiceReqIdCls;
            //sereServFilter.TDL_SERVICE_REQ_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__XN;
            //sereServFilter.TDL_SERVICE_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__XN;
            var sereServAlls = new BackendAdapter(param).Get<List<HIS_SERE_SERV>>("/api/HisSereServ/Get", ApiConsumers.MosConsumer, sereServFilter, param);

            var sereServCLSs = (sereServAlls != null && sereServAlls.Count > 0) ? sereServAlls.Where(o => (o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__XN || o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__CDHA || o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__SA || o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__NS || o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__GPBL)
                && currentServiceReqIdCls.Contains(o.SERVICE_REQ_ID ?? 0)).ToList() : null;

            #endregion
            Inventec.Common.Logging.LogSystem.Debug("LoadData_ERMv3. 2");
            #region ------- HanhChinhBenhNhan
            HanhChinhBenhNhan _HanhChinhBenhNhan = new HanhChinhBenhNhan();
            _HanhChinhBenhNhan.SoYTe = BackendDataWorker.Get<HIS_BRANCH>().FirstOrDefault(p => p.ID == WorkPlace.WorkPlaceSDO.FirstOrDefault().BranchId).PARENT_ORGANIZATION_NAME;
            _HanhChinhBenhNhan.BenhVien = BackendDataWorker.Get<HIS_BRANCH>().FirstOrDefault(p => p.ID == WorkPlace.WorkPlaceSDO.FirstOrDefault().BranchId).BRANCH_NAME;
            _HanhChinhBenhNhan.MaYTe = "";
            _HanhChinhBenhNhan.MaBenhNhan = _Patient.PATIENT_CODE;
            _HanhChinhBenhNhan.TenBenhNhan = this.currentTreatment.TDL_PATIENT_NAME;
            _HanhChinhBenhNhan.NgaySinh = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(this.currentTreatment.TDL_PATIENT_DOB) ?? DateTime.Now;
            _HanhChinhBenhNhan.Tuoi = MPS.AgeUtil.CalculateFullAge(this.currentTreatment.TDL_PATIENT_DOB);
            _HanhChinhBenhNhan.GioiTinh = this.currentTreatment.TDL_PATIENT_GENDER_ID == IMSys.DbConfig.HIS_RS.HIS_GENDER.ID__MALE ? GioiTinh.Nam : this.currentTreatment.TDL_PATIENT_GENDER_ID == IMSys.DbConfig.HIS_RS.HIS_GENDER.ID__FEMALE ? GioiTinh.Nu : GioiTinh.ChuaXacDinh;
            _HanhChinhBenhNhan.NgheNghiep = _Patient.CAREER_NAME;
            _HanhChinhBenhNhan.MaNgheNghiep = _Patient.CAREER_CODE;
            _HanhChinhBenhNhan.DanToc = _Patient.NATIONAL_NAME;
            _HanhChinhBenhNhan.MaDanhToc = _Patient.NATIONAL_CODE;
            _HanhChinhBenhNhan.NgoaiKieu = "";
            _HanhChinhBenhNhan.MaNgoaiKieu = "";
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

            _HanhChinhBenhNhan.NoiLamViec = _Patient.WORK_PLACE;
            _HanhChinhBenhNhan.DoiTuong = _PatientTypeAlter.PATIENT_TYPE_CODE == HisConfigCFG.PatientTypeCode__BHYT ? DoiTuong.BHYT : _PatientTypeAlter.PATIENT_TYPE_CODE == HisConfigCFG.PatientTypeCode__VP ? DoiTuong.ThuPhi : DoiTuong.Khac;
            if (_PatientTypeAlter.HEIN_CARD_FROM_TIME > 0)
                _HanhChinhBenhNhan.NgayDangKyBHYT = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(_PatientTypeAlter.HEIN_CARD_FROM_TIME ?? 0) ?? null;
            if (_PatientTypeAlter.HEIN_CARD_TO_TIME > 0)
                _HanhChinhBenhNhan.NgayHetHanBHYT = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(_PatientTypeAlter.HEIN_CARD_TO_TIME ?? 0) ?? null;
            _HanhChinhBenhNhan.SoTheBHYT = _PatientTypeAlter.HEIN_CARD_NUMBER;
            _HanhChinhBenhNhan.TenNoiDangKyBHYT = _PatientTypeAlter.HEIN_MEDI_ORG_NAME;
            _HanhChinhBenhNhan.MaNoiDangKyBHYT = _PatientTypeAlter.HEIN_MEDI_ORG_CODE;
            _HanhChinhBenhNhan.NgayDuocHuong5Nam = _PatientTypeAlter.JOIN_5_YEAR == MOS.LibraryHein.Bhyt.HeinJoin5Year.HeinJoin5YearCode.TRUE ? Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(_PatientTypeAlter.JOIN_5_YEAR_TIME ?? 0) : null;
            _HanhChinhBenhNhan.HoTenDiaChiNguoiNha = _Patient.RELATIVE_NAME + " - " + _Patient.RELATIVE_ADDRESS;
            //_HanhChinhBenhNhan.SoDienThoaiNguoiNha = _Patient.re;
            #endregion

            #region ------- ThongTinDieuTri
            ThongTinDieuTri _ThongTinDieuTri = new ThongTinDieuTri();
            if (_Baby != null)
            {
                _ThongTinDieuTri.LucVaoDe = "";
                _ThongTinDieuTri.NgayDe = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(_Baby.BORN_TIME ?? 0);
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

            _ThongTinDieuTri.NguyenNhan_BenhChinh_RaVien = this.currentTreatment.ICD_NAME;
            if (this.currentTreatment.END_DEPARTMENT_ID > 0)
            {
                var dpEnd = BackendDataWorker.Get<HIS_DEPARTMENT>().Where(p => p.ID == this.currentTreatment.END_DEPARTMENT_ID).FirstOrDefault();
                _ThongTinDieuTri.KhoaRaVien = dpEnd != null ? dpEnd.DEPARTMENT_NAME : "";
            }

            _ThongTinDieuTri.MaBenhAn = currentTreatment.TREATMENT_CODE;
            _ThongTinDieuTri.GiuongRaVien = "";
            _ThongTinDieuTri.SoLuuTru = currentTreatment.STORE_CODE;
            _ThongTinDieuTri.MaQuanLy = Inventec.Common.TypeConvert.Parse.ToDecimal(currentTreatment.TREATMENT_CODE);//Mỗi lần vào điều trị có cái mã
            _ThongTinDieuTri.MaBenhNhan = _Patient.PATIENT_CODE;
            _ThongTinDieuTri.NgayVaoVien = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(this.currentTreatment.IN_TIME) ?? null;
            _ThongTinDieuTri.TrucTiepVao = (_ExamServiceReq != null && _ExamServiceReq.IS_EMERGENCY == 1) ? TrucTiepVao.CapCuu : _PatientTypeAlter.TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__KHAM ? TrucTiepVao.KKB : TrucTiepVao.KhoaDieuTri;
            _ThongTinDieuTri.NoiGioiThieu = (_PatientTypeAlter.RIGHT_ROUTE_TYPE_CODE == MOS.LibraryHein.Bhyt.HeinRightRouteType.HeinRightRouteTypeCode.PRESENT && currentTreatment.TRANSFER_IN_FORM_ID > 0) ? NoiGioiThieu.CoQuanYTe : NoiGioiThieu.Khac;

            var departmentIdClss = BackendDataWorker.Get<HIS_DEPARTMENT>().Where(o => o.IS_CLINICAL == 1).Select(o => o.ID).ToArray();
            _DepartmentTrans = (_DepartmentTrans != null && _DepartmentTrans.Count > 0) ? _DepartmentTrans.Where(o => departmentIdClss.Contains(o.DEPARTMENT_ID)).ToList() : null;
            if (_DepartmentTrans != null && _DepartmentTrans.Count > 0)
            {
                _DepartmentTrans = _DepartmentTrans.OrderBy(o => o.DEPARTMENT_IN_TIME).ToList();

                _ThongTinDieuTri.TenKhoaVao = _DepartmentTrans[0].DEPARTMENT_NAME;
                _ThongTinDieuTri.NgayVaoKhoa = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(_DepartmentTrans[0].DEPARTMENT_IN_TIME ?? 0) ?? null;
                if (_DepartmentTrans.Count > 1)
                {
                    long? songay = null;
                    if (this.currentTreatment.TDL_PATIENT_TYPE_ID == HIS.Desktop.Plugins.TreatmentList.Config.HisConfigCFG.PatientTypeId__BHYT)
                    {
                        songay = HIS.Common.Treatment.Calculation.DayOfTreatment(_DepartmentTrans[0].DEPARTMENT_IN_TIME, _DepartmentTrans[1].DEPARTMENT_IN_TIME, currentTreatment.TREATMENT_END_TYPE_ID, currentTreatment.TREATMENT_RESULT_ID, HIS.Common.Treatment.PatientTypeEnum.TYPE.BHYT);
                    }
                    else
                    {
                        songay = HIS.Common.Treatment.Calculation.DayOfTreatment(_DepartmentTrans[0].DEPARTMENT_IN_TIME, _DepartmentTrans[1].DEPARTMENT_IN_TIME, currentTreatment.TREATMENT_END_TYPE_ID, currentTreatment.TREATMENT_RESULT_ID, HIS.Common.Treatment.PatientTypeEnum.TYPE.THU_PHI);
                    }

                    _ThongTinDieuTri.SoNgayDieuTriTaiKhoa = Inventec.Common.TypeConvert.Parse.ToInt32(songay.ToString());

                    _ThongTinDieuTri.ChuyenKhoa1 = _DepartmentTrans[1].DEPARTMENT_NAME;
                    _ThongTinDieuTri.NgayChuyenKhoa1 = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(_DepartmentTrans[1].DEPARTMENT_IN_TIME ?? 0) ?? null;
                    if (_DepartmentTrans.Count > 2)
                    {
                        long? songay1 = null;
                        if (this.currentTreatment.TDL_PATIENT_TYPE_ID == HIS.Desktop.Plugins.TreatmentList.Config.HisConfigCFG.PatientTypeId__BHYT)
                        {
                            songay1 = HIS.Common.Treatment.Calculation.DayOfTreatment(_DepartmentTrans[1].DEPARTMENT_IN_TIME, _DepartmentTrans[2].DEPARTMENT_IN_TIME, currentTreatment.TREATMENT_END_TYPE_ID, currentTreatment.TREATMENT_RESULT_ID, HIS.Common.Treatment.PatientTypeEnum.TYPE.BHYT);
                        }
                        else
                        {
                            songay1 = HIS.Common.Treatment.Calculation.DayOfTreatment(_DepartmentTrans[1].DEPARTMENT_IN_TIME, _DepartmentTrans[2].DEPARTMENT_IN_TIME, currentTreatment.TREATMENT_END_TYPE_ID, currentTreatment.TREATMENT_RESULT_ID, HIS.Common.Treatment.PatientTypeEnum.TYPE.THU_PHI);
                        }

                        _ThongTinDieuTri.SoNgayDieuTriKhoa1 = Inventec.Common.TypeConvert.Parse.ToInt32(songay1.ToString());

                        _ThongTinDieuTri.ChuyenKhoa2 = _DepartmentTrans[2].DEPARTMENT_NAME;
                        _ThongTinDieuTri.NgayChuyenKhoa2 = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(_DepartmentTrans[2].DEPARTMENT_IN_TIME ?? 0) ?? null;
                        if (_DepartmentTrans.Count > 3)
                        {
                            long? songay2 = null;
                            if (this.currentTreatment.TDL_PATIENT_TYPE_ID == HIS.Desktop.Plugins.TreatmentList.Config.HisConfigCFG.PatientTypeId__BHYT)
                            {
                                songay2 = HIS.Common.Treatment.Calculation.DayOfTreatment(_DepartmentTrans[2].DEPARTMENT_IN_TIME, _DepartmentTrans[3].DEPARTMENT_IN_TIME, currentTreatment.TREATMENT_END_TYPE_ID, currentTreatment.TREATMENT_RESULT_ID, HIS.Common.Treatment.PatientTypeEnum.TYPE.BHYT);
                            }
                            else
                            {
                                songay2 = HIS.Common.Treatment.Calculation.DayOfTreatment(_DepartmentTrans[2].DEPARTMENT_IN_TIME, _DepartmentTrans[3].DEPARTMENT_IN_TIME, currentTreatment.TREATMENT_END_TYPE_ID, currentTreatment.TREATMENT_RESULT_ID, HIS.Common.Treatment.PatientTypeEnum.TYPE.THU_PHI);
                            }
                            _ThongTinDieuTri.SoNgayDieuTriKhoa2 = Inventec.Common.TypeConvert.Parse.ToInt32(songay2.ToString());
                            _ThongTinDieuTri.ChuyenKhoa3 = _DepartmentTrans[3].DEPARTMENT_NAME;
                            _ThongTinDieuTri.NgayChuyenKhoa3 = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(_DepartmentTrans[3].DEPARTMENT_IN_TIME ?? 0) ?? null;

                            if (_DepartmentTrans.Count > 4)
                            {
                                long? songay3 = null;
                                if (this.currentTreatment.TDL_PATIENT_TYPE_ID == HIS.Desktop.Plugins.TreatmentList.Config.HisConfigCFG.PatientTypeId__BHYT)
                                {
                                    songay3 = HIS.Common.Treatment.Calculation.DayOfTreatment(_DepartmentTrans[3].DEPARTMENT_IN_TIME, _DepartmentTrans[4].DEPARTMENT_IN_TIME, currentTreatment.TREATMENT_END_TYPE_ID, currentTreatment.TREATMENT_RESULT_ID, HIS.Common.Treatment.PatientTypeEnum.TYPE.BHYT);
                                }
                                else
                                {
                                    songay3 = HIS.Common.Treatment.Calculation.DayOfTreatment(_DepartmentTrans[3].DEPARTMENT_IN_TIME, _DepartmentTrans[4].DEPARTMENT_IN_TIME, currentTreatment.TREATMENT_END_TYPE_ID, currentTreatment.TREATMENT_RESULT_ID, HIS.Common.Treatment.PatientTypeEnum.TYPE.THU_PHI);
                                }

                                _ThongTinDieuTri.SoNgayDieuTriKhoa3 = Inventec.Common.TypeConvert.Parse.ToInt32(songay3.ToString());
                            }
                        }
                    }
                }
            }

            //Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => _DepartmentTrans), _DepartmentTrans) +
            //Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => _ThongTinDieuTri), _ThongTinDieuTri));
            if (currentTreatment.TRAN_PATI_FORM_ID > 0)
                _ThongTinDieuTri.ChuyenVien = GetChuyenVienFromTranspatiForm(currentTreatment.TRAN_PATI_FORM_ID);
            _ThongTinDieuTri.TenVienChuyenBenhNhanDen = this.currentTreatment.TRANSFER_IN_MEDI_ORG_NAME;
            _ThongTinDieuTri.MaVienChuyenBenhNhanDen = this.currentTreatment.TRANSFER_IN_MEDI_ORG_CODE;
            if (this.currentTreatment.OUT_TIME > 0)
                _ThongTinDieuTri.NgayRaVien = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(this.currentTreatment.OUT_TIME ?? 0) ?? null;
            if (this.currentTreatment.TREATMENT_END_TYPE_ID > 0)
                _ThongTinDieuTri.TinhTrangRaVien = this.currentTreatment.TREATMENT_END_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__RAVIEN ? TinhTrangRaVien.RaVien : this.currentTreatment.TREATMENT_END_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__TRON ? TinhTrangRaVien.BoVe : this.currentTreatment.TREATMENT_END_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__XINRAVIEN ? TinhTrangRaVien.XinVe : TinhTrangRaVien.DuaVe;
            long? _snDieuTri = null;
            if (this.currentTreatment.TDL_PATIENT_TYPE_ID == HIS.Desktop.Plugins.TreatmentList.Config.HisConfigCFG.PatientTypeId__BHYT)
            {
                _snDieuTri = HIS.Common.Treatment.Calculation.DayOfTreatment(this.currentTreatment.CLINICAL_IN_TIME, this.currentTreatment.OUT_TIME, currentTreatment.TREATMENT_END_TYPE_ID, currentTreatment.TREATMENT_RESULT_ID, HIS.Common.Treatment.PatientTypeEnum.TYPE.BHYT);
            }
            else
            {
                _snDieuTri = HIS.Common.Treatment.Calculation.DayOfTreatment(this.currentTreatment.CLINICAL_IN_TIME, this.currentTreatment.OUT_TIME, currentTreatment.TREATMENT_END_TYPE_ID, currentTreatment.TREATMENT_RESULT_ID, HIS.Common.Treatment.PatientTypeEnum.TYPE.THU_PHI);
            }
            _ThongTinDieuTri.TongSoNgayDieuTri1 = _snDieuTri.ToString();
            _ThongTinDieuTri.TongSoNgayDieuTri2 = _snDieuTri.ToString();
            _ThongTinDieuTri.ChanDoan_NoiChuyenDen = this.currentTreatment.IN_ICD_NAME;
            _ThongTinDieuTri.MaICD_NoiChuyenDen = this.currentTreatment.IN_ICD_CODE;
            _ThongTinDieuTri.ChanDoan_KKB_CapCuu = "";
            _ThongTinDieuTri.MaICD_KKB_CapCuu = "";

            //Lấy chẩn đoán chính
            if (_ExamServiceReq != null)
            {
                _ThongTinDieuTri.ChanDoan_KhiVaoKhoaDieuTri = _ExamServiceReq.ICD_NAME;
                _ThongTinDieuTri.MaICD_KhiVaoKhoaDieuTri = _ExamServiceReq.ICD_CODE;
            }

            _ThongTinDieuTri.BenhChinh_RaVien = this.currentTreatment.ICD_NAME;
            _ThongTinDieuTri.MaICD_BenhChinh_RaVien = this.currentTreatment.ICD_CODE;
            _ThongTinDieuTri.BenhKemTheo_RaVien = this.currentTreatment.ICD_TEXT;
            _ThongTinDieuTri.MaICD_BenhKemTheo_RaVien = this.currentTreatment.ICD_SUB_CODE;

            _ThongTinDieuTri.VaoVienDoBenhNayLanThu = treatmentCount;
            if (sereServPttts != null && sereServPttts.Count > 0)
            {
                PhauThuatThuThuat_HISs = new List<PhauThuatThuThuat_HIS>();
                var sereServIds = sereServPttts.Select(o => o.SERE_SERV_ID).ToList();
                param = new CommonParam();
                HisSereServExtFilter ssExtFilter = new HisSereServExtFilter();
                ssExtFilter.SERE_SERV_IDs = sereServIds;
                var sereServExts = new BackendAdapter(param)
                    .Get<List<MOS.EFMODEL.DataModels.HIS_SERE_SERV_EXT>>("api/HisSereServExt/Get", ApiConsumers.MosConsumer, ssExtFilter, param);

                var sereServByTreatments = sereServAlls.Where(o => sereServIds.Contains(o.ID)).ToList();
                var ssInKipIds = sereServByTreatments.Where(o => o.EKIP_ID.HasValue).Select(o => o.EKIP_ID ?? 0).ToList();
                param = new CommonParam();
                HisEkipUserViewFilter hisEkipUserFilter = new HisEkipUserViewFilter();
                var hisEkipUsers = new BackendAdapter(param)
        .Get<List<V_HIS_EKIP_USER>>("api/HisEkipUser/GetView", ApiConsumers.MosConsumer, hisEkipUserFilter, param);

                param = new CommonParam();
                HisEkipPlanUserViewFilter hisEkipPlanUserFilter = new HisEkipPlanUserViewFilter();
                //hisEkipPlanUserFilter.EKIP_PLAN_ID = this.serviceReq.EKIP_PLAN_ID;
                var hisEkipPlanUsers = new BackendAdapter(param)
        .Get<List<V_HIS_EKIP_PLAN_USER>>("api/HisEkipPlanUser/GetView", ApiConsumers.MosConsumer, hisEkipPlanUserFilter, param);

                foreach (var sspttt in sereServPttts)
                {
                    HIS_SERE_SERV_EXT ssext = sereServExts != null && sereServExts.Count > 0 ? sereServExts.Where(o => o.SERE_SERV_ID == sspttt.SERE_SERV_ID).FirstOrDefault() : null;

                    DateTime? beginTime = (ssext != null && ssext.BEGIN_TIME.HasValue) ? Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(ssext.BEGIN_TIME.Value).Value : DateTime.MinValue;

                    var ss = sereServAlls != null ? sereServAlls.Where(o => o.ID == sspttt.SERE_SERV_ID).FirstOrDefault() : null;
                    var serviceReq = ss != null ? currentServiceReqs.Where(o => o.ID == ss.SERVICE_REQ_ID).FirstOrDefault() : null;
                    if (serviceReq != null && ss != null && ss.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__TT && HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("HIS.Desktop.Plugins.SurgServiceReqExecute.TakeIntrucionTimeByServiceReq") == "1" && ((ssext != null && ssext.BEGIN_TIME == null) || ssext == null))
                    {
                        beginTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(serviceReq.INTRUCTION_TIME) ?? DateTime.Now;
                    }

                    DateTime datePttt = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(Inventec.Common.TypeConvert.Parse.ToInt64(beginTime.Value.ToString("yyyyMMdd") + "000000")).Value;
                    string bacSyPhauThuat = "", bacSyPhauThuatHoVaTen = "", bacSyGayMe = "", bacSyGayMeHoVaTen = "";
                    string executeRoleCode__PhauThuat = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("DBCODE.HIS_RS.HIS_EXECUTE_ROLE.EXECUTE_ROLE_CODE.MAIN");//TODO
                    string executeRoleCode__GayMe = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("DBCODE.HIS_RS.HIS_EXECUTE_ROLE.EXECUTE_ROLE_CODE.ANESTHETIST");//TODO
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => executeRoleCode__PhauThuat), executeRoleCode__PhauThuat) + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => executeRoleCode__GayMe), executeRoleCode__GayMe));
                    if (ss.EKIP_ID.HasValue)
                    {
                        var ekipUsers = hisEkipUsers != null ? hisEkipUsers.Where(o => o.EKIP_ID == ss.EKIP_ID).ToList() : null;
                        if (ekipUsers != null && ekipUsers.Count > 0)
                        {
                            var ekipUserBSPhauThuat = ekipUsers.Where(o => o.EXECUTE_ROLE_CODE == executeRoleCode__PhauThuat).FirstOrDefault();
                            var ekipUserBSGatMe = ekipUsers.Where(o => o.EXECUTE_ROLE_CODE == executeRoleCode__GayMe).FirstOrDefault();
                            bacSyPhauThuat = ekipUserBSPhauThuat != null ? ekipUserBSPhauThuat.LOGINNAME : "";
                            bacSyPhauThuatHoVaTen = ekipUserBSPhauThuat != null ? ekipUserBSPhauThuat.USERNAME : "";
                            bacSyGayMe = ekipUserBSGatMe != null ? ekipUserBSGatMe.LOGINNAME : "";
                            bacSyGayMeHoVaTen = ekipUserBSGatMe != null ? ekipUserBSGatMe.USERNAME : "";
                        }
                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => ekipUsers), ekipUsers) + Inventec.Common.Logging.LogUtil.TraceData("hisEkipUsers.count", hisEkipUsers != null ? hisEkipUsers.Count : 0) + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => ss.EKIP_ID), ss.EKIP_ID));
                    }
                    else if (serviceReq.EKIP_PLAN_ID.HasValue)
                    {
                        var ekipPlans = hisEkipPlanUsers != null ? hisEkipPlanUsers.Where(o => o.EKIP_PLAN_ID == serviceReq.EKIP_PLAN_ID).ToList() : null;
                        if (ekipPlans != null && ekipPlans.Count > 0)
                        {
                            var ekipUserPlanBSPhauThuat = ekipPlans != null ? ekipPlans.Where(o => o.EXECUTE_ROLE_CODE == executeRoleCode__PhauThuat).FirstOrDefault() : null;
                            var ekipUserPlanBSGatMe = ekipPlans != null ? ekipPlans.Where(o => o.EXECUTE_ROLE_CODE == executeRoleCode__GayMe).FirstOrDefault() : null;
                            bacSyPhauThuat = ekipUserPlanBSPhauThuat != null ? ekipUserPlanBSPhauThuat.LOGINNAME : "";
                            bacSyPhauThuatHoVaTen = ekipUserPlanBSPhauThuat != null ? ekipUserPlanBSPhauThuat.USERNAME : "";
                            bacSyGayMe = ekipUserPlanBSGatMe != null ? ekipUserPlanBSGatMe.LOGINNAME : "";
                            bacSyGayMeHoVaTen = ekipUserPlanBSGatMe != null ? ekipUserPlanBSGatMe.USERNAME : "";
                        }
                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => ekipPlans), ekipPlans) + Inventec.Common.Logging.LogUtil.TraceData("hisEkipPlanUsers.count", hisEkipPlanUsers != null ? hisEkipPlanUsers.Count : 0) + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => serviceReq.EKIP_PLAN_ID), serviceReq.EKIP_PLAN_ID));
                    }

                    PhauThuatThuThuat_HISs.Add(new PhauThuatThuThuat_HIS()
                    {
                        PhuongPhapPhauThuatThuThuat = sspttt.PTTT_METHOD_NAME,
                        PhuongPhapVoCam = sspttt.EMOTIONLESS_METHOD_NAME,
                        NgayPhauThuatThuThuat = datePttt,
                        NgayPhauThuatThuThuat_Gio = beginTime,
                        BacSyPhauThuat = bacSyPhauThuat,
                        BacSyPhauThuatHoVaTen = bacSyPhauThuatHoVaTen,
                        BacSyGayMe = bacSyGayMe,
                        BacSyGayMeHoVaTen = bacSyGayMeHoVaTen
                    });
                }

                PhauThuatThuThuat_HISs = PhauThuatThuThuat_HISs.OrderBy(o => o.NgayPhauThuatThuThuat).ToList();

                //Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => PhauThuatThuThuat_HISs), PhauThuatThuThuat_HISs) + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => sereServPttts), sereServPttts) + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => sereServByTreatments), sereServByTreatments));

                _ThongTinDieuTri.TongSoLanPhauThuat = sereServPttts.Count;//TODO
                _ThongTinDieuTri.TongSoNgayDieuTriSauPT = null;//TODO
                _ThongTinDieuTri.LyDoTaiBienBienChung = null;//TODO
                //var ptttMethod = BackendDataWorker.Get<HIS_PTTT_METHOD>().Where(o => o.ID == sereServPttts[0].PTTT_METHOD_ID).FirstOrDefault();
                //_ThongTinDieuTri.PhuongPhapPhauThuat = ptttMethod != null ? ptttMethod.PTTT_METHOD_NAME : "";
                //_ThongTinDieuTri.TinhHinhPhauThuat = false;//TODO
                _ThongTinDieuTri.MaICD_ChanDoanSauPhauThuat = sereServPttts[0].AFTER_PTTT_ICD_CODE;
                _ThongTinDieuTri.MaICD_ChanDoanTruocPhauThuat = sereServPttts[0].BEFORE_PTTT_ICD_CODE;
                _ThongTinDieuTri.MaICD_NguyenNhan_BenhChinh_RV = sereServPttts[0].ICD_CODE;
                _ThongTinDieuTri.ChanDoanSauPhauThuat = sereServPttts[0].AFTER_PTTT_ICD_NAME;
                _ThongTinDieuTri.ChanDoanTruocPhauThuat = sereServPttts[0].BEFORE_PTTT_ICD_NAME;
                //_ThongTinDieuTri.ThuThuat = true;
                //_ThongTinDieuTri.PhauThuat = true;
                _ThongTinDieuTri.TaiBien = sereServPttts.Any(o => o.PTTT_CATASTROPHE_ID > 0);
                //_ThongTinDieuTri.BienChung = false;//khong co truong du lieu trong DB
            }


            if (this.currentTreatment.TREATMENT_RESULT_ID > 0)
                _ThongTinDieuTri.KetQuaDieuTri = this.currentTreatment.TREATMENT_RESULT_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_RESULT.ID__CHET ? KetQuaDieuTri.TuVong : this.currentTreatment.TREATMENT_RESULT_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_RESULT.ID__DO ? KetQuaDieuTri.GiamDo : this.currentTreatment.TREATMENT_RESULT_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_RESULT.ID__KHOI ? KetQuaDieuTri.Khoi : this.currentTreatment.TREATMENT_RESULT_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_RESULT.ID__NANG ? KetQuaDieuTri.NangHon : KetQuaDieuTri.KhongThayDoi;
            _ThongTinDieuTri.GiaiPhauBenh = 0;
            if (this.currentTreatment.DEATH_TIME > 0)
                _ThongTinDieuTri.NgayTuVong = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(this.currentTreatment.DEATH_TIME ?? 0);
            if (this.currentTreatment.DEATH_CAUSE_ID > 0)
            {
                var deathCause = BackendDataWorker.Get<HIS_DEATH_CAUSE>().Where(o => o.ID == this.currentTreatment.DEATH_CAUSE_ID).FirstOrDefault();
                if (deathCause != null)
                {
                    _ThongTinDieuTri.LyDoTuVong = deathCause.DEATH_CAUSE_CODE == "01" ? LyDoTuVong.DoBenh : deathCause.DEATH_CAUSE_CODE == "02" ? LyDoTuVong.DoTaiBienDieuTri : LyDoTuVong.Khac;
                    _ThongTinDieuTri.NguyenNhanChinhTuVong = deathCause.DEATH_CAUSE_NAME;
                    _ThongTinDieuTri.MaICD_NguyenNhanChinhTuVong = this.currentTreatment.ICD_CODE;
                }
            }
            if (this.currentTreatment.DEATH_WITHIN_ID > 0)
            {
                _ThongTinDieuTri.ThoiGianTuVong = this.currentTreatment.DEATH_WITHIN_ID == 1 ? ThoiGianTuVong.Trong24hVaoVien : ThoiGianTuVong.Sau24hvaoVien;

                _ThongTinDieuTri.KhamNghiemTuThi = false;
                _ThongTinDieuTri.ChanDoanGiaiPhauTuThi = "";
                _ThongTinDieuTri.MaICD_ChanDoanGiaiPhauTuThi = "";
            }


            V_HIS_TREATMENT_BED_ROOM treatmentBedRoom = new V_HIS_TREATMENT_BED_ROOM();
            param = new CommonParam();
            MOS.Filter.HisTreatmentBedRoomFilter treatmentBedroomFilter = new HisTreatmentBedRoomFilter();
            treatmentBedroomFilter.TREATMENT_ID = currentTreatment.ID;
            //treatmentBedroomFilter.IS_IN_ROOM = true;
            var treatmentBedRooms = new BackendAdapter(param).Get<List<V_HIS_TREATMENT_BED_ROOM>>("api/HisTreatmentBedRoom/GetView", ApiConsumer.ApiConsumers.MosConsumer, treatmentBedroomFilter, null);
            treatmentBedRoom = (treatmentBedRooms != null && treatmentBedRooms.Count() > 0) ? treatmentBedRooms.OrderByDescending(o => o.IN_TIME).FirstOrDefault() : null;
            //Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => treatmentBedRooms), treatmentBedRooms));
            if (treatmentBedRoom != null)
            {
                _ThongTinDieuTri.Buong = treatmentBedRoom.BED_ROOM_NAME;
                _ThongTinDieuTri.Giuong = treatmentBedRoom.BED_NAME;

                var bedroom = BackendDataWorker.Get<V_HIS_BED_ROOM>().Where(o => o.ID == treatmentBedRoom.BED_ROOM_ID).FirstOrDefault();

                if (bedroom != null)
                {
                    _ThongTinDieuTri.MaKhoa = bedroom.DEPARTMENT_CODE;
                    _ThongTinDieuTri.Khoa = bedroom.DEPARTMENT_NAME;
                }
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => bedroom), bedroom));
            }
            else
            {
                V_HIS_DEPARTMENT_TRAN departmentTran = new V_HIS_DEPARTMENT_TRAN();
                V_HIS_PATIENT_TYPE_ALTER patientTypeAlter = new V_HIS_PATIENT_TYPE_ALTER();

                MOS.Filter.HisPatientTypeAlterViewFilter patientTypeAlterFilter = new HisPatientTypeAlterViewFilter();
                patientTypeAlterFilter.TREATMENT_ID = currentTreatment.ID;
                param = new CommonParam();
                var patientTypeAlters = new BackendAdapter(param).Get<List<V_HIS_PATIENT_TYPE_ALTER>>("api/HisPatientTypeAlter/GetView", ApiConsumer.ApiConsumers.MosConsumer, patientTypeAlterFilter, param);
                if (patientTypeAlters != null && patientTypeAlters.Count() > 0)
                {
                    patientTypeAlter = patientTypeAlters.OrderByDescending(o => o.LOG_TIME).FirstOrDefault();

                    MOS.Filter.HisDepartmentTranViewFilter departmentTranFilter = new HisDepartmentTranViewFilter();
                    departmentTranFilter.ID = patientTypeAlter.DEPARTMENT_TRAN_ID;
                    param = new CommonParam();
                    var departmentTrans = new BackendAdapter(param).Get<List<V_HIS_DEPARTMENT_TRAN>>("api/HisDepartmentTran/GetView", ApiConsumer.ApiConsumers.MosConsumer, departmentTranFilter, param);
                    departmentTran = (departmentTrans != null && departmentTrans.Count > 0) ? departmentTrans.Where(p => p.DEPARTMENT_IN_TIME != null).First() : null;
                    _ThongTinDieuTri.MaKhoa = departmentTran != null ? departmentTran.DEPARTMENT_CODE : "";
                    _ThongTinDieuTri.Khoa = departmentTran != null ? departmentTran.DEPARTMENT_NAME : "";
                }
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => departmentTran), departmentTran));
            }

            _ThongTinDieuTri.MaGiamDocBenhVien = "";
            _ThongTinDieuTri.NgayThangNamTrangBia = DateTime.Now;
            _ThongTinDieuTri.MaTruongKhoa = "";
            #endregion

            #region ------ DHST
            _ThongTinDieuTri.DauSinhTon = new DauSinhTon();
            DauSinhTon _DauSinhTon = new DauSinhTon();
            if (_DHST != null)
            {
                _DauSinhTon.CanNang = (double)(_DHST.WEIGHT ?? 0);
                _DauSinhTon.HuyetAp = _DHST.BLOOD_PRESSURE_MAX + "/" + _DHST.BLOOD_PRESSURE_MIN;
                _DauSinhTon.Mach = (int)(_DHST.PULSE ?? 0);
                _DauSinhTon.NhietDo = (double)(_DHST.TEMPERATURE ?? 0);
                _DauSinhTon.NhipTho = (int)(_DHST.BREATH_RATE ?? 0);
                _DauSinhTon.ChieuCao = (double)(_DHST.HEIGHT ?? 0);
                _DauSinhTon.BMI = (double)(_DHST.VIR_BMI ?? 0);
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => _DauSinhTon), _DauSinhTon));
            }
            _ThongTinDieuTri.DauSinhTon = _DauSinhTon;
            #endregion

            #region ------ HoSo
            _ThongTinDieuTri.HoSo = new HoSo();
            HoSo _HoSo = new HoSo();
            //_HoSo.CTScanner = 1;
            //_HoSo.Khac = 3;
            //_HoSo.Khac_Text = "Khac_Text";
            //_HoSo.SieuAm = 4;
            //_HoSo.ToanBoHoSo = 5;
            //_HoSo.XetNghiem = 6;
            //_HoSo.XQuang = 7;
            _ThongTinDieuTri.HoSo = _HoSo;
            #endregion
            Inventec.Common.Logging.LogSystem.Debug("LoadData_ERMv3. 3");
            BenhAnCommonADO _BenhAnCommonADO = new BenhAnCommonADO();
            BenhAnNoiKhoa _BenhAnNoiKhoa = new BenhAnNoiKhoa();
            BenhAnNgoaiKhoa _BenhAnNgoaiKhoa = new BenhAnNgoaiKhoa();
            BenhAnDaLieu _BenhAnDaLieu = new BenhAnDaLieu();
            BenhAnBong _BenhAnBong = new BenhAnBong();
            BenhAnHuyetHocTruyenMau _BenhAnHuyetHocTruyenMau = new BenhAnHuyetHocTruyenMau();
            BenhAnMatBanPhanTruoc _BenhAnMatBanPhanTruoc = new BenhAnMatBanPhanTruoc();
            BenhAnMatChanThuong _BenhAnMatChanThuong = new BenhAnMatChanThuong();
            BenhAnMatDayMat _BenhAnMatDayMat = new BenhAnMatDayMat();
            BenhAnMatGlocom _BenhAnMatGlocom = new BenhAnMatGlocom();
            BenhAnMatLac _BenhAnMatLac = new BenhAnMatLac();
            BenhAnMatTreEm _BenhAnMatTreEm = new BenhAnMatTreEm();
            BenhAnNgoaiTru _BenhAnNgoaiTru = new BenhAnNgoaiTru();
            BenhAnNhiKhoa _BenhAnNhiKhoa = new BenhAnNhiKhoa();
            BenhAnNoiTruYHCT _BenhAnNoiTruYHCT = new BenhAnNoiTruYHCT();
            BenhAnNgoaiTruYHCT _BenhAnNgoaiTruYHCT = new BenhAnNgoaiTruYHCT();
            BenhAnPhucHoiChucNang _BenhAnPhucHoiChucNang = new BenhAnPhucHoiChucNang();
            BenhAnPhuKhoa _BenhAnPhuKhoa = new BenhAnPhuKhoa();
            BenhAnRangHamMat _BenhAnRangHamMat = new BenhAnRangHamMat();
            BenhAnSanKhoa _BenhAnSanKhoa = new BenhAnSanKhoa();
            BenhAnSoSinh _BenhAnSoSinh = new BenhAnSoSinh();
            BenhAnDieuTriBanNgay _BenhAnDieuTriBanNgay = new BenhAnDieuTriBanNgay();
            BenhAnTaiMuiHong _BenhAnTaiMuiHong = new BenhAnTaiMuiHong();
            BenhAnTruyenNhiem _BenhAnTruyenNhiem = new BenhAnTruyenNhiem();
            BenhAnNgoaiTruRangHamMat _BenhAnNgoaiTruRangHamMat = new BenhAnNgoaiTruRangHamMat();
            BenhAnNgoaiTruTaiMuiHong _BenhAnNgoaiTruTaiMuiHong = new BenhAnNgoaiTruTaiMuiHong();
            BenhAnTim _BenhAnTim = new BenhAnTim();
            // BenhAnCommonADO _BenhAnNoiKhoa = new BenhAnCommonADO();
            //BenhAnCommonADO _BenhAnNgoaiKhoa = new BenhAnCommonADO();
            //BenhAnCommonADO _BenhAnDaLieu = new BenhAnCommonADO();
            //BenhAnCommonADO _BenhAnBong = new BenhAnCommonADO();
            //BenhAnCommonADO _BenhAnHuyetHocTruyenMau = new BenhAnCommonADO();
            //BenhAnCommonADO _BenhAnMatBanPhanTruoc = new BenhAnCommonADO();
            //BenhAnCommonADO _BenhAnMatChanThuong = new BenhAnCommonADO();
            //BenhAnCommonADO _BenhAnMatDayMat = new BenhAnCommonADO();
            //BenhAnCommonADO _BenhAnMatGlocom = new BenhAnCommonADO();
            //BenhAnCommonADO _BenhAnMatLac = new BenhAnCommonADO();
            //BenhAnCommonADO _BenhAnMatTreEm = new BenhAnCommonADO();
            //BenhAnCommonADO _BenhAnNgoaiTru = new BenhAnCommonADO();
            //BenhAnCommonADO _BenhAnNhiKhoa = new BenhAnCommonADO();
            //BenhAnCommonADO _BenhAnNoiTruYHCT = new BenhAnCommonADO();
            //BenhAnCommonADO _BenhAnPhucHoiChucNang = new BenhAnCommonADO();
            //BenhAnCommonADO _BenhAnPhuKhoa = new BenhAnCommonADO();
            //BenhAnCommonADO _BenhAnRangHamMat = new BenhAnCommonADO();
            //BenhAnCommonADO _BenhAnSanKhoa = new BenhAnCommonADO();
            //BenhAnCommonADO _BenhAnSoSinh = new BenhAnCommonADO();
            //BenhAnCommonADO _BenhAnDieuTriBanNgay = new BenhAnCommonADO();
            //BenhAnCommonADO _BenhAnTaiMuiHong = new BenhAnCommonADO();
            //BenhAnCommonADO _BenhAnTruyenNhiem = new BenhAnCommonADO();
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

            if (_ExamServiceReq != null)
            {
                _BenhAnCommonADO.BacSyDieuTri = _ExamServiceReq.EXECUTE_USERNAME;
                _BenhAnCommonADO.BacSyLamBenhAn = _ExamServiceReq.REQUEST_USERNAME;
                _BenhAnCommonADO.BenhChinh = _ExamServiceReq.ICD_NAME;
                _BenhAnCommonADO.BenhKemTheo = _ExamServiceReq.ICD_TEXT;
                _BenhAnCommonADO.CacXetNghiemCanLamSangCanLam = sereServCLSs != null ? String.Join(";", sereServCLSs.Select(o => o.TDL_SERVICE_NAME).ToArray()) : "";
                _BenhAnCommonADO.CacXetNghiemCanLamSangCanLam = _BenhAnCommonADO.CacXetNghiemCanLamSangCanLam.Length > 2048 ? _BenhAnCommonADO.CacXetNghiemCanLamSangCanLam.Substring(0, 2048) : _BenhAnCommonADO.CacXetNghiemCanLamSangCanLam;
                _BenhAnCommonADO.CoXuongKhop = _ExamServiceReq.PART_EXAM_MUSCLE_BONE;

                _BenhAnCommonADO.HoHap = _ExamServiceReq.PART_EXAM_RESPIRATORY;
                _BenhAnCommonADO.HuongDieuTri = _ExamServiceReq.NEXT_TREATMENT_INSTRUCTION;
                _BenhAnCommonADO.HuongDieuTriVaCacCheDoTiepTheo = _ExamServiceReq.NEXT_TREATMENT_INSTRUCTION;
                _BenhAnCommonADO.Khac_CacCoQuan = _ExamServiceReq.PART_EXAM;
                _BenhAnCommonADO.LyDoVaoVien = _ExamServiceReq.HOSPITALIZATION_REASON;
                _BenhAnCommonADO.MaQuanLy = Inventec.Common.TypeConvert.Parse.ToDecimal(_ExamServiceReq.TREATMENT_CODE);
                _BenhAnCommonADO.Mat = _ExamServiceReq.PART_EXAM_EYE;
                _BenhAnCommonADO.NgayKhamBenh = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(_ExamServiceReq.INTRUCTION_DATE) ?? DateTime.Now;
                _BenhAnCommonADO.NgayTongKet = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(_ExamServiceReq.FINISH_TIME ?? 0) ?? DateTime.Now;
                _BenhAnCommonADO.NguoiGiaoHoSo = _ExamServiceReq.REQUEST_USERNAME;
                _BenhAnCommonADO.NguoiNhanHoSo = _ExamServiceReq.EXECUTE_USERNAME;
                _BenhAnCommonADO.PhanBiet = "";
                _BenhAnCommonADO.PhuongPhapDieuTri = _ExamServiceReq.TREATMENT_INSTRUCTION;
                _BenhAnCommonADO.QuaTrinhBenhLy = _ExamServiceReq.PATHOLOGICAL_PROCESS;
                _BenhAnCommonADO.QuaTrinhBenhLyVaDienBien = _ExamServiceReq.NOTE;
                _BenhAnCommonADO.RangHamMat = _ExamServiceReq.PART_EXAM_STOMATOLOGY;
                _BenhAnCommonADO.TaiMuiHong = String.IsNullOrEmpty(_ExamServiceReq.PART_EXAM_ENT) ? _ExamServiceReq.PART_EXAM_EAR + " " + _ExamServiceReq.PART_EXAM_NOSE + " " + _ExamServiceReq.PART_EXAM_THROAT : _ExamServiceReq.PART_EXAM_ENT;
                _BenhAnCommonADO.TienLuong = "";
                _BenhAnCommonADO.TienSuBenhBanThan = _ExamServiceReq.PATHOLOGICAL_HISTORY;
                _BenhAnCommonADO.TienSuBenhGiaDinh = _ExamServiceReq.PATHOLOGICAL_HISTORY_FAMILY;
                _BenhAnCommonADO.TieuHoa = _ExamServiceReq.PART_EXAM_DIGESTION;
                _BenhAnCommonADO.TinhTrangNguoiBenhRaVien = _ExamServiceReq.ADVISE;
                _BenhAnCommonADO.ToanThan = _ExamServiceReq.FULL_EXAM;
                _BenhAnCommonADO.TomTatBenhAn = _ExamServiceReq.SUBCLINICAL;
                _BenhAnCommonADO.TomTatKetQuaXetNghiem = "";
                _BenhAnCommonADO.TuanHoan = _ExamServiceReq.PART_EXAM_CIRCULATION;
                _BenhAnCommonADO.ThanKinh = _ExamServiceReq.PART_EXAM_NEUROLOGICAL;
                _BenhAnCommonADO.ThanTietNieuSinhDuc = _ExamServiceReq.PART_EXAM_KIDNEY_UROLOGY;
                _BenhAnCommonADO.VaoNgayThu = Inventec.Common.TypeConvert.Parse.ToInt32(_ExamServiceReq.SICK_DAY > 0 ? _ExamServiceReq.SICK_DAY.ToString() : "0");

                if (_TYpe == LoaiBenhAnEMR.NoiKhoa)
                {
                    #region ----NoiKhoa
                    Inventec.Common.Mapper.DataObjectMapper.Map<BenhAnNoiKhoa>(_BenhAnNoiKhoa, _BenhAnCommonADO);
                    #endregion
                }
                else if (_TYpe == LoaiBenhAnEMR.NgoaiKhoa)
                {
                    #region ----NgoaiKhoa
                    Inventec.Common.Mapper.DataObjectMapper.Map<BenhAnNgoaiKhoa>(_BenhAnNgoaiKhoa, _BenhAnCommonADO);
                    #endregion
                }
                else if (_TYpe == LoaiBenhAnEMR.DaLieu)
                {
                    #region ----DaLieu
                    Inventec.Common.Mapper.DataObjectMapper.Map<BenhAnDaLieu>(_BenhAnDaLieu, _BenhAnCommonADO);
                    #endregion
                }
                else if (_TYpe == LoaiBenhAnEMR.Bong)
                {
                    #region ----Bong
                    Inventec.Common.Mapper.DataObjectMapper.Map<BenhAnBong>(_BenhAnBong, _BenhAnCommonADO);
                    _BenhAnBong.HinhAnhHoacVe = "";
                    _BenhAnBong.PhauThuat = false;
                    _BenhAnBong.SinhDuc = "";
                    _BenhAnBong.ThuThuat = false;
                    _BenhAnBong.TonThuongBong = "";
                    #endregion
                }
                else if (_TYpe == LoaiBenhAnEMR.HuyetHocTruyenMau)
                {
                    #region ----Huyết học truyền máu
                    // base
                    Inventec.Common.Mapper.DataObjectMapper.Map<BenhAnHuyetHocTruyenMau>(_BenhAnHuyetHocTruyenMau, _BenhAnCommonADO);
                    // detail
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
                    #endregion
                }
                else if (_TYpe == LoaiBenhAnEMR.MatBanPhanTruoc)
                {
                    #region ----Mắt_bán phần trước
                    Inventec.Common.Mapper.DataObjectMapper.Map<BenhAnMatBanPhanTruoc>(_BenhAnMatBanPhanTruoc, _BenhAnCommonADO);
                    #endregion
                }
                else if (_TYpe == LoaiBenhAnEMR.MatChanThuong)
                {
                    #region ----Mắt_chấn thương
                    Inventec.Common.Mapper.DataObjectMapper.Map<BenhAnMatChanThuong>(_BenhAnMatChanThuong, _BenhAnCommonADO);
                    #endregion
                }
                else if (_TYpe == LoaiBenhAnEMR.MatDayMat)
                {
                    #region Mắt đáy mắt
                    Inventec.Common.Mapper.DataObjectMapper.Map<BenhAnMatDayMat>(_BenhAnMatDayMat, _BenhAnCommonADO);
                    #endregion
                }
                else if (_TYpe == LoaiBenhAnEMR.MatGloCom)
                {
                    #region mắt glocom
                    Inventec.Common.Mapper.DataObjectMapper.Map<BenhAnMatGlocom>(_BenhAnMatGlocom, _BenhAnCommonADO);
                    #endregion
                }
                else if (_TYpe == LoaiBenhAnEMR.MatLac)
                {
                    #region MatLac
                    Inventec.Common.Mapper.DataObjectMapper.Map<BenhAnMatLac>(_BenhAnMatLac, _BenhAnCommonADO);
                    #endregion
                }
                else if (_TYpe == LoaiBenhAnEMR.MatTreEm)
                {
                    #region MatTreEm
                    Inventec.Common.Mapper.DataObjectMapper.Map<BenhAnMatTreEm>(_BenhAnMatTreEm, _BenhAnCommonADO);
                    #endregion
                }
                else if (_TYpe == LoaiBenhAnEMR.NgoaiTru)
                {
                    #region NgoaiTru
                    Inventec.Common.Mapper.DataObjectMapper.Map<BenhAnNgoaiTru>(_BenhAnNgoaiTru, _BenhAnCommonADO);

                    // thong tin rieng
                    _BenhAnNgoaiTru.CacBoPhan = _ExamServiceReq.PART_EXAM;
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
                    Inventec.Common.Mapper.DataObjectMapper.Map<BenhAnNgoaiTru>(_BenhAnNgoaiTruRangHamMat, _BenhAnCommonADO);

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
                    //_BenhAnNgoaiTruRangHamMat.PhauThuat = false;
                    //_BenhAnNgoaiTruRangHamMat.ThuThuat = false;

                    json = Newtonsoft.Json.JsonConvert.SerializeObject(_BenhAnNgoaiTruRangHamMat);
                    #endregion
                }
                else if (_TYpe == LoaiBenhAnEMR.NgoaiTruTaiMuiHong)
                {
                    #region NgoaiTruTaiMuiHong
                    Inventec.Common.Mapper.DataObjectMapper.Map<BenhAnNgoaiTru>(_BenhAnNgoaiTruTaiMuiHong, _BenhAnCommonADO);

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
                    //_BenhAnNgoaiTruRangHamMat.PhauThuat = false;
                    //_BenhAnNgoaiTruRangHamMat.ThuThuat = false;
                    json = Newtonsoft.Json.JsonConvert.SerializeObject(_BenhAnNgoaiTruTaiMuiHong);
                    #endregion
                }
                else if (_TYpe == LoaiBenhAnEMR.NgoaiTruYHCT)
                {
                    #region NgoaiTruYHCT
                    Inventec.Common.Mapper.DataObjectMapper.Map<BenhAnNgoaiTru>(_BenhAnNgoaiTruYHCT, _BenhAnCommonADO);

                    // thong tin rieng    
                    _BenhAnNgoaiTruYHCT.ThoiGianDieuTri = (int)(this.currentTreatment.TREATMENT_DAY_COUNT ?? 0);
                    _BenhAnNgoaiTruYHCT.TinhTrangNguoiBenhKhiRavien = this.currentTreatment.PATIENT_CONDITION;

                    _BenhAnNgoaiTruYHCT.KetQuaDieuTriID = _ThongTinDieuTri.KetQuaDieuTri.HasValue ? (int)_ThongTinDieuTri.KetQuaDieuTri : 0;
                    _BenhAnNgoaiTruYHCT.ThoiGianDieuTriTuNgay = new DateTime();
                    _BenhAnNgoaiTruYHCT.ThoiGianDieuTriDenNgay = new DateTime();
                    _BenhAnNgoaiTruYHCT.ChanDoanRaVienTheoYHHD = "";
                    _BenhAnNgoaiTruYHCT.ChanDoanRaVienTheoYHCT = "";
                    _BenhAnNgoaiTruYHCT.PhuongPhapDieuTriTheoYHCT = "";
                    _BenhAnNgoaiTruYHCT.PhuongPhapDieuTriTheoYHHD = "";
                    _BenhAnNgoaiTruYHCT.ChanDoanVaoVienTheoYHHD = "";
                    _BenhAnNgoaiTruYHCT.ChanDoanVaoVienTheoYHCT = "";
                    _BenhAnNgoaiTruYHCT.KetQuaXetNghiemCanLamSang = "";
                    _BenhAnNgoaiTruYHCT.CheDoHoLyTaiNha = "";
                    _BenhAnNgoaiTruYHCT.CheDoDinhDuongTaiNha = "";
                    _BenhAnNgoaiTruYHCT.DieuTriKetHopVoiYHHD_Text = "";
                    _BenhAnNgoaiTruYHCT.DieuTriKetHopVoiYHHD = false;
                    _BenhAnNgoaiTruYHCT.DieuTriYHCT = false;
                    _BenhAnNgoaiTruYHCT.PhuongHuyet = "";
                    _BenhAnNgoaiTruYHCT.PhuongThuoc = "";
                    _BenhAnNgoaiTruYHCT.PhapDieuTri = "";
                    _BenhAnNgoaiTruYHCT.NguyenNhan = "";
                    _BenhAnNgoaiTruYHCT.TangPhuKinhLac = "";
                    _BenhAnNgoaiTruYHCT.BatCuong = "";
                    _BenhAnNgoaiTruYHCT.BietDanh = "";
                    _BenhAnNgoaiTruYHCT.BienPhapLuanTri = "";
                    _BenhAnNgoaiTruYHCT.TomTatTuChan = "";
                    _BenhAnNgoaiTruYHCT.MachTayPhai = "";
                    _BenhAnNgoaiTruYHCT.MachTayTrai = "";
                    _BenhAnNgoaiTruYHCT.MoTa_XucChan = "";
                    _BenhAnNgoaiTruYHCT.MoTa_VaanChan = "";
                    _BenhAnNgoaiTruYHCT.MoTa_VanChan = "";
                    _BenhAnNgoaiTruYHCT.MoTa_VongChan = "";
                    _BenhAnNgoaiTruYHCT.TomTatKetQuaCanLamSang = "";
                    _BenhAnNgoaiTruYHCT.CacBoPhan = "";
                    _BenhAnNgoaiTruYHCT.BenhSu = "";
                    json = Newtonsoft.Json.JsonConvert.SerializeObject(_BenhAnNgoaiTruYHCT);
                    #endregion
                }
                else if (_TYpe == LoaiBenhAnEMR.DieuTriBanNgay)
                {
                    #region DieuTriBanNgay
                    Inventec.Common.Mapper.DataObjectMapper.Map<BenhAnDieuTriBanNgay>(_BenhAnDieuTriBanNgay, _BenhAnCommonADO);
                    _BenhAnDieuTriBanNgay.TruongKhoa = "";
                    #endregion
                }
                else if (_TYpe == LoaiBenhAnEMR.SanKhoa)
                {
                    #region SanKhoa
                    Inventec.Common.Mapper.DataObjectMapper.Map<BenhAnSanKhoa>(_BenhAnSanKhoa, _BenhAnCommonADO);
                    //         public string AmDao { get; set; }
                    //public string AmHo { get; set; }
                    //public string Apgar_1 { get; set; }
                    //public string Apgar_10 { get; set; }
                    //public string Apgar_5 { get; set; }
                    //public string BanhRau { get; set; }
                    //public DateTime BatDauChuyenDaTu { get; set; }
                    //public int BatDauThayKinhNam { get; set; }
                    //public bool BienChung { get; set; }
                    //public string BienChuyen { get; set; }
                    //public bool BungCoSeoPhauThuatCu { get; set; }
                    //public string CachSoRau { get; set; }
                    //public double CanNang { get; set; }
                    //public double CanNangSoRau { get; set; }
                    //public double Cao { get; set; }
                    //public string ChanDoanKhiVaoKhoa { get; set; }
                    //public double ChieuCaoTC { get; set; }
                    //public double ChiSoBishop { get; set; }
                    //public string ChuanDoanSauPhauThuat { get; set; }
                    //public string ChuanDoanTruocPhauThuat { get; set; }
                    //public string ChucDanh { get; set; }
                    //public int ChuKy { get; set; }
                    //public double CoChayMauSauSo { get; set; }
                    //public bool CoHauMon { get; set; }
                    //public string ConCoTC { get; set; }
                    //public string CoTuCung { get; set; }
                    //public int CoTuCungID { get; set; }
                    //public string CuTheDiTatBamSinh { get; set; }
                    //public string DaNiemMac { get; set; }
                    //public bool? DaThai { get; set; }
                    //public bool DauBung { get; set; }
                    //public string DauHieuLucDau { get; set; }
                    //public int DoLotID { get; set; }
                    //public bool? DonThai { get; set; }
                    //public int DuocTiem { get; set; }
                    //public string DuongKinhNhoHaVe { get; set; }
                    //public int HetKinhNam { get; set; }
                    //public string HinhDangTuCung { get; set; }
                    //public string HuyetAp { get; set; }
                    //public string KhamThaiTai { get; set; }
                    //public string KhiVaoKhoa { get; set; }
                    //public bool KiemSoatTuCung { get; set; }
                    //public string KieuThe { get; set; }
                    //public DateTime KinhCuoiCungDenNgay { get; set; }
                    //public DateTime KinhCuoiCungTuNgay { get; set; }
                    //public int KinhLanCuoiNgay { get; set; }
                    //public int LayChongNam { get; set; }
                    //public string LuongKinh { get; set; }
                    //public double LuongMauMat { get; set; }
                    //public int LyDoBienChung { get; set; }
                    //public string LyDoCanThiep { get; set; }
                    //public string Mach { get; set; }
                    //public string MatMang { get; set; }
                    //public string MatMui { get; set; }
                    //public string MauSacNuocOi { get; set; }
                    //public int NgayThai { get; set; }
                    //public string Ngoi { get; set; }
                    //public string NhietDo { get; set; }
                    //public string NhipTho { get; set; }
                    //public string NhungBenhPhuKhoaDaDieuTri { get; set; }
                    //public string NuocOiNhieuHayIt { get; set; }
                    //public bool OiVoID { get; set; }
                    //public DateTime OiVoLuc { get; set; }
                    //public string PhanPhu { get; set; }
                    //public bool Phu { get; set; }
                    //public string PhuongPhapChinh { get; set; }
                    //public int PhuongPhapDeID { get; set; }
                    //public string PhuongPhapKhauChi { get; set; }
                    //public bool Rau { get; set; }
                    //public bool RauCuonCo { get; set; }
                    //public double RauCuonDai { get; set; }
                    //public bool Sau { get; set; }
                    //public int SoMuiKhau { get; set; }
                    //public int SoNgayThayKinh { get; set; }
                    //public bool TaiBienPhauThuat { get; set; }
                    //public string TangSinhMon { get; set; }
                    //public int TangSinhMonID { get; set; }
                    //public bool TatBamSinh { get; set; }
                    //public string TenNguoiTheoDoi { get; set; }
                    //public string The_KhamTrong { get; set; }
                    //public DateTime ThoiGianDe { get; set; }
                    //public DateTime ThoiGianRauSo { get; set; }
                    //public DateTime ThoiGianVaoBuongDe { get; set; }
                    //public bool TiemPhongUonVan { get; set; }
                    //public int TimThai { get; set; }
                    //public string TinhChatKinhNguyet { get; set; }
                    //public int? TinhTrangOiID { get; set; }
                    //public string TinhTrangSoSinhSauKhiDe { get; set; }
                    //public string ToanTrang { get; set; }
                    //public bool ToanTrang_Phu { get; set; }
                    //public bool Trong { get; set; }
                    //public bool Truoc { get; set; }
                    //public int TuoiHetKinh { get; set; }
                    //public int TuoiLayChong { get; set; }
                    //public int TuoiThai { get; set; }
                    //public int TuoiThayKinh { get; set; }
                    //public string TuThe { get; set; }
                    //public double VongBung { get; set; }
                    //public double VongDau { get; set; }
                    //public string Vu { get; set; }
                    //public string XuLyVaKetQua { get; set; }
                    //public string XuLyVaKetQuaSoRau { get; set; }
                    #endregion
                }
                else if (_TYpe == LoaiBenhAnEMR.RangHamMat)
                {
                    #region RangHamMat
                    Inventec.Common.Mapper.DataObjectMapper.Map<BenhAnRangHamMat>(_BenhAnRangHamMat, _BenhAnCommonADO);
                    //          public string BenhChuyenKhoa { get; set; }
                    //public string DaVaMoDuoiDa { get; set; }
                    //public string HamDuoi_HinhVe { get; set; }
                    //public string HamTrenVaHong_HinhVe { get; set; }
                    //public string Phai_HinhVe { get; set; }
                    //public string PhanLoai_HinhVe { get; set; }
                    //public bool PhauThuat { get; set; }
                    //public string Thang_HinhVe { get; set; }
                    //public bool ThuThuat { get; set; }
                    //public string Trai_HinhVe { get; set; }
                    #endregion
                }
                else if (_TYpe == LoaiBenhAnEMR.PhuKhoa)
                {
                    #region TruyenNhiem
                    Inventec.Common.Mapper.DataObjectMapper.Map<BenhAnPhuKhoa>(_BenhAnPhuKhoa, _BenhAnCommonADO);
                    //           public string AmDao { get; set; }
                    //public string AmHo { get; set; }
                    //public string AmVat { get; set; }
                    //public string CacDauHieuSinhDucThuPhat { get; set; }
                    //public string CacTuiCung { get; set; }
                    //public string ChiSoPara1 { get; set; }
                    //public string ChiSoPara2 { get; set; }
                    //public string ChiSoPara3 { get; set; }
                    //public string ChiSoPara4 { get; set; }
                    //public string CoTuCung { get; set; }
                    //public string Hach { get; set; }
                    //public string MangTrinh { get; set; }
                    //public string MoiBe { get; set; }
                    //public string MoiLon { get; set; }
                    //public string PhanPhu { get; set; }
                    //public bool PhauThuat { get; set; }
                    //public string TangSinhMon { get; set; }
                    //public string ThanTuCung { get; set; }
                    //public bool ThuThuat { get; set; }
                    //public TienSuSanPhuKhoa TienSuSanPhuKhoa { get; set; }
                    //public int TienThaiPara { get; set; }
                    //public string Vu { get; set; }
                    #endregion
                }
                else if (_TYpe == LoaiBenhAnEMR.NoiTruYHCT)
                {
                    #region NoiTruYHCT
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

                    ////          public bool An { get; set; }
                    //public bool An_AnIt { get; set; }
                    //public bool An_AnNhieu { get; set; }
                    //public bool An_AnVaoBungChuong { get; set; }
                    //public bool An_ChanAn { get; set; }
                    //public bool An_DangMieng { get; set; }
                    //public bool An_Khac { get; set; }
                    //public bool An_NhatMieng { get; set; }
                    //public bool An_ThemAn { get; set; }
                    //public bool An_ThichMat { get; set; }
                    //public bool An_ThichNong { get; set; }
                    //public string BatCuong { get; set; }
                    //public string BenhSu { get; set; }
                    //public string BenPhai_TongKhan { get; set; }
                    //public string BenTrai_TongKhan { get; set; }
                    //public string BienPhapLuanTri { get; set; }
                    //public string BietDanh { get; set; }
                    //public bool Bung_Chuong { get; set; }
                    //public bool Bung_CoChuong { get; set; }
                    //public bool Bung_CoHonCuc { get; set; }
                    //public bool Bung_DauCuAn { get; set; }
                    //public bool Bung_DauThienAn { get; set; }
                    //public bool Bung_Khac { get; set; }
                    //public bool Bung_Mem { get; set; }
                    //public bool BungNguc_BonChonKhongYen { get; set; }
                    //public bool BungNguc_DanhTrongNguc { get; set; }
                    //public bool BungNguc_Dau { get; set; }
                    //public bool BungNguc_DauTucCanhSuon { get; set; }
                    //public bool BungNguc_DayTruong { get; set; }
                    //public bool BungNguc_Khac { get; set; }
                    //public bool BungNguc_NgotNgatKhoTho { get; set; }
                    //public bool BungNguc_NongRuot { get; set; }
                    //public bool BungNguc_Soi { get; set; }
                    //public bool BungNguc_Tuc { get; set; }
                    //public bool BungVaUc { get; set; }
                    //public string ChanDoanRaVienTheoYHCT { get; set; }
                    //public string ChanDoanRaVienTheoYHHD { get; set; }
                    //public string ChanDoanVaoVienTheoYHCT { get; set; }
                    //public string ChanDoanVaoVienTheoYHHD { get; set; }
                    //public bool ChanTay { get; set; }
                    //public bool ChatLuoi_Beu { get; set; }
                    //public bool ChatLuoi_BinhThuong { get; set; }
                    //public bool ChatLuoi_Cung { get; set; }
                    //public bool ChatLuoi_GayMong { get; set; }
                    //public bool ChatLuoi_Khac { get; set; }
                    //public bool ChatLuoi_Lech { get; set; }
                    //public bool ChatLuoi_Loet { get; set; }
                    //public bool ChatLuoi_Nut { get; set; }
                    //public bool ChatLuoi_Rut { get; set; }
                    //public bool ChatThai_ChatNon { get; set; }
                    //public bool ChatThai_Dom { get; set; }
                    //public bool ChatThai_Khac { get; set; }
                    //public bool ChatThai_KhiHu { get; set; }
                    //public bool ChatThai_KinhNguyet { get; set; }
                    //public bool ChatThai_NuocTieu { get; set; }
                    //public bool ChatThai_Phan { get; set; }
                    //public bool ChatThaiBieuHienBenhLy { get; set; }
                    //public int CheDoChamSoc { get; set; }
                    //public int CheDoDinhDuong { get; set; }
                    //public bool CoVai_Dau { get; set; }
                    //public bool CoVai_Khac { get; set; }
                    //public bool CoVai_KhoVanDong { get; set; }
                    //public bool CoVai_Moi { get; set; }
                    //public bool CoXuongKhop_CangCung { get; set; }
                    //public bool CoXuongKhop_CoCoAnDau { get; set; }
                    //public bool CoXuongKhop_GanDau { get; set; }
                    //public bool CoXuongKhop_Khac { get; set; }
                    //public bool CoXuongKhop_Mem { get; set; }
                    //public bool CoXuongKhop_SanChac { get; set; }
                    //public bool CoXuongKhop_XuongKhopDau { get; set; }
                    //public bool Da_AnDau { get; set; }
                    //public bool Da_AnLom { get; set; }
                    //public bool Da_BinhThuong { get; set; }
                    //public bool Da_ChanTayLanh { get; set; }
                    //public bool Da_ChanTayNong { get; set; }
                    //public bool Da_CucCung { get; set; }
                    //public bool Da_Khac { get; set; }
                    //public bool Da_Kho { get; set; }
                    //public bool Da_Lanh { get; set; }
                    //public bool Da_Nong { get; set; }
                    //public bool Da_Uot { get; set; }
                    //public bool Daitien_Bi { get; set; }
                    //public bool Daitien_Khac { get; set; }
                    //public bool Daitien_Nhao { get; set; }
                    //public bool Daitien_NhayMui { get; set; }
                    //public bool Daitien_Song { get; set; }
                    //public bool Daitien_Tao { get; set; }
                    //public bool Daitien_ToanNuoc { get; set; }
                    //public bool DaiTieuTien { get; set; }
                    //public bool DauDau_CaDau { get; set; }
                    //public bool DauDau_Cang { get; set; }
                    //public bool DauDau_DiChuyen { get; set; }
                    //public bool DauDau_EAmNhuBiBuocLai { get; set; }
                    //public bool DauDau_MotCho { get; set; }
                    //public bool DauDau_NangDau { get; set; }
                    //public bool DauDau_Nhoi { get; set; }
                    //public bool DauDau_NuaDau { get; set; }
                    //public bool DauMat { get; set; }
                    //public bool DieuKienXuatHienBenh { get; set; }
                    //public bool DieuTriKetHopVoiYHHD { get; set; }
                    //public string DieuTriKetHopVoiYHHD_Text { get; set; }
                    //public bool DieuTriYHCT { get; set; }
                    //public bool DoiHa_Hoi { get; set; }
                    //public bool DoiHa_Hong { get; set; }
                    //public bool DoiHa_Khac { get; set; }
                    //public bool DoiHa_Trang { get; set; }
                    //public bool DoiHa_Vang { get; set; }
                    //public bool HanNhiet_BinhThuong { get; set; }
                    //public bool HanNhiet_Han { get; set; }
                    //public bool HanNhiet_HanNhietVangLai { get; set; }
                    //public bool HanNhiet_Khac { get; set; }
                    //public bool HanNhiet_Khac2 { get; set; }
                    //public bool HanNhiet_Nhiet { get; set; }
                    //public bool HanNhiet_RetRun { get; set; }
                    //public bool HanNhiet_SoLanh { get; set; }
                    //public bool HanNhiet_SoNong { get; set; }
                    //public bool HanNhiet_ThichMat { get; set; }
                    //public bool HanNhiet_ThichNong { get; set; }
                    //public bool HanNhiet_TrongNguoiLanh { get; set; }
                    //public bool HanNhiet_TrongNguoiNong { get; set; }
                    //public bool HinhThai_Beo { get; set; }
                    //public bool HinhThai_CanDoi { get; set; }
                    //public bool HinhThai_Gay { get; set; }
                    //public bool HinhThai_HieuDong { get; set; }
                    //public bool HinhThai_Khac { get; set; }
                    //public bool HinhThai_NamCo { get; set; }
                    //public bool HinhThai_NamDuoi { get; set; }
                    //public bool HinhThai_UaTinh { get; set; }
                    //public bool Ho { get; set; }
                    //public bool Ho_CoDom { get; set; }
                    //public bool Ho_Con { get; set; }
                    //public bool Ho_HoLienTuc { get; set; }
                    //public bool Ho_It { get; set; }
                    //public bool Ho_Khac { get; set; }
                    //public bool Ho_Khan { get; set; }
                    //public bool Ho_Nhieu { get; set; }
                    //public bool HoiTho_BinhThuong { get; set; }
                    //public bool HoiTho_Cham { get; set; }
                    //public bool HoiTho_DutQuang { get; set; }
                    //public bool HoiTho_Gap { get; set; }
                    //public bool HoiTho_Khac { get; set; }
                    //public bool HoiTho_KhoKhe { get; set; }
                    //public bool HoiTho_Manh { get; set; }
                    //public bool HoiTho_Ngan { get; set; }
                    //public bool HoiTho_Rit { get; set; }
                    //public bool HoiTho_Tho { get; set; }
                    //public bool HoiTho_Yeu { get; set; }
                    //public bool Hong_Dau { get; set; }
                    //public bool Hong_Kho { get; set; }
                    //public int KetQuaDieuTriID { get; set; }
                    //public string Khac_DieuTri { get; set; }
                    //public bool KinhNguyet { get; set; }
                    //public bool KinhNguyet_DenSauKy { get; set; }
                    //public bool KinhNguyet_DenTruocKy { get; set; }
                    //public bool KinhNguyet_Khac { get; set; }
                    //public bool KinhNguyet_LucDenTruocLucDenS { get; set; }
                    //public bool KinhNguyet_TacKinh { get; set; }
                    //public bool Lung { get; set; }
                    //public bool Lung_CoCungCo { get; set; }
                    //public bool Lung_Dau { get; set; }
                    //public bool Lung_Khac { get; set; }
                    //public bool Lung_KhoVanDong { get; set; }
                    //public bool MacChan_CoLuc { get; set; }
                    //public bool MacChan_Hoat { get; set; }
                    //public bool MacChan_Huyen { get; set; }
                    //public bool MacChan_Khac { get; set; }
                    //public bool MacChan_Phu { get; set; }
                    //public bool MacChan_Sac { get; set; }
                    //public bool MacChan_Te { get; set; }
                    //public bool MacChan_Tram { get; set; }
                    //public bool MacChan_Tri { get; set; }
                    //public bool MacChan_VoLuc { get; set; }
                    //public bool MachTayPhai_Quan1 { get; set; }
                    //public bool MachTayPhai_Quan2 { get; set; }
                    //public bool MachTayPhai_Quan3 { get; set; }
                    //public bool MachTayPhai_Thon1 { get; set; }
                    //public bool MachTayPhai_Thon2 { get; set; }
                    //public bool MachTayPhai_Thon3 { get; set; }
                    //public bool MachTayPhai_Xich1 { get; set; }
                    //public bool MachTayPhai_Xich2 { get; set; }
                    //public bool MachTayPhai_Xich3 { get; set; }
                    //public bool MachTayTrai_Quan1 { get; set; }
                    //public bool MachTayTrai_Quan2 { get; set; }
                    //public bool MachTayTrai_Quan3 { get; set; }
                    //public bool MachTayTrai_Thon1 { get; set; }
                    //public bool MachTayTrai_Thon2 { get; set; }
                    //public bool MachTayTrai_Thon3 { get; set; }
                    //public bool MachTayTrai_Xich1 { get; set; }
                    //public bool MachTayTrai_Xich2 { get; set; }
                    //public bool MachTayTrai_Xich3 { get; set; }
                    //public bool Mat_HoaMatChongMat { get; set; }
                    //public bool Mat_NhinKhongRo { get; set; }
                    //public bool MoHoi_BinhThuong { get; set; }
                    //public bool MoHoi_DaoHan { get; set; }
                    //public bool MoHoi_It { get; set; }
                    //public bool MoHoi_Khac { get; set; }
                    //public bool MoHoi_KhacXucChan { get; set; }
                    //public bool MoHoi_KhongCoMoHoi { get; set; }
                    //public bool MoHoi_Nhieu { get; set; }
                    //public bool MoHoi_TayChan { get; set; }
                    //public bool MoHoi_ToanThan { get; set; }
                    //public bool MoHoi_Tran { get; set; }
                    //public bool MoHoi_TuHan { get; set; }
                    //public string MoTaChiTietCoQuanBenhLy { get; set; }
                    //public string MoTaThietChan { get; set; }
                    //public string MoTaVaanChan { get; set; }
                    //public string MoTaVanChan { get; set; }
                    //public string MoTaVongChan { get; set; }
                    //public bool Mui_ChayMauCam { get; set; }
                    //public bool Mui_ChayNuoc { get; set; }
                    //public bool Mui_Dau { get; set; }
                    //public bool Mui_Ngat { get; set; }
                    //public bool MuiCoThe { get; set; }
                    //public bool MuiCoThe_Chua { get; set; }
                    //public bool MuiCoThe_Hoi { get; set; }
                    //public bool MuiCoThe_Khac { get; set; }
                    //public bool MuiCoThe_Kham { get; set; }
                    //public bool MuiCoThe_Tanh { get; set; }
                    //public bool MuiCoThe_Thoi { get; set; }
                    //public bool Nac { get; set; }
                    //public bool Nam_DiTinh { get; set; }
                    //public bool Nam_HoatTinh { get; set; }
                    //public bool Nam_LanhTinh { get; set; }
                    //public bool Nam_LietDuong { get; set; }
                    //public bool Nam_MongTinh { get; set; }
                    //public bool Nam_YeuKhiGiaoHop { get; set; }
                    //public bool Ngu { get; set; }
                    //public bool Ngu_DaySom { get; set; }
                    //public bool Ngu_HayMo { get; set; }
                    //public bool Ngu_HayTinh { get; set; }
                    //public bool Ngu_Khac { get; set; }
                    //public bool Ngu_KhoVaoGiacNgu { get; set; }
                    //public string NguyenNhan { get; set; }
                    //public bool Nu_Khac { get; set; }
                    //public bool Nu_KhongThuThai { get; set; }
                    //public bool Nu_SayThai_DongThai { get; set; }
                    //public bool Nu_SayThaiLienTiep { get; set; }
                    //public bool O { get; set; }
                    //public string PhapDieuTri { get; set; }
                    //public string PhuongHuyet { get; set; }
                    //public string PhuongPhapDieuTriTheoYHCT { get; set; }
                    //public string PhuongPhapDieuTriTheoYHHD { get; set; }
                    //public string PhuongThuoc { get; set; }
                    //public bool ReuLuoi_Bong { get; set; }
                    //public bool ReuLuoi_Co { get; set; }
                    //public bool ReuLuoi_Day { get; set; }
                    //public bool ReuLuoi_Den { get; set; }
                    //public bool ReuLuoi_Dinh { get; set; }
                    //public bool ReuLuoi_Khac { get; set; }
                    //public bool ReuLuoi_Kho { get; set; }
                    //public bool ReuLuoi_Khong { get; set; }
                    //public bool ReuLuoi_Mong { get; set; }
                    //public bool ReuLuoi_Trang { get; set; }
                    //public bool ReuLuoi_Uot { get; set; }
                    //public bool ReuLuoi_Vang { get; set; }
                    //public bool RoiHanKhaNangSinhDuc { get; set; }
                    //public bool Sac_BechTrang { get; set; }
                    //public bool Sac_BinhThuong { get; set; }
                    //public bool Sac_Den { get; set; }
                    //public bool Sac_Do { get; set; }
                    //public bool Sac_Khac { get; set; }
                    //public bool Sac_Vang { get; set; }
                    //public bool Sac_Xanh { get; set; }
                    //public bool SacLuoi_DamUHuyet { get; set; }
                    //public bool SacLuoi_Do { get; set; }
                    //public bool SacLuoi_DoSam { get; set; }
                    //public bool SacLuoi_Hong { get; set; }
                    //public bool SacLuoi_Khac { get; set; }
                    //public bool SacLuoi_Kho { get; set; }
                    //public bool SacLuoi_Nhot { get; set; }
                    //public bool SacLuoi_Nhuan { get; set; }
                    //public bool SacLuoi_XanhTim { get; set; }
                    //public bool Tai_ { get; set; }
                    //public bool Tai_Dau { get; set; }
                    //public bool Tai_Diec { get; set; }
                    //public bool Tai_Nang { get; set; }
                    //public bool Tai_U { get; set; }
                    //public string TangPhuKinhLac { get; set; }
                    //public bool Than_ConThan { get; set; }
                    //public bool Than_Khac { get; set; }
                    //public bool Than_KhongConThan { get; set; }
                    //public bool ThongKinh_DauSauKy { get; set; }
                    //public bool ThongKinh_DauTrongKy { get; set; }
                    //public bool ThongKinh_DauTruocKy { get; set; }
                    //public bool ThongKinh_Khac { get; set; }
                    //public bool TiengNoi_BinhThuong { get; set; }
                    //public bool TiengNoi_DutQuang { get; set; }
                    //public bool TiengNoi_Khac { get; set; }
                    //public bool TiengNoi_Khan { get; set; }
                    //public bool TiengNoi_Mat { get; set; }
                    //public bool TiengNoi_Ngong { get; set; }
                    //public bool TiengNoi_Nho { get; set; }
                    //public bool TiengNoi_To { get; set; }
                    //public bool Tieutien_Bi { get; set; }
                    //public bool Tieutien_Buot { get; set; }
                    //public bool Tieutien_Dat { get; set; }
                    //public bool Tieutien_Do { get; set; }
                    //public bool Tieutien_Duc { get; set; }
                    //public bool Tieutien_Khac { get; set; }
                    //public bool Tieutien_KhongTuChu { get; set; }
                    //public bool Tieutien_Vang { get; set; }
                    //public string TinhTrangNguoiBenhKhiRavien { get; set; }
                    //public string TomTatTuChan { get; set; }
                    //public bool Trach_Khac { get; set; }
                    //public bool Trach_Kho { get; set; }
                    //public bool Trach_TuoiNhuan { get; set; }
                    //public bool Uong { get; set; }
                    //public bool Uong_AmNong { get; set; }
                    //public bool Uong_It { get; set; }
                    //public bool Uong_Khac { get; set; }
                    //public bool Uong_Mat { get; set; }
                    //public bool Uong_Nhieu { get; set; }
                    //public string XoaBopBamHuyet { get; set; }
                    #endregion
                }
                else if (_TYpe == LoaiBenhAnEMR.PhucHoiChucNang)
                {
                    #region PhucHoiChucNang
                    Inventec.Common.Mapper.DataObjectMapper.Map<BenhAnPhucHoiChucNang>(_BenhAnPhucHoiChucNang, _BenhAnCommonADO);
                    //           public int AnUong { get; set; }
                    //public int BatCoThu { get; set; }
                    //public string CacKhuyetTatDacBiet { get; set; }
                    //public string CacVanDeKhiemkhuyet { get; set; }
                    //public string CamGiac { get; set; }
                    //public string CanDoiCacBoPhan { get; set; }
                    //public int ChaiToc { get; set; }
                    //public string CoDuocThu { get; set; }
                    //public string CoTron { get; set; }
                    //public int DanhRang { get; set; }
                    //public string DaVaMoDuoiDa { get; set; }
                    //public int DiVeSinh { get; set; }
                    //public int DungCuTroGiup { get; set; }
                    //public int DungNgoi { get; set; }
                    //public string HinhTheCacKhop { get; set; }
                    //public string HinhVeTonThuongKhiVaoVien { get; set; }
                    //public string Khac_ChucNangSinhHoat { get; set; }
                    //public int KhaNangDiChuyen { get; set; }
                    //public string LongNguc { get; set; }
                    //public int MacQuanAo { get; set; }
                    //public string MucDichDieuTri { get; set; }
                    //public int NamNguaNgoi { get; set; }
                    //public int NamNguaSap { get; set; }
                    //public string NhipTim { get; set; }
                    //public string PhanXa { get; set; }
                    //public bool PhauThuat { get; set; }
                    //public string RoiLoanChucNang { get; set; }
                    //public string RoiLoanChucNang_Co { get; set; }
                    //public string RoiLoanChucNang_CotSong { get; set; }
                    //public string RoiLoanChucNang_TieuHoa { get; set; }
                    //public string RoiLoanChucNangHoHap { get; set; }
                    //public string RoiLoanChucNangTimMach { get; set; }
                    //public string Schober { get; set; }
                    //public string Stibor { get; set; }
                    //public int Tam { get; set; }
                    //public string TamHoatDongCacKhopLucRaVien { get; set; }
                    //public string TamHoatDongCacKhopLucVaoVien { get; set; }
                    //public string ThanKinhSoNao { get; set; }
                    //public string TheTichKhi { get; set; }
                    //public bool ThuThuat { get; set; }
                    //public string TiengTim { get; set; }
                    //public string TinhTrangBenhLy_Co { get; set; }
                    //public string TinhTrangBenhLy_CotSong { get; set; }
                    //public string TinhTrangBenhLy_HoHap { get; set; }
                    //public string TinhTrangBenhLy_TieuHoa { get; set; }
                    //public int TuSanDungLen { get; set; }
                    //public string VanDong { get; set; }
                    #endregion
                }
                else if (_TYpe == LoaiBenhAnEMR.TruyenNhiem)
                {
                    #region TruyenNhiem
                    Inventec.Common.Mapper.DataObjectMapper.Map<BenhAnTruyenNhiem>(_BenhAnTruyenNhiem, _BenhAnCommonADO);
                    _BenhAnTruyenNhiem.BenhCapTinhDangLuuHanh = "";
                    _BenhAnTruyenNhiem.DaNoiONoiNao = "";
                    _BenhAnTruyenNhiem.DichTe = "";
                    _BenhAnTruyenNhiem.MoiSinh = "";
                    #endregion
                }
                else if (_TYpe == LoaiBenhAnEMR.TaiMuiHong)
                {
                    #region DieuTriBanNgay
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
                    //_BenhAnTaiMuiHong.PhauThuat = "";
                    _BenhAnTaiMuiHong.ThanhQuan_HinhAnh = "";
                    //_BenhAnTaiMuiHong.ThuThuat = "";
                    #endregion
                }
                else if (_TYpe == LoaiBenhAnEMR.NhiKhoa)
                {
                    #region NhiKhoa
                    Inventec.Common.Mapper.DataObjectMapper.Map<BenhAnNhiKhoa>(_BenhAnNhiKhoa, _BenhAnCommonADO);
                    //_BenhAnNhiKhoa.BachHau = "";
                    //_BenhAnNhiKhoa.BaiLiet = "";
                    //_BenhAnNhiKhoa.BenhKhacDuocTiemChungKhac = "";
                    //_BenhAnNhiKhoa.CacBenhLyKhac = "";
                    //_BenhAnNhiKhoa.CaiSuaThang = "";
                    //_BenhAnNhiKhoa.CanNangLucSinh = "";
                    //_BenhAnNhiKhoa.ChamSocID = null;
                    //_BenhAnNhiKhoa.ChieuCao = "";
                    //_BenhAnNhiKhoa.ConThuMay = "";
                    //_BenhAnNhiKhoa.DiTatBamSinh = "";
                    //_BenhAnNhiKhoa.DiTatBamSinh_Text = "";
                    //_BenhAnNhiKhoa.HoGa = "";
                    //_BenhAnNhiKhoa.Lao = "";
                    //_BenhAnNhiKhoa.NuoiDuongID = "";
                    //_BenhAnNhiKhoa.PhatTrienTinhThan = "";
                    //_BenhAnNhiKhoa.PhatTrienVanDong = "";
                    //_BenhAnNhiKhoa.Soi = "";
                    //_BenhAnNhiKhoa.TiemChungKhac = "";
                    //_BenhAnNhiKhoa.TienThaiPara = "";
                    //_BenhAnNhiKhoa.TinhTrangKhiSinhID = null;
                    //_BenhAnNhiKhoa.UonVan = "";
                    //_BenhAnNhiKhoa.VongDau = "";
                    //_BenhAnNhiKhoa.VongNguc = "";
                    #endregion
                }
                else if (_TYpe == LoaiBenhAnEMR.Tim)
                {
                    #region NhiKhoa
                    Inventec.Common.Mapper.DataObjectMapper.Map<BenhAnNhiKhoa>(_BenhAnTim, _BenhAnCommonADO);
                    #endregion
                }
            }
            Inventec.Common.Logging.LogSystem.Debug("LoadData_ERMv3. 4");
            #region Call Show ERM.Dll
            ERMADO _ERMADO = new ERMADO();
            _ERMADO.KyDienTu_ApplicationCode = GlobalVariables.APPLICATION_CODE;
            _ERMADO.KyDienTu_DiaChiACS = HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_ACS;
            _ERMADO.KyDienTu_DiaChiEMR = HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_EMR;
            _ERMADO.KyDienTu_DiaChiThuVienKy = HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_FSS;
            _ERMADO.KyDienTu_TREATMENT_CODE = currentTreatment.TREATMENT_CODE;

            _ERMADO._HanhChinhBenhNhan_s = new HanhChinhBenhNhan();
            _ERMADO._ThongTinDieuTri_s = new ThongTinDieuTri();
            _ERMADO._LoaiBenhAnEMR_s = new LoaiBenhAnEMR();
            _ERMADO._LoaiBenhAnEMR_s = _TYpe;

            // gán thông tin hành chính
            _ERMADO._HanhChinhBenhNhan_s = _HanhChinhBenhNhan;
            _ERMADO._ThongTinDieuTri_s = _ThongTinDieuTri;

            //Gán dữ liệu vào SDO , tương đương với mỗi loại bệnh án
            if (_TYpe == LoaiBenhAnEMR.Bong)
            {
                _ERMADO._BenhAnBong_s = new BenhAnBong();
                _ERMADO._BenhAnBong_s = _BenhAnBong;
            }
            else if (_TYpe == LoaiBenhAnEMR.DaLieu)
            {
                _ERMADO._BenhAnDaLieu_s = new BenhAnDaLieu();
                _ERMADO._BenhAnDaLieu_s = _BenhAnDaLieu;
            }
            else if (_TYpe == LoaiBenhAnEMR.HuyetHocTruyenMau)
            {
                _ERMADO._BenhAnHuyetHocTruyenMau_s = new BenhAnHuyetHocTruyenMau();
                _ERMADO._BenhAnHuyetHocTruyenMau_s = _BenhAnHuyetHocTruyenMau;
            }
            else if (_TYpe == LoaiBenhAnEMR.MatBanPhanTruoc)
            {
                _ERMADO._BenhAnMatBanPhanTruoc_s = new BenhAnMatBanPhanTruoc();
                _ERMADO._BenhAnMatBanPhanTruoc_s = _BenhAnMatBanPhanTruoc;
            }
            else if (_TYpe == LoaiBenhAnEMR.NoiKhoa)
            {
                _ERMADO._BenhAnNoiKhoa_s = new BenhAnNoiKhoa();
                _ERMADO._BenhAnNoiKhoa_s = _BenhAnNoiKhoa;
            }
            else if (_TYpe == LoaiBenhAnEMR.NgoaiKhoa)
            {
                _ERMADO._BenhAnNgoaiKhoa_s = new BenhAnNgoaiKhoa();
                _ERMADO._BenhAnNgoaiKhoa_s = _BenhAnNgoaiKhoa;
            }
            else if (_TYpe == LoaiBenhAnEMR.MatChanThuong)
            {
                _ERMADO._BenhAnMatChanThuong_s = new BenhAnMatChanThuong();
                _ERMADO._BenhAnMatChanThuong_s = _BenhAnMatChanThuong;
            }
            else if (_TYpe == LoaiBenhAnEMR.MatDayMat)
            {
                _ERMADO._BenhAnMatDayMat_s = new BenhAnMatDayMat();
                _ERMADO._BenhAnMatDayMat_s = _BenhAnMatDayMat;
            }
            else if (_TYpe == LoaiBenhAnEMR.MatGloCom)
            {
                _ERMADO._BenhAnMatGlocom_s = new BenhAnMatGlocom();
                _ERMADO._BenhAnMatGlocom_s = _BenhAnMatGlocom;
            }
            else if (_TYpe == LoaiBenhAnEMR.MatLac)
            {
                _ERMADO._BenhAnMatLac_s = new BenhAnMatLac();
                _ERMADO._BenhAnMatLac_s = _BenhAnMatLac;
            }
            else if (_TYpe == LoaiBenhAnEMR.MatTreEm)
            {
                _ERMADO._BenhAnMatTreEm_s = new BenhAnMatTreEm();
                _ERMADO._BenhAnMatTreEm_s = _BenhAnMatTreEm;
            }
            else if (_TYpe == LoaiBenhAnEMR.DieuTriBanNgay)
            {
                _ERMADO._BenhAnDieuTriBanNgay_s = new BenhAnDieuTriBanNgay();
                _ERMADO._BenhAnDieuTriBanNgay_s = _BenhAnDieuTriBanNgay;
            }
            else if (_TYpe == LoaiBenhAnEMR.NgoaiTru)
            {
                _ERMADO._BenhAnNgoaiTru_s = new BenhAnNgoaiTru();
                _ERMADO._BenhAnNgoaiTru_s = _BenhAnNgoaiTru;
            }
            else if (_TYpe == LoaiBenhAnEMR.NhiKhoa)
            {
                _ERMADO._BenhAnNhiKhoa_s = new BenhAnNhiKhoa();
                _ERMADO._BenhAnNhiKhoa_s = _BenhAnNhiKhoa;
            }
            else if (_TYpe == LoaiBenhAnEMR.NoiTruYHCT)
            {
                _ERMADO._BenhAnNoiTruYHCT_s = new BenhAnNoiTruYHCT();
                _ERMADO._BenhAnNoiTruYHCT_s = _BenhAnNoiTruYHCT;
            }
            else if (_TYpe == LoaiBenhAnEMR.PhucHoiChucNang)
            {
                _ERMADO._BenhAnPhucHoiChucNang_s = new BenhAnPhucHoiChucNang();
                _ERMADO._BenhAnPhucHoiChucNang_s = _BenhAnPhucHoiChucNang;
            }
            else if (_TYpe == LoaiBenhAnEMR.PhuKhoa)
            {
                _ERMADO._BenhAnPhuKhoa_s = new BenhAnPhuKhoa();
                _ERMADO._BenhAnPhuKhoa_s = _BenhAnPhuKhoa;
            }
            else if (_TYpe == LoaiBenhAnEMR.RangHamMat)
            {
                _ERMADO._BenhAnRangHamMat_s = new BenhAnRangHamMat();
                _ERMADO._BenhAnRangHamMat_s = _BenhAnRangHamMat;
            }
            else if (_TYpe == LoaiBenhAnEMR.SanKhoa)
            {
                _ERMADO._BenhAnSanKhoa_s = new BenhAnSanKhoa();
                _ERMADO._BenhAnSanKhoa_s = _BenhAnSanKhoa;
            }
            else if (_TYpe == LoaiBenhAnEMR.SoSinh)
            {
                _ERMADO._BenhAnSoSinh_s = new BenhAnSoSinh();
                _ERMADO._BenhAnSoSinh_s = _BenhAnSoSinh;
            }
            else if (_TYpe == LoaiBenhAnEMR.TaiMuiHong)
            {
                _ERMADO._BenhAnTaiMuiHong_s = new BenhAnTaiMuiHong();
                _ERMADO._BenhAnTaiMuiHong_s = _BenhAnTaiMuiHong;
            }
            else if (_TYpe == LoaiBenhAnEMR.TruyenNhiem)
            {
                _ERMADO._BenhAnTruyenNhiem_s = new BenhAnTruyenNhiem();
                _ERMADO._BenhAnTruyenNhiem_s = _BenhAnTruyenNhiem;
            }
            else if (_TYpe == LoaiBenhAnEMR.Tim)
            {
                _ERMADO._BenhAnTim_s = new BenhAnTim();
                _ERMADO._BenhAnTim_s = _BenhAnTim;
            }
            _ERMADO.PhauThuatThuThuat_HIS_s = PhauThuatThuThuat_HISs;
            Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => _ERMADO), _ERMADO));
            string cmdLn = EncodeData(_ERMADO);
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = Application.StartupPath + @"\Integrate\\EMR\\ConnectToEMR.exe";
            startInfo.Arguments = cmdLn;
            Process.Start(startInfo);

            WaitingManager.Hide();
            #endregion
        }

        ChuyenVien GetChuyenVienFromTranspatiForm(long? tranpatiFormId)
        {
            ChuyenVien cv = ChuyenVien.Khac;
            try
            {
                if (tranpatiFormId.HasValue && tranpatiFormId > 0)
                {
                    var tpt = BackendDataWorker.Get<HIS_TRAN_PATI_FORM>().FirstOrDefault(o => o.ID == tranpatiFormId.Value);
                    if (tpt != null)
                    {
                        cv = (tpt.TRAN_PATI_FORM_CODE == "01" || tpt.TRAN_PATI_FORM_CODE == "02") ? ChuyenVien.TuyenDuoi : tpt.TRAN_PATI_FORM_CODE == "03" ? ChuyenVien.TuyenTren : ChuyenVien.Khac;
                    }
                }
            }
            catch { }
            return cv;
        }

        long GetRoomTypeByRoomId(long? roomId)
        {
            long roomTypeId = 0;
            try
            {
                var room = BackendDataWorker.Get<V_HIS_ROOM>().Where(o => o.ID == roomId).FirstOrDefault();
                roomTypeId = room != null ? room.ROOM_TYPE_ID : 0;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return roomTypeId;
        }

        string EncodeData(object data)
        {
            string result = "";

            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(Newtonsoft.Json.JsonConvert.SerializeObject(data));

            result = System.Convert.ToBase64String(plainTextBytes);

            return Inventec.Common.String.StringCompressor.CompressString(result);
        }

        string DecodeData(string data)
        {
            string result = "";
            var base64EncodedBytes = System.Convert.FromBase64String(data);
            result = System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
            return result;
        }
    }

    public class ERMADO
    {
        public string KyDienTu_DiaChiACS = "http://192.168.1.201:81/";
        public string KyDienTu_DiaChiEMR = "http://192.168.1.201:288/";
        public string KyDienTu_DiaChiThuVienKy = "http://192.168.1.201:85/";
        public string KyDienTu_ApplicationCode = "HIS";
        public string KyDienTu_TREATMENT_CODE = "";

        public LoaiBenhAnEMR _LoaiBenhAnEMR_s { get; set; }
        public HanhChinhBenhNhan _HanhChinhBenhNhan_s { get; set; }
        public ThongTinDieuTri _ThongTinDieuTri_s { get; set; }
        public BenhAnBong _BenhAnBong_s { get; set; }
        public BenhAnDaLieu _BenhAnDaLieu_s { get; set; }
        public BenhAnHuyetHocTruyenMau _BenhAnHuyetHocTruyenMau_s { get; set; }
        public BenhAnMatBanPhanTruoc _BenhAnMatBanPhanTruoc_s { get; set; }
        public BenhAnMatChanThuong _BenhAnMatChanThuong_s { get; set; }
        public BenhAnMatDayMat _BenhAnMatDayMat_s { get; set; }
        public BenhAnMatGlocom _BenhAnMatGlocom_s { get; set; }
        public BenhAnMatLac _BenhAnMatLac_s { get; set; }
        public BenhAnMatTreEm _BenhAnMatTreEm_s { get; set; }
        public BenhAnDieuTriBanNgay _BenhAnDieuTriBanNgay_s { get; set; }
        public BenhAnNgoaiKhoa _BenhAnNgoaiKhoa_s { get; set; }
        public BenhAnNgoaiTru _BenhAnNgoaiTru_s { get; set; }
        public BenhAnNgoaiTruRangHamMat _BenhAnNgoaiTruRangHamMat_s { get; set; }
        public BenhAnNgoaiTruTaiMuiHong _BenhAnNgoaiTruTaiMuiHong_s { get; set; }
        public BenhAnNgoaiTruYHCT _BenhAnNgoaiTruYHCT_s { get; set; }
        public BenhAnNhiKhoa _BenhAnNhiKhoa_s { get; set; }
        public BenhAnNoiKhoa _BenhAnNoiKhoa_s { get; set; }
        public BenhAnNoiTruYHCT _BenhAnNoiTruYHCT_s { get; set; }
        public BenhAnPhuKhoa _BenhAnPhuKhoa_s { get; set; }
        public BenhAnPhucHoiChucNang _BenhAnPhucHoiChucNang_s { get; set; }
        public BenhAnRangHamMat _BenhAnRangHamMat_s { get; set; }
        public BenhAnSanKhoa _BenhAnSanKhoa_s { get; set; }
        public BenhAnSoSinh _BenhAnSoSinh_s { get; set; }
        public BenhAnTaiMuiHong _BenhAnTaiMuiHong_s { get; set; }
        public BenhAnTamThan _BenhAnTamThan_s { get; set; }
        public BenhAnTruyenNhiem _BenhAnTruyenNhiem_s { get; set; }
        public BenhAnUngBuou _BenhAnUngBuou_s { get; set; }
        public BenhAnXaPhuong _BenhAnXaPhuong_s { get; set; }
        public List<PhauThuatThuThuat_HIS> PhauThuatThuThuat_HIS_s { get; set; }
        public BenhAnTim _BenhAnTim_s { get; set; }
    }
}

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
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraEditors.ViewInfo;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.UC.Icd.ADO;
using HIS.UC.SecondaryIcd.ADO;
using Inventec.Common.Adapter;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using Inventec.Desktop.Plugins.BedRoomWithIn;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.BedRoomWithIn
{
    public partial class frmBedRoomWithIn : HIS.Desktop.Utility.FormBase
    {
        private void ProcessTreatmentBedRoomSaveClick(object sender, EventArgs e)
        {
            bool success = false;
            CommonParam param = new CommonParam();
            try
            {
                bool vali = true;
                this.positionHandleControl = -1;
                vali = IsValiICD() && vali;
                vali = IsValiICDSub() && vali;
                vali = (!dxValidationProvider2.Validate()) && vali;
                WaitingManager.Show();
                HisTreatmentBedRoomSDO treatmentBedRoomSDO = new MOS.SDO.HisTreatmentBedRoomSDO();
                treatmentBedRoomSDO.ADD_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtLogTime.DateTime) ?? 0;
                treatmentBedRoomSDO.TREATMENT_ID = treatmentId;
                var rs = new BackendAdapter(param).Post<HisTreatmentBedRoomSDO>(HisRequestUriStore.HIS_TREATMENT_BEDROOM_CREATE_SDO, ApiConsumers.MosConsumer, treatmentBedRoomSDO, param);
                WaitingManager.Hide();
                if (rs != null)
                {
                    success = true;
                    btnSave.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            #region Show message
            MessageManager.Show(this.ParentForm, param, success);
            SessionManager.ProcessTokenLost(param);
            #endregion
        }

        bool IsValiICDSub()
        {
            bool result = true;
            try
            {
                result = (bool)subIcdProcessor.GetValidate(ucSecondaryIcd);
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private void ProcessDepartmentTranSaveClick(object sender, EventArgs e)
        {
            bool success = false;
            CommonParam param = new CommonParam();
            try
            {
                if (!layoutControlItem15.Visible)
                    dxValidationProvider2.SetValidationRule(txtPATIENT_CLASSIFY, null);
                bool vali = true;
                this.positionHandleControl = -1;
                vali = IsValiICD() && vali;
                vali = IsValiICDSub() && vali;
                LogSystem.Debug("cboTreatment : " + cboTreatmentType.EditValue + " txt: " + txtTreatmentTypeCode.Text);
                LogSystem.Debug("cboPatientReceive : " + cboPatientReceive.EditValue + " txt: " + txtPatientReceive.Text);
                vali = dxValidationProvider2.Validate() && vali;
                LogSystem.Debug("Validation control. " + vali);
                vali = ValidSereServWithCondition() && vali;
                if (!vali) return;
                WaitingManager.Show();
                HisDepartmentTranReceiveSDO departmentTranReceiveSDO = new MOS.SDO.HisDepartmentTranReceiveSDO();

                UpdateDepartmentTran(ref departmentTranReceiveSDO);

                if (Config.IsUsingBedTemp == "1" && departmentTranReceiveSDO.BedId.HasValue && (!departmentTranReceiveSDO.BedServiceId.HasValue || !departmentTranReceiveSDO.PatientTypeId.HasValue))
                {
                    if (!departmentTranReceiveSDO.BedServiceId.HasValue)
                    {
                        WaitingManager.Hide();
                        DevExpress.XtraEditors.XtraMessageBox.Show(Resources.ResourceLanguageManager.BatBuocChonDichVuGiuongKhiChonGiuong);
                        CboBedService.Focus();
                        CboBedService.ShowPopup();
                    }
                    else if (!departmentTranReceiveSDO.PatientTypeId.HasValue)
                    {
                        WaitingManager.Hide();
                        DevExpress.XtraEditors.XtraMessageBox.Show(Resources.ResourceLanguageManager.BatBuocChonDoiTuongThanhToanKhiChonGiuong);
                        CboPatientType.Focus();
                        CboPatientType.ShowPopup();
                    }

                    return;
                }
                if (!departmentTranReceiveSDO.BedServiceId.HasValue && departmentTranReceiveSDO.PatientTypeId.HasValue)
                {
                    WaitingManager.Hide();
                    DevExpress.XtraEditors.XtraMessageBox.Show(Resources.ResourceLanguageManager.BatBuocChonDoiTuongThanhToanKhiChonDTPT);
                    return;
                }

                //if (departmentTranReceiveSDO.BedId.HasValue && departmentTranReceiveSDO.BedRoomId.HasValue)
                //{
                //    MOS.Filter.HisTreatmentBedRoomView1Filter treatmentBedRoomFilter = new HisTreatmentBedRoomView1Filter();
                //    treatmentBedRoomFilter.BED_ROOM_ID = departmentTranReceiveSDO.BedRoomId.Value;
                //    treatmentBedRoomFilter.BED_ID = departmentTranReceiveSDO.BedId.Value;
                //    treatmentBedRoomFilter.IS_IN_ROOM = true;
                //    var treatmentBedRoomList = new BackendAdapter(new CommonParam()).Get<List<V_HIS_TREATMENT_BED_ROOM_1>>("api/HisTreatmentBedRoom/GetView1", ApiConsumer.ApiConsumers.MosConsumer, treatmentBedRoomFilter, null);
                //    if (treatmentBedRoomList != null && treatmentBedRoomList.Count > 0)
                //    {
                //        WaitingManager.Hide();
                //        string patientName = "";
                //        List<string> patientNameList = treatmentBedRoomList.Select(o => o.TDL_PATIENT_NAME).Distinct().ToList();
                //        if (patientNameList != null && patientNameList.Count > 0)
                //            patientName = String.Join(", ", patientNameList);

                //        string mess = String.Format(Resources.ResourceLanguageManager.GiuongDaCoBenhNhanCoNamGhepKhong, patientName, treatmentBedRoomList.FirstOrDefault().BED_NAME);

                //        if (DevExpress.XtraEditors.XtraMessageBox.Show(mess, "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.No)
                //            return;
                //    }
                //}

                var department = BackendDataWorker.Get<HIS_DEPARTMENT>().FirstOrDefault(o => o.ID == this.currentHisDepartmentTran.DEPARTMENT_ID);
                if (department != null && department.IS_CLINICAL == 1 && departmentTranReceiveSDO.BedRoomId == null && Config.IsRequiredChooseRoom != "1")
                {
                    WaitingManager.Hide();
                    if (DevExpress.XtraEditors.XtraMessageBox.Show(Resources.ResourceLanguageManager.BanChuChonBuongTiepTuc, "Thông báo", MessageBoxButtons.YesNo) == DialogResult.No)
                    {
                        txtBedRoomCode.Focus();
                        return;
                    }
                }

                if (Config.IsRequiredChooseRoom == "1" && departmentTranReceiveSDO.BedRoomId == null)
                {
                    WaitingManager.Hide();
                    XtraMessageBox.Show(Resources.ResourceLanguageManager.BanPhaiChonBuongKhiTiepNhan);
                    txtBedRoomCode.Focus();
                    txtBedRoomCode.SelectAll();
                    return;
                }

                MOS.Filter.HisDepartmentTranLastFilter _TranFilter = new MOS.Filter.HisDepartmentTranLastFilter();
                _TranFilter.TREATMENT_ID = this.treatmentId;
                var dataTran = new BackendAdapter(new CommonParam()).Get<V_HIS_DEPARTMENT_TRAN>("api/HisDepartmentTran/GetLastByTreatmentId", ApiConsumers.MosConsumer, _TranFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, null);

                if (dataTran != null && departmentTranReceiveSDO.InTime <= dataTran.DEPARTMENT_IN_TIME)
                {
                    WaitingManager.Hide();
                    DevExpress.XtraEditors.XtraMessageBox.Show(string.Format(Resources.ResourceLanguageManager.ThoiGianTiepNhanPhaiLonHonThoiGianKhoaTruoc, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(dataTran.DEPARTMENT_IN_TIME ?? 0)), "Thông báo");
                    return;
                }

                if (HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("MOS.HIS_TREATMENT.IN_CODE_FORMAT_OPTION") == "2" && HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("MOS.HIS_TREATMENT.IN_CODE_GENERATE_OPTION") == "2")
                {
                    WaitingManager.Hide();
                    if (CheckInCode(departmentTranReceiveSDO) && DevExpress.XtraEditors.XtraMessageBox.Show(Resources.ResourceLanguageManager.BenhNhanCoDienDieuTriKhac, "Thông báo", MessageBoxButtons.YesNo) == DialogResult.No)
                    {
                        return;
                    }
                }

                #region kiem tra key -- 173789
                var key = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("MOS.HIS_DEPARTMENT_TRAN.DEPARTMENT_IN_TIME_GREATER_THAN_INSTRUCTION_TIME");
                if (key == "2")
                {
                    WaitingManager.Hide();
                    HisSereServFilter serFilter = new HisSereServFilter();
                    serFilter.HAS_EXECUTE = false;
                    serFilter.TDL_REQUEST_DEPARTMENT_ID = this.currentTreatment.LAST_DEPARTMENT_ID;
                    serFilter.TREATMENT_ID = this.treatmentId;
                    var listSereServ = new BackendAdapter(param).Get<List<HIS_SERE_SERV>>("/api/HisSereServ/Get", ApiConsumers.MosConsumer, serFilter, param);
                    listSereServ = listSereServ.Where(s => s.TDL_SERVICE_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__AN
                                                        && s.TDL_SERVICE_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__KHAC
                                                         && s.AMOUNT > 0).ToList();
                    if (listSereServ != null && listSereServ.Count() > 0)
                    {
                        var maxIntructionTime = listSereServ.OrderByDescending(s => s.TDL_INTRUCTION_TIME).FirstOrDefault();
                        if(departmentTranReceiveSDO.InTime <= maxIntructionTime.TDL_INTRUCTION_TIME)
                        {
                            if(MessageBox.Show(this, "Thời gian vào khoa nhỏ hơn thời gian y lệnh "
                                                +Inventec.Common.DateTime.Convert.TimeNumberToTimeString(maxIntructionTime.TDL_INTRUCTION_TIME )
                                                + ". Bạn có muốn tiếp tục?","Cảnh báo",MessageBoxButtons.YesNo) 
                                                == DialogResult.No)
                            {
                                return;
                            }
                        }
                    }
                    Inventec.Common.Logging.LogSystem.Debug("DS dich vu, List Sere Serv: " + LogUtil.TraceData("__listSereServ", listSereServ));
                }
                #endregion
                if (lciTxtInCode.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Always)
                {
                    departmentTranReceiveSDO.InCode = txtInCode.Text.Trim();
                }

                var rs = new BackendAdapter(param).Post<HisDepartmentTranSDO>(HisRequestUriStore.HIS_DEPARTMENT_TRAN_RECEIVE, ApiConsumers.MosConsumer, departmentTranReceiveSDO, param);
                if (rs != null)
                {
                    success = true;
                    var treatmentRs = getTreatment(this.treatmentId);
                    if (treatmentRs != null)
                    {
                        lblSoVaoVien.Text = treatmentRs.IN_CODE;
                        txtInCode.Text = treatmentRs.IN_CODE;
                    }
                    btnSave.Enabled = false;
                }

                WaitingManager.Hide();
                #region Show message
                MessageManager.Show(this.ParentForm, param, success);
                SessionManager.ProcessTokenLost(param);
                #endregion

            }
                
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private HIS_TREATMENT getTreatment(long treatmentID)
        {
            HIS_TREATMENT result = null;
            try
            {
                CommonParam param = new CommonParam();
                HisTreatmentFilter filter = new HisTreatmentFilter();
                filter.ID = treatmentId;
                var treatmentApi = new BackendAdapter(param).Get<List<HIS_TREATMENT>>("api/HisTreatment/Get", ApiConsumers.MosConsumer, filter, param);

                if (treatmentApi != null && treatmentApi.Count > 0)
                {
                    result = treatmentApi.FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return null;
            }
            return result;
        }

        private bool CheckInCode(HisDepartmentTranReceiveSDO data)
        {
            bool rs = false;
            try
            {
                if (this.currentTreatment != null && this.currentTreatment.TDL_TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__KHAM && data != null && (data.TreatmentTypeId == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTNOITRU || data.TreatmentTypeId == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTNGOAITRU)
                    && !string.IsNullOrEmpty(this.currentTreatment.IN_CODE)
                    && data.TreatmentTypeId != this.currentTreatment.IN_TREATMENT_TYPE_ID)
                {
                    rs = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return false;
            }
            return rs;
        }

        void UpdateDepartmentTran(ref HisDepartmentTranReceiveSDO departmentTranReceiveSDO)
        {
            try
            {
                long _InTime = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtLogTime.DateTime) ?? 0;

                departmentTranReceiveSDO = new HisDepartmentTranReceiveSDO();
                //LogTime ==> InTIme
                departmentTranReceiveSDO.InTime = Inventec.Common.TypeConvert.Parse.ToInt64(dtLogTime.DateTime.ToString("yyyyMMddHHmm") + "00");
                //PreviousDepartmentTranId => DepartmentTranId
                departmentTranReceiveSDO.DepartmentTranId = this.currentHisDepartmentTran.ID;
                departmentTranReceiveSDO.DoctorLoginname = this.doctorLoginname;
                departmentTranReceiveSDO.DoctorUsername = this.doctorUsername;
                if (cboBedRoom.EditValue != null)
                {
                    departmentTranReceiveSDO.BedRoomId = Inventec.Common.TypeConvert.Parse.ToInt64((cboBedRoom.EditValue ?? "").ToString());
                }

                //departmentTranReceiveSDO.BedRoomId = (cboBedRoom.EditValue != null) ? Inventec.Common.TypeConvert.Parse.ToInt64((cboBedRoom.EditValue ?? "").ToString()) : null;
                if (cboTreatmentType.EditValue != null)
                {
                    departmentTranReceiveSDO.TreatmentTypeId = Inventec.Common.TypeConvert.Parse.ToInt64((cboTreatmentType.EditValue ?? "").ToString());
                }
                if (cboTreatmentType.EditValue != null)
                {
                    departmentTranReceiveSDO.PatyAlterPatientTypeId = Inventec.Common.TypeConvert.Parse.ToInt64((cboPatientReceive.EditValue ?? "").ToString());
                }
                departmentTranReceiveSDO.DepartmentId = HIS.Desktop.LocalStorage.LocalData.WorkPlace.WorkPlaceSDO.FirstOrDefault(p => p.RoomId == this.currentModule.RoomId).DepartmentId;
                departmentTranReceiveSDO.RequestRoomId = this.currentModule.RoomId;

                IcdInputADO OjecIcd = (IcdInputADO)icdProcessor.GetValue(ucIcd);
                departmentTranReceiveSDO.IcdName = OjecIcd != null ? OjecIcd.ICD_NAME : "";
                departmentTranReceiveSDO.IcdCode = OjecIcd != null ? OjecIcd.ICD_CODE : "";

                IcdInputADO icdYhct = (IcdInputADO)icdYhctProcessor.GetValue(ucIcdYhct);
                departmentTranReceiveSDO.TraditionalIcdName = icdYhct != null ? icdYhct.ICD_NAME : "";
                departmentTranReceiveSDO.TraditionalIcdCode = icdYhct != null ? icdYhct.ICD_CODE : "";

                SecondaryIcdDataADO icdSub = (SecondaryIcdDataADO)this.subIcdProcessor.GetValue(this.ucSecondaryIcd);
                departmentTranReceiveSDO.IcdSubCode = icdSub != null ? icdSub.ICD_SUB_CODE : "";
                departmentTranReceiveSDO.IcdText = icdSub != null ? icdSub.ICD_TEXT : "";

                SecondaryIcdDataADO icdYhctSub = (SecondaryIcdDataADO)this.subIcdYhctProcessor.GetValue(this.ucSecondaryIcdYhct);
                departmentTranReceiveSDO.TraditionalIcdSubCode = icdYhctSub != null ? icdYhctSub.ICD_SUB_CODE : "";
                departmentTranReceiveSDO.TraditionalIcdText = icdYhctSub != null ? icdYhctSub.ICD_TEXT : "";

                if (cboBed.EditValue != null)
                {
                    departmentTranReceiveSDO.BedId = Inventec.Common.TypeConvert.Parse.ToInt64((cboBed.EditValue ?? "").ToString());
                    if (SpNamGhep.EditValue != null)
                    {
                        departmentTranReceiveSDO.ShareCount = (long)SpNamGhep.Value;
                    }
                }

                if (CboBedService.EditValue != null)
                {
                    departmentTranReceiveSDO.BedServiceId = Inventec.Common.TypeConvert.Parse.ToInt64((CboBedService.EditValue ?? "").ToString());
                }

                if (CboPatientType.EditValue != null)
                {
                    departmentTranReceiveSDO.PatientTypeId = Inventec.Common.TypeConvert.Parse.ToInt64((CboPatientType.EditValue ?? "").ToString());
                }

                if (CboPrimaryPatientType.EditValue != null)
                {
                    departmentTranReceiveSDO.PrimaryPatientTypeId = Inventec.Common.TypeConvert.Parse.ToInt64((CboPrimaryPatientType.EditValue ?? "").ToString());
                }
                if (cboPATIENT_CLASSIFY.EditValue != null) 
                {
                    departmentTranReceiveSDO.PatientClassifyId = (long)cboPATIENT_CLASSIFY.EditValue;
                }
                if (cboServiceCondition.EditValue != null)
                    departmentTranReceiveSDO.ServiceConditionId = PatyCondition.SERVICE_CONDITION_ID;
                if(this.lciReasonNt.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Always)
                {
                    departmentTranReceiveSDO.HospitalizeReasonCode = this.REASON_CODE;
                    departmentTranReceiveSDO.HospitalizeReasonName = this.REASON_NAME;
                }
            }

            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}

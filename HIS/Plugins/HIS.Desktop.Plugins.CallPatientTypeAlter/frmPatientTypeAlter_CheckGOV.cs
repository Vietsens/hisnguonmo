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
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using His.Bhyt.InsuranceExpertise;
using His.Bhyt.InsuranceExpertise.LDO;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.CallPatientTypeAlter.Config;
using Inventec.Common.Adapter;
using Inventec.Common.QrCodeBHYT;
using Inventec.Core;
using MOS.SDO;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Common;
using HIS.Desktop.Plugins.Library.CheckHeinGOV;
using HIS.Desktop.Plugins.CallPatientTypeAlter.Resources;

namespace HIS.Desktop.Plugins.CallPatientTypeAlter
{
    public partial class frmPatientTypeAlter : HIS.Desktop.Utility.FormBase
    {
        bool IsRuning = true;
        internal async Task CheckThongTuyen(HeinCardData dataHein)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("CheckThongTuyen__" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => dataHein), dataHein));
                long keyCheck = HisConfigCFG.CheDoTuDongCheckThongTinTheBHYT;
                if (keyCheck > 0 && IsRuning)
                {
                    if (String.IsNullOrEmpty(dataHein.PatientName)
                        || String.IsNullOrEmpty(dataHein.Dob)
                        || String.IsNullOrEmpty(dataHein.HeinCardNumber)
                        )
                    {
                        Inventec.Common.Logging.LogSystem.Info("Khong goi cong BHXH check thong tin the do du lieu truyen vao chua du du lieu bat buoc___" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => dataHein), dataHein));
                        return;
                    }
                    IsRuning = false;
                    Inventec.Common.Logging.LogSystem.Debug("_Bat dau check thong tuyen_");
                    HeinGOVManager heinGOVManager = new HeinGOVManager(ResourceMessage.GoiSangCongBHXHTraVeMaLoi);

                    ResultDataADO = await heinGOVManager.Check(dataHein, null, true, (dataHein.Address != null ? dataHein.Address : ""), dtLogTime.DateTime, true);
                    Inventec.Common.Logging.LogUtil.TraceData("_ResultADO_____", ResultDataADO);
                    Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => dataHein), dataHein));
                    uCMainHein.SetResultDataADOBhyt(ucHein__BHYT, ResultDataADO);
                    //if (uCMainHein != null && ucHein__BHYT != null)
                    //    uCMainHein.FillDataAfterFindQrCode(ucHein__BHYT, dataHein);
                    if (ResultDataADO != null)
                    {
                        bool isNotWrongAddress = true;
                        string maKQ = ResultDataADO.ResultHistoryLDO.maKetQua;
                        string message = "";
                        if (maKQ == "000")
                        {
                            if (currenPatient != null)
                            {
                                HIS_PATIENT patient = GetCurrentpatient(currenPatient);
                                if (string.IsNullOrEmpty(patient.COMMUNE_CODE) || string.IsNullOrEmpty(patient.PROVINCE_CODE) || string.IsNullOrEmpty(patient.DISTRICT_CODE))
                                {
                                    message = "Bệnh nhân thiếu thông tin địa chỉ ";
                                }
                                else
                                {
                                    isNotWrongAddress = false;
                                }
                            }

                        }
                        string thongBao = "";
                        if (maKQ == "060" || maKQ == "061" || maKQ == "070" || maKQ == "051" || maKQ == "052" || maKQ == "053" || maKQ == "050" || isNotWrongAddress)
                        {
                            
                            if(string.IsNullOrEmpty(message)) message = ResultDataADO.ResultHistoryLDO.message;
                            thongBao = message + ". Bạn có muốn sửa thông tin bệnh nhân?";
                            
                            DialogResult drReslt = DevExpress.XtraEditors.XtraMessageBox.Show(thongBao, "Thông báo!", MessageBoxButtons.OKCancel, MessageBoxIcon.Question, DevExpress.Utils.DefaultBoolean.True);
                            if (drReslt == DialogResult.OK)
                            {
                                List<object> listArgs = new List<object>();
                                listArgs.Add(this._HisTreatment.PATIENT_ID);
                                listArgs.Add(this.treatmentId);
                                listArgs.Add((RefeshReference)RefeshTreatment);
                                HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.PatientUpdate", this.module.RoomId, this.module.RoomTypeId, listArgs);
                            }
                            
                            Inventec.Common.Logging.LogSystem.Info("CheckThongTuyen____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => maKQ), maKQ) + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => thongBao), thongBao));
                        }
                        else
                        {                          
                            //Trường hợp tìm kiếm BN theo qrocde & BN có số thẻ bhyt mới, cần tìm kiếm BN theo số thẻ mới này & người dùng chọn lấy thông tin thẻ mới => tìm kiếm Bn theo số thẻ mới
                            if (ResultDataADO.IsShowQuestionWhileChangeHeinTime__Choose || ResultDataADO.IsThongTinNguoiDungThayDoiSoVoiCong__Choose)
                            {
                                dataHein.HeinCardNumber = ResultDataADO.ResultHistoryLDO.maTheMoi ?? dataHein.HeinCardNumber;
                            }
                            dataHein.Address = ResultDataADO.ResultHistoryLDO.diaChi ?? dataHein.Address;
                            dataHein.FineYearMonthDate = ResultDataADO.ResultHistoryLDO.ngayDu5Nam;
                            dataHein.FromDate = ResultDataADO.ResultHistoryLDO.gtTheTu ?? dataHein.FromDate;
                            dataHein.ToDate = ResultDataADO.ResultHistoryLDO.gtTheDen ?? dataHein.ToDate;
                            dataHein.LiveAreaCode = ResultDataADO.ResultHistoryLDO.maKV ?? dataHein.LiveAreaCode;
                            ResultDataADO.IsThongTinNguoiDungThayDoiSoVoiCong__Choose = true;
                            this.CheckTTProcessResultData(dataHein, ResultDataADO);
                        }
                    }
                }
                IsRuning = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private HIS_PATIENT GetCurrentpatient(HIS_PATIENT currenPatient)
        {
            HIS_PATIENT result = new HIS_PATIENT();
            try
            {
                var id = currenPatient.ID;
                HisPatientFilter PatientFilter = new HisPatientFilter();
                PatientFilter.ID = id;
                var rs = new BackendAdapter(new CommonParam()).Get<List<HIS_PATIENT>>("/api/HisPatient/Get", ApiConsumers.MosConsumer, PatientFilter, new CommonParam());
                if(rs != null)
                {
                    result = rs.FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private void RefeshTreatment()
        {
            try
            {
                this._HisTreatment = this.GetSDO(this.treatmentId).SingleOrDefault();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private V_HIS_PATIENT GetPatient(long _treatmentId)
        {
            V_HIS_PATIENT rs = null;
            try
            {
                if (_treatmentId > 0)
                {
                    CommonParam param = new CommonParam();

                    HisTreatmentFilter treatmentFilter = new HisTreatmentFilter();
                    treatmentFilter.ID = _treatmentId;

                    var treatmentRs = new BackendAdapter(param).Get<List<HIS_TREATMENT>>("api/HisTreatment/Get", ApiConsumers.MosConsumer, treatmentFilter, param);

                    if (treatmentRs != null && treatmentRs.Count > 0)
                    {
                        HisPatientViewFilter filter = new HisPatientViewFilter();
                        filter.ID = treatmentRs.FirstOrDefault().PATIENT_ID;

                        var resultApi = new BackendAdapter(param).Get<List<V_HIS_PATIENT>>("api/HisPatient/GetView", ApiConsumers.MosConsumer, filter, param);
                        if (resultApi != null && resultApi.Count > 0)
                        {
                            rs = resultApi.FirstOrDefault();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return null;
            }

            return rs;
        }

        private V_HIS_PATIENT GetPatientById(long _patientId)
        {
            V_HIS_PATIENT rs = null;
            try
            {
                CommonParam param = new CommonParam();

                HisPatientViewFilter filter = new HisPatientViewFilter();
                filter.ID = _patientId;

                var resultApi = new BackendAdapter(param).Get<List<V_HIS_PATIENT>>("api/HisPatient/GetView", ApiConsumers.MosConsumer, filter, param);
                if (resultApi != null && resultApi.Count > 0)
                {
                    rs = resultApi.FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return null;
            }

            return rs;
        }


        /// <summary>
        /// Lấy dữ liệu Bn cũ chính xác theo số thẻ bhyt
        /// </summary>
        /// <returns>List<HisPatientSDO></returns>
        private List<HIS_TREATMENT> GetSDO(long treatmentId)
        {
            try
            {
                CommonParam param = new CommonParam();
                MOS.Filter.HisTreatmentFilter filter = new MOS.Filter.HisTreatmentFilter();
                filter.ID = treatmentId;
                return new BackendAdapter(param).Get<List<HIS_TREATMENT>>("/api/HisTreatment/Get", ApiConsumers.MosConsumer, filter, param);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return null;
        }
        HIS_TREATMENT _HisTreatment { get; set; }
        private async void CheckTT(HeinCardData dataHein)
        {
            try
            {
                if (_HisTreatment == null)
                    _HisTreatment = this.GetSDO(this.treatmentId).SingleOrDefault();

                if (_HisTreatment != null)
                {
                    var patient = GetPatientById(_HisTreatment.PATIENT_ID);

                    dataHein.Gender = (patient != null ? HisToHein(patient.GENDER_ID.ToString()) : "2");
                    dataHein.PatientName = patient.VIR_PATIENT_NAME;
                    if (patient.IS_HAS_NOT_DAY_DOB == 1)
                        dataHein.Dob = patient.DOB.ToString().Substring(0, 4);
                    else
                    {
                        dataHein.Dob = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(patient.DOB);
                        var dobSplit = dataHein.Dob.Split(new String[] { " " }, StringSplitOptions.None);
                        dataHein.Dob = dobSplit[0];
                    }
                }

                if (this.currentTreatmentLogSDO != null
                    && this.currentTreatmentLogSDO.patientTypeAlter != null
                    && this.currentTreatmentLogSDO.patientTypeAlter.HAS_BIRTH_CERTIFICATE == MOS.LibraryHein.Bhyt.HeinHasBirthCertificate.HeinHasBirthCertificateCode.TRUE)
                    return;
                else
                    await this.CheckThongTuyen(dataHein);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn("HIS.Desktop.Plugins.CallPatientTypeAlter/CheckTT:\n" + ex);
            }
        }

    }
}

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
using DevExpress.XtraTab;
using HIS.Desktop.ADO;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.ModuleExt;
using HIS.Desktop.Plugins.SurgServiceReqExecute.Base;
using HIS.Desktop.Plugins.SurgServiceReqExecute.Resources;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.SurgServiceReqExecute
{
    public partial class SurgServiceReqExecuteControl : UserControlBase
    {
        private async void SetIcdFromServiceReq(V_HIS_SERVICE_REQ data)
        {
            try
            {
                FillDataToCboIcd(txtIcdCode1, txtIcd1, cboIcd1, chkIcd1, data.ICD_CODE, this.serviceReq.ICD_NAME);
                FillDataToCboIcd(txtIcdCode2, txtIcd2, cboIcd2, chkIcd2, data.ICD_CODE, this.serviceReq.ICD_NAME);
                FillDataToCboIcd(txtIcdCode3, txtIcd3, cboIcd3, chkIcd3, data.ICD_CODE, this.serviceReq.ICD_NAME);

                if (!string.IsNullOrEmpty(data.ICD_SUB_CODE))
                {
                    txtIcdExtraCode.Text = data.ICD_SUB_CODE;
                }

                if (!string.IsNullOrEmpty(data.ICD_TEXT))
                {
                    txtIcdText.Text = data.ICD_TEXT;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void CallBackLoadSereServLast()
        {
            try
            {
                if (this.sereServLasts != null && this.sereServLasts.Count > 0)
                {
                    if (InvokeRequired)
                    {
                        this.Invoke(new MethodInvoker(delegate
                        {
                            gridControlSereServLast.DataSource = this.sereServLasts;
                        }));
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetDataControlBySereServPttt()
        {
            try
            {
                if (this.sereServPTTT != null)
                {
                    txtMethodCode.Text = this.sereServPTTT.PTTT_METHOD_CODE;
                    cboMethod.EditValue = this.sereServPTTT.PTTT_METHOD_ID;

                    txtPtttGroupCode.Text = this.sereServPTTT.PTTT_GROUP_CODE;
                    cbbPtttGroup.EditValue = this.sereServPTTT.PTTT_GROUP_ID;
                    //txtEmotionlessMethod.Text = this.sereServPTTT.EMOTIONLESS_METHOD_CODE;
                    if (dataEmotionlessMethod.Exists(o => o.ID == this.sereServPTTT.EMOTIONLESS_METHOD_ID))
                    {
                        cbbEmotionlessMethod.EditValue = this.sereServPTTT.EMOTIONLESS_METHOD_ID;
                    }
                    else
                    {
                        cbbEmotionlessMethod.EditValue = null;
                    }

                    txtBlood.Text = Patient.BLOOD_ABO_CODE ?? sereServPTTT.BLOOD_ABO_CODE ?? "";
                    var bloodAbo = dataBloodAbo.FirstOrDefault(o => o.BLOOD_ABO_CODE == (Patient.BLOOD_ABO_CODE ?? sereServPTTT.BLOOD_ABO_CODE ?? ""));
                    if (bloodAbo != null)
                        cbbBlood.EditValue = bloodAbo.ID;
                    else
                        cbbBlood.EditValue = null;
                    txtBloodRh.Text = Patient.BLOOD_RH_CODE ?? sereServPTTT.BLOOD_RH_CODE ?? "";
                    var bloodRh = dataBloodRh.FirstOrDefault(o => o.BLOOD_RH_CODE == (Patient.BLOOD_RH_CODE ?? sereServPTTT.BLOOD_RH_CODE ?? ""));
                    if (bloodRh != null)
                        cbbBloodRh.EditValue = bloodRh.ID;
                    else
                        cbbBloodRh.EditValue = null;

                    txtCondition.Text = this.sereServPTTT.PTTT_CONDITION_CODE;
                    cboCondition.EditValue = this.sereServPTTT.PTTT_CONDITION_ID;
                    txtCatastrophe.Text = this.sereServPTTT.PTTT_CATASTROPHE_CODE;
                    cboCatastrophe.EditValue = this.sereServPTTT.PTTT_CATASTROPHE_ID;
                    txtDeathSurg.Text = this.sereServPTTT.DEATH_WITHIN_CODE;
                    cboDeathSurg.EditValue = this.sereServPTTT.DEATH_WITHIN_ID;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetDefaultCboPTTTGroup(V_HIS_SERE_SERV_5 sereServ)
        {
            try
            {
                if (sereServ != null && cbbPtttGroup.EditValue == null)
                {
                    cbbPtttGroup.Enabled = true;
                    txtPtttGroupCode.Enabled = true;
                    if (sereServ.EKIP_ID == null)
                    {
                        long ptttGroupId = 0;
                        long ptttMethodId = 0;

                        var surgMisuService = lstService.FirstOrDefault(o => o.ID == sereServ.SERVICE_ID);
                        if (surgMisuService != null)
                        {
                            if (surgMisuService.PTTT_GROUP_ID.HasValue)
                            {
                                HIS_PTTT_GROUP ptttGroup = BackendDataWorker.Get<HIS_PTTT_GROUP>().FirstOrDefault(o => o.ID == surgMisuService.PTTT_GROUP_ID);
                                ptttGroupId = ptttGroup.ID;
                                txtPtttGroupCode.Text = ptttGroup.PTTT_GROUP_CODE;
                            }

                            if (surgMisuService.PTTT_METHOD_ID.HasValue)
                            {
                                HIS_PTTT_METHOD ptttMethod = BackendDataWorker.Get<HIS_PTTT_METHOD>().FirstOrDefault(o => o.ID == surgMisuService.PTTT_METHOD_ID);
                                ptttMethodId = ptttMethod.ID;
                                txtMethodCode.Text = ptttMethod.PTTT_METHOD_CODE;
                            }
                        }

                        if (ptttMethodId > 0)
                        {
                            cboMethod.EditValue = ptttMethodId;
                        }

                        if (ptttGroupId > 0)
                        {
                            cbbPtttGroup.EditValue = ptttGroupId;
                            if (Config.HisConfigKeys.CheckPermissonOption == "2" && this.serviceReq.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__PT)
                            {
                                cbbPtttGroup.Enabled = true;
                                txtPtttGroupCode.Enabled = true;
                            }
                            else
                            {
                                cbbPtttGroup.Enabled = false;
                                txtPtttGroupCode.Enabled = false;
                            }
                        }
                        //else
                        //{
                        //    cbbPtttGroup.EditValue = null;
                        //    cbbPtttGroup.Enabled = true;
                        //    txtPtttGroupCode.Enabled = true;
                        //}
                    }
                    else
                    {
                        if (hisSereServPttt != null)
                        {
                            var surgService = lstService.FirstOrDefault(o => o.ID == sereServ.SERVICE_ID);
                            if (surgService != null && surgService.PTTT_GROUP_ID != null)
                            {
                                HIS_PTTT_GROUP ptttGroup = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_PTTT_GROUP>().FirstOrDefault(o => o.ID == surgService.PTTT_GROUP_ID);
                                cbbPtttGroup.EditValue = ptttGroup.ID;
                                txtPtttGroupCode.Text = ptttGroup.PTTT_GROUP_CODE;
                                cbbPtttGroup.Enabled = false;
                                txtPtttGroupCode.Enabled = false;
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
        private void SetDefaultCboICD9CmChinh(V_HIS_SERE_SERV_5 sereServ)
        {
            try
            {
                if (this.serviceReq != null && (this.serviceReq.SERVICE_REQ_STT_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__CXL || this.serviceReq.SERVICE_REQ_STT_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__DXL))
                {
                    if (sereServ != null && cboIcdCmName.EditValue == null)
                    {
                        var surgService = lstService.FirstOrDefault(o => o.ID == sereServ.SERVICE_ID);
                        if (surgService != null && surgService.ICD_CM_ID != null)
                        {
                            HIS_ICD_CM icdCm = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_ICD_CM>().FirstOrDefault(o => o.ID == surgService.ICD_CM_ID);
                            cboIcdCmName.EditValue = icdCm.ID;
                            txtIcdCmCode.Text = icdCm.ICD_CM_CODE;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void SetDefaultCboPTTTGroupOnly(V_HIS_SERE_SERV_5 sereServ)
        {
            try
            {
                if (sereServ != null)
                {
                    if (sereServ.EKIP_ID == null)
                    {
                        long ptttGroupId = 0;

                        var surgMisuService = lstService.FirstOrDefault(o => o.ID == sereServ.SERVICE_ID);
                        if (surgMisuService != null)
                        {
                            if (surgMisuService.PTTT_GROUP_ID.HasValue)
                            {
                                HIS_PTTT_GROUP ptttGroup = BackendDataWorker.Get<HIS_PTTT_GROUP>().FirstOrDefault(o => o.ID == surgMisuService.PTTT_GROUP_ID);
                                ptttGroupId = ptttGroup.ID;
                                txtPtttGroupCode.Text = ptttGroup.PTTT_GROUP_CODE;
                            }

                        }

                        if (ptttGroupId > 0)
                        {
                            cbbPtttGroup.EditValue = ptttGroupId;
                            if (Config.HisConfigKeys.CheckPermissonOption == "2" && this.serviceReq.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__PT)
                            {
                                cbbPtttGroup.Enabled = true;
                                txtPtttGroupCode.Enabled = true;
                            }
                            else
                            {
                                cbbPtttGroup.Enabled = false;
                                txtPtttGroupCode.Enabled = false;
                            }
                        }
                        //else
                        //{
                        //    cbbPtttGroup.EditValue = null;
                        //    cbbPtttGroup.Enabled = true;
                        //    txtPtttGroupCode.Enabled = true;
                        //}
                    }
                    else
                    {
                        if (this.sereServPTTT != null)
                        {
                            var surgService = lstService.FirstOrDefault(o => o.ID == sereServ.SERVICE_ID);
                            if (surgService != null && surgService.PTTT_GROUP_ID != null)
                            {
                                HIS_PTTT_GROUP ptttGroup = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_PTTT_GROUP>().FirstOrDefault(o => o.ID == surgService.PTTT_GROUP_ID);
                                cbbPtttGroup.EditValue = ptttGroup.ID;
                                txtPtttGroupCode.Text = ptttGroup.PTTT_GROUP_CODE;
                                if (Config.HisConfigKeys.CheckPermissonOption == "2" && this.serviceReq.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__PT)
                                {
                                    cbbPtttGroup.Enabled = true;
                                    txtPtttGroupCode.Enabled = true;
                                }
                                else
                                {
                                    cbbPtttGroup.Enabled = false;
                                    txtPtttGroupCode.Enabled = false;
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

        private void SetDefaultCboPTMethod(V_HIS_SERE_SERV_5 sereServ, DevExpress.XtraEditors.GridLookUpEdit cbo, DevExpress.XtraEditors.TextEdit txt)
        {
            try
            {
                HIS_PTTT_METHOD ptttMethod = BackendDataWorker.Get<HIS_PTTT_METHOD>().FirstOrDefault(o => o.IS_ACTIVE == (short)1 && o.PTTT_METHOD_NAME.ToLower() == sereServ.TDL_SERVICE_NAME.ToLower());
                if (ptttMethod != null)
                {
                    txt.Text = ptttMethod.PTTT_METHOD_CODE;
                    cbo.EditValue = ptttMethod.ID;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SuccessLog(MOS.EFMODEL.DataModels.V_HIS_SERVICE_REQ result)
        {
            try
            {
                if (result != null)
                {
                    //string message = String.Format(EXE.LOGIC.Base.EventLogUtil.SetLog(His.EventLog.Message.Enum.TraKetQuaDichVuKhamBenh), result.ID, result.SERVICE_REQ_CODE, result.FINISH_TIME, result.EXECUTE_LOGINNAME, result.EXECUTE_USERNAME, result.TREATMENT_CODE, result.PATIENT_CODE, result.VIR_PATIENT_NAME);
                    //His.EventLog.Logger.Log(LOGIC.LocalStore.GlobalStore.APPLICATION_CODE, Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName(), message, Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginAddress());
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private bool CheckStartTimeFinishTime()
        {
            bool vali = true;
            try
            {
                List<string> errMess = new List<string>();
                if (dtStart.EditValue != null || dtFinish.EditValue != null)
                {
                    long timeStart = dtStart.EditValue != null ? Inventec.Common.TypeConvert.Parse.ToInt64(dtStart.DateTime.ToString("yyyyMMddHHmm") + "00") : 0;
                    long timeFinish = dtFinish.EditValue != null ? Inventec.Common.TypeConvert.Parse.ToInt64(dtFinish.DateTime.ToString("yyyyMMddHHmm") + "00") : 0;

                    if (this.vhisTreatment != null && timeStart < this.vhisTreatment.IN_TIME)
                    {
                        errMess.Add(String.Format(ResourceMessage.ThoiGianBatDauThoiGianVaoVien));
                        vali = false;
                    }

                    if (this.vhisTreatment != null && timeStart > this.vhisTreatment.OUT_TIME)
                    {
                        errMess.Add(String.Format(ResourceMessage.ThoiGianBatDauThoiGianRaVien));
                        vali = false;
                    }

                    if (timeFinish > 0 && timeStart > timeFinish)
                    {
                        errMess.Add(String.Format(ResourceMessage.ThoiGianBatDauThoiGianKetThuc));
                        vali = false;
                    }

                    if (this.vhisTreatment != null && timeFinish < this.vhisTreatment.IN_TIME)
                    {
                        errMess.Add(String.Format(ResourceMessage.ThoiGianKetThucThoiGianVaoVien));
                        vali = false;
                    }

                    if (this.vhisTreatment != null && timeFinish > this.vhisTreatment.OUT_TIME)
                    {
                        errMess.Add(String.Format(ResourceMessage.ThoiGianKetThucThoiGianRaVien));
                        vali = false;
                    }

                    if (!vali && errMess != null && errMess.Count > 0)
                    {
                        MessageBox.Show(String.Join("\r\n", errMess));
                    }
                }
            }
            catch (Exception ex)
            {
                vali = false;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return vali;
        }

        private bool ValidStartDatePTTT(ref HisSurgServiceReqUpdateListSDO hisSurgResultSDO)
        {
            bool valid = true;
            try
            {
                var dtIntructime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(serviceReq.INTRUCTION_TIME);

                if (dtStart.EditValue != null)
                {
                    if (dtIntructime != null && Int64.Parse(dtIntructime.Value.ToString("yyyyMMddHHmm")) > Int64.Parse(dtStart.DateTime.ToString("yyyyMMddHHmm")))
                    {
                        DevExpress.XtraEditors.XtraMessageBox.Show(ResourceMessage.ThoiGianBatDauPhaiLonHonThoiGianYLenh, HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaCanhBao), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        valid = false;
                        //if (DevExpress.XtraEditors.XtraMessageBox.Show(ResourceMessage.BanCoMuonSuaThoiGianYLenhBangThoiGianBatDauPTTT, HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageBoxButtons.YesNo) == DialogResult.No)
                        //{
                        //    //valid = false;//#21733  Chon khong van luu
                        //}
                        //else
                        //{
                        //    hisSurgResultSDO.UpdateInstructionTimeByStartTime = true;
                        //}
                    }
                }

                if (dtFinish.EditValue != null)
                {
                    if (dtIntructime != null && Int64.Parse(dtIntructime.Value.ToString("yyyyMMddHHmm")) > Int64.Parse(dtFinish.DateTime.ToString("yyyyMMddHHmm")))
                    {
                        DevExpress.XtraEditors.XtraMessageBox.Show(ResourceMessage.ThoiGianKetThucKhongDuocNhoHonThoiGianYLenh, HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaCanhBao), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        valid = false;
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

        private void SereServOutEkipResult(object data)
        {
            try
            {
                List<V_HIS_SERE_SERV_5> sereServResults = data as List<V_HIS_SERE_SERV_5>;
                if (sereServResults != null && sereServResults.Count > 0)
                {
                    sereServInPackages.AddRange(sereServResults);
                    if (gridControlSereServAttach.DataSource != null)
                        gridControlSereServAttach.RefreshDataSource();
                    else
                        gridControlSereServAttach.DataSource = sereServInPackages;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SereServInEkipResult(object data)
        {
            try
            {
                List<V_HIS_SERE_SERV_5> sereServResults = data as List<V_HIS_SERE_SERV_5>;
                if (sereServResults != null && sereServResults.Count > 0)
                {
                    //Load gridControlSerservKip
                    sereServInEkips.AddRange(sereServResults);
                    if (gridControlServServInEkip.DataSource != null)
                        gridControlServServInEkip.RefreshDataSource();
                    else
                        gridControlServServInEkip.DataSource = sereServInEkips;

                    sereServInPackages.AddRange(sereServResults);
                    if (gridControlSereServAttach.DataSource != null)
                        gridControlSereServAttach.RefreshDataSource();
                    else
                        gridControlSereServAttach.DataSource = sereServInPackages;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private bool CheckExistFinishTime()
        {
            bool result = true;
            try
            {
                if (dtFinish.EditValue == null)
                {
                    //MessageBox.Show("Vui lòng chọn thời gian kết thúc", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    result = false;
                }

                string warning = "";
                CommonParam paramW = new CommonParam();

                if (this.ModuleControls == null || this.ModuleControls.Count == 0)
                {
                    ModuleControlProcess controlProcess = new ModuleControlProcess(true);
                    this.ModuleControls = controlProcess.GetControls(this);
                }

                GetMessageErrorControlInvalidProcess getMessageErrorControlInvalidProcess = new Utility.GetMessageErrorControlInvalidProcess();
                getMessageErrorControlInvalidProcess.Run(this, this.dxValidationProvider1, this.ModuleControls, paramW);

                warning = paramW.GetMessage();
                if (!String.IsNullOrEmpty(warning))
                {
                    MessageBox.Show(warning, Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private void reloadSereServ(object data)
        {
            try
            {
                HIS_SERE_SERV sereServResult = data as HIS_SERE_SERV;
                if (sereServResult != null)
                {
                    foreach (var sereServbyServiceReq in sereServbyServiceReqs)
                    {
                        if (sereServbyServiceReq.ID == sereServResult.ID)
                        {
                            var vSereServ = lstService.FirstOrDefault(o => o.ID == sereServResult.SERVICE_ID);
                            if (vSereServ != null)
                            {
                                sereServbyServiceReq.TDL_SERVICE_CODE = vSereServ.SERVICE_CODE;
                                sereServbyServiceReq.TDL_SERVICE_NAME = vSereServ.SERVICE_NAME;
                                sereServbyServiceReq.SERVICE_UNIT_NAME = vSereServ.SERVICE_UNIT_NAME;
                            }

                            sereServbyServiceReq.EKIP_ID = sereServResult.EKIP_ID;
                            sereServbyServiceReq.SERVICE_ID = sereServResult.SERVICE_ID;
                            sereServbyServiceReq.PATIENT_TYPE_ID = sereServResult.PATIENT_TYPE_ID;
                            sereServbyServiceReq.PARENT_ID = sereServResult.PARENT_ID;
                            sereServbyServiceReq.IS_OUT_PARENT_FEE = sereServResult.IS_OUT_PARENT_FEE;
                            sereServbyServiceReq.IS_EXPEND = sereServResult.IS_OUT_PARENT_FEE;
                            sereServbyServiceReq.AMOUNT = sereServResult.AMOUNT;
                        }
                    }

                    grdControlService.RefreshDataSource();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ProcessSereServPttt(SurgUpdateSDO hisSurgResultSDO)
        {
            try
            {
                MOS.EFMODEL.DataModels.HIS_SERE_SERV_PTTT HisSereServPttt = new MOS.EFMODEL.DataModels.HIS_SERE_SERV_PTTT();
                if (this.sereServPTTT != null)
                {
                    Inventec.Common.Mapper.DataObjectMapper.Map<HIS_SERE_SERV_PTTT>(HisSereServPttt, this.sereServPTTT);
                }

                HisSereServPttt.SERE_SERV_ID = sereServ.ID;
                if (vhisTreatment != null)
                    HisSereServPttt.TDL_TREATMENT_ID = vhisTreatment.ID;

                if (txtIcd1.ErrorText == "")
                {
                    if (chkIcd1.Checked)
                        HisSereServPttt.ICD_NAME = txtIcd1.Text;
                    else
                        HisSereServPttt.ICD_NAME = cboIcd1.Text;

                    if (!String.IsNullOrEmpty(txtIcdCode1.Text))
                    {
                        HisSereServPttt.ICD_CODE = txtIcdCode1.Text;
                    }
                }

                if (txtIcdCmName.ErrorText == "")
                {
                    if (chkIcdCm.Checked)
                        HisSereServPttt.ICD_CM_NAME = txtIcdCmName.Text;
                    else
                        HisSereServPttt.ICD_CM_NAME = cboIcdCmName.Text;

                    HisSereServPttt.ICD_CM_CODE = txtIcdCmCode.Text;
                }

                if (txtIcd2.ErrorText == "")
                {
                    if (chkIcd2.Checked)
                        HisSereServPttt.BEFORE_PTTT_ICD_NAME = txtIcd2.Text;
                    else
                        HisSereServPttt.BEFORE_PTTT_ICD_NAME = cboIcd2.Text;

                    if (!String.IsNullOrEmpty(txtIcdCode2.Text))
                    {
                        HisSereServPttt.BEFORE_PTTT_ICD_CODE = txtIcdCode2.Text;
                    }
                }

                if (txtIcd3.ErrorText == "")
                {
                    if (chkIcd3.Checked)
                        HisSereServPttt.AFTER_PTTT_ICD_NAME = txtIcd3.Text;
                    else
                        HisSereServPttt.AFTER_PTTT_ICD_NAME = cboIcd3.Text;

                    if (!String.IsNullOrEmpty(txtIcdCode3.Text))
                    {
                        HisSereServPttt.AFTER_PTTT_ICD_CODE = txtIcdCode3.Text;
                    }
                }

                //Chuan doan phu
                HisSereServPttt.ICD_TEXT = txtIcdText.Text;
                HisSereServPttt.ICD_SUB_CODE = txtIcdExtraCode.Text;

                HisSereServPttt.ICD_CM_TEXT = txtIcdCmSubName.Text;
                HisSereServPttt.ICD_CM_SUB_CODE = txtIcdCmSubCode.Text;

                // nhom mau
                if (cbbBlood.EditValue != null)
                {
                    HisSereServPttt.BLOOD_ABO_ID = (long)cbbBlood.EditValue;
                }
                else
                {
                    HisSereServPttt.BLOOD_ABO_ID = null;
                }
                //Nhom mau RH
                if (cbbBloodRh.EditValue != null)
                {
                    HisSereServPttt.BLOOD_RH_ID = (long)cbbBloodRh.EditValue;
                }
                else
                {
                    HisSereServPttt.BLOOD_RH_ID = null;
                }
                //Phuong phap vô cảm
                if (cbbEmotionlessMethod.EditValue != null)
                {
                    HisSereServPttt.EMOTIONLESS_METHOD_ID = (long)cbbEmotionlessMethod.EditValue;
                }
                else
                {
                    HisSereServPttt.EMOTIONLESS_METHOD_ID = null;
                }
                //Tai bien PTTT
                if (cboCatastrophe.EditValue != null)
                {
                    HisSereServPttt.PTTT_CATASTROPHE_ID = (long)cboCatastrophe.EditValue;
                }
                else
                {
                    HisSereServPttt.PTTT_CATASTROPHE_ID = null;
                }
                //Tinh hinh PTTT
                if (cboCondition.EditValue != null)
                {
                    HisSereServPttt.PTTT_CONDITION_ID = (long)cboCondition.EditValue;
                }
                else
                {
                    HisSereServPttt.PTTT_CONDITION_ID = null;
                }
                //Loai PTTT
                if (cbbPtttGroup.EditValue != null)
                {
                    HisSereServPttt.PTTT_GROUP_ID = Convert.ToInt64(cbbPtttGroup.EditValue);
                }
                else
                {
                    HisSereServPttt.PTTT_GROUP_ID = null;
                }
                //Phuong phap PTTT
                if (cboMethod.EditValue != null)
                {
                    HisSereServPttt.PTTT_METHOD_ID = Convert.ToInt64(cboMethod.EditValue);
                }
                else
                {
                    HisSereServPttt.PTTT_METHOD_ID = null;
                }
                //Phuong phap Thuc te
                if (cboPhuongPhapThucTe.EditValue != null)
                {
                    HisSereServPttt.REAL_PTTT_METHOD_ID = Convert.ToInt64(cboPhuongPhapThucTe.EditValue);
                }
                else
                {
                    HisSereServPttt.REAL_PTTT_METHOD_ID = null;
                }

                if (!String.IsNullOrEmpty(txtMANNER.Text))
                {
                    HisSereServPttt.MANNER = txtMANNER.Text;
                }

                //Tu vong
                if (cboDeathSurg.EditValue != null)
                {
                    HisSereServPttt.DEATH_WITHIN_ID = (long)cboDeathSurg.EditValue;
                }
                else
                {
                    HisSereServPttt.DEATH_WITHIN_ID = null;
                }

                if (cboLoaiPT.EditValue != null)
                {
                    HisSereServPttt.PTTT_PRIORITY_ID = (long)cboLoaiPT.EditValue;
                }
                else
                {
                    HisSereServPttt.PTTT_PRIORITY_ID = null;
                }

                if (cboPhuongPhap2.EditValue != null)
                {
                    HisSereServPttt.EMOTIONLESS_METHOD_SECOND_ID = (long)cboPhuongPhap2.EditValue;
                }
                else
                {
                    HisSereServPttt.EMOTIONLESS_METHOD_SECOND_ID = null;
                }

                if (cboKQVoCam.EditValue != null)
                {
                    HisSereServPttt.EMOTIONLESS_RESULT_ID = (long)cboKQVoCam.EditValue;
                }
                else
                {
                    HisSereServPttt.EMOTIONLESS_RESULT_ID = null;
                }

                if (cboMoKTCao.EditValue != null)
                {
                    HisSereServPttt.PTTT_HIGH_TECH_ID = (long)cboMoKTCao.EditValue;
                }
                else
                {
                    HisSereServPttt.PTTT_HIGH_TECH_ID = null;
                }

                if (cboBanMo.EditValue != null)
                {
                    HisSereServPttt.PTTT_TABLE_ID = (long)cboBanMo.EditValue;
                }
                else
                {
                    HisSereServPttt.PTTT_TABLE_ID = null;
                }

                if (currentEyeSurgDesc != null && currentEyeSurgDesc.ID > 0)
                {
                    HisSereServPttt.EYE_SURGRY_DESC_ID = currentEyeSurgDesc.ID;
                }

                if (this.currentEyeSurgDesc != null && this.currentEyeSurgDesc.LOAI_PT_MAT > 0)
                    hisSurgResultSDO.EyeSurgryDesc = this.currentEyeSurgDesc;
                else if (HisSereServPttt.EYE_SURGRY_DESC_ID > 0)
                    HisSereServPttt.EYE_SURGRY_DESC_ID = null;

                if (this.SkinSurgeryDes != null && this.SkinSurgeryDes != null && (this.SkinSurgeryDes.HasValue || this.SkinSurgeryDes.ID > 0))
                {
                    HisSereServPttt.SURGERY_POSITION_ID = this.SkinSurgeryDes.SURGERY_POSITION_ID;
                    hisSurgResultSDO.SkinSurgeryDesc = new HIS_SKIN_SURGERY_DESC();
                    Inventec.Common.Mapper.DataObjectMapper.Map<HIS_SKIN_SURGERY_DESC>(hisSurgResultSDO.SkinSurgeryDesc, this.SkinSurgeryDes);
                    //hisSurgResultSDO.SkinSurgeryDesc = this.SkinSurgeryDes;
                }
                else if (HisSereServPttt.SKIN_SURGERY_DESC_ID > 0)
                {
                    HisSereServPttt.SURGERY_POSITION_ID = null;
                    HisSereServPttt.SKIN_SURGERY_DESC_ID = null;
                }

                if (cboHospSubs.EditValue != null)
                {
                    HisSereServPttt.SUBS_DIRECTOR_LOGINNAME = cboHospSubs.EditValue.ToString();
                    HisSereServPttt.SUBS_DIRECTOR_USERNAME = lstReAcsUserADO.FirstOrDefault(o => o.LOGINNAME == cboHospSubs.EditValue.ToString()).USERNAME;
                }
                else
                {
                    HisSereServPttt.SUBS_DIRECTOR_LOGINNAME = null;
                    HisSereServPttt.SUBS_DIRECTOR_USERNAME = null;
                }
                if (cboEndDeptSubs.EditValue != null)
                {
                    HisSereServPttt.SUBS_HEAD_LOGINNAME = cboEndDeptSubs.EditValue.ToString();
                    HisSereServPttt.SUBS_HEAD_USERNAME = lstReAcsUserADO.FirstOrDefault(o => o.LOGINNAME == cboEndDeptSubs.EditValue.ToString()).USERNAME;
                }
                else
                {
                    HisSereServPttt.SUBS_HEAD_LOGINNAME = null;
                    HisSereServPttt.SUBS_HEAD_USERNAME = null;
                }

                var RoomV = BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == Module.RoomId);
                var Department = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_DEPARTMENT>().FirstOrDefault(o => o.ID == RoomV.DEPARTMENT_ID);
                HisSereServPttt.HEAD_LOGINNAME = Department.HEAD_LOGINNAME;
                HisSereServPttt.HEAD_USERNAME = Department.HEAD_USERNAME;
                var Branch = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_BRANCH>().FirstOrDefault(o => o.ID == Department.BRANCH_ID);
                HisSereServPttt.DIRECTOR_LOGINNAME = Branch.DIRECTOR_LOGINNAME;
                HisSereServPttt.DIRECTOR_USERNAME = Branch.DIRECTOR_USERNAME;
                hisSurgResultSDO.SereServPttt = HisSereServPttt;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private List<HIS_EKIP_USER> ProcessEkipUserOther(List<HisEkipUserADO> sereServPTTTADOs)
        {
            List<MOS.EFMODEL.DataModels.HIS_EKIP_USER> ekipUsers = new List<MOS.EFMODEL.DataModels.HIS_EKIP_USER>();
            try
            {

                if (sereServPTTTADOs != null && sereServPTTTADOs.Count > 0)
                {
                    foreach (var item in sereServPTTTADOs)
                    {

                        MOS.EFMODEL.DataModels.HIS_EKIP_USER ekipUser = new HIS_EKIP_USER();
                        Inventec.Common.Mapper.DataObjectMapper.Map<HIS_EKIP_USER>(ekipUser, item);

                        var acsUser = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<ACS.EFMODEL.DataModels.ACS_USER>().SingleOrDefault(o => o.LOGINNAME == ekipUser.LOGINNAME);
                        if (acsUser != null)
                        {
                            ekipUser.USERNAME = acsUser.USERNAME;
                        }

                        if (sereServ != null && sereServ.EKIP_ID.HasValue)
                        {
                            ekipUser.EKIP_ID = sereServ.EKIP_ID.Value;
                        }
                        ekipUsers.Add(ekipUser);
                    }
                }
                return ekipUsers;
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return ekipUsers;
        }

        private bool ProcessEkipUser(SurgUpdateSDO hisSurgResultSDO)
        {
            bool result = true;
            try
            {
                List<MOS.EFMODEL.DataModels.HIS_EKIP_USER> ekipUsers = new List<MOS.EFMODEL.DataModels.HIS_EKIP_USER>();
                var sereServPTTTADOs = ucEkip.GetDataSource();
                //= grdControlInformationSurg.DataSource as List<HisEkipUserADO>;
                if (sereServPTTTADOs != null && sereServPTTTADOs.Count > 0)
                {
                    foreach (var item in sereServPTTTADOs)
                    {

                        MOS.EFMODEL.DataModels.HIS_EKIP_USER ekipUser = new HIS_EKIP_USER();
                        Inventec.Common.Mapper.DataObjectMapper.Map<HIS_EKIP_USER>(ekipUser, item);

                        var acsUser = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<ACS.EFMODEL.DataModels.ACS_USER>().SingleOrDefault(o => o.LOGINNAME == ekipUser.LOGINNAME);
                        if (acsUser != null)
                        {
                            ekipUser.USERNAME = acsUser.USERNAME;
                        }

                        if (sereServ != null && sereServ.EKIP_ID.HasValue)
                        {
                            ekipUser.EKIP_ID = sereServ.EKIP_ID.Value;
                        }
                        //if (item.DEPARTMENT_ID <= 0 ||item.DEPARTMENT_ID == null)
                        //{
                        //    ekipUser.DEPARTMENT_ID = null;
                        //}

                        ekipUsers.Add(ekipUser);
                    }
                }

                var groupEkipUser = ekipUsers.GroupBy(x => new { x.LOGINNAME, x.EXECUTE_ROLE_ID });
                foreach (var item in groupEkipUser)
                {
                    if (item.Count() >= 2)
                    {
                        return false;
                    }
                }
                hisSurgResultSDO.EkipUsers = ekipUsers;
            }
            catch (Exception ex)
            {
                result = true;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private void ProcessSereServExt(SurgUpdateSDO hisSurgResultSDO)
        {
            try
            {
                if (this.sereServ != null)
                {
                    if (SereServExt == null)
                    {
                        SereServExt = new HIS_SERE_SERV_EXT();
                    }

                    SereServExt.NOTE = txtResultNote.Text.Trim();
                    SereServExt.CONCLUDE = txtConclude.Text.Trim();
                    SereServExt.DESCRIPTION = txtDescription.Text.Trim();
                    SereServExt.SERE_SERV_ID = this.sereServ.ID;
                    if (dtStart.EditValue != null)
                        SereServExt.BEGIN_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtStart.DateTime);
                    else
                        SereServExt.BEGIN_TIME = null;

                    if (dtFinish.EditValue != null)
                        SereServExt.END_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtFinish.DateTime);
                    else
                    {
                        SereServExt.END_TIME = null;
                    }

                    if (cboMachine.EditValue != null)
                    {
                        SereServExt.MACHINE_ID = (long)cboMachine.EditValue;
                        SereServExt.MACHINE_CODE = this.txtMachineCode.Text;
                    }
                    else
                    {
                        SereServExt.MACHINE_ID = null;
                        SereServExt.MACHINE_CODE = "";
                    }
                    SereServExt.INSTRUCTION_NOTE = !string.IsNullOrEmpty(txtIntructionNote.Text.Trim()) ? txtIntructionNote.Text.Trim() : null;
                    hisSurgResultSDO.SereServExt = SereServExt;
                }
                //AutoMapper.Mapper.CreateMap<MOS.EFMODEL.DataModels.V_HIS_SERE_SERV_5, HIS_SERE_SERV>();
                //HIS_SERE_SERV HisSereServ = AutoMapper.Mapper.Map<V_HIS_SERE_SERV_5, HIS_SERE_SERV>(this.sereServ);
                //hisSurgResultSDO.SereServ = HisSereServ;
                Inventec.Common.Logging.LogSystem.Debug("MACHINE_ID=" + SereServExt.MACHINE_ID + "____MACHINE_CODE=" + SereServExt.MACHINE_CODE);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SaveSurgServiceReq(HisSurgServiceReqUpdateListSDO hisSuimResultSDO, ref bool success, [Optional] bool notShowMess)
        {
            CommonParam param = new CommonParam();
            success = false;
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("SaveSurgServiceReq. 1");
                if (this.Validate(hisSuimResultSDO))
                {
                    Inventec.Common.Logging.LogSystem.Debug("SaveSurgServiceReq. 2");
                    if (this.sereServ == null)
                        throw new Exception("Khong tim thay dich vu");

                    //chỉ lọc sau this.Validate(hisSuimResultSDO) khi người dùng đã nhập thông tin 
                    foreach (var item in hisSuimResultSDO.SurgUpdateSDOs)
                    {
                        // http://redmine.vietsens.vn/redmine/issues/35542 lọc và gửi lên các vai trò có thông tin Userss
                        if (item.EkipUsers != null && item.EkipUsers.Count > 0)
                            item.EkipUsers = item.EkipUsers.Where(o => !String.IsNullOrWhiteSpace(o.LOGINNAME) && !String.IsNullOrWhiteSpace(o.USERNAME) && o.EXECUTE_ROLE_ID > 0).ToList();
                    }

                    WaitingManager.Show();
                    Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData(" api/HisServiceReq/SurgUpdateList INPUT hisSuimResultSDO ", hisSuimResultSDO));

                    //Check thông tin thực hiện cùng lúc (HisServiceReq/CheckSurgSimultaneily)
                    if (Config.HisConfigKeys.CHECK_SIMULTANEITY_OPTION == "2"
                        && dtStart.EditValue != null && dtFinish.EditValue != null
                        && notShowMess == false)
                    {
                        bool notAllowSimutaneity = false;
                        foreach (var item in this.sereServbyServiceReqs)
                        {
                            var service = lstService.FirstOrDefault(o => o.ID == item.SERVICE_ID);
                            if (service != null && service.ALLOW_SIMULTANEITY != 1)
                                notAllowSimutaneity = true;
                        }
                        if (notAllowSimutaneity)
                        {
                            CommonParam paramCheck = new CommonParam();
                            bool resultCheckSurgSimultaneily = new BackendAdapter(paramCheck).Post<bool>("api/HisServiceReq/CheckSurgSimultaneily", ApiConsumers.MosConsumer, hisSuimResultSDO, paramCheck);
                            if (resultCheckSurgSimultaneily == false)
                            {
                                string message = paramCheck.GetMessage();
                                if (String.IsNullOrWhiteSpace(message) || !message.EndsWith("."))
                                    message += ".";
                                if (DevExpress.XtraEditors.XtraMessageBox.Show(String.Format("{0} {1}", message, ResourceMessage.BanCoMuonTiepTucKhong), HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
                                {
                                    return;
                                }
                            }
                        }
                    }

                    currentHisSurgResultSDO = new BackendAdapter(param)
                    .Post<MOS.SDO.HisSurgServiceReqUpdateListSDO>("api/HisServiceReq/SurgUpdateList", ApiConsumers.MosConsumer, hisSuimResultSDO, param);
                    Inventec.Common.Logging.LogSystem.Debug("SaveSurgServiceReq. 3");
                    WaitingManager.Hide();
                    if (currentHisSurgResultSDO != null && currentHisSurgResultSDO.SurgUpdateSDOs != null && currentHisSurgResultSDO.SurgUpdateSDOs.Count > 0)
                    {
                        Inventec.Common.Logging.LogSystem.Debug("SaveSurgServiceReq. 4____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => currentHisSurgResultSDO), currentHisSurgResultSDO));
                        this.SereServExt = currentHisSurgResultSDO.SurgUpdateSDOs.First().SereServExt;
                        if (this.sereServPTTT == null)
                            this.sereServPTTT = new V_HIS_SERE_SERV_PTTT();
                        Inventec.Common.Mapper.DataObjectMapper.Map<V_HIS_SERE_SERV_PTTT>(this.sereServPTTT, currentHisSurgResultSDO.SurgUpdateSDOs.First().SereServPttt);
                        this.currentEyeSurgDesc = currentHisSurgResultSDO.SurgUpdateSDOs.First().EyeSurgryDesc;
                        this.stentConcludeSave = currentHisSurgResultSDO.SurgUpdateSDOs.First().StentConcludes;
                        if (currentHisSurgResultSDO.SurgUpdateSDOs.First().SkinSurgeryDesc != null)
                        {
                            if (this.SkinSurgeryDes == null)
                                this.SkinSurgeryDes = new Base.SkinSurgeryDesADO();
                            Inventec.Common.Mapper.DataObjectMapper.Map<Base.SkinSurgeryDesADO>(SkinSurgeryDes, currentHisSurgResultSDO.SurgUpdateSDOs.First().SkinSurgeryDesc);
                            this.SkinSurgeryDes.SURGERY_POSITION_ID = currentHisSurgResultSDO.SurgUpdateSDOs.First().SereServPttt.SURGERY_POSITION_ID;
                        }

                        LoadServiceReq(serviceReq.ID);

                        success = true;
                        btnPrint.Enabled = true;
                        btnSwapService.Enabled = false;
                        SetEnableControl();
                        if (currentHisSurgResultSDO.SurgUpdateSDOs.First().EkipUsers != null && currentHisSurgResultSDO.SurgUpdateSDOs.First().EkipUsers.Count > 0)
                        {
                            sereServ.EKIP_ID = currentHisSurgResultSDO.SurgUpdateSDOs.First().EkipUsers[0].EKIP_ID;
                            foreach (var item in sereServs)
                            {
                                if (sereServ.ID == item.ID)
                                {
                                    item.EKIP_ID = sereServ.EKIP_ID;
                                    break;
                                }
                            }
                            grdControlService.RefreshDataSource();
                        }

                        //xuandv -- reload lai btnKhac
                        this.InitButtonOther();
                        this.InitPrintSurgService();
                        if ((chkSign.Checked && !IsActionOtherButton) || chkXemIn.Checked || chkIn.Checked)
                        {
                            PrintProcess(PrintTypeSurg.PHIEU_THU_THUAT_PHAU_THUAT);
                        }
                    }

                    #region Show message
                    if (!notShowMess)
                        MessageManager.Show(this.ParentForm, param, success);
                    #endregion

                    #region Process has exception
                    SessionManager.ProcessTokenLost(param);
                    #endregion
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Fatal(ex);
            }
        }

        private bool Validate(HisSurgServiceReqUpdateListSDO hisSuimResultSDO)
        {
            bool result = false;
            try
            {
                if (hisSuimResultSDO != null && hisSuimResultSDO.SurgUpdateSDOs != null && hisSuimResultSDO.SurgUpdateSDOs.First().EkipUsers != null && hisSuimResultSDO.SurgUpdateSDOs.First().EkipUsers.Count > 0 && !IsReadOnlyGridViewEkipUser)
                {
                    List<HIS_EKIP_USER> hasInvalid = hisSuimResultSDO.SurgUpdateSDOs.First().EkipUsers
                        .Where(o => String.IsNullOrEmpty(o.LOGINNAME)
                            || o.EXECUTE_ROLE_ID <= 0).Distinct().ToList();
                    if (hasInvalid != null && hasInvalid.Count > 0)
                    {
                        List<HIS_EXECUTE_ROLE> datas = null;
                        if (BackendDataWorker.IsExistsKey<HIS_EXECUTE_ROLE>())
                        {
                            datas = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_EXECUTE_ROLE>();
                        }
                        else
                        {
                            CommonParam paramCommon = new CommonParam();
                            dynamic filter = new System.Dynamic.ExpandoObject();
                            datas = new Inventec.Common.Adapter.BackendAdapter(paramCommon).GetAsync<List<HIS_EXECUTE_ROLE>>("api/HisExecuteRole/Get", ApiConsumers.MosConsumer, filter, paramCommon);
                            if (datas != null) BackendDataWorker.UpdateToRam(typeof(HIS_EXECUTE_ROLE), datas, long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss")));
                        }
                        var executeRoleNull = datas.Where(o => hasInvalid.Select(p => p.EXECUTE_ROLE_ID).Contains(o.ID)).ToList();
                        if (executeRoleNull == null)
                            result = true;

                        if (this.currentHisService != null && this.currentHisService.IS_OUT_OF_MANAGEMENT == 1)
                        {
                            result = true;
                        }
                        else
                        {
                            string mess = String.Format(ResourceMessage.BanChuaNhapThongTinTuongUngVoiCacVaiTRo, String.Join(",", executeRoleNull.Select(o => o.EXECUTE_ROLE_NAME).ToArray()));

                            if (MessageBox.Show(mess, ResourceMessage.ThongBao, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                                return false;
                            else
                                result = true;
                        }

                    }

                    List<string> messError = new List<string>();
                    var grLoginname = hisSuimResultSDO.SurgUpdateSDOs.First().EkipUsers.Where(o => !String.IsNullOrWhiteSpace(o.LOGINNAME)).GroupBy(o => o.LOGINNAME).ToList();
                    foreach (var item in grLoginname)
                    {
                        if (item.Count() > 1)
                        {
                            var lstExeRole = BackendDataWorker.Get<HIS_EXECUTE_ROLE>().Where(o => item.Select(s => s.EXECUTE_ROLE_ID).Contains(o.ID)).ToList();

                            messError.Add(string.Format("Tài khoản {0} được thiết lập với các vai trò {1}", item.Key, string.Join(",", lstExeRole.Select(s => s.EXECUTE_ROLE_NAME))));
                        }
                    }

                    if (messError.Count > 0)
                    {
                        MessageBox.Show(string.Join("\n", messError), "Thông báo");
                        return false;
                    }
                }
                //else if (hisSuimResultSDO != null && hisSuimResultSDO.SurgUpdateSDOs != null && hisSuimResultSDO.SurgUpdateSDOs.First().EkipUsers != null && hisSuimResultSDO.SurgUpdateSDOs.First().EkipUsers.Count == 0)
                //{
                //    if (MessageBox.Show("Bạn chưa nhập thông tin tương ứng với (các) vai trò. Bạn có muốn thực hiện không?", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                //        return false;
                //    else
                //        result = true;
                //}

                result = true;
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return result;
        }

        private void SaveSurgServiceReq(HisSurgServiceReqUpdateSDO hisSuimResultSDO, ref bool success, [Optional] bool notShowMess)
        {
            CommonParam param = new CommonParam();
            success = false;
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("SaveSurgServiceReq. 1");
                if (this.Validate(hisSuimResultSDO))
                {
                    Inventec.Common.Logging.LogSystem.Debug("SaveSurgServiceReq. 2");
                    if (this.sereServ == null)
                        throw new Exception("Khong tim thay dich vu");

                    if (hisSuimResultSDO.EkipUsers != null && hisSuimResultSDO.EkipUsers.Count > 0)
                        hisSuimResultSDO.EkipUsers = hisSuimResultSDO.EkipUsers.Where(o => !String.IsNullOrWhiteSpace(o.LOGINNAME) && !String.IsNullOrWhiteSpace(o.USERNAME) && o.EXECUTE_ROLE_ID > 0).ToList();
                    hisSuimResultSDO.EkipUsers = hisSuimResultSDO.EkipUsers != null && hisSuimResultSDO.EkipUsers.Count > 0 ? hisSuimResultSDO.EkipUsers : null;
                    WaitingManager.Show();
                    Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData(" api/HisServiceReq/SurgUpdate INPUT hisSuimResultSDO ", hisSuimResultSDO));

                    //Check thông tin thực hiện cùng lúc (HisServiceReq/CheckSurgSimultaneily)
                    if (Config.HisConfigKeys.CHECK_SIMULTANEITY_OPTION == "2"
                        && dtStart.EditValue != null && dtFinish.EditValue != null)
                    {
                        bool notAllowSimutaneity = false;
                        var service = lstService.FirstOrDefault(o => o.ID == this.sereServ.SERVICE_ID);
                        if (service != null && service.ALLOW_SIMULTANEITY != 1)
                            notAllowSimutaneity = true;

                        if (notAllowSimutaneity)
                        {
                            CommonParam paramCheck = new CommonParam();
                            HisSurgServiceReqUpdateListSDO sdoCheckSurgSimultaneily = ConvertTo_HisSurgServiceReqUpdateListSDO(hisSuimResultSDO);
                            Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("api/HisServiceReq/CheckSurgSimultaneily INPUT", sdoCheckSurgSimultaneily));
                            bool resultCheckSurgSimultaneily = new BackendAdapter(paramCheck).Post<bool>("api/HisServiceReq/CheckSurgSimultaneily", ApiConsumers.MosConsumer, sdoCheckSurgSimultaneily, paramCheck);
                            if (resultCheckSurgSimultaneily == false)
                            {
                                WaitingManager.Hide();
                                string message = paramCheck.GetMessage();
                                if (!message.Trim().EndsWith(".")) //String.IsNullOrWhiteSpace(message) ||
                                    message += ".";
                                if (DevExpress.XtraEditors.XtraMessageBox.Show(String.Format("{0} {1}", message, ResourceMessage.BanCoMuonTiepTucKhong), HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
                                {
                                    return;
                                }
                                WaitingManager.Show();
                            }
                        }
                    }

                    currentSurgResultSDO = new BackendAdapter(param)
                    .Post<MOS.SDO.HisSurgServiceReqUpdateSDO>("api/HisServiceReq/SurgUpdate", ApiConsumers.MosConsumer, hisSuimResultSDO, param);
                    Inventec.Common.Logging.LogSystem.Debug("SaveSurgServiceReq. 3");
                    WaitingManager.Hide();
                    if (currentSurgResultSDO != null)
                    {
                        Inventec.Common.Logging.LogSystem.Debug("SaveSurgServiceReq. 4____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => currentSurgResultSDO), currentSurgResultSDO));
                        this.SereServExt = currentSurgResultSDO.SereServExt;
                        if (this.sereServPTTT == null)
                            this.sereServPTTT = new V_HIS_SERE_SERV_PTTT();
                        Inventec.Common.Mapper.DataObjectMapper.Map<V_HIS_SERE_SERV_PTTT>(this.sereServPTTT, currentSurgResultSDO.SereServPttt);
                        this.currentEyeSurgDesc = currentSurgResultSDO.EyeSurgryDesc;
                        this.stentConcludeSave = currentSurgResultSDO.StentConcludes;
                        if (currentSurgResultSDO.SkinSurgeryDesc != null)
                        {
                            if (this.SkinSurgeryDes == null)
                                this.SkinSurgeryDes = new Base.SkinSurgeryDesADO();
                            Inventec.Common.Mapper.DataObjectMapper.Map<Base.SkinSurgeryDesADO>(SkinSurgeryDes, currentSurgResultSDO.SkinSurgeryDesc);
                            this.SkinSurgeryDes.SURGERY_POSITION_ID = currentSurgResultSDO.SereServPttt.SURGERY_POSITION_ID;
                        }

                        LoadServiceReq(serviceReq.ID);

                        success = true;
                        btnPrint.Enabled = true;
                        btnSwapService.Enabled = false;
                        SetEnableControl();
                        if (currentSurgResultSDO.EkipUsers != null && currentSurgResultSDO.EkipUsers.Count > 0)
                        {
                            sereServ.EKIP_ID = currentSurgResultSDO.EkipUsers[0].EKIP_ID;
                            foreach (var item in sereServs)
                            {
                                if (sereServ.ID == item.ID)
                                {
                                    item.EKIP_ID = sereServ.EKIP_ID;
                                    break;
                                }
                            }
                            grdControlService.RefreshDataSource();
                        }

                        //xuandv -- reload lai btnKhac
                        this.InitButtonOther();
                        this.InitPrintSurgService();

                        if ((chkSign.Checked && !IsActionOtherButton) || chkXemIn.Checked || chkIn.Checked)
                        {
                            PrintProcess(PrintTypeSurg.PHIEU_THU_THUAT_PHAU_THUAT);
                        }

                    }

                    #region Show message
                    if (!notShowMess)
                        MessageManager.Show(this.ParentForm, param, success);
                    #endregion

                    #region Process has exception
                    SessionManager.ProcessTokenLost(param);
                    #endregion
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Fatal(ex);
            }
        }

        private HisSurgServiceReqUpdateListSDO ConvertTo_HisSurgServiceReqUpdateListSDO(HisSurgServiceReqUpdateSDO sdo)
        {
            HisSurgServiceReqUpdateListSDO result = new HisSurgServiceReqUpdateListSDO();
            if (sdo == null)
                return null;
            try
            {
                result.IsFinished = sdo.IsFinished;
                result.UpdateInstructionTimeByStartTime = sdo.UpdateInstructionTimeByStartTime;
                result.SurgUpdateSDOs = new List<SurgUpdateSDO>();
                SurgUpdateSDO singleData = new SurgUpdateSDO();
                singleData.EkipUsers = sdo.EkipUsers;
                singleData.EyeSurgryDesc = sdo.EyeSurgryDesc;
                singleData.SereServExt = sdo.SereServExt;
                singleData.SereServId = sdo.SereServId;
                singleData.SereServPttt = sdo.SereServPttt;
                singleData.SesePtttMethos = sdo.SesePtttMethos;
                singleData.SkinSurgeryDesc = sdo.SkinSurgeryDesc;

                result.SurgUpdateSDOs.Add(singleData);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private bool Validate(HisSurgServiceReqUpdateSDO hisSuimResultSDO)
        {
            bool result = false;
            try
            {

                if (hisSuimResultSDO != null && hisSuimResultSDO.EkipUsers != null && hisSuimResultSDO.EkipUsers.Count > 0 && !IsReadOnlyGridViewEkipUser)
                {
                    List<HIS_EKIP_USER> hasInvalid = hisSuimResultSDO.EkipUsers
                        .Where(o => String.IsNullOrEmpty(o.LOGINNAME)
                            || o.EXECUTE_ROLE_ID <= 0).Distinct().ToList();
                    if (hasInvalid != null && hasInvalid.Count > 0)
                    {
                        List<HIS_EXECUTE_ROLE> datas = null;
                        if (BackendDataWorker.IsExistsKey<HIS_EXECUTE_ROLE>())
                        {
                            datas = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_EXECUTE_ROLE>();
                        }
                        else
                        {
                            CommonParam paramCommon = new CommonParam();
                            dynamic filter = new System.Dynamic.ExpandoObject();
                            datas = new Inventec.Common.Adapter.BackendAdapter(paramCommon).GetAsync<List<HIS_EXECUTE_ROLE>>("api/HisExecuteRole/Get", ApiConsumers.MosConsumer, filter, paramCommon);
                            if (datas != null) BackendDataWorker.UpdateToRam(typeof(HIS_EXECUTE_ROLE), datas, long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss")));
                        }
                        var executeRoleNull = datas.Where(o => hasInvalid.Select(p => p.EXECUTE_ROLE_ID).Contains(o.ID)).ToList();
                        if (executeRoleNull == null)
                            result = true;

                        if (this.currentHisService != null && this.currentHisService.IS_OUT_OF_MANAGEMENT == 1)
                        {
                            result = true;
                        }
                        else
                        {
                            string mess = String.Format(ResourceMessage.BanChuaNhapThongTinTuongUngVoiCacVaiTRo, String.Join(",", executeRoleNull.Select(o => o.EXECUTE_ROLE_NAME).ToArray()));

                            if (MessageBox.Show(mess, ResourceMessage.ThongBao, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                                return false;
                            else
                                result = true;
                        }

                    }

                    List<string> messError = new List<string>();
                    var grLoginname = hisSuimResultSDO.EkipUsers.Where(o => !String.IsNullOrWhiteSpace(o.LOGINNAME)).GroupBy(o => o.LOGINNAME).ToList();
                    foreach (var item in grLoginname)
                    {
                        if (item.Count() > 1)
                        {
                            var lstExeRole = BackendDataWorker.Get<HIS_EXECUTE_ROLE>().Where(o => item.Select(s => s.EXECUTE_ROLE_ID).Contains(o.ID)).ToList();
                            messError.Add(string.Format("Tài khoản {0} được thiết lập với các vai trò {1}", item.Key, string.Join(",", lstExeRole.Select(s => s.EXECUTE_ROLE_NAME))));
                        }
                    }

                    if (messError.Count > 0)
                    {
                        MessageBox.Show(string.Join("\n", messError), "Thông báo");
                        return false;
                    }
                }
                //else if (hisSuimResultSDO != null && hisSuimResultSDO.EkipUsers != null && hisSuimResultSDO.EkipUsers.Count == 0)
                //{
                //    if (MessageBox.Show("Bạn chưa nhập thông tin tương ứng với (các) vai trò. Bạn có muốn thực hiện không?", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                //        return false;
                //    else
                //        result = true;
                //}
                result = true;
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return result;
        }

        private HIS_SERE_SERV_PTTT_TEMP GetDataForTemp()
        {
            HIS_SERE_SERV_PTTT_TEMP result = null;
            try
            {
                bool valid = true;
                valid = valid && cbbBlood.EditValue == null;
                valid = valid && cbbBloodRh.EditValue == null;
                valid = valid && cbbEmotionlessMethod.EditValue == null;
                valid = valid && cboCatastrophe.EditValue == null;
                valid = valid && cboCondition.EditValue == null;
                //valid = valid && cbbPtttGroup.EditValue == null;
                valid = valid && cboMethod.EditValue == null;
                valid = valid && cboPhuongPhapThucTe.EditValue == null;
                valid = valid && cboDeathSurg.EditValue == null;
                valid = valid && cboLoaiPT.EditValue == null;
                valid = valid && cboPhuongPhap2.EditValue == null;
                valid = valid && cboKQVoCam.EditValue == null;
                valid = valid && cboMoKTCao.EditValue == null;
                valid = valid && cboBanMo.EditValue == null;
                valid = valid && String.IsNullOrWhiteSpace(txtMANNER.Text);
                valid = valid && String.IsNullOrWhiteSpace(txtResultNote.Text);
                valid = valid && String.IsNullOrWhiteSpace(txtConclude.Text);
                valid = valid && String.IsNullOrWhiteSpace(txtDescription.Text);

                // Tất cả null thì ko lưu mẫu
                if (!valid)
                {
                    result = new HIS_SERE_SERV_PTTT_TEMP();

                    if (cbbBlood.EditValue != null)
                    {
                        result.BLOOD_ABO_ID = (long)cbbBlood.EditValue;
                    }

                    //Nhom mau RH
                    if (cbbBloodRh.EditValue != null)
                    {
                        result.BLOOD_RH_ID = (long)cbbBloodRh.EditValue;
                    }

                    //Phuong phap vô cảm
                    if (cbbEmotionlessMethod.EditValue != null)
                    {
                        result.EMOTIONLESS_METHOD_ID = (long)cbbEmotionlessMethod.EditValue;
                    }

                    //Tai bien PTTT
                    if (cboCatastrophe.EditValue != null)
                    {
                        result.PTTT_CATASTROPHE_ID = (long)cboCatastrophe.EditValue;
                    }

                    //Tinh hinh PTTT
                    if (cboCondition.EditValue != null)
                    {
                        result.PTTT_CONDITION_ID = (long)cboCondition.EditValue;
                    }

                    //Loai PTTT
                    if (cbbPtttGroup.EditValue != null)
                    {
                        result.PTTT_GROUP_ID = (long)cbbPtttGroup.EditValue;
                    }

                    //Phuong phap PTTT
                    if (cboMethod.EditValue != null)
                    {
                        result.PTTT_METHOD_ID = (long)cboMethod.EditValue;
                    }

                    //Phuong phap Thuc te
                    if (cboPhuongPhapThucTe.EditValue != null)
                    {
                        result.REAL_PTTT_METHOD_ID = (long)cboPhuongPhapThucTe.EditValue;
                    }

                    if (!String.IsNullOrEmpty(txtMANNER.Text))
                    {
                        result.MANNER = txtMANNER.Text;
                    }

                    //Tu vong
                    if (cboDeathSurg.EditValue != null)
                    {
                        result.DEATH_WITHIN_ID = (long)cboDeathSurg.EditValue;
                    }

                    if (cboLoaiPT.EditValue != null)
                    {
                        result.PTTT_PRIORITY_ID = (long)cboLoaiPT.EditValue;
                    }

                    if (cboPhuongPhap2.EditValue != null)
                    {
                        result.EMOTIONLESS_METHOD_SECOND_ID = (long)cboPhuongPhap2.EditValue;
                    }

                    if (cboKQVoCam.EditValue != null)
                    {
                        result.EMOTIONLESS_RESULT_ID = (long)cboKQVoCam.EditValue;
                    }

                    if (cboMoKTCao.EditValue != null)
                    {
                        result.PTTT_HIGH_TECH_ID = (long)cboMoKTCao.EditValue;
                    }

                    if (cboBanMo.EditValue != null)
                    {
                        result.PTTT_TABLE_ID = (long)cboBanMo.EditValue;
                    }

                    result.NOTE = txtResultNote.Text.Trim();
                    result.CONCLUDE = txtConclude.Text.Trim();
                    result.DESCRIPTION = txtDescription.Text.Trim();
                }
            }
            catch (Exception ex)
            {
                result = null;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private void FillDataToControlFromTemp(HIS_SERE_SERV_PTTT_TEMP fillData)
        {
            try
            {
                if (fillData != null)
                {
                    if (!string.IsNullOrEmpty(this.Patient.BLOOD_ABO_CODE))
                    {
                        var bloodAbo = dataBloodAbo.FirstOrDefault(o => o.BLOOD_ABO_CODE == Patient.BLOOD_ABO_CODE);
                        if (bloodAbo != null)
                        {
                            cbbBlood.EditValue = bloodAbo.ID;
                            txtBlood.Text = bloodAbo.BLOOD_ABO_CODE;
                        }
                        else
                        {
                            cbbBlood.EditValue = null;
                            txtBlood.Text = null;
                        }
                    }
                    else
                    {
                        cbbBlood.EditValue = fillData.BLOOD_ABO_ID;
                    }
                    if (!string.IsNullOrEmpty(this.Patient.BLOOD_RH_CODE))
                    {
                        var bloodRh = dataBloodRh.FirstOrDefault(o => o.BLOOD_RH_CODE == Patient.BLOOD_RH_CODE);
                        if (bloodRh != null)
                        {
                            cbbBloodRh.EditValue = bloodRh.ID;
                            txtBloodRh.Text = bloodRh.BLOOD_RH_CODE;
                        }
                        else
                        {
                            cbbBloodRh.EditValue = null;
                            txtBloodRh.Text = null;
                        }
                    }
                    else
                    {
                        cbbBloodRh.EditValue = fillData.BLOOD_RH_ID;
                    }
                    cboDeathSurg.EditValue = fillData.DEATH_WITHIN_ID;
                    if (dataEmotionlessMethod.Exists(o => o.ID == fillData.EMOTIONLESS_METHOD_ID))
                    {
                        cbbEmotionlessMethod.EditValue = fillData.EMOTIONLESS_METHOD_ID;
                    }
                    else
                    {
                        cbbEmotionlessMethod.EditValue = null;
                    }
                    cboPhuongPhap2.EditValue = fillData.EMOTIONLESS_METHOD_SECOND_ID;
                    cboKQVoCam.EditValue = fillData.EMOTIONLESS_RESULT_ID;
                    txtMANNER.EditValue = fillData.MANNER;
                    cboCatastrophe.EditValue = fillData.PTTT_CATASTROPHE_ID;
                    cboCondition.EditValue = fillData.PTTT_CONDITION_ID;
                    cboMoKTCao.EditValue = fillData.PTTT_HIGH_TECH_ID;
                    cboMethod.EditValue = fillData.PTTT_METHOD_ID;
                    cboLoaiPT.EditValue = fillData.PTTT_PRIORITY_ID;
                    cboBanMo.EditValue = fillData.PTTT_TABLE_ID;
                    cboPhuongPhapThucTe.EditValue = fillData.REAL_PTTT_METHOD_ID;

                    if (cbbPtttGroup.Enabled && !cbbPtttGroup.ReadOnly)
                    {
                        cbbPtttGroup.EditValue = fillData.PTTT_GROUP_ID;
                    }

                    txtConclude.EditValue = fillData.CONCLUDE;
                    txtDescription.EditValue = fillData.DESCRIPTION;
                    txtResultNote.EditValue = fillData.NOTE;
                }
                else
                {
                    cboHospSubs.EditValue = null;
                    cboEndDeptSubs.EditValue = null;
                    cbbBlood.EditValue = null;
                    cbbBloodRh.EditValue = null;
                    cboDeathSurg.EditValue = null;
                    cbbEmotionlessMethod.EditValue = null;
                    cboPhuongPhap2.EditValue = null;
                    cboKQVoCam.EditValue = null;
                    txtMANNER.EditValue = null;
                    cboCatastrophe.EditValue = null;
                    cboCondition.EditValue = null;
                    cboMoKTCao.EditValue = null;
                    cboMethod.EditValue = null;
                    cboLoaiPT.EditValue = null;
                    cboBanMo.EditValue = null;
                    cboPhuongPhapThucTe.EditValue = null;

                    if (cbbPtttGroup.Enabled && !cbbPtttGroup.ReadOnly)
                    {
                        cbbPtttGroup.EditValue = null;
                    }

                    txtConclude.EditValue = null;
                    txtDescription.EditValue = null;
                    txtResultNote.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}

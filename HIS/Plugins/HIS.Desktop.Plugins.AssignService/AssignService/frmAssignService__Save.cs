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
using ACS.EFMODEL.DataModels;
using DevExpress.Office.Utils;
using EMR.Filter;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.BackendData.ADO;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.AssignService.ADO;
using HIS.Desktop.Plugins.AssignService.Config;
using HIS.Desktop.Plugins.AssignService.Resources;
using HIS.Desktop.Plugins.Library.AlertWarningFee;
using HIS.Desktop.Print;
using HIS.Desktop.Utilities.Extensions;
using HIS.UC.Icd.ADO;
using HIS.UC.SecondaryIcd.ADO;
using Inventec.Common.Adapter;
using Inventec.Common.Logging;
using Inventec.Common.SignLibrary.DTO;
using Inventec.Core;
using Inventec.Desktop.Common.LibraryMessage;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.AssignService.AssignService
{
    public partial class frmAssignService : HIS.Desktop.Utility.FormBase
    {
        private void ProcessSaveData(bool isSaveAndPrint, bool printTH, bool isSaveAndShow, bool isSign = false, bool isPrintDocumentSigned = false)
        {
            try
            {
                if (!ValidForSave()) return;

                if (this.gridViewServiceProcess.IsEditing)
                    this.gridViewServiceProcess.CloseEditor();

                if (this.gridViewServiceProcess.FocusedRowModified)
                    this.gridViewServiceProcess.UpdateCurrentRow();

                bool isValid = true;

                List<SereServADO> serviceCheckeds__Send = this.ServiceIsleafADOs.FindAll(o => o.IsChecked);
                if (serviceTypeIdRequired != null && serviceTypeIdRequired.Count > 0)
                {
                    var serviceTypeInGrid = serviceCheckeds__Send.Select(o => new { o.TDL_SERVICE_NAME, o.SERVICE_TYPE_ID, o.TEST_SAMPLE_TYPE_ID }).ToList();
                    var lstServiceName = serviceTypeInGrid.Where(item => serviceTypeIdRequired.Exists(o => o == item.SERVICE_TYPE_ID) && item.TEST_SAMPLE_TYPE_ID <= 0).Select(o => o.TDL_SERVICE_NAME);
                    if (lstServiceName != null && lstServiceName.Count() > 0)
                    {
                        DevExpress.XtraEditors.XtraMessageBox.Show(String.Format("Dịch vụ {0} bắt buộc chọn Loại mẫu bệnh phẩm xét nghiệm", String.Join(", ", lstServiceName.ToList())), HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageBoxButtons.OK);
                        return;
                    }
                }
                if (ucIcdYhct != null)
                    isValid = isValid && (bool)icdYhctProcessor.ValidationIcd(ucIcdYhct);
                if (ucSecondaryIcdYhct != null)
                    isValid = isValid && subIcdYhctProcessor.GetValidate(ucSecondaryIcdYhct);
                isValid = isValid && this.Valid(serviceCheckeds__Send);
                isValid = isValid && this.CheckIcd(new List<V_HIS_TREATMENT_BED_ROOM> { new V_HIS_TREATMENT_BED_ROOM() { TREATMENT_ID = currentTreatment.ID, ICD_CODE = txtIcdCode.Text.Trim(), ICD_SUB_CODE = txtIcdSubCode.Text.Trim() } });
                List<HIS_ICD_SERVICE> icdServicePhacDos = null;
                List<HIS_ICD> icdFromUc = GetIcdCodeListFromUcIcd();
                if (icdFromUc != null && icdFromUc.Count > 0)
                {
                    MOS.Filter.HisIcdServiceFilter icdServiceFilter = new HisIcdServiceFilter();
                    icdServiceFilter.ICD_CODE__EXACTs = icdFromUc.Select(o => o.ICD_CODE).Distinct().ToList();
                    icdServicePhacDos = new BackendAdapter(null).Get<List<HIS_ICD_SERVICE>>("api/HisIcdService/Get", ApiConsumer.ApiConsumers.MosConsumer, icdServiceFilter, null);

                    //isValid = isValid && ValidServiceIcdForIcdSelected(icdServices, serviceCheckeds__Send);
                    Inventec.Common.Logging.LogSystem.Debug("Valid3:" + isValid);
                    isValid = isValid && ValidServiceIcdForServiceSelected(icdFromUc, icdServicePhacDos, serviceCheckeds__Send);
                    Inventec.Common.Logging.LogSystem.Debug("Valid4:" + isValid);
                    if (!isValid && HisConfigCFG.IcdServiceHasCheck == "4")
                        return;
                    if (isValid && HisConfigCFG.IcdServiceHasCheck == "5")
                    {
                        icdFromUc = GetIcdCodeListFromUcIcd();
                        icdServiceFilter = new HisIcdServiceFilter();
                        icdServiceFilter.ICD_CODE__EXACTs = icdFromUc.Select(o => o.ICD_CODE).Distinct().ToList();
                        icdServicePhacDos = new BackendAdapter(null).Get<List<HIS_ICD_SERVICE>>("api/HisIcdService/Get", ApiConsumer.ApiConsumers.MosConsumer, icdServiceFilter, null);
                    }
                }
                else if (HisConfigCFG.IcdServiceHasCheck == "3" && serviceCheckeds__Send != null && serviceCheckeds__Send.Count > 0)
                {
                    MOS.Filter.HisIcdServiceFilter icdServiceFilter = new HisIcdServiceFilter();
                    icdServiceFilter.SERVICE_IDs = serviceCheckeds__Send.Select(o => o.SERVICE_ID).Distinct().ToList();
                    icdServicePhacDos = new BackendAdapter(new CommonParam()).Get<List<HIS_ICD_SERVICE>>("api/HisIcdService/Get", ApiConsumer.ApiConsumers.MosConsumer, icdServiceFilter, null);

                    if (icdServicePhacDos != null && icdServicePhacDos.Count > 0 && icdFromUc != null && icdFromUc.Count > 0)
                    {
                        icdServicePhacDos = icdServicePhacDos.Where(o => !icdFromUc.Select(p => p.ICD_CODE).Contains(o.ICD_CODE)).ToList();
                    }
                    if (icdServicePhacDos != null && icdServicePhacDos.Count > 0)
                    {
                        frmMissingIcd frmWaringConfigIcdService = new frmMissingIcd(icdFromUc, serviceCheckeds__Send, this.currentModule, icdServicePhacDos, getDataFromMissingIcdDelegate);
                        frmWaringConfigIcdService.ShowDialog();
                        if (!isYes)
                            isValid = false;
                    }
                }
                List<string> lstIcd = new List<string>();
                if (!string.IsNullOrEmpty(txtIcdCode.Text))
                {
                    var arrIcdCode = txtIcdCode.Text.Trim().Split(';');
                    foreach (var item in arrIcdCode)
                    {
                        if (!string.IsNullOrEmpty(item))
                            lstIcd.Add(item);
                    }
                }
                List<string> lstSubIcd = new List<string>();
                if (!string.IsNullOrEmpty(txtIcdSubCode.Text))
                {
                    var arrIcdCode = txtIcdSubCode.Text.Trim().Split(';');
                    foreach (var item in arrIcdCode)
                    {
                        if (!string.IsNullOrEmpty(item))
                            lstSubIcd.Add(item);
                    }
                }
                string EmptyMessage = null;
                isValid = isValid && ValidGenderServiceAllow(serviceCheckeds__Send);
                Inventec.Common.Logging.LogSystem.Debug("Valid5__ValidGenderServiceAllow:" + isValid);
                isValid = isValid && ValidSereServWithMinDuration(serviceCheckeds__Send);
                Inventec.Common.Logging.LogSystem.Debug("Valid6.1__ValidSereServWithMinDuration:" + isValid);
                isValid = isValid && ValidSereServWithCondition(serviceCheckeds__Send);
                Inventec.Common.Logging.LogSystem.Debug("Valid7__ValidSereServWithCondition:" + isValid);
                isValid = isValid && CheckMaxPatientbyDayOption(serviceCheckeds__Send);
                Inventec.Common.Logging.LogSystem.Debug("Valid8__ValidSereServWithCondition:" + isValid);
                if (lstIcd.Count > 0 || lstSubIcd.Count > 0)
                    isValid = isValid && checkContraindicated(lstIcd, lstSubIcd, icdServicePhacDos, serviceCheckeds__Send);
                Inventec.Common.Logging.LogSystem.Debug("Valid9__ValidSereServWithCondition:" + isValid);
                isValid = isValid && ValidSereServWithOtherPaySource(serviceCheckeds__Send);
                Inventec.Common.Logging.LogSystem.Debug("Valid10__ValidSereServWithOtherPaySource:" + isValid);
                isValid = isValid && ValidCheckTreatmentTypeBed(serviceCheckeds__Send, ref EmptyMessage);
                Inventec.Common.Logging.LogSystem.Debug("Valid11__ValidCheckTreatmentTypeBed:" + isValid);
                isValid = isValid && ValidSereServWithBed(serviceCheckeds__Send);
                Inventec.Common.Logging.LogSystem.Debug("Valid12__ValidSereServWithBed:" + isValid);
                //isValid = isValid && WarningAlertWarningFeeProcess(serviceCheckeds__Send);
                //Inventec.Common.Logging.LogSystem.Debug("Valid7__WarningAlertWarningFeeProcess:" + isValid);
                isValid = isValid && CheckIcdByRoom();
                Inventec.Common.Logging.LogSystem.Debug("Valid13__CheckIcdByRoom:" + isValid);
                isValid = isValid && ValidFeeForExamTreatment();
                Inventec.Common.Logging.LogSystem.Debug("Valid14__ValidFeeForExamTreatment:" + isValid);
                isValid = isValid && CheckMaxAmount(serviceCheckeds__Send);
                isValid = isValid && ValidICD();
                Inventec.Common.Logging.LogSystem.Debug("Valid15__CheckMaxAmount:" + isValid);
                if (this.USE_TIME != null && this.USE_TIME.Count > 0)
                {
                    var exits = serviceCheckeds__Send.Where(s => s.SERVICE_TYPE_ID == 1 || s.SERVICE_TYPE_ID == 12);
                    if (exits.Any())
                    {
                        MessageBox.Show(this, "Dịch vụ loại khám và dịch vụ loại khác không cho phép dự trù");
                        isValid = false;
                        return;
                    }

                }
                if (HisConfigCFG.IsCheckDepartmentInTimeWhenPresOrAssign && this.currentWorkingRoom != null && currentWorkingRoom.ROOM_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_ROOM_TYPE.ID__BUONG)
                {
                    isValid = isValid && CheckTimeInDepartment(this.intructionTimeSelecteds);
                }

                ValidConsultationReqiured(serviceCheckeds__Send, this.treatmentId);

                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => serviceCheckeds__Send), serviceCheckeds__Send));
                if (isValid)
                {
                    ChangeLockButtonWhileProcess(false);
                    AssignServiceSDO serviceReqSDO = new AssignServiceSDO();
                    serviceReqSDO.ServiceReqDetails = new List<ServiceReqDetailSDO>();
                    bool isDupicate = false;
                    this.ProcessServiceReqSDO(serviceReqSDO, serviceCheckeds__Send, ref isDupicate, treatmentId, true);
                    if (isDupicate)
                    {
                        this.ChangeLockButtonWhileProcess(true);
                        return;
                    }
                    this.ProcessServiceReqSDOForIcd(serviceReqSDO);
                    //Cập nhật với trường hợp có dịch vụ đính kèm của các dịch vụ đã chọn chỉ định
                    if (this.ServiceAttachForServicePrimary(ref serviceReqSDO, this.currentHisPatientTypeAlter.PATIENT_TYPE_ID))
                    {
                        this.SaveServiceReqCombo(serviceReqSDO, isSaveAndPrint, printTH, isSaveAndShow, isSign, isPrintDocumentSigned);
                        if (isSaveAndPrint)
                        {
                            long isClosedForm = ConfigApplicationWorker.Get<long>(AppConfigKeys.CONFIG_KEY_HIS_DESKTOP_ASSIGN_SERVICE_CLOSED_FORM_AFTER_PRINT);
                            if (isClosedForm == 1)
                            {
                                this.Dispose();
                                this.Close();
                            }
                        }
                        this.RefeshServiceDatasourceAfterSave(serviceCheckeds__Send);
                    }
                    this.isCheckAssignServiceSimultaneityOption = false;
                    this.ChangeLockButtonWhileProcess(true);
                }
            }
            catch (Exception ex)
            {
                this.ChangeLockButtonWhileProcess(true);
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        V_HIS_SERVICE_REQ vServiceReq;
        private void LoadVServiceReq()
        {
            try
            {
                if (serviceReqParentId == null)
                    return;
                CommonParam param = new CommonParam();
                HisServiceReqViewFilter filter = new HisServiceReqViewFilter();
                filter.ID = serviceReqParentId;
                vServiceReq = new BackendAdapter(param)
                        .Get<List<MOS.EFMODEL.DataModels.V_HIS_SERVICE_REQ>>("api/HisServiceReq/GetView", ApiConsumers.MosConsumer, filter, param).FirstOrDefault();

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private bool CheckMaxAmount(List<SereServADO> serviceCheckeds__Send, List<long> TreatmentIds = null)
        {
            bool IsValid = true;
            try
            {
                dicMaxAmount = new Dictionary<long?, List<string>>();
                decimal totalAmountSereServ = 0;
                HisSereServFilter fitler = new HisSereServFilter();
                if (TreatmentIds != null && TreatmentIds.Count > 0)
                {
                    fitler.TREATMENT_IDs = TreatmentIds;
                }
                else
                {
                    fitler.TREATMENT_ID = treatmentId;
                }
                var dtTotalSereServ = new BackendAdapter(null).Get<List<HIS_SERE_SERV>>("api/HisSereServ/Get", ApiConsumer.ApiConsumers.MosConsumer, fitler, null);

                List<string> lstMess = new List<string>();
                foreach (var item in serviceCheckeds__Send)
                {
                    if (dtTotalSereServ != null && dtTotalSereServ.Count > 0)
                    {
                        var dtTotalSereServTmp = dtTotalSereServ.Where(o => o.IS_NO_EXECUTE != 1 && o.IS_DELETE != 1 && o.SERVICE_ID == item.SERVICE_ID).ToList();
                        if (dtTotalSereServTmp != null && dtTotalSereServTmp.Count > 0)
                        {
                            totalAmountSereServ = dtTotalSereServTmp.Sum(o => o.AMOUNT);
                        }
                    }
                    decimal amountPlus = item.AMOUNT;
                    if (assignMulti)
                    {
                        if (dicServiceReqList != null && dicServiceReqList.Count > 0 && dicServiceReqList.ContainsKey(TreatmentIds[0]))
                        {
                            var sdo = dicServiceReqList[TreatmentIds[0]];
                            amountPlus = GetAmountPlus(sdo, item, TreatmentIds[0], ref totalAmountSereServ);
                        }
                        else
                        {
                            amountPlus = GetAmountPlus(serviceReqComboResultSDO, item, TreatmentIds[0], ref totalAmountSereServ);
                        }
                    }
                    else
                    {
                        amountPlus = GetAmountPlus(serviceReqComboResultSDO, item, treatmentId, ref totalAmountSereServ);
                    }
                    if (item.MAX_AMOUNT.HasValue && (totalAmountSereServ + amountPlus) > item.MAX_AMOUNT)
                    {
                        if (!dicMaxAmount.ContainsKey(item.MAX_AMOUNT))
                        {
                            dicMaxAmount[item.MAX_AMOUNT] = new List<string>();
                        }
                        dicMaxAmount[item.MAX_AMOUNT].Add(item.TDL_SERVICE_NAME);
                    }
                }

                foreach (var item in dicMaxAmount)
                {
                    lstMess.Add(String.Format(ResourceMessage.DichVuVuotQuaSoLuongThucHien, string.Join(",", item.Value), item.Key));
                }
                if (lstMess != null && lstMess.Count > 0)
                {
                    if (MessageBox.Show(string.Join(". ", lstMess) + ". Bạn có muốn tiếp tục không?", Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaCanhBao), MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.No)
                    {
                        IsValid = false;
                    }
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return IsValid;
        }

        private decimal GetAmountPlus(HisServiceReqListResultSDO sdo, SereServADO item, long treatmenId, ref decimal totalAmountSereServ)
        {
            decimal rs = item.AMOUNT;
            try
            {
                if (sdo != null && sdo.SereServs != null)
                {
                    var sereServOld = sdo.SereServs.FirstOrDefault(o => o.SERVICE_ID == item.ID && o.TDL_TREATMENT_ID == treatmenId);
                    if (sereServOld != null)
                    {
                        if (sereServOld.AMOUNT == item.AMOUNT)
                        {
                            rs = 0;
                            totalAmountSereServ = 0;
                        }
                        else if (sereServOld.AMOUNT < item.AMOUNT)
                            rs = item.AMOUNT - sereServOld.AMOUNT;
                    }
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return rs;
        }

        /// <summary>
        /// Nếu có dịch vụ giường đủ thông tin giường thì chỉ được chỉ định 1 ngày
        /// Nếu hồ sơ trong buồng có chỉ định giường mà chưa có thông tin giường thì thông báo bổ sung
        /// </summary>
        /// <param name="serviceCheckeds__Send"></param>
        /// <returns></returns>
        private bool ValidSereServWithBed(List<SereServADO> serviceCheckeds__Send)
        {
            bool result = true;
            try
            {
                List<SereServADO> listBed = serviceCheckeds__Send.Where(o => o.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__G).ToList();
                if (!HisConfigCFG.AssignBedServiceWithBedInfo)
                    return result;
                if (this.IsTreatmentInBedRoom)
                {
                    if (listBed != null && listBed.Count > 0)
                    {
                        List<SereServADO> listBedMissInfo = listBed.Where(o => !o.BedId.HasValue).ToList();
                        if ((listBedMissInfo == null || listBedMissInfo.Count <= 0) && intructionTimeSelecteds.Count > 1)
                        {
                            this.txtInstructionTime.Focus();
                            this.txtInstructionTime.SelectAll();
                            MessageBox.Show(ResourceMessage.DichVuCoThongTinGiuongChiDuocChiDinhTrongNgay, MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao));
                            result = false;
                        }
                        else if (listBedMissInfo != null && listBedMissInfo.Count > 0 && MessageBox.Show(string.Format(ResourceMessage.DichVuThieuThongTinGiuong, string.Join(",", listBedMissInfo.Select(s => s.TDL_SERVICE_CODE))), MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageBoxButtons.OKCancel) == System.Windows.Forms.DialogResult.OK)
                        {
                            result = false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private bool ValidCheckTreatmentTypeBed(List<SereServADO> serviceCheckeds__Send, ref string MessageType, List<V_HIS_TREATMENT_BED_ROOM> lst = null)
        {
            bool result = true;
            try
            {
                List<SereServADO> listBed = serviceCheckeds__Send.Where(o => o.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__G).ToList();
                if (listBed != null && listBed.Count > 0 && (HisConfigCFG.BedServiceType_NotAllow_For_OutPatient == "1" || HisConfigCFG.BedServiceType_NotAllow_For_OutPatient == "2"))
                {
                    if (lst != null && lst.Count > 0)
                    {
                        bool resultTemp = true;
                        foreach (var item in lst.GroupBy(o => o.TREATMENT_ID))
                        {
                            if (item.First().TDL_TREATMENT_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTNOITRU)
                            {
                                MessageType += "Bệnh nhân " + item.First().TDL_PATIENT_NAME + " có mã điều trị " + item.First().TREATMENT_CODE + ".\r\n";
                                result = false;
                                if (!result)
                                    resultTemp = result;
                            }
                        }
                        result = resultTemp;
                    }
                    else if (currentTreatment.TDL_TREATMENT_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTNOITRU)
                    {
                        if ((HisConfigCFG.BedServiceType_NotAllow_For_OutPatient == "1" && MessageBox.Show(ResourceMessage.KhongPhaiNoiTruChiDinhGiuong, MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) != System.Windows.Forms.DialogResult.Yes) || (HisConfigCFG.BedServiceType_NotAllow_For_OutPatient == "2" && MessageBox.Show(ResourceMessage.ChanKhongPhaiNoiTruChiDinhGiuong, MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageBoxButtons.OK) == System.Windows.Forms.DialogResult.OK))
                        {
                            result = false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private void ValidConsultationReqiured(List<SereServADO> serviceCheckeds__Send, long treatmentId)
        {
            try
            {
                List<HIS_DEBATE> lstHisDebate = new List<HIS_DEBATE>();
                List<V_HIS_SERVICE> lstServiceWarn = new List<V_HIS_SERVICE>();
                V_HIS_ROOM currentWorkingRoom = null;
                string message = "";

                if (cboAssignRoom.EditValue != null)
                {
                    currentWorkingRoom = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_ROOM>().First(o => o.ID == Convert.ToInt64(cboAssignRoom.EditValue));
                }

                if (serviceCheckeds__Send != null && serviceCheckeds__Send.Count > 0)
                {
                    CommonParam param = new CommonParam();

                    var lstHisService = lstService.Where(o => o.MUST_BE_CONSULTED == 1 && serviceCheckeds__Send.Select(p => p.SERVICE_ID).ToList().Exists(p => p == o.ID)).ToList();

                    if (lstHisService != null && lstHisService.Count > 0 && currentWorkingRoom != null)
                    {
                        HisDebateFilter DebateFilter = new HisDebateFilter();
                        DebateFilter.DEPARTMENT_ID = currentWorkingRoom.DEPARTMENT_ID;
                        DebateFilter.SERVICE_IDs = lstHisService.Select(o => o.ID).ToList();
                        DebateFilter.TREATMENT_ID = treatmentId;

                        lstHisDebate = new BackendAdapter(param).Get<List<HIS_DEBATE>>(HisRequestUriStore.HIS_DEBATE_GET, ApiConsumers.MosConsumer, DebateFilter, param);

                        if (lstHisDebate != null && lstHisDebate.Count > 0)
                        {
                            foreach (var itemS in lstHisService)
                            {
                                var check = lstHisDebate.FirstOrDefault(o => o.SERVICE_ID == itemS.ID);
                                if (check == null)
                                {
                                    lstServiceWarn.Add(itemS);
                                }
                            }
                        }
                        else
                        {
                            lstServiceWarn = lstHisService;
                        }
                    }
                }

                if (lstServiceWarn != null && lstServiceWarn.Count > 0)
                {
                    message = String.Format(ResourceMessage.KhoaChiDinhChuaTaoBienBanHoiChuan, String.Join(",", lstServiceWarn.Select(o => o.SERVICE_NAME).ToList()));

                    frmServiceDebateConfirm frm = new frmServiceDebateConfirm(this.currentModule, lstServiceWarn, lstHisDebate, message, treatmentId);
                    frm.ShowDialog();
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }


        private bool WarningAlertWarningFeeProcess(List<SereServADO> serviceCheckeds__Send)
        {
            bool valid = true;
            try
            {
                string messageErr = "";
                decimal tmp = 0;
                decimal tongtienThuocPhatSinh = GetDefaultSerServTotalPrice(ref tmp, HisConfigCFG.PatientTypeId__BHYT);
                AlertWarningFeeManager alertWarningFeeManager = new AlertWarningFeeManager();
                if (!alertWarningFeeManager.RunOption(treatmentId, currentHisPatientTypeAlter.PATIENT_TYPE_ID, currentHisPatientTypeAlter.TREATMENT_TYPE_ID, currentHisPatientTypeAlter.HEIN_MEDI_ORG_CODE, HisConfigCFG.PatientTypeId__BHYT, totalHeinPriceByTreatment, HisConfigCFG.IsUsingWarningHeinFee, tongtienThuocPhatSinh, ref messageErr, true))
                {
                    valid = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return valid;
        }

        private bool ValidFeeForExamTreatment()
        {
            bool result = true;
            try
            {
                if ((HisConfigCFG.WarningOverTotalPatientPrice__IsCheck == "2" || HisConfigCFG.WarningOverTotalPatientPrice__IsCheck == "3") && this.currentHisPatientTypeAlter != null && this.currentHisPatientTypeAlter.TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__KHAM)
                {
                    decimal tmp = 0;
                    decimal tongtienBHYT = GetDefaultSerServTotalPrice(ref tmp, HisConfigCFG.PatientTypeId__BHYT);
                    decimal totalPrice = GetDefaultSerServTotalPrice(ref tmp);
                    decimal checkPrice = this.transferTreatmentFee + totalPrice - tongtienBHYT;

                    if (checkPrice > 0 && MessageBox.Show(String.Format(ResourceMessage.BenhNhanDangThieuVienPhi,
                        Inventec.Common.Number.Convert.NumberToString(checkPrice, ConfigApplications.NumberSeperator)), "Cảnh báo",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == System.Windows.Forms.DialogResult.No)
                    {
                        result = false;
                    }
                }
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private bool ValidGenderServiceAllow(List<SereServADO> serviceCheckeds__Send)
        {
            bool valid = true;
            try
            {
                if (this.currentHisTreatment != null)
                {
                    // check giới tính

                    var genderCheck = GetDiffGender(serviceCheckeds__Send, this.currentHisTreatment.TDL_PATIENT_GENDER_ID);
                    if (genderCheck != null && genderCheck.Count() > 0)
                    {
                        string gender = genderCheck.FirstOrDefault().GENDER_ID == 1 ? "nữ" : "nam";

                        MessageManager.Show(ResourceMessage.DichVuKhongChiDinhChoGioiTinh + " " + gender + ": " + String.Join("; ", genderCheck.Select(o => o.TDL_SERVICE_NAME).ToArray()));
                        return false;
                    }

                    // check tuổi từ - đến (DVKT)
                    var ageDate = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(this.currentHisTreatment.TDL_PATIENT_DOB);
                    //int age = DateTime.Now.Year - int.Parse(this.currentHisTreatment.TDL_PATIENT_DOB.ToString().Substring(0, 4));
                    TimeSpan timeSpan2 = System.DateTime.Now.Date - ageDate.Value.Date;
                    long ticks = timeSpan2.Ticks;
                    System.DateTime dateTime = new System.DateTime(ticks);
                    int ageMonth = (dateTime.Year - 1) * 12 + dateTime.Month - 1;
                    //Inventec.Common.Logging.LogSystem.Debug("age: " + age);
                    var checkAge = serviceCheckeds__Send.Where(o => (o.AGE_FROM.HasValue && o.AGE_FROM > ageMonth) || (o.AGE_TO.HasValue && o.AGE_TO < ageMonth));

                    if (checkAge != null && checkAge.Count() > 0)
                    {
                        MessageManager.Show(ResourceMessage.DoTuoiCuaBNKhongPhuHopVoiDieuKienCuaDV + String.Join("; ", checkAge.Select(o => o.TDL_SERVICE_NAME).ToArray()) + ResourceMessage._VuiLongChonDVKhac);
                        return false;
                    }

                    // check dịch vụ giường với diện điều trị là khám, điều trị ngoại trú, điều trị ban ngày
                    //if (this.currentHisTreatment.TDL_TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__KHAM || this.currentHisTreatment.TDL_TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTBANNGAY || this.currentHisTreatment.TDL_TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTNGOAITRU)
                    //{
                    //    var treatmentType = BackendDataWorker.Get<HIS_TREATMENT_TYPE>().FirstOrDefault(o => o.ID == this.currentHisTreatment.TDL_TREATMENT_TYPE_ID);
                    //    var dichVuGiuong = serviceCheckeds__Send.Where(o => o.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__G).ToList();
                    //    if (dichVuGiuong != null && dichVuGiuong.Count() > 0 && treatmentType != null)
                    //    {
                    //        if (MessageBox.Show(ResourceMessage.DienDieuTriCuaBNLa + treatmentType.TREATMENT_TYPE_NAME + ResourceMessage._BanCoMuonChiDinhGiuong, MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == DialogResult.No)
                    //            return false;
                    //    }
                    //}
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return valid;
        }

        private bool ValidOnlyShowNoticeService(SereServADO serviceChecked__Send)
        {
            bool valid = true;
            try
            {
                List<SereServADO> SereServADOSelecteds = new List<SereServADO>();
                SereServADOSelecteds.Add(serviceChecked__Send);

                valid = ValidOnlyShowNoticeService(SereServADOSelecteds);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return valid;
        }

        private bool ValidOnlyShowNoticeService(List<SereServADO> serviceCheckeds__Send)
        {
            bool valid = true;
            try
            {
                string messNotice = "";
                var svNotice = (serviceCheckeds__Send != null && serviceCheckeds__Send.Count > 0) ? serviceCheckeds__Send.Where(o => !String.IsNullOrEmpty(o.NOTICE)).ToList() : null;
                messNotice = (svNotice != null && svNotice.Count > 0) ? String.Join(",", svNotice.Select(o => o.TDL_SERVICE_NAME + ":" + o.NOTICE).ToArray()) : "";

                if (!String.IsNullOrEmpty(messNotice))
                {
                    MessageManager.Show(messNotice);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return valid;
        }

        private bool ValidServiceAttackAlert(List<SereServADO> serviceCheckeds__Send)
        {
            bool valid = true;
            try
            {
                string messNotice = "";
                var svNotice = (serviceCheckeds__Send != null && serviceCheckeds__Send.Count > 0) ? serviceCheckeds__Send.Where(o => !String.IsNullOrEmpty(o.NOTICE)).ToList() : null;

                messNotice = (svNotice != null && svNotice.Count > 0) ? String.Join(",", svNotice.Select(o => o.TDL_SERVICE_NAME + ":" + o.NOTICE).ToArray()) : "";

                if (!String.IsNullOrEmpty(messNotice))
                {
                    valid = false;
                    MessageManager.Show(messNotice);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return valid;
        }

        /// <summary>
        /// #18108
        /// Bổ sung key cấu hình cho phép TG tối thiểu của những dịch vụ được cấu hình.
        ///Nếu set 1 DV có thời gian tối thiểu thì PM chặn không cho phép chỉ định ĐT thanh toán: BHYT và đưa ra thông báo Bạn có muốn chuyển đổi ĐT thanh toán sang VP -> Có hoặc Không -> Có: PM tự chuyển đổi ĐT thanh toán sang VP, Không: PM tự động ẩn nút "Lưu(Ctrl S)"
        ///Nếu để #1 thì chỉ dừng lại cảnh báo.
        ///Mặc định là #1.
        /// </summary>
        /// <param name="serviceCheckeds__Send"></param>
        /// <returns></returns>
        private bool ValidSereServWithMinDuration(List<SereServADO> serviceCheckeds__Send)
        {
            bool valid = true;
            try
            {
                List<HIS_SERE_SERV> sereServMinDurations = getSereServWithMinDuration(serviceCheckeds__Send, this.currentTreatment.PATIENT_ID);
                if (sereServMinDurations != null && sereServMinDurations.Count > 0)
                {
                    string sereServMinDurationStr = "";
                    foreach (var item in sereServMinDurations)
                    {
                        sereServMinDurationStr += item.TDL_SERVICE_CODE + " - " + item.TDL_SERVICE_NAME + " - " +
                           Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(item.TDL_INTRUCTION_TIME) +
                           " (" + item.TDL_SERVICE_REQ_CODE +
                           "); ";
                    }
                    if (HisConfigCFG.IsSereServMinDurationAlert == 1)
                    {
                        if (MessageBox.Show(string.Format(ResourceMessage.SereServMinDurationAlert__BanCoMuonChuyenDoiDTTTSangVienPhi, sereServMinDurationStr), MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageBoxButtons.YesNo) == DialogResult.Yes)
                        {
                            foreach (var sv in serviceCheckeds__Send)
                            {
                                //Thực hiện tự động chuyển đổi đối tượng sang viện phí                     
                                if (sereServMinDurations.Any(o => o.SERVICE_ID == sv.SERVICE_ID))
                                {
                                    sv.PATIENT_TYPE_ID = HisConfigCFG.PatientTypeId__VP;
                                }
                            }
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else if (HisConfigCFG.IsSereServMinDurationAlert == 2)
                    {
                        if (MessageBox.Show(string.Format(ResourceMessage.DichVuCoThoiGianChiDinhNamTrongKhoangThoiGianKhongChoPhep, sereServMinDurationStr), MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageBoxButtons.YesNo) == DialogResult.Yes)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        if (HisConfigCFG.IsSereServMinDurationAlert == 0 || (HisConfigCFG.IsSereServMinDurationAlert != 1 && HisConfigCFG.IsSereServMinDurationAlert != 2))
                        {
                            if (MessageBox.Show(string.Format(ResourceMessage.DichVuCoThoiGianChiDinhNamTrongKhoangThoiGianKhongChoPhep, sereServMinDurationStr), MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageBoxButtons.YesNo) == DialogResult.Yes)
                            {
                                return true;
                            }
                            else
                            {
                                return false;
                            }
                        }
                    }
                }                
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return valid;
        }

        /// <summary>
        /// - Khi người dùng nhấn "Lưu", nếu kiểm tra tồn tại 1 dịch vụ được check chọn là dịch vụ điều kiện (HIS_SERVICE có IS_CONDITIONED = 1) nhưng người dùng không chọn điều kiện (cột "Điều kiện" không chọn giá trị) thì hiển thị cảnh báo và không cho lưu:
        ///"Dịch vụ XXXX, YYYY chưa được nhập điều kiện" (trong đó XXXX, YYYY là tên các dịch vụ điều kiện chưa được nhập điều kiện)
        /// </summary>
        /// <param name="serviceCheckeds__Send"></param>
        /// <returns></returns>
        private bool ValidSereServWithCondition(List<SereServADO> serviceCheckeds__Send)
        {
            bool valid = true;
            try
            {
                if (serviceCheckeds__Send != null && serviceCheckeds__Send.Count > 0)
                {
                    string sereServConditionStr = "";
                    foreach (var item in serviceCheckeds__Send)
                    {
                        var dataCondition = BranchDataWorker.ServicePatyWithListPatientType(item.SERVICE_ID, new List<long> { item.PATIENT_TYPE_ID });
                        List<V_HIS_SERVICE_PATY> dataSource = new List<V_HIS_SERVICE_PATY>(); 
                        long instructionTime = this.intructionTimeSelecteds != null && this.intructionTimeSelecteds.Count > 0 ? this.intructionTimeSelecteds.FirstOrDefault() : 0;
                        long? intructionNumByType = null;
                        List<HIS_SERE_SERV> sameServiceType = this.sereServWithTreatment != null ? this.sereServWithTreatment.Where(o => o.TDL_SERVICE_TYPE_ID == item.SERVICE_TYPE_ID).ToList() : null;
                        List<HIS_SERE_SERV> sameService = this.sereServWithTreatment != null ? this.sereServWithTreatment.Where(o => o.SERVICE_ID == item.SERVICE_ID).ToList() : null;
                        intructionNumByType = sameServiceType != null ? (long)sameServiceType.Count() + 1 : 1;
                        var intructionNum = sameService != null ? (long)sameService.Count() + 1 : 1;
                        foreach (var con in dataCondition)
                        {
                            var dt = MOS.ServicePaty.ServicePatyUtil.GetApplied(new List<V_HIS_SERVICE_PATY>() { con }, item.TDL_EXECUTE_BRANCH_ID, item.TDL_EXECUTE_ROOM_ID, this.requestRoom.ID, this.requestRoom.DEPARTMENT_ID, instructionTime, this.currentHisTreatment.IN_TIME, item.SERVICE_ID, item.PATIENT_TYPE_ID, intructionNum, intructionNumByType, item.PackagePriceId, con.SERVICE_CONDITION_ID, this.currentHisTreatment.TDL_PATIENT_CLASSIFY_ID, null);
                            if (dt != null)
                                dataSource.Add(dt);
                        }
                        dataCondition = dataSource;
                        if (dataCondition != null && dataCondition.Count > 0 && lstConditionService != null && lstConditionService.Count > 0)
                        {
                            dataCondition = dataCondition.Where(o => lstConditionService.Exists(p => p.SERVICE_ID == item.SERVICE_ID && p.ID == o.SERVICE_CONDITION_ID)).ToList();
                        }
                        if (dataCondition != null && dataCondition.Count > 0 && dataCondition.Exists(t => t.SERVICE_CONDITION_ID.HasValue && t.SERVICE_CONDITION_ID > 0) && !dataCondition.Exists(t => t.SERVICE_CONDITION_ID == null || t.SERVICE_CONDITION_ID == 0))
                        {
                            dataCondition = dataCondition.Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE && o.SERVICE_CONDITION_ID.HasValue && o.SERVICE_CONDITION_ID > 0 && o.SERVICE_ID == item.SERVICE_ID).ToList();
                            if (dataCondition != null && dataCondition.Count > 0)
                            {
                                List<V_HIS_SERVICE_PATY> dataConditionTmps = new List<V_HIS_SERVICE_PATY>();
                                foreach (var itemCon in dataCondition)
                                {
                                    if (dataConditionTmps.Count == 0 || !dataConditionTmps.Exists(t => t.SERVICE_CONDITION_NAME == itemCon.SERVICE_CONDITION_NAME && t.HEIN_RATIO == itemCon.HEIN_RATIO))
                                    {
                                        dataConditionTmps.Add(itemCon);
                                    }
                                }
                                dataCondition.Clear();
                                dataCondition.AddRange(dataConditionTmps);
                            }
                        }
                        else
                        {
                            dataCondition = null;
                        }
                        if (dataCondition != null && dataCondition.Count > 0 && (item.SERVICE_CONDITION_ID ?? 0) <= 0)
                        {
                            sereServConditionStr += item.TDL_SERVICE_NAME + ",";
                        }
                    }

                    if (!String.IsNullOrEmpty(sereServConditionStr))
                    {
                        sereServConditionStr = sereServConditionStr.TrimEnd(',');
                        MessageBox.Show(string.Format(ResourceMessage.SereServConditionAlert__DVChuaDuocNhapDieuKien, sereServConditionStr), MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao));
                        Inventec.Common.Logging.LogSystem.Warn("ValidSereServWithCondition: valid = false_____" + sereServConditionStr);
                        valid = false;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return valid;
        }

        /// <summary>
        /// - Khi nhấn nút "Lưu", nếu tồn tại dịch vụ chưa chọn "Nguồn khác" thì hiển thị cảnh báo: "Bạn chưa chọn "Nguồn khác" đối với dịch vụ XXX, YYY", không cho phép lưu.
        ///Trong đó: XXX, YYY là tên các dịch vụ chưa chọn nguồn khác và có Đối tượng thanh toán có cấu hình "Nguồn khác"
        ///- Khi lưu, gửi thông tin nguồn khác vào OtherPaySourceId vào SDO để xử lý lưu thông tin vào server
        /// </summary>
        /// <param name="serviceCheckeds__Send"></param>
        /// <returns></returns>
        private bool ValidSereServWithOtherPaySource(List<SereServADO> serviceCheckeds__Send)
        {
            bool valid = true;
            try
            {
                if (serviceCheckeds__Send != null && serviceCheckeds__Send.Count > 0)
                {
                    string sereServOtherpaysourceStr = "";
                    foreach (var item in serviceCheckeds__Send)
                    {
                        var workingPatientType = currentPatientTypes.Where(t => t.ID == item.PATIENT_TYPE_ID).FirstOrDefault();
                        if (workingPatientType != null && !String.IsNullOrEmpty(workingPatientType.OTHER_PAY_SOURCE_IDS) && (item.OTHER_PAY_SOURCE_ID ?? 0) <= 0)
                        {
                            sereServOtherpaysourceStr += item.TDL_SERVICE_NAME + ",";
                        }
                    }

                    if (!String.IsNullOrEmpty(sereServOtherpaysourceStr))
                    {
                        sereServOtherpaysourceStr = sereServOtherpaysourceStr.TrimEnd(',');
                        MessageBox.Show(string.Format(ResourceMessage.SereServOtherpaySourceAlert__DVChuaDuocNhapNguonChiTra, sereServOtherpaysourceStr), MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao));
                        Inventec.Common.Logging.LogSystem.Warn("ValidSereServWithOtherPaySource: valid = false_____" + sereServOtherpaysourceStr);
                        valid = false;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return valid;
        }

        private bool ValidServiceIcdForIcdSelected(List<HIS_ICD_SERVICE> icdServices, List<SereServADO> serviceCheckeds__Send)
        {
            bool valid = true;
            try
            {
                string serviceErrStr = "";
                //string icdServiceCFG = HisConfigCFG.IcdServiceHasCheck;
                List<SereServADO> sereServAdoResult = new List<SereServADO>();
                bool checkServiceIcd = this.CheckIcdServiceForService(icdServices, ref serviceErrStr, serviceCheckeds__Send, ref sereServAdoResult);
                if (HisConfigCFG.IcdServiceHasCheck == "1" && !checkServiceIcd)
                {
                    Inventec.Common.Logging.LogSystem.Debug(ResourceMessage.DichVuChuaDuocCauHinhICDDichVu + serviceErrStr);
                    MessageManager.Show(String.Format(ResourceMessage.DichVuChuaDuocCauHinhICDDichVu, serviceErrStr));
                    valid = false;
                }
                else if (HisConfigCFG.IcdServiceHasCheck == "2" && !checkServiceIcd)
                {
                    valid = MessageBox.Show(String.Format(ResourceMessage.DichVuChuaDuocCauHinhICDDichVu, serviceErrStr) + " " + ResourceMessage.BanCoMuonTiepTuc, MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageBoxButtons.OKCancel) == System.Windows.Forms.DialogResult.OK;
                }
                //else if (icdServiceCFG == "3" && !checkServiceIcd)
                //{
                //    valid = false;
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return valid;
        }

        private bool ValidServiceIcdForServiceSelected(List<HIS_ICD> icdFromUc, List<HIS_ICD_SERVICE> icdServices, List<SereServADO> serviceCheckeds__Send)
        {
            bool valid = true;
            try
            {
                isYes = false;
                string serviceErrStr = "";
                //string icdServiceCFG = HisConfigCFG.IcdServiceHasCheck;
                List<SereServADO> sereServAdoResult = new List<SereServADO>();
                bool checkServiceIcd = this.CheckIcdServiceForService(icdServices, ref serviceErrStr, serviceCheckeds__Send, ref sereServAdoResult);
                if (HisConfigCFG.IcdServiceHasCheck == "1" && !checkServiceIcd)
                {
                    Inventec.Common.Logging.LogSystem.Debug(ResourceMessage.DichVuChuaDuocCauHinhICDDichVu + serviceErrStr);
                    MessageManager.Show(String.Format(ResourceMessage.DichVuChuaDuocCauHinhICDDichVu, serviceErrStr));
                    valid = false;
                }
                else if (HisConfigCFG.IcdServiceHasCheck == "2" && !checkServiceIcd)
                {
                    frmWaringConfigIcdService frmWaringConfigIcdService = new frmWaringConfigIcdService(icdFromUc, sereServAdoResult, this.currentModule, getDataFromOtherFormDelegate);
                    frmWaringConfigIcdService.ShowDialog();
                    if (!isYes)
                        valid = false;
                }
                else if ((HisConfigCFG.IcdServiceHasCheck == "4" || HisConfigCFG.IcdServiceHasCheck == "3" || HisConfigCFG.IcdServiceHasCheck == "5") && !checkServiceIcd
                    && sereServAdoResult != null && sereServAdoResult.Count > 0)
                {
                    MOS.Filter.HisIcdServiceFilter icdServiceFilter = new HisIcdServiceFilter();
                    icdServiceFilter.SERVICE_IDs = sereServAdoResult.Select(o => o.SERVICE_ID).Distinct().ToList();
                    List<HIS_ICD_SERVICE> icdServiceByServices = new BackendAdapter(new CommonParam()).Get<List<HIS_ICD_SERVICE>>("api/HisIcdService/Get", ApiConsumer.ApiConsumers.MosConsumer, icdServiceFilter, null);
                    if (HisConfigCFG.IcdServiceHasCheck == "4")
                        icdServiceByServices = icdServiceByServices.Where(o => o.IS_CONTRAINDICATION != IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
                    else if (HisConfigCFG.IcdServiceHasCheck == "5")
                        icdServiceByServices = icdServiceByServices.Where(o => o.IS_CONTRAINDICATION != IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE && o.IS_WARNING != IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
                    if (icdServiceByServices != null && icdServiceByServices.Count > 0 && icdFromUc != null && icdFromUc.Count > 0)
                    {
                        icdServiceByServices = icdServiceByServices.Where(o => !icdFromUc.Select(p => p.ICD_CODE).Contains(o.ICD_CODE)).ToList();
                    }

                    if (icdServiceByServices != null && icdServiceByServices.Count > 0)
                    {
                        frmMissingIcd frmWaringConfigIcdService = new frmMissingIcd(icdFromUc, sereServAdoResult, this.currentModule, icdServiceByServices, getDataFromMissingIcdDelegate, HisConfigCFG.IcdServiceHasCheck == "5", SkipIcd);
                        frmWaringConfigIcdService.ShowDialog();
                        if (isYes && HisConfigCFG.IcdServiceHasCheck == "5")
                            valid = true;
                        else
                            valid = false;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return valid;
        }

        private void SkipIcd(bool obj)
        {
            try
            {
                isYes = obj;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private bool CheckMaxPatientbyDayOption(List<SereServADO> serviceCheckeds__Send)
        {
            bool valid = true;
            try
            {
                if (HisConfigCFG.MaxPatientByDay == 1 && serviceCheckeds__Send != null && serviceCheckeds__Send.Count > 0)
                {
                    if (this.hisRoomCounters == null || this.hisRoomCounters.Count == 0)
                    {
                        this.hisRoomCounters = GetLCounter1();
                    }


                    var ProcessingRoom = this.hisRoomCounters != null ? this.hisRoomCounters.Where(p => p.MAX_PATIENT_BY_DAY > 0 && p.TOTAL_TODAY_PATIENT >= p.MAX_PATIENT_BY_DAY).ToList() : null;


                    if (ProcessingRoom != null && ProcessingRoom.Count > 0)
                    {
                        var serviceCheckeds__Send__Validmax = serviceCheckeds__Send.Where(k => k.TDL_EXECUTE_ROOM_ID > 0 && ProcessingRoom.Exists(t => t.ROOM_ID == k.TDL_EXECUTE_ROOM_ID)).Select(p => p.TDL_EXECUTE_ROOM_ID).Distinct().ToList();


                        List<string> txt_ = new List<string>();
                        string text;
                        if (serviceCheckeds__Send__Validmax != null && serviceCheckeds__Send__Validmax.Count > 0)
                        {
                            foreach (var item in serviceCheckeds__Send__Validmax)
                            {
                                Convert.ToInt32(ProcessingRoom.Where(p => p.ROOM_ID == item).FirstOrDefault().TOTAL_TODAY_PATIENT);
                                text = ProcessingRoom.Where(p => p.ROOM_ID == item).FirstOrDefault().EXECUTE_ROOM_NAME + ": Số lượng hiện tại :" + Convert.ToInt32(ProcessingRoom.Where(p => p.ROOM_ID == item).FirstOrDefault().TOTAL_TODAY_PATIENT) + ",Số lượng tối đa:" + ProcessingRoom.Where(p => p.ROOM_ID == item).FirstOrDefault().MAX_PATIENT_BY_DAY;
                                txt_.Add(text);
                                txt_.Distinct();
                            }

                            if (MessageBox.Show(string.Join("\r\n", txt_) + "\n\rBạn có muốn tiếp tục không?", Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaCanhBao), MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.No)
                            {
                                valid = false;
                            }
                        }
                    }
                }
                GetLCounter1Async();
                return valid;
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return valid;
        }

        private void CheckContinue(bool obj)
        {
            IsActionKey = obj;
        }

        private bool checkContraindicated(List<string> icd, List<string> icdSub, List<HIS_ICD_SERVICE> icdServices, List<SereServADO> serviceCheckeds__Send)
        {

            var icd_code = icd;
            var icd_sub_code = icdSub;
            bool valid = true;
            try
            {
                string serviceErrStr = "";
                long icdServiceCFG = HisConfigCFG.contraindicated;
                IsActionKey = false;
                if (icdServiceCFG == 1 || icdServiceCFG == 2)
                {
                    List<string> serviceCon = new List<string>();
                    List<string> serviceWar = new List<string>();

                    if (icd_sub_code != null && icd_sub_code.Count() > 0)
                    {
                        icd_code.AddRange(icd_sub_code);
                    }
                    icd_code = icd_code.Distinct().ToList();

                    if (icd_code != null && icd_code.Count > 0)
                    {
                        var is_condi = icdServices.Where(k => k.IS_CONTRAINDICATION == 1 && (icd_code.Contains(k.ICD_CODE)));
                        var is_warning = icdServices.Where(k => k.IS_WARNING == 1 && (icd_code.Contains(k.ICD_CODE)));
                        var serviceCheckeds = serviceCheckeds__Send.Select(p => p.SERVICE_ID);
                        if (icdServiceCFG == 2)
                        {
                            foreach (var item in serviceCheckeds)
                            {
                                string is_condiserid_name = null;
                                string is_warningid_name = null;
                                List<string> is_condiserCode = new List<string>();
                                List<string> is_warningCode = new List<string>();
                                if (is_condi != null && is_condi.Count() > 0)
                                {
                                    is_condiserCode = is_condi.Where(k => k.SERVICE_ID == item).Select(o => o.ICD_CODE).ToList();
                                    is_condiserCode.Distinct();
                                    //foreach (var item_ in is_condiserid)
                                    //{
                                    //    var is_sub_condiserid = icd_code.Where(k => k.ICD_CODE == item_).Select(o => o.ICD_SUB_CODE).ToList();
                                    //    is_sub_condiserid.Distinct();
                                    //     mess_is_sub_condiserid = string.Join(", ", is_sub_condiserid);
                                    //}
                                    is_condiserid_name = dicServices.Values.FirstOrDefault(p => p.ID == item).SERVICE_NAME;
                                }
                                if (is_warning != null && is_warning.Count() > 0)
                                {
                                    is_warningCode = is_warning.Where(k => k.SERVICE_ID == item).Select(o => o.ICD_CODE).ToList();
                                    is_warningCode.Distinct();
                                    //foreach (var item_ in is_condiserid)
                                    //{
                                    //    var is_sub_condiserid = icd_code.Where(k => k.ICD_CODE == item_).Select(o => o.ICD_SUB_CODE).ToList();
                                    //    is_sub_condiserid.Distinct();
                                    //     mess_is_sub_condiserid = string.Join(", ", is_sub_condiserid);
                                    //}
                                    is_warningid_name = dicServices.Values.FirstOrDefault(p => p.ID == item).SERVICE_NAME;
                                }

                                if (is_condiserCode != null && is_condiserCode.Count() > 0)
                                {
                                    string mess = string.Format("{0}: ", is_condiserid_name);
                                    foreach (var i in is_condi.Where(k => k.SERVICE_ID == item).ToList())
                                    {
                                        mess += String.Format("\r\n{0} - {1} - {2}", i.ICD_CODE, i.ICD_NAME, i.CONTRAINDICATION_CONTENT);
                                    }
                                    serviceCon.Add(mess);
                                    serviceCon.Distinct();
                                }

                                if (is_warningCode != null && is_warningCode.Count() > 0)
                                {
                                    string mess = string.Format("{0}: ", is_condiserid_name);
                                    foreach (var i in is_warning.Where(k => k.SERVICE_ID == item).ToList())
                                    {
                                        mess += String.Format("\r\n- {0} - {1} - {2}", i.ICD_CODE, i.ICD_NAME, i.CONTRAINDICATION_CONTENT);
                                    }
                                    serviceWar.Add(mess);
                                    serviceWar.Distinct();
                                }
                            }

                            if (serviceCon != null && serviceCon.Count > 0)
                            {
                                if (MessageBox.Show("Chặn báo chống chỉ định\r\n\r\n" + string.Join("\r\n\r\n", serviceCon), Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaCanhBao), MessageBoxButtons.OK, MessageBoxIcon.Warning) == System.Windows.Forms.DialogResult.OK)
                                {
                                    return valid = false;
                                }
                            }
                            if (serviceWar != null && serviceWar.Count > 0)
                            {
                                if (MessageBox.Show("Cảnh báo chống chỉ định\r\n\r\n" + string.Join("\r\n\r\n", serviceWar) + "\n\rBạn có muốn tiếp tục không?", Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaCanhBao), MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.No)
                                {
                                    return valid = false;
                                }
                            }
                        }
                        if (icdServiceCFG == 1)
                        {
                            var is_condiKey1 = icdServices != null ? icdServices.Where(o => o.IS_CONTRAINDICATION == 1).ToList() : null;
                            var is_warKey1 = icdServices != null ? icdServices.Where(o => o.IS_WARNING == 1).ToList() : null;
                            if (is_warKey1 != null && is_warKey1.Count() > 0)
                            {
                                MOS.Filter.HisIcdServiceFilter icdServiceFilter = new HisIcdServiceFilter();
                                icdServiceFilter.SERVICE_IDs = is_warKey1.Select(o => o.SERVICE_ID ?? 0).ToList();
                                var is_condi_allInService = new BackendAdapter(null).Get<List<HIS_ICD_SERVICE>>("api/HisIcdService/Get", ApiConsumer.ApiConsumers.MosConsumer, icdServiceFilter, null);
                                var is_war_allInService = is_condi_allInService != null ? is_condi_allInService.Where(o => o.IS_WARNING == 1).ToList() : null;
                                List<HIS_ICD_SERVICE> chanChongChiDinhWar = is_war_allInService.Where(o => serviceCheckeds.Contains(o.SERVICE_ID ?? -1)).ToList();
                                if (chanChongChiDinhWar != null && chanChongChiDinhWar.Count() > 0)
                                {
                                    FormContraindicated.frmContraindicated form = new FormContraindicated.frmContraindicated(this.currentModule, chanChongChiDinhWar, CheckContinue);
                                    form.ShowDialog();
                                    valid = IsActionKey;
                                    if (!valid)
                                        return valid;
                                }
                            }
                            if (is_condiKey1 != null && is_condiKey1.Count() > 0)
                            {
                                MOS.Filter.HisIcdServiceFilter icdServiceFilter = new HisIcdServiceFilter();
                                icdServiceFilter.SERVICE_IDs = is_condiKey1.Select(o => o.SERVICE_ID ?? 0).ToList();
                                var is_condi_allInService = new BackendAdapter(null).Get<List<HIS_ICD_SERVICE>>("api/HisIcdService/Get", ApiConsumer.ApiConsumers.MosConsumer, icdServiceFilter, null);
                                is_condi_allInService = is_condi_allInService != null ? is_condi_allInService.Where(o => o.IS_CONTRAINDICATION == 1).ToList() : null;
                                List<HIS_ICD_SERVICE> chanChongChiDinhCon = is_condi_allInService.Where(o => serviceCheckeds.Contains(o.SERVICE_ID ?? -1)).ToList();
                                if (chanChongChiDinhCon != null && chanChongChiDinhCon.Count() > 0)
                                {
                                    FormContraindicated.frmContraindicated form = new FormContraindicated.frmContraindicated(this.currentModule, chanChongChiDinhCon);
                                    form.ShowDialog();
                                    valid = false;
                                }
                            }

                        }
                    }

                }
                return valid;
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return valid;
        }
        private bool IsReloadIcd = false;
        public void getDataFromMissingIcdDelegate(object data)
        {
            List<MissingIcdADO> missingIcdADOList = new List<MissingIcdADO>();
            try
            {
                isNotProcessWhileChangedTextSubIcd = true;
                if (data != null && data is List<MissingIcdADO>)
                {
                    missingIcdADOList = (List<MissingIcdADO>)data;
                    if (missingIcdADOList != null && missingIcdADOList.Count > 0)
                    {
                        this.isYes = true;
                        var icdMainCheck = missingIcdADOList.FirstOrDefault(o => o.ICD_MAIN_CHECK);

                        if (icdMainCheck != null)
                        {
                            var icdMainData = BackendDataWorker.Get<HIS_ICD>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).FirstOrDefault(o => o.ICD_CODE == icdMainCheck.ICD_CODE);
                            if (icdMainData != null)
                            {
                                cboIcds.EditValue = icdMainData.ID;
                                txtIcdCode.Text = icdMainData.ICD_CODE;
                                txtIcdMainText.Text = icdMainData.ICD_NAME;
                            }

                        }

                        var icdCauses = missingIcdADOList.Where(o => o.ICD_CAUSE_CHECK).ToList();
                        if (icdCauses != null && icdCauses.Count > 0)
                        {
                            var icdCauseDatas = BackendDataWorker.Get<HIS_ICD>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).Where(o => icdCauses.Select(p => p.ICD_CODE).Contains(o.ICD_CODE)).ToList();

                            if (icdCauseDatas != null && icdCauseDatas.Count > 0)
                            {
                                icdCauseDatas = icdCauseDatas.GroupBy(o => o.ICD_CODE)
                                    .Select(p => p.FirstOrDefault())
                                    .OrderBy(k => k.ICD_CODE).ToList();

                                string icdCausesCodestr = String.Join(";", icdCauseDatas.Select(o => o.ICD_CODE).ToList());
                                string icdCausesstr = String.Join(";", icdCauseDatas.Select(o => o.ICD_NAME).ToList());
                                txtIcdSubCode.Text += !string.IsNullOrEmpty(txtIcdSubCode.Text) ? ";" + icdCausesCodestr : icdCausesCodestr;
                                txtIcdText.Text += !string.IsNullOrEmpty(txtIcdText.Text) ? ";" + icdCausesstr : icdCausesstr;
                            }
                        }
                    }
                    string[] codes = this.txtIcdSubCode.Text.Split(IcdUtil.seperator.ToCharArray());
                    this.icdSubcodeAdoChecks = (from m in this.currentIcds select new ADO.IcdADO(m, codes)).ToList();

                    customGridViewSubIcdName.BeginUpdate();
                    customGridViewSubIcdName.GridControl.DataSource = this.icdSubcodeAdoChecks;
                    customGridViewSubIcdName.EndUpdate();

                    if (HisConfigCFG.IcdServiceHasCheck == "3" || HisConfigCFG.IcdServiceHasCheck == "4")
                    {
                        List<HIS_ICD> icdFromUc = GetIcdCodeListFromUcIcd();
                        MOS.Filter.HisIcdServiceFilter icdServiceFilter = new HisIcdServiceFilter();
                        icdServiceFilter.ICD_CODE__EXACTs = icdFromUc.Select(o => o.ICD_CODE).Distinct().ToList();
                        icdServicePhacDos = new BackendAdapter(null).Get<List<HIS_ICD_SERVICE>>("api/HisIcdService/Get", ApiConsumer.ApiConsumers.MosConsumer, icdServiceFilter, null);
                        if (HisConfigCFG.IcdServiceHasCheck == "4")
                        {
                            if (icdServicePhacDos != null && icdServicePhacDos.Count > 0)
                                ProcessChoiceIcdPhacDo(icdServicePhacDos);
                            else
                            {
                                this.ResetDefaultGridData();
                            }
                        }
                    }

                }
                isNotProcessWhileChangedTextSubIcd = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void getDataFromOtherFormDelegate(object data)
        {
            try
            {
                if (data != null && data is bool)
                {
                    isYes = (bool)data;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private List<SereServADO> GetDiffGender(List<SereServADO> serviceCheckeds__Send, long patientGenderId)
        {
            List<SereServADO> result = new List<SereServADO>();
            try
            {
                foreach (var item in serviceCheckeds__Send)
                {
                    if (item.GENDER_ID != null && patientGenderId != item.GENDER_ID)
                    {
                        result.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private List<HIS_SERE_SERV> getSereServWithMinDuration(List<SereServADO> serviceCheckeds, long patientId, List<HIS_ICD_SERVICE> icdServiceDuration = null)
        {
            List<HIS_SERE_SERV> listSereServResult = null;
            try
            {                     
                    if (HisConfigCFG.IsSereServMinDurationAlert == 1 || HisConfigCFG.IsSereServMinDurationAlert == 2)
                    {
                        if (serviceCheckeds != null && serviceCheckeds.Count > 0)
                        {
                            var bhytServices = serviceCheckeds.Where(o => o.PATIENT_TYPE_ID == HisConfigCFG.PatientTypeId__BHYT).ToList();
                            List<SereServADO> sereServADOExistMinDUration = bhytServices.Where(o => o.MIN_DURATION != null).ToList();
                            if (icdServiceDuration != null && icdServiceDuration.Count > 0)
                                sereServADOExistMinDUration = bhytServices;
                            if (sereServADOExistMinDUration != null && sereServADOExistMinDUration.Count > 0)
                            {
                                List<ServiceDuration> serviceDurations = new List<ServiceDuration>();
                                foreach (var item in sereServADOExistMinDUration)
                                {
                                    ServiceDuration serviceDuration = new ServiceDuration();
                                    serviceDuration.ServiceId = item.SERVICE_ID;
                                    if (icdServiceDuration != null && icdServiceDuration.Count > 0)
                                        serviceDuration.MinDuration = icdServiceDuration.Where(o => o.SERVICE_ID == item.SERVICE_ID).Min(o => o.MIN_DURATION ?? 0);
                                    else
                                        serviceDuration.MinDuration = (item.MIN_DURATION ?? 0);
                                    serviceDurations.Add(serviceDuration);
                                }
                                CommonParam param = new CommonParam();
                                HisSereServMinDurationFilter filter = new HisSereServMinDurationFilter
                                {
                                    ServiceDurations = serviceDurations,
                                    InstructionTime = this.isMultiDateState ? intructionTimeSelecteds.First() : intructionTimeSelecteds.First(),
                                    PatientId = patientId
                                };
                                var result = new BackendAdapter(param).Get<List<HIS_SERE_SERV>>("api/HisSereServ/GetExceedMinDuration", ApiConsumer.ApiConsumers.MosConsumer, filter, param);
                                if (result != null && result.Any())
                                {
                                    listSereServResult = result.GroupBy(o => o.SERVICE_ID).Select(g => g.OrderByDescending(x => x.TDL_INTRUCTION_TIME).First()).ToList();
                                }
                            }
                            else
                            {
                                listSereServResult = null;
                            }
                        }
                        else
                        {
                            listSereServResult = null;
                        }
                    }
                    if (HisConfigCFG.IsSereServMinDurationAlert == 0 || (HisConfigCFG.IsSereServMinDurationAlert != 1 && HisConfigCFG.IsSereServMinDurationAlert != 2))
                    {
                        var allServices = serviceCheckeds.Where(o => o.MIN_DURATION != null).ToList();
                        if (allServices.Count > 0)
                        {
                            List<ServiceDuration> serviceDurations = new List<ServiceDuration>();
                            foreach (var item in allServices)
                            {
                                ServiceDuration serviceDuration = new ServiceDuration();
                                serviceDuration.ServiceId = item.SERVICE_ID;
                                if (icdServiceDuration != null && icdServiceDuration.Count > 0)
                                    serviceDuration.MinDuration = icdServiceDuration.Where(o => o.SERVICE_ID == item.SERVICE_ID).Min(o => o.MIN_DURATION ?? 0);
                                else
                                    serviceDuration.MinDuration = (item.MIN_DURATION ?? 0);
                                serviceDurations.Add(serviceDuration);
                            }
                            CommonParam param = new CommonParam();
                            HisSereServMinDurationFilter filter = new HisSereServMinDurationFilter
                            {
                                ServiceDurations = serviceDurations,
                                InstructionTime = this.isMultiDateState ? intructionTimeSelecteds.First() : intructionTimeSelecteds.First(),
                                PatientId = patientId
                            };
                            var result = new BackendAdapter(param).Get<List<HIS_SERE_SERV>>("api/HisSereServ/GetExceedMinDuration", ApiConsumer.ApiConsumers.MosConsumer, filter, param);
                            if (result != null && result.Any())
                            {
                                listSereServResult = result.GroupBy(o => o.SERVICE_ID).Select(g => g.OrderByDescending(x => x.TDL_INTRUCTION_TIME).First()).ToList();
                            }
                        }
                    }                               
            }
            catch (Exception ex)
            {
                listSereServResult = null;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return listSereServResult;
        }
        
        private bool ServiceAttachForServicePrimary(ref AssignServiceSDO result, long pTypeId)
        {
            bool valid = true;
            string messErr = "";
            List<string> serviceErrs = new List<string>();
            try
            {
                List<ServiceReqDetailSDO> serviceReqDetailSDOTemp = new List<ServiceReqDetailSDO>();
                List<V_HIS_SERVICE_FOLLOW> serviceFollows = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_SERVICE_FOLLOW>().Where(o => new List<long> { IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__MAU, IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC, IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__VT, IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__G, IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__AN }.Exists(p => p != o.FOLLOW_TYPE_ID) && (string.IsNullOrEmpty(o.TREATMENT_TYPE_IDS) || ("," + o.TREATMENT_TYPE_IDS + ",").Contains("," + currentTreatment.TDL_TREATMENT_TYPE_ID + ","))).ToList();
                if (result.ServiceReqDetails != null && result.ServiceReqDetails.Count > 0
                    && serviceFollows != null && serviceFollows.Count > 0)
                {
                    List<long> serviceIds = result.ServiceReqDetails.Select(o => o.ServiceId).Distinct().ToList();
                    long defaultPatientTypeId = pTypeId;
                    List<long> allowPatientTypeIds = this.currentPatientTypeAllows != null ? this.currentPatientTypeAllows
                        .Where(o => o.PATIENT_TYPE_ID == defaultPatientTypeId)
                        .Select(o => o.PATIENT_TYPE_ALLOW_ID).ToList() : null;

                    foreach (ServiceReqDetailSDO sdo in result.ServiceReqDetails)
                    {
                        List<V_HIS_SERVICE_FOLLOW> follows = serviceFollows.Where(o => o.SERVICE_ID == sdo.ServiceId && (o.CONDITIONED_AMOUNT == null || o.CONDITIONED_AMOUNT == sdo.Amount)).ToList();
                        if (follows != null && follows.Count > 0)
                        {
                            foreach (V_HIS_SERVICE_FOLLOW f in follows)
                            {
                                bool hasServicePaty = BranchDataWorker.DicServicePatyInBranch.ContainsKey(f.FOLLOW_ID) ? BranchDataWorker.HasServicePatyWithListPatientType(f.FOLLOW_ID, new List<long>() { defaultPatientTypeId }) : false;
                                long? patientTypeId = null;
                                if (hasServicePaty)
                                {
                                    patientTypeId = defaultPatientTypeId;
                                }
                                else
                                {
                                    V_HIS_SERVICE_PATY otherServicePaty = BranchDataWorker.ServicePatyWithListPatientType(f.FOLLOW_ID, allowPatientTypeIds).FirstOrDefault();
                                    var patientTypeAll = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE>().Where(o => o.IS_ACTIVE == 1).ToList();

                                    patientTypeId = otherServicePaty != null ? (long?)otherServicePaty.PATIENT_TYPE_ID : null;

                                    var patientTypeIdPlus = patientTypeAll.Where(k => k.BASE_PATIENT_TYPE_ID != null && allowPatientTypeIds.Contains(k.BASE_PATIENT_TYPE_ID.Value)).ToList();
                                    if (patientTypeIdPlus != null && patientTypeIdPlus.Count > 0 && (otherServicePaty != null && !String.IsNullOrEmpty(otherServicePaty.INHERIT_PATIENT_TYPE_IDS) && patientTypeIdPlus.Exists(k => k.ID != patientTypeId)))
                                    {
                                        patientTypeId = patientTypeIdPlus.First().ID;
                                    }
                                    Inventec.Common.Logging.LogSystem.Debug("ServiceAttachForServicePrimary____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => otherServicePaty), otherServicePaty)
                                         + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => patientTypeIdPlus), patientTypeIdPlus)
                                        + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => patientTypeId), patientTypeId));
                                    //patientTypeId = otherServicePaty != null ? new Nullable<long>(otherServicePaty.PATIENT_TYPE_ID) : null;
                                }

                                if (patientTypeId.HasValue)
                                {
                                    var serviceIdErrs = serviceIds.Where(o => o == f.FOLLOW_ID).ToList();

                                    if (serviceIdErrs != null && serviceIdErrs.Count > 0)
                                    {
                                        foreach (var sve in serviceIdErrs)
                                        {
                                            var svs = ServiceAllADOs.Where(o => o.ID == sdo.ServiceId).FirstOrDefault();
                                            var svsFL = lstService.Where(o => o.ID == f.FOLLOW_ID).FirstOrDefault();
                                            if (svs != null && svsFL != null && !serviceErrs.Contains(string.Format(ResourceMessage.DichVuADaDuocTietLapDinhKemDichVuB, svs.SERVICE_NAME, svsFL.SERVICE_NAME)))
                                            {
                                                serviceErrs.Add(string.Format(ResourceMessage.DichVuADaDuocTietLapDinhKemDichVuB, svs.SERVICE_NAME, svsFL.SERVICE_NAME));
                                            }
                                        }
                                    }
                                    else
                                    {
                                        ServiceReqDetailSDO attach = new ServiceReqDetailSDO();
                                        attach.ServiceId = f.FOLLOW_ID;
                                        attach.Amount = f.AMOUNT;
                                        attach.PatientTypeId = patientTypeId.Value;
                                        attach.IsExpend = f.IS_EXPEND;
                                        if (HisConfigCFG.IsSetPrimaryPatientType == "2")
                                        {
                                            attach.PrimaryPatientTypeId = sdo.PrimaryPatientTypeId;
                                        }
                                        serviceReqDetailSDOTemp.Add(attach);
                                    }
                                }
                                else
                                {
                                    Inventec.Common.Logging.LogSystem.Debug("Tim thay V_HIS_SERVICE_FOLLOW theo service " + sdo.ServiceId + " nhung khong co doi tuong thanh toan hop le__bo qua khong them vao danh sach dv se chi dinh____Chi tiet____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => f), f));
                                }
                            }
                        }
                    }

                    if (serviceReqDetailSDOTemp != null && serviceReqDetailSDOTemp.Count > 0)
                    {
                        result.ServiceReqDetails.AddRange(serviceReqDetailSDOTemp);
                    }
                }

                if (!VerifyCheckFeeWhileAssign(serviceReqDetailSDOTemp))
                {
                    return false;
                }

                if (serviceErrs != null && serviceErrs.Count > 0)
                {
                    messErr = String.Join("\r\n", serviceErrs);
                    // #25501- Bổ sung cấu hình hệ thống "HIS.Desktop.Plugins.AssignService.IsAllowingChooseServiceWhichInAttachments": "1: Cho phép chỉ định dịch vụ trùng với 1 dịch vụ đi kèm với 1 dịch vụ đã chọn trước đó. 0: Không cho phép".
                    //- Sửa chức năng "Chỉ định DVKT":
                    //+ Khi cấu hình trên được bật, thì khi chỉ định DVKT, nếu chỉ định dịch vụ trùng với 1 dịch vụ đi kèm với dịch vụ đã chọn trước đó, thì hiển thị cảnh báo "Dịch vụ B đã được thiết lập đính kèm dịch vụ A. Bạn có muốn tiếp tục không?". Nếu người dùng chọn "Không" thì dừng xử lý, nếu người dùng chọn "có" thì vẫn thực hiện lưu.
                    //+ Khi cấu hình tắt, thì xử lý như hiện tại: hiển thị cảnh báo và không cho phép lưu.
                    if (HisConfigCFG.IsAllowingChooseServiceWhichInAttachments)
                    {
                        messErr += ". Bạn có muốn tiếp tục không?";
                        if (MessageBox.Show(messErr, Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaCanhBao), MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.No)
                        {
                            valid = false;
                        }
                    }
                    else
                    {
                        valid = false;
                        MessageManager.Show(messErr);
                    }
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => HisConfigCFG.IsAllowingChooseServiceWhichInAttachments), HisConfigCFG.IsAllowingChooseServiceWhichInAttachments) + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => serviceErrs), serviceErrs));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return valid;
        }

        private void ProcessServiceReqSDO(AssignServiceSDO serviceReqSDO, List<SereServADO> dataSereServModel, ref bool isDupicate, long treatmentId, bool IsNeedTrackingCreate)
        {
            try
            {
                if (this.currentHisTreatment != null)
                    serviceReqSDO.TreatmentId = treatmentId;

                if (this.chkPriority.Checked)
                    serviceReqSDO.Priority = GlobalVariables.HAS_PRIORITY;
                else
                    serviceReqSDO.Priority = null;

                if (this.chkIsNotRequireFee.Checked)
                    serviceReqSDO.IsNotRequireFee = 1;

                if (this.serviceReqParentId != 0)
                    serviceReqSDO.ParentServiceReqId = this.serviceReqParentId;

                if (this.txtDescription.Text != "")
                    serviceReqSDO.Description = this.txtDescription.Text.Trim();
                serviceReqSDO.ProvisionalDiagnosis = txtProvisionalDiagnosis.Text;
                ACS_USER acsUser = null;
                string loginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                if (cboUser.EditValue != null)
                {
                    acsUser = BackendDataWorker.Get<ACS_USER>().FirstOrDefault(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE && o.LOGINNAME.Equals(cboUser.EditValue.ToString()));
                }

                if (acsUser == null)
                {
                    acsUser = BackendDataWorker.Get<ACS_USER>().FirstOrDefault(o => o.LOGINNAME.Equals(loginName));
                }

                if (acsUser != null)
                {
                    serviceReqSDO.RequestLoginName = acsUser.LOGINNAME;
                    serviceReqSDO.RequestUserName = acsUser.USERNAME;
                    txtLoginName.Text = acsUser.LOGINNAME;
                    cboUser.EditValue = acsUser.LOGINNAME;
                }

                if (cboConsultantUser.EditValue != null)
                {
                    var conUser = BackendDataWorker.Get<ACS_USER>().FirstOrDefault(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE && o.LOGINNAME.Equals(cboConsultantUser.EditValue.ToString()));

                    if (conUser != null)
                    {
                        serviceReqSDO.ConsultantLoginName = conUser.LOGINNAME;
                        serviceReqSDO.ConsultantUserName = conUser.USERNAME;
                    }
                }


                if (this.cboExecuteGroup.EditValue != null)
                    serviceReqSDO.ExecuteGroupId = Inventec.Common.TypeConvert.Parse.ToInt64(this.cboExecuteGroup.EditValue.ToString());

                if (IsNeedTrackingCreate)
                {
                    // điều kiện nhiều tờ điều trị
                    GridCheckMarksSelection gridCheckMarkBusiness = cboTracking.Properties.Tag as GridCheckMarksSelection;
                    var lstCheck = intructionTimeSelecteds.Select(o => o.ToString().Substring(0, 8)).ToList();
                    if (gridCheckMarkBusiness != null && gridCheckMarkBusiness.SelectedCount > 0)
                    {
                        List<string> lstTrackingTimeDupicate = new List<string>();
                        serviceReqSDO.TrackingInfos = new List<TrackingInfoSDO>();
                        string mgsKhongNamTrongNgayChiDinh = "";
                        string mgsTrungNgay = "";
                        foreach (TrackingAdo rv in gridCheckMarkBusiness.Selection)
                        {
                            if (rv != null && !lstCheck.Exists(o => o == rv.TRACKING_TIME.ToString().Substring(0, 8)))
                            {
                                mgsKhongNamTrongNgayChiDinh += rv.TrackingTimeStr.Substring(0, 10) + ",";
                            }
                            else if (rv != null && lstTrackingTimeDupicate.Exists(o => o == rv.TRACKING_TIME.ToString().Substring(0, 8)))
                            {
                                mgsTrungNgay += rv.TrackingTimeStr.Substring(0, 10) + ",";
                            }
                            else
                            {
                                lstTrackingTimeDupicate.Add(rv.TRACKING_TIME.ToString().Substring(0, 8));
                                TrackingInfoSDO sdo = new TrackingInfoSDO();
                                sdo.TrackingId = rv.ID;
                                sdo.IntructionTime = Convert.ToInt64(intructionTimeSelecteds.Where(o => o.ToString().Substring(0, 8) == rv.TRACKING_TIME.ToString().Substring(0, 8)).FirstOrDefault());
                                serviceReqSDO.TrackingInfos.Add(sdo);
                            }
                        }
                        if (!string.IsNullOrEmpty(mgsKhongNamTrongNgayChiDinh))
                        {
                            MessageBox.Show(string.Format("Tờ điều trị ngày {0} không nằm trong ngày chỉ định", mgsKhongNamTrongNgayChiDinh), "Thông báo");
                            isDupicate = true;
                            return;
                        }
                        if (!string.IsNullOrEmpty(mgsTrungNgay))
                        {
                            MessageBox.Show(string.Format("Ngày {0} có nhiều hơn 1 tờ điều trị", mgsTrungNgay), "Thông báo");
                            isDupicate = true;
                            return;
                        }

                    }// nếu chỉ có 1 tờ điều trị
                    else if (this.cboTracking.EditValue != null && !string.IsNullOrEmpty(this.cboTracking.EditValue.ToString()))
                    {
                        serviceReqSDO.TrackingId = Inventec.Common.TypeConvert.Parse.ToInt64(this.cboTracking.EditValue.ToString());
                    }
                    else
                    {
                        serviceReqSDO.TrackingId = null;
                    }
                }

                serviceReqSDO.IsNotRequireFee = (chkIsNotRequireFee.CheckState == CheckState.Checked) ? (short?)1 : null;
                serviceReqSDO.IsInformResultBySms = (chkIsInformResultBySms.CheckState == CheckState.Checked);
                serviceReqSDO.IsEmergency = (chkIsEmergency.CheckState == CheckState.Checked);

                if (dataSereServModel != null && dataSereServModel.Count > 0)
                {
                    foreach (var item in dataSereServModel)
                    {
                        ServiceReqDetailSDO sdo = new ServiceReqDetailSDO();
                        sdo.EkipInfos = new List<EkipSDO>();
                        sdo.Amount = item.AMOUNT;
                        sdo.PatientTypeId = item.PATIENT_TYPE_ID;
                        sdo.RoomId = item.TDL_EXECUTE_ROOM_ID;
                        sdo.ServiceId = item.SERVICE_ID;
                        sdo.ParentId = null;
                        sdo.InstructionNote = item.InstructionNote;
                        sdo.IsExpend = (item.IsExpend == true ? 1 : (short?)null);
                        sdo.IsOutParentFee = (item.IsOutKtcFee == true ? 1 : (short?)null);
                        sdo.ShareCount = item.ShareCount;
                        sdo.UserPrice = item.AssignSurgPriceEdit;
                        sdo.UserPackagePrice = item.AssignPackagePriceEdit;
                        sdo.PackageId = item.PackagePriceId;
                        if (item.OTHER_PAY_SOURCE_ID.HasValue)
                            sdo.OtherPaySourceId = item.OTHER_PAY_SOURCE_ID;
                        if (HisConfigCFG.IsSetPrimaryPatientType == commonString__true
                            || HisConfigCFG.IsSetPrimaryPatientType == "2")
                        {
                            sdo.PrimaryPatientTypeId = item.PRIMARY_PATIENT_TYPE_ID;
                        }
                        if (item.IsNoDifference.HasValue)
                            sdo.IsNoHeinDifference = item.IsNoDifference.Value;
                        if (item.SERVICE_CONDITION_ID.HasValue)
                            sdo.ServiceConditionId = item.SERVICE_CONDITION_ID.Value;
                        if (item.SereServEkipADO != null && item.SereServEkipADO.listEkipUser != null && item.SereServEkipADO.listEkipUser.Count() > 0)
                        {
                            foreach (var ekip in item.SereServEkipADO.listEkipUser)
                            {
                                EkipSDO ekipSdo = new EkipSDO();
                                ekipSdo.ExecuteRoleId = ekip.EXECUTE_ROLE_ID;
                                ekipSdo.LoginName = ekip.LOGINNAME;
                                ekipSdo.UserName = ekip.USERNAME;
                                sdo.EkipInfos.Add(ekipSdo);
                            }
                        }
                        sdo.BedId = item.BedId;
                        sdo.BedFinishTime = item.BedFinishTime;
                        sdo.BedStartTime = item.BedStartTime;
                        sdo.IsNotUseBhyt = item.IsNotUseBhyt;
                        sdo.AssignNumOrder = item.AssignNumOrder;
                        if (item.TEST_SAMPLE_TYPE_ID > 0)
                            sdo.SampleTypeCode = item.TEST_SAMPLE_TYPE_CODE;
                        serviceReqSDO.ServiceReqDetails.Add(sdo);
                    }
                }

                serviceReqSDO.RequestRoomId = GetRoomId();
                serviceReqSDO.ManualRequestRoomId = GetManualRequestRoom();

                if (serviceReqSDO.RequestRoomId == 0)
                    Inventec.Common.Logging.LogSystem.Warn("Khong xac dinh du lieu phong lam viec trong module, chuc nang goi module nay khong truyen vao phong lam viec. " + LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => currentModule), currentModule));

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ProcessServiceReqSDOForIcd(AssignServiceSDO serviceReqSDO)
        {
            try
            {
                var icdValue = UcIcdGetValue() as HIS.UC.Icd.ADO.IcdInputADO;
                if (icdValue != null)
                {
                    serviceReqSDO.IcdCode = icdValue.ICD_CODE;
                    if (!string.IsNullOrEmpty(icdValue.ICD_CODE))
                    {
                        serviceReqSDO.IcdCode = icdValue.ICD_CODE;
                    }
                    serviceReqSDO.IcdName = icdValue.ICD_NAME;
                }

                var icdValueCause = UcIcdCauseGetValue() as HIS.UC.Icd.ADO.IcdInputADO;
                if (icdValueCause != null)
                {
                    serviceReqSDO.IcdCauseCode = icdValueCause.ICD_CODE;
                    if (!string.IsNullOrEmpty(icdValueCause.ICD_CODE))
                    {
                        serviceReqSDO.IcdCauseCode = icdValueCause.ICD_CODE;
                    }
                    serviceReqSDO.IcdCauseName = icdValueCause.ICD_NAME;
                }

                var subIcd = UcSecondaryIcdGetValue() as HIS.UC.SecondaryIcd.ADO.SecondaryIcdDataADO;
                if (subIcd != null)
                {
                    serviceReqSDO.IcdSubCode = subIcd.ICD_SUB_CODE;
                    serviceReqSDO.IcdText = subIcd.ICD_TEXT;
                }


                var icdTranditional = this.icdYhctProcessor.GetValue(this.ucIcdYhct);
                if (icdTranditional != null && icdTranditional is IcdInputADO)
                {
                    serviceReqSDO.TraditionalIcdCode = ((IcdInputADO)icdTranditional).ICD_CODE;
                    serviceReqSDO.TraditionalIcdName = ((IcdInputADO)icdTranditional).ICD_NAME;
                }
                var subIcdTranditional = subIcdYhctProcessor.GetValue(ucSecondaryIcdYhct);
                if (subIcdTranditional != null && subIcdTranditional is SecondaryIcdDataADO)
                {
                    serviceReqSDO.TraditionalIcdSubCode = ((SecondaryIcdDataADO)subIcdTranditional).ICD_SUB_CODE;
                    serviceReqSDO.TraditionalIcdText = ((SecondaryIcdDataADO)subIcdTranditional).ICD_TEXT;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void UpdateIcdToTreatment(HisTreatmentWithPatientTypeInfoSDO hisTreatmentWithPatientTypeInfoSDO)
        {
            try
            {

                var icdValue = UcIcdGetValue() as HIS.UC.Icd.ADO.IcdInputADO;
                if (icdValue != null)
                {
                    hisTreatmentWithPatientTypeInfoSDO.ICD_CODE = icdValue.ICD_CODE;
                    if (!string.IsNullOrEmpty(icdValue.ICD_CODE))
                    {
                        hisTreatmentWithPatientTypeInfoSDO.ICD_CODE = icdValue.ICD_CODE;
                    }
                    hisTreatmentWithPatientTypeInfoSDO.ICD_NAME = icdValue.ICD_NAME;
                }

                var icdValueCause = UcIcdCauseGetValue() as HIS.UC.Icd.ADO.IcdInputADO;
                if (icdValueCause != null)
                {
                    hisTreatmentWithPatientTypeInfoSDO.ICD_CAUSE_CODE = icdValueCause.ICD_CODE;
                    if (!string.IsNullOrEmpty(icdValueCause.ICD_CODE))
                    {
                        hisTreatmentWithPatientTypeInfoSDO.ICD_CAUSE_CODE = icdValueCause.ICD_CODE;
                    }
                    hisTreatmentWithPatientTypeInfoSDO.ICD_CAUSE_NAME = icdValueCause.ICD_NAME;
                }

                var subIcd = UcSecondaryIcdGetValue() as HIS.UC.SecondaryIcd.ADO.SecondaryIcdDataADO;
                if (subIcd != null)
                {
                    hisTreatmentWithPatientTypeInfoSDO.ICD_SUB_CODE = subIcd.ICD_SUB_CODE;
                    hisTreatmentWithPatientTypeInfoSDO.ICD_TEXT = subIcd.ICD_TEXT;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SaveServiceReqCombo(AssignServiceSDO serviceReqSDO, bool issaveandprint, bool printTH, bool isSaveAndShow, bool isSign = false, bool isPrintPreview = false, bool IsPatientSelect = false, string patientName = null, string treatmentCode = null)
        {
            CommonParam param = new CommonParam();
            bool success = false;
            try
            {
                WaitingManager.Show();
                serviceReqSDO.InstructionTime = intructionTimeSelecteds.First();
                serviceReqSDO.InstructionTimes = intructionTimeSelecteds;//TODO

                serviceReqSDO.UseTimes = this.USE_TIME;
                //Trường hợp chỉ định từ màn hình xử lý pttt, cập nhật dữ liệu cùng kíp, khác kíp tương ứng
                long sereservid = this.GetSereServInKip();
                if (sereservid > 0)
                {
                    foreach (var item in serviceReqSDO.ServiceReqDetails)
                    {
                        item.ParentId = sereservid;
                        item.EkipId = (this.currentSereServInEkip != null ? this.currentSereServInEkip.EKIP_ID : null);
                    }
                }

                if (this.serviceReqComboResultSDO != null && dicSessionCode != null && dicSessionCode.Count > 0 && dicSessionCode.ContainsKey(serviceReqSDO.TreatmentId) && !String.IsNullOrEmpty(dicSessionCode[serviceReqSDO.TreatmentId]) && this.actionType == GlobalVariables.ActionEdit)
                {
                    serviceReqSDO.SessionCode = dicSessionCode[serviceReqSDO.TreatmentId];
                    Inventec.Common.Logging.LogSystem.Debug("Sua chi dinh SessionCode =" + serviceReqComboResultSDO.SessionCode);
                    if (HisConfigCFG.AutoDeleteEmrDocumentWhenEditReq == "1" && dSignedList != null && dSignedList.Count > 0 && dSignedList.ContainsKey(serviceReqSDO.TreatmentId) && dSignedList[serviceReqSDO.TreatmentId] != null && dSignedList[serviceReqSDO.TreatmentId].Count > 0)
                    {

                        WaitingManager.Hide();
                        if (DevExpress.XtraEditors.XtraMessageBox.Show("Y lệnh đã tồn tại văn bản ký, hệ thống sẽ tự động xóa văn bản ký hiện tại. Bạn có muốn tiếp tục?", HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaCanhBao), MessageBoxButtons.YesNo) != DialogResult.Yes)
                            return;
                        List<DocumentSignedUpdateIGSysResultDTO> lst = new List<DocumentSignedUpdateIGSysResultDTO>();
                        foreach (var item in dSignedList[serviceReqSDO.TreatmentId])
                        {
                            CommonParam paramEmr = new CommonParam();
                            bool apiResult = new BackendAdapter(paramEmr).Post<bool>("api/EmrDocument/DeleteByCode", ApiConsumers.EmrConsumer, item.DocumentCode, paramEmr);
                            if (apiResult)
                            {
                                lst.Add(item);
                            }
                            else
                            {
                                #region Hien thi message thong bao
                                MessageManager.Show(this,paramEmr, apiResult);
                                #endregion
                            }
                        }
                        foreach (var item in lst)
                        {
                            dSignedList[serviceReqSDO.TreatmentId].Remove(item);
                        }
                    }
                }

                Inventec.Common.Logging.LogSystem.Debug("Luu chi dinh____Du lieu dau vao____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => serviceReqSDO), serviceReqSDO));
                //Gọi api chỉ định dv
                var rs = new BackendAdapter(param).Post<HisServiceReqListResultSDO>(RequestUriStore.HIS_SERVICE_REQ__ASSIGN_SERVICE, ApiConsumers.MosConsumer, serviceReqSDO, ProcessLostToken, param);
                Inventec.Common.Logging.LogSystem.Info("this.serviceReqComboResultSDO: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => rs), rs));

                if (rs != null)
                {
                    this.serviceReqComboResultSDO = rs;
                    dicSessionCode[serviceReqComboResultSDO.ServiceReqs[0].TREATMENT_ID] = serviceReqComboResultSDO.SessionCode;
                    dicServiceReqList[serviceReqComboResultSDO.ServiceReqs[0].TREATMENT_ID] = serviceReqComboResultSDO;
                    Inventec.Common.Logging.LogSystem.Debug("Chi dinh dich vu. Du lieu chi tiet dich vụ gui len api: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => serviceReqSDO.ServiceReqDetails), serviceReqSDO.ServiceReqDetails));

                    // distint để tránh lặp #27825
                    if (this.serviceReqComboResultSDO.ServiceReqs != null && this.serviceReqComboResultSDO.ServiceReqs.Count > 0)
                    {
                        this.serviceReqComboResultSDO.ServiceReqs = this.serviceReqComboResultSDO.ServiceReqs.GroupBy(x => x.ID).Select(x => x.FirstOrDefault()).ToList();
                    }

                    if (this.serviceReqComboResultSDO.SereServs != null && this.serviceReqComboResultSDO.SereServs.Count > 0)
                    {
                        this.serviceReqComboResultSDO.SereServs = this.serviceReqComboResultSDO.SereServs.GroupBy(x => x.ID).Select(x => x.FirstOrDefault()).ToList();
                    }

                    if (this.serviceReqComboResultSDO.SereServExts != null && this.serviceReqComboResultSDO.SereServExts.Count > 0)
                    {
                        this.serviceReqComboResultSDO.SereServExts = this.serviceReqComboResultSDO.SereServExts.GroupBy(x => x.ID).Select(x => x.FirstOrDefault()).ToList();
                    }

                    if (this.serviceReqComboResultSDO.SereServRations != null && this.serviceReqComboResultSDO.SereServRations.Count > 0)
                    {
                        this.serviceReqComboResultSDO.SereServRations = this.serviceReqComboResultSDO.SereServRations.GroupBy(x => x.ID).Select(x => x.FirstOrDefault()).ToList();
                    }

                    //Gọi delegate xử lý ở module thực hiện gọi module chỉ định sau khi chỉ định thành công
                    //Truyền vào đầu vào là kết quả api trả về
                    if (this.processDataResult != null)
                        this.processDataResult(this.serviceReqComboResultSDO);

                    //Gọi delegate xử lý cập nhật bệnh phụ tại module thực hiện gọi module chỉ định, truyền vào các giá trị "bệnh chính", "bệnh phụ",... đã nhập trên form chỉ định
                    if (this.processRefeshIcd != null)
                        this.processRefeshIcd(this.serviceReqComboResultSDO.ServiceReqs[0].ICD_CODE, this.serviceReqComboResultSDO.ServiceReqs[0].ICD_NAME, this.serviceReqComboResultSDO.ServiceReqs[0].ICD_SUB_CODE, this.serviceReqComboResultSDO.ServiceReqs[0].ICD_TEXT);

                    success = true;
                    this.toggleSwitchDataChecked.EditValue = true;

                    this.actionType = GlobalVariables.ActionEdit;
                    this.SetEnableButtonControl(this.actionType);
                    this.isSaveAndPrint = issaveandprint;
                    //this.RefeshSereServInTreatmentData();

                    //Nếu mở từ tiếp đón chưa có icd và có nhập icd thì cập nhật Icd để in ra
                    //Comment do code gây lỗi, không biết lý do code sử dụng hàm này
                    //this.UpdateIcdToCurrentHisTreatment();

                    MPS.ProcessorBase.PrintConfig.PreviewType? previewType = null;
                    if (isSign)
                    {
                        if (isSaveAndPrint)
                        {
                            previewType = MPS.ProcessorBase.PrintConfig.PreviewType.EmrSignAndPrintNow;
                        }
                        else if (isPrintPreview)
                        {
                            previewType = MPS.ProcessorBase.PrintConfig.PreviewType.EmrSignAndPrintPreview;
                            isSaveAndShow = true;
                        }
                        else
                            previewType = MPS.ProcessorBase.PrintConfig.PreviewType.EmrSignNow;
                    }
                    else
                    {
                        if (isSaveAndPrint)
                        {
                            previewType = MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow;
                        }
                        else if (isPrintPreview)
                        {
                            previewType = MPS.ProcessorBase.PrintConfig.PreviewType.Show;
                            isSaveAndShow = true;
                        }
                    }
                    //InTamUng(isSaveAndShow, previewType);
                    Inventec.Common.Logging.LogSystem.Debug("SaveServiceReqCombo____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => previewType), previewType)
                        + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => isSaveAndPrint), isSaveAndPrint)
                        + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => isSign), isSign)
                        + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => isPrintPreview), isPrintPreview)
                        + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => lstLoaiPhieu), lstLoaiPhieu)
                        + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => printTH), printTH));
                    //Nếu click nút lưu in => tự động gọi hàm xử lý in ngay
                    if (this.isSaveAndPrint || isPrintPreview || isSign)
                    {
                        if (workingAssignServiceADO.OpenFromBedRoomPartial && this.patientSelectProcessor != null && this.ucPatientSelect != null && this.patientSelectProcessor.GetSelectedRows(this.ucPatientSelect).Count > 1 && serviceReqComboResultSDO.ServiceReqs != null && serviceReqComboResultSDO.ServiceReqs.Count > 0)
                        {
                            LoadDataToCurrentTreatmentData(serviceReqComboResultSDO.ServiceReqs[0].TREATMENT_ID, serviceReqComboResultSDO.ServiceReqs[0].INTRUCTION_TIME);
                        }

                        UpdateIcdToTreatment(this.currentHisTreatment);

                        if (this.lstLoaiPhieu != null && this.lstLoaiPhieu.Count > 0)
                        {
                            var checkHDBN = this.lstLoaiPhieu.FirstOrDefault(o => o.Check == true && o.ID == "gridView7_2");

                            var checkYCDV = this.lstLoaiPhieu.FirstOrDefault(o => o.Check == true && o.ID == "gridView7_1");

                            var checkQR = this.lstLoaiPhieu.FirstOrDefault(o => o.Check == true && o.ID == "gridView7_3");

                            if (checkHDBN != null)
                            {
                                if (!isPrinted) InTamUng(isSaveAndShow, previewType);
                                InPhieuHuoangDanBenhNhan(isSaveAndShow);

                            }

                            if (checkYCDV != null)
                            {
                                if (!isPrinted) InTamUng(isSaveAndShow, previewType);
                                InPhieuYeuCauDichVu(isSaveAndShow, previewType);

                            }

                            if (checkQR != null)
                            {
                                if (!isPrinted) InTamUng(isSaveAndShow, previewType);
                                InYeuCauThanhToanQR(isSaveAndPrint, isSign, isPrintPreview);

                            }
                        }

                        //foreach (var item in this.lstLoaiPhieu)
                        //{
                        //    if (item.Check)
                        //    {
                        //        if (item.ID == "gridView7_1")
                        //        {
                        //            InPhieuYeuCauDichVu(isSaveAndShow, previewType);
                        //        }
                        //        if (item.ID == "gridView7_2")
                        //        {
                        //            InPhieuHuoangDanBenhNhan(isSaveAndShow);
                        //        }
                        //    }
                        //}

                        if (printTH)
                        {
                            DelegateRunPrinter(PrintTypeCodeStore.PRINT_TYPE_CODE__IN__PHIEU_YEU_CAU_CHI_DINH_TONG_HOP__MPS000037, isSaveAndShow, previewType);
                        }
                    }

                }
                else
                {
                    ListMessError.Add("Bệnh nhân " + patientName + " (" + treatmentCode + ") :" + param.GetMessage());

                }
                WaitingManager.Hide();

                if (!IsPatientSelect)
                {
                    #region Show message
                    MessageManager.Show(this, param, success);
                    #endregion

                    #region Process has exception
                    SessionManager.ProcessTokenLost(param);
                    #endregion

                    WaitingManager.Hide();
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Fatal(ex);
            }
        }

        private void ProcessOpenVoBenhAn(List<V_HIS_SERE_SERV> sereServs)
        {
            try
            {
                var emrFormsCodes = lstService.Where(o => sereServs.Exists(p => p.SERVICE_ID == o.ID) && !string.IsNullOrEmpty(o.EMR_FORM_CODES)).Select(o => o.EMR_FORM_CODES).ToList();
                if (emrFormsCodes != null && emrFormsCodes.Count > 0 && serviceReqComboResultSDO != null)
                {
                    HIS.Desktop.Plugins.Library.FormMedicalRecord.Base.EmrInputADO emrInputAdo = new Library.FormMedicalRecord.Base.EmrInputADO();
                    emrInputAdo.TreatmentId = serviceReqComboResultSDO.ServiceReqs.FirstOrDefault().TREATMENT_ID;
                    emrInputAdo.PatientId = serviceReqComboResultSDO.ServiceReqs.FirstOrDefault().TDL_PATIENT_ID;
                    emrInputAdo.roomId = this.currentModule.RoomId;
                    if (currentTreatment.EMR_COVER_TYPE_ID != null)
                    {
                        emrInputAdo.EmrCoverTypeId = currentTreatment.EMR_COVER_TYPE_ID;
                    }
                    else
                    {
                        var data = BackendDataWorker.Get<HIS_EMR_COVER_CONFIG>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE
                            && o.ROOM_ID == this.currentModule.RoomId
                        && o.TREATMENT_TYPE_ID == currentTreatment.TDL_TREATMENT_TYPE_ID
                        ).ToList();
                        if (data != null && data.Count > 0)
                        {
                            if (data.Count == 1)
                            {
                                emrInputAdo.EmrCoverTypeId = data.FirstOrDefault().EMR_COVER_TYPE_ID;

                            }
                            else
                            {
                                emrInputAdo.lstEmrCoverTypeId = new List<long>();
                                emrInputAdo.lstEmrCoverTypeId = data.Select(o => o.EMR_COVER_TYPE_ID).ToList();
                            }
                        }
                        else
                        {
                            var DepartmentID = HIS.Desktop.LocalStorage.LocalData.WorkPlace.WorkPlaceSDO.FirstOrDefault(o => o.RoomId == this.currentModule.RoomId).DepartmentId;

                            var DataConfig = BackendDataWorker.Get<HIS_EMR_COVER_CONFIG>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE
                        && o.DEPARTMENT_ID == DepartmentID && o.TREATMENT_TYPE_ID == currentTreatment.TDL_TREATMENT_TYPE_ID).ToList();

                            if (DataConfig != null && DataConfig.Count > 0)
                            {
                                if (DataConfig.Count == 1)
                                {
                                    emrInputAdo.EmrCoverTypeId = DataConfig.FirstOrDefault().EMR_COVER_TYPE_ID;
                                }
                                else
                                {
                                    emrInputAdo.lstEmrCoverTypeId = new List<long>();
                                    emrInputAdo.lstEmrCoverTypeId = DataConfig.Select(o => o.EMR_COVER_TYPE_ID).ToList();
                                }
                            }
                        }
                    }

                    HIS.Desktop.Plugins.Library.FormMedicalRecord.MediRecordMenuPopupProcessor processor = new Library.FormMedicalRecord.MediRecordMenuPopupProcessor();

                    long EmrCoverTypeId_ = emrInputAdo.EmrCoverTypeId ?? 0;
                    long EmrCoverTypeId_Send;

                    if (EmrCoverTypeId_ <= 0)
                    {
                        EmrCoverTypeId_Send = 0;
                    }
                    else
                    {
                        EmrCoverTypeId_Send = EmrCoverTypeId_;
                    }

                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => emrInputAdo), emrInputAdo));

                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => emrFormsCodes), emrFormsCodes));
                    processor.FormOpenEmr(EmrCoverTypeId_Send, emrInputAdo,string.Join(",", emrFormsCodes));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void CallTransReqCreateByService()
        {
            try
            {
                if (serviceReqComboResultSDO.ServiceReqs != null && serviceReqComboResultSDO.SereServs != null && serviceReqComboResultSDO.SereServs.Count > 0)
                {

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void RefeshSereServInTreatmentData()
        {
            try
            {
                DateTime intructionTime = (Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(this.intructionTimeSelecteds.First()) ?? DateTime.Now);

                List<long> setyAllowsIds = new List<long>();
                setyAllowsIds.Add(IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__MAU);
                setyAllowsIds.Add(IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC);
                setyAllowsIds.Add(IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__VT);
                long? INTRUCTION_TIME_FROM = null, INTRUCTION_TIME_TO = null;
                var existServiceByType = BackendDataWorker.Get<V_HIS_SERVICE_PATY>().Where(o => (o.INSTR_NUM_BY_TYPE_FROM.HasValue && o.INSTR_NUM_BY_TYPE_FROM.Value > 0) || (o.INSTR_NUM_BY_TYPE_TO.HasValue && o.INSTR_NUM_BY_TYPE_TO.Value > 0)).ToList();
                if (existServiceByType == null || existServiceByType.Count() == 0)
                {
                    if (intructionTime != null && intructionTime != DateTime.MinValue)
                    {
                        INTRUCTION_TIME_FROM = Inventec.Common.TypeConvert.Parse.ToInt64(intructionTime.ToString("yyyyMMdd") + "000000");
                        INTRUCTION_TIME_TO = Inventec.Common.TypeConvert.Parse.ToInt64(intructionTime.ToString("yyyyMMdd") + "235959");
                    }
                    else
                    {
                        INTRUCTION_TIME_FROM = Inventec.Common.DateTime.Get.StartDay();
                        INTRUCTION_TIME_TO = Inventec.Common.DateTime.Get.EndDay();
                    }
                }

                if (this.sereServsInTreatmentRaw == null || this.sereServsInTreatmentRaw.Count == 0)
                {
                    CommonParam param = new CommonParam();
                    HisSereServView1Filter hisSereServFilter = new HisSereServView1Filter();
                    hisSereServFilter.TREATMENT_ID = treatmentId;
                    hisSereServFilter.INTRUCTION_TIME_FROM = INTRUCTION_TIME_FROM;
                    hisSereServFilter.INTRUCTION_TIME_TO = INTRUCTION_TIME_TO;
                    hisSereServFilter.NOT_IN_SERVICE_TYPE_IDs = setyAllowsIds;
                    this.sereServWithTreatment = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.HIS_SERE_SERV>>(RequestUriStore.HIS_SERE_SERV_GETVIEW_1, ApiConsumers.MosConsumer, hisSereServFilter, ProcessLostToken, param);
                }
                else
                {
                    //Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => INTRUCTION_TIME_FROM), INTRUCTION_TIME_FROM)
                    //    + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => INTRUCTION_TIME_TO), INTRUCTION_TIME_TO)
                    //    + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => sereServWithTreatment), sereServWithTreatment)
                    //    + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => sereServsInTreatmentRaw), sereServsInTreatmentRaw));
                    this.sereServWithTreatment = this.sereServsInTreatmentRaw.Where(o =>
                        o.TDL_TREATMENT_ID == treatmentId
                        && (INTRUCTION_TIME_FROM == null || (INTRUCTION_TIME_FROM.HasValue && o.TDL_INTRUCTION_TIME >= INTRUCTION_TIME_FROM.Value))
                        && (INTRUCTION_TIME_TO == null || (INTRUCTION_TIME_TO.HasValue && o.TDL_INTRUCTION_TIME <= INTRUCTION_TIME_TO.Value))
                        && !setyAllowsIds.Contains(o.TDL_SERVICE_TYPE_ID)).ToList();
                }

                //Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => INTRUCTION_TIME_FROM), INTRUCTION_TIME_FROM)
                //   + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => INTRUCTION_TIME_TO), INTRUCTION_TIME_TO)
                //   + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => sereServWithTreatment), sereServWithTreatment)
                //   + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => sereServsInTreatmentRaw), sereServsInTreatmentRaw));
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Fatal(ex);
            }
        }

        private void UpdateIcdToCurrentHisTreatment()
        {
            try
            {
                if (String.IsNullOrEmpty(currentHisTreatment.ICD_NAME) && (string.IsNullOrEmpty(currentHisTreatment.ICD_CODE)))
                {
                    LoadIcdToControl(currentHisTreatment.PREVIOUS_ICD_CODE, this.currentHisTreatment.PREVIOUS_ICD_NAME);
                    var icdCaus = this.currentIcds.FirstOrDefault(o => o.ICD_CODE == currentHisTreatment.PREVIOUS_ICD_CODE);
                    if (icdCaus != null)
                    {
                        LoadRequiredCause((icdCaus.IS_REQUIRE_CAUSE == 1));
                    }

                    LoadIcdToControlIcdSub(this.currentHisTreatment.PREVIOUS_ICD_SUB_CODE, this.currentHisTreatment.PREVIOUS_ICD_NAME);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void RefeshServiceDatasourceAfterSave(List<SereServADO> serviceCheckeds__Send)
        {
            try
            {
                foreach (var sv in serviceCheckeds__Send)
                {
                    var sv1 = this.ServiceIsleafADOs.Where(o => o.SERVICE_ID == sv.SERVICE_ID).FirstOrDefault();
                    sv1.PATIENT_TYPE_ID = sv.PATIENT_TYPE_ID;
                }

                gridControlServiceProcess.RefreshDataSource();

                //gridViewServiceProcess.BeginUpdate();
                //gridViewServiceProcess.GridControl.DataSource = serviceCheckeds__Send;
                //gridViewServiceProcess.EndUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}

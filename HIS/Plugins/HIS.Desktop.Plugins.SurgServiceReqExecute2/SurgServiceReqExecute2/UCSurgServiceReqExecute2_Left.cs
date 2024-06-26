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
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraNavBar;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using Inventec.UC.Paging;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Common;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Utilities;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Windows.Forms;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using Inventec.Desktop.Common.Controls.ValidationRule;
using DevExpress.XtraEditors.DXErrorProvider;
using System.Resources;
using Inventec.Desktop.Common.LanguageManager;
using System.Security.Cryptography;
using HIS.Desktop.Plugins.SurgServiceReqExecute2.ADO;
using HIS.Desktop.Plugins.SurgServiceReqExecute2.EkipTemp;
using HIS.Desktop.ADO;
using ACS.EFMODEL.DataModels;
using HIS.Desktop.Plugins.SurgServiceReqExecute2.Config;
using MOS.SDO;
using Inventec.Common.RichEditor.Base;
using Inventec.Common.ThreadCustom;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using HIS.Desktop.Utility;
using DevExpress.XtraGrid.Views.Grid;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.SurgServiceReqExecute2
{
    public partial class UCSurgServiceReqExecute2 : UserControlBase
    {

        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                SetDefaultRight();
                FillDataToGrid();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }
        private void toolTipControllerGrid_GetActiveObjectInfo(object sender, ToolTipControllerGetActiveObjectInfoEventArgs e)
        {
            try
            {
                if (e.Info == null && e.SelectedControl == gridControl1)
                {
                    DevExpress.XtraGrid.Views.Grid.GridView view = gridControl1.FocusedView as DevExpress.XtraGrid.Views.Grid.GridView;
                    GridHitInfo info = view.CalcHitInfo(e.ControlMousePosition);
                    if (info.InRowCell)
                    {
                        if (lastRowHandle != info.RowHandle || lastColumn != info.Column)
                        {
                            lastColumn = info.Column;
                            lastRowHandle = info.RowHandle;
                            string text = "";
                            if (info.Column.FieldName == "TRANGTHAI_IMG")
                            {
                                long sttId = (long)view.GetRowCellValue(lastRowHandle, "SERVICE_REQ_STT_ID");
                                if (sttId == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__CXL)
                                {
                                    text = "Chưa xử lý";
                                }
                                else if (sttId == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__DXL)
                                {
                                    text = "Đang xử lý";
                                }
                                else if (sttId == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__HT)
                                {
                                    text = "Hoàn thành";
                                }
                            }

                            lastInfo = new ToolTipControlInfo(new DevExpress.XtraGrid.GridToolTipInfo(view, new DevExpress.XtraGrid.Views.Base.CellToolTipInfo(info.RowHandle, info.Column, "Text")), text);
                        }
                        e.Info = lastInfo;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void dxValidationProvider_ValidationFailed(object sender, ValidationFailedEventArgs e)
        {
            try
            {
                BaseEdit edit = e.InvalidControl as BaseEdit;
                if (edit == null)
                    return;

                BaseEditViewInfo viewInfo = edit.GetViewInfo() as BaseEditViewInfo;
                if (viewInfo == null)
                    return;

                if (positionHandle == -1)
                {
                    positionHandle = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.SelectAll();
                        edit.Focus();
                    }
                }
                if (positionHandle > edit.TabIndex)
                {
                    positionHandle = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.SelectAll();
                        edit.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void FillDataToGrid()
        {
            try
            {
                WaitingManager.Show();
                SetDefaultRight();
                HisSereServView1Filter filter = new HisSereServView1Filter();
                filter.EXECUTE_ROOM_IDs = new List<long>() { moduleData.RoomId };
                filter.SERVICE_REQ_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__TT;
                if (dteFrom.EditValue != null && dteFrom.DateTime != DateTime.MinValue)
                    filter.INTRUCTION_TIME_FROM = Int64.Parse(dteFrom.DateTime.ToString("yyyyMMdd") + "000000");
                if (dteTo.EditValue != null && dteTo.DateTime != DateTime.MinValue)
                    filter.INTRUCTION_TIME_TO = Int64.Parse(dteTo.DateTime.ToString("yyyyMMdd") + "235959");
                switch (cboStt.SelectedIndex)
                {
                    case 0:
                        filter.SERVICE_REQ_STT_IDs = new List<long>()
                        {
                            IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__CXL, IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__DXL, IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__HT
                        };
                        break;
                    case 1:
                        filter.SERVICE_REQ_STT_IDs = new List<long>()
                        {
                            IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__CXL
                        };
                        break;
                    case 2:
                        filter.SERVICE_REQ_STT_IDs = new List<long>()
                        {
                            IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__DXL
                        };
                        break;
                    case 3:
                        filter.SERVICE_REQ_STT_IDs = new List<long>()
                        {
                            IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__HT
                        };
                        break;
                    default:
                        cboStt.SelectedIndex = 1;
                        filter.SERVICE_REQ_STT_IDs = new List<long>()
                        {
                            IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__CXL
                        };
                        break;
                }
                if (serviceSelecteds != null && serviceSelecteds.Count > 0)
                    filter.SERVICE_IDs = serviceSelecteds.Select(o => o.ID).ToList();
                if (!string.IsNullOrEmpty(txtPatientCode.Text.Trim()))
                {
                    string patientCode = txtPatientCode.Text.Trim();
                    if (patientCode.Length < 10 && checkDigit(patientCode))
                    {
                        patientCode = string.Format("{0:0000000000}", Convert.ToInt64(patientCode));
                        txtPatientCode.Text = patientCode;
                    }
                    filter.TDL_PATIENT_CODE = patientCode;
                }
                filter.KEY_WORD = txtFind.Text.Trim();
                filter.ORDER_FIELD = "INTRUCTION_TIME";
                filter.ORDER_DIRECTION = "DESC";
                CommonParam paramCommon = new CommonParam();
                var lst = new Inventec.Common.Adapter.BackendAdapter(paramCommon).Get<List<V_HIS_SERE_SERV_1>>("api/HisSereServ/GetView1", ApiConsumers.MosConsumer, filter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, paramCommon);
                lstGrid = new List<SereServView1ADO>();
                if (lst != null && lst.Count > 0)
                {
                    lst = lst.OrderByDescending(o => o.TDL_PATIENT_ID).ThenByDescending(o => o.TDL_INTRUCTION_TIME).ToList();
                    lst.ForEach(o => lstGrid.Add(new SereServView1ADO(o)));
                    gridControl1.DataSource = lstGrid;
                }
                else
                    gridControl1.DataSource = null;
                gridView1.ExpandAllGroups();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private bool checkDigit(string s)
        {
            bool result = true;
            try
            {
                for (int i = 0; i < s.Length; i++)
                {
                    if (char.IsDigit(s[i]) == false) return false;
                }
                return result;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return false;
            }
        }

        private void gridView1_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            try
            {
                currentRow = (SereServView1ADO)gridView1.GetFocusedRow();
                SetDefaultRight();
                ClickGridView();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void LoadUsingExecuteRoomPaymentProcess()
        {
            CommonParam param = new CommonParam();
            Inventec.Common.Logging.LogSystem.Debug("begin call HisPatient/GetCardBalance");
            var balance = new BackendAdapter(param).Get<decimal?>("api/HisPatient/GetCardBalance", ApiConsumers.MosConsumer, this.currentRow.TDL_PATIENT_ID, param);
            Inventec.Common.Logging.LogSystem.Debug("end call HisPatient/GetCardBalance");
        }
        private void SetDefaultRight()
        {
            try
            {
                btnSave.Enabled = false;
                lblPatientName.Text = null;
                lblPatientCode.Text = null;
                lblPatientDob.Text = null;
                lblGender.Text = null;
                lblHeinCardNumber.Text = null;
                lblKCBBD.Text = null;
                lblHeinCardFromTo.Text = null;
                lblType.Text = null;
                lblAddress.Text = null;
                lblNote.Text = null;
                cboDepartment.EditValue = null;
                dteStart.EditValue = null;
                dteFinish.EditValue = null;
                cboPtttMethod.EditValue = null;
                cboEmotionLessMethod.EditValue = null;
                cboPtttMethodReal.EditValue = null;
                cboPtttGroup.EditValue = null;
                cboEkipUser.EditValue = null;
                txtEmotionLessMethod.Text = null;
                txtPtttGroup.Text = null;
                txtPtttMethod.Text = null;
                txtPtttMethodReal.Text = null;
                FillDataToGrid(new List<HisEkipUserADO>() { new HisEkipUserADO() });
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }
        private void ClickGridView()
        {

            try
            {
                if (currentRow == null)
                    return;
                if (currentRow != null && !string.IsNullOrEmpty(currentRow.NOTE))
                {
                    XtraMessageBox.Show(currentRow.NOTE);
                }
                if (HisConfigCFG.StartTimeMustBeGreaterThanInstructionTime == "1" && currentRow != null && Int64.Parse(DateTime.Now.ToString("yyyyMMddHHmmss")) < currentRow.INTRUCTION_TIME)
                {
                    XtraMessageBox.Show("Thời gian bắt đầu không được nhỏ hơn thời gian y lệnh");
                    btnSave.Enabled = false;
                    return;
                }
                ValidForm();
                btnSave.Enabled = true;
                string loginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                L_HIS_SERVICE_REQ serviceReqResult = new BackendAdapter(param)
                .Post<MOS.EFMODEL.DataModels.L_HIS_SERVICE_REQ>(HisRequestUriStore.HIS_SERVICE_REQ_START, ApiConsumers.MosConsumer, currentRow.SERVICE_REQ_ID, param);
                WaitingManager.Hide();
                if (serviceReqResult == null)
                {
                    bool IsShowMessErr = true;
                    if (param.MessageCodes.Contains("HisServiceReq_KhongChoPhepBatDauKhiThieuVienPhi"))
                    {
                        if (HisConfigCFG.IsUsingExecuteRoomPayment)
                        {
                            LoadUsingExecuteRoomPaymentProcess();
                            var room = BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == this.moduleData.RoomId);
                            if (room.DEPOSIT_ACCOUNT_BOOK_ID != null && room.DEFAULT_CASHIER_ROOM_ID != null)
                            {
                                HisCardFilter cfilter = new HisCardFilter();
                                cfilter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                                cfilter.PATIENT_ID = currentRow.TDL_PATIENT_ID;
                                var cards = new BackendAdapter(new CommonParam()).Get<List<HIS_CARD>>("api/HisCard/Get", ApiConsumers.MosConsumer, cfilter, new CommonParam());
                                if (cards != null && cards.Count > 0)
                                {
                                    IsShowMessErr = false;
                                    if (DevExpress.XtraEditors.XtraMessageBox.Show(String.Format("{0} Bạn có muốn đóng tiền không?", param.GetMessage()), "Thông bấo", MessageBoxButtons.YesNo) == DialogResult.Yes)
                                    {
                                        WaitingManager.Show();
                                        EpaymentDepositSD sd = new EpaymentDepositSD();
                                        sd.RequestRoomId = this.moduleData.RoomId;
                                        sd.ServiceReqIds = new List<long>() { currentRow.SERVICE_REQ_ID ?? 0 };
                                        sd.CardServiceCode = null;
                                        sd.IncludeAttachment = false;
                                        CommonParam paramEpay = new CommonParam();
                                        this.epaymentDepositResultSDO = new BackendAdapter(paramEpay).Post<EpaymentDepositResultSDO>("api/HisTransaction/EpaymentDeposit", ApiConsumers.MosConsumer, sd, paramEpay);
                                        WaitingManager.Hide();
                                        if (this.epaymentDepositResultSDO != null)
                                        {
                                            Inventec.Common.RichEditor.RichEditorStore richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumer.ApiConsumers.SarConsumer, HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR, LanguageManager.GetLanguage(), LocalStorage.LocalData.GlobalVariables.TemnplatePathFolder);
                                            richEditorMain.RunPrintTemplate("Mps000102", ProcessPrintMps000102);
                                            param = new CommonParam();
                                            serviceReqResult = new BackendAdapter(param)
                .Post<MOS.EFMODEL.DataModels.L_HIS_SERVICE_REQ>(HisRequestUriStore.HIS_SERVICE_REQ_START, ApiConsumers.MosConsumer, currentRow.SERVICE_REQ_ID, param);
                                        }
                                        else
                                        {
                                            ResultManager.ShowMessage(paramEpay, false);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    if (IsShowMessErr)
                    {
                        #region Show message
                        ResultManager.ShowMessage(param, null);
                        btnSave.Enabled = false;
                        #endregion
                        return;
                    }
                }

                if (currentRow != null && serviceReqResult != null && currentRow.SERVICE_REQ_ID == serviceReqResult.ID)
                {
                    currentRow.SERVICE_REQ_STT_ID = serviceReqResult.SERVICE_REQ_STT_ID;
                    gridControl1.RefreshDataSource();
                    ShowInforPatient();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void ShowInforPatient()
        {
            try
            {
                lblPatientName.Text = currentRow.TDL_PATIENT_NAME;
                lblPatientCode.Text = currentRow.TDL_PATIENT_CODE;
                lblPatientDob.Text = Inventec.Common.DateTime.Convert.TimeNumberToDateString(currentRow.TDL_PATIENT_DOB);
                lblGender.Text = currentRow.TDL_PATIENT_GENDER_NAME;
                lblAddress.Text = currentRow.TDL_PATIENT_ADDRESS;
                lblNote.Text = currentRow.NOTE;
                cboDepartment.EditValue = currentRow.LAST_DEPARTMENT_ID;
                dteStart.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(currentRow.INTRUCTION_TIME) ?? DateTime.MinValue;
                patientTyleAlter = null;
                sp = null;
                ekData = null;
                currentHisService = ServiceList.FirstOrDefault(o => o.ID == currentRow.SERVICE_ID);
                CreatThreadLoadDataInfor();
                if (sp != null)
                {
                    cboPtttMethod.EditValue = sp.PTTT_METHOD_ID;
                    cboEmotionLessMethod.EditValue = sp.EMOTIONLESS_METHOD_SECOND_ID;
                    cboPtttMethodReal.EditValue = sp.REAL_PTTT_METHOD_ID;
                    cboPtttGroup.EditValue = sp.PTTT_GROUP_ID;

                    txtEmotionLessMethod.Text = sp.EMOTIONLESS_METHOD_SECOND_CODE;
                    txtPtttGroup.Text = sp.PTTT_GROUP_CODE;
                    txtPtttMethod.Text = sp.PTTT_METHOD_CODE;
                    txtPtttMethodReal.Text = sp.PTTT_METHOD_CODE;
                }
                else
                {
                    cboPtttMethod.EditValue = null;
                    cboEmotionLessMethod.EditValue = null;
                    cboPtttMethodReal.EditValue = null;
                    cboPtttGroup.EditValue = null;
                    txtEmotionLessMethod.Text = null;
                    txtPtttGroup.Text = null;
                    txtPtttMethod.Text = null;
                    txtPtttMethodReal.Text = null;
                }
                if (patientTyleAlter != null)
                {
                    lblHeinCardNumber.Text = patientTyleAlter.HEIN_CARD_NUMBER;
                    lblKCBBD.Text = patientTyleAlter.HEIN_MEDI_ORG_CODE;
                    lblHeinCardFromTo.Text = Inventec.Common.DateTime.Convert.TimeNumberToDateString(patientTyleAlter.HEIN_CARD_FROM_TIME ?? 0) + (patientTyleAlter.HEIN_CARD_TO_TIME != null ? (" - " + Inventec.Common.DateTime.Convert.TimeNumberToDateString(patientTyleAlter.HEIN_CARD_TO_TIME ?? 0)) : null);

                    var heinRightRouteData = MOS.LibraryHein.Bhyt.HeinRightRoute.HeinRightRouteStore.GetByCode(patientTyleAlter.RIGHT_ROUTE_CODE);
                    lblType.Text = heinRightRouteData != null ? heinRightRouteData.HeinRightRouteName : "";
                }
                else
                {
                    lblHeinCardNumber.Text = null;
                    lblKCBBD.Text = null;
                    lblHeinCardFromTo.Text = null;
                    lblType.Text = null;
                }
                if (ekData != null && ekData.Count > 0)
                {
                    hisEkipUserADOs = new List<HisEkipUserADO>();
                    foreach (var item in ekData)
                    {
                        var dataCheck = BackendDataWorker.Get<HIS_EXECUTE_ROLE>().FirstOrDefault(p => p.ID == item.EXECUTE_ROLE_ID && p.IS_ACTIVE == 1);
                        if (dataCheck == null || dataCheck.ID == 0)
                            continue;
                        HisEkipUserADO HisEkipUserProcessing = new HisEkipUserADO();
                        Inventec.Common.Mapper.DataObjectMapper.Map<HisEkipUserADO>(HisEkipUserProcessing, item);
                        SetDepartment(HisEkipUserProcessing);
                        hisEkipUserADOs.Add(HisEkipUserProcessing);
                    }
                }
                else
                {
                    hisEkipUserADOs = new List<HisEkipUserADO>() { new HisEkipUserADO() };
                }
                FillDataToGrid(hisEkipUserADOs);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private bool ProcessPrintMps000102(string printCode, string fileName)
        {
            bool result = false;
            try
            {
                CommonParam param = new CommonParam();
                HisTreatmentFeeViewFilter filter = new HisTreatmentFeeViewFilter();
                filter.ID = currentRow.TDL_TREATMENT_ID;
                var treatmentFees = new BackendAdapter(param)
                    .Get<List<MOS.EFMODEL.DataModels.V_HIS_TREATMENT_FEE>>(HisRequestUriStore.HIS_TREATMENT_GETFEEVIEW, ApiConsumers.MosConsumer, filter, param).FirstOrDefault();
                V_HIS_PATIENT patientPrint = null;
                if (treatmentFees != null)
                {
                    HisPatientViewFilter filterPatient = new HisPatientViewFilter();
                    filterPatient.ID = treatmentFees != null ? treatmentFees.PATIENT_ID : 0;
                    patientPrint = new BackendAdapter(param)
                        .Get<List<MOS.EFMODEL.DataModels.V_HIS_PATIENT>>(HisRequestUriStore.HIS_PATIENT_GETVIEW, ApiConsumers.MosConsumer, filterPatient, param).FirstOrDefault();
                }
                HisPatientTypeAlterViewFilter filterPatienTypeAlter = new HisPatientTypeAlterViewFilter();
                filterPatienTypeAlter.TREATMENT_ID = currentRow.TDL_TREATMENT_ID;
                var patientTypeAlter = new BackendAdapter(param)
                    .Get<List<MOS.EFMODEL.DataModels.V_HIS_PATIENT_TYPE_ALTER>>("/api/HisPatientTypeAlter/GetView", ApiConsumers.MosConsumer, filterPatienTypeAlter, param).OrderByDescending(o => o.ID).ThenByDescending(o => o.LOG_TIME).FirstOrDefault();

                if (this.epaymentDepositResultSDO != null && this.epaymentDepositResultSDO.SereServDeposit != null && this.epaymentDepositResultSDO.SereServDeposit.Count > 0 && this.epaymentDepositResultSDO.Transaction != null)
                {
                    V_HIS_TRANSACTION transactionPrint = new V_HIS_TRANSACTION();
                    List<HIS_SERE_SERV_DEPOSIT> ssDepositPrint = new List<HIS_SERE_SERV_DEPOSIT>();
                    if (this.epaymentDepositResultSDO.Transaction.TRANSACTION_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TRANSACTION_TYPE.ID__TU)
                    {
                        transactionPrint = this.epaymentDepositResultSDO.Transaction;
                    }
                    if (transactionPrint == null)
                        return result;
                    ssDepositPrint = this.epaymentDepositResultSDO.SereServDeposit.Where(o => o.DEPOSIT_ID == transactionPrint.ID).ToList();

                    //chỉ định chưa có thời gian ra viện nên chưa cso số ngày điều trị
                    long? totalDay = null;
                    string departmentName = "";

                    //sử dụng SereServs để hiển thị thêm dịch vụ thanh toán cha
                    List<V_HIS_SERE_SERV> sereServs = new List<V_HIS_SERE_SERV>();
                    if (this.epaymentDepositResultSDO.SereServs != null && this.epaymentDepositResultSDO.SereServs.Count > 0)
                    {
                        sereServs = this.epaymentDepositResultSDO.SereServs.Where(o => ssDepositPrint.Exists(e => e.SERE_SERV_ID == o.ID)).ToList();
                    }
                    var SERVICE_REPORT_ID__HIGHTECH = IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__DVKTC;

                    var sereServHitechs = sereServs.Where(o => o.TDL_HEIN_SERVICE_TYPE_ID == SERVICE_REPORT_ID__HIGHTECH).ToList();
                    var sereServHitechADOs = PriceBHYTSereServAdoProcess(sereServHitechs);
                    //các sereServ trong nhóm vật tư
                    var SERVICE_REPORT__MATERIAL_VTTT_ID = IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__VT_TT;
                    var sereServVTTTs = sereServs.Where(o => o.TDL_HEIN_SERVICE_TYPE_ID == SERVICE_REPORT__MATERIAL_VTTT_ID && o.IS_OUT_PARENT_FEE != null).ToList();
                    var sereServVTTTADOs = PriceBHYTSereServAdoProcess(sereServVTTTs);
                    var sereServNotHitechs = sereServs.Where(o => o.TDL_HEIN_SERVICE_TYPE_ID != SERVICE_REPORT_ID__HIGHTECH).ToList();

                    var servicePatyPrpos = BackendDataWorker.Get<V_HIS_SERVICE>();
                    //Cộng các sereServ trong gói vào dv ktc
                    foreach (var sereServHitech in sereServHitechADOs)
                    {
                        List<MPS.Processor.Mps000102.PDO.SereServGroupPlusADO> sereServVTTTInKtcADOs = new List<MPS.Processor.Mps000102.PDO.SereServGroupPlusADO>();
                        var sereServVTTTInKtcs = sereServs.Where(o => o.PARENT_ID == sereServHitech.ID && o.IS_OUT_PARENT_FEE == null).ToList();
                        sereServVTTTInKtcADOs = PriceBHYTSereServAdoProcess(sereServVTTTInKtcs);
                        if (sereServHitech.PRICE_POLICY != 0)
                        {
                            var servicePatyPrpo = servicePatyPrpos.Where(o => o.ID == sereServHitech.SERVICE_ID && o.BILL_PATIENT_TYPE_ID == sereServHitech.PATIENT_TYPE_ID && o.PACKAGE_PRICE == sereServHitech.PRICE_POLICY).ToList();
                            if (servicePatyPrpo != null && servicePatyPrpo.Count > 0)
                            {
                                sereServHitech.VIR_PRICE = sereServHitech.PRICE;
                            }
                        }
                        else
                            sereServHitech.VIR_PRICE += sereServVTTTInKtcADOs.Sum(o => o.VIR_TOTAL_PRICE);

                        sereServHitech.VIR_HEIN_PRICE += sereServVTTTInKtcADOs.Sum(o => o.VIR_HEIN_PRICE);
                        sereServHitech.VIR_PATIENT_PRICE += sereServVTTTInKtcADOs.Sum(o => o.VIR_HEIN_PRICE);

                        decimal totalHeinPrice = 0;
                        foreach (var sereServVTTTInKtcADO in sereServVTTTInKtcADOs)
                        {
                            totalHeinPrice += sereServVTTTInKtcADO.AMOUNT * sereServVTTTInKtcADO.PRICE_BHYT;
                        }
                        sereServHitech.PRICE_BHYT += totalHeinPrice;
                        sereServHitech.HEIN_LIMIT_PRICE += sereServVTTTInKtcADOs.Sum(o => o.HEIN_LIMIT_PRICE);

                        sereServHitech.VIR_TOTAL_PRICE += sereServVTTTInKtcADOs.Sum(o => o.VIR_TOTAL_PRICE);
                        sereServHitech.VIR_TOTAL_HEIN_PRICE += sereServVTTTInKtcADOs.Sum(o => o.VIR_TOTAL_HEIN_PRICE);
                        sereServHitech.VIR_TOTAL_PATIENT_PRICE = sereServHitech.VIR_TOTAL_PRICE - sereServHitech.VIR_TOTAL_HEIN_PRICE;
                        sereServHitech.SERVICE_UNIT_NAME = BackendDataWorker.Get<HIS_SERVICE_UNIT>().FirstOrDefault(o => o.ID == sereServHitech.TDL_SERVICE_UNIT_ID).SERVICE_UNIT_NAME;
                    }

                    //Lọc các sereServ không nằm trong dịch vụ ktc và vật tư thay thế
                    //
                    var sereServDeleteADOs = new List<MPS.Processor.Mps000102.PDO.SereServGroupPlusADO>();
                    foreach (var sereServVTTTADO in sereServVTTTADOs)
                    {
                        var sereServADODelete = sereServHitechADOs.Where(o => o.ID == sereServVTTTADO.PARENT_ID).ToList();
                        if (sereServADODelete.Count == 0)
                        {
                            sereServDeleteADOs.Add(sereServVTTTADO);
                        }
                    }

                    foreach (var sereServDelete in sereServDeleteADOs)
                    {
                        sereServVTTTADOs.Remove(sereServDelete);
                    }
                    var sereServVTTTIds = sereServVTTTADOs.Select(o => o.ID);
                    sereServNotHitechs = sereServNotHitechs.Where(o => !sereServVTTTIds.Contains(o.ID)).ToList();
                    var sereServNotHitechADOs = PriceBHYTSereServAdoProcess(sereServNotHitechs);
                    string ratio_text = "";
                    if (patientTypeAlter != null)
                    {
                        ratio_text = ((new MOS.LibraryHein.Bhyt.BhytHeinProcessor().GetDefaultHeinRatio(patientTypeAlter.HEIN_TREATMENT_TYPE_CODE, patientTypeAlter.HEIN_CARD_NUMBER, patientTypeAlter.LEVEL_CODE, patientTypeAlter.RIGHT_ROUTE_CODE) ?? 0) * 100) + "";
                    }
                    MPS.Processor.Mps000102.PDO.PatientADO patientAdo = new MPS.Processor.Mps000102.PDO.PatientADO(patientPrint);

                    if (sereServNotHitechADOs != null && sereServNotHitechADOs.Count > 0)
                    {
                        sereServNotHitechADOs = sereServNotHitechADOs.OrderBy(o => o.TDL_SERVICE_NAME).ToList();
                    }

                    if (sereServHitechADOs != null && sereServHitechADOs.Count > 0)
                    {
                        sereServHitechADOs = sereServHitechADOs.OrderBy(o => o.TDL_SERVICE_NAME).ToList();
                    }

                    if (sereServVTTTADOs != null && sereServVTTTADOs.Count > 0)
                    {
                        sereServVTTTADOs = sereServVTTTADOs.OrderBy(o => o.TDL_SERVICE_NAME).ToList();
                    }

                    V_HIS_SERVICE_REQ firsExamRoom = new V_HIS_SERVICE_REQ();
                    if (treatmentFees.TDL_FIRST_EXAM_ROOM_ID.HasValue)
                    {
                        var room = BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == treatmentFees.TDL_FIRST_EXAM_ROOM_ID);
                        if (room != null)
                        {
                            firsExamRoom.EXECUTE_ROOM_NAME = room.ROOM_NAME;
                        }
                    }
                    MPS.Processor.Mps000102.PDO.Mps000102PDO mps000102RDO = new MPS.Processor.Mps000102.PDO.Mps000102PDO(
                            patientAdo,
                            patientTypeAlter,
                            departmentName,

                            sereServNotHitechADOs,
                            sereServHitechADOs,
                            sereServVTTTADOs,

                            null,//bản tin chuyển khoa, mps lấy ramdom thời gian vào khoa khi chỉ định tạm thời chưa cần
                            treatmentFees,

                            BackendDataWorker.Get<HIS_HEIN_SERVICE_TYPE>(),
                            transactionPrint,
                            ssDepositPrint,
                            totalDay,
                            ratio_text,
                            firsExamRoom
                            );
                    WaitingManager.Hide();

                    string printerName = "";
                    if (GlobalVariables.dicPrinter.ContainsKey(printCode))
                    {
                        printerName = GlobalVariables.dicPrinter[printCode];
                    }

                    Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((treatmentFees != null ? treatmentFees.TREATMENT_CODE : ""), printCode, moduleData != null ? moduleData.RoomId : 0);
                    result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printCode, fileName, mps000102RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName) { EmrInputADO = inputADO });
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        public List<MPS.Processor.Mps000102.PDO.SereServGroupPlusADO> PriceBHYTSereServAdoProcess(List<V_HIS_SERE_SERV> sereServs)
        {
            List<MPS.Processor.Mps000102.PDO.SereServGroupPlusADO> sereServADOs = new List<MPS.Processor.Mps000102.PDO.SereServGroupPlusADO>();
            try
            {
                foreach (var item in sereServs)
                {
                    MPS.Processor.Mps000102.PDO.SereServGroupPlusADO sereServADO = new MPS.Processor.Mps000102.PDO.SereServGroupPlusADO();
                    Inventec.Common.Mapper.DataObjectMapper.Map<MPS.Processor.Mps000102.PDO.SereServGroupPlusADO>(sereServADO, item);

                    if (sereServADO.PATIENT_TYPE_ID != HisConfigCFG.PatientTypeId__BHYT)
                    {
                        sereServADO.PRICE_BHYT = 0;
                    }
                    else
                    {
                        if (sereServADO.HEIN_LIMIT_PRICE != null && sereServADO.HEIN_LIMIT_PRICE > 0)
                            sereServADO.PRICE_BHYT = (item.HEIN_LIMIT_PRICE ?? 0);
                        else
                            sereServADO.PRICE_BHYT = item.VIR_PRICE_NO_ADD_PRICE ?? 0;
                    }

                    sereServADOs.Add(sereServADO);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return sereServADOs;
        }

        private void gridView1_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData)
                {
                    if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                    {
                        var data = (SereServView1ADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                        if (data != null)
                        {
                            if (e.Column.FieldName == "INTRUCTION_TIME_str")
                            {
                                e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.INTRUCTION_TIME);
                            }
                            else if (e.Column.FieldName == "TRANGTHAI_IMG")
                            {
                                switch (data.SERVICE_REQ_STT_ID)
                                {
                                    case IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__HT:
                                        e.Value = imageList1.Images[0];
                                        break;
                                    case IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__CXL:
                                        e.Value = imageList1.Images[1];
                                        break;
                                    case IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__DXL:
                                        e.Value = imageList1.Images[2];
                                        break;
                                    default:
                                        e.Value = null;
                                        break;
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

        private void gridView1_CustomDrawGroupRow(object sender, RowObjectCustomDrawEventArgs e)
        {
            try
            {
                var info = e.Info as DevExpress.XtraGrid.Views.Grid.ViewInfo.GridGroupRowInfo;
                info.GroupText = "Họ tên: " + Convert.ToString(this.gridView1.GetGroupRowValue(e.RowHandle, this.gridColumn12) ?? "") + ": " + lstGrid.Where(o => o.GroupFieldName == Convert.ToString(this.gridView1.GetGroupRowValue(e.RowHandle, this.gridColumn12) ?? "")).Count();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}

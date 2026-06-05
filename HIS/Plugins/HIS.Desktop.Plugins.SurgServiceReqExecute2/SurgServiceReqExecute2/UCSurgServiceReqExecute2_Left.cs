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
                gridView1.BeginUpdate();
                try
                {
                if (lst != null && lst.Count > 0)
                {
                    var patientTypeRaw = BackendDataWorker.Get<HIS_PATIENT_TYPE>();
                    var patientTypeDict = patientTypeRaw != null
                        ? patientTypeRaw.GroupBy(o => o.ID).ToDictionary(g => g.Key, g => g.First())
                        : new Dictionary<long, HIS_PATIENT_TYPE>();

                    Dictionary<long, HIS_SERE_SERV_EXT> extDict = BatchLoadSereServExt_v45072(lst.Select(o => o.ID).ToList());

                    foreach (var o in lst)
                    {
                        var ado = new SereServView1ADO(o);
                        FillView45072Fields(ado, o, patientTypeDict);
                        HIS_SERE_SERV_EXT ext;
                        if (extDict != null && extDict.TryGetValue(o.ID, out ext) && ext != null)
                        {
                            ado.BEGIN_TIME_STR = ext.BEGIN_TIME.HasValue
                                ? Inventec.Common.DateTime.Convert.TimeNumberToTimeString(ext.BEGIN_TIME.Value)
                                : "";
                            ado.END_TIME_STR = ext.END_TIME.HasValue
                                ? Inventec.Common.DateTime.Convert.TimeNumberToTimeString(ext.END_TIME.Value)
                                : "";
                        }
                        lstGrid.Add(ado);
                    }

                    // Sinh PatientGroupKey + sắp xếp lại danh sách (gom nhóm theo BN + phút chỉ định + tier trạng thái)
                    BuildPatientGroupKeyAndSort_v45072();
                    ApplyPatientGrouping_v45072();
                    gridControl1.DataSource = lstGrid;
                }
                else
                    gridControl1.DataSource = null;
                gridView1.ExpandAllGroups();

                // F: giữ vị trí cuộn + focus dòng vừa lưu (chỉ khi đến từ luồng Lưu)
                if (isRestoreAfterSave_v45072)
                {
                    RestoreGridPositionAfterSave_v45072();
                    isRestoreAfterSave_v45072 = false;
                }
                }
                finally
                {
                    gridView1.EndUpdate();
                }
                UpdateFooter45072();
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

                if (currentRow.SERVICE_REQ_STT_ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__CXL)
                {
                    ShowInforPatient();
                    return;
                }

                WaitingManager.Show();
                CommonParam param = new CommonParam();
                var startSdo_v45072 = new MOS.SDO.HisServiceReqStartSDO();
                startSdo_v45072.ID = currentRow.SERVICE_REQ_ID ?? 0;
                L_HIS_SERVICE_REQ serviceReqResult = new BackendAdapter(param)
                .Post<MOS.EFMODEL.DataModels.L_HIS_SERVICE_REQ>(HisRequestUriStore.HIS_SERVICE_REQ_START, ApiConsumers.MosConsumer, startSdo_v45072, param);
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
                                            var startSdoRetry_v45072 = new MOS.SDO.HisServiceReqStartSDO();
                                            startSdoRetry_v45072.ID = currentRow.SERVICE_REQ_ID ?? 0;
                                            serviceReqResult = new BackendAdapter(param)
                .Post<MOS.EFMODEL.DataModels.L_HIS_SERVICE_REQ>(HisRequestUriStore.HIS_SERVICE_REQ_START, ApiConsumers.MosConsumer, startSdoRetry_v45072, param);
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

                // Reset 4 cặp control trước
                cboPtttMethod.EditValue = null;
                cboEmotionLessMethod.EditValue = null;
                cboPtttMethodReal.EditValue = null;
                cboPtttGroup.EditValue = null;
                txtEmotionLessMethod.Text = null;
                txtPtttGroup.Text = null;
                txtPtttMethod.Text = null;
                txtPtttMethodReal.Text = null;

                // 1. Phương pháp
                if (sp != null && sp.PTTT_METHOD_ID > 0)
                {
                    cboPtttMethod.EditValue = sp.PTTT_METHOD_ID;
                    txtPtttMethod.Text = sp.PTTT_METHOD_CODE;
                }
                else
                {
                    SetDefaultCboPTMethod_v45072(cboPtttMethod, txtPtttMethod);
                }

                // 2. Phương pháp TT
                if (sp != null && sp.REAL_PTTT_METHOD_ID > 0)
                {
                    cboPtttMethodReal.EditValue = sp.REAL_PTTT_METHOD_ID;
                    txtPtttMethodReal.Text = LookupPtttMethodCode_v45072(sp.REAL_PTTT_METHOD_ID);
                }
                else
                {
                    SetDefaultCboPTMethod_v45072(cboPtttMethodReal, txtPtttMethodReal);
                }

                // 3. Phân loại
                if (sp != null && sp.PTTT_GROUP_ID.HasValue)
                {
                    cboPtttGroup.EditValue = sp.PTTT_GROUP_ID;
                    txtPtttGroup.Text = sp.PTTT_GROUP_CODE;
                }
                else
                {
                    SetDefaultCboPtttGroup_v45072(cboPtttGroup, txtPtttGroup);
                }

                // 4. Phương pháp 2 (EmotionLessMethod) — chỉ fill khi sp có data, không có default từ service
                if (sp != null)
                {
                    cboEmotionLessMethod.EditValue = sp.EMOTIONLESS_METHOD_SECOND_ID;
                    txtEmotionLessMethod.Text = sp.EMOTIONLESS_METHOD_SECOND_CODE;
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

                FillExtendedDataWhenClickRow(currentRow);
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
                if (info == null) return;
                // Cột gom nhóm (gridColumn5) đã đổi binding sang PatientGroupKey; gridColumn12 (GroupFieldName)
                // giữ nhãn "Tên: Mã" — không đổi trong cùng 1 nhóm nên GetGroupRowValue trả về đúng.
                string groupKey = Convert.ToString(this.gridView1.GetGroupRowValue(e.RowHandle, this.gridColumn5) ?? "");
                string display = Convert.ToString(this.gridView1.GetGroupRowValue(e.RowHandle, this.gridColumn12) ?? "");
                int count = lstGrid != null ? lstGrid.Count(o => o.PatientGroupKey == groupKey) : 0;
                info.GroupText = "Họ tên: " + display + ": " + count;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #region Sắp xếp + gom nhóm danh sách BN (PatientGroupKey) + giữ vị trí cuộn sau lưu

        /// <summary>ID dòng vừa lưu — dùng focus lại sau khi reload.</summary>
        internal long savedFocusedId_v45072 = 0;
        /// <summary>Vị trí cuộn dọc trước khi reload.</summary>
        internal int savedTopRowIndex_v45072 = -1;
        /// <summary>true khi reload đến từ luồng Lưu (cần giữ vị trí); false khi user bấm Tìm thủ công.</summary>
        internal bool isRestoreAfterSave_v45072 = false;
        /// <summary>Chỉ đổi cột gom nhóm sang PatientGroupKey 1 lần.</summary>
        private bool isGroupingApplied_v45072 = false;

        /// <summary>Độ ưu tiên trạng thái: CXL=0, DXL=1, HT=2, khác=3.</summary>
        private int StatusPri_v45072(long sttId)
        {
            if (sttId == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__CXL) return 0;
            if (sttId == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__DXL) return 1;
            if (sttId == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__HT) return 2;
            return 3;
        }

        /// <summary>
        /// Sinh PatientGroupKey cho từng dòng và sắp xếp lstGrid.
        /// Nhóm = (phút chỉ định + mã BN); tier nhóm = MIN trạng thái trong nhóm.
        /// </summary>
        private void BuildPatientGroupKeyAndSort_v45072()
        {
            try
            {
                if (lstGrid == null || lstGrid.Count == 0) return;

                // MinStatusOfGroup theo cặp (phút chỉ định + mã BN)
                var minStatusDict = new Dictionary<string, int>();
                foreach (var o in lstGrid)
                {
                    string subKey = (o.INTRUCTION_TIME / 100L) + "_" + (o.TDL_PATIENT_CODE ?? "");
                    int pri = StatusPri_v45072(o.SERVICE_REQ_STT_ID);
                    int cur;
                    if (!minStatusDict.TryGetValue(subKey, out cur) || pri < cur)
                        minStatusDict[subKey] = pri;
                }

                // PatientGroupKey = minStatus _ (đảo phút để sort tăng dần = giờ mới nhất lên trên) _ mã BN
                foreach (var o in lstGrid)
                {
                    long minute = o.INTRUCTION_TIME / 100L;
                    string subKey = minute + "_" + (o.TDL_PATIENT_CODE ?? "");
                    int minStatus = minStatusDict.ContainsKey(subKey) ? minStatusDict[subKey] : 3;
                    o.PatientGroupKey = string.Format("{0}_{1:D12}_{2}", minStatus, (999999999999L - minute), o.TDL_PATIENT_CODE ?? "");
                }

                // Sort: tier nhóm ASC -> phút chỉ định DESC (mới nhất trên) -> mã BN -> trạng thái trong nhóm -> giờ đầy đủ
                lstGrid = lstGrid
                    .OrderBy(o => minStatusDict[(o.INTRUCTION_TIME / 100L) + "_" + (o.TDL_PATIENT_CODE ?? "")])
                    .ThenByDescending(o => o.INTRUCTION_TIME / 100L)
                    .ThenBy(o => o.TDL_PATIENT_ID)
                    .ThenBy(o => StatusPri_v45072(o.SERVICE_REQ_STT_ID))
                    .ThenBy(o => o.INTRUCTION_TIME)
                    .ToList();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>Đổi cột gom nhóm (gridColumn5) sang PatientGroupKey, sắp tăng dần (key đã mã hóa thứ tự).</summary>
        private void ApplyPatientGrouping_v45072()
        {
            try
            {
                if (isGroupingApplied_v45072) return;
                if (gridColumn5 != null)
                {
                    gridColumn5.FieldName = "PatientGroupKey";
                    gridColumn5.SortOrder = DevExpress.Data.ColumnSortOrder.Ascending;
                }
                isGroupingApplied_v45072 = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>Sau khi lưu: focus lại dòng vừa lưu (hoặc dòng kế cận) và giữ nguyên vị trí cuộn.</summary>
        private void RestoreGridPositionAfterSave_v45072()
        {
            try
            {
                if (gridView1 == null) return;

                int targetListIndex = -1;
                if (lstGrid != null && savedFocusedId_v45072 > 0)
                {
                    for (int i = 0; i < lstGrid.Count; i++)
                    {
                        if (lstGrid[i].ID == savedFocusedId_v45072) { targetListIndex = i; break; }
                    }
                }
                if (targetListIndex >= 0)
                {
                    int rh = gridView1.GetRowHandle(targetListIndex);
                    if (rh >= 0)
                    {
                        gridView1.FocusedRowHandle = rh;
                        currentRow = lstGrid[targetListIndex];
                    }
                }

                if (savedTopRowIndex_v45072 >= 0)
                {
                    int maxTop = Math.Max(0, gridView1.RowCount - 1);
                    int top = Math.Min(savedTopRowIndex_v45072, maxTop);
                    if (top >= 0) gridView1.TopRowIndex = top;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion

        #region 5 cột grid + Footer

        private void FillView45072Fields(SereServView1ADO ado, V_HIS_SERE_SERV_1 raw, Dictionary<long, HIS_PATIENT_TYPE> patientTypeDict)
        {
            try
            {
                if (ado == null || raw == null) return;

                // ĐTTT — Tên đối tượng thanh toán (V_HIS_SERE_SERV_1.PATIENT_TYPE_ID là long, default 0 → bỏ qua nếu 0)
                if (raw.PATIENT_TYPE_ID > 0 && patientTypeDict != null)
                {
                    HIS_PATIENT_TYPE pt;
                    if (patientTypeDict.TryGetValue(raw.PATIENT_TYPE_ID, out pt) && pt != null)
                        ado.PATIENT_TYPE_NAME = pt.PATIENT_TYPE_NAME;
                }

                string reqUsername = raw.TDL_REQUEST_USERNAME;
                string reqLoginname = raw.TDL_REQUEST_LOGINNAME;
                if (!string.IsNullOrWhiteSpace(reqUsername))
                    ado.REQUEST_DOCTOR_DISPLAY = string.Format("{0} - {1}", reqUsername, reqLoginname ?? "");
                else if (!string.IsNullOrWhiteSpace(reqLoginname))
                    ado.REQUEST_DOCTOR_DISPLAY = reqLoginname;

                // BEGIN_TIME / END_TIME — pre-loaded batch trong FillDataToGrid (KHÔNG cần set "" ở đây vì
                // ADO mặc định là null; nếu batch không có ext sẽ giữ null).

                // Đơn giá — V_HIS_SERE_SERV_1.PRICE là Decimal (non-nullable)
                ado.PRICE_V45072 = raw.PRICE;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private Dictionary<long, HIS_SERE_SERV_EXT> BatchLoadSereServExt_v45072(List<long> sereServIds)
        {
            var result = new Dictionary<long, HIS_SERE_SERV_EXT>();
            try
            {
                if (sereServIds == null || sereServIds.Count == 0) return result;
                CommonParam param = new CommonParam();
                var filter = new HisSereServExtFilter();
                filter.SERE_SERV_IDs = sereServIds;
                var lst = new BackendAdapter(param).Get<List<HIS_SERE_SERV_EXT>>(
                    HisRequestUriStore.MOSHIS_HIS_SERE_SERV_EXT_GET, ApiConsumers.MosConsumer, filter, param);
                if (lst != null && lst.Count > 0)
                {
                    foreach (var item in lst)
                    {
                        if (item == null) continue;
                        if (!result.ContainsKey(item.SERE_SERV_ID))
                            result[item.SERE_SERV_ID] = item;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private void UpdateFooter45072()
        {
            try
            {
                int totalPatient = 0;
                int totalService = 0;
                if (lstGrid != null && lstGrid.Count > 0)
                {
                    totalService = lstGrid.Count;
                    totalPatient = lstGrid.Select(o => o.TDL_PATIENT_ID).Distinct().Count();
                }
                if (lblTotalPatient_v45072 != null)
                    lblTotalPatient_v45072.Text = Resources.ResourceMessage.TongSoBN + totalPatient;
                if (lblTotalService_v45072 != null)
                    lblTotalService_v45072.Text = Resources.ResourceMessage.TongSoDichVu + totalService;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void GridView1_CustomUnbound_v45072(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (!e.IsGetData || e.Column == null) return;
                var data = (SereServView1ADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                if (data == null) return;
                switch (e.Column.FieldName)
                {
                    case "PATIENT_TYPE_NAME":
                        e.Value = data.PATIENT_TYPE_NAME;
                        break;
                    case "REQUEST_DOCTOR_DISPLAY":
                        e.Value = data.REQUEST_DOCTOR_DISPLAY;
                        break;
                    case "BEGIN_TIME_STR":
                        e.Value = data.BEGIN_TIME_STR;
                        break;
                    case "END_TIME_STR":
                        e.Value = data.END_TIME_STR;
                        break;
                    case "PRICE_V45072":
                        e.Value = data.PRICE_V45072;
                        break;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private string LookupPtttMethodCode_v45072(long? methodId)
        {
            try
            {
                if (!methodId.HasValue || methodId.Value <= 0) return string.Empty;
                var m = BackendDataWorker.Get<HIS_PTTT_METHOD>()
                    .FirstOrDefault(o => o.ID == methodId.Value);
                return m != null ? m.PTTT_METHOD_CODE : string.Empty;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return string.Empty;
            }
        }

        private void SetDefaultCboPTMethod_v45072(
            Inventec.Desktop.CustomControl.CustomGridLookUpEditWithFilterMultiColumn cbo,
            DevExpress.XtraEditors.TextEdit txt)
        {
            try
            {
                if (currentRow == null || currentRow.SERVICE_ID <= 0) return;

                // 1. ƯU TIÊN: HIS_SERVICE.PTTT_METHOD_ID (cấu hình DVKT) — phản ánh chỉnh sửa danh mục
                var service = BackendDataWorker.Get<HIS_SERVICE>()
                    .FirstOrDefault(o => o.ID == currentRow.SERVICE_ID);
                if (service != null && service.PTTT_METHOD_ID.HasValue)
                {
                    var ptttMethodCfg = BackendDataWorker.Get<HIS_PTTT_METHOD>()
                        .FirstOrDefault(o => o.ID == service.PTTT_METHOD_ID.Value);
                    if (ptttMethodCfg != null)
                    {
                        if (cbo != null) cbo.EditValue = ptttMethodCfg.ID;
                        if (txt != null) txt.Text = ptttMethodCfg.PTTT_METHOD_CODE;
                        return;
                    }
                }

                // 2. FALLBACK: name-match HIS_PTTT_METHOD.NAME == TDL_SERVICE_NAME
                if (string.IsNullOrEmpty(currentRow.TDL_SERVICE_NAME)) return;
                string svcName = currentRow.TDL_SERVICE_NAME.ToLower();
                var ptttMethod = BackendDataWorker.Get<HIS_PTTT_METHOD>()
                    .FirstOrDefault(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE
                        && o.PTTT_METHOD_NAME != null
                        && o.PTTT_METHOD_NAME.ToLower() == svcName);
                if (ptttMethod != null)
                {
                    if (cbo != null) cbo.EditValue = ptttMethod.ID;
                    if (txt != null) txt.Text = ptttMethod.PTTT_METHOD_CODE;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetDefaultCboPtttGroup_v45072(
            Inventec.Desktop.CustomControl.CustomGridLookUpEditWithFilterMultiColumn cbo,
            DevExpress.XtraEditors.TextEdit txt)
        {
            try
            {
                if (currentRow == null || currentRow.SERVICE_ID <= 0) return;
                var service = BackendDataWorker.Get<HIS_SERVICE>()
                    .FirstOrDefault(o => o.ID == currentRow.SERVICE_ID);
                if (service == null || !service.PTTT_GROUP_ID.HasValue) return;

                var ptttGroup = BackendDataWorker.Get<HIS_PTTT_GROUP>()
                    .FirstOrDefault(o => o.ID == service.PTTT_GROUP_ID.Value);
                if (ptttGroup != null)
                {
                    if (cbo != null) cbo.EditValue = ptttGroup.ID;
                    if (txt != null) txt.Text = ptttGroup.PTTT_GROUP_CODE;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion
    }
}

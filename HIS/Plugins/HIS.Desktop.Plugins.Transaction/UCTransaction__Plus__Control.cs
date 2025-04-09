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
using DevExpress.XtraBars;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.ConfigSystem;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.Transaction.ADO;
using HIS.Desktop.Plugins.Transaction.Base;
using HIS.Desktop.Plugins.Transaction.Config;
using HIS.Desktop.Print;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.Transaction
{
    public partial class UCTransaction : UserControlBase
    {
        bool IsShowPatientInfomation = false;
        private void dtFromTime_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    dtInTimeTo.Focus();
                    dtInTimeTo.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dtToTime_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    txtKeyword.Focus();
                    txtKeyword.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtFindTreatmentCode_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (Char.IsDigit(e.KeyChar) || Char.IsControl(e.KeyChar))
                {
                }
                else
                {
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtFindTreatmentCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (!String.IsNullOrEmpty(txtFindTreatmentCode.Text))
                    {
                        positionHandleControl = -1;
                        if (!dxValidationProvider1.Validate())
                            return;
                        WaitingManager.Show();
                        FillDataToGridTreatment();
                        if (listTreatment != null && listTreatment.Count == 1)
                        {
                            this.currentTreatment = listTreatment.First();
                            FillDataToControlBySelectTreatment(false, true);
                            SetEnableButton(null);
                        }
                        txtFindTreatmentCode.SelectAll();
                        WaitingManager.Hide();
                    }
                    else
                    {
                        txtKeyword.Focus();
                        txtKeyword.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtKeyword_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    WaitingManager.Show();
                    FillDataToGridTreatment();
                    if (listTreatment != null && listTreatment.Count == 1)
                    {
                        this.currentTreatment = listTreatment.First();
                        FillDataToControlBySelectTreatment(false);
                        SetEnableButton(null);
                    }
                    gridControlTreatment.Focus();
                    WaitingManager.Hide();
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewTreatment_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.ListSourceRowIndex >= 0 && e.IsGetData && e.Column.UnboundType != DevExpress.Data.UnboundColumnType.Bound)
                {
                    var data = (V_HIS_TREATMENT_FEE)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        if (e.Column.FieldName == "STT")
                        {
                            try
                            {
                                e.Value = e.ListSourceRowIndex + 1 + (ucPaging1.pagingGrid.CurrentPage - 1) * ucPaging1.pagingGrid.PageSize;
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Error(ex);
                            }
                        }
                        else if (e.Column.FieldName == "DOB_STR")
                        {
                            try
                            {
                                e.Value = data.TDL_PATIENT_DOB.ToString().Substring(0, 4);
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Error(ex);
                            }
                        }
                        else if (e.Column.FieldName == "FEE_LOCK_USERNAME_STR" && !string.IsNullOrEmpty(data.FEE_LOCK_LOGINNAME))
                        {
                            try
                            {
                                e.Value = data.FEE_LOCK_USERNAME.ToString() + "-" + data.FEE_LOCK_LOGINNAME.ToString();
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Error(ex);
                            }
                        }
                        else if (e.Column.FieldName == "FEE_LOCK_TIME_STR")
                        {
                            try
                            {
                                e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(data.FEE_LOCK_TIME ?? 0);
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Error(ex);
                            }
                        }
                        else if (e.Column.FieldName == "OUT_TIME_STR")
                        {
                            try
                            {
                                e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(data.OUT_TIME ?? 0);
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Error(ex);
                            }
                        }
                        else if (e.Column.FieldName == "CREATE_TIME_STR")
                        {
                            try
                            {
                                e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(data.IN_TIME);
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Error(ex);
                            }
                        }
                        else if (e.Column.FieldName == "TREATMENT_RESULT_NAME")
                        {
                            try
                            {
                                if (data.TREATMENT_RESULT_ID != null && data.TREATMENT_RESULT_ID > 0 && this.treatmentResult != null)
                                    e.Value = this.treatmentResult[data.TREATMENT_RESULT_ID.Value].TREATMENT_RESULT_NAME;
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Error(ex);
                            }
                        }

                    }
                }
            }
            catch { }
        }

        private void gridViewTreatment_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                if (e.RowHandle < 0)
                    return;
                //var data = (V_HIS_TREATMENT_FEE)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                //if (data != null)
                //{
                short isActive = Inventec.Common.TypeConvert.Parse.ToInt16((gridViewTreatment.GetRowCellValue(e.RowHandle, "IS_ACTIVE") ?? "").ToString());

                short isPause = Inventec.Common.TypeConvert.Parse.ToInt16((gridViewTreatment.GetRowCellValue(e.RowHandle, "IS_PAUSE") ?? "").ToString());
                short isTemporaryLock = Inventec.Common.TypeConvert.Parse.ToInt16((gridViewTreatment.GetRowCellValue(e.RowHandle, "IS_TEMPORARY_LOCK") ?? "").ToString());
                if (e.Column.FieldName == "IMAGE_STATUS")
                {
                    if (isActive == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__FALSE)
                    {
                        e.RepositoryItem = repositoryItemImgIsLockFee;
                    }
                    else if (isTemporaryLock == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)
                    {
                        e.RepositoryItem = ButtonEdit_TemporaryLock;
                    }
                    else if (isPause == 1)
                    {
                        e.RepositoryItem = repositoryItemImgIsPause;
                    }
                    else
                    {
                        e.RepositoryItem = repositoryItemImgOpen;
                    }
                }
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewTreatment_RowClick(object sender, DevExpress.XtraGrid.Views.Grid.RowClickEventArgs e)
        {
            try
            {
                if (e.RowHandle < 0)
                    return;
                ProcessClickViewTreatment(sender, e.RowHandle);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewTreatment_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            try
            {
                if (e.Column.FieldName == "CallPatient")
                {
                    this.currentTreatment = (V_HIS_TREATMENT_FEE)gridViewTreatment.GetFocusedRow();
                    if (this.currentTreatment != null)
                    {
                        Inventec.Common.Logging.LogSystem.Debug("gridView_RowCellClick. 1");

                        CallModuleCallPatientNumOrder(this.currentTreatment);

                        LoadCallPatientByThread(this.currentTreatment);
                        Inventec.Common.Logging.LogSystem.Debug("gridView_RowCellClick. 3");
                    }
                    gridViewTreatment.FocusedColumn = gridViewTreatment.Columns[1];
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void gridViewTreatment_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (gridViewTreatment.FocusedRowHandle < 0)
                    return;
                ProcessClickViewTreatment(sender, gridViewTreatment.FocusedRowHandle);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewTreatment_RowCellStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs e)
        {
            try
            {
                if (e.RowHandle < 0)
                    return;
                int rowHandleSelected = gridViewTreatment.GetVisibleRowHandle(e.RowHandle);
                var data = (V_HIS_TREATMENT_FEE)gridViewTreatment.GetRow(rowHandleSelected);
                if (data != null && data.IS_LOCK_HEIN == 1)
                {
                    e.Appearance.ForeColor = Color.Red;
                }
                else if (data != null && data.MEDI_RECORD_ID != null && data.MEDI_RECORD_ID > 0)
                {
                    e.Appearance.ForeColor = Color.DodgerBlue;


                }
                if (e.Column.FieldName == "DISPLAY_COLOR_STR")
                {
                    if (data.TDL_PATIENT_CLASSIFY_ID != null)
                    {
                        List<int> parentColor = new List<int>();
                        string color = BackendDataWorker.Get<HIS_PATIENT_CLASSIFY>().Where(o => o.ID == data.TDL_PATIENT_CLASSIFY_ID).FirstOrDefault().DISPLAY_COLOR;
                        if (!string.IsNullOrEmpty(color))
                        {
                            parentColor = GetGrbColor(color);
                            e.Appearance.BackColor = Color.FromArgb(parentColor[0], parentColor[1], parentColor[2]);
                        }
                    }

                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private List<int> GetGrbColor(string rgb)
        {
            List<int> rs = new List<int>();
            try
            {
                rs = rgb.Split(',').Select(o => Convert.ToInt32(o)).ToList();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return rs;
        }

        private void gridViewTreatment_PopupMenuShowing(object sender, DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventArgs e)
        {
            try
            {
                GridHitInfo hi = e.HitInfo;
                if (hi.InRowCell)
                {
                    int rowHandleSelected = gridViewTreatment.GetVisibleRowHandle(hi.RowHandle);
                    this.currentTreatment = (V_HIS_TREATMENT_FEE)gridViewTreatment.GetRow(rowHandleSelected);
                    SetEnableButton(null);

                    if (this.barManager == null)
                        this.barManager = new DevExpress.XtraBars.BarManager();
                    this.barManager.Images = imageCollection2;
                    this.barManager.Form = this;
                    this.popupMenuProcessor = new PopupMenuProcessor(this.barManager, Transaction_MouseRightClick, this.currentTreatment, this.PopupItemStatusAdo);
                    this.popupMenuProcessor.InitMenu(this.allowUnlock, this.loginname);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessClickViewTreatment(object sender, int rowHandle)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("ProcessClickViewTreatment. 1");
                WaitingManager.Show();

                //Phuongdt sửa
                //Issue 5186
                //Lỗi do khi sort lại danh sách, index (rowhandle) vẫn giữ nguyên trong khi dữ liệu lấy ra từ datasource đã bị thay đổi sort lại
                //--> dẫn đến việc lấy dữ liệu của dòng đang select bị sai (trừ trường hợp grid đó disable tất cả các sort của các column đi sẽ ko bị phát sinh lỗi này)
                //Thay thế tất cả các đoạn xử lý lấy dữ liệu dòng theo cách mới ở dưới để khắc phục lỗi
                int rowHandleSelected = gridViewTreatment.GetVisibleRowHandle(rowHandle);

                this.currentTreatment = (V_HIS_TREATMENT_FEE)gridViewTreatment.GetRow(rowHandleSelected);
                FillDataToControlBySelectTreatment(false, true);
                SetEnableButton(null);
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Debug("ProcessClickViewTreatment. 2");
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void EnableButttonFromFeil()
        {
            try
            {

                Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData("this.currentTreatment____", this.currentTreatment));
                if (this.currentTreatment != null && this.currentTreatment.DIS_DEPOSIT_OPTION != null)

                    if (this.currentTreatment.DIS_DEPOSIT_OPTION == 1)
                    {
                        btnDeposit.Enabled = false;
                    }
                    else if (this.currentTreatment.DIS_DEPOSIT_OPTION == 2)
                    {
                        btnDeposit.Enabled = this.currentTreatment.IS_PAUSE != 1;
                    }

                if (this.currentTreatment != null && this.currentTreatment.DIS_SERVICE_DEPOSIT_OPTION != null)
                {
                    if (this.currentTreatment.DIS_SERVICE_DEPOSIT_OPTION == 1)
                    {
                        btnDepositService.Enabled = false;
                    }
                    else if (this.currentTreatment.DIS_SERVICE_DEPOSIT_OPTION == 2)
                    {
                        btnDepositService.Enabled = this.currentTreatment.IS_PAUSE != 1;
                    }
                }
                if (this.currentTreatment != null && this.currentTreatment.IS_DIS_SERVICE_REPAY == 1)
                {
                    btnRepayService.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToTreatmentInfo(V_HIS_TREATMENT_FEE treatment)
        {
            try
            {
                if (treatment != null)
                {
                    lblInDate.Text = Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(treatment.IN_TIME);
                    lblInHospital.Text = Inventec.Common.DateTime.Convert.TimeNumberToDateString(treatment.CLINICAL_IN_TIME ?? 0);
                    lblOutDate.Text = Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(treatment.OUT_TIME ?? 0);
                    lblName.Text = treatment.TDL_PATIENT_NAME;
                    lblNote.Text = treatment.TDL_PATIENT_NOTE;
                    if (treatment.TDL_PATIENT_IS_HAS_NOT_DAY_DOB == 1)
                    {
                        lblDob.Text = string.Format("{0} ({1})", treatment.TDL_PATIENT_DOB.ToString().Substring(0, 4), MPS.AgeUtil.CalculateFullAge(treatment.TDL_PATIENT_DOB));
                    }
                    else
                    {
                        lblDob.Text = string.Format("{0} ({1})", Inventec.Common.DateTime.Convert.TimeNumberToDateString(treatment.TDL_PATIENT_DOB), MPS.AgeUtil.CalculateFullAge(treatment.TDL_PATIENT_DOB));
                    }
                    lblSex.Text = treatment.TDL_PATIENT_GENDER_NAME;
                    lblAddress.Text = treatment.TDL_PATIENT_ADDRESS;
                    if (treatment.TDL_PATIENT_TYPE_ID == Config.HisConfigCFG.PatientTypeId__BHYT)
                    {
                        lblTreatDay.Text = HIS.Common.Treatment.Calculation.DayOfTreatment(treatment.IN_TIME, treatment.OUT_TIME, treatment.TREATMENT_END_TYPE_ID, treatment.TREATMENT_RESULT_ID,

                            HIS.Common.Treatment.PatientTypeEnum.TYPE.BHYT) + "";
                    }
                    else if (treatment.TDL_PATIENT_TYPE_ID == Config.HisConfigCFG.PatientTypeId__VP)
                    {
                        lblTreatDay.Text = HIS.Common.Treatment.Calculation.DayOfTreatment(treatment.IN_TIME, treatment.OUT_TIME, treatment.TREATMENT_END_TYPE_ID, treatment.TREATMENT_RESULT_ID,
                            HIS.Common.Treatment.PatientTypeEnum.TYPE.THU_PHI) + "";
                    }
                    else
                    {
                        lblTreatDay.Text = "";
                    }

                    lblIcd.Text = !string.IsNullOrEmpty(treatment.ICD_CODE) ? treatment.ICD_CODE + " - " + treatment.ICD_NAME : "";
                    lblSubIcd.Text = !string.IsNullOrEmpty(treatment.ICD_SUB_CODE) ? treatment.ICD_SUB_CODE + " - " + treatment.ICD_TEXT : treatment.ICD_TEXT;
                    if (treatment.TREATMENT_END_TYPE_ID.HasValue)
                    {
                        var treatmentEndType = BackendDataWorker.Get<HIS_TREATMENT_END_TYPE>().FirstOrDefault(o => o.ID == treatment.TREATMENT_END_TYPE_ID.Value);
                        lblTreatmentEndType.Text = treatmentEndType != null ? treatmentEndType.TREATMENT_END_TYPE_NAME : "";
                    }
                    else
                    {
                        lblTreatmentEndType.Text = "";
                    }

                    if (treatment.TREATMENT_RESULT_ID.HasValue)
                    {
                        var treatmentResult = BackendDataWorker.Get<HIS_TREATMENT_RESULT>().FirstOrDefault(o => o.ID == treatment.TREATMENT_RESULT_ID.Value);
                        lblTreatmentResult.Text = treatmentResult != null ? treatmentResult.TREATMENT_RESULT_NAME : "";
                    }
                    else
                    {
                        lblTreatmentResult.Text = "";
                    }
                }
                else
                {
                    lblInDate.Text = "";
                    lblInHospital.Text = "";
                    lblOutDate.Text = "";
                    lblTreatDay.Text = "";
                    lblIcd.Text = "";
                    lblSubIcd.Text = "";
                    lblTreatmentEndType.Text = "";
                    lblTreatmentResult.Text = "";
                }

                Inventec.Common.Logging.LogSystem.Debug("step 8");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void RefreshCurrentTreatment()
        {
            try
            {
                HisTreatmentFeeViewFilter treatFilter = new HisTreatmentFeeViewFilter();
                treatFilter.ID = this.currentTreatment.ID;
                var treat = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_TREATMENT_FEE>>(HisRequestUriStore.HIS_TREATMENT_GETFEEVIEW, ApiConsumers.MosConsumer, treatFilter, null).FirstOrDefault();
                if (treat != null)
                {
                    this.listTreatment[this.listTreatment.IndexOf(this.currentTreatment)] = treat;
                    this.currentTreatment = treat;
                    this.gridControlTreatment.RefreshDataSource();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private async Task GetAsysnSereServBill()
        {
            try
            {
                HisSereServBillFilter ssBillFilter = new HisSereServBillFilter();
                ssBillFilter.TDL_TREATMENT_ID = this.currentTreatment.ID;
                Inventec.Common.Logging.LogSystem.Debug("step 3");
                var listSereServBill = await new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).GetAsync<List<HIS_SERE_SERV_BILL>>("api/HisSereServBill/Get", ApiConsumers.MosConsumer, ssBillFilter, null);
                Inventec.Common.Logging.LogSystem.Debug("step 4");
                if (listSereServBill != null)
                {
                    Inventec.Common.Logging.LogSystem.Debug("step 5");
                    foreach (var item in listSereServBill)
                    {
                        if (item.IS_CANCEL == 1 || item.TDL_TREATMENT_ID == null)
                            continue;
                        if (!dicSereServBill.ContainsKey(item.SERE_SERV_ID))
                            dicSereServBill[item.SERE_SERV_ID] = new List<HIS_SERE_SERV_BILL>();
                        dicSereServBill[item.SERE_SERV_ID].Add(item);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private async Task FillDataToTreatmentInfoAsyn()
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("step 6");
                HisSereServView5Filter ssFilter = new HisSereServView5Filter();
                ssFilter.ORDER_DIRECTION = "TDL_INTRUCTION_TIME";
                ssFilter.ORDER_FIELD = "ACS";
                ssFilter.TDL_TREATMENT_ID = this.currentTreatment.ID;
                this.listSereServ = await new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).GetAsync<List<V_HIS_SERE_SERV_5>>("api/HisSereServ/GetView5", ApiConsumers.MosConsumer, ssFilter, null);
                Inventec.Common.Logging.LogSystem.Debug("step 7");
                this.FillDataToTreatmentInfo(this.currentTreatment);
                this.ssTreeProcessor.Reload(this.ucSereServTree, this.listSereServ);

                RefreshDisplaySereServTree();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void RefreshDisplaySereServTree()
        {
            try
            {
                System.Threading.Thread thread = new System.Threading.Thread(new System.Threading.ThreadStart(DisplaySereServTree));
                thread.Priority = System.Threading.ThreadPriority.Normal;
                thread.IsBackground = true;
                thread.SetApartmentState(System.Threading.ApartmentState.STA);
                try
                {
                    thread.Start();
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Error(ex);
                    thread.Abort();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void DisplaySereServTree()
        {
            try
            {
                List<V_HIS_SERE_SERV_DEPOSIT> listSereServDeposit = GetSereServDepositAsync();
                List<V_HIS_SESE_DEPO_REPAY> listSeseDepoRepay = GetSeseDepoRepayAsync();
                listSereServID_DaTamUngDichVu = new List<long>();

                foreach (var item in this.listSereServ)
                {
                    bool isDaTamUngDichVu = false;
                    if (listSereServDeposit != null && listSereServDeposit.Exists(o => o.SERE_SERV_ID == item.ID && o.IS_CANCEL != 1))
                    {
                        isDaTamUngDichVu = true;
                        var listSereServDeposit_Valid = listSereServDeposit.Where(o => o.SERE_SERV_ID == item.ID && o.IS_CANCEL != 1).ToList();

                        foreach (var sereServDeposi in listSereServDeposit_Valid)
                        {
                            if (listSeseDepoRepay != null && listSeseDepoRepay.Exists(o => o.SERE_SERV_ID == item.ID && o.SERE_SERV_DEPOSIT_ID == sereServDeposi.ID))
                            {
                                isDaTamUngDichVu = false;
                            }
                        }
                    }
                    if (isDaTamUngDichVu)
                    {
                        listSereServID_DaTamUngDichVu.Add(item.ID);
                    }
                }

                Invoke(new Action(() =>
                {
                    this.ssTreeProcessor.Reload(this.ucSereServTree, this.listSereServ);
                }));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private List<V_HIS_SERE_SERV_DEPOSIT> GetSereServDepositAsync()
        {
            List<V_HIS_SERE_SERV_DEPOSIT> result = null;
            try
            {
                if (this.currentTreatment == null)
                    return null;
                HisSereServDepositViewFilter filter = new HisSereServDepositViewFilter();
                filter.TDL_TREATMENT_ID = this.currentTreatment.ID;
                result = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_SERE_SERV_DEPOSIT>>("api/HisSereServDeposit/GetView", ApiConsumers.MosConsumer, filter, new CommonParam());
            }
            catch (Exception ex)
            {
                result = null;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private List<V_HIS_SESE_DEPO_REPAY> GetSeseDepoRepayAsync()
        {
            List<V_HIS_SESE_DEPO_REPAY> result = null;
            try
            {
                if (this.currentTreatment == null)
                    return null;
                HisSeseDepoRepayViewFilter filter = new HisSeseDepoRepayViewFilter();
                filter.TDL_TREATMENT_ID = this.currentTreatment.ID;
                result = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_SESE_DEPO_REPAY>>("api/HisSeseDepoRepay/GetView", ApiConsumers.MosConsumer, filter, new CommonParam());
            }
            catch (Exception ex)
            {
                result = null;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private async Task GetPatientTypeAlter()
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("step 9");
                List<PatientTypeAlterAdo> PatientTypeAlterAdos = new List<PatientTypeAlterAdo>();
                HisPatientTypeAlterViewFilter patientTypeAlterFilter = new HisPatientTypeAlterViewFilter();
                patientTypeAlterFilter.TREATMENT_ID = this.currentTreatment.ID;
                patientTypeAlterFilter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                patientTypeAlterFilter.ORDER_FIELD = "CREATE_TIME";
                patientTypeAlterFilter.ORDER_DIRECTION = "DESC";
                var patientTypeAlters = await new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).GetAsync<List<V_HIS_PATIENT_TYPE_ALTER>>("api/HisPatientTypeAlter/GetView", ApiConsumer.ApiConsumers.MosConsumer, patientTypeAlterFilter, null);
                Inventec.Common.Logging.LogSystem.Debug("step 10");
                var rightRouteTypes = MOS.LibraryHein.Bhyt.HeinRightRouteType.HeinRightRouteTypeStore.Get();
                if (patientTypeAlters != null && patientTypeAlters.Count > 0)
                {
                    this.lastPatientType = patientTypeAlters.OrderByDescending(o => o.LOG_TIME)
                        .ThenByDescending(o => o.ID)
                        .FirstOrDefault();

                    Inventec.Common.Logging.LogSystem.Debug("step 11");
                    var Group = patientTypeAlters.GroupBy(g => new
                    {
                        g.PATIENT_TYPE_ID,
                        g.HEIN_MEDI_ORG_CODE,
                        g.HEIN_CARD_NUMBER,
                        g.HEIN_CARD_FROM_TIME,
                        g.HEIN_CARD_TO_TIME,
                        g.RIGHT_ROUTE_TYPE_CODE,
                        g.RIGHT_ROUTE_CODE
                    }).ToList();

                    foreach (var group in Group)
                    {
                        var distintPatientType = group.ToList<V_HIS_PATIENT_TYPE_ALTER>().FirstOrDefault();

                        PatientTypeAlterAdo PatientType = new PatientTypeAlterAdo();
                        PatientType.TITLE = BackendDataWorker.Get<HIS_PATIENT_TYPE>().FirstOrDefault(o => o.ID == distintPatientType.PATIENT_TYPE_ID).PATIENT_TYPE_NAME;
                        PatientType.VALUE = HeinCardHelper.SetHeinCardNumberDisplayByNumber(distintPatientType.HEIN_CARD_NUMBER);
                        PatientTypeAlterAdos.Add(PatientType);
                        if (distintPatientType.PATIENT_TYPE_ID == HIS.Desktop.Plugins.Transaction.Config.HisConfigCFG.PatientTypeId__BHYT)
                        {
                            PatientTypeAlterAdo hanThe = new PatientTypeAlterAdo();
                            hanThe.TITLE = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_TRANSACTION__TREATMENT_INFO__LAYOUT_HEIN_CARD_TIME", ResourceLangManager.LanguageUCTransaction, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                            hanThe.VALUE = Inventec.Common.DateTime.Convert.TimeNumberToDateString(distintPatientType.HEIN_CARD_FROM_TIME ?? 0) +
                                (distintPatientType.HEIN_CARD_TO_TIME != null ? " - " + Inventec.Common.DateTime.Convert.TimeNumberToDateString(distintPatientType.HEIN_CARD_TO_TIME ?? 0) : "");
                            PatientTypeAlterAdos.Add(hanThe);

                            var rightRoute = MOS.LibraryHein.Bhyt.HeinRightRoute.HeinRightRouteStore.GetByCode(distintPatientType.RIGHT_ROUTE_CODE);
                            var rightRouteType = MOS.LibraryHein.Bhyt.HeinRightRouteType.HeinRightRouteTypeStore.GetByCode(distintPatientType.RIGHT_ROUTE_TYPE_CODE);

                            PatientTypeAlterAdo loaiDungTuyen = new PatientTypeAlterAdo();
                            string type = !string.IsNullOrEmpty(currentTreatment.TDL_HEIN_CARD_NUMBER) ? " (" + GetDefaultHeinRatioForView(currentTreatment.TDL_HEIN_CARD_NUMBER, BackendDataWorker.Get<HIS_TREATMENT_TYPE>().FirstOrDefault(o => o.TREATMENT_TYPE_CODE == currentTreatment.TDL_TREATMENT_TYPE_CODE).HEIN_TREATMENT_TYPE_CODE, distintPatientType.LEVEL_CODE, distintPatientType.RIGHT_ROUTE_CODE) + ")" : "";
                            loaiDungTuyen.TITLE = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_TRANSACTION__TREATMENT_INFO__LAYOUT_RIGH_ROUTE_TYPE", ResourceLangManager.LanguageUCTransaction, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                            loaiDungTuyen.VALUE = (rightRoute != null ? rightRoute.HeinRightRouteName + type : "") + (rightRouteType != null && !string.IsNullOrEmpty(rightRouteType.HeinRightRouteTypeName) ? " - " + rightRouteType.HeinRightRouteTypeName : "");
                            PatientTypeAlterAdos.Add(loaiDungTuyen);


                            PatientTypeAlterAdo noiDKKCBBD = new PatientTypeAlterAdo();

                            noiDKKCBBD.TITLE = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_TRANSACTION__TREATMENT_INFO__LAYOUT_NOI_DKKCBBD", ResourceLangManager.LanguageUCTransaction, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                            noiDKKCBBD.VALUE = !string.IsNullOrEmpty(distintPatientType.HEIN_MEDI_ORG_CODE) ? distintPatientType.HEIN_MEDI_ORG_CODE + " - " + distintPatientType.HEIN_MEDI_ORG_NAME : "";
                            PatientTypeAlterAdos.Add(noiDKKCBBD);
                        }
                        else if (distintPatientType.PATIENT_TYPE_ID == HIS.Desktop.Plugins.Transaction.Config.HisConfigCFG.PatientTypeId__KSK)
                        {
                            PatientTypeAlterAdo MaDoiTuong = new PatientTypeAlterAdo();
                            MaDoiTuong.TITLE = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_TRANSACTION__TREATMENT_INFO__LAYOUT_KSK_CONTRAC_CODE", ResourceLangManager.LanguageUCTransaction, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                            MaDoiTuong.VALUE = distintPatientType.KSK_CONTRACT_CODE;
                            PatientTypeAlterAdos.Add(MaDoiTuong);

                            PatientTypeAlterAdo TenCongTy = new PatientTypeAlterAdo();
                            TenCongTy.TITLE = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_TRANSACTION__TREATMENT_INFO__LAYOUT_KSK_WORK_PLACE_NAME", ResourceLangManager.LanguageUCTransaction, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                            TenCongTy.VALUE = distintPatientType.WORK_PLACE_NAME;
                            PatientTypeAlterAdos.Add(TenCongTy);

                            PatientTypeAlterAdo NgayHieuLucHopDong = new PatientTypeAlterAdo();
                            NgayHieuLucHopDong.TITLE = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_TRANSACTION__TREATMENT_INFO__LAYOUT_KSK_EFFECT_EXPIRY_DATE", ResourceLangManager.LanguageUCTransaction, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                            NgayHieuLucHopDong.VALUE = Inventec.Common.DateTime.Convert.TimeNumberToDateString(distintPatientType.EFFECT_DATE ?? 0) + " - " + Inventec.Common.DateTime.Convert.TimeNumberToDateString(distintPatientType.EXPIRY_DATE ?? 0);
                            PatientTypeAlterAdos.Add(NgayHieuLucHopDong);

                            PatientTypeAlterAdo PaymentRatio = new PatientTypeAlterAdo();
                            PaymentRatio.TITLE = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_TRANSACTION__TREATMENT_INFO__LAYOUT_KSK_PAYMENT_RATIO", ResourceLangManager.LanguageUCTransaction, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                            PaymentRatio.VALUE = distintPatientType.PAYMENT_RATIO != null ? (distintPatientType.PAYMENT_RATIO * 100) + "(%)" : "(%)";
                            PatientTypeAlterAdos.Add(PaymentRatio);

                        }

                        if (Group.Count > 1 && Group.IndexOf(group) < Group.Count - 1)
                        {
                            PatientTypeAlterAdo lineSeparate = new PatientTypeAlterAdo();
                            lineSeparate.TITLE = "-------   -------";
                            lineSeparate.VALUE = "-------   -------   -------";
                            PatientTypeAlterAdos.Add(lineSeparate);
                        }
                    }
                }

                gridControlPatientTypeInfo.BeginUpdate();
                gridControlPatientTypeInfo.DataSource = null;
                gridControlPatientTypeInfo.DataSource = PatientTypeAlterAdos;
                gridControlPatientTypeInfo.EndUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public static string GetDefaultHeinRatioForView(string heinCardNumber, string treatmentTypeCode, string levelCode, string rightRouteCode)
        {
            string result = "";
            try
            {
                Inventec.Common.Logging.LogSystem.Error(String.Format("treatmentTypeCode {0}", new MOS.LibraryHein.Bhyt.BhytHeinProcessor().GetDefaultHeinRatio(treatmentTypeCode, heinCardNumber, levelCode, rightRouteCode) ?? 0));
                result = ((int)((new MOS.LibraryHein.Bhyt.BhytHeinProcessor().GetDefaultHeinRatio(treatmentTypeCode, heinCardNumber, levelCode, rightRouteCode) ?? 0) * 100)) + "%";
                Inventec.Common.Logging.LogSystem.Error(String.Format("treatmentTypeCode {0} , heinCardNumber {1}, levelCode {2}, rightRouteCode {3} ", treatmentTypeCode, heinCardNumber, levelCode, rightRouteCode));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private async Task FillDataToControlBySelectTreatment(bool callBack, bool isReloadTreatmentExtInfo = false)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("step 1");
                this.listSereServ = new List<V_HIS_SERE_SERV_5>();
                this.dicSereServBill = new Dictionary<long, List<HIS_SERE_SERV_BILL>>();
                UC.TotalPriceInfo.ADO.TotalPriceADO adoPrice = new UC.TotalPriceInfo.ADO.TotalPriceADO();
                if (this.currentTreatment != null)
                {
                    if (callBack)
                    {
                        RefreshCurrentTreatment();
                    }

                    adoPrice.Discount = Inventec.Common.Number.Convert.NumberToString(this.currentTreatment.TOTAL_BILL_EXEMPTION ?? 0, ConfigApplications.NumberSeperator);
                    adoPrice.TotalDiscount = Inventec.Common.Number.Convert.NumberToString(this.currentTreatment.TOTAL_DISCOUNT ?? 0, ConfigApplications.NumberSeperator);
                    if (this.currentTreatment.TOTAL_PATIENT_PRICE.HasValue && this.currentTreatment.TOTAL_PATIENT_PRICE.Value > 0)
                    {
                        decimal discountRatio = 0;
                        if (this.currentTreatment.TOTAL_DISCOUNT.HasValue)
                        {
                            discountRatio = (this.currentTreatment.TOTAL_DISCOUNT.Value) / this.currentTreatment.TOTAL_PATIENT_PRICE.Value;
                        }
                    }
                    adoPrice.TotalBillFundPrice = Inventec.Common.Number.Convert.NumberToString(this.currentTreatment.TOTAL_BILL_FUND ?? 0, ConfigApplications.NumberSeperator);
                    adoPrice.TotalBillPrice = Inventec.Common.Number.Convert.NumberToString(this.currentTreatment.TOTAL_BILL_AMOUNT ?? 0, ConfigApplications.NumberSeperator);
                    adoPrice.TotalBillTransferPrice = Inventec.Common.Number.Convert.NumberToString(this.currentTreatment.TOTAL_BILL_TRANSFER_AMOUNT ?? 0, ConfigApplications.NumberSeperator);
                    //adoPrice.TotalDepositPrice = Inventec.Common.Number.Convert.NumberToString(this.currentTreatment.TOTAL_DEPOSIT_AMOUNT ?? 0, ConfigApplications.NumberSeperator);
                    adoPrice.TotalHeinPrice = Inventec.Common.Number.Convert.NumberToString(this.currentTreatment.TOTAL_HEIN_PRICE ?? 0, ConfigApplications.NumberSeperator);
                    adoPrice.TotalPatientPrice = Inventec.Common.Number.Convert.NumberToString(this.currentTreatment.TOTAL_PATIENT_PRICE ?? 0, ConfigApplications.NumberSeperator);
                    adoPrice.TotalPrice = Inventec.Common.Number.Convert.NumberToString(this.currentTreatment.TOTAL_PRICE ?? 0, ConfigApplications.NumberSeperator);
                    adoPrice.TotalRepayPrice = Inventec.Common.Number.Convert.NumberToString(this.currentTreatment.TOTAL_REPAY_AMOUNT ?? 0, ConfigApplications.NumberSeperator);
                    adoPrice.VirTotalPriceNoExpend = Inventec.Common.Number.Convert.NumberToString(this.currentTreatment.TOTAL_PRICE_EXPEND ?? 0, ConfigApplications.NumberSeperator);
                    adoPrice.TotalDebtAmount = Inventec.Common.Number.Convert.NumberToString(this.currentTreatment.TOTAL_DEBT_AMOUNT ?? 0, ConfigApplications.NumberSeperator);

                    adoPrice.TotalOtherCopaidPrice = Inventec.Common.Number.Convert.NumberToString(this.currentTreatment.TOTAL_OTHER_COPAID_PRICE ?? 0, ConfigApplications.NumberSeperator);

                    decimal totalReceive = ((this.currentTreatment.TOTAL_DEPOSIT_AMOUNT ?? 0) + (this.currentTreatment.TOTAL_BILL_AMOUNT ?? 0) - (this.currentTreatment.TOTAL_BILL_TRANSFER_AMOUNT ?? 0) - (this.currentTreatment.TOTAL_BILL_FUND ?? 0) - (this.currentTreatment.TOTAL_REPAY_AMOUNT ?? 0)) - (this.currentTreatment.TOTAL_BILL_EXEMPTION ?? 0) + (this.currentTreatment.LOCKING_AMOUNT ?? 0);

                    decimal totalReceiveMore = (this.currentTreatment.TOTAL_PATIENT_PRICE ?? 0) - totalReceive - (this.currentTreatment.TOTAL_BILL_FUND ?? 0);
                    adoPrice.TotalReceiveMorePrice = Inventec.Common.Number.Convert.NumberToString(totalReceiveMore - (this.currentTreatment.TOTAL_BILL_EXEMPTION ?? 0), ConfigApplications.NumberSeperator);
                    adoPrice.TotalReceivePrice = Inventec.Common.Number.Convert.NumberToString(totalReceive, ConfigApplications.NumberSeperator);
                    adoPrice.TotalOtherBillAmount = Inventec.Common.Number.Convert.NumberToString(this.currentTreatment.TOTAL_BILL_OTHER_AMOUNT ?? 0, ConfigApplications.NumberSeperator);
                    adoPrice.TotalOtherSourcePrice = Inventec.Common.Number.Convert.NumberToString(this.currentTreatment.TOTAL_OTHER_SOURCE_PRICE ?? 0, ConfigApplications.NumberSeperator);

                    adoPrice.TotalServiceDepositPrice = Inventec.Common.Number.Convert.NumberToString(this.currentTreatment.TOTAL_SERVICE_DEPOSIT_AMOUNT ?? 0, ConfigApplications.NumberSeperator);
                    adoPrice.TotalDepositPrice = Inventec.Common.Number.Convert.NumberToString((this.currentTreatment.TOTAL_DEPOSIT_AMOUNT ?? 0) - (this.currentTreatment.TOTAL_SERVICE_DEPOSIT_AMOUNT ?? 0), ConfigApplications.NumberSeperator);
                    adoPrice.LockingAmount = Inventec.Common.Number.Convert.NumberToString(this.currentTreatment.LOCKING_AMOUNT ?? 0, ConfigApplications.NumberSeperator);

                    Inventec.Common.Logging.LogSystem.Debug("step 2");
                    await GetAsysnSereServBill();

                    FillDataToTreatmentInfoAsyn();

                    if (isReloadTreatmentExtInfo)
                    {
                        GetPatientTypeAlter();
                        GetCurrentTreatmentRoom();
                        GetNextDepartment();
                    }

                }
                else
                {
                    adoPrice.TotalBillFundPrice = Inventec.Common.Number.Convert.NumberToString(0);
                    adoPrice.TotalBillPrice = Inventec.Common.Number.Convert.NumberToString(0);
                    adoPrice.TotalBillTransferPrice = Inventec.Common.Number.Convert.NumberToString(0);
                    adoPrice.TotalDepositPrice = Inventec.Common.Number.Convert.NumberToString(0);
                    adoPrice.TotalServiceDepositPrice = Inventec.Common.Number.Convert.NumberToString(0);
                    adoPrice.TotalHeinPrice = Inventec.Common.Number.Convert.NumberToString(0);
                    adoPrice.TotalPatientPrice = Inventec.Common.Number.Convert.NumberToString(0);
                    adoPrice.TotalPrice = Inventec.Common.Number.Convert.NumberToString(0);
                    adoPrice.TotalRepayPrice = Inventec.Common.Number.Convert.NumberToString(0);
                    adoPrice.TotalReceiveMorePrice = Inventec.Common.Number.Convert.NumberToString(0);
                    adoPrice.TotalReceivePrice = Inventec.Common.Number.Convert.NumberToString(0);
                    adoPrice.TotalOtherBillAmount = Inventec.Common.Number.Convert.NumberToString(0);
                    adoPrice.TotalDiscount = Inventec.Common.Number.Convert.NumberToString(0);
                    adoPrice.VirTotalPriceNoExpend = Inventec.Common.Number.Convert.NumberToString(0);
                    adoPrice.TotalDebtAmount = Inventec.Common.Number.Convert.NumberToString(0);
                    adoPrice.TotalOtherSourcePrice = Inventec.Common.Number.Convert.NumberToString(0);

                    adoPrice.TotalOtherCopaidPrice = Inventec.Common.Number.Convert.NumberToString(0);
                    adoPrice.LockingAmount = Inventec.Common.Number.Convert.NumberToString(0);
                    FillDataToTreatmentInfo(null);
                    this.ssTreeProcessor.Reload(this.ucSereServTree, this.listSereServ);
                }
                Inventec.Common.Logging.LogSystem.Debug("step 12");
                totalPriceProcessor.SetValue(ucTotalPriceInfo, adoPrice);
                if (this.currentTreatment != null && this.currentTreatment.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)
                {
                    btnLock.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_TRANSACTION__BTN_LOCK_FEE", Base.ResourceLangManager.LanguageUCTransaction, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                else
                {
                    btnLock.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_TRANSACTION__BTN_UNLOCK_FEE", Base.ResourceLangManager.LanguageUCTransaction, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }

                if (this.currentTreatment != null && this.currentTreatment.IS_TEMPORARY_LOCK == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)
                {
                    btnTemporaryLock.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_TRANSACTION__BTN_TEMPORARY_UN_LOCK", Base.ResourceLangManager.LanguageUCTransaction, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                else
                {
                    btnTemporaryLock.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_TRANSACTION__BTN_TEMPORARY_LOCK", Base.ResourceLangManager.LanguageUCTransaction, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }

                if (this.currentTreatment != null && (this.currentTreatment.DIS_SERVICE_DEPOSIT_OPTION != 1 || (this.currentTreatment.IN_TREATMENT_TYPE_ID != null && this.currentTreatment.DIS_DEPOSIT_OPTION_REQUEST != 1)))
                {
                    bool isLockHein = (this.currentTreatment.IS_LOCK_HEIN == 1);
                    bool isActive = (this.currentTreatment.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE);
                    bool valid = isLockHein ? false : isActive;
                    btnDeposit.Enabled = true && valid;
                }
                else
                {
                    btnDeposit.Enabled = false;
                }

                EnableButttonFromFeil();
                Inventec.Common.Logging.LogSystem.Debug("step 13");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private async Task GetCurrentTreatmentRoom()
        {
            try
            {
                LblCurrentRoom.Text = "";

                if (this.currentTreatment != null)
                {
                    if (this.currentTreatment.TDL_TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__KHAM)
                    {
                        var endRoom = this.currentTreatment.END_ROOM_ID.HasValue ? BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == this.currentTreatment.END_ROOM_ID) : null;
                        if (endRoom != null)
                        {
                            LblCurrentRoom.Text = endRoom.ROOM_NAME;
                        }
                        else
                        {
                            HisServiceReqViewFilter seviceReqFilter = new HisServiceReqViewFilter();
                            seviceReqFilter.TREATMENT_ID = this.currentTreatment.ID;
                            seviceReqFilter.SERVICE_REQ_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__KH;
                            var seviceReqKham = await new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).GetAsync<List<V_HIS_SERVICE_REQ>>("api/HisServiceReq/GetView", ApiConsumers.MosConsumer, seviceReqFilter, null);

                            if (seviceReqKham != null && seviceReqKham.Count > 0)
                            {
                                var kham = seviceReqKham.OrderByDescending(o => o.IS_MAIN_EXAM ?? 0).ThenByDescending(o => o.INTRUCTION_TIME).First();
                                if (kham != null)
                                    LblCurrentRoom.Text = kham.EXECUTE_ROOM_NAME;
                            }
                        }
                    }
                    else
                    {
                        string lastRoom = "";
                        string lastBedRoom = "";
                        HisDepartmentTranLastFilter filter = new HisDepartmentTranLastFilter();
                        filter.TREATMENT_ID = this.currentTreatment.ID;
                        V_HIS_DEPARTMENT_TRAN tran = await new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).GetAsync<V_HIS_DEPARTMENT_TRAN>("api/HisDepartmentTran/GetLastByTreatmentId", ApiConsumer.ApiConsumers.MosConsumer, filter, null);

                        if (tran != null)
                            lastRoom = tran.DEPARTMENT_NAME;


                        var endRoom = this.currentTreatment.END_ROOM_ID.HasValue ? BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == this.currentTreatment.END_ROOM_ID) : null;
                        if (endRoom != null)
                        {
                            lastBedRoom = endRoom.ROOM_NAME;
                        }
                        else
                        {
                            HisTreatmentBedRoomViewFilter brFilter = new HisTreatmentBedRoomViewFilter();
                            brFilter.TREATMENT_ID = this.currentTreatment.ID;
                            var treatmentBedRooms = await new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).GetAsync<List<V_HIS_TREATMENT_BED_ROOM>>("api/HisTreatmentBedRoom/GetView", ApiConsumers.MosConsumer, brFilter, null);
                            if (treatmentBedRooms != null)
                                lastBedRoom = treatmentBedRooms.Where(o => o.ADD_TIME != null && o.ADD_TIME > 0).ToList().OrderByDescending(p => p.ADD_TIME).FirstOrDefault().BED_ROOM_NAME;
                        }
                        LblCurrentRoom.Text = lastRoom + " / " + lastBedRoom;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private string AddStringByConfig(int num)
        {
            string str = "";
            try
            {
                if (num > 0)
                {
                    for (int i = 1; i <= num; i++)
                    {
                        str += "0";
                    }
                }
                else
                {
                    return str = "";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return str = "";
            }
            return str;
        }

        private void SetEnableButton(bool? enable)
        {
            try
            {
                if (this.currentTreatment == null || enable.HasValue)
                {
                    bool valid = (this.currentTreatment == null) ? false : enable.Value;
                    this.btnBill.Enabled = valid;
                    if (HisConfigCFG.DirectlyBillingOption == "2")
                    {
                        this.btnBillNotKC.Enabled = valid && HisConfigCFG.IsketChuyenCFG != null && HisConfigCFG.IsketChuyenCFG.Equals("4") && !this.currentTreatment.IS_PAUSE.HasValue && this.currentTreatment.TDL_TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__KHAM;
                    }
                    else
                    {
                        this.btnBillNotKC.Enabled = valid && HisConfigCFG.IsketChuyenCFG != null && HisConfigCFG.IsketChuyenCFG.Equals("4");
                    }


                    this.btnTemporaryLock.Enabled = valid;
                    this.PopupItemStatusAdo.TemporaryLockStt = valid;
                    this.PopupItemStatusAdo.TransactionAllStt = valid;
                    this.btnDeposit.Enabled = valid;
                    this.btnMienGiam.Enabled = valid;
                    this.btnDepositService.Enabled = valid;
                    this.btnInvoice.Enabled = valid;
                    this.btnLock.Enabled = valid;
                    this.PopupItemStatusAdo.LockStt = valid;
                    this.btnRepay.Enabled = valid;
                    this.btnRepayService.Enabled = valid;
                    this.btnTranfer.Enabled = valid;
                    this.btnLockHistory.Enabled = false;
                    this.btnBordereau.Enabled = valid;
                    this.btnTranList.Enabled = valid;
                    this.btnOtherPayment.Enabled = valid;
                }
                else
                {

                    bool isPause = (this.currentTreatment.IS_PAUSE == 1);
                    bool isActive = (this.currentTreatment.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE);
                    bool isTemporaryLock = this.currentTreatment.IS_TEMPORARY_LOCK == 1;
                    bool isLockHein = (this.currentTreatment.IS_LOCK_HEIN == 1);
                    this.btnLock.Enabled = (!isPause || isLockHein) ? false : true;
                    long keyconfig = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<int>("MOS.HIS_TREATMENT.LOCK_FEE_AFTER_APPROVE_MEDI_RECORD.OPTION");

                    if (!isActive && HisConfigCFG.UNLOCK_FEE_OPTION == "1")
                    {
                        if (!(this.currentTreatment.FEE_LOCK_LOGINNAME == this.loginname
                            || this.allowUnlock))
                        {
                            btnLock.Enabled = false;
                        }
                    }
                    else if (isActive && (this.currentTreatment.MEDI_RECORD_ID == null || this.currentTreatment.MEDI_RECORD_ID <= 0) && btnLock.Enabled)
                    {
                        switch (keyconfig)
                        {
                            case 1:
                                if (this.currentTreatment.TDL_TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__KHAM)
                                {
                                    btnLock.Enabled = false;
                                }
                                break;
                            case 2:
                                if (this.currentTreatment.TDL_TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTNGOAITRU)
                                {
                                    btnLock.Enabled = false;
                                }
                                break;
                            case 3:
                                if (this.currentTreatment.TDL_TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTBANNGAY)
                                {
                                    btnLock.Enabled = false;
                                }
                                break;
                            case 4:
                                if (this.currentTreatment.TDL_TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTNOITRU)
                                {
                                    btnLock.Enabled = false;
                                }
                                break;
                            case 5:
                                if (this.currentTreatment.TDL_TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTBANNGAY || this.currentTreatment.TDL_TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTNGOAITRU || this.currentTreatment.TDL_TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTNOITRU)
                                {
                                    btnLock.Enabled = false;
                                }
                                break;
                            case 6:
                                btnLock.Enabled = false;
                                break;
                            default:
                                btnLock.Enabled = true;
                                break;
                        }
                    }
                    Inventec.Common.Logging.LogSystem.Debug("controlAcs_________________________" + HisConfigCFG.UNLOCK_FEE_OPTION);

                    Inventec.Common.Logging.LogSystem.Debug("controlAcs_________________________" + controlAcs.FirstOrDefault().CONTROL_CODE);
                    Inventec.Common.Logging.LogSystem.Debug("controlAcs_________________________2" + ControlCode.BtnUnlock);

                    if (HisConfigCFG.UNLOCK_FEE_OPTION != null && HisConfigCFG.UNLOCK_FEE_OPTION == "2")
                    {
                        this.btnLock.Enabled = (controlAcs != null && controlAcs.FirstOrDefault(o => o.CONTROL_CODE == ControlCode.BtnUnlock) != null);
                    }
                    bool valid = isLockHein ? false : isActive;
                    if (HisConfigCFG.MustFinishedForBilling == "4")
                    {
                        this.btnBill.Enabled = (this.currentTreatment.IS_PAUSE == (short)1) ||
                                               (this.currentTreatment.TDL_PATIENT_TYPE_ID != GetId(HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("MOS.HIS_PATIENT_TYPE.PATIENT_TYPE_CODE.BHYT")) && this.currentTreatment.HEIN_TREATMENT_TYPE_CODE == "KH");
                    }
                    else
                        this.btnBill.Enabled = ((this.currentTreatment.IS_ACTIVE != 0 || (isLockHein && this.currentTreatment.IS_ACTIVE == 0)) && (HisConfigCFG.MustFinishedForBilling != "3" || this.currentTreatment.IS_PAUSE == (short)1));
                    if (HisConfigCFG.DirectlyBillingOption == "2")
                    {
                        this.btnBillNotKC.Enabled = (this.currentTreatment.IS_ACTIVE != 0 || (isLockHein && this.currentTreatment.IS_ACTIVE == 0)) && HisConfigCFG.IsketChuyenCFG != null && HisConfigCFG.IsketChuyenCFG.Equals("4") && !this.currentTreatment.IS_PAUSE.HasValue && this.currentTreatment.TDL_TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__KHAM;
                    }
                    else
                    {
                        this.btnBillNotKC.Enabled = (this.currentTreatment.IS_ACTIVE != 0 || (isLockHein && this.currentTreatment.IS_ACTIVE == 0)) && HisConfigCFG.IsketChuyenCFG != null && HisConfigCFG.IsketChuyenCFG.Equals("4");
                    }

                    if (this.currentTreatment != null && (this.currentTreatment.DIS_SERVICE_DEPOSIT_OPTION != 1 || (this.currentTreatment.IN_TREATMENT_TYPE_ID != null && this.currentTreatment.DIS_DEPOSIT_OPTION_REQUEST != 1)))
                    {
                        btnDeposit.Enabled = true && valid;
                    }
                    else
                    {
                        btnDeposit.Enabled = false;
                    }

                    this.btnMienGiam.Enabled = false;
                    this.PopupItemStatusAdo.TransactionAllStt = valid;
                    this.PopupItemStatusAdo.TransactionDepositStt = HisConfigCFG.IsAllowAfterLocking ? true : valid;
                    this.btnTemporaryLock.Enabled = isTemporaryLock ? true : ((!isPause || isLockHein) ? false : true);
                    this.PopupItemStatusAdo.TemporaryLockStt = isTemporaryLock ? true : ((!isPause || isLockHein) ? false : true);
                    this.btnInvoice.Enabled = true;
                    this.PopupItemStatusAdo.LockStt = (!isPause || isLockHein) ? false : true;
                    this.btnDepositService.Enabled = valid;
                    this.btnRepay.Enabled = HisConfigCFG.IsAllowAfterLocking ? true : valid;
                    this.btnRepayService.Enabled = valid;
                    this.btnTranfer.Enabled = valid;
                    this.btnLockHistory.Enabled = true;
                    this.btnBordereau.Enabled = true;
                    this.btnTranList.Enabled = true;
                    this.btnOtherPayment.Enabled = true;
                }

                if (this.currentTreatment != null && this.currentTreatment.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)
                {
                    this.btnLock.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_TRANSACTION__BTN_LOCK_FEE", Base.ResourceLangManager.LanguageUCTransaction, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                else
                {
                    this.btnLock.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_TRANSACTION__BTN_UNLOCK_FEE", Base.ResourceLangManager.LanguageUCTransaction, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }

                if (this.currentTreatment != null && this.currentTreatment.IS_TEMPORARY_LOCK == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)
                {
                    this.btnTemporaryLock.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_TRANSACTION__BTN_TEMPORARY_UN_LOCK", Base.ResourceLangManager.LanguageUCTransaction, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                else
                {
                    this.btnTemporaryLock.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_TRANSACTION__BTN_TEMPORARY_LOCK", Base.ResourceLangManager.LanguageUCTransaction, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                EnableButttonFromFeil();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void Transaction_MouseRightClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if ((e.Item is BarButtonItem) && this.currentTreatment != null)
                {
                    var type = (PopupMenuProcessor.ItemType)e.Item.Tag;
                    switch (type)
                    {
                        case PopupMenuProcessor.ItemType.InBangKe:
                            PrintBangKeThanhToanChiPhiKCB();
                            break;
                        case PopupMenuProcessor.ItemType.LichSuGiaoDich:
                            OpenFormTransactionList();
                            break;
                        case PopupMenuProcessor.ItemType.InPhieuGiuThe:
                            PrintPhieuGiuTheBhyt();
                            break;
                        case PopupMenuProcessor.ItemType.InPhieuThanhToan:
                            PrintInPhieuThanhToan();
                            break;
                        case PopupMenuProcessor.ItemType.TamKhoa:
                            TemporaryLock();
                            break;
                        case PopupMenuProcessor.ItemType.MoTamKhoa:
                            TemporaryLock();
                            break;
                        case PopupMenuProcessor.ItemType.HoaDonDo:
                            BtnInvoice();
                            break;
                        case PopupMenuProcessor.ItemType.HoanUngDichVu:
                            BtnRepayService();
                            break;
                        case PopupMenuProcessor.ItemType.ChotNo:
                            btnTransactionDebt_Click(null, null);
                            break;
                        case PopupMenuProcessor.ItemType.ThuNo:
                            btnTransactionDebtCollect_Click(null, null);
                            break;
                        case PopupMenuProcessor.ItemType.TamThuDichVu:
                            BtnDepositService();
                            break;
                        case PopupMenuProcessor.ItemType.HoanUngNoiTru:
                            BtnRepay();
                            break;
                        case PopupMenuProcessor.ItemType.TamUngNoiTru:
                            BtnDeposit();
                            break;
                        case PopupMenuProcessor.ItemType.ThanhToan:
                            BtnBill();
                            break;
                        case PopupMenuProcessor.ItemType.ThuTrucTiep:
                            BtnBillNotKc();
                            break;
                        case PopupMenuProcessor.ItemType.Khoa:
                            BtnLock();
                            break;
                        case PopupMenuProcessor.ItemType.MoKhoa:
                            BtnLock();
                            break;
                        case PopupMenuProcessor.ItemType.LichSuTacDongHoSo:
                            BtnLockHistory();
                            break;
                        case PopupMenuProcessor.ItemType.MienGiam:
                            BtnMienGiam();
                            break;
                        case PopupMenuProcessor.ItemType.ThanhToanKhac:
                            BtnThanhToanKhac();
                            break;
                        case PopupMenuProcessor.ItemType.CheckInfoBHYT:
                            BtnCheckInfoBHYT();
                            break;
                        case PopupMenuProcessor.ItemType.InThanhToan:
                            PrintPhieuThanhToan();
                            break;
                        case PopupMenuProcessor.ItemType.InTrichSao:
                            PrintTrichSao();
                            break;
                        case PopupMenuProcessor.ItemType.HoaDonDienTu:
                            HoaDonDienTu();
                            break;
                        case PopupMenuProcessor.ItemType.CLS:
                            DuyetYLenhCLS();
                            break;
                        case PopupMenuProcessor.ItemType.YCTU:
                            YeuCauTamUng();
                            break;
                        default:
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void YeuCauTamUng()
        {
            try
            {
                if (this.currentTreatment == null)
                    return;
                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.RequestDeposit").FirstOrDefault();
                Inventec.Desktop.Common.Modules.Module moduleDeposit = new Inventec.Desktop.Common.Modules.Module();
                if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'HIS.Desktop.Plugins.RequestDeposit'");
                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                {
                    List<object> listArgs = new List<object>();
                    listArgs.Add(currentTreatment.ID);

                    HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.RequestDeposit", this.currentModule.RoomId, this.currentModule.RoomTypeId, listArgs);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void DuyetYLenhCLS()
        {
            try
            {
                if (this.currentTreatment == null)
                    return;
                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.ApproveServiceReqCLS").FirstOrDefault();
                if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'HIS.Desktop.Plugins.ApproveServiceReqCLS'");
                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                {
                    List<object> listArgs = new List<object>();
                    listArgs.Add(this.currentTreatment);
                    var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.RoomId, this.RoomTypeId), listArgs);
                    if (extenceInstance == null)
                    {
                        throw new ArgumentNullException("extenceInstance is null");
                    }

                    ((Form)extenceInstance).ShowDialog();

                }
                else
                {
                    MessageManager.Show(Base.ResourceMessageLang.ChucNangNayChuaDuocHoTroTrongPhienBanNay);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void HoaDonDienTu()
        {
            try
            {
                if (this.currentTreatment == null)
                    return;


                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.ElectronicBillTotal").FirstOrDefault();
                if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'HIS.Desktop.Plugins.ElectronicBillTotal'");
                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                {
                    moduleData.RoomId = this.RoomId;
                    moduleData.RoomTypeId = this.RoomTypeId;
                    List<object> listArgs = new List<object>();

                    listArgs.Add(this.currentTreatment);
                    listArgs.Add(this.listSereServ);
                    listArgs.Add(this.lastPatientType);
                    listArgs.Add(moduleData);
                    listArgs.Add(true);
                    var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.RoomId, this.RoomTypeId), listArgs);
                    if (extenceInstance == null)
                    {
                        throw new ArgumentNullException("extenceInstance is null");
                    }

                    ((Form)extenceInstance).ShowDialog();
                    FillDataToControlBySelectTreatment(true);
                    txtFindTreatmentCode.Focus();
                    txtFindTreatmentCode.SelectAll();
                }
                else
                {
                    MessageManager.Show(Base.ResourceMessageLang.ChucNangNayChuaDuocHoTroTrongPhienBanNay);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void BtnCheckInfoBHYT()
        {
            try
            {
                if (this.currentTreatment == null)
                    throw new NullReferenceException("CurrentTreatment=null");
                //V_HIS_TREATMENT treatment = new V_HIS_TREATMENT();
                //Inventec.Common.Mapper.DataObjectMapper.Map<V_HIS_TREATMENT>(treatment, this.currentTreatment);
                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.CheckInfoBHYT").FirstOrDefault();
                if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'HIS.Desktop.Plugins.CheckInfoBHYT'");
                moduleData.RoomId = this.RoomId;
                moduleData.RoomTypeId = this.RoomTypeId;
                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                {
                    List<object> listArgs = new List<object>();
                    listArgs.Add(this.currentTreatment.ID);
                    listArgs.Add(moduleData);
                    var extenceInstance = PluginInstance.GetPluginInstance(moduleData, listArgs);
                    if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");

                    ((Form)extenceInstance).ShowDialog();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void OpenFormTransactionList()
        {
            try
            {
                if (this.currentTreatment == null)
                    throw new NullReferenceException("CurrentTreatment=null");
                //V_HIS_TREATMENT treatment = new V_HIS_TREATMENT();
                //Inventec.Common.Mapper.DataObjectMapper.Map<V_HIS_TREATMENT>(treatment, this.currentTreatment);
                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.TransactionList").FirstOrDefault();
                if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'HIS.Desktop.Plugins.TransactionList'");
                moduleData.RoomId = this.RoomId;
                moduleData.RoomTypeId = this.RoomTypeId;
                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                {
                    List<object> listArgs = new List<object>();
                    listArgs.Add(this.currentTreatment);
                    listArgs.Add(moduleData);
                    var extenceInstance = PluginInstance.GetPluginInstance(moduleData, listArgs);
                    if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");

                    ((Form)extenceInstance).ShowDialog();
                    FillDataToControlBySelectTreatment(true);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void PrintBangKeThanhToanChiPhiKCB()
        {
            try
            {
                if (this.currentTreatment == null)
                    return;
                OpenFormBordereau(this.currentTreatment);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void PrintPhieuGiuTheBhyt()
        {
            try
            {
                if (this.currentTreatment == null)
                    return;
                Inventec.Common.RichEditor.RichEditorStore store = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumers.SarConsumer, ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), GlobalVariables.TemnplatePathFolder);
                store.RunPrintTemplate(MPS.Processor.Mps000173.PDO.PrintTypeCode.Mps000173, DelegatePrintTempalte);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void PrintInPhieuThanhToan()
        {
            try
            {
                if (this.currentTreatment == null)
                    return;
                CommonParam param = new CommonParam();
                HisTransactionViewFilter filterTran = new HisTransactionViewFilter();
                filterTran.TREATMENT_ID = currentTreatment.ID;
                filterTran.TRANSACTION_TYPE_IDs = new List<long>() { IMSys.DbConfig.HIS_RS.HIS_TRANSACTION_TYPE.ID__TT };
                filterTran.IS_CANCEL = false;
                List<V_HIS_TRANSACTION> transa = new BackendAdapter(param).Get<List<V_HIS_TRANSACTION>>("api/HisTransaction/GetView", ApiConsumers.MosConsumer, filterTran, param);
                if (transa != null && transa.Count > 0)
                {
                    Inventec.Common.RichEditor.RichEditorStore store = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumers.SarConsumer, ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), GlobalVariables.TemnplatePathFolder);
                    store.RunPrintTemplate(MPS.Processor.Mps000111.PDO.Mps000111PDO.printTypeCode, DelegatePrintTempalte);
                }
                else
                {
                    decimal totalReceive = (this.currentTreatment.TOTAL_PATIENT_PRICE ?? 0) - (this.currentTreatment.TOTAL_DEPOSIT_AMOUNT ?? 0) -
                        (this.currentTreatment.TOTAL_BILL_AMOUNT ?? 0) + (this.currentTreatment.TOTAL_BILL_TRANSFER_AMOUNT ?? 0) + (this.currentTreatment.TOTAL_REPAY_AMOUNT ?? 0);
                    if (totalReceive == 0)
                    {
                        IsShowPatientInfomation = true;
                        Inventec.Common.RichEditor.RichEditorStore store = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumers.SarConsumer, ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), GlobalVariables.TemnplatePathFolder);
                        store.RunPrintTemplate(MPS.Processor.Mps000111.PDO.Mps000111PDO.printTypeCode, DelegatePrintTempalte);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void PrintPhieuThanhToan()
        {
            try
            {
                if (this.currentTreatment == null)
                    return;
                Inventec.Common.RichEditor.RichEditorStore store = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumers.SarConsumer, ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), GlobalVariables.TemnplatePathFolder);
                store.RunPrintTemplate(PrintTypeCodeStore.PRINT_TYPE_CODE__PhieuHoanUngThanhToanRaVien_Mps000361, DelegatePrintTempalte);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private bool DelegatePrintTempalte(string printTypeCode, string fileName)
        {
            bool result = false;
            try
            {
                switch (printTypeCode)
                {
                    case PrintTypeCodeStore.PRINT_TYPE_CODE__PhieuHoanUngThanhToanRaVien_Mps000361:
                        InPhieuThanhToan(printTypeCode, fileName, ref result);
                        break;
                    case PrintTypeCodeStore.PRINT_TYPE_CODE__PhieuGiuTheBHYT_Mps000173:
                        InPhieuPhieuGiuTheBhyt(printTypeCode, fileName, ref result);
                        break;
                    case "MPS000383":
                        InPhieuTrichSao(printTypeCode, fileName, ref result);
                        break;
                    case "Mps000111":
                        InPhieuPhieuThuThanhToan(printTypeCode, fileName, ref result);
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
            return result;
        }

        private void InPhieuPhieuThuThanhToan(string printTypeCode, string fileName, ref bool result)
        {
            try
            {

                WaitingManager.Show();
                CommonParam param = new CommonParam();
                HisTransactionViewFilter filterTran = new HisTransactionViewFilter();
                filterTran.TREATMENT_ID = currentTreatment.ID;
                filterTran.TRANSACTION_TYPE_IDs = new List<long>() { IMSys.DbConfig.HIS_RS.HIS_TRANSACTION_TYPE.ID__TT };
                filterTran.IS_CANCEL = false;
                List<V_HIS_TRANSACTION> transa = new BackendAdapter(param).Get<List<V_HIS_TRANSACTION>>("api/HisTransaction/GetView", ApiConsumers.MosConsumer, filterTran, param);
                if (transa != null && transa.Count > 0)
                {
                    foreach (var item in transa)
                    {
                        //1
                        V_HIS_TRANSACTION transactionPrint = item;
                        HisBillFundFilter billFundFilter = new HisBillFundFilter();
                        billFundFilter.BILL_ID = transactionPrint.ID;
                        var listBillFund = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<HIS_BILL_FUND>>("api/HisBillFund/Get", ApiConsumers.MosConsumer, billFundFilter, null);
                        //

                        HisSereServBillFilter ssBillFilter = new HisSereServBillFilter();
                        ssBillFilter.BILL_ID = transactionPrint.ID;
                        var hisSSBills = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<HIS_SERE_SERV_BILL>>("api/HisSereServBill/Get", ApiConsumers.MosConsumer, ssBillFilter, null);
                        if (hisSSBills == null || hisSSBills.Count <= 0)
                        {
                            throw new Exception("Khong lay duoc SSBill theo billId: " + transactionPrint.BILL_TYPE_ID);
                        }

                        //
                        List<HIS_SERE_SERV> listSereServ = new List<HIS_SERE_SERV>();
                        HisSereServFilter ssFilter = new HisSereServFilter();
                        ssFilter.TREATMENT_ID = transactionPrint.TREATMENT_ID.Value;
                        List<HIS_SERE_SERV> listSereServApi = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<HIS_SERE_SERV>>("api/HisSereServ/Get", ApiConsumers.MosConsumer, ssFilter, null);

                        if (listSereServApi != null && listSereServApi.Count > 0 && hisSSBills != null && hisSSBills.Count > 0)
                        {
                            listSereServ = listSereServApi.Where(o => hisSSBills.Select(p => p.SERE_SERV_ID).Contains(o.ID)).ToList();
                        }
                        //
                        #region
                        HisPatientTypeAlterViewAppliedFilter patyAlterAppliedFilter = new HisPatientTypeAlterViewAppliedFilter();
                        patyAlterAppliedFilter.InstructionTime = Convert.ToInt64(DateTime.Now.ToString("yyyyMMddHHmmss"));
                        patyAlterAppliedFilter.TreatmentId = transactionPrint.TREATMENT_ID.Value;
                        var currentPatientTypeAlter = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<V_HIS_PATIENT_TYPE_ALTER>(HisRequestUriStore.HIS_PATIENT_TYPE_ALTER_GET_APPLIED, ApiConsumers.MosConsumer, patyAlterAppliedFilter, null);
                        if (currentPatientTypeAlter == null)
                        {
                            Inventec.Common.Logging.LogSystem.Info("Khong lay duoc PatientTypeAlterApplied: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => transactionPrint.TREATMENT_CODE), transactionPrint.TREATMENT_CODE));
                        }
                        //
                        HisDepartmentTranLastFilter departLastFilter = new HisDepartmentTranLastFilter();
                        departLastFilter.TREATMENT_ID = transactionPrint.TREATMENT_ID.Value;
                        departLastFilter.BEFORE_LOG_TIME = Convert.ToInt64(DateTime.Now.ToString("yyyyMMddHHmmss"));
                        var departmentTran = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<V_HIS_DEPARTMENT_TRAN>("api/HisDepartmentTran/GetLastByTreatmentId", ApiConsumers.MosConsumer, departLastFilter, null);
                        //
                        //2
                        V_HIS_PATIENT patient = new V_HIS_PATIENT();
                        if (transactionPrint.TDL_PATIENT_ID != null)
                        {
                            HisPatientViewFilter patientFilter = new HisPatientViewFilter();
                            patientFilter.ID = transactionPrint.TDL_PATIENT_ID;
                            var patients = new BackendAdapter(new CommonParam()).Get<List<V_HIS_PATIENT>>("api/HisPatient/GetView", ApiConsumer.ApiConsumers.MosConsumer, patientFilter, null);

                            if (patients != null && patients.Count > 0)
                            {
                                patient = patients.FirstOrDefault();
                            }
                        }
                        //
                        #endregion
                        WaitingManager.Hide();
                        string printerName = "";
                        if (GlobalVariables.dicPrinter.ContainsKey(printTypeCode))
                        {
                            printerName = GlobalVariables.dicPrinter[printTypeCode];
                        }

                        Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((transactionPrint != null ? transactionPrint.TREATMENT_CODE : ""), printTypeCode, currentModule != null ? currentModule.RoomId : 0);

                        // Lay thong tin cac dich vu da tam ung khong bi huy
                        List<HIS_SERE_SERV_DEPOSIT> listSereServDeposit = new List<HIS_SERE_SERV_DEPOSIT>();
                        var listssId = listSereServ.Select(o => o.ID).ToList();
                        CommonParam paramCommon = new CommonParam();
                        var skip = 0;
                        while (listssId.Count - skip > 0)
                        {
                            var limit = listssId.Skip(skip).Take(100).ToList();
                            skip = skip + 100;
                            MOS.Filter.HisSereServDepositFilter filter = new MOS.Filter.HisSereServDepositFilter();
                            filter.SERE_SERV_IDs = limit;
                            filter.IS_CANCEL = false;
                            var sereServDeposit = new Inventec.Common.Adapter.BackendAdapter(paramCommon).Get<List<HIS_SERE_SERV_DEPOSIT>>("api/HisSereServDeposit/Get", ApiConsumer.ApiConsumers.MosConsumer, filter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, paramCommon);

                            if (sereServDeposit != null)
                            {
                                listSereServDeposit.AddRange(sereServDeposit);
                            }
                        }


                        List<HIS_SESE_DEPO_REPAY> listSeseDepoRepay = new List<HIS_SESE_DEPO_REPAY>();
                        var listssDepoRepayId = listSereServDeposit.Select(o => o.ID).ToList();
                        CommonParam paramCommonDepoRepay = new CommonParam();
                        var skipDepoRepay = 0;
                        while (listssDepoRepayId.Count - skipDepoRepay > 0)
                        {
                            var limit = listssDepoRepayId.Skip(skipDepoRepay).Take(100).ToList();
                            skipDepoRepay = skipDepoRepay + 100;

                            MOS.Filter.HisSeseDepoRepayFilter filterSeseDepoRepay = new MOS.Filter.HisSeseDepoRepayFilter();
                            filterSeseDepoRepay.SERE_SERV_DEPOSIT_IDs = limit;
                            filterSeseDepoRepay.IS_CANCEL = false;
                            var seseDepoRepay = new Inventec.Common.Adapter.BackendAdapter(paramCommonDepoRepay).Get<List<HIS_SESE_DEPO_REPAY>>("api/HisSeseDepoRepay/Get", ApiConsumer.ApiConsumers.MosConsumer, filterSeseDepoRepay, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, paramCommonDepoRepay);

                            if (seseDepoRepay != null)
                            {
                                listSeseDepoRepay.AddRange(seseDepoRepay);
                            }
                        }

                        HisTransactionViewFilter depositFilter = new HisTransactionViewFilter();
                        depositFilter.TREATMENT_ID = transactionPrint.TREATMENT_ID.Value;
                        var lstTran = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_TRANSACTION>>("api/HisTransaction/GetView", ApiConsumers.MosConsumer, depositFilter, null);

                        HisSeseDepoRepayFilter ft = new HisSeseDepoRepayFilter();
                        ft.TDL_TREATMENT_ID = transactionPrint.TREATMENT_ID.Value;
                        var listDepoRepay = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<HIS_SESE_DEPO_REPAY>>("api/HisSeseDepoRepay/Get", ApiConsumers.MosConsumer, ft, null);
                        List<HIS_SERE_SERV_DEPOSIT> finalListSereServDeposit = new List<HIS_SERE_SERV_DEPOSIT>();

                        if (listSeseDepoRepay != null && listSeseDepoRepay.Count > 0)
                        {
                            finalListSereServDeposit = listSereServDeposit.Where(o => listSeseDepoRepay.All(k => k.SERE_SERV_DEPOSIT_ID != o.ID)).ToList();
                        }
                        else
                        {
                            finalListSereServDeposit = listSereServDeposit;
                        }

                        MPS.Processor.Mps000111.PDO.Mps000111PDO pdo = new MPS.Processor.Mps000111.PDO.Mps000111PDO(transactionPrint,
                            patient,
                            listBillFund,
                            listSereServ,
                            departmentTran,
                            currentPatientTypeAlter,
                            GetId(HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("MOS.HIS_PATIENT_TYPE.PATIENT_TYPE_CODE.BHYT")),
                            null,
                            finalListSereServDeposit,
                            lstTran,
                            listDepoRepay
                            );

                        if (ConfigApplications.CheDoInChoCacChucNangTrongPhanMem == 2)
                        {
                            result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName) { EmrInputADO = inputADO });
                        }
                        else
                        {
                            result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.Show, printerName) { EmrInputADO = inputADO });
                        }
                    }
                }
                else
                {
                    if (IsShowPatientInfomation)
                    {
                        #region
                        HisPatientTypeAlterViewAppliedFilter patyAlterAppliedFilter = new HisPatientTypeAlterViewAppliedFilter();
                        patyAlterAppliedFilter.InstructionTime = Convert.ToInt64(DateTime.Now.ToString("yyyyMMddHHmmss"));
                        patyAlterAppliedFilter.TreatmentId = currentTreatment.ID;
                        var currentPatientTypeAlter = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<V_HIS_PATIENT_TYPE_ALTER>(HisRequestUriStore.HIS_PATIENT_TYPE_ALTER_GET_APPLIED, ApiConsumers.MosConsumer, patyAlterAppliedFilter, null);
                        if (currentPatientTypeAlter == null)
                        {
                            Inventec.Common.Logging.LogSystem.Info("Khong lay duoc PatientTypeAlterApplied: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => currentTreatment.TREATMENT_CODE), currentTreatment.TREATMENT_CODE));
                        }
                        //
                        HisDepartmentTranLastFilter departLastFilter = new HisDepartmentTranLastFilter();
                        departLastFilter.TREATMENT_ID = currentTreatment.ID;
                        departLastFilter.BEFORE_LOG_TIME = Convert.ToInt64(DateTime.Now.ToString("yyyyMMddHHmmss"));
                        var departmentTran = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<V_HIS_DEPARTMENT_TRAN>("api/HisDepartmentTran/GetLastByTreatmentId", ApiConsumers.MosConsumer, departLastFilter, null);
                        //
                        //2
                        V_HIS_PATIENT patient = new V_HIS_PATIENT();

                        HisPatientViewFilter patientFilter = new HisPatientViewFilter();
                        patientFilter.ID = currentTreatment.PATIENT_ID;
                        var patients = new BackendAdapter(new CommonParam()).Get<List<V_HIS_PATIENT>>("api/HisPatient/GetView", ApiConsumer.ApiConsumers.MosConsumer, patientFilter, null);

                        if (patients != null && patients.Count > 0)
                        {
                            patient = patients.FirstOrDefault();
                        }

                        //
                        #endregion
                        WaitingManager.Hide();
                        string printerName = "";
                        if (GlobalVariables.dicPrinter.ContainsKey(printTypeCode))
                        {
                            printerName = GlobalVariables.dicPrinter[printTypeCode];
                        }

                        Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((currentTreatment != null ? currentTreatment.TREATMENT_CODE : ""), printTypeCode, currentModule != null ? currentModule.RoomId : 0);

                        MPS.Processor.Mps000111.PDO.Mps000111PDO pdo = new MPS.Processor.Mps000111.PDO.Mps000111PDO(null,
                            patient,
                            null,
                            null,
                            departmentTran,
                            currentPatientTypeAlter,
                            GetId(HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("MOS.HIS_PATIENT_TYPE.PATIENT_TYPE_CODE.BHYT")),
                            null,
                            null,
                            null,
                            null
                            );

                        if (ConfigApplications.CheDoInChoCacChucNangTrongPhanMem == 2)
                        {
                            result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName) { EmrInputADO = inputADO });
                        }
                        else
                        {
                            result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.Show, printerName) { EmrInputADO = inputADO });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            finally
            {
                WaitingManager.Hide();
            }
        }

        private static long GetId(string code)
        {
            long result = 0;
            try
            {
                var data = BackendDataWorker.Get<HIS_PATIENT_TYPE>().FirstOrDefault(o => o.PATIENT_TYPE_CODE == code);
                if ((data != null && data.ID > 0))
                    result = data.ID;
            }
            catch (Exception ex)
            {
                result = 0;
            }
            return result;
        }
        private void OpenFormBordereau(V_HIS_TREATMENT_FEE treatment)
        {
            Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.Bordereau").FirstOrDefault();
            if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'HIS.Desktop.Plugins.Bordereau'");
            if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
            {
                List<object> listArgs = new List<object>();
                listArgs.Add(treatment.ID);
                var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, RoomId, RoomTypeId), listArgs);
                if (extenceInstance == null)
                {
                    throw new ArgumentNullException("moduleData is null");
                }

                ((Form)extenceInstance).ShowDialog();
            }
            else
            {
                MessageManager.Show(Base.ResourceMessageLang.ChucNangNayChuaDuocHoTroTrongPhienBanNay);
            }
        }

        private void GetNextDepartment()
        {
            try
            {
                HisDepartmentTranFilter filter = new HisDepartmentTranFilter();
                filter.TREATMENT_ID = this.currentTreatment.ID;
                List<HIS_DEPARTMENT_TRAN> departmentTranList = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<HIS_DEPARTMENT_TRAN>>("api/HisDepartmentTran/Get", ApiConsumer.ApiConsumers.MosConsumer, filter, null);
                if (departmentTranList != null && departmentTranList.Count > 0)
                {
                    var departmentTran = departmentTranList.Where(o => !o.DEPARTMENT_IN_TIME.HasValue).OrderByDescending(o => o.ID).FirstOrDefault();
                    if (departmentTran != null)
                    {
                        var department = BackendDataWorker.Get<HIS_DEPARTMENT>().FirstOrDefault(o => o.ID == departmentTran.DEPARTMENT_ID);
                        lblNextDepartment.Text = department != null ? department.DEPARTMENT_NAME : "";
                    }
                    else
                        lblNextDepartment.Text = "";
                }
                else
                    lblNextDepartment.Text = "";
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InPhieuThanhToan(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                if (this.currentTreatment == null)
                    return;
                WaitingManager.Show();
                string printerName = "";
                if (GlobalVariables.dicPrinter.ContainsKey(printTypeCode))
                {
                    printerName = GlobalVariables.dicPrinter[printTypeCode];
                }
                CommonParam param = new CommonParam();
                HisTransactionViewFilter filterTran = new HisTransactionViewFilter();
                filterTran.TREATMENT_ID = currentTreatment.ID;
                filterTran.TRANSACTION_TYPE_IDs = new List<long>() { IMSys.DbConfig.HIS_RS.HIS_TRANSACTION_TYPE.ID__TT };
                filterTran.IS_CANCEL = false;
                List<V_HIS_TRANSACTION> transa = new BackendAdapter(param).Get<List<V_HIS_TRANSACTION>>("api/HisTransaction/GetView", ApiConsumers.MosConsumer, filterTran, param);
                if (transa == null) transa = new List<V_HIS_TRANSACTION>();

                transa = transa.OrderByDescending(o => o.TRANSACTION_TIME).ToList();
                //HisDepartmentViewFilter filterDepar = new HisDepartmentViewFilter();
                //filterDepar.ID = this.currentTreatment.END_DEPARTMENT_ID;
                //CommonParam param1 = new CommonParam();
                //var department = new BackendAdapter(param1).Get<List<V_HIS_DEPARTMENT>>("api/HisDepartment/GetView", ApiConsumers.MosConsumer, filterDepar, param1);
                var department = BackendDataWorker.Get<V_HIS_DEPARTMENT>().FirstOrDefault(o => o.ID == this.currentTreatment.END_DEPARTMENT_ID);

                //var trans = transa.Where(o => o.CREATE_TIME >= o.TRANSACTION_TIME && o.TRANSACTION_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TRANSACTION_TYPE.ID__TT);
                //var transac = trans.OrderByDescending(o => o.TRANSACTION_TIME);
                //var transaction = transac.FirstOrDefault<V_HIS_TRANSACTION>();

                //thông tin số và sổ thì lấy như sau:
                //2. ở màn hình viện phí (lấy giao dịch thanh toán có create_time >= outtime(thời gian kết thúc) )
                //==>> TRANSACTION_TIME >= currentTreatment.OUT_TIME
                //V_HIS_TRANSACTION transaction = null;
                //if (this.currentTreatment.OUT_TIME.HasValue)
                //{
                //    transaction = transa.Where(o => !o.SALE_TYPE_ID.HasValue).FirstOrDefault(o => o.TRANSACTION_TIME >= this.currentTreatment.OUT_TIME.Value);
                //}

                //if (transaction == null) transaction = transa.FirstOrDefault();

                Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((this.currentTreatment != null ? this.currentTreatment.TREATMENT_CODE : ""), printTypeCode, currentModule != null ? currentModule.RoomId : 0);

                MPS.Processor.Mps000361.PDO.Mps000361PDO pdo = new MPS.Processor.Mps000361.PDO.Mps000361PDO(this.currentTreatment, transa, department);
                MPS.ProcessorBase.Core.PrintData printData = null;
                if (ConfigApplications.CheDoInChoCacChucNangTrongPhanMem == 2)
                {
                    printData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName) { EmrInputADO = inputADO };
                }
                else
                {
                    printData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.Show, printerName) { EmrInputADO = inputADO };
                }
                WaitingManager.Hide();
                result = MPS.MpsPrinter.Run(printData);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
        }

        private void InPhieuPhieuGiuTheBhyt(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                var currentPatientTypeAlter = new Inventec.Common.Adapter.BackendAdapter(param).Get<V_HIS_PATIENT_TYPE_ALTER>("api/HisPatientTypeAlter/GetViewLastByTreatmentId", ApiConsumers.MosConsumer, this.currentTreatment.ID, param);
                if (currentPatientTypeAlter == null)
                {
                    throw new Exception("Khong lay duoc thong tin doi tuong cua ho so dieu tri Id: " + this.currentTreatment.ID);
                }

                HisDepartmentTranLastFilter departLastFilter = new HisDepartmentTranLastFilter();
                departLastFilter.TREATMENT_ID = this.currentTreatment.ID;
                long nowTime = Convert.ToInt64(DateTime.Now.ToString("yyyyMMddHHmmss"));
                if (this.currentTreatment.IN_TIME > nowTime)
                {
                    departLastFilter.BEFORE_LOG_TIME = this.currentTreatment.IN_TIME;
                }
                else
                {
                    departLastFilter.BEFORE_LOG_TIME = nowTime;
                }
                var departmentTran = new Inventec.Common.Adapter.BackendAdapter(param).Get<V_HIS_DEPARTMENT_TRAN>("api/HisDepartmentTran/GetLastByTreatmentId", ApiConsumers.MosConsumer, departLastFilter, param);

                string printerName = "";
                if (GlobalVariables.dicPrinter.ContainsKey(printTypeCode))
                {
                    printerName = GlobalVariables.dicPrinter[printTypeCode];
                }

                Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((this.currentTreatment != null ? this.currentTreatment.TREATMENT_CODE : ""), printTypeCode, currentModule != null ? currentModule.RoomId : 0);

                MPS.Processor.Mps000173.PDO.Mps000173PDO pdo = new MPS.Processor.Mps000173.PDO.Mps000173PDO(this.currentTreatment, currentPatientTypeAlter, departmentTran);
                MPS.ProcessorBase.Core.PrintData printData = null;
                if (ConfigApplications.CheDoInChoCacChucNangTrongPhanMem == 2)
                {

                    printData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName) { EmrInputADO = inputADO };
                }
                else
                {
                    printData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.Show, printerName) { EmrInputADO = inputADO };
                }
                WaitingManager.Hide();
                result = MPS.MpsPrinter.Run(printData);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
        }

        private void PrintTrichSao()
        {
            try
            {
                if (this.currentTreatment == null)
                    return;
                Inventec.Common.RichEditor.RichEditorStore store = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumers.SarConsumer, ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), GlobalVariables.TemnplatePathFolder);
                store.RunPrintTemplate("MPS000383", DelegatePrintTempalte);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InPhieuTrichSao(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                if (this.currentTreatment == null)
                    return;
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                HisTransactionViewFilter filterTran = new HisTransactionViewFilter();
                filterTran.TREATMENT_ID = currentTreatment.ID;
                filterTran.TRANSACTION_TYPE_IDs = new List<long>() { IMSys.DbConfig.HIS_RS.HIS_TRANSACTION_TYPE.ID__TT };
                filterTran.IS_CANCEL = false;
                List<HIS_TRANSACTION> transa = new BackendAdapter(param).Get<List<HIS_TRANSACTION>>("api/HisTransaction/Get", ApiConsumers.MosConsumer, filterTran, param);
                if (transa == null) transa = new List<HIS_TRANSACTION>();

                List<HIS_DEPARTMENT> listDepartments = BackendDataWorker.Get<HIS_DEPARTMENT>();
                List<HIS_HEIN_SERVICE_TYPE> listHeinServiceTypes = BackendDataWorker.Get<HIS_HEIN_SERVICE_TYPE>();

                HisSereServFilter ssFilter = new HisSereServFilter();
                ssFilter.TREATMENT_ID = currentTreatment.ID;
                List<HIS_SERE_SERV> lisSereServ = new BackendAdapter(param).Get<List<HIS_SERE_SERV>>("api/HisSereServ/Get", ApiConsumers.MosConsumer, ssFilter, param);

                HisSereServBillFilter ssbFilter = new HisSereServBillFilter();
                ssbFilter.TDL_TREATMENT_ID = currentTreatment.ID;
                List<HIS_SERE_SERV_BILL> lisSereServBill = new BackendAdapter(param).Get<List<HIS_SERE_SERV_BILL>>("api/HisSereServBill/Get", ApiConsumers.MosConsumer, ssbFilter, param);

                HisPatientTypeAlterViewAppliedFilter filter = new HisPatientTypeAlterViewAppliedFilter();
                filter.TreatmentId = currentTreatment.ID;
                filter.InstructionTime = Inventec.Common.DateTime.Get.Now() ?? 0;
                V_HIS_PATIENT_TYPE_ALTER currentPatientTypeAlter = new BackendAdapter(param).Get<V_HIS_PATIENT_TYPE_ALTER>(HisRequestUriStore.HIS_PATIENT_TYPE_ALTER_GET_APPLIED, ApiConsumers.MosConsumer, filter, param);

                Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((this.currentTreatment != null ? this.currentTreatment.TREATMENT_CODE : ""), printTypeCode, currentModule != null ? currentModule.RoomId : 0);

                MPS.Processor.Mps000383.PDO.Mps000383PDO pdo = new MPS.Processor.Mps000383.PDO.Mps000383PDO(lisSereServ, transa, lisSereServBill, currentTreatment, currentPatientTypeAlter, listDepartments, listHeinServiceTypes);

                MPS.ProcessorBase.Core.PrintData printData = null;
                string printerName = "";
                if (GlobalVariables.dicPrinter.ContainsKey(printTypeCode))
                {
                    printerName = GlobalVariables.dicPrinter[printTypeCode];
                }
                if (ConfigApplications.CheDoInChoCacChucNangTrongPhanMem == 2)
                {
                    printData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName) { EmrInputADO = inputADO };
                }
                else
                {
                    printData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.Show, printerName) { EmrInputADO = inputADO };
                }

                WaitingManager.Hide();
                result = MPS.MpsPrinter.Run(printData);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
        }
    }
}

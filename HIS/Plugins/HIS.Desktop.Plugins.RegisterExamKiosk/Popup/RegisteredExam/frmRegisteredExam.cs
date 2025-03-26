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
using His.Bhyt.InsuranceExpertise;
using His.Bhyt.InsuranceExpertise.LDO;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Common;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Plugins.RegisterExamKiosk.ADO;
using HIS.Desktop.Plugins.RegisterExamKiosk.Config;
using HIS.Desktop.Plugins.RegisterExamKiosk.Popup.CheckHeinCardGOV;
using HIS.Desktop.Plugins.RegisterExamKiosk.Popup.ChooseObject;
using HIS.Desktop.Plugins.RegisterExamKiosk.Popup.RegisterExamKiosk;
using Inventec.Common.Adapter;
using Inventec.Common.QrCodeBHYT;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
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
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.RegisterExamKiosk.Popup.RegisteredExam
{
    public partial class frmRegisteredExam : HIS.Desktop.Utility.FormBase
    {
        Inventec.Desktop.Common.Modules.Module currentModule;
        HisPatientForKioskSDO currentPatientSdo;
        HIS_TREATMENT currentTreatment;
        InformationObjectADO patientSdo;
        HIS.Desktop.Common.DelegateSelectData loadFormRegisterExamKiosk;
        Action OpenFormByPatientData;
        HisPatientForKioskSDO patientForKioskSDO;
        bool IsEmergency;
        HIS.Desktop.Common.DelegateCloseForm_Uc DelegateClose;
        System.Threading.Thread CloseThread;
        public frmRegisteredExam(Inventec.Desktop.Common.Modules.Module module, InformationObjectADO _PatientSdo, HIS.Desktop.Common.DelegateSelectData selectDataPatientType, Action _OpenFormByPatientData, HisPatientForKioskSDO _patientForKioskSDO, bool _IsEmergency, HIS.Desktop.Common.DelegateCloseForm_Uc closeForm_Uc)
            : base(module)
        {
            InitializeComponent();
            this.currentModule = module;
            this.currentPatientSdo = _PatientSdo.PatientForKiosk;
            this.patientSdo = _PatientSdo;
            this.loadFormRegisterExamKiosk = selectDataPatientType;
            this.OpenFormByPatientData = _OpenFormByPatientData;
            this.patientForKioskSDO = _patientForKioskSDO;
            this.IsEmergency = _IsEmergency;
            this.DelegateClose = closeForm_Uc;
            CloseThread = new System.Threading.Thread(ClosingForm);
            CloseThread.Start();
            try
            {
                string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                this.Icon = Icon.ExtractAssociatedIcon(iconPath);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                stopThread = true;
                PrintProcess();
                stopThread = false;
                ResetLoopCount();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void frmRegisteredExam_Load(object sender, EventArgs e)
        {
            try
            {
                LoadTreatment();
                FillDataToEditor(currentPatientSdo);
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("currentPatientSdo:_", currentPatientSdo));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataTile(decimal patientMissing, decimal balance)
        {
            try
            {
                stopThread = true;
                //IN
                var groupPrint = new TileGroup();
                groupPrint.Text = "IN PHIẾU KHÁM";
                TileItem tilePrint = new TileItem();
                tilePrint.Text = "IN PHIẾU KHÁM";
                tilePrint.AppearanceItem.Normal.FontSizeDelta = 18;
                tilePrint.AppearanceItem.Normal.ForeColor = Color.White;
                tilePrint.TextAlignment = TileItemContentAlignment.MiddleCenter;
                tilePrint.ItemSize = TileItemSize.Large;
                Thread.Sleep(10);
                tilePrint.AppearanceItem.Normal.BorderColor = Color.DarkGreen;
                tilePrint.Checked = false;
                tilePrint.Visible = true;
                tilePrint.ItemClick += btnPrint_Click;
                tilePrint.AppearanceItem.Normal.BackColor = Color.DarkGreen;
                groupPrint.Items.Add(tilePrint);
                tileControl.Groups.Add(groupPrint);

                if (balance > 0 && patientMissing > 0 && balance >= patientMissing && !string.IsNullOrEmpty(this.patientSdo.ServiceCode))
                {
                    // thanh toán
                    var groupPay = new TileGroup();
                    groupPay.Text = "THANH TOÁN";
                    TileItem tilePay = new TileItem();
                    tilePay.Text = "THANH TOÁN";
                    tilePay.AppearanceItem.Normal.FontSizeDelta = 18;
                    tilePay.AppearanceItem.Normal.ForeColor = Color.White;
                    tilePay.TextAlignment = TileItemContentAlignment.MiddleCenter;
                    tilePay.ItemSize = TileItemSize.Large;
                    Thread.Sleep(10);
                    tilePay.AppearanceItem.Normal.BorderColor = Color.DarkGreen;
                    tilePay.Checked = false;
                    tilePay.Visible = true;
                    tilePay.ItemClick += btnPay_Click;
                    tilePay.AppearanceItem.Normal.BackColor = Color.DarkGreen;
                    groupPay.Items.Add(tilePay);
                    tileControl.Groups.Add(groupPay);

                    // In và thanh toán
                    var groupPayAndPrint = new TileGroup();
                    groupPayAndPrint.Text = "IN PHIẾU KHÁM & THANH TOÁN";
                    TileItem tilePayAndPrint = new TileItem();
                    tilePayAndPrint.Text = "IN PHIẾU KHÁM & THANH TOÁN";
                    tilePayAndPrint.AppearanceItem.Normal.FontSizeDelta = 18;
                    tilePayAndPrint.AppearanceItem.Normal.ForeColor = Color.White;
                    tilePayAndPrint.TextAlignment = TileItemContentAlignment.MiddleCenter;
                    tilePayAndPrint.ItemSize = TileItemSize.Large;
                    Thread.Sleep(10);
                    tilePayAndPrint.AppearanceItem.Normal.BorderColor = Color.DarkGreen;
                    tilePayAndPrint.Checked = false;
                    tilePayAndPrint.Visible = true;
                    tilePayAndPrint.ItemClick += btnPrintAndPay_Click;
                    tilePayAndPrint.AppearanceItem.Normal.BackColor = Color.DarkGreen;
                    groupPayAndPrint.Items.Add(tilePayAndPrint);
                    tileControl.Groups.Add(groupPayAndPrint);
                }
                // Đăng ký mới
                var groupRegisterNew = new TileGroup();
                groupRegisterNew.Text = "ĐĂNG KÝ MỚI";
                TileItem tileRegisterNew = new TileItem();
                tileRegisterNew.Text = "ĐĂNG KÝ MỚI";
                tileRegisterNew.AppearanceItem.Normal.FontSizeDelta = 18;
                tileRegisterNew.AppearanceItem.Normal.ForeColor = Color.White;
                tileRegisterNew.TextAlignment = TileItemContentAlignment.MiddleCenter;
                tileRegisterNew.ItemSize = TileItemSize.Large;
                Thread.Sleep(10);
                tileRegisterNew.AppearanceItem.Normal.BorderColor = Color.DarkGreen;
                tileRegisterNew.Checked = false;
                tileRegisterNew.Visible = true;
                tileRegisterNew.ItemClick += btnRegisterNew_Click;
                tileRegisterNew.AppearanceItem.Normal.BackColor = Color.DarkGreen;
                groupRegisterNew.Items.Add(tileRegisterNew);
                tileControl.Groups.Add(groupRegisterNew);

                // Kết thúc
                var groupFinish = new TileGroup();
                groupFinish.Text = "KẾT THÚC";
                TileItem tileFinish = new TileItem();
                tileFinish.Text = "KẾT THÚC";
                tileFinish.AppearanceItem.Normal.FontSizeDelta = 18;
                tileFinish.AppearanceItem.Normal.ForeColor = Color.White;
                tileFinish.TextAlignment = TileItemContentAlignment.MiddleCenter;
                tileFinish.ItemSize = TileItemSize.Large;
                Thread.Sleep(10);
                tileFinish.AppearanceItem.Normal.BorderColor = Color.Red;
                tileFinish.Checked = false;
                tileFinish.Visible = true;
                tileFinish.ItemClick += btnFinish_Click;
                tileFinish.AppearanceItem.Normal.BackColor = Color.Red;
                groupFinish.Items.Add(tileFinish);
                tileControl.Groups.Add(groupFinish);
                stopThread = false;
                ResetLoopCount();
            }
            catch (Exception ex)
            {
                stopThread = false;
                ResetLoopCount();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadTreatment()
        {
            try
            {
                stopThread = true;
                CommonParam param = new CommonParam();
                HisTreatmentFilter filter = new HisTreatmentFilter();
                filter.ID = this.currentPatientSdo.TreatmentId ?? -1;
                var apirs = new BackendAdapter(param).Get<List<HIS_TREATMENT>>("api/HisTreatment/Get", ApiConsumer.ApiConsumers.MosConsumer, filter, param);
                this.currentTreatment = apirs != null ? apirs.FirstOrDefault() : null;
                stopThread = false;
                ResetLoopCount();
            }
            catch (Exception ex)
            {
                stopThread = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToEditor(HisPatientForKioskSDO data)
        {
            try
            {
                stopThread = true;
                decimal totalPatientPrice = 0;
                decimal patientPaid = 0;
                decimal patientMissing = 0;
                decimal balance = 0;
                if (data.SereServs != null && data.SereServs.Count() > 0)
                {
                    lblServiceName.Text = String.Join(", ", data.SereServs.Where(o => o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__KH).Select(o => o.TDL_SERVICE_NAME));
                    totalPatientPrice = Math.Round(data.SereServs.Sum(o => o.VIR_TOTAL_PATIENT_PRICE ?? 0));
                }

                if (data.ServiceReqs != null && data.ServiceReqs.Count() > 0)
                {
                    var executeRoom = data.ServiceReqs.FirstOrDefault(o => o.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__KH);
                    lblExecuteRoomName.Text = executeRoom != null ? executeRoom.EXECUTE_ROOM_NAME : "";
                    lblIntructionDate.Text = executeRoom != null ? Inventec.Common.DateTime.Convert.TimeNumberToDateString(executeRoom.INTRUCTION_DATE) : "";
                    lblNumOrder.Text = executeRoom != null ? executeRoom.NUM_ORDER.ToString() : "";
                }

                if ((data.SereServDeposits != null && data.SereServDeposits.Count() > 0) || (data.SereServBills != null && data.SereServBills.Count() > 0))
                {
                    var _AmountDeposits = data.SereServDeposits != null && data.SereServDeposits.Count() > 0 ? data.SereServDeposits.Where(o => o.IS_CANCEL != 1).Sum(o => o.AMOUNT) : 0;
                    var _AmountBills = data.SereServBills != null && data.SereServBills.Count() > 0 ? data.SereServBills.Where(o => o.IS_CANCEL != 1).Sum(o => o.PRICE) : 0;
                    patientPaid = _AmountDeposits + _AmountBills;
                }
                patientMissing = Math.Round(totalPatientPrice - patientPaid);

                if (patientMissing >= 0)
                {
                    lciConThieu.Visibility = lcilbConThieu.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    lblPatientMissing.Text = patientMissing.ToString() + " đ";
                }

                if (data.Balance != null)
                {
                    lciSoDu.Visibility = layoutControlItem18.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    balance = Math.Round(data.Balance ?? 0);
                    lblBalance.Text = balance.ToString() + " đ";
                }
                lblTotalPatientPrice.Text = totalPatientPrice.ToString() + " đ";
                lblPatientPaid.Text = Math.Round(patientPaid).ToString() + " đ";
                LoadDataTile(patientMissing, balance);
                stopThread = false;
                ResetLoopCount();
            }
            catch (Exception ex)
            {
                stopThread = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void PrintProcess()
        {
            try
            {
                Inventec.Common.RichEditor.RichEditorStore richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(HIS.Desktop.ApiConsumer.ApiConsumers.SarConsumer, HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), HIS.Desktop.LocalStorage.Location.PrintStoreLocation.PrintTemplatePath);

                richEditorMain.RunPrintTemplate(PrintCode.PhieuInYeuCauKham, DelegateRunPrinterCare);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private bool DelegateRunPrinterCare(string printTypeCode, string fileName)
        {
            bool result = false;
            try
            {
                switch (printTypeCode)
                {
                    case PrintCode.PhieuInYeuCauKham:
                        InPhieuYeuCauKham(printTypeCode, fileName, ref result);
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return result;
        }

        private void InPhieuYeuCauKham(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                stopThread = true;
                if (currentPatientSdo.TreatmentId == null)
                {
                    return;
                }
                var PatientTypeAlterPrint = this.currentPatientSdo.PatientTypeAlters != null && this.currentPatientSdo.PatientTypeAlters.Count() > 0 ? this.currentPatientSdo.PatientTypeAlters.OrderByDescending(o => o.LOG_TIME).FirstOrDefault() : null;
                var ServiceReqPrint = this.currentPatientSdo.ServiceReqs != null && this.currentPatientSdo.ServiceReqs.Count() > 0 ? this.currentPatientSdo.ServiceReqs.FirstOrDefault(o => o.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__KH) : null;
                if (this.currentTreatment != null && this.currentTreatment.HAS_CARD == 1)
                {
                    List<HIS_CARD> hisCardList = new List<HIS_CARD>();
                    HisCardFilter cardfilter = new HisCardFilter();
                    cardfilter.PATIENT_ID = this.currentTreatment.PATIENT_ID;
                    hisCardList = new BackendAdapter(new CommonParam()).Get<List<HIS_CARD>>("api/HisCard/Get", ApiConsumers.MosConsumer, cardfilter, null);
                    if (hisCardList != null && hisCardList.Count > 0)
                    {
                        PrintKiosk printKiosk = new PrintKiosk(PatientTypeAlterPrint, ServiceReqPrint, this.currentPatientSdo.SereServs, null, printTypeCode, fileName, this.currentTreatment, currentPatientSdo.SereServDeposits, currentPatientSdo.SereServBills, currentPatientSdo.Transactions, false, hisCardList);
                        printKiosk.RunPrintHasCard();
                    }
                }
                else
                {
                    PrintKiosk printKiosk = new PrintKiosk(PatientTypeAlterPrint, ServiceReqPrint, this.currentPatientSdo.SereServs, null, printTypeCode, fileName, this.currentTreatment, currentPatientSdo.SereServDeposits, currentPatientSdo.SereServBills, currentPatientSdo.Transactions, false);
                    //printKiosk.PrintMps25();
                    printKiosk.RunPrint();

                }
                stopThread = false;
                ResetLoopCount();
            }
            catch (Exception ex)
            {
                stopThread = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnPay_Click(object sender, EventArgs e)
        {
            try
            {
                stopThread = true;
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                bool success = false;
                EpaymentDepositSD sdo = new EpaymentDepositSD();
                sdo.RequestRoomId = currentModule.RoomId;
                sdo.ServiceReqIds = currentPatientSdo.ServiceReqs != null ? currentPatientSdo.ServiceReqs.Select(o => o.ID).ToList() : new List<long>();
                sdo.CardServiceCode = patientSdo.ServiceCode;
                var rs = new BackendAdapter(param).Post<EpaymentDepositResultSDO>("api/HisTransaction/EpaymentDeposit", ApiConsumers.MosConsumer, sdo, param);
                if (rs != null)
                {
                    success = true;
                    if (rs.Transaction != null)
                    {
                        currentPatientSdo.Transactions = currentPatientSdo.Transactions != null ? currentPatientSdo.Transactions : new List<V_HIS_TRANSACTION>();
                        currentPatientSdo.Transactions.Add(rs.Transaction);
                    }
                    if (rs.SereServDeposit != null && rs.SereServDeposit.Count() > 0)
                    {
                        currentPatientSdo.SereServDeposits = currentPatientSdo.SereServDeposits != null ? currentPatientSdo.SereServDeposits : new List<HIS_SERE_SERV_DEPOSIT>();
                        currentPatientSdo.SereServDeposits.AddRange(rs.SereServDeposit);
                    }

                }
                WaitingManager.Hide();

                MessageManager.Show(this, param, success);
                stopThread = false;
                ResetLoopCount();
            }
            catch (Exception ex)
            {
                stopThread = false;
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnPrintAndPay_Click(object sender, EventArgs e)
        {
            try
            {
                btnPay_Click(null, null);
                btnPrint_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnFinish_Click(object sender, EventArgs e)
        {
            try
            {
                
                this.Close();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnRegisterNew_Click(object sender, EventArgs e)
        {
            try
            {
                stopThread = true;
                if (patientForKioskSDO != null)
                {
                    string message = "";

                    if (HisConfigCFG.CHECK_PREVIOUS_DEBT_OPTION == "1" && patientForKioskSDO.PreviousDebtTreatments != null
                        && patientForKioskSDO.PreviousDebtTreatments.Count > 0)
                    {
                        string treatmentPrevis = String.Join(",", patientForKioskSDO.PreviousDebtTreatments.Distinct().ToList());
                        message += String.Format("Đợt khám/điều trị trước đó của bệnh nhân còn nợ tiền viện phí. Mã hồ sơ điều trị {0}.", treatmentPrevis);

                        if (DevExpress.XtraEditors.XtraMessageBox.Show(message + " Bạn có muốn tiếp tục?", "Thông báo", MessageBoxButtons.YesNo) != DialogResult.Yes)
                        {
                            return;
                        }
                    }
                    else if (HisConfigCFG.CHECK_PREVIOUS_DEBT_OPTION == "3" && patientForKioskSDO.LastTreatmentFee != null)
                    {
                        var soTienBnPhaiNopThem = patientForKioskSDO.LastTreatmentFee.TOTAL_PATIENT_PRICE - patientForKioskSDO.LastTreatmentFee.TOTAL_DEPOSIT_AMOUNT - patientForKioskSDO.LastTreatmentFee.TOTAL_BILL_AMOUNT + patientForKioskSDO.LastTreatmentFee.TOTAL_BILL_TRANSFER_AMOUNT + patientForKioskSDO.LastTreatmentFee.TOTAL_REPAY_AMOUNT;
                        if (patientForKioskSDO.LastTreatmentFee.IS_ACTIVE == 1 || soTienBnPhaiNopThem > 0)
                        {
                            message += String.Format("Đợt khám/điều trị trước đó của bệnh nhân có số tiền phải trả > 0 hoặc chưa duyệt khóa viện phí. Mã hồ sơ điều trị {0}.", patientForKioskSDO.LastTreatmentFee.TREATMENT_CODE);
                            if (DevExpress.XtraEditors.XtraMessageBox.Show(message, "Thông báo", MessageBoxButtons.YesNo) != DialogResult.Yes)
                            {
                                return;
                            }
                        }
                    }
                    else if (HisConfigCFG.CHECK_PREVIOUS_DEBT_OPTION == "2" && !IsEmergency && patientForKioskSDO.PreviousDebtTreatmentDetails != null
                       && patientForKioskSDO.PreviousDebtTreatmentDetails.Count > 0)
                    {
                        var dtTreatmentDetails = patientForKioskSDO.PreviousDebtTreatmentDetails.Where(o => o.PATIENT_TYPE_ID == HisConfigCFG.PATIENT_TYPE_ID__BHYT).ToList();

                        if (dtTreatmentDetails != null && dtTreatmentDetails.Count > 0)
                        {
                            string treatmentPrevis = String.Join(",", dtTreatmentDetails.Select(o => o.TDL_TREATMENT_CODE).ToList());
                            message += String.Format("Đợt khám/điều trị trước đó của bệnh nhân còn nợ viện phí. Mã hồ sơ điều trị {0}. Không cho phép tiếp đón", treatmentPrevis);
                            DevExpress.XtraEditors.XtraMessageBox.Show(message, "Thông báo", MessageBoxButtons.OK);
                            return;
                        }


                    }
                    else if (HisConfigCFG.CHECK_PREVIOUS_DEBT_OPTION == "4" && patientForKioskSDO.PreviousDebtTreatments != null
                            && patientForKioskSDO.PreviousDebtTreatments.Count > 0)
                    {
                        string treatmentPrevis = String.Join(",", patientForKioskSDO.PreviousDebtTreatments.Distinct().ToList());
                        message += String.Format("Đợt khám/điều trị trước đó của bệnh nhân có số tiền phải trả lớn hơn 0 hoặc chưa duyệt khóa viện phí. Mã hồ sơ điều trị {0}.", treatmentPrevis);
                        if (DevExpress.XtraEditors.XtraMessageBox.Show(message, "Thông báo", MessageBoxButtons.YesNo) != DialogResult.Yes)
                        {
                            return;
                        }
                    }
                }
                if (OpenFormByPatientData != null)
                {
                    this.Close();
                    this.OpenFormByPatientData();
                }
                stopThread = false;
                ResetLoopCount();
            }
            catch (Exception ex)
            {
                stopThread = false;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        bool stopThread = false;
        private void ClosingForm()
        {
            try
            {
                if (HisConfigCFG.timeWaitingMilisecond > 0)
                {
                    bool time_out = false;
                    ResetLoopCount();
                    while (!time_out)
                    {
                        if (stopThread)
                        {
                            ResetLoopCount();
                        }
                        if (loopCount <= 0)
                        {
                            time_out = true;
                        }

                        System.Threading.Thread.Sleep(50);
                        loopCount--;
                    }

                    this.Invoke(new MethodInvoker(delegate () { this.Close(); }));
                    if (DelegateClose != null)
                    {
                        DelegateClose(null);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        int loopCount = HisConfigCFG.timeWaitingMilisecond / 50;
        private void ResetLoopCount()
        {
            try
            {
                this.loopCount = HisConfigCFG.timeWaitingMilisecond / 50;

                Inventec.Common.Logging.LogSystem.Info("ResetLoopCount");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}

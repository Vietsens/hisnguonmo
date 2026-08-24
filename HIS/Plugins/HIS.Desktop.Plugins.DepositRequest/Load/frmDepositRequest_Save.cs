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
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.DepositRequest.Config;
using HIS.Desktop.Plugins.DepositRequest.DepositRequest;
using HIS.Desktop.Print;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.SDO;
using MPS.ADO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.DepositRequest
{
    public partial class UCDepositRequest : UserControlBase
    {
        internal MOS.SDO.HisTransactionDepositSDO hisDepositSDO;
        MOS.EFMODEL.DataModels.V_HIS_TRANSACTION_5 hisTransaction { get; set; }

        private bool DelegateRunPrinter(string printTypeCode, string fileName)
        {
            bool result = false;
            try
            {
                if (depositReqPrint != null)
                {
                    InYeuCauTamUng(printTypeCode, fileName, depositReqPrint);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private void InYeuCauTamUng(string printTypeCode, string fileName, V_HIS_DEPOSIT_REQ depositReq)
        {
            try
            {
                bool result = false;

                WaitingManager.Show();

                //Thông tin bệnh nhân
                MPS.Processor.Mps000091.PDO.PatientADO mpspatient = new MPS.Processor.Mps000091.PDO.PatientADO();
                var patient = PrintGlobalStore.getPatient(currentdepositReq.TREATMENT_ID);
                if (patient != null)
                {
                    Inventec.Common.Mapper.DataObjectMapper.Map<MPS.Processor.Mps000091.PDO.PatientADO>(mpspatient, patient);
                }
                long instructionTime = Inventec.Common.DateTime.Get.Now() ?? 0;

                //BHYT
                //var patyAlterBhyt = PrintGlobalStore.getPatyAlterBhyt(currentdepositReq.TREATMENT_ID, instructionTime);
                MPS.Processor.Mps000091.PDO.PatyAlterBhytADO mpsPatyalterBHYT = new MPS.Processor.Mps000091.PDO.PatyAlterBhytADO();
                var patyAlterBhyt = new V_HIS_PATIENT_TYPE_ALTER();
                PrintGlobalStore.LoadCurrentPatientTypeAlter(currentdepositReq.TREATMENT_ID, instructionTime, ref patyAlterBhyt);
                if (patyAlterBhyt != null)
                {
                    Inventec.Common.Mapper.DataObjectMapper.Map<MPS.Processor.Mps000091.PDO.PatyAlterBhytADO>(mpsPatyalterBHYT, patyAlterBhyt);
                }
                WaitingManager.Hide();
                if (patient != null && depositReq != null)
                {
                    //Giường hiện tại của bệnh nhân - gán vào PatientADO để in mã/tên giường
                    CommonParam paramBed = new CommonParam();
                    MOS.Filter.HisTreatmentBedRoomViewFilter bedRoomFilter = new MOS.Filter.HisTreatmentBedRoomViewFilter();
                    bedRoomFilter.TREATMENT_ID = currentdepositReq.TREATMENT_ID;
                    bedRoomFilter.IS_ACTIVE = 1;
                    var treatmentBedRooms = new BackendAdapter(paramBed).Get<List<V_HIS_TREATMENT_BED_ROOM>>("api/HisTreatmentBedRoom/GetView", ApiConsumers.MosConsumer, bedRoomFilter, paramBed);
                    var treatmentBedRoom = treatmentBedRooms != null && treatmentBedRooms.Count > 0 ? treatmentBedRooms.OrderByDescending(o => o.ID).FirstOrDefault() : null;
                    if (treatmentBedRoom != null)
                    {
                        mpspatient.BED_CODE = treatmentBedRoom.BED_CODE;
                        mpspatient.BED_NAME = treatmentBedRoom.BED_NAME;
                    }

                    MPS.Processor.Mps000091.PDO.Mps000091PDO mps = new MPS.Processor.Mps000091.PDO.Mps000091PDO(
                    mpspatient,
                    depositReq,
                    mpsPatyalterBHYT);
                    MPS.ProcessorBase.Core.PrintData printData = null;

                    if (ConfigApplications.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        printData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, "");
                    }
                    else
                    {
                        printData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, "");
                    }
                    WaitingManager.Hide();
                    result = MPS.MpsPrinter.Run(printData);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InPhieuThuPhiDichVu(string printTypeCode, string fileName, ref bool result, bool isPrintNow)
        {
            try
            {
                CommonParam param = new CommonParam();
                MOS.Filter.HisSereServViewFilter sereServFilter = new MOS.Filter.HisSereServViewFilter();
                //xemlai...
                sereServFilter.TREATMENT_ID = hisTransaction.ID;

                MOS.Filter.HisSereServDepositFilter sereServDepositFiter = new MOS.Filter.HisSereServDepositFilter();
                sereServDepositFiter.DEPOSIT_ID = this.HisDeposit.ID;
                var sereServDeposit = new BackendAdapter(param).Get<List<HIS_SERE_SERV_DEPOSIT>>("api/HisSereServDeposit/Get", ApiConsumers.MosConsumer, sereServDepositFiter, param);
                var sereServIds = sereServDeposit.Select(o => o.SERE_SERV_ID).ToList();
                sereServFilter.IDs = sereServIds;
                var sereServs = new BackendAdapter(param).Get<List<V_HIS_SERE_SERV>>(HisRequestUriStore.HIS_SERE_SERV_GETVIEW, ApiConsumers.MosConsumer, sereServFilter, param);

                foreach (var item in sereServs)
                {
                    var itemCheck = sereServDeposit.FirstOrDefault(o => o.SERE_SERV_ID == item.ID);
                    if (itemCheck != null)
                    {
                        item.VIR_TOTAL_PATIENT_PRICE = itemCheck.AMOUNT;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SaveProcess(bool isSaveAndPrint)
        {
            try
            {
                CommonParam param = new CommonParam();
                bool success = false;
                positionHandleLeft = -1;
                if (!dxValidationProvider.Validate())
                    return;

                //Viec 54923: dong bo so tien tam ung vua sua vao yeu cau truoc khi tao giao dich
                if (!ProcessUpdateDepositReqAmount())
                    return;

                this.hisDepositSDO = null;
                UpdateDataFormTransactionDepositToDTO(ref this.hisDepositSDO, currentdepositReq);

                WaitingManager.Show();

                if (CheckValidForSave())
                {
                    this.HisDeposit = new Inventec.Common.Adapter.BackendAdapter(param).Post<MOS.EFMODEL.DataModels.V_HIS_TRANSACTION>(HisRequestUriStore.HIS_TRANSACTION_CREATE, ApiConsumers.MosConsumer, this.hisDepositSDO, param);
                    if (this.HisDeposit != null)
                    {
                        btnSavePrint.Enabled = false;
                        btnSave.Enabled = false;
                        btnPrint.Enabled = true;
                        success = true;
                        if (isSaveAndPrint)
                        {
                            this.isPrintNow = true;
                            Grid_PrintClick(currentdepositReq);
                        }
                        AddLastAccountToLocal();
                        var accountBook = ListAccountBook.FirstOrDefault(o => o.ID == Convert.ToInt64(cboAccountBook.EditValue));
                        UpdateDictionaryNumOrderAccountBook(accountBook);
                        loadAccountBook();
                        FillDataToGrid();
                        refreshDataAfterSave();
                        //SetDefaultAccountBookForUser();
                        //LoadDataToForm(listDepositReq[0]);
                        if (!SpNumOrder.Enabled)
                        {
                            SpNumOrder.EditValue = HisDeposit.NUM_ORDER;
                        }
                    }

                    MessageManager.Show(this.ParentForm, param, success);
                    SessionManager.ProcessTokenLost(param);
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Fatal(ex);
            }
        }

        private void AddLastAccountToLocal()
        {
            try
            {
                if (GlobalVariables.LastAccountBook == null) GlobalVariables.LastAccountBook = new List<V_HIS_ACCOUNT_BOOK>();

                var accountBook = ListAccountBook.FirstOrDefault(o => o.ID == Convert.ToInt64(cboAccountBook.EditValue));
                if (accountBook != null)
                {
                    //sổ chưa có sẽ add thêm vào
                    //xóa bỏ sổ cũ cùng loại
                    if (!GlobalVariables.LastAccountBook.Exists(o => o.ID == accountBook.ID))
                    {
                        var lstSameType = GlobalVariables.LastAccountBook.Where(o => o.IS_FOR_BILL == accountBook.IS_FOR_BILL && o.ID != accountBook.ID && o.BILL_TYPE_ID == accountBook.BILL_TYPE_ID).ToList();
                        if (lstSameType != null && lstSameType.Count > 0)
                        {
                            foreach (var item in lstSameType)
                            {
                                GlobalVariables.LastAccountBook.Remove(item);
                            }
                        }
                        GlobalVariables.LastAccountBook.Add(accountBook);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void UpdateDictionaryNumOrderAccountBook(V_HIS_ACCOUNT_BOOK accountBook)
        {
            try
            {
                if (HIS.Desktop.LocalStorage.LocalData.GlobalVariables.dicNumOrderInAccountBook != null && HIS.Desktop.LocalStorage.LocalData.GlobalVariables.dicNumOrderInAccountBook.Count > 0 && HIS.Desktop.LocalStorage.LocalData.GlobalVariables.dicNumOrderInAccountBook.ContainsKey(accountBook.ID))
                {
                    HIS.Desktop.LocalStorage.LocalData.GlobalVariables.dicNumOrderInAccountBook[accountBook.ID] = SpNumOrder.Value;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void refreshDataAfterSave()
        {
            if (this.sendResultToOtherForm != null)
            {
                this.sendResultToOtherForm(this.HisDeposit);
            }
        }

        private bool CheckValidForSave()
        {
            bool valid = true;
            this.positionHandleLeft = -1;
            try
            {
                valid = valid && (dxValidationProvider.Validate());
                valid = valid && (this.hisDepositSDO != null);
                if (valid)
                    if (this.hisDepositSDO.Transaction.AMOUNT == null)//|| this.hisDepositSDO.DereDetails.Count <= 0)
                    {
                        MessageManager.Show("Phòng thu ngân không thể sửa");
                        txtAccountBookCode.SelectAll();
                        txtAccountBookCode.Focus();
                        valid = false;
                    }

                if (valid && this.hisDepositSDO.Transaction.PAY_FORM_ID == IMSys.DbConfig.HIS_RS.HIS_PAY_FORM.ID__TMCK
                    && spinTransferAmount.EditValue != null && spinTransferAmount.Value > this.hisDepositSDO.Transaction.AMOUNT)
                {
                    decimal amountTransaction = this.hisDepositSDO.Transaction.AMOUNT;
                    WaitingManager.Hide();
                    MessageManager.Show(String.Format("Số tiền chuyển khoản [{0}] lớn hơn số tiền tạm ứng [{1}]",
                        Inventec.Common.Number.Convert.NumberToString(spinTransferAmount.Value, ConfigApplications.NumberSeperator),
                        Inventec.Common.Number.Convert.NumberToString(amountTransaction, ConfigApplications.NumberSeperator)));
                    spinTransferAmount.Focus();
                    spinTransferAmount.SelectAll();
                    valid = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return valid;
        }

        /// <summary>
        /// Viec 54923: thu ngan sua o "Số tiền" thi cap nhat lai HIS_DEPOSIT_REQ TRUOC khi tao giao dich.
        /// Backend chan Transaction.AMOUNT khac depositReq.AMOUNT (HisTransactionDepositCreate) nen phai dong bo truoc.
        /// </summary>
        private bool ProcessUpdateDepositReqAmount()
        {
            bool result = true;
            try
            {
                if (!HisConfigCFG.IsAllowEditAmount) return true;
                if (this.currentdepositReq == null) return true;
                if (this.currentdepositReq.DEPOSIT_ID != null) return true;

                decimal amountNew = 0;
                if (!TryGetEditAmount(out amountNew))
                {
                    MessageManager.Show("Số tiền tạm ứng không hợp lệ");
                    txtAmount.Focus();
                    txtAmount.SelectAll();
                    return false;
                }

                //Khong sua gi thi giu nguyen luong cu, khong goi API
                if (amountNew == this.currentdepositReq.AMOUNT) return true;

                if (amountNew <= 0)
                {
                    MessageManager.Show("Số tiền tạm ứng phải lớn hơn 0");
                    txtAmount.Focus();
                    txtAmount.SelectAll();
                    return false;
                }

                CommonParam param = new CommonParam();
                HIS_DEPOSIT_REQ dataUpdate = NewDepositReqForUpdate(this.currentdepositReq, amountNew);

                WaitingManager.Show();
                var dataResult = new BackendAdapter(param).Post<HIS_DEPOSIT_REQ>(HisRequestUriStore.HIS_DEPOSIT_REQ_UPDATE, ApiConsumers.MosConsumer, dataUpdate, param);
                WaitingManager.Hide();

                if (dataResult == null)
                {
                    MessageManager.Show(this.ParentForm, param, false);
                    SessionManager.ProcessTokenLost(param);
                    txtAmount.Focus();
                    txtAmount.SelectAll();
                    return false;
                }

                this.currentdepositReq.AMOUNT = dataResult.AMOUNT;
                this.currentdepositReq.MODIFY_TIME = dataResult.MODIFY_TIME;
                this.currentdepositReq.MODIFIER = dataResult.MODIFIER;
                txtAmount.EditValue = this.currentdepositReq.AMOUNT;
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
            return result;
        }

        /// <summary>
        /// Viec 54923: doc so tien tu o nhap (o dang mask so nen EditValue la kieu so).
        /// </summary>
        private bool TryGetEditAmount(out decimal amount)
        {
            amount = 0;
            try
            {
                object editValue = txtAmount.EditValue;
                if (editValue == null) return true;

                if (editValue is string)
                {
                    string text = ((string)editValue).Trim();
                    if (String.IsNullOrEmpty(text)) return true;
                    return decimal.TryParse(text, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.CurrentCulture, out amount);
                }

                amount = Convert.ToDecimal(editValue);
                return true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return false;
            }
        }

        /// <summary>
        /// Viec 54923: dung ban ghi HIS_DEPOSIT_REQ day du de goi api/HisDepositReq/Update (API cap nhat toan bo entity).
        /// </summary>
        private HIS_DEPOSIT_REQ NewDepositReqForUpdate(V_HIS_DEPOSIT_REQ data, decimal amount)
        {
            HIS_DEPOSIT_REQ result = new HIS_DEPOSIT_REQ();
            result.ID = data.ID;
            result.CREATE_TIME = data.CREATE_TIME;
            result.MODIFY_TIME = data.MODIFY_TIME;
            result.CREATOR = data.CREATOR;
            result.MODIFIER = data.MODIFIER;
            result.APP_CREATOR = data.APP_CREATOR;
            result.APP_MODIFIER = data.APP_MODIFIER;
            result.IS_ACTIVE = data.IS_ACTIVE;
            result.IS_DELETE = data.IS_DELETE;
            result.GROUP_CODE = data.GROUP_CODE;
            result.DEPOSIT_REQ_CODE = data.DEPOSIT_REQ_CODE;
            result.TREATMENT_ID = data.TREATMENT_ID;
            result.AMOUNT = amount;
            result.REQUEST_ROOM_ID = data.REQUEST_ROOM_ID;
            result.REQUEST_DEPARTMENT_ID = data.REQUEST_DEPARTMENT_ID;
            result.REQUEST_LOGINNAME = data.REQUEST_LOGINNAME;
            result.REQUEST_USERNAME = data.REQUEST_USERNAME;
            result.DESCRIPTION = data.DESCRIPTION;
            result.DEPOSIT_ID = data.DEPOSIT_ID;
            result.TRANS_REQ_ID = data.TRANS_REQ_ID;
            return result;
        }

        private void UpdateDataFormTransactionDepositToDTO(ref HisTransactionDepositSDO transactionData, V_HIS_DEPOSIT_REQ depo)
        {
            try
            {
                if (transactionData == null)
                {
                    transactionData = new HisTransactionDepositSDO();
                    //transactionData.Transaction = new HIS_DEPOSIT();
                    transactionData.Transaction = new HIS_TRANSACTION();
                }

                transactionData.SereServDeposits = new List<HIS_SERE_SERV_DEPOSIT>();
                transactionData.Transaction.AMOUNT = this.currentdepositReq.AMOUNT;
                var roomcash = BackendDataWorker.Get<V_HIS_CASHIER_ROOM>().FirstOrDefault(o => o.ROOM_ID == roomId);
                transactionData.Transaction.CASHIER_ROOM_ID = roomcash.ID;
                if (cboAccountBook.EditValue != null)
                {
                    var accountBook = ListAccountBook.FirstOrDefault(o => o.ID == Convert.ToInt64(cboAccountBook.EditValue));
                    if (accountBook != null)
                    {
                        transactionData.Transaction.ACCOUNT_BOOK_ID = accountBook.ID;
                    }

                    if (accountBook != null && accountBook.IS_NOT_GEN_TRANSACTION_ORDER == 1)
                    {
                        transactionData.Transaction.NUM_ORDER = (long)SpNumOrder.Value;
                    }
                    //transactionData.Transaction.ACCOUNT_BOOK_ID = Inventec.Common.TypeConvert.Parse.ToInt64((cboAccountBook.EditValue ?? "").ToString());
                }

                if (cboPayForm.EditValue != null)
                {
                    transactionData.Transaction.PAY_FORM_ID = (Inventec.Common.TypeConvert.Parse.ToInt64((cboPayForm.EditValue ?? "").ToString()));
                }

                //Viec 54923: luu so tien chuyen khoan khi hinh thuc thanh toan la "Tien mat/Chuyen khoan"
                transactionData.Transaction.TRANSFER_AMOUNT = null;
                if (transactionData.Transaction.PAY_FORM_ID == IMSys.DbConfig.HIS_RS.HIS_PAY_FORM.ID__TMCK && spinTransferAmount.EditValue != null)
                {
                    transactionData.Transaction.TRANSFER_AMOUNT = spinTransferAmount.Value;
                }

                if (depo != null)
                {
                    transactionData.Transaction.TREATMENT_ID = depo.TREATMENT_ID;
                }

                transactionData.Transaction.DESCRIPTION = txtDescription.Text;

                if (dtTransactionTime.DateTime != null && dtTransactionTime.DateTime != DateTime.MinValue)
                {
                    transactionData.Transaction.TRANSACTION_TIME = Inventec.Common.TypeConvert.Parse.ToInt64(dtTransactionTime.DateTime.ToString("yyyyMMddHHmm") + "00");
                }

                transactionData.DepositReqId = depo.ID;
                transactionData.RequestRoomId = this.roomId;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}

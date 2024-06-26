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
using DevExpress.XtraGrid;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.HisHoldReturn.ADO;
using HIS.Desktop.Plugins.HisHoldReturn.Config;
using HIS.Desktop.Plugins.HisHoldReturn.Resources;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Common.Logging;
using Inventec.Common.QrCodeBHYT;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.LibraryHein.Bhyt;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HIS.Desktop.Plugins.HisHoldReturn.HoldReturn
{
    public partial class UCHoldReturn : UserControlBase
    {
        /// <summary>
        /// Khoi Tao Du Lieu Bed Room
        /// </summary>
        private void FillDataToGridHoldReturn()
        {
            try
            {
                if (ucPaging1.pagingGrid != null)
                {
                    pageSize = ucPaging1.pagingGrid.PageSize;
                }
                else
                {
                    pageSize = (int)ConfigApplications.NumPageSize;
                }
                FillDataToGridHoldReturnPagging(new CommonParam(0, (int)pageSize));
                CommonParam param = new CommonParam();
                param.Limit = rowCount;
                param.Count = dataTotal;
                ucPaging1.Init(FillDataToGridHoldReturnPagging, param, pageSize, gridControlHoldReturn);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToGridHoldReturnPagging(object param)
        {
            try
            {
                WaitingManager.Show();
                List<V_HIS_HOLD_RETURN> lstHoldReturns = new List<V_HIS_HOLD_RETURN>();
                List<HoldReturnDataADO> lstHoldReturnADOs = new List<HoldReturnDataADO>();
                gridControlHoldReturn.DataSource = null;
                start = ((CommonParam)param).Start ?? 0;
                limit = ((CommonParam)param).Limit ?? 0;
                CommonParam paramCommon = new CommonParam(start, limit);

                HisHoldReturnViewFilter treatFilter = new MOS.Filter.HisHoldReturnViewFilter();
                SetFilter(ref treatFilter);
                Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => treatFilter), treatFilter));
                var resultRO = new Inventec.Common.Adapter.BackendAdapter(paramCommon).GetRO<List<V_HIS_HOLD_RETURN>>(RequestUriStore.HIS_HOLD_RETURN_GETVIEW, ApiConsumers.MosConsumer, treatFilter, paramCommon);
                if (resultRO != null)
                {
                    lstHoldReturns = (List<V_HIS_HOLD_RETURN>)resultRO.Data;
                    rowCount = (lstHoldReturns == null ? 0 : lstHoldReturns.Count);
                    dataTotal = (resultRO.Param == null ? 0 : resultRO.Param.Count ?? 0);

                    lstHoldReturnADOs = (from m in lstHoldReturns select new HoldReturnDataADO(m)).ToList();
                }

                lstHoldReturnADOs = lstHoldReturnADOs.OrderByDescending(o => o.HOLD_TIME).ThenBy(o => o.TDL_PATIENT_FIRST_NAME).ThenBy(o => o.TDL_PATIENT_LAST_NAME).ThenBy(o => o.TDL_PATIENT_CODE).ToList();
                gridControlHoldReturn.BeginUpdate();
                gridControlHoldReturn.DataSource = lstHoldReturnADOs;
                gridControlHoldReturn.EndUpdate();
                gridViewHoldReturn.BestFitColumns();
                Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => lstHoldReturnADOs), lstHoldReturnADOs));
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                gridControlHoldReturn.EndUpdate();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetFilter(ref HisHoldReturnViewFilter holdReturnFilter)
        {
            try
            {
                holdReturnFilter = holdReturnFilter == null ? new MOS.Filter.HisHoldReturnViewFilter() : holdReturnFilter;
                holdReturnFilter.ORDER_DIRECTION = "ASC";
                holdReturnFilter.ORDER_FIELD = "FIRST_NAME";//TODO   

                if (!String.IsNullOrEmpty(txtPatientCodeForSearch.Text))
                {
                    string str = string.Format("{0:0000000000}", Convert.ToInt64(this.txtPatientCodeForSearch.Text));
                    this.txtPatientCodeForSearch.Text = str;
                    holdReturnFilter.PATIENT_CODE__EXACT = this.txtPatientCodeForSearch.Text;
                }
                if (!string.IsNullOrEmpty(txtTreatmentCode.Text))
                {
                    string codeTreatment = txtTreatmentCode.Text.Trim();
                    if (codeTreatment.Length < 12)
                    {
                        codeTreatment = string.Format("{0:000000000000}", Convert.ToInt64(codeTreatment));
                        txtTreatmentCode.Text = codeTreatment;
                    }
                }
               

                if (cboHandoverForSearch.EditValue != null)
                {
                    //Tất cả
                    if ((long)cboHandoverForSearch.EditValue == 1)
                    {

                    }
                    //Đang bàn giao
                    else if ((long)cboHandoverForSearch.EditValue == 2)
                    {
                        holdReturnFilter.IS_HANDOVERING = true;
                    }
                    //Đã bàn giao
                    else if ((long)cboHandoverForSearch.EditValue == 3)
                    {
                        holdReturnFilter.IS_HANDOVERING = false;
                    }
                    //Chưa trả
                    else if ((long)cboHandoverForSearch.EditValue == 4)
                    {
                        holdReturnFilter.HAS_RETURN_TIME = false;
                    }
                    //Đã trả
                    else if ((long)cboHandoverForSearch.EditValue == 5)
                    {
                        holdReturnFilter.HAS_RETURN_TIME = true;
                    }
                }
                holdReturnFilter.WORKING_ROOM_ID = requestRoom.ID;//TODO  phòng giữ hoặc phòng chịu trách nhiệm là phòng làm việc
                if (!string.IsNullOrEmpty(txtKeywordForSearch.Text))
                {
                    holdReturnFilter.KEY_WORD = txtKeywordForSearch.Text;
                }
                if (!string.IsNullOrEmpty(txtTreatmentCode.Text))
                {
                    holdReturnFilter.TREATMENT_CODE__EXACT = txtTreatmentCode.Text;
                }
                
                if (!string.IsNullOrEmpty(cboDocHoldTypeForSearch.EditValue.ToString()) && !string.IsNullOrWhiteSpace(cboDocHoldTypeForSearch.EditValue.ToString()))
                {
                    holdReturnFilter.DOC_HOLD_TYPE_IDs = new List<long>() { (long)cboDocHoldTypeForSearch.EditValue };
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void HoldReturnRowClick()
        {
            try
            {
                this.ResetStateControlForm();

               
               
                this.currentHoldReturn = (HoldReturnDataADO)this.gridViewHoldReturn.GetFocusedRow();


                HisTreatmentFilter treatmentFilter = new HisTreatmentFilter();
                treatmentFilter.ORDER_DIRECTION = "DESC";
                treatmentFilter.ORDER_FIELD = "IN_TIME";
                treatmentFilter.TDL_PATIENT_CODE__EXACT = this.currentHoldReturn.TDL_PATIENT_CODE;
                CommonParam paramCommon = new CommonParam();
                paramCommon.Limit = 1;
                paramCommon.Start = 0;
                this.currentTreatment = new Inventec.Common.Adapter.BackendAdapter(paramCommon).Get<List<HIS_TREATMENT>>(RequestUriStore.HIS_TREATMENT_GET, ApiConsumers.MosConsumer, treatmentFilter, paramCommon).FirstOrDefault();
               
                this.currentPatientId = this.currentHoldReturn.PATIENT_ID;
                this.txtPatientCodeForAdd.Text = this.currentHoldReturn.TDL_PATIENT_CODE;
                this.lblPatientName.Text = this.currentHoldReturn.TDL_PATIENT_NAME;
                this.lblGenderName.Text = this.currentHoldReturn.TDL_PATIENT_GENDER_NAME;
                this.lblPatientAddress.Text = this.currentHoldReturn.TDL_PATIENT_ADDRESS;
                this.lblHeinCardNumber.Text = this.currentHoldReturn.HEIN_CARD_NUMBER;
                this.lblPatientDob.Text = Inventec.Common.DateTime.Convert.TimeNumberToDateString(this.currentHoldReturn.TDL_PATIENT_DOB);
                var roomHold = BackendDataWorker.Get<V_HIS_ROOM>().Where(o => o.ID == this.currentHoldReturn.HOLD_ROOM_ID).FirstOrDefault();
                this.lblHandoverRoom.Text = roomHold != null ? roomHold.ROOM_NAME : "";
                this.dateHoldTime.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(this.currentHoldReturn.HOLD_TIME);
                this.dateHoldTime.Enabled = true;
                txtIFTreatmentCode.Text = this.currentHoldReturn.TREATMENT_CODE;
                this.EnableButtonByData((this.currentHoldReturn.IS_HANDOVERING == null || this.currentHoldReturn.IS_HANDOVERING != GlobalVariables.CommonNumberTrue) && this.currentHoldReturn.CREATOR == Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName() && this.currentHoldReturn.HOLD_ROOM_ID == currentModule.RoomId && this.currentHoldReturn.RESPONSIBLE_ROOM_ID == currentModule.RoomId);
                this.actionType = GlobalVariables.ActionEdit;
                //TODO
                //Load du lieu tu bang HIS_HOLD_DHTY theo HOLD_RETURN_ID
                // Check cac dong tai cac ID tuong ung co trong bang da tim thay o tren

                gridControlDocType.BeginUpdate();
                if (!String.IsNullOrEmpty(this.currentHoldReturn.DOC_HOLD_TYPE_IDS))
                {
                    var listIdChecks = GetDocHoldTypeIdFromData(this.currentHoldReturn.DOC_HOLD_TYPE_IDS);
                    if (listIdChecks != null && listIdChecks.Count > 0)
                    {
                        this.currentDocHoldTypeSelecteds = BackendDataWorker.Get<HIS_DOC_HOLD_TYPE>().Where(o => listIdChecks.Contains(o.ID)).ToList();

                        foreach (var item in listIdChecks)
                        {
                            int rowHandle = gridViewDocType.LocateByValue("ID", item);
                            if (rowHandle != GridControl.InvalidRowHandle)
                                gridViewDocType.SelectRow(rowHandle);
                        }
                    }
                }
                else
                {
                    gridViewDocType.ClearSelection();
                }
                gridControlDocType.EndUpdate();
            }
            catch (Exception ex)
            {
                gridControlDocType.EndUpdate();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        List<long> GetDocHoldTypeIdFromData(string ids)
        {
            List<long> returnIds = new List<long>();
            if (!String.IsNullOrEmpty(ids))
            {
                var arrIds = ids.Split(new string[] { ",", ";" }, StringSplitOptions.RemoveEmptyEntries);
                if (arrIds != null && arrIds.Count() > 0)
                {
                    foreach (var item in arrIds)
                    {
                        long idHRT = Inventec.Common.TypeConvert.Parse.ToInt64(item);
                        if (idHRT > 0)
                            returnIds.Add(idHRT);
                    }
                }
            }

            return returnIds;
        }

        private void ProcessForGetDataQrCodeHeinCard(string qrCode)
        {
            HeinCardData dataHein = null;
            try
            {
                //Lay thong tin tren th BHYT cua benh nhan khi quet the doc chuoi qrcode
                ReadQrCodeHeinCard readQrCode = new ReadQrCodeHeinCard();
                dataHein = readQrCode.ReadDataQrCode(qrCode);

                if (dataHein.HeinCardNumber.Length > 15)
                    dataHein.HeinCardNumber = dataHein.HeinCardNumber.Substring(0, 15);

                BhytHeinProcessor _BhytHeinProcessor = new BhytHeinProcessor();
                if (!_BhytHeinProcessor.IsValidHeinCardNumber(dataHein.HeinCardNumber))
                {
                    MessageManager.Show("Mã QR không hợp lệ. Vui lòng kiểm tra lại.");
                    Inventec.Common.Logging.LogSystem.Debug("Ma qrcode khong hop le, So the bhyt khong hop le, HeinCardNumber= " + dataHein.HeinCardNumber + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => qrCode), qrCode));
                    return;
                }
                CommonParam param = new CommonParam();
                HisPatientAdvanceFilter filter = new HisPatientAdvanceFilter();
                filter.HEIN_CARD_NUMBER__EXACT = dataHein.HeinCardNumber;
                var patients = (new BackendAdapter(param).Get<List<HisPatientSDO>>(RequestUriStore.HIS_PATIENT_GETSDOADVANCE, ApiConsumers.MosConsumer, filter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param));
                if (patients != null && patients.Count > 0)
                {
                    if (patients.Count > 1)
                    {
                        LogSystem.Debug("Quet the BHYT tim thay " + patients.Count + " benh nhan cu => mo form chon benh nhan => chon 1 => fill du lieu bn duoc chon.");
                        frmPatientChoice frm = new frmPatientChoice(patients, this.SelectOnePatientProcess);
                        frm.ShowDialog();
                    }
                    else
                    {
                        LogSystem.Debug("Quet the BHYT tim thay thong tin bhyt cua benh nhan cu theo so the HeinCardNumber = " + dataHein.HeinCardNumber + ". " + LogUtil.TraceData("HisPatientSDO searched", patients[0]));
                        //An hien cac button lam moi thong tin benh nhan
                        this.SelectOnePatientProcess(patients[0]);
                    }
                }
                else
                {
                    Inventec.Common.Logging.LogSystem.Debug("Không tìm thấy bệnh nhân có số thẻ bhyt tương ứng");
                    MessageManager.Show("Không tìm thấy bệnh nhân có số thẻ bhyt tương ứng____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => dataHein), dataHein));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SelectOnePatientProcess(HisPatientSDO patientSDO)
        {
            try
            {
                this.currentTreatmentId = patientSDO.TreatmentId;
                this.currentPatientId = patientSDO.ID;
                this.txtPatientCodeForAdd.Text = patientSDO.PATIENT_CODE;
                this.lblPatientName.Text = patientSDO.VIR_PATIENT_NAME;
                MOS.EFMODEL.DataModels.HIS_GENDER gioitinh = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_GENDER>().SingleOrDefault(o => o.ID == patientSDO.GENDER_ID);
                this.lblGenderName.Text = gioitinh != null ? gioitinh.GENDER_NAME : "";
                this.lblPatientAddress.Text = patientSDO.VIR_ADDRESS;
                this.lblHeinCardNumber.Text = patientSDO.TDL_HEIN_CARD_NUMBER;
                this.lblPatientDob.Text = Inventec.Common.DateTime.Convert.TimeNumberToDateString(patientSDO.DOB);

                HisHoldReturnViewFilter holdReturnFilter = new HisHoldReturnViewFilter();
                holdReturnFilter.PATIENT_ID = this.currentPatientId;
                //holdReturnFilter.HOLD_ROOM_ID = this.currentModule.RoomId;
                //holdReturnFilter.RESPONSIBLE_ROOM_ID = this.currentModule.RoomId;
                holdReturnFilter.IS_HANDOVERING = false;//TODO
                CommonParam paramCommon = new CommonParam();
                var holdReturns = new Inventec.Common.Adapter.BackendAdapter(paramCommon).Get<List<V_HIS_HOLD_RETURN>>(RequestUriStore.HIS_HOLD_RETURN_GETVIEW, ApiConsumers.MosConsumer, holdReturnFilter, paramCommon);

                if (holdReturns != null && holdReturns.Count > 0)
                {
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => holdReturns), holdReturns));
                    var roomHold = BackendDataWorker.Get<V_HIS_ROOM>().Where(o => o.ID == holdReturns[0].HOLD_ROOM_ID).FirstOrDefault();
                    this.lblHandoverRoom.Text = roomHold != null ? roomHold.ROOM_NAME : "";
                    this.btnSave.Enabled = false;
                }
                else
                {
                    this.btnSave.Enabled = true;
                    this.gridViewDocType.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ResetStateControlForm()
        {
            try
            {
                this.actionType = GlobalVariables.ActionAdd;
                this.txtPatientCodeForAdd.Text = "";
                this.lblPatientName.Text = "";
                this.lblGenderName.Text = "";
                this.lblPatientAddress.Text = "";
                this.lblHeinCardNumber.Text = "";
                this.lblPatientDob.Text = "";
                this.lblHandoverRoom.Text = "";
                this.dateHoldTime.EditValue = null;
                this.dateHoldTime.Enabled = false;
                this.txtIFTreatmentCode.Text = "";
                this.gridViewDocType.BeginSelection();
                this.gridViewDocType.ClearSelection();
                this.gridViewDocType.EndSelection();

                this.currentDocHoldTypeSelecteds = new List<HIS_DOC_HOLD_TYPE>();
                this.currentHoldReturn = null;
                this.currentTreatment = null;
                this.currentPatientId = 0;
                this.currentTreatmentId = 0;
                this.ResetRequiredField();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToControlsForm()
        {
            try
            {
                this.InitComboHoldDhty(cboDocHoldTypeForSearch);
                this.InitComboBanGiao(cboHandoverForSearch);
                this.InitGridHoldDhyt();
                FillDataToControlHR(this.currentTreatment, this.currentDocHoldTypeSelecteds);
                this.ValidateForm();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

    }
}

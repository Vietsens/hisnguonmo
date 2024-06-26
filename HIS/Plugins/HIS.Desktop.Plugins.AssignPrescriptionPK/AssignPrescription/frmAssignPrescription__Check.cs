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
using DevExpress.XtraEditors.DXErrorProvider;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Plugins.AssignPrescriptionPK.ADO;
using HIS.Desktop.Plugins.AssignPrescriptionPK.Config;
using HIS.Desktop.Plugins.AssignPrescriptionPK.MessageBoxForm;
using HIS.Desktop.Plugins.AssignPrescriptionPK.Resources;
using Inventec.Common.Adapter;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.AssignPrescriptionPK.AssignPrescription
{
    public partial class frmAssignPrescription : HIS.Desktop.Utility.FormBase
    {
        public OptionChonThuocThayThe ChonThuocThayThe { get; set; }
        public string reasonMaxPrescription;
        public string reasonMaxPrescriptionDay;
        public string reasonOddPrescription;

        private bool CheckExistMedicinePaymentLimit(string medicineTypeCode)
        {
            bool result = false;
            try
            {
                string medicePaymentLimit = HisConfigCFG.MedicineHasPaymentLimitBHYT.ToLower();
                if (!String.IsNullOrEmpty(medicePaymentLimit))
                {
                    string[] medicineArr = medicePaymentLimit.Split(',');
                    if (medicineArr != null && medicineArr.Length > 0)
                    {
                        if (medicineArr.Contains(medicineTypeCode.ToLower()))
                        {
                            return true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }


        /// <summary>
        /// Sửa chức năng "Kê đơn": #36634
        ///- Hiện tại nếu kê quá số lượng cho phép kê trên 1 đơn thì hiển thị cảnh báo.
        ///- Sửa lại nếu kê quá số lượng cho phép kê trên 1 đơn thì kiểm tra trường "Chặn hay cảnh báo" (IS_BLOCK_MAX_IN_PRESCRIPTION) của loại thuốc đang kê:
        ///+ Nếu bằng 1 thì chặn không cho phép kê có hiển thị thông báo.
        ///+ Nếu khác 1 thì thực hiện cảnh báo.
        ///- Nếu sửa số lượng ở danh sách thuốc đã bổ sung thì kiểm tra thuốc số lượng có vượt quá số lương cho phép trên 1 đơn không. Nếu có thì kiểm tra trường "Chặn hay cảnh báo" (IS_BLOCK_MAX_IN_PRESCRIPTION) của loại thuốc đang kê:
        ///+ Nếu bằng 1 thì chặn không cho phép kê có hiển thị thông báo.
        ///+ Nếu khác 1 thì thực hiện cảnh báo.
        /// </summary>
        /// <param name="mediMaTy"></param>
        /// <param name="_amount"></param>
        /// <returns></returns>
        private bool CheckMaxInPrescription(MediMatyTypeADO mediMaTy, decimal? _amount)
        {
            bool result = true;
            try
            {
                //reasonMaxPrescription = "";
                decimal? amount = null;
                if (mediMaTy != null && mediMaTy.ALERT_MAX_IN_PRESCRIPTION.HasValue)
                {
                    List<MediMatyTypeADO> mediMatyTypeADOs = gridControlServiceProcess.DataSource as List<MediMatyTypeADO>;
                    if (mediMatyTypeADOs == null)
                        mediMatyTypeADOs = new List<MediMatyTypeADO>();
                    amount = mediMatyTypeADOs.Where(o => o.ID == mediMaTy.ID && o.PrimaryKey != mediMaTy.PrimaryKey).Sum(o => o.AMOUNT)
                        + _amount;

                    if (amount > mediMaTy.ALERT_MAX_IN_PRESCRIPTION.Value)
                    {
                        Inventec.Common.Logging.LogSystem.Debug("CheckMaxInPrescription. Ke don kiem tra so luong ke lơn hon so luong canh bao ALERT_MAX_IN_PRESCRIPTION____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => mediMaTy), mediMaTy) + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => mediMaTy), mediMaTy));
                        string notice = "";
                        if (mediMaTy.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC)
                        {

                            if (mediMaTy.IS_BLOCK_MAX_IN_PRESCRIPTION == 1)
                            {
                                notice = ResourceMessage.ThuocXKeQuaSoLuongChoPhepCoChan;
                                MessageBox.Show(String.Format(notice, mediMaTy.MEDICINE_TYPE_NAME, amount - mediMaTy.ALERT_MAX_IN_PRESCRIPTION.Value, mediMaTy.SERVICE_UNIT_NAME), HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao));
                                return false;
                            }
                            notice = ResourceMessage.ThuocXKeQuaSoLuongChoPhepBanCoMuonBoSung;
                        }
                        else if (mediMaTy.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__VT)
                        {
                            notice = ResourceMessage.VatTuXKeQuaSoLuongChoPhepBanCoMuonBoSung;
                        }
                        frmReasonMaxPrescription FrmMessage = new frmReasonMaxPrescription(String.Format(notice, mediMaTy.MEDICINE_TYPE_NAME, amount - mediMaTy.ALERT_MAX_IN_PRESCRIPTION.Value, mediMaTy.SERVICE_UNIT_NAME), GetReasonMaxPrescription, mediMaTy.EXCEED_LIMIT_IN_PRES_REASON);
                        FrmMessage.ShowDialog();
                        if (String.IsNullOrEmpty(this.reasonMaxPrescription))
                        {
                            result = false;
                        }
                    }
                    else
                        this.reasonMaxPrescription = null;
                }
                else
                    this.reasonMaxPrescription = null;
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }
        private bool CheckMaxInPrescriptionForMemoReason(MediMatyTypeADO mediMaTy, decimal? _amount)
        {
            bool result = true;
            try
            {
                decimal? amount = null;
                if (mediMaTy != null && mediMaTy.ALERT_MAX_IN_PRESCRIPTION.HasValue && (mediMaTy.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC || mediMaTy.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__VT))
                {
                    List<MediMatyTypeADO> mediMatyTypeADOs = gridControlServiceProcess.DataSource as List<MediMatyTypeADO>;
                    if (mediMatyTypeADOs == null)
                        mediMatyTypeADOs = new List<MediMatyTypeADO>();
                    amount = mediMatyTypeADOs.Where(o => o.ID == mediMaTy.ID && o.PrimaryKey != mediMaTy.PrimaryKey).Sum(o => o.AMOUNT)
                        + _amount;
                    if (amount > mediMaTy.ALERT_MAX_IN_PRESCRIPTION.Value)
                    {
                        result = false;
                    }
                }
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }
        private bool CheckMaxInPrescriptionWhenSave(List<MediMatyTypeADO> mediMatyTypeADOs)
        {
            bool result = true;
            try
            {
                if (mediMatyTypeADOs != null && mediMatyTypeADOs.Count > 0)
                {
                    List<string> medicineTypeNames = new List<string>();
                    var listmediMatyAlert = mediMatyTypeADOs.Where(o => o.ALERT_MAX_IN_PRESCRIPTION != null);
                    if (listmediMatyAlert != null && listmediMatyAlert.Count() > 0)
                    {
                        var mediMatyTypeGroup = listmediMatyAlert.GroupBy(o => new { o.SERVICE_ID }).ToList();
                        foreach (var item in mediMatyTypeGroup)
                        {
                            var mediMaty = item.FirstOrDefault(o => !String.IsNullOrWhiteSpace(o.EXCEED_LIMIT_IN_PRES_REASON));
                            if (mediMaty == null)
                            {
                                mediMaty = item.First();
                                var amount = item.Sum(o => o.AMOUNT);
                                if (mediMaty.ALERT_MAX_IN_PRESCRIPTION != null && amount > mediMaty.ALERT_MAX_IN_PRESCRIPTION.Value && String.IsNullOrWhiteSpace(mediMaty.EXCEED_LIMIT_IN_PRES_REASON))
                                {
                                    medicineTypeNames.Add(mediMaty.MEDICINE_TYPE_NAME);
                                }
                            }
                        }
                    }
                    if (medicineTypeNames != null && medicineTypeNames.Count > 0)
                    {
                        result = false;
                        MessageBox.Show(String.Format(ResourceMessage.ThuocVatTuChuaNhapLyDoKeQuaSoLuongToiDaTrongDon, String.Join(",", medicineTypeNames)), HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao));
                    }

                }

            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }
        private bool CheckMaxInPrescriptionInDay(MediMatyTypeADO mediMaTy, decimal? _amount)
        {
            bool result = true;
            try
            {
                decimal? amount = null;
                decimal amountPrescribed = 0;
                if (mediMaTy != null && mediMaTy.ALERT_MAX_IN_DAY.HasValue)
                {
                    amountPrescribed = GetAmountPrescriptionInDay(mediMaTy);
                    List<MediMatyTypeADO> mediMatyTypeADOs = gridControlServiceProcess.DataSource as List<MediMatyTypeADO>;
                    if (mediMatyTypeADOs == null)
                        mediMatyTypeADOs = new List<MediMatyTypeADO>();

                    amount = amountPrescribed + mediMatyTypeADOs.Where(o => o.ID == mediMaTy.ID && o.PrimaryKey != mediMaTy.PrimaryKey).Sum(o => o.UseDays != 0 ? o.AMOUNT/o.UseDays : o.AMOUNT)
                        + _amount;

                    if (amount > mediMaTy.ALERT_MAX_IN_DAY.Value)
                    {
                        string notice = "";
                        if (mediMaTy.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC)
                        {
                            if (mediMaTy.IS_BLOCK_MAX_IN_DAY == 1)
                            {
                                notice = ResourceMessage.ThuocXKeQuaSoLuongChoPhepCoChan;
                                MessageBox.Show(String.Format(notice, mediMaTy.MEDICINE_TYPE_NAME, mediMaTy.ALERT_MAX_IN_DAY.Value, mediMaTy.SERVICE_UNIT_NAME), HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao));
                                return false;
                            }
                            notice = ResourceMessage.ThuocXKeQuaSoLuongChoPhepTrongNgayBanCoMuonBoSung;
                        }
                        else if (mediMaTy.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__VT)
                        {
                            notice = ResourceMessage.VatTuXKeQuaSoLuongChoPhepTrongNgayBanCoMuonBoSung;
                        }
                        frmReasonMaxPrescriptionInDay FrmMessage = new frmReasonMaxPrescriptionInDay(String.Format(notice, mediMaTy.MEDICINE_TYPE_NAME, mediMaTy.ALERT_MAX_IN_DAY.Value, mediMaTy.SERVICE_UNIT_NAME), GetReasonMaxPrescriptionInDay, mediMaTy.EXCEED_LIMIT_IN_DAY_REASON);
                        FrmMessage.ShowDialog();
                        if (String.IsNullOrEmpty(this.reasonMaxPrescriptionDay))
                        {
                            result = false;
                        }
                    }
                    else
                        this.reasonMaxPrescriptionDay = null;
                }
                else
                    this.reasonMaxPrescriptionDay = null;
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }
        private bool CheckMaxInPrescriptionInDayForMemoReason(MediMatyTypeADO mediMaTy, decimal? _amount)
        {
            bool result = true;
            try
            {
                decimal? amount = null;
                decimal amountPrescribed = 0;
                if (mediMaTy != null && mediMaTy.ALERT_MAX_IN_DAY.HasValue && (mediMaTy.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC || mediMaTy.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__VT))
                {
                    amountPrescribed = GetAmountPrescriptionInDay(mediMaTy);
                    List<MediMatyTypeADO> mediMatyTypeADOs = gridControlServiceProcess.DataSource as List<MediMatyTypeADO>;
                    if (mediMatyTypeADOs == null)
                        mediMatyTypeADOs = new List<MediMatyTypeADO>();

                    amount = amountPrescribed + mediMatyTypeADOs.Where(o => o.ID == mediMaTy.ID && o.PrimaryKey != mediMaTy.PrimaryKey).Sum(o => o.AMOUNT)
                        + _amount;

                    if (mediMaTy.IS_BLOCK_MAX_IN_DAY != 1 && amount > mediMaTy.ALERT_MAX_IN_DAY.Value)
                    {
                        result = false;
                    }
                }
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }
        private bool CheckMaxInPrescriptionInDayWhenSave(List<MediMatyTypeADO> mediMatyTypeADOs)
        {
            bool result = true;
            try
            {
                if (mediMatyTypeADOs != null && mediMatyTypeADOs.Count > 0)
                {
                    List<string> medicineTypeNames = new List<string>();
                    List<string> blockMedicineTypeNames = new List<string>();
                    var listmediMatyAlert = mediMatyTypeADOs.Where(o => o.ALERT_MAX_IN_DAY != null && (o.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC || o.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__VT));
                    if (listmediMatyAlert != null && listmediMatyAlert.Count() > 0)
                    {
                        var listSereServ = this.sereServWithTreatment.Where(o => o.TDL_INTRUCTION_DATE.ToString().Substring(0, 8) == intructionTimeSelecteds.OrderByDescending(t => t).First().ToString().Substring(0, 8));
                        if (assignPrescriptionEditADO != null && this.oldServiceReq != null && listSereServ != null)
                        {
                            listSereServ = listSereServ.Where(o => o.SERVICE_REQ_ID != this.oldServiceReq.ID).ToList();
                        }
                        var mediMatyTypeGroup = listmediMatyAlert.GroupBy(o => new { o.SERVICE_ID }).ToList();
                        foreach (var item in mediMatyTypeGroup)
                        {
                            var mediMaty = item.FirstOrDefault(o => !String.IsNullOrWhiteSpace(o.EXCEED_LIMIT_IN_DAY_REASON));
                            if (mediMaty == null)
                            {
                                decimal amountPrescribed = 0;
                                mediMaty = item.First();
                                amountPrescribed = GetAmountPrescriptionInDay(mediMaty);
                                var amount = amountPrescribed + item.Sum(o => o.UseDays != 0 ? o.AMOUNT / o.UseDays : o.AMOUNT);
                                if (amount > mediMaty.ALERT_MAX_IN_DAY.Value)
                                {
                                    if (mediMaty.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC && mediMaty.IS_BLOCK_MAX_IN_DAY == 1)
                                    {
                                        blockMedicineTypeNames.Add(mediMaty.MEDICINE_TYPE_NAME);
                                        continue;
                                    }
                                    if (String.IsNullOrWhiteSpace(mediMaty.EXCEED_LIMIT_IN_DAY_REASON))
                                    {
                                        medicineTypeNames.Add(mediMaty.MEDICINE_TYPE_NAME);
                                    }
                                }

                            }
                        }
                    }
                    if (blockMedicineTypeNames != null && blockMedicineTypeNames.Count > 0)
                    {
                        MessageBox.Show(String.Format(ResourceMessage.DsThuocKeQuaSoLuongChoPhepCoChan, String.Join(",", blockMedicineTypeNames)), HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao));
                        return false;
                    }
                    if (medicineTypeNames != null && medicineTypeNames.Count > 0)
                    {
                        result = false;
                        MessageBox.Show(String.Format(ResourceMessage.ThuocVatTuChuaNhapLyDoKeQuaSoLuongToiDaTrongNgay, String.Join(",", medicineTypeNames)), HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao));
                    }

                }

            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }
        private HIS_EXP_MEST_MEDICINE Medicine(long serviceReqId)
        {
            HIS_EXP_MEST_MEDICINE medicine = new HIS_EXP_MEST_MEDICINE();
            HisExpMestMedicineFilter filter = new HisExpMestMedicineFilter();
            filter.TDL_SERVICE_REQ_ID = serviceReqId;
            var lstMedicine = new BackendAdapter(new Inventec.Core.CommonParam()).Get<List<HIS_EXP_MEST_MEDICINE>>("api/HisExpMestMedicine/Get", ApiConsumers.MosConsumer, filter, ProcessLostToken, null);
            if (lstMedicine != null && lstMedicine.Count > 0)
                medicine = lstMedicine.FirstOrDefault();
            return medicine;
        }
        private HIS_SERVICE_REQ_METY Mety(long serviceReqId)
        {
            HIS_SERVICE_REQ_METY mety = new HIS_SERVICE_REQ_METY();
            HisServiceReqMetyFilter filter = new HisServiceReqMetyFilter();
            filter.SERVICE_REQ_ID = serviceReqId;
            var lstMedicine = new BackendAdapter(new Inventec.Core.CommonParam()).Get<List<HIS_SERVICE_REQ_METY>>("api/HisServiceReqMety/Get", ApiConsumers.MosConsumer, filter, ProcessLostToken, null);
            if (lstMedicine != null && lstMedicine.Count > 0)
                mety = lstMedicine.FirstOrDefault();
            return mety;
        }
        private decimal GetAmountPrescriptionInDay(MediMatyTypeADO mediMaTy)
        {
            decimal amountPrescribed = 0;
            try
            {
                if (this.sereServWithTreatment != null && this.sereServWithTreatment.Count > 0)
                {
                    if (ucPeriousExpMestList != null && periousExpMestListProcessor != null && this.sereServWithTreatment != null)
                    {
                        serviceReqPreExpmestAll = this.periousExpMestListProcessor.GetServiceReqDataAll(this.ucPeriousExpMestList);
                    }
                    var listSereServ = this.sereServWithTreatment.Where(o => o.SERVICE_ID == mediMaTy.SERVICE_ID
                        && o.TDL_INTRUCTION_DATE.ToString().Substring(0, 8) == intructionTimeSelecteds.OrderByDescending(t => t).First().ToString().Substring(0, 8));
                    if (this.oldServiceReq != null && listSereServ != null)
                    {
                        listSereServ = listSereServ.Where(o => o.SERVICE_REQ_ID != this.oldServiceReq.ID);
                    }
                    if (listSereServ != null)
                    {
                        foreach (var item in listSereServ)
                        {
                            var sr = serviceReqPreExpmestAll.FirstOrDefault(o => o.ID == item.SERVICE_REQ_ID);
                            if (!sr.USE_TIME_TO.HasValue)
                                sr.USE_TIME_TO = Medicine(sr.ID).USE_TIME_TO;
                            int useDays = 1;
                            if (sr.USE_TIME_TO.HasValue)
                            {
                                System.DateTime dtUseTimeTo = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(sr.USE_TIME_TO ?? 0).Value;
                                System.DateTime dtInstructionTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(sr.USE_TIME > 0 ? (sr.USE_TIME ?? 0) : (sr.INTRUCTION_TIME)).Value;
                                TimeSpan diff__Day = (dtUseTimeTo - dtInstructionTime);
                                useDays = diff__Day.Days + 1;
                            }
                            amountPrescribed += item.AMOUNT / useDays;
                        }
                    }
                }

                if (mediMaTy.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC && this.serviceReqMetyInDay != null && this.serviceReqMetyInDay.Count > 0)
                {
                    var serviceReqMety = this.serviceReqMetyInDay.Where(o => o.MEDICINE_TYPE_ID == mediMaTy.ID);
                    if (this.oldServiceReq != null && serviceReqMety != null)
                    {
                        serviceReqMety = serviceReqMety.Where(o => o.SERVICE_REQ_ID != this.oldServiceReq.ID);
                    }
                    if (serviceReqMety != null)
                    {
                        foreach (var item in serviceReqMety)
                        {
                            var sr = serviceReqPreExpmestAll.FirstOrDefault(o => o.ID == item.SERVICE_REQ_ID);
                            if (!sr.USE_TIME_TO.HasValue)
                                sr.USE_TIME_TO = Mety(sr.ID).USE_TIME_TO;
                            int useDays = 1;
                            if (sr.USE_TIME_TO.HasValue)
                            {
                                System.DateTime dtUseTimeTo = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(sr.USE_TIME_TO ?? 0).Value;
                                System.DateTime dtInstructionTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(sr.USE_TIME > 0 ? (sr.USE_TIME ?? 0) : (sr.INTRUCTION_TIME)).Value;
                                TimeSpan diff__Day = (dtUseTimeTo - dtInstructionTime);
                                useDays = diff__Day.Days + 1;
                            }
                            amountPrescribed += item.AMOUNT / useDays;
                        }
                    }    
                }
                else if (mediMaTy.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__VT && this.serviceReqMatyInDay != null && this.serviceReqMatyInDay.Count > 0)
                {
                    var serviceReqMaty = this.serviceReqMatyInDay.Where(o => o.MATERIAL_TYPE_ID == mediMaTy.ID);
                    if (this.oldServiceReq != null && serviceReqMaty != null)
                    {
                        serviceReqMaty = serviceReqMaty.Where(o => o.SERVICE_REQ_ID != this.oldServiceReq.ID);
                    }
                    if (serviceReqMaty != null)
                    {
                        foreach (var item in serviceReqMaty)
                        {
                            var sr = serviceReqPreExpmestAll.FirstOrDefault(o => o.ID == item.SERVICE_REQ_ID);
                            int useDays = 1;
                            if (sr.USE_TIME_TO.HasValue)
                            {
                                System.DateTime dtUseTimeTo = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(sr.USE_TIME_TO ?? 0).Value;
                                System.DateTime dtInstructionTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(sr.USE_TIME > 0 ? (sr.USE_TIME ?? 0) : (sr.INTRUCTION_TIME)).Value;
                                TimeSpan diff__Day = (dtUseTimeTo - dtInstructionTime);
                                useDays = diff__Day.Days + 1;
                            }
                            amountPrescribed += item.AMOUNT / useDays;
                        }
                    } 
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return 0;
            }
            return amountPrescribed;
        }
        private void GetReasonMaxPrescription(string ReasonMaxPrescription)
        {
            try
            {
                this.reasonMaxPrescription = ReasonMaxPrescription;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void GetReasonMaxPrescriptionInDay(string ReasonMaxPrescription)
        {
            try
            {
                this.reasonMaxPrescriptionDay = ReasonMaxPrescription;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private bool CheckOddPrescription(MediMatyTypeADO mediMaTy, decimal _amount) //check kê lẻ
        {
            bool result = true;
            try
            {
                if (mediMaTy != null && mediMaTy.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC
                    && (_amount != (int)_amount) && !String.IsNullOrEmpty(mediMaTy.ODD_WARNING_CONTENT))
                {
                    frmReasonOddPres FrmMessage = new frmReasonOddPres(mediMaTy.ODD_WARNING_CONTENT + "\n" + ResourceMessage.BanCoMuonBoSungKhong + "\n" + ResourceMessage.TrongTruongHopCoVuiLongNhapLyDo, GetReasonOddPrescription, mediMaTy.ODD_PRES_REASON);
                    FrmMessage.ShowDialog();
                    if (String.IsNullOrEmpty(this.reasonOddPrescription))
                    {
                        result = false;
                        mediMaTy.ErrorMessageOddPres = "";
                        mediMaTy.ErrorTypeOddPres = ErrorType.None;
                    }
                }
                else
                {
                    this.reasonOddPrescription = null;
                    if (mediMaTy != null)
                    {
                        mediMaTy.ErrorMessageOddPres = "";
                        mediMaTy.ErrorTypeOddPres = ErrorType.None;
                    }
                }
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }
        private void GetReasonOddPrescription(string ReasonOddPrescription)
        {
            try
            {
                this.reasonOddPrescription = ReasonOddPrescription;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private bool CheckMaMePackage(MediMatyTypeADO mediMaTy)
        {
            bool result = true;
            try
            {
                List<MediMatyTypeADO> mediMatyTypeADOs = gridControlServiceProcess.DataSource as List<MediMatyTypeADO>;
                if (mediMatyTypeADOs == null || mediMatyTypeADOs.Count == 0) return true;

                var existsMameType = mediMatyTypeADOs.Any(o => !(o.IsAssignPackage.HasValue && o.IsAssignPackage.Value) && (o.MEDI_STOCK_ID ?? 0) > 0 && (o.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC || o.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__VT));
                var existsMame = mediMatyTypeADOs.Any(o => (o.IsAssignPackage.HasValue && o.IsAssignPackage.Value) && (o.MEDI_STOCK_ID ?? 0) > 0 && (o.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC || o.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__VT));

                if (mediMaTy != null && (mediMaTy.IsAssignPackage.HasValue && mediMaTy.IsAssignPackage.Value) && existsMameType)
                {
                    string mameWarm = String.Join(",", mediMatyTypeADOs.Where(o => !(o.IsAssignPackage.HasValue && o.IsAssignPackage.Value) && (o.MEDI_STOCK_ID ?? 0) > 0 && (o.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC || o.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__VT)).Select(o => o.MEDICINE_TYPE_NAME));
                    MessageBox.Show(String.Format(ResourceMessage.DanhSachThuocDaKeDaCoThuocTheoXKhongTheBoSungThemThuocYZ, mameWarm, "loại", mediMaTy.MEDICINE_TYPE_NAME, "lô"));
                    result = false;
                }
                else if (mediMaTy != null && !(mediMaTy.IsAssignPackage.HasValue && mediMaTy.IsAssignPackage.Value) && existsMame)
                {
                    string mameWarm = String.Join(",", mediMatyTypeADOs.Where(o => (o.IsAssignPackage.HasValue && o.IsAssignPackage.Value) && (o.MEDI_STOCK_ID ?? 0) > 0 && (o.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC || o.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__VT)).Select(o => o.MEDICINE_TYPE_NAME));
                    MessageBox.Show(String.Format(ResourceMessage.DanhSachThuocDaKeDaCoThuocTheoXKhongTheBoSungThemThuocYZ, mameWarm, "lô", mediMaTy.MEDICINE_TYPE_NAME, "loại"));
                    result = false;
                }
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private bool CheckGenderMediMaty(MediMatyTypeADO mediMaTy)
        {
            bool result = true;
            try
            {
                if (currentTreatmentWithPatientType != null && mediMaTy != null && mediMaTy.TDL_GENDER_ID.HasValue)
                {
                    if (currentTreatmentWithPatientType.TDL_PATIENT_GENDER_ID != mediMaTy.TDL_GENDER_ID)
                    {
                        HIS_GENDER gender = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_GENDER>()
                            .FirstOrDefault(o => o.ID == mediMaTy.TDL_GENDER_ID);
                        if (gender != null)
                        {
                            MessageBox.Show(String.Format(ResourceMessage.ThuocVatTuChiSuDungChoGioiTinhX, mediMaTy.MEDICINE_TYPE_NAME, gender.GENDER_NAME), HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            result = false;
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        /// <summary>
        /// Kiểm tra hẹn khám sớm
        /// #13751
        /// </summary>
        private void CheckAppoinmentEarly()
        {
            try
            {
                if (GlobalStore.IsCabinet || GlobalStore.IsTreatmentIn)
                {
                    return;
                }

                if (this.currentTreatmentWithPatientType != null
                    && this.currentTreatmentWithPatientType.PREVIOUS_APPOINTMENT_TIME.HasValue)
                {
                    DateTime dtAppoinmentTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(this.currentTreatmentWithPatientType.PREVIOUS_APPOINTMENT_TIME.Value).Value;
                    if (dtAppoinmentTime > DateTime.Now)
                    {
                        TimeSpan ts = new TimeSpan();
                        ts = (TimeSpan)(dtAppoinmentTime.Date - DateTime.Now.Date);
                        if (ts.TotalDays > 0)
                        {
                            lblNotice.Text = String.Format(ResourceMessage.BenhNhanDiKamSomTruocXNgay, ts.TotalDays);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        //Kiểm tra thuốc vật tư ngoài kho có kê vượt quá số lượng khả dụng trong kho không
        private bool CheckAmoutMediMaty(MediMatyTypeADO mediMaTy)
        {
            bool result = true;
            try
            {
                if (mediMaTy != null)
                {
                    decimal? Amount = 0;
                    Amount = mediMaTy != null ? mediMaTy.AMOUNT : 0;
                    if (this.actionBosung == HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionEdit)
                    {
                        var listDatas = Newtonsoft.Json.JsonConvert.DeserializeObject<List<MediMatyTypeADO>>(Newtonsoft.Json.JsonConvert.SerializeObject(this.gridControlMediMaty.DataSource));

                        if (listDatas != null && listDatas.Count > 0 && mediMaTy != null)
                        {
                            var data = listDatas.FirstOrDefault(o => o.ID == mediMaTy.ID);
                            Amount = data != null ? data.AMOUNT : 0;
                        }
                        else
                        {
                            Amount = 0;
                        }
                    }

                    if ((HisConfigCFG.OutStockListItemInCaseOfNoStockChosenOption == "2" || (this.currentMediStockNhaThuocSelecteds != null && this.currentMediStockNhaThuocSelecteds.Count > 0)) && ((Amount ?? 0) < this.GetAmount()))
                    {
                        if (mediMaTy.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC)
                        {
                            var medicineTypeAcin__SameAcinBhyt = GetDataByActiveIngrBhyt();

                            if (!HisConfigCFG.IsAutoCreateSaleExpMest || !String.IsNullOrEmpty(medicineTypeAcin__SameAcinBhyt))
                            {
                                Rectangle buttonBounds = new Rectangle(this.txtMediMatyForPrescription.Bounds.X, this.txtMediMatyForPrescription.Bounds.Y, this.txtMediMatyForPrescription.Bounds.Width, this.txtMediMatyForPrescription.Bounds.Height);

                                frmMessageBoxChooseThuocExceAmout form = new frmMessageBoxChooseThuocExceAmout(ChonThuocTrongKhoCungHoatChat, medicineTypeAcin__SameAcinBhyt);
                                form.ShowDialog();
                                switch (this.ChonThuocThayThe)
                                {
                                    case OptionChonThuocThayThe.ThuocCungHoatChat:
                                        //thì copy tên hoạt chất vào ô tìm kiếm ==> tìm ra các thuốc cùng hoạt chất khác để người dùng chọn
                                        this.txtMediMatyForPrescription.Text = medicineTypeAcin__SameAcinBhyt;
                                        this.gridViewMediMaty.ActiveFilterString = " [ACTIVE_INGR_BHYT_NAME] Like '%" + this.txtMediMatyForPrescription.Text + "%'" + " AND [AMOUNT] >= " + this.GetAmount();
                                        //+ " OR [CONCENTRA] Like '%" + txtMediMatyForPrescription.Text + "%'"
                                        //+ " OR [MEDI_STOCK_NAME] Like '%" + txtMediMatyForPrescription.Text + "%'";
                                        this.gridViewMediMaty.OptionsFilter.FilterEditorUseMenuForOperandsAndOperators = false;
                                        this.gridViewMediMaty.OptionsFilter.ShowAllTableValuesInCheckedFilterPopup = false;
                                        this.gridViewMediMaty.OptionsFilter.ShowAllTableValuesInFilterPopup = false;
                                        this.gridViewMediMaty.FocusedRowHandle = 0;
                                        this.gridViewMediMaty.OptionsView.ShowFilterPanelMode = DevExpress.XtraGrid.Views.Base.ShowFilterPanelMode.Never;
                                        this.gridViewMediMaty.OptionsFind.HighlightFindResults = true;

                                        this.popupControlContainerMediMaty.ShowPopup(new Point(buttonBounds.X, buttonBounds.Bottom + 25));
                                        this.txtMediMatyForPrescription.Focus();
                                        this.txtMediMatyForPrescription.SelectAll();
                                        result = false;
                                        break;
                                    case OptionChonThuocThayThe.None:
                                        result = true;
                                        break;
                                    case OptionChonThuocThayThe.SuaSoLuong:
                                        this.spinAmount.SelectAll();
                                        this.spinAmount.Focus();
                                        result = false;
                                        break;
                                    case OptionChonThuocThayThe.NoOption:
                                        result = false;
                                        break;
                                }
                            }
                            else if (HisConfigCFG.IsAutoCreateSaleExpMest && String.IsNullOrEmpty(medicineTypeAcin__SameAcinBhyt))
                            {
                                MessageBox.Show("Thuốc trong nhà thuốc không đủ khả dụng để kê.", HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                this.spinAmount.Focus();
                                this.spinAmount.SelectAll();
                                result = false;
                            }
                        }
                        //else if (mediMaTy.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.VATTU || mediMaTy.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.VATTU_DM || mediMaTy.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.VATTU_TSD)
                        else if (mediMaTy.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__VT)
                        {
                            if (!HisConfigCFG.IsAutoCreateSaleExpMest)
                            {
                                if (MessageBox.Show("Vật tư trong nhà thuốc không đủ khả dụng để kê. Bạn có muốn tiếp tục?", HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.Cancel)
                                {
                                    this.spinAmount.Focus();
                                    this.spinAmount.SelectAll();
                                    result = false;
                                }
                            }
                            else
                            {
                                MessageBox.Show("Vật tư trong nhà thuốc không đủ khả dụng để kê.", HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                this.spinAmount.Focus();
                                this.spinAmount.SelectAll();
                                result = false;
                            }
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

        protected string GetDataByActiveIngrBhyt()
        {
            string result = "";
            try
            {
                var rs = this.mediStockD1ADOs.FirstOrDefault(o =>
                    !String.IsNullOrEmpty(this.currentMedicineTypeADOForEdit.ACTIVE_INGR_BHYT_NAME)
                    && !String.IsNullOrEmpty(o.ACTIVE_INGR_BHYT_NAME)
                    && (o.ACTIVE_INGR_BHYT_NAME.Contains(this.currentMedicineTypeADOForEdit.ACTIVE_INGR_BHYT_NAME))
                    && o.AMOUNT >= this.GetAmount()
                    && ((o.SERVICE_ID == this.currentMedicineTypeADOForEdit.SERVICE_ID && o.MEDI_STOCK_ID != this.currentMedicineTypeADOForEdit.MEDI_STOCK_ID) || (o.SERVICE_ID != this.currentMedicineTypeADOForEdit.SERVICE_ID)));
                if (rs != null)
                {
                    result = this.currentMedicineTypeADOForEdit.ACTIVE_INGR_BHYT_NAME;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        protected void ChonThuocTrongKhoCungHoatChat(OptionChonThuocThayThe chonThuocThayThe)
        {
            try
            {
                this.ChonThuocThayThe = chonThuocThayThe;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Kiểm tra "Cảnh báo" trong nhóm thuốc có được check hay không
        /// </summary>
        /// <returns></returns>
        private bool CheckMedicineGroupWarning()
        {
            bool result = true;
            try
            {
                List<string> medicineTypeNames = new List<string>();
                List<string> ActiveIngredientNames = new List<string>();
                List<string> Date = new List<string>();
                List<string> intructionDateSelectedProcess = new List<string>();

                foreach (var item in this.intructionTimeSelecteds)
                {
                    string intructionDate = item.ToString().Substring(0, 8) + "000000";
                    intructionDateSelectedProcess.Insert(0, intructionDate);
                }

                if (this.currentMedicineTypeADOForEdit != null && this.currentMedicineTypeADOForEdit.MEDICINE_GROUP_ID != null)
                {
                    var medicineGroup = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_MEDICINE_GROUP>().FirstOrDefault(o => o.ID == this.currentMedicineTypeADOForEdit.MEDICINE_GROUP_ID);

                    if (medicineGroup != null && medicineGroup.IS_WARNING == 1 && medicineGroup.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)
                    {
                        if (ValidAcinInteractiveWorker.currentMedicineTypeAcins != null && ValidAcinInteractiveWorker.currentMedicineTypeAcins.Count > 0)
                        {
                            List<V_HIS_MEDICINE_TYPE_ACIN> ActiveIngredients = ValidAcinInteractiveWorker.currentMedicineTypeAcins.Where(o => o.MEDICINE_TYPE_ID == this.currentMedicineTypeADOForEdit.ID).ToList();

                            if (intructionDateSelectedProcess != null && intructionDateSelectedProcess.Count > 1)
                            {
                                for (int i = 1; i < intructionDateSelectedProcess.Count; i++)
                                {
                                    System.DateTime? dateBefore = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(long.Parse(intructionDateSelectedProcess[0]));
                                    System.DateTime? dateAfter = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(long.Parse(intructionDateSelectedProcess[i]));
                                    if (dateBefore != null && dateAfter != null)
                                    {
                                        TimeSpan difference = dateAfter.Value - dateBefore.Value;

                                        var NumberDay = difference.Days + 1;

                                        if (NumberDay > medicineGroup.NUMBER_DAY)
                                        {
                                            medicineTypeNames.Add(this.currentMedicineTypeADOForEdit.MEDICINE_TYPE_NAME);
                                            ActiveIngredientNames.AddRange(ActiveIngredients.Select(o => o.ACTIVE_INGREDIENT_NAME));
                                            Date.Insert(0, Inventec.Common.DateTime.Convert.TimeNumberToDateString(long.Parse(intructionDateSelectedProcess[i])));
                                        }
                                    }
                                }
                            }

                            List<HIS.UC.PeriousExpMestList.ADO.PreServiceReqADO> LstPreServiceReqADO = this.periousExpMestListProcessor.GetPreServiceReqADOData(this.ucPeriousExpMestList);

                            if (LstPreServiceReqADO != null && LstPreServiceReqADO.Count > 0)
                            {
                                var PreServiceReqADOs = LstPreServiceReqADO.Where(o => o.TREATMENT_ID == this.treatmentId && o.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONDT && o.TYPE == 1 && o.MEDICINE_GROUP_ID == this.currentMedicineTypeADOForEdit.MEDICINE_GROUP_ID).ToList();

                                if (PreServiceReqADOs != null && PreServiceReqADOs.Count > 0)
                                {
                                    var lstAcinInteractive = ValidAcinInteractiveWorker.currentMedicineTypeAcins.Where(o => PreServiceReqADOs.Select(p => p.MEDICINE_TYPE_ID).ToList().Contains(o.MEDICINE_TYPE_ID)).ToList();

                                    if (lstAcinInteractive != null && lstAcinInteractive.Count > 0 && ActiveIngredients != null && ActiveIngredients.Count > 0)
                                    {
                                        var checkExistInteractive = lstAcinInteractive.Where(o => ActiveIngredients.Select(p => p.ACTIVE_INGREDIENT_ID).ToList().Contains(o.ACTIVE_INGREDIENT_ID)).ToList();

                                        if (checkExistInteractive != null && checkExistInteractive.Count > 0)
                                        {
                                            foreach (var item in checkExistInteractive)
                                            {
                                                var medicines = PreServiceReqADOs.Where(o => o.MEDICINE_TYPE_ID == item.MEDICINE_TYPE_ID).OrderBy(p => p.INTRUCTION_TIME).FirstOrDefault();

                                                if (medicines != null)
                                                {
                                                    foreach (var itemDate in intructionDateSelectedProcess)
                                                    {
                                                        System.DateTime? dateBefore = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(medicines.INTRUCTION_DATE);
                                                        System.DateTime? dateAfter = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(long.Parse(itemDate));
                                                        if (dateBefore != null && dateAfter != null)
                                                        {
                                                            TimeSpan difference = dateAfter.Value - dateBefore.Value;

                                                            var NumberDay = difference.Days + 1;

                                                            if (NumberDay > medicineGroup.NUMBER_DAY)
                                                            {
                                                                medicineTypeNames.Add(this.currentMedicineTypeADOForEdit.MEDICINE_TYPE_NAME);
                                                                ActiveIngredientNames.Add(item.ACTIVE_INGREDIENT_NAME);
                                                                Date.Insert(0, Inventec.Common.DateTime.Convert.TimeNumberToDateString(long.Parse(itemDate)));
                                                            }
                                                        }
                                                    }

                                                }
                                            }

                                        }

                                    }

                                }

                            }

                            if ((medicineTypeNames != null && medicineTypeNames.Count > 0) && (ActiveIngredientNames != null && ActiveIngredientNames.Count > 0) && (Date != null && Date.Count > 0))
                            {
                                DialogResult myResult;
                                myResult = MessageBox.Show(String.Format(ResourceMessage.CanhBaoThuocKeVuotQuaSoNGaySuDung, string.Join(", ", medicineTypeNames.Distinct()), string.Join(", ", ActiveIngredientNames.Distinct()), string.Join(", ", Date.Distinct())), Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaCanhBao), MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                                if (myResult == System.Windows.Forms.DialogResult.No)
                                {
                                    result = false;
                                    txtMediMatyForPrescription.Focus();
                                    txtMediMatyForPrescription.SelectAll();
                                }
                            }
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

    }
}

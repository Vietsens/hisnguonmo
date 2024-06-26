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
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.Library.AlertWarningFee
{
    /// <summary>
    ///- #21423
    ///- Bổ sung cấu hình hệ thống "His.Desktop.IsUsingWarningHeinFee" - "1: Có cảnh báo trần chi phí BHYT" 
    ///- Khi cấu hình trên bằng 1, khi mở màn hình chỉ định DVKT, và kê đơn, thực hiện lấy các dữ liệu cấu hình trần BHYT để kiểm tra (HIS_WARNING_FEE_CFG)
    ///- Nếu tổng chi phí BHYT của hồ sơ vượt quá định mức thì hiển thị cảnh báo. Nhưng vẫn cho phép người dùng lưu.
    ///- Quy tắc lấy ra thông tin định mức:
    ///+ Chỉ cảnh báo với các hồ sơ có số tiền BHYT chi trả > 0 (tạm thời chỉ cảnh báo với BHYT, sau này sửa để mở rộng với các đối tượng khác sau)
    ///+ Căn cứ vào dữ liệu cấu hình định mức cảnh báo (HIS_WARNING_FEE_CFG)
    ///+ Khi lấy ra được nhiều dòng định mức, thì lấy dòng định mức có số tiền cấu hình (warning_price) cao nhất, và BÉ HƠN tổng BHYT phải trả (VIR_TOTAL_HEIN_PRICE) của hồ sơ đấy.
    ///vd:
    ///- Tổng chi phí BHYT của hồ sơ là 203K (VIR_TOTAL_HEIN_PRICE)
    ///- Tương ứng với diện điều trị + đúng mã KCB ban đầu/khác mã KCB ban đầu --> có 2 dòng định mức: 
    ///+ Mức 1: 100K
    ///+ Mức 2: 200K
    ///+ Mức 3: 300K
    ///--> hiển thị cảnh báo "Chi phí BHYT phải trả của hồ sơ vượt quá Mức 2. Bạn có muốn thực hiện?"
    /// </summary>
    public class AlertWarningFeeManager
    {
        public AlertWarningFeeManager() { }

        public bool Run(long treatmentId, long patientTypeId, long treatmentTypeId, string heinMediorgCode, long patientTypeIdBHYT, decimal totalHeinPriceByTreatment, bool isUsingWarningHeinFee, decimal amountPlus, ref string messageWarning, bool isShowMessage = true)
        {
            bool success = true;
            try
            {
                bool condiValid = true;

                condiValid = condiValid && isUsingWarningHeinFee;
                condiValid = condiValid && patientTypeId > 0;
                condiValid = condiValid && !String.IsNullOrEmpty(heinMediorgCode);
                condiValid = condiValid && treatmentTypeId > 0;
                condiValid = condiValid && totalHeinPriceByTreatment > 0;

                bool isRightMediorg = heinMediorgCode == BranchDataWorker.Branch.HEIN_MEDI_ORG_CODE;
                if (condiValid)
                {
                    decimal amountTotal = 0;
                    var warnFeeCFG = GetWarningHeinFee(treatmentId, patientTypeId, treatmentTypeId, isRightMediorg, amountPlus, totalHeinPriceByTreatment, ref amountTotal);
                    if (warnFeeCFG != null)
                    {
                        string warnFeeCFGName = warnFeeCFG.WARNING_FEE_CFG_NAME;
                        string colorCode = warnFeeCFG.COLOR_CODE;
                        messageWarning = String.Format(MessageUtil.GetMessage(LibraryMessage.Message.Enum.AlertWarningHeinFee), Inventec.Desktop.Common.HtmlString.ProcessorString.InsertFontStyle(Inventec.Desktop.Common.HtmlString.ProcessorString.InsertColor(warnFeeCFGName, GetColor(colorCode)), FontStyle.Bold));

                        if (isShowMessage && DevExpress.XtraEditors.XtraMessageBox.Show(messageWarning, MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                        {
                            success = false;
                        }
                    }
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => warnFeeCFG), warnFeeCFG));
                }
                //else
                //{
                Inventec.Common.Logging.LogSystem.Info("AlertWarningFeeManager.condiValid= " + condiValid
                    + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => isUsingWarningHeinFee), isUsingWarningHeinFee)
                    + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => treatmentId), treatmentId)
                    + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => patientTypeId), patientTypeId)
                    + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => treatmentTypeId), treatmentTypeId)
                    + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => isRightMediorg), isRightMediorg)
                    + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => amountPlus), amountPlus)
                    + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => totalHeinPriceByTreatment), totalHeinPriceByTreatment));
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return success;
        }

        public bool RunOption(long treatmentId, long patientTypeId, long treatmentTypeId, string heinMediorgCode, long patientTypeIdBHYT, decimal totalHeinPriceByTreatment, string isUsingWarningHeinFee, decimal amountPlus, ref string messageWarning, bool isShowMessage = true)
        {
            bool success = true;
            try
            {
                bool condiValid = true;

                condiValid = condiValid && (isUsingWarningHeinFee == "1" || isUsingWarningHeinFee == "2");
                condiValid = condiValid && patientTypeId > 0;
                condiValid = condiValid && !String.IsNullOrEmpty(heinMediorgCode);
                condiValid = condiValid && treatmentTypeId > 0;
                condiValid = condiValid && totalHeinPriceByTreatment > 0;

                bool isRightMediorg = heinMediorgCode == BranchDataWorker.Branch.HEIN_MEDI_ORG_CODE;
                if (condiValid)
                {
                    decimal amountTotal = 0;
                    var warnFeeCFG = GetWarningHeinFee(treatmentId, patientTypeId, treatmentTypeId, isRightMediorg, amountPlus, totalHeinPriceByTreatment, ref amountTotal);
                    if (warnFeeCFG != null)
                    {
                        string warnFeeCFGName = warnFeeCFG.WARNING_FEE_CFG_NAME;
                        string colorCode = warnFeeCFG.COLOR_CODE;
                        if (isUsingWarningHeinFee == "1")
                            messageWarning = String.Format(MessageUtil.GetMessage(LibraryMessage.Message.Enum.AlertWarningHeinFee), Inventec.Desktop.Common.HtmlString.ProcessorString.InsertFontStyle(Inventec.Desktop.Common.HtmlString.ProcessorString.InsertColor(warnFeeCFGName, GetColor(colorCode)), FontStyle.Bold));
                        else if (isUsingWarningHeinFee == "2")
                            messageWarning = String.Format(MessageUtil.GetMessage(LibraryMessage.Message.Enum.SoTienDaKeXChoBHYTDaVuotMucGioiHanYLaZ), Inventec.Common.Number.Convert.NumberToStringRoundMax4(amountTotal), Inventec.Common.Number.Convert.NumberToStringRoundMax4(warnFeeCFG.WARNING_PRICE ?? 0), Inventec.Common.Number.Convert.NumberToStringRoundMax4(amountTotal - warnFeeCFG.WARNING_PRICE ?? 0), String.IsNullOrEmpty(warnFeeCFGName) ? Inventec.Desktop.Common.HtmlString.ProcessorString.InsertFontStyle(Inventec.Desktop.Common.HtmlString.ProcessorString.InsertColor(warnFeeCFGName, GetColor(colorCode)), FontStyle.Bold) : "");

                        if (isShowMessage && DevExpress.XtraEditors.XtraMessageBox.Show(messageWarning, MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                        {
                            success = false;
                        }
                    }
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => warnFeeCFG), warnFeeCFG));
                }
                //else
                //{
                Inventec.Common.Logging.LogSystem.Info("AlertWarningFeeManager.condiValid= " + condiValid
                    + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => isUsingWarningHeinFee), isUsingWarningHeinFee)
                    + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => treatmentId), treatmentId)
                    + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => patientTypeId), patientTypeId)
                    + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => treatmentTypeId), treatmentTypeId)
                    + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => isRightMediorg), isRightMediorg)
                    + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => amountPlus), amountPlus)
                    + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => totalHeinPriceByTreatment), totalHeinPriceByTreatment));
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return success;
        }

        Color GetColor(string colorCode)
        {
            try
            {
                if (!String.IsNullOrEmpty(colorCode))
                {
                    return System.Drawing.ColorTranslator.FromHtml(colorCode);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return Color.Red;
        }

        decimal GetTotalHeinPrice(long treatmentId)
        {
            try
            {
                CommonParam paramCommon = new CommonParam();
                HisTreatmentFeeViewFilter treatFilter = new HisTreatmentFeeViewFilter();
                treatFilter.ORDER_DIRECTION = "MODIFY_TIME";
                treatFilter.ORDER_FIELD = "DESC";
                treatFilter.ID = treatmentId;

                var result = new Inventec.Common.Adapter.BackendAdapter(paramCommon).Get<List<V_HIS_TREATMENT_FEE>>("api/HisTreatment/GetFeeView", ApiConsumers.MosConsumer, treatFilter, paramCommon).FirstOrDefault();
                if (result != null)
                {
                    return result.TOTAL_HEIN_PRICE ?? 0;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return 0;
        }

        HIS_WARNING_FEE_CFG GetWarningHeinFee(long treatmentId, long patientTypeId, long treatmentTypeId, bool isRightMediorg, decimal amountPlus, decimal totalHeinPriceByTreatment, ref decimal amountTotal)
        {
            try
            {
                decimal amountTotalTmp = (totalHeinPriceByTreatment > 0 ? totalHeinPriceByTreatment : GetTotalHeinPrice(treatmentId));

                if (amountTotalTmp > 0)
                {
                    List<HIS_WARNING_FEE_CFG> results = BackendDataWorker.Get<HIS_WARNING_FEE_CFG>();
                    results = (results != null && results.Count > 0 ? results.Where(o =>
                        (!o.IS_ACTIVE.HasValue || (o.IS_ACTIVE.HasValue && o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)) &&
                        o.PATIENT_TYPE_ID == patientTypeId &&
                        o.TREATMENT_TYPE_ID == treatmentTypeId &&
                        (isRightMediorg ? o.IS_RIGHT_MEDI_ORG == 1 : (o.IS_RIGHT_MEDI_ORG == null || o.IS_RIGHT_MEDI_ORG != 1))).ToList() : null);
                    if (results != null && results.Count > 0)
                    {
                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => results), results));
                        if (amountPlus > 0)
                        {
                            amountTotalTmp += amountPlus;
                        }

                        var warningFeeFirst = results.Where(o => (o.WARNING_PRICE ?? 0) < amountTotalTmp).OrderByDescending(o => o.WARNING_PRICE).FirstOrDefault();
                        amountTotal = amountTotalTmp;
                        return warningFeeFirst;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return null;
        }
    }
}

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
using HIS.Desktop.LocalStorage.BackendData;
using HIS.UC.UCHeniInfo.Data;
using HIS.UC.UCHeniInfo.Utils;
using Inventec.Common.Logging;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.UC.UCHeniInfo.CustomValidateRule
{
    class TemplateHeinBHYT1__HeinCardNumber__ValidationRule : DevExpress.XtraEditors.DXErrorProvider.ValidationRule
    {
        internal DevExpress.XtraEditors.TextEdit txtSoThe;
        internal long PatientTypeId;
        internal DevExpress.XtraEditors.CheckEdit chkHasDobCertificate;
        internal List<MOS.EFMODEL.DataModels.HIS_BHYT_BLACKLIST> BhytBlackLists;
        internal List<MOS.EFMODEL.DataModels.HIS_BHYT_WHITELIST> BhytWhiteLists;
        public override bool Validate(Control control, object value)
        {
            bool valid = true;
            try
            {
                valid = valid && (txtSoThe != null);
                valid = valid && (PatientTypeId > 0);
                valid = valid && (chkHasDobCertificate != null);
                var patientType = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE>().FirstOrDefault(o => o.ID == PatientTypeId);
                if (patientType == null)
                {
                    LogSystem.Debug("Khong tim thay thong tin doi tuong benh nhan theo Id doi tuong. PatientTypeId = " + PatientTypeId);
                    valid = true;
                }
                else
                {
                    if (patientType.ID == HIS.Desktop.Plugins.Library.RegisterConfig.HisConfigCFG.PatientTypeId__BHYT)
                    {
                        if (!chkHasDobCertificate.Checked)
                        {
                            //Neu doi tuong benh nhan la Bhyt
                            valid = valid && (!String.IsNullOrEmpty(txtSoThe.Text) && !String.IsNullOrEmpty(txtSoThe.Text.Trim()));
                            if (valid)
                            {
                                string currentValue = txtSoThe.Text.Replace(" ", "").ToUpper();
                                string heincardNumber = HeinUtils.TrimHeinCardNumber(currentValue);
                                valid = valid && (new MOS.LibraryHein.Bhyt.BhytHeinProcessor().IsValidHeinCardNumber(heincardNumber) && !Inventec.Common.String.CheckString.IsOverMaxLengthUTF8(heincardNumber, 15));
                                if (!valid)
                                {
                                    this.ErrorText = MessageUtil.GetMessage(His.UC.LibraryMessage.Message.Enum.NguoiDungNhapSoTheBHYTKhongHopLe);
                                }
                                else //Ktra Thẻ hợp lệ là thẻ: có đầu mã thẻ nằm trong danh sách được khai báo trong HIS_BHYT_WHITE_LIST và ko nằm trong d/s các thẻ trong HIS_BHYT_BLACK_LIST
                                {
                                    string heinCardNumberCode = heincardNumber.Substring(0, 3).ToString();
                                    var lstWhite = BhytWhiteLists.Where(p => p.BHYT_WHITELIST_CODE == heinCardNumberCode).ToList();
                                    if (lstWhite != null && lstWhite.Count() > 0)
                                    {
                                        if (BhytBlackLists != null)
                                        {
                                            foreach (var itemBlack in BhytBlackLists)
                                            {
                                                if (heincardNumber.StartsWith(itemBlack.HEIN_CARD_NUMBER))
                                                {
                                                    this.ErrorText = MessageUtil.GetMessage(His.UC.LibraryMessage.Message.Enum.His_UCHein__SoTheBHYTNamTrongDanhSachDenVuiLongKiemTraLai);
                                                    valid = valid && false;
                                                    break;
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        this.ErrorText = MessageUtil.GetMessage(His.UC.LibraryMessage.Message.Enum.NguoiDungNhapSoTheBHYTKhongHopLe);
                                        valid = valid && false;
                                    }
                                }
                            }
                            else
                                this.ErrorText = MessageUtil.GetMessage(His.UC.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                        }
                    }
                    else
                    {
                        valid = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return valid;
        }

        //public override bool Validate(Control control, object value)
        //{
        //    bool valid = true;
        //    try
        //    {
        //        valid = valid && (txtSoThe != null);
        //        valid = valid && (PatientTypeId > 0);
        //        valid = valid && (chkHasDobCertificate != null);
        //        var patientType = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE>().FirstOrDefault(o => o.ID == PatientTypeId);
        //        if (patientType == null)
        //        {
        //            LogSystem.Debug("Khong tim thay thong tin doi tuong benh nhan theo Id doi tuong. PatientTypeId = " + PatientTypeId);
        //            valid = true;
        //        }
        //        else
        //        {
        //            if (patientType.ID == DataStore.PatientTypeId)
        //            {
        //                if (!chkHasDobCertificate.Checked)
        //                {
        //                    //Neu doi tuong benh nhan la Bhyt
        //                    valid = valid && (!String.IsNullOrEmpty(txtSoThe.Text) && !String.IsNullOrEmpty(txtSoThe.Text.Trim()));
        //                    if (valid)
        //                    {
        //                        string currentValue = txtSoThe.Text.Replace(" ", "").ToUpper();
        //                        string heincardNumber = HeinUtils.TrimHeinCardNumber(currentValue);
        //                        valid = valid && (new MOS.LibraryHein.Bhyt.BhytHeinProcessor().IsValidHeinCardNumber(heincardNumber) && heincardNumber.Length == 15);
        //                        if (!valid)
        //                        {
        //                            this.ErrorText = MessageUtil.GetMessage(His.UC.LibraryMessage.Message.Enum.NguoiDungNhapSoTheBHYTKhongHopLe);
        //                        }
        //                        else //Ktra Thẻ hợp lệ là thẻ: có đầu mã thẻ nằm trong danh sách được khai báo trong HIS_BHYT_WHITE_LIST và ko nằm trong d/s các thẻ trong HIS_BHYT_BLACK_LIST
        //                        {
        //                            string heinCardNumberCode = heincardNumber.Substring(0, 3).ToString();
        //                            var lstWhite = BackendDataWorker.Get<HIS_BHYT_WHITELIST>().Where(p => p.BHYT_WHITELIST_CODE == heinCardNumberCode).ToList();
        //                            if (lstWhite != null && lstWhite.Count() > 0)
        //                            {
        //                                if (BackendDataWorker.Get<HIS_BHYT_BLACKLIST>() != null)
        //                                {
        //                                    foreach (var itemBlack in BackendDataWorker.Get<HIS_BHYT_BLACKLIST>())
        //                                    {
        //                                        if (heincardNumber.StartsWith(itemBlack.HEIN_CARD_NUMBER))
        //                                        {
        //                                            this.ErrorText = MessageUtil.GetMessage(His.UC.LibraryMessage.Message.Enum.His_UCHein__SoTheBHYTNamTrongDanhSachDenVuiLongKiemTraLai);
        //                                            valid = valid && false;
        //                                            break;
        //                                        }
        //                                    }
        //                                }                                        
        //                            }
        //                            else
        //                            {
        //                                this.ErrorText = MessageUtil.GetMessage(His.UC.LibraryMessage.Message.Enum.NguoiDungNhapSoTheBHYTKhongHopLe);
        //                                valid = valid && false;
        //                            }
        //                        }
        //                    }
        //                    else
        //                        this.ErrorText = MessageUtil.GetMessage(His.UC.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
        //                }
        //            }
        //            else
        //            {
        //                valid = true;
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Error(ex);
        //    }
        //    return valid;
        //}
    }
}

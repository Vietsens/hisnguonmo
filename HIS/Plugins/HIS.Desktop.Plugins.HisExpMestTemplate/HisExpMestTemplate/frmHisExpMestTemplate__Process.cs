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
using DevExpress.XtraEditors.DXErrorProvider;
using HIS.Desktop.ADO;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.HisExpMestTemplate.Resources;
using HIS.Desktop.Utilities.Extensions;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.HisExpMestTemplate
{
    public partial class frmHisExpMestTemplate : HIS.Desktop.Utility.FormBase
    {
        private void CalculateAmount()
        {
            try
            {
                decimal vlAmount = CalculateValueAmount();
                if (vlAmount >= 0)
                    this.txtAmount.EditValue = vlAmount;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal decimal CalculateValueAmount()
        {
            decimal vl = -1;
            try
            {
                double sang, trua, chieu, toi = 0, tocdotho, thoigiantho;
                sang = this.GetValueSpin(this.txtSang.Text);
                trua = this.GetValueSpin(this.txtTrua.Text);
                chieu = this.GetValueSpin(this.txtChieu.Text);
                toi = this.GetValueSpin(this.txtToi.Text);
                if (sang > 0
                    || trua > 0
                    || chieu > 0
                    || toi > 0
                    )
                {
                    double tongCong = sang + trua + chieu + toi;

                    vl = Inventec.Common.Number.Convert.NumberToNumberRoundAuto((decimal)tongCong, GetNumberDisplaySeperateFormat());
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return vl;
        }

        private void SpinKeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) &&
       (e.KeyChar != '.') && (e.KeyChar != ',') && (e.KeyChar != '/'))
                {
                    e.Handled = true;
                }

                // only allow one decimal point                
                if ((e.KeyChar == '/') && ((sender as TextEdit).Text.IndexOf('/') > -1))
                {
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SpinValidating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                decimal amountInput = 0;
                string vl = (sender as DevExpress.XtraEditors.TextEdit).Text;
                try
                {
                    if (vl.Contains("/"))
                    {
                        vl = vl.Replace(".", System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator);
                        vl = vl.Replace(",", System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator);
                        var arrNumber = vl.Split('/');
                        if (arrNumber != null && arrNumber.Count() > 1)
                        {
                            amountInput = Convert.ToDecimal(arrNumber[0]) / Convert.ToDecimal(arrNumber[1]);
                        }
                    }
                }
                catch (Exception ex)
                {
                    amountInput = 0;
                    e.Cancel = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal double GetValueSpin(string strValue)
        {
            double value = 0;
            try
            {
                if (!String.IsNullOrEmpty(strValue))
                {
                    string vl = strValue;
                    vl = vl.Replace(".", System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator);
                    vl = vl.Replace(",", System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator);
                    if (vl.Contains("/"))
                    {
                        var arrNumber = vl.Split('/');
                        if (arrNumber != null && arrNumber.Count() > 1)
                        {
                            value = Convert.ToDouble(arrNumber[0]) / Convert.ToDouble(arrNumber[1]);
                        }
                    }
                    else
                    {
                        value = Convert.ToDouble(vl);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return value;
        }

        int GetNumberDisplaySeperateFormat()
        {
            int numberDisplaySeperateFormatAmountTemp = 0;
            if (HisConfigCFG.AmountDecimalNumber > 0 && numberDisplaySeperateFormatAmount > 0)
            {
                numberDisplaySeperateFormatAmountTemp = HisConfigCFG.AmountDecimalNumber;
            }
            else
            {
                numberDisplaySeperateFormatAmountTemp = numberDisplaySeperateFormatAmount;
            }
            return numberDisplaySeperateFormatAmountTemp;
        }

        private void SetHuongDanFromSoLuongNgay()
        {
            try
            {
                if (HisConfigCFG.IsNotAutoGenerateTutorial)
                {
                    return;
                }

                string serviceUnitName = "";
                if (this.MediMatyTypeADO != null)
                {
                    serviceUnitName = !String.IsNullOrEmpty(this.MediMatyTypeADO.CONVERT_UNIT_NAME) ? this.MediMatyTypeADO.CONVERT_UNIT_NAME : (this.MediMatyTypeADO.SERVICE_UNIT_NAME ?? "").ToLower();

                    //if (!String.IsNullOrEmpty(this.MediMatyTypeADO.TUTORIAL))
                    //{
                    //    this.txtTutorial.Text = this.MediMatyTypeADO.TUTORIAL;
                    //}
                    //else
                    {
                        int numberDisplaySeperateFormatAmountTemp = GetNumberDisplaySeperateFormat();

                        StringBuilder huongDan = new StringBuilder();
                        string format__NgayUong = ResourceMessage.NgayUong;
                        string format__NgayUongTemp2 = ResourceMessage.NgayUongTemp2;
                        string format__NgayUongTemp3 = ResourceMessage.NgayUongTemp3;
                        string format__NgayUongTemp4 = ResourceMessage.NgayUongTemp4;
                        string format___NgayXVienBuoiYZ = ResourceMessage._NgayXVienBuoiYZ;
                        string format__Sang = ResourceMessage.Sang;
                        string format__Trua = ResourceMessage.Trua;
                        string format__Chieu = ResourceMessage.Chieu;
                        string format__Toi = ResourceMessage.Toi;
                        string strSeperator = ", ";
                        int solan = 0;
                        double soLuongTrenlanMin = 0;
                        string buoiChon = "";
                        double sang, trua, chieu, toi = 0;

                        sang = this.GetValueSpin(this.txtSang.Text);
                        trua = this.GetValueSpin(this.txtTrua.Text);
                        chieu = this.GetValueSpin(this.txtChieu.Text);
                        toi = this.GetValueSpin(this.txtToi.Text);
                        if (sang > 0
                            || trua > 0
                            || chieu > 0
                            || toi > 0
                        )
                        {
                            if (sang > 0)
                            {
                                if (soLuongTrenlanMin == 0 || soLuongTrenlanMin > sang)
                                    soLuongTrenlanMin = sang;
                                solan += 1;
                                buoiChon = ResourceMessage.BuoiSang;
                            }
                            if (trua > 0)
                            {
                                if (soLuongTrenlanMin == 0 || soLuongTrenlanMin > trua)
                                    soLuongTrenlanMin = trua;
                                solan += 1;
                                buoiChon = ResourceMessage.BuoiTrua;
                            }
                            if (chieu > 0)
                            {
                                if (soLuongTrenlanMin == 0 || soLuongTrenlanMin > chieu)
                                    soLuongTrenlanMin = chieu;
                                solan += 1;
                                buoiChon = ResourceMessage.BuoiChieu;
                            }
                            if (toi > 0)
                            {
                                if (soLuongTrenlanMin == 0 || soLuongTrenlanMin > toi)
                                    soLuongTrenlanMin = toi;
                                solan += 1;
                                buoiChon = ResourceMessage.BuoiToi;
                            }
                            double tongCong = sang + trua + chieu + toi;

                            if (HisConfigCFG.TutorialFormat == 4)
                            {
                                //< Đường dùng> <Tổng số ngày> ngày. Ngày <đường dùng> <số lượng tổng số 1 ngày> <đơn vị> / số lần : thời điểm trong ngày : số lượng <Dạng dùng>      
                                //==> Uống {0} ngày. Ngày uống {1} lần {2} {3} 
                                //==>  Uống 4 ngày. Ngày uống 4 lần sáng 1, trưa 1, chiều 1, tối 1 sau ăn
                                huongDan.Append(String.Format(format__NgayUongTemp4, (String.IsNullOrEmpty(this.cboMedicineUseForm.Text) ? "" : (FirstCharToUpper(this.cboMedicineUseForm.Text) + " ")), "", (String.IsNullOrEmpty(this.cboMedicineUseForm.Text) ? "" : this.cboMedicineUseForm.Text.ToLower() + " "), solan) + " ");
                                huongDan.Append(sang > 0 ? String.Format(format__Sang, Inventec.Common.Number.Convert.NumberToStringRoundAuto((decimal)sang, 4), "").Trim().ToLower() : "");//Sáng {0} {1}
                                huongDan.Append(trua > 0 ? ((String.IsNullOrEmpty(huongDan.ToString()) ? "" : strSeperator) + String.Format(format__Trua, Inventec.Common.Number.Convert.NumberToStringRoundAuto((decimal)trua, 4), "").Trim().ToLower()) : "");
                                huongDan.Append(chieu > 0 ? ((String.IsNullOrEmpty(huongDan.ToString()) ? "" : strSeperator) + String.Format(format__Chieu, Inventec.Common.Number.Convert.NumberToStringRoundAuto((decimal)chieu, 4), "").Trim().ToLower()) : "");
                                huongDan.Append(toi > 0 ? ((String.IsNullOrEmpty(huongDan.ToString()) ? "" : strSeperator) + String.Format(format__Toi, Inventec.Common.Number.Convert.NumberToStringRoundAuto((decimal)toi, 4), "").Trim().ToLower()) : "");
                                huongDan.Append((String.IsNullOrEmpty(this.cboHtuName.Text) ? "" : " " + this.cboHtuName.Text.ToLower()));
                            }
                            else
                            {
                                if (solan == 1)
                                {
                                    if (HisConfigCFG.TutorialFormat == 2)
                                    {
                                        huongDan.Append(tongCong > 0 ? String.Format(format__NgayUongTemp2, (String.IsNullOrEmpty(this.cboMedicineUseForm.Text) ? "" : " " + this.cboMedicineUseForm.Text.ToLower() + " "), "", "", solan) : "");
                                    }
                                    else
                                    {
                                        if ((int)tongCong == tongCong)
                                            huongDan.Append(!String.IsNullOrEmpty(this.txtAmount.Text) ? String.Format(format___NgayXVienBuoiYZ, (String.IsNullOrEmpty(this.cboMedicineUseForm.Text) ? "" : this.cboMedicineUseForm.Text + " "), "" + (int)tongCong, serviceUnitName, buoiChon) : "");
                                        else
                                            huongDan.Append(!String.IsNullOrEmpty(this.txtAmount.Text) ? String.Format(format___NgayXVienBuoiYZ, (String.IsNullOrEmpty(this.cboMedicineUseForm.Text) ? " " : this.cboMedicineUseForm.Text + " "), ConvertNumber.ConvertDecToFracByConfig(tongCong, 4), serviceUnitName, buoiChon) : "");
                                    }
                                }
                                else
                                {
                                    if (HisConfigCFG.TutorialFormat == 2)
                                    {
                                        huongDan.Append(tongCong > 0 ? String.Format(format__NgayUongTemp2, (String.IsNullOrEmpty(this.cboMedicineUseForm.Text) ? "" : " " + this.cboMedicineUseForm.Text.ToLower() + " "), "", "", solan) : "");
                                    }
                                    else
                                    {
                                        if ((int)tongCong == tongCong)
                                            huongDan.Append(tongCong > 0 ? String.Format(format__NgayUong, (String.IsNullOrEmpty(this.cboMedicineUseForm.Text) ? "" : " " + this.cboMedicineUseForm.Text.ToLower() + " "), " " + (int)tongCong, serviceUnitName, solan) : "");
                                        else
                                            huongDan.Append(tongCong > 0 ? String.Format(format__NgayUong, (String.IsNullOrEmpty(this.cboMedicineUseForm.Text) ? "" : " " + this.cboMedicineUseForm.Text.ToLower() + " "), ConvertNumber.ConvertDecToFracByConfig(tongCong, 4), serviceUnitName, solan) : "");
                                    }

                                    huongDan.Append(sang > 0 ? String.Format(format__Sang, ConvertNumber.ConvertDecToFracByConfig(sang, 4), serviceUnitName) : "");
                                    huongDan.Append(trua > 0 ? ((String.IsNullOrEmpty(huongDan.ToString()) ? "" : strSeperator) + String.Format(format__Trua, ConvertNumber.ConvertDecToFracByConfig(trua, 4), serviceUnitName)) : "");
                                    huongDan.Append(chieu > 0 ? ((String.IsNullOrEmpty(huongDan.ToString()) ? "" : strSeperator) + String.Format(format__Chieu, ConvertNumber.ConvertDecToFracByConfig(chieu, 4), serviceUnitName)) : "");
                                    huongDan.Append(toi > 0 ? ((String.IsNullOrEmpty(huongDan.ToString()) ? "" : strSeperator) + String.Format(format__Toi, ConvertNumber.ConvertDecToFracByConfig(toi, 4), serviceUnitName)) : "");

                                    huongDan.Append(!String.IsNullOrEmpty(this.cboHtuName.Text) ? " (" + this.cboHtuName.Text + ")" : "");

                                    huongDan = new StringBuilder().Append(FirstCharToUpper(huongDan.ToString()));
                                }

                                if (HisConfigCFG.TutorialFormat == 3)
                                {
                                    string hdKey1 = huongDan.ToString();
                                    //<Số lượng> <đơn vị>/lần * <Số lần>/ngày (<Định dạnh như cấu hình giá trị 1>)   {0} {1}/lần * {2} lần/ngày
                                    //số lượng: số lượng thấp nhất trong 1 lần dùng/số ngày dùng làm tròn đến 2 chữ số sau phần thập phân
                                    //Ví dụ: Thuốc A được kê là Ngày uống 3 viên chia 2 lần, sáng 1 viên, chiều 2 viên thì hiển thị: 1 viên/lần * 2 lần/ngày (Ngày uống 3 viên chia 2 lần, sáng 01 viên, chiều 02 viên)
                                    huongDan = new StringBuilder();
                                    huongDan.Append(soLuongTrenlanMin > 0 ? String.Format(format__NgayUongTemp3, Inventec.Common.Number.Convert.NumberToStringRoundAuto((decimal)soLuongTrenlanMin, 2), serviceUnitName, solan) : "");
                                    huongDan.Append(" (");
                                    huongDan.Append(hdKey1);
                                    huongDan.Append(")");
                                }
                            }
                            this.txtTutorial.Text = huongDan.ToString().Replace("  ", " ").Replace(", ,", ",");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        string FirstCharToUpper(string str)
        {
            string result = "";
            try
            {
                result = !String.IsNullOrEmpty(str) ? (str.First().ToString().ToUpper() + String.Join("", str.Skip(1)).ToLower()) : "";
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private void SpinAmountKeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                int numberDisplaySeperateFormatAmountTemp = GetNumberDisplaySeperateFormat();
                
                if (numberDisplaySeperateFormatAmountTemp == 0)
                {
                    if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != (char)System.Windows.Forms.Keys.Back)
                    {
                        e.Handled = true;
                    }
                }
                else if (numberDisplaySeperateFormatAmountTemp > 0)
                {
                    if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) &&
   (e.KeyChar != '.') && (e.KeyChar != ',') && (e.KeyChar != '/') && e.KeyChar != (char)System.Windows.Forms.Keys.Back)
                    {
                        e.Handled = true;
                    }
                }

                // only allow one decimal point                
                if ((e.KeyChar == '/') && ((sender as TextEdit).Text.IndexOf('/') > -1))
                {
                    e.Handled = true;
                }
                if (((e.KeyChar == '.') || (e.KeyChar == ',')) && (((sender as TextEdit).Text.IndexOf('.') > -1) || ((sender as TextEdit).Text.IndexOf(',') > -1)))
                {
                    e.Handled = true;
                }

                string oldText = (sender as TextEdit).Text;
                if ((oldText.IndexOf('.') > -1) || (oldText.IndexOf(',') > -1))
                {
                    var arrAmount = oldText.Split(new string[] { ",", "." }, StringSplitOptions.RemoveEmptyEntries);
                    if (arrAmount != null && arrAmount.Length > 1)
                    {
                        string afterSeperate = arrAmount[1];
                        if (afterSeperate.Length >= numberDisplaySeperateFormatAmountTemp && e.KeyChar != (char)System.Windows.Forms.Keys.Back)
                        {
                            e.Handled = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}

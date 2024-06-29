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
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HIS.UC.TotalPriceInfo.ADO;

namespace HIS.UC.TotalPriceInfo
{
    internal partial class UCTotalPriceInfo : UserControl
    {
        private InitADO entiy;

        public UCTotalPriceInfo(InitADO data)
        {
            InitializeComponent();
            try
            {
                this.entiy = data;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void UCTotalPriceInfo_Load(object sender, EventArgs e)
        {
            try
            {
                if (this.entiy != null)
                {
                    layoutDiscount.Text = this.entiy.LayoutDiscount;//chiet khau
                    //layoutDiscountRatio.Text = this.entiy.LayoutDiscountRatio;
                    layoutVirTotalBillFundPrice.Text = this.entiy.LayoutVirTotalBillFundPrice;//phai thu quy
                    layoutVirTotalBillPrice.Text = this.entiy.LayoutVirTotalBillPrice;//thanh toan
                    layoutVirTotalBillTransferPrice.Text = this.entiy.LayoutVirTotalBillTransferPrice;//ket chuyen
                    layoutVirTotalDepositPrice.Text = this.entiy.LayoutVirTotalDepositPrice;//tam ung
                    layoutVirTotalHeinPrice.Text = this.entiy.LayoutVirTotalHeinPrice;//thu dong chi tra
                    layoutVirTotalPatientPrice.Text = this.entiy.LayoutVirTotalPatientPrice;//thu benh nhan
                    layoutVirTotalPrice.Text = this.entiy.LayoutVirTotalPrice;//Phai thu
                    layoutVirTotalPrice.OptionsToolTip.ToolTip = this.entiy.LayoutVirTotalPriceTotip;
                    layoutVirTotalServiceDepositPrice.Text = this.entiy.LayoutVirTotalServiceDepositPrice;

                    licTotalOtherCopaidPrice.Text = this.entiy.LayoutVirTotalOtherCopaidPrice;//Cong ty thanh toan
                    licTotalOtherCopaidPrice.OptionsToolTip.ToolTip = this.entiy.LayoutVirTotalOtherCopaidPriceTotip;

                    layoutVirTotalReceiveMorePrice.Text = this.entiy.LayoutVirTotalReceiveMorePrice;//can thu them

                    layoutVirTotalReceivePrice.Text = this.entiy.LayoutVirTotalReceivePrice;//da thu
                    layoutVirTotalRepayPrice.Text = this.entiy.LayoutVirTotalRepayPrice;//hoan ung
                    lciTotalDiscount.Text = this.entiy.LayoutTotalDiscount;//mien giam
                    layoutTotalRepayPrice.Text = this.entiy.LayoutTotalRepayPrice;//can hoan lai
                    lciTotalOtherBillAmount.Text = this.entiy.LayoutTotalOtherBillAmount;
                    layoutVirTotalHeinPrice.OptionsToolTip.ToolTip = this.entiy.LayoutVirTotalHeinPriceTotip;
                    layoutVirTotalPatientPrice.OptionsToolTip.ToolTip = this.entiy.LayoutVirTotalPatientPriceToTip;
                    layoutVirTotalReceiveMorePrice.OptionsToolTip.ToolTip = this.entiy.LayoutVirTotalReceiveMorePriceTotip;
                    lciTotalOtherBillAmount.OptionsToolTip.ToolTip = this.entiy.LayoutTotalOtherBillAmountTotip;
                    lciVirTotalPriceNoExpend.Text = this.entiy.LayoutVirTotalPriceNoExpend;
                    lciOtherSourcePrice.Text = this.entiy.layoutOtherSourcePrice;

                    layoutTotalDebtAmount.Text = this.entiy.layoutTotalDebtAmount;
                    layoutTotalDebtAmount.OptionsToolTip.ToolTip = this.entiy.layoutTotalDebtAmountTotip;
                    //mặc định cấu hình ẩn dòng hoàn tiền
                    if (String.IsNullOrEmpty(this.entiy.IsShowRepayPriceCFG) || this.entiy.IsShowRepayPriceCFG != "1")
                    {
                        SetControlHeight(19);
                        layoutTotalRepayPrice.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    }
                    else
                    {
                        SetControlHeight(17);
                        layoutTotalRepayPrice.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    }

                    if (!string.IsNullOrEmpty(this.entiy.LayoutLockingAmount))
                        lciLockingAmount.Text = this.entiy.LayoutLockingAmount;
                    if (!string.IsNullOrEmpty(this.entiy.LayoutLockingAmountTotip))
                        lciLockingAmount.OptionsToolTip.ToolTip = this.entiy.LayoutLockingAmountTotip;
                }
                else
                {
                    layoutDiscount.Text = "";
                    //layoutDiscountRatio.Text = "";
                    layoutVirTotalBillFundPrice.Text = "";
                    layoutVirTotalBillPrice.Text = "";
                    layoutVirTotalBillTransferPrice.Text = "";
                    layoutVirTotalDepositPrice.Text = "";
                    layoutVirTotalServiceDepositPrice.Text = "";
                    layoutVirTotalHeinPrice.Text = "";
                    layoutVirTotalPatientPrice.Text = "";
                    layoutVirTotalPrice.Text = "";
                    layoutVirTotalReceiveMorePrice.Text = "";
                    layoutVirTotalReceivePrice.Text = "";
                    layoutVirTotalRepayPrice.Text = "";
                    lciTotalDiscount.Text = "";
                    layoutTotalRepayPrice.Text = "";
                    lciTotalOtherBillAmount.Text = "";
                    lciVirTotalPriceNoExpend.Text = "";
                    layoutTotalDebtAmount.Text = "";

                    licTotalOtherCopaidPrice.Text = "";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetControlHeight(int p)
        {
            try
            {
                if (p > 10)
                {
                    layoutDiscount.TextSize = new Size(layoutDiscount.TextSize.Width, p);
                    layoutVirTotalBillFundPrice.TextSize = new Size(layoutVirTotalBillFundPrice.TextSize.Width, p);
                    layoutVirTotalBillPrice.TextSize = new Size(layoutVirTotalBillPrice.TextSize.Width, p);
                    layoutVirTotalBillTransferPrice.TextSize = new Size(layoutVirTotalBillTransferPrice.TextSize.Width, p);
                    layoutVirTotalDepositPrice.TextSize = new Size(layoutVirTotalDepositPrice.TextSize.Width, p);
                    layoutVirTotalServiceDepositPrice.TextSize = new Size(layoutVirTotalServiceDepositPrice.TextSize.Width, p);
                    layoutVirTotalHeinPrice.TextSize = new Size(layoutVirTotalHeinPrice.TextSize.Width, p);
                    layoutVirTotalPatientPrice.TextSize = new Size(layoutVirTotalPatientPrice.TextSize.Width, p);
                    layoutVirTotalPrice.TextSize = new Size(layoutVirTotalPrice.TextSize.Width, p);
                    layoutVirTotalReceiveMorePrice.TextSize = new Size(layoutVirTotalReceiveMorePrice.TextSize.Width, p);
                    layoutVirTotalReceivePrice.TextSize = new Size(layoutVirTotalReceivePrice.TextSize.Width, p);
                    layoutVirTotalRepayPrice.TextSize = new Size(layoutVirTotalRepayPrice.TextSize.Width, p);
                    lciTotalDiscount.TextSize = new Size(lciTotalDiscount.TextSize.Width, p);
                    layoutTotalRepayPrice.TextSize = new Size(layoutTotalRepayPrice.TextSize.Width, p);
                    lciTotalOtherBillAmount.TextSize = new Size(lciTotalOtherBillAmount.TextSize.Width, p);
                    lciVirTotalPriceNoExpend.TextSize = new Size(lciVirTotalPriceNoExpend.TextSize.Width, p);
                    layoutTotalDebtAmount.TextSize = new Size(layoutTotalDebtAmount.TextSize.Width, p);

                    licTotalOtherCopaidPrice.TextSize = new Size(licTotalOtherCopaidPrice.TextSize.Width, p);
                    lciLockingAmount.TextSize = new Size(licTotalOtherCopaidPrice.TextSize.Width, p);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        internal void SetValueToControl(TotalPriceADO data)
        {
            try
            {
                if (data != null)
                {
                    this.lblDiscount.Text = data.Discount;
                    //this.lblDiscountRatio.Text = data.DiscountRatio;
                    this.lblVirTotalBillPrice.Text = data.TotalBillPrice;
                    this.lblVirTotalBillFundPrice.Text = data.TotalBillFundPrice;
                    this.lblVirTotalBillTransferPrice.Text = data.TotalBillTransferPrice;
                    this.lblVirTotalDepositPrice.Text = data.TotalDepositPrice;
                    this.lblVirTotalServiceDepositPrice.Text = data.TotalServiceDepositPrice;
                    this.lblVirTotalHeinPrice.Text = data.TotalHeinPrice;
                    this.lblVirTotalPatientPrice.Text = data.TotalPatientPrice;
                    this.lblVirTotalPrice.Text = data.TotalPrice;

                    this.lblTotalOtherCopaidPrice.Text = data.TotalOtherCopaidPrice;

                    this.lblVirTotalReceivePrice.Text = data.TotalReceivePrice;
                    this.lblVirTotalRepayPrice.Text = data.TotalRepayPrice;
                    this.lblTotalDiscount.Text = data.TotalDiscount;
                    this.lblTotalOtherBillAmount.Text = data.TotalOtherBillAmount;
                    this.lblVirTotalPriceNoExpend.Text = data.VirTotalPriceNoExpend;
                    this.lblTotalDebtAmount.Text = data.TotalDebtAmount;
                    this.lblOtherSourcePrice.Text = data.TotalOtherSourcePrice;
                    if (String.IsNullOrEmpty(this.entiy.IsShowRepayPriceCFG) || this.entiy.IsShowRepayPriceCFG != "1")
                    {
                        this.lblVirTotalReceiveMorePrice.Text = data.TotalReceiveMorePrice;

                        if (!String.IsNullOrEmpty(data.TotalReceiveMorePrice) && Convert.ToDecimal(data.TotalReceiveMorePrice) <= 0)
                        {
                            this.lblVirTotalReceiveMorePrice.Appearance.ForeColor = Color.Blue;
                            layoutVirTotalReceiveMorePrice.AppearanceItemCaption.ForeColor = Color.Blue;
                        }
                        else
                        {
                            this.lblVirTotalReceiveMorePrice.Appearance.ForeColor = Color.Red;
                            layoutVirTotalReceiveMorePrice.AppearanceItemCaption.ForeColor = Color.Red;
                        }
                    }
                    else
                    {
                        this.LblTotalRepayPrice.Appearance.ForeColor = Color.Green;
                        layoutTotalRepayPrice.AppearanceItemCaption.ForeColor = Color.Green;

                        if (!String.IsNullOrEmpty(data.TotalReceiveMorePrice) && Convert.ToDecimal(data.TotalReceiveMorePrice) <= 0)
                        {
                            this.LblTotalRepayPrice.Text = data.TotalReceiveMorePrice.Replace('-', ' ');
                            this.lblVirTotalReceiveMorePrice.Text = "0";
                        }
                        else
                        {
                            this.LblTotalRepayPrice.Text = "0";
                            this.lblVirTotalReceiveMorePrice.Text = data.TotalReceiveMorePrice.Replace('-', ' ');
                        }
                    }
                    lblLockingAmount.Text = data.LockingAmount;
                    if (!string.IsNullOrEmpty(data.LockingAmount) && Convert.ToDecimal(data.LockingAmount) > 0)
                        lciLockingAmount.AppearanceItemCaption.ForeColor = Color.Red;
                    else
                        lciLockingAmount.AppearanceItemCaption.ForeColor = Color.Black;
                }
                else
                {
                    this.lblDiscount.Text = "0.0000";
                    //this.lblDiscountRatio.Text = "0.00";
                    this.lblVirTotalBillPrice.Text = "0.0000";
                    this.lblVirTotalBillFundPrice.Text = "0.0000";
                    this.lblVirTotalBillTransferPrice.Text = "0.0000";
                    this.lblVirTotalDepositPrice.Text = "0.0000";
                    this.lblVirTotalServiceDepositPrice.Text = "0.0000";
                    this.lblVirTotalHeinPrice.Text = "0.0000";
                    this.lblVirTotalPatientPrice.Text = "0.0000";
                    this.lblVirTotalPrice.Text = "0.0000";

                    this.lblTotalOtherCopaidPrice.Text = "0.0000";

                    this.lblVirTotalReceiveMorePrice.Text = "0.0000";
                    this.lblVirTotalReceivePrice.Text = "0.0000";
                    this.lblVirTotalRepayPrice.Text = "0.0000";
                    this.lblTotalDiscount.Text = "0.0000";
                    this.lblTotalOtherBillAmount.Text = "0.0000";
                    this.lblVirTotalPriceNoExpend.Text = "0.0000";
                    this.lblTotalDebtAmount.Text = "0.0000";
                    this.lblOtherSourcePrice.Text = "0.0000";
                    this.lblVirTotalReceiveMorePrice.Appearance.ForeColor = Color.Blue;
                    layoutVirTotalReceiveMorePrice.AppearanceItemCaption.ForeColor = Color.Blue;
                    lblLockingAmount.Text = "0.0000";
                    lciLockingAmount.AppearanceItemCaption.ForeColor = Color.Black;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}

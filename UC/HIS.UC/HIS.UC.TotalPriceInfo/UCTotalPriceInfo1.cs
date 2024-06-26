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
using HIS.UC.TotalPriceInfo.ADO;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Linq;

namespace HIS.UC.TotalPriceInfo
{
    public partial class UCTotalPriceInfo1 : UserControl
    {
        private InitADO entiy;
        List<TotalPriceRowADO> totalPriceRowADOs = new List<TotalPriceRowADO>();
        bool isHasConfig = false;

        public UCTotalPriceInfo1(InitADO data)
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

        private void UCTotalPriceInfo1_Load(object sender, EventArgs e)
        {
            try
            {
                this.isHasConfig = (!String.IsNullOrEmpty(this.entiy.IsShowRepayPriceCFG) && this.entiy.IsShowRepayPriceCFG == "1");
                InitDefault();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitDefault()
        {
            try
            {
                this.totalPriceRowADOs = new List<TotalPriceRowADO>();
                this.totalPriceRowADOs.Add(new TotalPriceRowADO() { FieldLable = this.entiy.LayoutVirTotalPrice });//Phai thu
                this.totalPriceRowADOs.Add(new TotalPriceRowADO() { FieldLable = this.entiy.LayoutVirTotalHeinPrice });//thu dong chi tra
                this.totalPriceRowADOs.Add(new TotalPriceRowADO() { FieldLable = this.entiy.LayoutVirTotalPatientPrice });//thu benh nhan
                this.totalPriceRowADOs.Add(new TotalPriceRowADO() { FieldLable = this.entiy.LayoutVirTotalBillFundPrice });//phai thu quy
                this.totalPriceRowADOs.Add(new TotalPriceRowADO() { FieldLable = this.entiy.LayoutDiscount });//chiet khau
                this.totalPriceRowADOs.Add(new TotalPriceRowADO() { FieldLable = this.entiy.LayoutVirTotalReceivePrice });//da thu
                this.totalPriceRowADOs.Add(new TotalPriceRowADO() { FieldLable = this.entiy.LayoutVirTotalReceiveMorePrice, IsFieldColorRed = true });//can thu them

                if (this.isHasConfig)
                {
                    this.totalPriceRowADOs.Add(new TotalPriceRowADO() { FieldLable = this.entiy.LayoutTotalRepayPrice, IsFieldColorBlue = true });//can hoan lai
                }

                this.totalPriceRowADOs.Add(new TotalPriceRowADO() { FieldLable = this.entiy.LayoutVirTotalDepositPrice });//tam ung
                this.totalPriceRowADOs.Add(new TotalPriceRowADO() { FieldLable = this.entiy.LayoutVirTotalServiceDepositPrice });
                this.totalPriceRowADOs.Add(new TotalPriceRowADO() { FieldLable = this.entiy.LayoutVirTotalBillPrice });//thanh toan
                this.totalPriceRowADOs.Add(new TotalPriceRowADO() { FieldLable = this.entiy.LayoutVirTotalBillTransferPrice });//ket chuyen
                this.totalPriceRowADOs.Add(new TotalPriceRowADO() { FieldLable = this.entiy.LayoutVirTotalRepayPrice });//hoan ung
                this.totalPriceRowADOs.Add(new TotalPriceRowADO() { FieldLable = this.entiy.LayoutTotalDiscount });//mien giam

                gridControlTotalPrice.DataSource = this.totalPriceRowADOs;
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
                    foreach (var item in this.totalPriceRowADOs)
                    {
                        if (item.FieldLable == this.entiy.LayoutVirTotalPrice)
                        {
                            item.FieldValue = data.TotalPrice;
                        }
                        else if (item.FieldLable == this.entiy.LayoutVirTotalHeinPrice)
                        {
                            item.FieldValue = data.TotalHeinPrice;
                        }
                        else if (item.FieldLable == this.entiy.LayoutVirTotalPatientPrice)
                        {
                            item.FieldValue = data.TotalPatientPrice;
                        }
                        else if (item.FieldLable == this.entiy.LayoutVirTotalBillFundPrice)
                        {
                            item.FieldValue = data.TotalBillFundPrice;
                        }
                        else if (item.FieldLable == this.entiy.LayoutDiscount)
                        {
                            item.FieldValue = data.Discount;
                        }
                        else if (item.FieldLable == this.entiy.LayoutVirTotalReceivePrice)
                        {
                            item.FieldValue = data.TotalReceivePrice;
                        }
                        else if (item.FieldLable == this.entiy.LayoutVirTotalReceiveMorePrice)
                        {
                            item.FieldValue = (this.isHasConfig ? Inventec.Common.TypeConvert.Parse.ToDecimal(data.TotalReceiveMorePrice) <= 0 ? "0" : data.TotalReceiveMorePrice.Replace('-', ' ') : data.TotalReceiveMorePrice);
                            item.IsFieldColorBlue = (!this.isHasConfig ? Inventec.Common.TypeConvert.Parse.ToDecimal(data.TotalReceiveMorePrice) <= 0 ? true : false : false);
                            item.IsFieldColorRed = !item.IsFieldColorBlue;
                        }
                        else if (item.FieldLable == this.entiy.LayoutTotalRepayPrice)
                        {
                            item.FieldValue = data.TotalRepayPrice;
                        }
                        else if (item.FieldLable == this.entiy.LayoutVirTotalDepositPrice)
                        {
                            item.FieldValue = data.TotalDepositPrice;
                        }
                        else if (item.FieldLable == this.entiy.LayoutVirTotalServiceDepositPrice)
                        {
                            item.FieldValue = data.TotalDepositPrice;
                        }
                        else if (item.FieldLable == this.entiy.LayoutVirTotalBillPrice)
                        {
                            item.FieldValue = data.TotalBillPrice;
                        }
                        else if (item.FieldLable == this.entiy.LayoutVirTotalBillTransferPrice)
                        {
                            item.FieldValue = data.TotalBillTransferPrice;
                        }
                        else if (item.FieldLable == this.entiy.LayoutVirTotalRepayPrice)
                        {
                            item.FieldValue = (Inventec.Common.TypeConvert.Parse.ToDecimal(data.TotalReceiveMorePrice) <= 0 ? data.TotalReceiveMorePrice.Replace('-', ' ') : "0");
                        }
                        else if (item.FieldLable == this.entiy.LayoutTotalDiscount)
                        {
                            item.FieldValue = data.TotalDiscount;
                        }
                    }
                }
                else
                {
                    foreach (var item in this.totalPriceRowADOs)
                    {
                        item.FieldValue = "0.0000";
                    }
                }
                gridControlTotalPrice.DataSource = null;
                gridControlTotalPrice.DataSource = this.totalPriceRowADOs;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewTotalPrice_RowCellStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs e)
        {
            try
            {
                var index = this.gridViewTotalPrice.GetDataSourceRowIndex(e.RowHandle);
                if (index < 0) return;

                var listDatas = this.gridControlTotalPrice.DataSource as List<TotalPriceRowADO>;
                var dataRow = listDatas[index];

                if (dataRow != null)
                {
                    if (dataRow.IsFieldColorBlue)
                    {
                        e.Appearance.ForeColor = System.Drawing.Color.Blue;
                        e.Appearance.Font = new System.Drawing.Font(e.Appearance.Font.FontFamily, 8.75f, System.Drawing.FontStyle.Bold);
                    }
                    else if (dataRow.IsFieldColorRed)
                    {
                        e.Appearance.ForeColor = System.Drawing.Color.Red;
                        e.Appearance.Font = new System.Drawing.Font(e.Appearance.Font.FontFamily, 8.75f, System.Drawing.FontStyle.Bold);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}

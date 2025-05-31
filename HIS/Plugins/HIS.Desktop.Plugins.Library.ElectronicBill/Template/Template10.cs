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
using HIS.Desktop.Plugins.Library.ElectronicBill.Config;
using HIS.Desktop.Plugins.Library.ElectronicBill.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.Library.ElectronicBill.Template
{
    class Template10 : IRunTemplate
    {
        //17995 - Bv 199- BCA: Kết nối hóa đơn điện tử Viettel
        private Base.ElectronicBillDataInput DataInput;

        public Template10(Base.ElectronicBillDataInput dataInput)
        {
            // TODO: Complete member initialization
            this.DataInput = dataInput;
        }

        public object Run()
        {
            List<ProductBase> result = new List<ProductBase>();
            if (DataInput.SereServBill != null && DataInput.SereServBill.Count > 0)
            {
                int roundNum = 4;
                if (HisConfigCFG.RoundTransactionAmountOption == "1"
                    || HisConfigCFG.RoundTransactionAmountOption == "2")
                {
                    roundNum = 0;
                }

                decimal totalPrice = 0;

                //1
                var sereServXn = DataInput.SereServBill.Where(o => o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__XN || o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__GPBL).ToList();
                ProductBase productXn = new ProductBase();
                productXn.ProdCode = "TONG";
                productXn.ProdName = "Xét nghiệm (Medical test)";
                if (sereServXn != null && sereServXn.Count > 0)
                {
                    productXn.Amount = sereServXn.Sum(s => s.PRICE);
                    productXn.Amount = Math.Round(productXn.Amount, roundNum, MidpointRounding.AwayFromZero);
                }

                totalPrice += productXn.Amount;
                result.Add(productXn);

                //2
                var sereServCdha = DataInput.SereServBill.Where(o => o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__CDHA
                    || o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__TDCN
                    || o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__SA
                    || o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__NS).ToList();
                ProductBase productCdha = new ProductBase();
                productCdha.ProdCode = "TONG";
                productCdha.ProdName = "CĐHA - TDCN (Diagnostic imaging - Functional assessments)";
                if (sereServCdha != null && sereServCdha.Count > 0)
                {
                    productCdha.Amount = sereServCdha.Sum(s => s.PRICE);
                    productCdha.Amount = Math.Round(productCdha.Amount, roundNum, MidpointRounding.AwayFromZero);
                }

                totalPrice += productCdha.Amount;
                result.Add(productCdha);

                //3
                var sereServTvt = DataInput.SereServBill.Where(o => o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC
                    || o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__VT).ToList();
                ProductBase productTvt = new ProductBase();
                productTvt.ProdCode = "TONG";
                productTvt.ProdName = "Thuốc - VTYT (Medication - Medical supplies)";
                if (sereServTvt != null && sereServTvt.Count > 0)
                {
                    productTvt.Amount = sereServTvt.Sum(s => s.PRICE);
                    productTvt.Amount = Math.Round(productTvt.Amount, roundNum, MidpointRounding.AwayFromZero);
                }

                totalPrice += productTvt.Amount;
                result.Add(productTvt);

                //4
                var sereServPttt = DataInput.SereServBill.Where(o => o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__PT || o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__TT).ToList();
                ProductBase productPttt = new ProductBase();
                productPttt.ProdCode = "TONG";
                productPttt.ProdName = "Thủ thuật - phẫu thuật (Medical procedures - Surgery)";
                if (sereServPttt != null && sereServPttt.Count > 0)
                {
                    productPttt.Amount = sereServPttt.Sum(s => s.PRICE);
                    productPttt.Amount = Math.Round(productPttt.Amount, roundNum, MidpointRounding.AwayFromZero);
                }

                totalPrice += productPttt.Amount;
                result.Add(productPttt);

                //5
                var sereServKh = DataInput.SereServBill.Where(o => o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__KH).ToList();
                ProductBase productKh = new ProductBase();
                productKh.ProdCode = "TONG";
                productKh.ProdName = "Tiền khám (Examination cost)";
                if (sereServKh != null && sereServKh.Count > 0)
                {
                    productKh.Amount = sereServKh.Sum(s => s.PRICE);
                    productKh.Amount = Math.Round(productKh.Amount, roundNum, MidpointRounding.AwayFromZero);
                }

                totalPrice += productKh.Amount;
                result.Add(productKh);

                //6
                var sereServG = DataInput.SereServBill.Where(o => o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__G).ToList();
                ProductBase productG = new ProductBase();
                productG.ProdCode = "TONG";
                productG.ProdName = "Tiền giường (Hospital bed cost)";
                if (sereServG != null && sereServG.Count > 0)
                {
                    productG.Amount = sereServG.Sum(s => s.PRICE);
                    productG.Amount = Math.Round(productG.Amount, roundNum, MidpointRounding.AwayFromZero);
                }

                totalPrice += productG.Amount;
                result.Add(productG);

                //7
                List<long> allType = new List<long>()
                {
                    IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__XN,
                    IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__GPBL,
                    IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__CDHA,
                    IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__TDCN,
                    IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__SA,
                    IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__NS,
                    IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC,
                    IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__VT,
                    IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__PT,
                    IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__TT,
                    IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__KH,
                    IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__G
                };

                var sereServKhac = DataInput.SereServBill.Where(o => !allType.Contains(o.TDL_SERVICE_TYPE_ID ?? 0)).ToList();
                ProductBase productKhac = new ProductBase();
                productKhac.ProdCode = "TONG";
                productKhac.ProdName = "Khác (Others)";
                if (sereServKhac != null && sereServKhac.Count > 0)
                {
                    productKhac.Amount = sereServKhac.Sum(s => s.PRICE);
                    productKhac.Amount = Math.Round(productKhac.Amount, roundNum, MidpointRounding.AwayFromZero);
                }

                totalPrice += productKhac.Amount;
                result.Add(productKhac);

                decimal billFund = 0;
                if (DataInput.Transaction != null && DataInput.Transaction.TDL_BILL_FUND_AMOUNT.HasValue)
                {
                    billFund = DataInput.Transaction.TDL_BILL_FUND_AMOUNT ?? 0;
                }
                else if (DataInput.Transaction != null && DataInput.Transaction.HIS_BILL_FUND != null && DataInput.Transaction.HIS_BILL_FUND.Count > 0)
                {
                    billFund = DataInput.Transaction.HIS_BILL_FUND.Sum(s => s.AMOUNT);
                }
                else if (DataInput.ListTransaction != null && DataInput.ListTransaction.Count > 0)
                {
                    billFund = DataInput.ListTransaction.Sum(o => o.TDL_BILL_FUND_AMOUNT ?? 0);
                }

                billFund = Math.Round(billFund, roundNum, MidpointRounding.AwayFromZero);

                decimal discount = DataInput.SereServBill.Sum(o => o.TDL_DISCOUNT ?? 0);
                discount = Math.Round(discount, roundNum, MidpointRounding.AwayFromZero);

                //8
                ProductBase productTongVp = new ProductBase();
                productTongVp.ProdCode = "0";
                productTongVp.ProdName = "Tổng viện phí (Total hospital cost)";
                productTongVp.Amount = totalPrice + discount;
                result.Add(productTongVp);
                //9
                ProductBase productThuBn = new ProductBase();
                productThuBn.ProdCode = "0";
                productThuBn.ProdName = "+ Tổng thu BN (Total Patient Revenue)";
                productThuBn.Amount = totalPrice - billFund;
                productThuBn.Amount = Math.Round(productThuBn.Amount, roundNum, MidpointRounding.AwayFromZero);
                result.Add(productThuBn);

                //10
                ProductBase productCctbh = new ProductBase();
                productCctbh.ProdCode = "0";
                productCctbh.ProdName = "- Cùng chi trả BH (Patient payment)";
                productCctbh.Amount = DataInput.SereServBill.Sum(o => (o.TDL_TOTAL_PATIENT_PRICE_BHYT ?? 0) - (o.TDL_DISCOUNT ?? 0));
                productCctbh.Amount = Math.Round(productCctbh.Amount, roundNum, MidpointRounding.AwayFromZero);
                result.Add(productCctbh);
                if (productCctbh.Amount < 0)
                {
                    productCctbh.Amount = 0;
                }

                //11
                ProductBase productDichVu = new ProductBase();
                productDichVu.ProdCode = "0";
                productDichVu.ProdName = "- Dịch vụ (Service cost)";
                productDichVu.Amount = DataInput.SereServBill.Sum(o => o.PRICE - ((o.TDL_TOTAL_PATIENT_PRICE_BHYT ?? 0) - (o.TDL_DISCOUNT ?? 0)));
                productDichVu.Amount = Math.Round(productDichVu.Amount, roundNum, MidpointRounding.AwayFromZero);
                result.Add(productDichVu);
                if (productDichVu.Amount < 0)
                {
                    productDichVu.Amount = 0;
                }


                //chênh tiền do có miễn giảm dịch vụ
                if (productCctbh.Amount + productDichVu.Amount > productThuBn.Amount)
                {
                    Inventec.Common.Logging.LogSystem.Info("_____productThuBn.Amount:" + productThuBn.Amount + "_____productCctbh.Amount:" + productCctbh.Amount + "_____productDichVu.Amount:" + productDichVu.Amount);

                    if (productThuBn.Amount - productDichVu.Amount > 0)
                    {
                        productCctbh.Amount = productThuBn.Amount - productDichVu.Amount;
                    }
                    else if (productThuBn.Amount - productCctbh.Amount > 0)
                    {
                        productDichVu.Amount = productThuBn.Amount - productCctbh.Amount;
                    }
                    else
                    {
                        decimal chenhLech = productCctbh.Amount + productDichVu.Amount - productThuBn.Amount;
                        if (productDichVu.Amount - chenhLech > 0)
                        {
                            productDichVu.Amount -= chenhLech;
                        }
                        else if (productCctbh.Amount - chenhLech > 0)
                        {
                            productCctbh.Amount -= chenhLech;
                        }
                        else
                        {
                            productCctbh.Amount = 0;
                            productDichVu.Amount = productThuBn.Amount;
                        }
                    }
                }

                //12
                ProductBase productBillFund = new ProductBase();
                productBillFund.ProdCode = "0";
                productBillFund.ProdName = "+ Bảo lãnh viện phí (Hospital expenses insurance)";
                productBillFund.Amount = billFund;
                result.Add(productBillFund);

                //13
                ProductBase productDiscount = new ProductBase();
                productDiscount.ProdCode = "0";
                productDiscount.ProdName = "+ Giảm giá (Discount)";
                productDiscount.Amount = discount;
                result.Add(productDiscount);

                //14
                //tạm thu chỉ hiển thị khi người dùng chọn kết chuyển.
                //Tại thời điểm tạo giao dịch đã lưu các thông tin tạm ứng, hoàn ứng, kết chuyển
                //số tiền tạm ứng tại thời điểm thanh toán sẽ là tổng tiền tạm ứng - hoàn ứng - kết chuyển
                //thanh toán lần 1 kết chuyển và hoàn ứng hết thì lần thanh toán 2 sẽ còn số tiền tạm ứng lần 2.
                ProductBase productTamThu = new ProductBase();
                productTamThu.ProdCode = "0";
                productTamThu.ProdName = "Tạm thu (Advance received)";

                if (DataInput.Transaction != null && DataInput.Transaction.KC_AMOUNT.HasValue && DataInput.Transaction.KC_AMOUNT.Value > 0)
                {
                    productTamThu.Amount = (DataInput.Transaction.TREATMENT_DEPOSIT_AMOUNT ?? 0) - (DataInput.Transaction.TREATMENT_REPAY_AMOUNT ?? 0) - (DataInput.Transaction.TREATMENT_TRANSFER_AMOUNT ?? 0);
                }
                //else if (DataInput.ListTransaction != null && DataInput.ListTransaction.Count > 0 && DataInput.ListTransaction.Exists(o => (o.KC_AMOUNT ?? 0) > 0))
                //{
                //    productTamThu.Amount = DataInput.ListTransaction.Where(o => (o.KC_AMOUNT ?? 0) > 0).Sum(s => (s.TREATMENT_DEPOSIT_AMOUNT ?? 0) - (s.TREATMENT_REPAY_AMOUNT ?? 0) - (s.TREATMENT_TRANSFER_AMOUNT ?? 0));
                //}

                productTamThu.Amount = Math.Round(productTamThu.Amount, roundNum, MidpointRounding.AwayFromZero);

                result.Add(productTamThu);

                //15 Thu thêm/thoái trả
                ProductBase productThuTra = new ProductBase();
                productThuTra.ProdCode = "0";
                productThuTra.ProdName = "+ Thu thêm (Additional revenue)";
                productThuTra.Amount = productThuBn.Amount - productTamThu.Amount;
                if (productThuBn.Amount - productTamThu.Amount < 0)
                {
                    productThuTra.ProdName = "+ Thoái trả (Change)";
                    productThuTra.Amount = productTamThu.Amount - productThuBn.Amount;
                }

                result.Add(productThuTra);
            }

            return result;
        }
    }
}

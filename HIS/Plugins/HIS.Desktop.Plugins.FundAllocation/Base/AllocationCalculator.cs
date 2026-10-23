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
using HIS.Desktop.Plugins.FundAllocation.ADO;
using HIS.Desktop.Plugins.FundAllocation.Resources;
using System;

namespace HIS.Desktop.Plugins.FundAllocation.Base
{
    /// <summary>
    /// Side of the allocation being edited
    /// </summary>
    enum AllocationSide
    {
        /// <summary>Insurer pays</summary>
        Fund = 1,
        /// <summary>Patient pays</summary>
        Patient = 2
    }

    /// <summary>
    /// Edited cell of a side
    /// </summary>
    enum AllocationField
    {
        Amount = 1,
        Price = 2,
        TotalPrice = 3
    }

    /// <summary>
    /// Split rules of one cost line between insurer and patient.
    /// Q0, T0: original quantity and amount (CurrentAmount, CurrentTotalPrice); the edited side is the active side,
    /// the other side is always Q0 - quantity and T0 - amount
    /// </summary>
    static class AllocationCalculator
    {
        private const int MONEY_DECIMALS = 4;

        internal static void MoveAll(FundAllocationLineADO line, AllocationSide toSide)
        {
            if (toSide == AllocationSide.Fund)
            {
                Set(line, line.CurrentAmount, line.CurrentTotalPrice);
            }
            else
            {
                Set(line, 0, 0);
            }
        }

        /// <summary>
        /// Apply the new value of an edited cell. Returns false and an error message when the value is not valid;
        /// the line is not changed in that case
        /// </summary>
        internal static bool TryApply(FundAllocationLineADO line, AllocationSide side, AllocationField field, decimal value, out string error)
        {
            error = null;
            decimal q0 = line.CurrentAmount;
            decimal t0 = line.CurrentTotalPrice;
            decimal sideAmount = side == AllocationSide.Fund ? line.FUND_AMOUNT : line.PATIENT_AMOUNT;
            decimal sidePrice = side == AllocationSide.Fund ? line.FUND_PRICE : line.PATIENT_PRICE;

            decimal newAmount;
            decimal newTotal;
            switch (field)
            {
                case AllocationField.Amount:
                    if (value < 0 || value > q0 || (value != q0 && value != Math.Truncate(value)))
                    {
                        error = string.Format(ResourceLanguageManager.Get("frmFundAllocation.Message.SoLuongKhongHopLe"), FormatNumber(q0));
                        return false;
                    }
                    newAmount = value;
                    if (newAmount == q0)
                    {
                        newTotal = t0;
                    }
                    else if (newAmount == 0)
                    {
                        newTotal = 0;
                    }
                    else
                    {
                        if (sidePrice <= 0)
                        {
                            sidePrice = q0 != 0 ? t0 / q0 : 0;
                        }
                        newTotal = Math.Round(newAmount * sidePrice, MONEY_DECIMALS);
                    }
                    break;
                case AllocationField.Price:
                    if (value < 0)
                    {
                        error = ResourceLanguageManager.Get("frmFundAllocation.Message.DonGiaKhongHopLe");
                        return false;
                    }
                    newAmount = sideAmount;
                    newTotal = Math.Round(sideAmount * value, MONEY_DECIMALS);
                    break;
                default:
                    if (value < 0 || value > t0)
                    {
                        error = string.Format(ResourceLanguageManager.Get("frmFundAllocation.Message.ThanhTienKhongHopLe"), FormatNumber(t0));
                        return false;
                    }
                    newAmount = sideAmount;
                    newTotal = value;
                    break;
            }

            if (newTotal < 0 || newTotal > t0)
            {
                error = string.Format(ResourceLanguageManager.Get("frmFundAllocation.Message.ThanhTienKhongHopLe"), FormatNumber(t0));
                return false;
            }

            decimal otherAmount = q0 - newAmount;
            decimal otherTotal = t0 - newTotal;
            if ((newAmount == 0 && newTotal != 0) || (otherAmount == 0 && otherTotal != 0))
            {
                error = ResourceLanguageManager.Get("frmFundAllocation.Message.BenSoLuong0KhongNhanThanhTien");
                return false;
            }
            if ((newAmount > 0 && newTotal <= 0) || (otherAmount > 0 && otherTotal <= 0))
            {
                error = ResourceLanguageManager.Get("frmFundAllocation.Message.SoLuongLonHon0ThanhTienPhaiLonHon0");
                return false;
            }

            if (side == AllocationSide.Fund)
            {
                Set(line, newAmount, newTotal);
            }
            else
            {
                Set(line, otherAmount, otherTotal);
            }
            if (field == AllocationField.Price)
            {
                // keep the exact typed price on the edited side
                if (side == AllocationSide.Fund)
                {
                    line.FUND_PRICE = value;
                }
                else
                {
                    line.PATIENT_PRICE = value;
                }
            }
            return true;
        }

        /// <summary>
        /// Unit price has more than 3 decimals (displayed rounded, shown in red)
        /// </summary>
        internal static bool IsOddPrice(decimal price)
        {
            return Math.Round(price, 3) != price;
        }

        internal static string FormatNumber(decimal value)
        {
            return value.ToString("#,##0.####");
        }

        private static void Set(FundAllocationLineADO line, decimal fundAmount, decimal fundTotal)
        {
            line.FUND_AMOUNT = fundAmount;
            line.FUND_TOTAL_PRICE = fundTotal;
            line.FUND_PRICE = fundAmount != 0 ? fundTotal / fundAmount : 0;
            line.PATIENT_AMOUNT = line.CurrentAmount - fundAmount;
            line.PATIENT_TOTAL_PRICE = line.CurrentTotalPrice - fundTotal;
            line.PATIENT_PRICE = line.PATIENT_AMOUNT != 0 ? line.PATIENT_TOTAL_PRICE / line.PATIENT_AMOUNT : 0;
        }
    }
}

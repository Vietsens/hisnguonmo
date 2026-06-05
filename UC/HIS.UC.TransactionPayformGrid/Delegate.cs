/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2026 INVENTEC
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HIS.UC.TransactionPayformGrid
{
    /// <summary>
    /// Bao form cha khi tong thanh tien thay doi.
    /// </summary>
    /// <param name="totalAmount">Tong thanh tien (VND) cua tat ca cac dong</param>
    /// <param name="remainAmount">So tien con thieu = Phai thu - Tong thanh tien (0 neu da du)</param>
    public delegate void DelegateTotalAmountChanged(decimal totalAmount, decimal remainAmount);
}

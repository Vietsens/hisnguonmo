/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2017 INVENTEC
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 */
using System;
using System.Windows.Forms;

namespace HIS.UC.MediOrgPicker
{
    /// <summary>
    /// API public cho cac plugin khac goi vao de chon CSKCB chuyen tuyen.
    /// </summary>
    public static class MediOrgPickerProcessor
    {
        /// <summary>
        /// Mo form "Tim chon CSKCB" o che do modal. Tra ve chuoi da ghep san
        /// (vi du "C.01234"). Tra ve null neu nguoi dung dong form ma khong chon.
        /// </summary>
        /// <param name="initialValue">
        /// Gia tri hien co tren TextEdit cua form goi (vi du "C.01234").
        /// Form picker se tach prefix + ma de pre-select dong tuong ung.
        /// </param>
        public static string Pick(string initialValue)
        {
            string result = null;
            try
            {
                using (var frm = new frmMediOrgPicker(initialValue))
                {
                    if (frm.ShowDialog() == DialogResult.OK)
                    {
                        result = frm.SelectedValue;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }
    }
}

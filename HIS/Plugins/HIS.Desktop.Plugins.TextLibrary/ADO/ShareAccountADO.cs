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

namespace HIS.Desktop.Plugins.TextLibrary.ADO
{
    /// <summary>
    /// One row of the "Account list" popup (employee account that a text library item can be shared with).
    /// </summary>
    public class ShareAccountADO
    {
        public string LOGINNAME { get; set; }
        public string EMPLOYEE_CODE { get; set; }
        public string TDL_USERNAME { get; set; }
        public long? DEPARTMENT_ID { get; set; }
        public string DEPARTMENT_NAME { get; set; }
        public bool IsChecked { get; set; }

        /// <summary>
        /// Lower-case, unsigned (no Vietnamese accents) text of code + name + login name, built once for searching.
        /// </summary>
        public string SearchText { get; set; }

        public ShareAccountADO() { }

        public ShareAccountADO(MOS.EFMODEL.DataModels.V_HIS_EMPLOYEE employee)
        {
            try
            {
                if (employee != null)
                {
                    this.LOGINNAME = employee.LOGINNAME;
                    this.EMPLOYEE_CODE = employee.EMPLOYEE_CODE;
                    this.TDL_USERNAME = employee.TDL_USERNAME;
                    this.DEPARTMENT_ID = employee.DEPARTMENT_ID;
                    this.DEPARTMENT_NAME = employee.DEPARTMENT_NAME;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}

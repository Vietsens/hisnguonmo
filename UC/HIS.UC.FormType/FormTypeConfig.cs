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
using Inventec.Core;
using SAR.EFMODEL.DataModels;
using System.Collections.Generic;
using System;
using Inventec.Common.Logging;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using SDA.Filter;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace HIS.UC.FormType
{
    public class FormTypeConfig
    {
        public static string ReportTypeCode = null;
        public static string Language { get; set; }
        public static string UserName { get; set; }
        public static long BranchId { get; set; }

        /// <summary>
        /// Ung dung co trach nhiem load data tu sda & set vao day (thuc hien 1 lan khi dang nhap)
        /// </summary>
        private static List<SAR_FORM_FIELD> formFields = new List<SAR_FORM_FIELD>();
        public static List<SAR_FORM_FIELD> FormFields
        {

            get
            {
                return formFields;
            }
            set
            {
                if (value != null && value.Count > 0)
                {
                    formFields = value;

                }
                else
                {
                    formFields = new List<SAR_FORM_FIELD>();
                }
            }
        }

        /// <summary>
        /// Thong tin ve toi
        /// </summary>
        private static HIS_EMPLOYEE myInfo = new HIS_EMPLOYEE();
        public static HIS_EMPLOYEE MyInfo
        {

            get
            {
                return myInfo;
            }
            set
            {
                if (value != null)
                {
                    myInfo = value;

                }
                else
                {
                    myInfo = new HIS_EMPLOYEE();
                }
            }
        }
    }
}

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
using SAR.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His.UC.CreateReport
{
    public class CreateReportConfig
    {
        
        public static string Language { get; set; }
        public static string LoginName { get; set; }
        public static string UserName { get; set; }

        public static long BranchId { get; set; }

        public static List<SAR.EFMODEL.DataModels.SAR_REPORT_TEMPLATE> ReportTemplates;

        private static List<V_SAR_RETY_FOFI> retyFofis = new List<V_SAR_RETY_FOFI>();
        public static List<V_SAR_RETY_FOFI> RetyFofis
        {

            get
            {
                return retyFofis;
            }
            set
            {
                if (value != null && value.Count > 0)
                {
                    retyFofis = value;

                }
                else
                {
                    retyFofis = new List<V_SAR_RETY_FOFI>();
                }
            }
        }

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


    }
}

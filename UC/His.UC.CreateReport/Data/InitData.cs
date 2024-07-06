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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His.UC.CreateReport.Data
{
    public class InitData
    {
        public SAR.EFMODEL.DataModels.SAR_REPORT_TYPE ReportType { get; set; }
        public List<SAR.EFMODEL.DataModels.SAR_REPORT_TEMPLATE> ReportTemplates { get; set; }
        public List<MOS.EFMODEL.DataModels.HIS_DEPARTMENT> HisDepartments { get; set; }
        public List<MOS.EFMODEL.DataModels.HIS_TREATMENT_TYPE> HisTreatmentTypes { get; set; }
        public List<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE> HisPatientTypes { get; set; }
        public List<MOS.EFMODEL.DataModels.V_HIS_ROOM> HisRooms { get; set; }

        /// <summary>
        /// Init cho form chi co thoi gian tu den
        /// </summary>
        /// <param name="reportType"></param>
        /// <param name="reportTemplates"></param>
        public InitData(SAR.EFMODEL.DataModels.SAR_REPORT_TYPE reportType, List<SAR.EFMODEL.DataModels.SAR_REPORT_TEMPLATE> reportTemplates)
        {
            this.ReportType = reportType;
            this.ReportTemplates = reportTemplates;
        }

        /// <summary>
        /// Init cho form gom thoi gian tu den va khoa
        /// </summary>
        /// <param name="reportType"></param>
        /// <param name="reportTemplates"></param>
        /// <param name="hisDepartments"></param>
        public InitData(SAR.EFMODEL.DataModels.SAR_REPORT_TYPE reportType, List<SAR.EFMODEL.DataModels.SAR_REPORT_TEMPLATE> reportTemplates, List<MOS.EFMODEL.DataModels.HIS_DEPARTMENT> hisDepartments)
        {
            this.ReportType = reportType;
            this.ReportTemplates = reportTemplates;
            this.HisDepartments = hisDepartments;
        }

        /// <summary>
        /// Init cho form gom thoi gian tu dien va loai dieu tri
        /// </summary>
        /// <param name="reportType"></param>
        /// <param name="reportTemplates"></param>
        /// <param name="hisTreatmentTypes"></param>
        public InitData(SAR.EFMODEL.DataModels.SAR_REPORT_TYPE reportType, List<SAR.EFMODEL.DataModels.SAR_REPORT_TEMPLATE> reportTemplates, List<MOS.EFMODEL.DataModels.HIS_TREATMENT_TYPE> hisTreatmentTypes)
        {
            this.ReportType = reportType;
            this.ReportTemplates = reportTemplates;
            this.HisTreatmentTypes = hisTreatmentTypes;
        }


        /// <summary>
        /// Init cho form gom thoi gian va doi tuong benh nhan (thanh toan)
        /// </summary>
        /// <param name="reportType"></param>
        /// <param name="reportTemplates"></param>
        /// <param name="hisPatientTypes"></param>
        public InitData(SAR.EFMODEL.DataModels.SAR_REPORT_TYPE reportType, List<SAR.EFMODEL.DataModels.SAR_REPORT_TEMPLATE> reportTemplates, List<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE> hisPatientTypes)
        {
            this.ReportType = reportType;
            this.ReportTemplates = reportTemplates;
            this.HisPatientTypes = hisPatientTypes;
        }

        /// <summary>
        /// Init cho form gom thoi gian tu den, khoa va loai dieu tri
        /// </summary>
        /// <param name="reportType"></param>
        /// <param name="reportTemplates"></param>
        /// <param name="hisDepartments"></param>
        /// <param name="hisTreatmentTypes"></param>
        public InitData(SAR.EFMODEL.DataModels.SAR_REPORT_TYPE reportType, List<SAR.EFMODEL.DataModels.SAR_REPORT_TEMPLATE> reportTemplates, List<MOS.EFMODEL.DataModels.HIS_DEPARTMENT> hisDepartments, List<MOS.EFMODEL.DataModels.HIS_TREATMENT_TYPE> hisTreatmentTypes)
        {
            this.ReportType = reportType;
            this.ReportTemplates = reportTemplates;
            this.HisDepartments = hisDepartments;
            this.HisTreatmentTypes = hisTreatmentTypes;
        }

        /// <summary>
        /// Init cho form gom thoi gian tu den, khoa va phong
        /// </summary>
        /// <param name="reportType"></param>
        /// <param name="reportTemplates"></param>
        /// <param name="hisDepartments"></param>
        /// <param name="hisRooms"></param>
        public InitData(SAR.EFMODEL.DataModels.SAR_REPORT_TYPE reportType, List<SAR.EFMODEL.DataModels.SAR_REPORT_TEMPLATE> reportTemplates, List<MOS.EFMODEL.DataModels.HIS_DEPARTMENT> hisDepartments, List<MOS.EFMODEL.DataModels.V_HIS_ROOM> hisRooms)
        {
            this.ReportType = reportType;
            this.ReportTemplates = reportTemplates;
            this.HisDepartments = hisDepartments;
            this.HisRooms = hisRooms;
        }

        /// <summary>
        /// Init cho form gom thoi gian tu den, doi tuong va loai dieu tri
        /// </summary>
        /// <param name="reportType"></param>
        /// <param name="reportTemplates"></param>
        /// <param name="hisTreatmentTypes"></param>
        /// <param name="hisPatientTypes"></param>
        public InitData(SAR.EFMODEL.DataModels.SAR_REPORT_TYPE reportType, List<SAR.EFMODEL.DataModels.SAR_REPORT_TEMPLATE> reportTemplates, List<MOS.EFMODEL.DataModels.HIS_TREATMENT_TYPE> hisTreatmentTypes, List<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE> hisPatientTypes)
        {
            this.ReportType = reportType;
            this.ReportTemplates = reportTemplates;
            this.HisPatientTypes = hisPatientTypes;
            this.HisTreatmentTypes = hisTreatmentTypes;
        }
    }
}

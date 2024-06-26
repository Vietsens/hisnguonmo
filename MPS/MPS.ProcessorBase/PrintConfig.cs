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
using EMR.EFMODEL.DataModels;
using SAR.EFMODEL.DataModels;
using System.Collections.Generic;

namespace MPS.ProcessorBase
{
    public class PrintConfig
    {
        public enum PreviewType
        {
            Show,
            ShowDialog,
            PrintNow,
            SaveFile,
            EmrShow,
            EmrSignNow,
            EmrSignAndPrintNow,
            EmrCreateDocument,
            EmrSignAndPrintPreview
        }

        public enum TemplateType
        {
            Excel,
            Word,
            XtraReport,
        }

        public enum CallPrintType
        {
            Flexcel,
            Devexpress,
        }

        public delegate void DelegateShowPrintLog(string printTypeCode, string uniqueCode);

        public static DelegateShowPrintLog ShowModulePrintLog { get; set; }

        public static bool? IsExportExcel { get; set; }
        public static bool? IsAllowEditTemplateFile { get; set; }
        public static string MediOrgCode { get; set; }
        public static string OrganizationName { get; set; }
        public static string ParentOrganizationName { get; set; }
        public static byte[] OrganizationLogo { get; set; }
        public static string OrganizationLogoUri { get; set; }
        public static string OrganizationAddress { get; set; }

        public static List<EMR_BUSINESS> EmrBusiness { get; set; }
        public static List<EMR_SIGNER> EmrSigners { get; set; }

        /// <summary>
        /// Ung dung co trach nhiem load data tu sda & set vao day (thuc hien 1 lan khi dang nhap)
        /// </summary>
        private static List<SAR_PRINT_TYPE> printTypes = new List<SAR_PRINT_TYPE>();
        public static List<SAR_PRINT_TYPE> PrintTypes
        {
            get
            {
                return printTypes;
            }
            set
            {
                if (value != null && value.Count > 0)
                {
                    printTypes = value;
                }
                else
                {
                    printTypes = new List<SAR_PRINT_TYPE>();
                }
            }
        }
        public static string TemnplatePathFolder { get; set; }
        public static string Language { get; set; }
        public static string URI_API_SAR { get; set; }

        public static string VersionApp { get; set; }
        public static string AppCode = "HIS";
        public static string CustomerCode { get; set; }
        public static string IpLocal { get; set; }
        public static bool IsDisposeAfterProcess = true;

        private static System.Action showTutorialModule;
        public static System.Action ShowTutorialModule
        {
            get
            {
                return showTutorialModule;
            }
            set
            {
                showTutorialModule = value;
            }
        }
    }
}

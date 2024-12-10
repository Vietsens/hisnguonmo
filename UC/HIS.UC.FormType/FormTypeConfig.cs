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

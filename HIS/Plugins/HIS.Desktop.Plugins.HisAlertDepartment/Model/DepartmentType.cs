using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.HisAlertDepartment.Model
{
    class DepartmentType
    {
        public long ID { get; set; }
        public string DEPARTMENT_TYPE_CODE { get; set; }
        public string DEPARTMENT_TYPE_NAME { get; set; }
    }
    class DepartmentDTO : HIS_DEPARTMENT
    {
        public bool SELECT_ONE { get; set; }
    }
    class DepartmentDTOWithCheck : DepartmentDTO
    {
        public bool Check { get; set; } // Thuộc tính mới để lưu trạng thái check
    }
}

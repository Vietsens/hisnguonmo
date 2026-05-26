using MOS.EFMODEL.DataModels;

namespace HIS.Desktop.Plugins.HisDepaPatientTypeList.ADO
{
    public class DepartmentADO : HIS_DEPARTMENT
    {
        public bool IsRadioChecked { get; set; }
        public bool IsCheckBoxChecked { get; set; }
        public bool IsAutoExpend { get; set; }
        public bool IsNotExpend { get; set; }

        public DepartmentADO() { }

        public DepartmentADO(HIS_DEPARTMENT data)
        {
            Inventec.Common.Mapper.DataObjectMapper.Map<DepartmentADO>(this, data);
        }
    }
}

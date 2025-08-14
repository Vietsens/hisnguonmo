using MOS.EFMODEL.DataModels;

namespace HIS.Desktop.Plugins.MaterialTypeCreate.MaterialTypeCreate
{
    public class DepartmentADO : HIS_DEPARTMENT
    {
        public bool IsRadioChecked { get; set; }
        public bool IsCheckBoxChecked { get; set; }

        public DepartmentADO() { }

        public DepartmentADO(HIS_DEPARTMENT data)
        {
            Inventec.Common.Mapper.DataObjectMapper.Map<DepartmentADO>(this, data);
        }
    }
}
using MOS.EFMODEL.DataModels;

namespace HIS.Desktop.Plugins.HisDepaPatientTypeList.ADO
{
    public class PatientTypeADO : HIS_PATIENT_TYPE
    {
        public bool IsRadioChecked { get; set; }
        public bool IsCheckBoxChecked { get; set; }
        public bool IsAutoExpend { get; set; }
        public bool IsNotExpend { get; set; }

        public PatientTypeADO() { }

        public PatientTypeADO(HIS_PATIENT_TYPE data)
        {
            Inventec.Common.Mapper.DataObjectMapper.Map<PatientTypeADO>(this, data);
        }
    }
}

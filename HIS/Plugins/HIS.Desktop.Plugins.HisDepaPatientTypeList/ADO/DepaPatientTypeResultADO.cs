using System.Collections.Generic;
using MOS.EFMODEL.DataModels;

namespace HIS.Desktop.Plugins.HisDepaPatientTypeList.ADO
{
    /// <summary>
    /// Kết quả trả về plugin cha qua DelegateSelectData sau khi user nhấn "Chọn".
    /// Plugin cha (MedicineTypeCreate / MaterialTypeCreate) đọc 3 trường này để cập nhật state.
    /// </summary>
    public class DepaPatientTypeResultADO
    {
        public List<HIS_DEPA_PATIENT_TYPE> DepaPatientTypes { get; set; }
        public bool IsCalledApi { get; set; }
        public bool IsClickPick { get; set; }

        public DepaPatientTypeResultADO()
        {
            DepaPatientTypes = new List<HIS_DEPA_PATIENT_TYPE>();
        }
    }
}

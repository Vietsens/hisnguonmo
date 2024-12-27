using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace HIS.UC.FormType.Base
{
    public class ResouceManager
    {
        public static void ResourceLanguageManager()
        {
            try
            {
                Resources.ResourceLanguageManager.LanguageUCAccountBookCombo = new ResourceManager("HIS.UC.FormType.Resources.Lang", typeof(HIS.UC.FormType.AccountBookCombo.UCAccountBookCombo).Assembly);
                Resources.ResourceLanguageManager.LanguageUCDateTime = new ResourceManager("HIS.UC.FormType.Resources.Lang", typeof(HIS.UC.FormType.DateTime.UCDateTime).Assembly);
                Resources.ResourceLanguageManager.LanguageUCDepartmentCombo = new ResourceManager("HIS.UC.FormType.Resources.Lang", typeof(HIS.UC.FormType.DepartmentCombo.UCDepartmentCombo).Assembly);
                Resources.ResourceLanguageManager.LanguageUCExpMestTypeComboCheck = new ResourceManager("HIS.UC.FormType.Resources.Lang", typeof(HIS.UC.FormType.ExpMestTypeComboCheck.UCExpMestTypeComboCheck).Assembly);
                Resources.ResourceLanguageManager.LanguageUCMaterialTypeCombo = new ResourceManager("HIS.UC.FormType.Resources.Lang", typeof(HIS.UC.FormType.MaterialTypeCombo.UCMaterialTypeCombo).Assembly);
                Resources.ResourceLanguageManager.LanguageUCMediStockComboFilterByDepartmentCombo = new ResourceManager("HIS.UC.FormType.Resources.Lang", typeof(HIS.UC.FormType.MediStockComboFilterByDepartmentCombo.UCMediStockComboFilterByDepartmentCombo).Assembly);
                Resources.ResourceLanguageManager.LanguageUCMediStockPereiodByMediStock = new ResourceManager("HIS.UC.FormType.Resources.Lang", typeof(HIS.UC.FormType.MediStockPereiodByMediStock.UCMediStockPereiodByMediStock).Assembly);
                Resources.ResourceLanguageManager.LanguageUCMediStockPeriodMultiRadio = new ResourceManager("HIS.UC.FormType.Resources.Lang", typeof(HIS.UC.FormType.AccountBookCombo.UCAccountBookCombo).Assembly);
                Resources.ResourceLanguageManager.LanguageUCMultiMedicineGridCheckFilterByMediStockPeriodByMediStockByDepartment = new ResourceManager("HIS.UC.FormType.Resources.Lang", typeof(HIS.UC.FormType.MediStockSttFilterCheckBoxGroup.UCMediStockSttFilterCheckBoxGroup).Assembly);
                //Resources.ResourceLanguageManager.LanguageUCMultipleRoomComboCheckFilterByDepartmentComboCheckReturnCode = new ResourceManager("HIS.UC.FormType.Resources.Lang", typeof(HIS.UC.FormType.MultipleRoomComboCheckFilterByDepartmentComboCheckReturnCode.UCMultipleRoomComboCheckFilterByDepartmentComboCheckReturnCode).Assembly);
                Resources.ResourceLanguageManager.LanguageUCPatientTypeCombo = new ResourceManager("HIS.UC.FormType.Resources.Lang", typeof(HIS.UC.FormType.PatientTypeCombo.UCPatientTypeCombo).Assembly);

                Resources.ResourceLanguageManager.LanguageUCRoomCombo = new ResourceManager("HIS.UC.FormType.Resources.Lang", typeof(HIS.UC.FormType.RoomCombo.UCRoomCombo).Assembly);
                Resources.ResourceLanguageManager.LanguageUCThuaThieuVienPhiRadio = new ResourceManager("HIS.UC.FormType.Resources.Lang", typeof(HIS.UC.FormType.ThuaThieuVienPhiRadio.UCThuaThieuVienPhiRadio).Assembly);
                Resources.ResourceLanguageManager.LanguageUCTimeFromTo = new ResourceManager("HIS.UC.FormType.Resources.Lang", typeof(HIS.UC.FormType.TimeFromTo.UCTimeFromTo).Assembly);
                //Resources.ResourceLanguageManager.LanguageUCTreatmentTypeCombo = new ResourceManager("HIS.UC.FormType.Resources.Lang", typeof(HIS.UC.FormType.TreatmentTypeCombo.UCTreatmentTypeCombo).Assembly);
                Resources.ResourceLanguageManager.LanguageUCTreatmentTypeComboCheck = new ResourceManager("HIS.UC.FormType.Resources.Lang", typeof(HIS.UC.FormType.TreatmentTypeComboCheck.UCTreatmentTypeComboCheck).Assembly);
                //Resources.ResourceLanguageManager.LanguageUCMultiFilter = new ResourceManager("HIS.UC.FormType.Resources.Lang", typeof(HIS.UC.FormType.MultiFilter.UCMultiFilter).Assembly);
                Resources.ResourceLanguageManager.languageUCUserNameCombo = new ResourceManager("HIS.UC.FormType.Resources.Lang", typeof(HIS.UC.FormType.UserNameCombo.UCUserNameCombo).Assembly);
                Resources.ResourceLanguageManager.LanguageUCMedicin = new ResourceManager("HIS.UC.FormType.Resources.Lang", typeof(HIS.UC.FormType.Medicin.UCMedicin).Assembly);

            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}

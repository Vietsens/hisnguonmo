using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.MchTreatmentExamService.MainForm
{
    public partial class UCMchTreatmentExamService : HIS.Desktop.Utility.FormBase
    {
        /// <summary>
        /// Khởi tạo giá trị mặc định null cho tất cả SpinEdit và set MaxValue = 999999
        /// </summary>
        private void InitAllSpinEditDefaultValue()
        {
            try
            {
                // Tab 1: Sàng lọc ung thư cổ tử cung
                if (spnHC1 != null)
                {
                    spnHC1.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;
                    spnHC1.EditValue = null;
                    spnHC1.Properties.MaxValue = 999999;
                }
                if (spnH1 != null)
                {
                    spnH1.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;
                    spnH1.EditValue = null;
                    spnH1.Properties.MaxValue = 999999;
                }
                if (spnW1 != null)
                {
                    spnW1.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;
                    spnW1.EditValue = null;
                    spnW1.Properties.MaxValue = 999999;
                }

                // Tab 2: Khám thai
                if (spnBloodPressureDiastolic2 != null)
                {
                    spnBloodPressureDiastolic2.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;
                    spnBloodPressureDiastolic2.EditValue = null;
                    spnBloodPressureDiastolic2.Properties.MaxValue = 999999;
                }
                if (spnBloodPressureSystolic2 != null)
                {
                    spnBloodPressureSystolic2.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;
                    spnBloodPressureSystolic2.EditValue = null;
                    spnBloodPressureSystolic2.Properties.MaxValue = 999999;
                }
                if (spnHeight2 != null)
                {
                    spnHeight2.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;
                    spnHeight2.EditValue = null;
                    spnHeight2.Properties.MaxValue = 999999;
                }
                if (spnWeight2 != null)
                {
                    spnWeight2.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;
                    spnWeight2.EditValue = null;
                    spnWeight2.Properties.MaxValue = 999999;
                }
                if (spnGravida2 != null)
                {
                    spnGravida2.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;
                    spnGravida2.EditValue = null;
                    spnGravida2.Properties.MaxValue = 999999;
                }
                if (spnGestationalAge2 != null)
                {
                    spnGestationalAge2.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;
                    spnGestationalAge2.EditValue = null;
                    spnGestationalAge2.Properties.MaxValue = 999999;
                }
                if (spnAbdominalCircumference2 != null)
                {
                    spnAbdominalCircumference2.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;
                    spnAbdominalCircumference2.EditValue = null;
                    spnAbdominalCircumference2.Properties.MaxValue = 999999;
                }
                if (spnFundalHeight2 != null)
                {
                    spnFundalHeight2.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;
                    spnFundalHeight2.EditValue = null;
                    spnFundalHeight2.Properties.MaxValue = 999999;
                }

                // Tab 3: Sinh đẻ (Mẹ) - xtraTabControl2 Page 1
                if (spnGestationalWeeks3 != null)
                {
                    spnGestationalWeeks3.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;
                    spnGestationalWeeks3.EditValue = null;
                    spnGestationalWeeks3.Properties.MaxValue = 999999;
                }
                if (spnMiscarriage3 != null)
                {
                    spnMiscarriage3.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;
                    spnMiscarriage3.EditValue = null;
                    spnMiscarriage3.Properties.MaxValue = 999999;
                }
                if (spnChildCount3 != null)
                {
                    spnChildCount3.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;
                    spnChildCount3.EditValue = null;
                    spnChildCount3.Properties.MaxValue = 999999;
                }
                if (spnPretermBirth3 != null)
                {
                    spnPretermBirth3.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;
                    spnPretermBirth3.EditValue = null;
                    spnPretermBirth3.Properties.MaxValue = 999999;
                }
                if (spnTermBirths3 != null)
                {
                    spnTermBirths3.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;
                    spnTermBirths3.EditValue = null;
                    spnTermBirths3.Properties.MaxValue = 999999;
                }
                if (spnBirthOrder3 != null)
                {
                    spnBirthOrder3.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;
                    spnBirthOrder3.EditValue = null;
                    spnBirthOrder3.Properties.MaxValue = 999999;
                }
                if (spnNewbornAlive3 != null)
                {
                    spnNewbornAlive3.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;
                    spnNewbornAlive3.EditValue = null;
                    spnNewbornAlive3.Properties.MaxValue = 999999;
                }
                if (spnNumberNewbornBirth3 != null)
                {
                    spnNumberNewbornBirth3.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;
                    spnNumberNewbornBirth3.EditValue = null;
                    spnNumberNewbornBirth3.Properties.MaxValue = 999999;
                }

                // Tab 3: Sinh đẻ (Trẻ) - xtraTabControl2 Page 2
                if (spnVH3 != null)
                {
                    spnVH3.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;
                    spnVH3.EditValue = null;
                    spnVH3.Properties.MaxValue = 999999;
                }
                if (spnH3 != null)
                {
                    spnH3.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;
                    spnH3.EditValue = null;
                    spnH3.Properties.MaxValue = 999999;
                }
                if (spnW3 != null)
                {
                    spnW3.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;
                    spnW3.EditValue = null;
                    spnW3.Properties.MaxValue = 999999;
                }

                // Tab 5: Phá thai
                if (spnGestationalWeeks5 != null)
                {
                    spnGestationalWeeks5.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;
                    spnGestationalWeeks5.EditValue = null;
                    spnGestationalWeeks5.Properties.MaxValue = 999999;
                }
                
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}

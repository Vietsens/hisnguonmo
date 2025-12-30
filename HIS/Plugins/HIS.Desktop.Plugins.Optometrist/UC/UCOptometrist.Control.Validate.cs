using DevExpress.XtraEditors;
using DevExpress.XtraEditors.ViewInfo;
using HIS.Desktop.Utility;
using Inventec.Desktop.Common.Controls.ValidationRule;
using System;

namespace HIS.Desktop.Plugins.Optometrist.UC
{
    public partial class UCOptometrist : UserControlBase
    {
        private int positionHandleControl = -1;

        private void ValidateControl()
        {
            try
            {
                ValidationControlMaxLength(VISION_TEST_NUM, 19, false);

                ValidationControlMaxLength(MACHINE_RIGHT_SPH, 100, false);
                ValidationControlMaxLength(MACHINE_RIGHT_CYL, 100, false);
                ValidationControlMaxLength(MACHINE_RIGHT_AXIS, 100, false);
                ValidationControlMaxLength(MACHINE_LEFT_SPH, 100, false);
                ValidationControlMaxLength(MACHINE_LEFT_CYL, 100, false);
                ValidationControlMaxLength(MACHINE_LEFT_AXIS, 100, false);

                ValidationControlMaxLength(OLD_RIGHT_SPH, 100, false);
                ValidationControlMaxLength(OLD_RIGHT_CYL, 100, false);
                ValidationControlMaxLength(OLD_RIGHT_AXIS, 100, false);
                ValidationControlMaxLength(OLD_RIGHT_GLASS, 100, false);
                ValidationControlMaxLength(OLD_LEFT_SPH, 100, false);
                ValidationControlMaxLength(OLD_LEFT_CYL, 100, false);
                ValidationControlMaxLength(OLD_LEFT_AXIS, 100, false);
                ValidationControlMaxLength(OLD_LEFT_GLASS, 100, false);

                ValidationControlMaxLength(NOW_RIGHT_EYE, 100, false);
                ValidationControlMaxLength(NOW_RIGHT_HOLE, 100, false);
                ValidationControlMaxLength(NOW_RIGHT_SPH, 100, false);
                ValidationControlMaxLength(NOW_RIGHT_CYL, 100, false);
                ValidationControlMaxLength(NOW_RIGHT_AXIS, 100, false);
                ValidationControlMaxLength(NOW_RIGHT_GLASS, 100, false);
                ValidationControlMaxLength(NOW_LEFT_EYE, 100, false);
                ValidationControlMaxLength(NOW_LEFT_HOLE, 100, false);
                ValidationControlMaxLength(NOW_LEFT_SPH, 100, false);
                ValidationControlMaxLength(NOW_LEFT_CYL, 100, false);
                ValidationControlMaxLength(NOW_LEFT_AXIS, 100, false);
                ValidationControlMaxLength(NOW_LEFT_GLASS, 100, false);

                ValidationControlMaxLength(REAL_RIGHT_EYE, 100, false);
                ValidationControlMaxLength(REAL_RIGHT_SPH, 100, false);
                ValidationControlMaxLength(REAL_RIGHT_CYL, 100, false);
                ValidationControlMaxLength(REAL_RIGHT_AXIS, 100, false);
                ValidationControlMaxLength(REAL_RIGHT_GLASS, 100, false);
                ValidationControlMaxLength(REAL_LEFT_EYE, 100, false);
                ValidationControlMaxLength(REAL_LEFT_SPH, 100, false);
                ValidationControlMaxLength(REAL_LEFT_CYL, 100, false);
                ValidationControlMaxLength(REAL_LEFT_AXIS, 100, false);
                ValidationControlMaxLength(REAL_LEFT_GLASS, 100, false);

                ValidationControlMaxLength(RIGHT_EYE_BEFORE, 100, false);
                ValidationControlMaxLength(LEFT_EYE_BEFORE, 100, false);
                ValidationControlMaxLength(RIGHT_EYE_AFTER, 100, false);
                ValidationControlMaxLength(LEFT_EYE_AFTER, 100, false);
                ValidationControlMaxLength(RIGHT_EYE_PRESSURE, 100, false);
                ValidationControlMaxLength(LEFT_EYE_PRESSURE, 100, false);
                ValidationControlMaxLength(RIGHT_EYE_THICKNESS, 100, false);
                ValidationControlMaxLength(LEFT_EYE_THICKNESS, 100, false);
                ValidationControlMaxLength(RIGHT_EYE_K1, 100, false);
                ValidationControlMaxLength(LEFT_EYE_K1, 100, false);
                ValidationControlMaxLength(RIGHT_EYE_K2, 100, false);
                ValidationControlMaxLength(LEFT_EYE_K2, 100, false);

                ValidationControlMaxLength(RIGHT_EYE_MOVE, 100, false);
                ValidationControlMaxLength(LEFT_EYE_MOVE, 100, false);
                ValidationControlMaxLength(RIGHT_EYE_SENSE, 100, false);
                ValidationControlMaxLength(LEFT_EYE_SENSE, 100, false);
                ValidationControlMaxLength(RIGHT_EYE_3D, 100, false);
                ValidationControlMaxLength(LEFT_EYE_3D, 100, false);
                ValidationControlMaxLength(RIGHT_EYE_DEVIATION, 100, false);
                ValidationControlMaxLength(LEFT_EYE_DEVIATION, 100, false);
                ValidationControlMaxLength(RIGHT_EYE_BALL, 100, false);
                ValidationControlMaxLength(LEFT_EYE_BALL, 100, false);

                ValidationControlMaxLength(FAR_RIGHT_SPH, 100, false);
                ValidationControlMaxLength(FAR_RIGHT_CYL, 100, false);
                ValidationControlMaxLength(FAR_RIGHT_AXIS, 100, false);
                ValidationControlMaxLength(FAR_RIGHT_VISION, 100, false);
                ValidationControlMaxLength(FAR_LEFT_SPH, 100, false);
                ValidationControlMaxLength(FAR_LEFT_CYL, 100, false);
                ValidationControlMaxLength(FAR_LEFT_AXIS, 100, false);
                ValidationControlMaxLength(FAR_LEFT_VISION, 100, false);
                ValidationControlMaxLength(FAR_ADD, 100, false);

                ValidationControlMaxLength(NEAR_RIGHT_SPH, 100, false);
                ValidationControlMaxLength(NEAR_RIGHT_CYL, 100, false);
                ValidationControlMaxLength(NEAR_RIGHT_AXIS, 100, false);
                ValidationControlMaxLength(NEAR_RIGHT_VISION, 100, false);
                ValidationControlMaxLength(NEAR_LEFT_SPH, 100, false);
                ValidationControlMaxLength(NEAR_LEFT_CYL, 100, false);
                ValidationControlMaxLength(NEAR_LEFT_AXIS, 100, false);
                ValidationControlMaxLength(NEAR_LEFT_VISION, 100, false);

                ValidationControlMaxLength(FAR_BP, 100, false);
                ValidationControlMaxLength(NEAR_BP, 100, false);

                ValidationControlMaxLength(VISION_EXAM_NOTE, 2000, false);
                ValidationControlMaxLength(GLASS_STATUS, 100, false);
                ValidationControlMaxLength(GLASS_AMOUNT, 100, false);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidationControlMaxLength(BaseEdit control, int? maxLength, bool IsRequired)
        {
            try
            {
                ControlMaxLengthValidationRule validate = new ControlMaxLengthValidationRule();
                validate.editor = control;
                validate.maxLength = maxLength;
                validate.IsRequired = IsRequired;
                validate.ErrorText = "Nhập quá kí tự cho phép [" + maxLength + "]";
                validate.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(control, validate);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dxValidationProvider1_ValidationFailed(object sender, DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventArgs e)
        {
            try
            {
                BaseEdit edit = e.InvalidControl as BaseEdit;
                if (edit == null)
                    return;
                BaseEditViewInfo viewInfo = edit.GetViewInfo() as BaseEditViewInfo;
                if (viewInfo == null)
                    return;
                if (positionHandleControl == -1)
                {
                    positionHandleControl = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.SelectAll();
                        edit.Focus();
                    }
                }
                if (positionHandleControl > edit.TabIndex)
                {
                    positionHandleControl = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.Focus();
                        edit.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

    }
}
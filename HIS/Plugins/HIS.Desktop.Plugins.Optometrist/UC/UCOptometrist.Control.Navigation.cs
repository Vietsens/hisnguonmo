using DevExpress.XtraEditors;
using HIS.Desktop.Utility;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.Optometrist.UC
{
    public partial class UCOptometrist : UserControlBase
    {
        private BaseEdit[] navigationOrder;
        private void InitEnterTabNavigation()
        {
            var navOrder = new List<BaseEdit>();
            int tab = 0;

            void Register(params BaseEdit[] edits)
            {
                foreach (var edit in edits)
                {
                    if (edit == null)
                    {
                        continue;
                    }
                    edit.TabStop = true;
                    edit.EnterMoveNextControl = false;
                    edit.TabIndex = tab++;
                    edit.KeyDown -= OnEditEnterTabNext;
                    edit.KeyDown += OnEditEnterTabNext;
                    edit.PreviewKeyDown -= OnEditPreviewKeyDown;
                    edit.PreviewKeyDown += OnEditPreviewKeyDown;
                    navOrder.Add(edit);
                }
            }
            Register(
                VISION_TEST_TIME, VISION_TEST_NUM,
                VISION_TEST_USERNAME, VISION_TEST_ROOM_NAME);
            Register(
                MACHINE_RIGHT_SPH, MACHINE_RIGHT_CYL, MACHINE_RIGHT_AXIS,
                MACHINE_LEFT_SPH, MACHINE_LEFT_CYL, MACHINE_LEFT_AXIS);
            Register(
                OLD_RIGHT_SPH, OLD_RIGHT_CYL, OLD_RIGHT_AXIS, OLD_RIGHT_GLASS,
                OLD_LEFT_SPH, OLD_LEFT_CYL, OLD_LEFT_AXIS, OLD_LEFT_GLASS);
            Register(
                NOW_RIGHT_EYE, NOW_RIGHT_HOLE, NOW_RIGHT_SPH, NOW_RIGHT_CYL, NOW_RIGHT_AXIS, NOW_RIGHT_GLASS,
                NOW_LEFT_EYE, NOW_LEFT_HOLE, NOW_LEFT_SPH, NOW_LEFT_CYL, NOW_LEFT_AXIS, NOW_LEFT_GLASS);
            Register(
                REAL_RIGHT_EYE, REAL_RIGHT_SPH, REAL_RIGHT_CYL, REAL_RIGHT_AXIS, REAL_RIGHT_GLASS,
                REAL_LEFT_EYE, REAL_LEFT_SPH, REAL_LEFT_CYL, REAL_LEFT_AXIS, REAL_LEFT_GLASS);
            Register(
                RIGHT_EYE_BEFORE, LEFT_EYE_BEFORE,
                RIGHT_EYE_AFTER, LEFT_EYE_AFTER,
                RIGHT_EYE_PRESSURE, LEFT_EYE_PRESSURE,
                RIGHT_EYE_THICKNESS, LEFT_EYE_THICKNESS,
                RIGHT_EYE_K1, LEFT_EYE_K1,
                RIGHT_EYE_K2, LEFT_EYE_K2);
            Register(
                RIGHT_EYE_MOVE, LEFT_EYE_MOVE,
                RIGHT_EYE_SENSE, LEFT_EYE_SENSE,
                RIGHT_EYE_3D, LEFT_EYE_3D,
                RIGHT_EYE_DEVIATION, LEFT_EYE_DEVIATION,
                RIGHT_EYE_BALL, LEFT_EYE_BALL);
            Register(
                FAR_RIGHT_SPH, FAR_RIGHT_CYL, FAR_RIGHT_AXIS, FAR_RIGHT_VISION,
                FAR_LEFT_SPH, FAR_LEFT_CYL, FAR_LEFT_AXIS, FAR_LEFT_VISION,
                FAR_ADD
                );
            Register(
                NEAR_RIGHT_SPH, NEAR_RIGHT_CYL, NEAR_RIGHT_AXIS, NEAR_RIGHT_VISION,
                NEAR_LEFT_SPH, NEAR_LEFT_CYL, NEAR_LEFT_AXIS, NEAR_LEFT_VISION);
            Register(
                FAR_BP, NEAR_BP);
            Register(
                IS_BIFOCAL_GLASS, IS_PHOTOCHROMIC_GLASS,
                IS_PROGRESSIVE_GLASS, IS_POLYCARBONATE_GLASS,
                IS_READING_GLASS, IS_CONTACT_LENSE);
            Register(
                VISION_EXAM_NOTE);
            Register(
                GLASS_STATUS);
            Register(
                GLASS_USE_TIME, GLASS_AMOUNT, IS_GLASS_SCRATCHED);
            Register(
                MEDI_USE_TIME);
            navigationOrder = navOrder.ToArray();
        }

        private void OnEditEnterTabNext(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter && e.KeyCode != Keys.Tab)
            {
                return;
            }

            e.Handled = true;
            e.SuppressKeyPress = true;

            var edit = sender as BaseEdit;
            if (edit == null || navigationOrder == null || navigationOrder.Length == 0)
            {
                return;
            }

            int idx = Array.IndexOf(navigationOrder, edit);
            if (idx < 0)
            {
                return;
            }

            BaseEdit next = null;
            for (int offset = 1; offset <= navigationOrder.Length; offset++)
            {
                var candidate = navigationOrder[(idx + offset) % navigationOrder.Length];
                if (candidate != null && candidate.Visible && candidate.Enabled && candidate.CanFocus && candidate.TabStop)
                {
                    next = candidate;
                    break;
                }
            }

            if (next != null)
            {
                next.Focus();
                next.SelectAll();
            }
        }

        private void OnEditPreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Tab)
            {
                e.IsInputKey = true;
            }
        }
    }
}
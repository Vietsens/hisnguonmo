using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.MchTreatmentExamService.MainForm
{
    public partial class UCMchTreatmentExamService : HIS.Desktop.Utility.FormBase
    {
        /// <summary>
        /// Kh?i t?o button Delete cho t?t c? GridLookUpEdit
        /// </summary>
        private void InitAllGridLookUpEditDeleteButton()
        {
            try
            {
                // Tab 1: Sàng l?c ung th? c? t? cung
                AddDeleteButtonToGridLookUp(cboUser1);
                AddDeleteButtonToGridLookUp(cboDiploma1);
                AddDeleteButtonToGridLookUp(cboCervicalCancerDx1);
                AddDeleteButtonToGridLookUp(cboPreCervicalCancerTreat1);

                // Tab 2: Khám thai
                AddDeleteButtonToGridLookUp(cboUser2);
                AddDeleteButtonToGridLookUp(cboDiploma2);
                AddDeleteButtonToGridLookUp(cboMedicalHistoryInternal2);

                // Tab 3: Sinh ?? (M?)
                AddDeleteButtonToGridLookUp(cboUser3);
                AddDeleteButtonToGridLookUp(cboDiploma3);
                AddDeleteButtonToGridLookUp(cboBirthplaceType3);
                AddDeleteButtonToGridLookUp(cboMaternalComplication3);
                AddDeleteButtonToGridLookUp(cboBirthMethod3);

                // Tab 3: Sinh ?? (Tr?)
                AddDeleteButtonToGridLookUp(cboFoundLocation3);
                AddDeleteButtonToGridLookUp(cboChildStatus3);
                AddDeleteButtonToGridLookUp(cboChildGender3);
                AddDeleteButtonToGridLookUp(cboEthnic3);
                AddDeleteButtonToGridLookUp(cboNewbornCondition3);

                // Tab 4: Tránh thai
                AddDeleteButtonToGridLookUp(cboUser4);
                AddDeleteButtonToGridLookUp(cboDiploma4);
                AddDeleteButtonToGridLookUp(cboContraceptionMethod4);
                AddDeleteButtonToGridLookUp(cboContraceptionComplication4);

                // Tab 5: Phá thai
                AddDeleteButtonToGridLookUp(cboUser5);
                AddDeleteButtonToGridLookUp(cboDiploma5);
                AddDeleteButtonToGridLookUp(cboAbortionMethod5);
                AddDeleteButtonToGridLookUp(cboTissueExaminationResult5);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Thêm button Delete vào GridLookUpEdit
        /// </summary>
        private void AddDeleteButtonToGridLookUp(GridLookUpEdit gridLookUp)
        {
            try
            {
                if (gridLookUp == null) return;

                // Ki?m tra xem ?ã có button Delete ch?a
                bool hasDeleteButton = false;
                foreach (EditorButton btn in gridLookUp.Properties.Buttons)
                {
                    if (btn.Kind == ButtonPredefines.Delete)
                    {
                        hasDeleteButton = true;
                        break;
                    }
                }

                // N?u ch?a có thì thêm button Delete
                if (!hasDeleteButton)
                {
                    EditorButton deleteButton = new EditorButton(ButtonPredefines.Delete);
                    deleteButton.ToolTip = "Xóa l?a ch?n";
                    gridLookUp.Properties.Buttons.Add(deleteButton);
                }

                // ??ng ký s? ki?n ButtonClick n?u ch?a có
                gridLookUp.Properties.ButtonClick -= GridLookUp_ButtonClick;
                gridLookUp.Properties.ButtonClick += GridLookUp_ButtonClick;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// X? lý s? ki?n click button Delete trong GridLookUpEdit
        /// </summary>
        private void GridLookUp_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    GridLookUpEdit gridLookUp = sender as GridLookUpEdit;
                    if (gridLookUp != null)
                    {
                        gridLookUp.EditValue = null;
                        gridLookUp.Properties.NullText = string.Empty;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Kh?i t?o recursively cho t?t c? GridLookUpEdit trong container
        /// </summary>
        private void InitGridLookUpEditDeleteButtonRecursive(Control container)
        {
            try
            {
                if (container == null) return;

                foreach (Control ctrl in container.Controls)
                {
                    if (ctrl is GridLookUpEdit)
                    {
                        AddDeleteButtonToGridLookUp((GridLookUpEdit)ctrl);
                    }
                    else if (ctrl.HasChildren)
                    {
                        InitGridLookUpEditDeleteButtonRecursive(ctrl);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}

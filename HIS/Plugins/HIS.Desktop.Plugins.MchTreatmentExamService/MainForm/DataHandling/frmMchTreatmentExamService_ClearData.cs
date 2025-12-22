using DevExpress.XtraEditors;
using HIS.Desktop.Utilities.Extensions;
using System;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.MchTreatmentExamService.MainForm
{
    public partial class UCMchTreatmentExamService : HIS.Desktop.Utility.FormBase
    {
        #region Clear Data Methods

        private void ClearAllTabsData()
        {
            try
            {
                if (xtraTabControl1 == null) return;

                xtraTabControl1.BeginUpdate();
                try
                {
                    foreach (DevExpress.XtraTab.XtraTabPage tabPage in xtraTabControl1.TabPages)
                    {
                        ClearControlsInContainer(tabPage);
                    }

                    if (addressMother != null)
                    {
                        addressMother.ResetValue();
                    }

                    if (addressBaby != null)
                    {
                        addressBaby.ResetValue();
                    }

                    if (ucSecondaryIcd != null && subIcdProcessor != null)
                    {
                        subIcdProcessor.Reload(ucSecondaryIcd, null);
                    }
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                finally
                {
                    xtraTabControl1.EndUpdate();
                }
                InitAllSpinEditDefaultValue();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Clear toàn b? d? li?u các tab NGO?I TR?: Ngày khám, Ng??i khám, Trình ??
        /// Dùng cho nút New
        /// </summary>
        private void ClearAllTabsDataExceptExamInfo()
        {
            try
            {
                if (xtraTabControl1 == null) return;

                // L?u l?i giá tr? Ngày khám, Ng??i khám, Trình ?? c?a t?t c? các tab
                DateTime? examDate1 = null;
                if (dteExam1 != null && dteExam1.EditValue != null)
                {
                    examDate1 = dteExam1.EditValue as DateTime?;
                }

                object user1 = null;
                if (cboUser1 != null)
                {
                    user1 = cboUser1.EditValue;
                }

                object diploma1 = null;
                if (cboDiploma1 != null)
                {
                    diploma1 = cboDiploma1.EditValue;
                }

                DateTime? examDate2 = null;
                if (dteExam2 != null && dteExam2.EditValue != null)
                {
                    examDate2 = dteExam2.EditValue as DateTime?;
                }

                object user2 = null;
                if (cboUser2 != null)
                {
                    user2 = cboUser2.EditValue;
                }

                object diploma2 = null;
                if (cboDiploma2 != null)
                {
                    diploma2 = cboDiploma2.EditValue;
                }

                DateTime? examDate3 = null;
                if (dteExam3 != null && dteExam3.EditValue != null)
                {
                    examDate3 = dteExam3.EditValue as DateTime?;
                }

                object user3 = null;
                if (cboUser3 != null)
                {
                    user3 = cboUser3.EditValue;
                }

                object diploma3 = null;
                if (cboDiploma3 != null)
                {
                    diploma3 = cboDiploma3.EditValue;
                }

                DateTime? examDate4 = null;
                if (dteExam4 != null && dteExam4.EditValue != null)
                {
                    examDate4 = dteExam4.EditValue as DateTime?;
                }

                object user4 = null;
                if (cboUser4 != null)
                {
                    user4 = cboUser4.EditValue;
                }

                object diploma4 = null;
                if (cboDiploma4 != null)
                {
                    diploma4 = cboDiploma4.EditValue;
                }

                DateTime? examDate5 = null;
                if (dteExam5 != null && dteExam5.EditValue != null)
                {
                    examDate5 = dteExam5.EditValue as DateTime?;
                }

                object user5 = null;
                if (cboUser5 != null)
                {
                    user5 = cboUser5.EditValue;
                }

                object diploma5 = null;
                if (cboDiploma5 != null)
                {
                    diploma5 = cboDiploma5.EditValue;
                }

                // Clear toàn b? d? li?u trong xtraTabControl1
                xtraTabControl1.BeginUpdate();
                try
                {
                    foreach (DevExpress.XtraTab.XtraTabPage tabPage in xtraTabControl1.TabPages)
                    {
                        ClearControlsInContainer(tabPage);
                    }

                    if (addressMother != null)
                    {
                        addressMother.ResetValue();
                    }

                    if (addressBaby != null)
                    {
                        addressBaby.ResetValue();
                    }

                    if (ucSecondaryIcd != null && subIcdProcessor != null)
                    {
                        subIcdProcessor.Reload(ucSecondaryIcd, null);
                    }
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                finally
                {
                    xtraTabControl1.EndUpdate();
                }

                // Clear toàn b? d? li?u trong xtraTabControl2 (Tab M? và Con - Tab 3: Sinh ??)
                if (xtraTabControl2 != null)
                {
                    xtraTabControl2.BeginUpdate();
                    try
                    {
                        foreach (DevExpress.XtraTab.XtraTabPage tabPage in xtraTabControl2.TabPages)
                        {
                            ClearControlsInContainer(tabPage);
                        }

                        Inventec.Common.Logging.LogSystem.Debug("ClearAllTabsDataExceptExamInfo: Cleared xtraTabControl2 (Mother & Child tabs)");
                    }
                    catch (Exception ex)
                    {
                        Inventec.Common.Logging.LogSystem.Warn("Error clearing xtraTabControl2: " + ex.Message);
                    }
                    finally
                    {
                        xtraTabControl2.EndUpdate();
                    }
                }

                // Restore l?i giá tr? Ngày khám, Ng??i khám, Trình ??
                if (dteExam1 != null && examDate1.HasValue)
                {
                    dteExam1.EditValue = examDate1.Value;
                }
                if (cboUser1 != null && user1 != null)
                {
                    cboUser1.EditValue = user1;
                }
                if (cboDiploma1 != null && diploma1 != null)
                {
                    cboDiploma1.EditValue = diploma1;
                }

                if (dteExam2 != null && examDate2.HasValue)
                {
                    dteExam2.EditValue = examDate2.Value;
                }
                if (cboUser2 != null && user2 != null)
                {
                    cboUser2.EditValue = user2;
                }
                if (cboDiploma2 != null && diploma2 != null)
                {
                    cboDiploma2.EditValue = diploma2;
                }

                if (dteExam3 != null && examDate3.HasValue)
                {
                    dteExam3.EditValue = examDate3.Value;
                }
                if (cboUser3 != null && user3 != null)
                {
                    cboUser3.EditValue = user3;
                }
                if (cboDiploma3 != null && diploma3 != null)
                {
                    cboDiploma3.EditValue = diploma3;
                }

                if (dteExam4 != null && examDate4.HasValue)
                {
                    dteExam4.EditValue = examDate4.Value;
                }
                if (cboUser4 != null && user4 != null)
                {
                    cboUser4.EditValue = user4;
                }
                if (cboDiploma4 != null && diploma4 != null)
                {
                    cboDiploma4.EditValue = diploma4;
                }

                if (dteExam5 != null && examDate5.HasValue)
                {
                    dteExam5.EditValue = examDate5.Value;
                }
                if (cboUser5 != null && user5 != null)
                {
                    cboUser5.EditValue = user5;
                }
                if (cboDiploma5 != null && diploma5 != null)
                {
                    cboDiploma5.EditValue = diploma5;
                }


                GridCheckMarksSelection gridCheckMarkChiSo = cboMedicalHistoryInternal2.Properties.Tag as GridCheckMarksSelection;
                if (gridCheckMarkChiSo != null)
                {
                    gridCheckMarkChiSo.ClearSelection(cboMedicalHistoryInternal2.Properties.View);
                    MedicalHistoryInternal2Selected = new System.Collections.Generic.List<ADO.KeyValueADO>();
                    cboMedicalHistoryInternal2.Focus();
                    btnNew.Select();
                }
                // Đặt lại giá trị mặc định cho các radio group bắt buộc
                SetDefaultRequiredRadioGroups();
                InitAllSpinEditDefaultValue();
                Inventec.Common.Logging.LogSystem.Debug("ClearAllTabsDataExceptExamInfo: Cleared data except Exam Date, User, Diploma. Set default for required radio groups.");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ClearControlsInContainer(Control container)
        {
            try
            {
                if (container == null || !container.HasChildren) return;

                foreach (Control control in container.Controls)
                {
                    if (control is DevExpress.XtraLayout.LayoutControl)
                    {
                        DevExpress.XtraLayout.LayoutControl layoutControl = control as DevExpress.XtraLayout.LayoutControl;
                        if (!layoutControl.IsInitialized) continue;

                        layoutControl.BeginUpdate();
                        try
                        {
                            foreach (DevExpress.XtraLayout.BaseLayoutItem item in layoutControl.Items)
                            {
                                DevExpress.XtraLayout.LayoutControlItem lci = item as DevExpress.XtraLayout.LayoutControlItem;
                                if (lci != null && lci.Control != null)
                                {
                                    ClearSingleControl(lci.Control);
                                }
                            }
                        }
                        finally
                        {
                            layoutControl.EndUpdate();
                        }
                    }
                    else if (control.HasChildren)
                    {
                        ClearControlsInContainer(control);
                    }
                    else
                    {
                        ClearSingleControl(control);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ClearSingleControl(Control control)
        {
            try
            {
                if (control == null) return;

                if (control is TextEdit)
                {
                    TextEdit textEdit = control as TextEdit;
                    textEdit.EditValue = null;
                    textEdit.Text = string.Empty;
                }
                else if (control is MemoEdit)
                {
                    MemoEdit memoEdit = control as MemoEdit;
                    memoEdit.EditValue = null;
                    memoEdit.Text = string.Empty;
                }
                else if (control is SpinEdit)
                {
                    SpinEdit spinEdit = control as SpinEdit;

                    // Đảm bảo Properties cho phép null
                    if (spinEdit.Properties.AllowNullInput == DevExpress.Utils.DefaultBoolean.False)
                    {
                        spinEdit.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;
                    }
                    // QUAN TRỌNG: Set về null, KHÔNG phải 0
                    spinEdit.EditValue = null;

                }
                else if (control is DateEdit)
                {
                    DateEdit dateEdit = control as DateEdit;
                    dateEdit.EditValue = null;
                }
                else if (control is GridLookUpEdit)
                {
                    GridLookUpEdit gridLookUpEdit = control as GridLookUpEdit;
                    gridLookUpEdit.EditValue = null;
                }
                else if (control is CheckEdit)
                {
                    CheckEdit checkEdit = control as CheckEdit;
                    checkEdit.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion
    }
}

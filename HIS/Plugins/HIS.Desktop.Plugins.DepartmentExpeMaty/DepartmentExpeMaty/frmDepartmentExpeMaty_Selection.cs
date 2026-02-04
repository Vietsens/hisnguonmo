using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using HIS.Desktop.LocalStorage.BackendData;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Common.Controls.PopupLoader;
using Inventec.Common.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.DepartmentExpeMaty.DepartmentExpeMaty
{
    public partial class frmDepartmentExpeMaty : HIS.Desktop.Utility.FormBase
    {
        private void InitCombo(GridLookUpEdit cbo)
        {
            try
            {
                // Attach event
                cbo.Closed += new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.cbo_Closed);
                cbo.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.cbo_ButtonClick);
                //cbo.KeyUp += new System.Windows.Forms.KeyEventHandler(this.cbo_KeyUp);
                cbo.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.cbo_PreviewKeyDown);
                cbo.EditValueChanged += new System.EventHandler(this.cbo_EditValueChanged);
                // Load data
                var currentBranchs = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_BRANCH>();
                var branch = currentBranchs.SingleOrDefault(o => o.ID == BranchDataWorker.GetCurrentBranchId());
                if (branch == null && currentBranchs != null && currentBranchs.Count > 0)
                {
                    branch = currentBranchs[0];
                }
                if (cbo == this.cboMaty)
                {
                    try
                    {
                        txtMaty.PreviewKeyDown += (s, e) =>
                        {
                            try
                            {
                                if (e.KeyCode == Keys.Enter)
                                {
                                    var text = s as TextEdit;
                                    var source = cbo.Properties.DataSource as List<MOS.EFMODEL.DataModels.HIS_MATERIAL_TYPE>;
                                    cbo.EditValue = source.FirstOrDefault(o => o.MATERIAL_TYPE_CODE == text.Text.Trim())?.ID;
                                    if (cbo.EditValue != null)
                                    {
                                        cbo.Focus();
                                        cbo.SelectAll();
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                LogSystem.Error(ex);
                            }
                        };
                        cbo.Closed += (s, e) =>
                        {
                            try
                            {
                                if (e.CloseMode == PopupCloseMode.Normal)
                                {
                                    var cboSender = s as GridLookUpEdit;
                                    var selected = cboSender.Properties.DataSource as List<MOS.EFMODEL.DataModels.HIS_MATERIAL_TYPE>;
                                    txtMaty.Text = selected?.FirstOrDefault(o => o.ID == (long)(cboSender.EditValue ?? 0))?.MATERIAL_TYPE_CODE;
                                }
                            }
                            catch (Exception ex)
                            {
                                LogSystem.Error(ex);
                            }
                        };
                        cbo.ButtonClick += (s, e) => 
                        {
                            try
                            {
                                var cboSender = s as GridLookUpEdit;
                                if (e.Button.Kind == ButtonPredefines.Delete)
                                {
                                    txtMaty.Text = string.Empty;
                                }
                            }
                            catch (Exception ex)
                            {
                                LogSystem.Error(ex);
                            }
                        };
                        // Load material type
                        var matys = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_MATERIAL_TYPE>().Where(o =>
                        o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
                        List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                        columnInfos.Add(new ColumnInfo("MATERIAL_TYPE_CODE", "", 150, 1));
                        columnInfos.Add(new ColumnInfo("MATERIAL_TYPE_NAME", "", 350, 2));
                        ControlEditorADO controlEditorADO = new ControlEditorADO("MATERIAL_TYPE_NAME", "ID", columnInfos, false, 440);
                        ControlEditorLoader.Load(cbo, matys, controlEditorADO);

                    }
                    catch (Exception ex)
                    {
                        Inventec.Common.Logging.LogSystem.Warn(ex);
                    }
                }
                else if (cbo == this.cboDepartment)
                {
                    try
                    {
                        txtDepartment.PreviewKeyDown += (s, e) =>
                        {
                            try
                            {
                                if (e.KeyCode == Keys.Enter)
                                {
                                    var text = s as TextEdit;
                                    var source = cbo.Properties.DataSource as List<MOS.EFMODEL.DataModels.HIS_DEPARTMENT>;
                                    cbo.EditValue = source?.FirstOrDefault(o => o.DEPARTMENT_CODE == text.Text.Trim())?.ID;
                                    if (cbo.EditValue != null)
                                    {
                                        cbo.Focus();
                                        cbo.SelectAll();
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                LogSystem.Error(ex);
                            }
                        };
                        cbo.Closed += (s, e) =>
                        {
                            try
                            {
                                if (e.CloseMode == PopupCloseMode.Normal)
                                {
                                    var cboSender = s as GridLookUpEdit;
                                    var selected = cboSender.Properties.DataSource as List<MOS.EFMODEL.DataModels.HIS_DEPARTMENT>;
                                    txtDepartment.Text = selected?.FirstOrDefault(o => o.ID == (long)(cboSender.EditValue ?? 0))?.DEPARTMENT_CODE;
                                }
                            }
                            catch (Exception ex)
                            {
                                LogSystem.Error(ex);
                            }
                        };  
                        cbo.ButtonClick += (s, e) =>
                        {
                            try
                            {
                                var cboSender = s as GridLookUpEdit;
                                if (e.Button.Kind == ButtonPredefines.Delete)
                                {
                                    txtDepartment.Text = string.Empty;
                                }
                            }
                            catch (Exception ex)
                            {
                                LogSystem.Error(ex);
                            }
                        };
                        //
                        var departments = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_DEPARTMENT>().Where(o => o.BRANCH_ID == branch.ID && o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
                        List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                        columnInfos.Add(new ColumnInfo("DEPARTMENT_CODE", "", 120, 1));
                        columnInfos.Add(new ColumnInfo("DEPARTMENT_NAME", "", 350, 2));
                        ControlEditorADO controlEditorADO = new ControlEditorADO("DEPARTMENT_NAME", "ID", columnInfos, false, 440);
                        ControlEditorLoader.Load(cbo, departments, controlEditorADO);
                    }
                    catch (Exception ex)
                    {
                        Inventec.Common.Logging.LogSystem.Warn(ex);
                    }
                }
                else if (cbo == this.cboStock)
                {
                    try
                    {
                        var stocks = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_MEDI_STOCK>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
                        List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                        columnInfos.Add(new ColumnInfo("MEDI_STOCK_CODE", "", 120, 1));
                        columnInfos.Add(new ColumnInfo("MEDI_STOCK_NAME", "", 350, 2));
                        ControlEditorADO controlEditorADO = new ControlEditorADO("MEDI_STOCK_NAME", "ID", columnInfos, false, 440);
                        ControlEditorLoader.Load(cbo, stocks, controlEditorADO);
                    }
                    catch (Exception ex)
                    {
                        Inventec.Common.Logging.LogSystem.Warn(ex);
                    }
                }

            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private BaseEdit[] navigationOrder;
        private BaseEdit GetNextControl(BaseEdit current)
        {
            if (navigationOrder == null)
            {
                navigationOrder = new BaseEdit[] { txtMaty, cboMaty, txtDepartment, cboDepartment, cboStock, spinMaxExpend };
            }
            var index = Array.IndexOf(navigationOrder, current);
            if (index == -1) return null;
            return index < navigationOrder.Length - 1 ? navigationOrder[index + 1] : navigationOrder[0];
        }
        private void FocusNextControl(BaseEdit current)
        {
            var next = GetNextControl(current);
            if (next != null)
            {
                next.Focus();
                next.SelectAll();
            }
        }
        private void VisibleDeleteButton(GridLookUpEdit cbo, bool isVisible)
        {
            try
            {
                foreach (EditorButton item in cbo.Properties.Buttons)
                {
                    if (item != null && item.Kind == ButtonPredefines.Delete)
                    {
                        item.Visible = isVisible;
                    }
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }
        private void cbo_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                var cbo = sender as GridLookUpEdit;
                if (!cbo.IsEditorActive)
                {
                    VisibleDeleteButton(cbo, cbo.EditValue != null);
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }
        private void cbo_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var cbo = sender as GridLookUpEdit;
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                {
                    VisibleDeleteButton(cbo, false);
                    cbo.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void cbo_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    var cbo = sender as GridLookUpEdit;
                    if (cbo.IsEditorActive)
                    {
                        VisibleDeleteButton(cbo, cbo.EditValue != null);
                        FocusNextControl(cbo);
                    }
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void cbo_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                var cbo = sender as GridLookUpEdit;
                if (e.KeyCode == Keys.Enter)
                {
                    VisibleDeleteButton(cbo, cbo.EditValue != null);
                    if (cbo.EditValue != null)
                    {
                        FocusNextControl(cbo);
                    }
                }
                else
                {
                    //if (e.KeyCode == Keys.Delete || e.KeyCode == Keys.Back)
                    //{
                    //    if (cbo.SelectionLength > 0 && cbo.SelectedText == cbo.Text)
                    //    {
                    //        cbo.EditValue = null;
                    //        VisibleDeleteButton_CboMaty(cbo);
                    //    }
                    //}
                    cbo.ShowPopup();
                    PopupLoader.SelectFirstRowPopup(cbo);
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }
    }
}

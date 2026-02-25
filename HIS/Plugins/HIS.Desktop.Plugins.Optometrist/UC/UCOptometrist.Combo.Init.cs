using ACS.EFMODEL.DataModels;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Utility;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Common.Integrate;
using Inventec.Common.Logging;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.Optometrist.UC
{
    public partial class UCOptometrist : UserControlBase
    {
        private List<ACS_USER> listDataUser = new List<ACS_USER>();

        private void InitExecuteName()
        {
            try
            {
                List<ACS_USER> datas = BackendDataWorker.Get<ACS_USER>();
                List<HIS_EMPLOYEE> employeeList = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_EMPLOYEE>();
                listDataUser = new List<ACS_USER>();

                foreach (var item in employeeList)
                {
                    if (String.IsNullOrWhiteSpace(item.LOGINNAME)) continue;

                    ACS_USER user = new ACS_USER();
                    user.LOGINNAME = item.LOGINNAME;
                    var check = datas.FirstOrDefault(o => o.LOGINNAME == item.LOGINNAME);
                    if (check != null)
                    {
                        user.USERNAME = check.USERNAME;
                        user.MOBILE = check.MOBILE;
                        user.PASSWORD = check.PASSWORD;
                    }

                    listDataUser.Add(user);
                }

                listDataUser = listDataUser.OrderBy(o => o.USERNAME).ToList();

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("LOGINNAME", "", 150, 1));
                columnInfos.Add(new ColumnInfo("USERNAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("USERNAME", "LOGINNAME", columnInfos, false, 750);
                ControlEditorLoader.Load(VISION_TEST_USERNAME, listDataUser, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void InitExcuteRoom()
        {
            try
            {
                //var serviceRoomViews = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_SERVICE_ROOM>();
                var executeRoomViews = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_EXECUTE_ROOM>();
                //if (excuteRoomCombo != null && executeRoomViews != null && serviceRoomViews != null && serviceRoomViews.Count > 0)
                {
                    //var arrExcuteRoomCode = serviceRoomViews.Where(o => data != null && o.SERVICE_ID == data.SERVICE_ID).Select(o => o.ROOM_ID).ToList();
                    //if (arrExcuteRoomCode != null && arrExcuteRoomCode.Count > 0)
                    {
                        //List<MOS.EFMODEL.DataModels.V_HIS_EXECUTE_ROOM> dataCombo = executeRoomViews.Where(o => arrExcuteRoomCode.Contains(o.ROOM_ID)).ToList();
                        this.InitComboExecuteRoom(VISION_TEST_ROOM_NAME, executeRoomViews);
                    }
                    //else
                    //{
                    //    this.InitComboExecuteRoom(excuteRoomCombo, null);
                    //}
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void InitComboExecuteRoom(DevExpress.XtraEditors.GridLookUpEdit excuteRoomCombo, List<MOS.EFMODEL.DataModels.V_HIS_EXECUTE_ROOM> data)
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("EXECUTE_ROOM_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("EXECUTE_ROOM_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("EXECUTE_ROOM_NAME", "ID", columnInfos, false, 350);
                ControlEditorLoader.Load(excuteRoomCombo, data, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        //
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
                //var currentBranchs = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_BRANCH>();
                //var branch = currentBranchs.SingleOrDefault(o => o.ID == BranchDataWorker.GetCurrentBranchId());
                //if (branch == null && currentBranchs != null && currentBranchs.Count > 0)
                //{
                //    branch = currentBranchs[0];
                //}
                if (cbo == this.RIGHT_ICD_NAME)
                {
                    try
                    {
                        RIGHT_ICD_CODE.PreviewKeyDown += (s, e) =>
                        {
                            try
                            {
                                if (e.KeyCode == Keys.Enter)
                                {
                                    var text = s as TextEdit;
                                    var source = cbo.Properties.DataSource as List<MOS.EFMODEL.DataModels.HIS_ICD>;
                                    cbo.EditValue = source.FirstOrDefault(o => o.ICD_CODE == text.Text.Trim())?.ICD_CODE;
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
                                    var selected = cboSender.Properties.DataSource as List<MOS.EFMODEL.DataModels.HIS_ICD>;
                                    RIGHT_ICD_CODE.Text = selected?.FirstOrDefault(o => o.ICD_CODE == cboSender.EditValue.ToString())?.ICD_CODE;
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
                                    RIGHT_ICD_CODE.Text = string.Empty;
                                }
                            }
                            catch (Exception ex)
                            {
                                LogSystem.Error(ex);
                            }
                        };
                        // Load material type
                        var matys = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_ICD>().Where(o =>
                        o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
                        List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                        columnInfos.Add(new ColumnInfo("ICD_CODE", "", 150, 1));
                        columnInfos.Add(new ColumnInfo("ICD_NAME", "", 350, 2));
                        ControlEditorADO controlEditorADO = new ControlEditorADO("ICD_NAME", "ICD_CODE", columnInfos, false, 440);
                        ControlEditorLoader.Load(cbo, matys, controlEditorADO);

                    }
                    catch (Exception ex)
                    {
                        Inventec.Common.Logging.LogSystem.Warn(ex);
                    }
                }
                else if (cbo == this.LEFT_ICD_NAME)
                {
                    try
                    {
                        LEFT_ICD_CODE.PreviewKeyDown += (s, e) =>
                        {
                            try
                            {
                                if (e.KeyCode == Keys.Enter)
                                {
                                    var text = s as TextEdit;
                                    var source = cbo.Properties.DataSource as List<MOS.EFMODEL.DataModels.HIS_ICD>;
                                    cbo.EditValue = source?.FirstOrDefault(o => o.ICD_CODE == text.Text.Trim())?.ICD_CODE;
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
                                    var selected = cboSender.Properties.DataSource as List<MOS.EFMODEL.DataModels.HIS_ICD>;
                                    LEFT_ICD_CODE.Text = selected?.FirstOrDefault(o => o.ICD_CODE == cboSender.EditValue.ToString())?.ICD_CODE;
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
                                    LEFT_ICD_CODE.Text = string.Empty;
                                }
                            }
                            catch (Exception ex)
                            {
                                LogSystem.Error(ex);
                            }
                        };
                        //
                        var departments = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_ICD>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
                        List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                        columnInfos.Add(new ColumnInfo("ICD_CODE", "", 150, 1));
                        columnInfos.Add(new ColumnInfo("ICD_NAME", "", 350, 2));
                        ControlEditorADO controlEditorADO = new ControlEditorADO("ICD_NAME", "ICD_CODE", columnInfos, false, 440);
                        ControlEditorLoader.Load(cbo, departments, controlEditorADO);
                    }
                    catch (Exception ex)
                    {
                        Inventec.Common.Logging.LogSystem.Warn(ex);
                    }
                }
                else if (cbo == this.BOTH_ICD_NAME)
                {
                    try
                    {
                        BOTH_ICD_CODE.PreviewKeyDown += (s, e) =>
                        {
                            try
                            {
                                if (e.KeyCode == Keys.Enter)
                                {
                                    var text = s as TextEdit;
                                    var source = cbo.Properties.DataSource as List<MOS.EFMODEL.DataModels.HIS_ICD>;
                                    cbo.EditValue = source?.FirstOrDefault(o => o.ICD_CODE == text.Text.Trim())?.ICD_CODE;
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
                                    var selected = cboSender.Properties.DataSource as List<MOS.EFMODEL.DataModels.HIS_ICD>;
                                    BOTH_ICD_CODE.Text = selected?.FirstOrDefault(o => o.ICD_CODE == cboSender.EditValue.ToString())?.ICD_CODE;
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
                                    BOTH_ICD_CODE.Text = string.Empty;
                                }
                            }
                            catch (Exception ex)
                            {
                                LogSystem.Error(ex);
                            }
                        };
                        //
                        var departments = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_ICD>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
                        List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                        columnInfos.Add(new ColumnInfo("ICD_CODE", "", 150, 1));
                        columnInfos.Add(new ColumnInfo("ICD_NAME", "", 350, 2));
                        ControlEditorADO controlEditorADO = new ControlEditorADO("ICD_NAME", "ICD_CODE", columnInfos, false, 440);
                        ControlEditorLoader.Load(cbo, departments, controlEditorADO);
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

        //private BaseEdit[] navigationOrder;
        //private BaseEdit GetNextControl(BaseEdit current)
        //{
        //    if (navigationOrder == null)
        //    {
        //        navigationOrder = new BaseEdit[] { txtMaty, cboMaty, txtDepartment, cboDepartment, cboStock, spinMaxExpend };
        //    }
        //    var index = Array.IndexOf(navigationOrder, current);
        //    if (index == -1) return null;
        //    return index < navigationOrder.Length - 1 ? navigationOrder[index + 1] : navigationOrder[0];
        //}
        //private void FocusNextControl(BaseEdit current)
        //{
        //    var next = GetNextControl(current);
        //    if (next != null)
        //    {
        //        next.Focus();
        //        next.SelectAll();
        //    }
        //}
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
                        //FocusNextControl(cbo);
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
                if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Tab)
                {
                    VisibleDeleteButton(cbo, cbo.EditValue != null);
                    if (cbo.EditValue != null)
                    {
                        //FocusNextControl(cbo);
                    }
                }
                else
                {
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
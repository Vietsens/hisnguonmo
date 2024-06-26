/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2017 INVENTEC
 *  
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *  
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
 * GNU General Public License for more details.
 *  
 * You should have received a copy of the GNU General Public License
 * along with this program. If not, see <http://www.gnu.org/licenses/>.
 */
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.HisAssignBlood.ADO;
using Inventec.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.HisAssignBlood
{
    public partial class frmHisAssignBlood : HIS.Desktop.Utility.FormBase
    {
        private void cboBloodABO_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    this.cboBloodABO.Properties.Buttons[1].Visible = false;
                    this.cboBloodABO.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboBloodABO_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (this.cboBloodABO.EditValue != null)
                    {
                        this.cboBloodABO.Properties.Buttons[1].Visible = true;
                    }
                    this.cboBloodRH.Focus();
                    this.cboBloodRH.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboBloodABO_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (this.cboBloodABO.EditValue != null)
                    {
                        this.cboBloodABO.Properties.Buttons[1].Visible = true;
                    }
                    this.cboBloodRH.Focus();
                    this.cboBloodRH.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboBloodRH_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    this.cboBloodRH.Properties.Buttons[1].Visible = false;
                    this.cboBloodRH.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboBloodRH_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (this.cboBloodRH.EditValue != null)
                    {
                        this.cboBloodRH.Properties.Buttons[1].Visible = true;
                    }
                    this.btnAddBlood.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboBloodRH_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (this.cboBloodRH.EditValue != null)
                    {
                        this.cboBloodRH.Properties.Buttons[1].Visible = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboBloodRH_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    this.btnAddBlood.Focus();
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spinAmount__BloodPage_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    this.cboBloodABO.Focus();
                    this.cboBloodABO.SelectAll();
                    this.cboBloodABO.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboMediStockExport_TabBlood_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (this.cboMediStockExport_TabBlood.EditValue != null)
                    {
                        MOS.EFMODEL.DataModels.V_HIS_MEDI_STOCK paty = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_MEDI_STOCK>().SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((this.cboMediStockExport_TabBlood.EditValue ?? "0").ToString()));
                        if (paty != null)
                        {
                            this.LoadDataToGridBloodType(paty);
                            this.gridControlServiceProcess__TabBlood.DataSource = null;
                            this.EnableAndDisableControlWithGirdcontrol();
                        }
                    }

                    this.txtKeyword.Focus();
                    this.txtKeyword.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboMediStockExport_TabBlood_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (this.cboMediStockExport_TabBlood.EditValue != null)
                    {
                        MOS.EFMODEL.DataModels.V_HIS_MEDI_STOCK paty = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_MEDI_STOCK>().SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((this.cboMediStockExport_TabBlood.EditValue ?? "0").ToString()));
                        if (paty != null)
                        {
                            this.LoadDataToGridBloodType(paty);
                            this.gridControlServiceProcess__TabBlood.DataSource = null;
                            this.EnableAndDisableControlWithGirdcontrol();
                        }

                        this.txtKeyword.Focus();
                        this.txtKeyword.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void repositoryItemBtnDelete__BloodPage_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (actionType == GlobalVariables.ActionView || actionType == GlobalVariables.ActionViewForEdit)
                    return;

                CommonParam param = new CommonParam();
                var servModel = (BloodTypeADO)this.gridViewServiceProcess__TabBlood.GetFocusedRow();
                if (servModel != null)
                {
                    this.ListBloodTypeADOProcess.Remove(servModel);
                    this.gridControlServiceProcess__TabBlood.DataSource = null;
                    this.gridControlServiceProcess__TabBlood.DataSource = this.ListBloodTypeADOProcess;

                    this.EnableAndDisableControlWithGirdcontrol();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtKeyword_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                //if (e.KeyCode == Keys.Down)
                //{
                //    var rowCount = (gridViewBloodType__BloodPage.DataSource as List<BloodADO>).Count();
                //    if (rowCount == 1)
                //    {
                //        gridViewBloodType__BloodPage.Focus();
                //        gridViewBloodType__BloodPage.FocusedRowHandle = 0;
                //    }
                //    else if (rowCount > 1)
                //    {
                //        gridViewBloodType__BloodPage.Focus();
                //        gridViewBloodType__BloodPage.FocusedRowHandle = 1;
                //    }
                //    else
                //    {
                //        //Nothing
                //    }
                //}

                //else
                //{
                //    bloodGroups = bloodGroups ?? new List<BloodADO>();
                //    List<BloodADO> data = null;
                //    gridViewBloodType__BloodPage.BeginUpdate();
                //    if (!String.IsNullOrEmpty(txtKeyword.Text.Trim()))
                //    {
                //        data = bloodGroups.Where(o =>
                //                                ((o.BLOOD_TYPE_CODE ?? "").ToLower().Contains(txtKeyword.Text.Trim().ToLower())
                //                                || (o.BLOOD_RH_CODE ?? "").ToLower().Contains(txtKeyword.Text.Trim().ToLower())
                //                                || (o.BLOOD_ABO_CODE ?? "").ToLower().Contains(txtKeyword.Text.Trim().ToLower())
                //                                || (o.BLOOD_TYPE_NAME ?? "").ToLower().Contains(txtKeyword.Text.Trim().ToLower()))
                //                                ).ToList();
                //    }
                //    else
                //    {
                //        data = bloodGroups;
                //    }

                //    gridViewBloodType__BloodPage.GridControl.DataSource = data;
                //    gridViewBloodType__BloodPage.EndUpdate();
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtKeyword_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Down)
                {
                    var rowCount = (this.gridViewBloodType__BloodPage.DataSource as List<BloodADO>).Count();
                    if (rowCount == 1)
                    {
                        this.gridViewBloodType__BloodPage.Focus();
                        this.gridViewBloodType__BloodPage.FocusedRowHandle = 0;
                    }
                    else if (rowCount > 1)
                    {
                        this.gridViewBloodType__BloodPage.Focus();
                        this.gridViewBloodType__BloodPage.FocusedRowHandle = 1;
                    }
                    else
                    {
                        //Nothing
                    }
                }
                else if (e.KeyCode == Keys.Enter)
                {
                    this.gridViewBloodType__BloodPage.FocusedRowHandle = 0;
                    this.gridControlBloodType__BloodPage_Click(this.gridViewBloodType__BloodPage.GetFocusedRow(), null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtKeyword_TextChanged(object sender, EventArgs e)
        {
            try
            {
                this.bloodGroups = bloodGroups ?? new List<BloodADO>();
                List<BloodADO> data = null;
                this.gridViewBloodType__BloodPage.BeginUpdate();
                if (!String.IsNullOrEmpty(txtKeyword.Text.Trim()))
                {
                    data = this.bloodGroups.Where(o =>
                                            ((o.SERVICE_CODE_HIDDEN ?? "").ToLower().Contains(txtKeyword.Text.Trim().ToLower())
                                            //|| (o.MEDI_STOCK_CODE ?? "").ToLower().Contains(txtKeyword.Text.Trim().ToLower())
                                            //|| (o.MEDI_STOCK_NAME ?? "").ToLower().Contains(txtKeyword.Text.Trim().ToLower())
                                            || (o.SERVICE_NAME_HIDDEN ?? "").ToLower().Contains(txtKeyword.Text.Trim().ToLower()))
                                            ).ToList();
                }
                else
                {
                    data = this.bloodGroups;
                }

                this.gridViewBloodType__BloodPage.GridControl.DataSource = data;
                this.gridViewBloodType__BloodPage.EndUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtMediStockExportCode_TabBlood_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string strValue = (sender as DevExpress.XtraEditors.TextEdit).Text.ToUpper();
                    this.LoadMediStockExportCombo(strValue);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboExecuteGroup_TabBlood_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (this.cboExecuteGroup_TabBlood.EditValue != null)
                    {
                        this.cboExecuteGroup_TabBlood.Properties.Buttons[1].Visible = true;
                    }

                    this.txtKeyword.Focus();
                    this.txtKeyword.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboMediStockExport_TabBlood_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    this.cboMediStockExport_TabBlood.EditValue = null;
                    this.gridControlBloodType__BloodPage.DataSource = null;
                    this.gridControlServiceProcess__TabBlood.DataSource = null;
                    this.actionBosung = GlobalVariables.ActionAdd;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboExecuteGroup_TabBlood_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    this.cboExecuteGroup_TabBlood.EditValue = null;
                    this.cboExecuteGroup_TabBlood.Properties.Buttons[1].Visible = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}

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
using DevExpress.XtraEditors.Controls;
using HIS.Desktop.LocalStorage.BackendData;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.ExpMestChmsUpdate
{
    public partial class frmExpMestChmsUpdate : HIS.Desktop.Utility.FormBase
    {
        private void cboImpMediStock_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                //if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                //{
                //    txtExpMediStock.Focus();
                //    txtExpMediStock.SelectAll();
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboImpMediStock_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                //WaitingManager.Show();
                //V_HIS_MEDI_STOCK stock = null;
                //if (cboImpMediStock.EditValue != null)
                //{
                //    stock = BackendDataWorker.Get<V_HIS_MEDI_STOCK>().FirstOrDefault(o => o.ID == Convert.ToInt64(cboImpMediStock.EditValue));
                //}
                //if (stock != null)
                //{
                //    txtImpMediStock.Properties.Buttons[1].Visible = true;
                //    txtImpMediStock.Text = stock.MEDI_STOCK_NAME;
                //}
                //else
                //{
                //    txtImpMediStock.Properties.Buttons[1].Visible = false;
                //    txtImpMediStock.Text = "";
                //}
                //FillDataToCboExpMestByImpMest(stock);
                //FillDataToCboMediStock(true);
                //ResetValueControlCommon();
                //ResetGridControlDetail();
                //FillDataToGridExpMest();
                //if (cboExpMediStock.EditValue == null)
                //{
                //    LoadDataToTreeList(null);
                //}
                //WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtImpMediStock_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                //if (e.KeyCode == Keys.Enter)
                //{
                //    bool valid = false;
                //    if (!String.IsNullOrEmpty(txtImpMediStock.Text))
                //    {
                //        string key = txtImpMediStock.Text.ToLower();
                //        var listData = listImpMediStock.Where(o => o.MEDI_STOCK_CODE.ToLower().Contains(key) || o.MEDI_STOCK_NAME.ToLower().Contains(key)).ToList();
                //        if (listData != null && listData.Count == 1)
                //        {
                //            valid = true;
                //            cboImpMediStock.EditValue = listData.First().ID;
                //        }
                //    }
                //    if (!valid)
                //    {
                //        cboImpMediStock.Focus();
                //        cboImpMediStock.ShowPopup();
                //    }
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtImpMediStock_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Down)
                {
                    cboImpMediStock.Focus();
                    cboImpMediStock.ShowPopup();
                }
                else if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboImpMediStock.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtImpMediStock_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                //if (txtImpMediStock.EditValue != txtImpMediStock.OldEditValue && String.IsNullOrEmpty(txtImpMediStock.Text) && cboImpMediStock.EditValue != null)
                //{
                //    cboImpMediStock.EditValue = null;
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboExpMediStock_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    txtDescription.Focus();
                    txtDescription.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboExpMediStock_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                //WaitingManager.Show();
                //V_HIS_MEDI_STOCK mestRoom = null;
                //if (cboExpMediStock.EditValue != null)
                //{
                //    mestRoom = BackendDataWorker.Get<V_HIS_MEDI_STOCK>().FirstOrDefault(o => o.ID == Convert.ToInt64(cboExpMediStock.EditValue));
                //}
                //if (mestRoom != null)
                //{
                //    txtExpMediStock.Properties.Buttons[1].Visible = true;
                //    txtExpMediStock.Text = mestRoom.MEDI_STOCK_NAME;
                //}
                //else
                //{
                //    txtExpMediStock.Properties.Buttons[1].Visible = false;
                //    txtExpMediStock.Text = "";
                //}
                //FillDataToCboMediStock(false);
                //LoadDataToTreeList(mestRoom);
                //ResetGridControlDetail();

                //WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtExpMediStock_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Down)
                {
                    cboExpMediStock.Focus();
                    cboExpMediStock.ShowPopup();
                }
                else if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboExpMediStock.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtExpMediStock_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                //if (e.KeyCode == Keys.Enter)
                //{
                //    bool valid = false;
                //    if (!String.IsNullOrEmpty(txtExpMediStock.Text))
                //    {
                //        string key = txtExpMediStock.Text.ToLower();
                //        var listData = listExpMediStock.Where(o => o.MEDI_STOCK_CODE.ToLower().Contains(key) || o.MEDI_STOCK_NAME.ToLower().Contains(key)).ToList();
                //        if (listData != null && listData.Count == 1)
                //        {
                //            valid = true;
                //            cboExpMediStock.EditValue = listData.First().ID;
                //        }
                //    }
                //    if (!valid)
                //    {
                //        cboExpMediStock.Focus();
                //        cboExpMediStock.ShowPopup();
                //    }
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtExpMediStock_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                //if (txtExpMediStock.EditValue != txtExpMediStock.OldEditValue && String.IsNullOrEmpty(txtExpMediStock.Text) && cboExpMediStock.EditValue != null)
                //{
                //    cboExpMediStock.EditValue = null;
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtDescription_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (xtraTabControlMain.SelectedTabPageIndex == 0)
                    {
                        txtSearchMedicine.Focus();
                    }
                    else if (xtraTabControlMain.SelectedTabPageIndex == 1)
                    {
                        txtSearchMaterial.Focus();
                    }
                    else
                    {
                        txtSearch.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void spinExpAmount_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtNote.Focus();
                    txtNote.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtNote_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    SendKeys.Send("{TAB}");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void xtraTabControlMain_SelectedPageChanged(object sender, DevExpress.XtraTab.TabPageChangedEventArgs e)
        {
            try
            {
                try
                {
                    if (xtraTabControlMain.SelectedTabPageIndex == 0)
                    {
                        txtSearchMedicine.Focus();
                        lciABO.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                        lciRH.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    }
                    else if (xtraTabControlMain.SelectedTabPageIndex == 1)
                    {
                        txtSearchMaterial.Focus();
                        lciABO.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                        lciRH.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    }
                    else if (xtraTabControlMain.SelectedTabPageIndex == 2)
                    {
                        txtSearch.Focus();
                        lciABO.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                        lciRH.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    }
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Error(ex);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void radioImport_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                LoadDataToCboMediStock();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkLinh_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                LoadDataToCboMediStock();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void chkTra_CheckedChanged(object sender, EventArgs e)
        {
            //try
            //{
            //    if (chkTra.Checked)
            //    {
            //        chkLinh.Checked = false;
            //    }
            //    else
            //    {
            //        chkLinh.Checked = true;
            //    }
            //}
            //catch (Exception ex)
            //{
            //    Inventec.Common.Logging.LogSystem.Warn(ex);
            //}

        }

        private void barThem_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                BtnAdd();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void barLuu_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                BtnSave();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }
    }
}

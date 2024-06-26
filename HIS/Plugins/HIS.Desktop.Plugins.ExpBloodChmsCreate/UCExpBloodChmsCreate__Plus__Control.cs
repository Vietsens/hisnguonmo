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

namespace HIS.Desktop.Plugins.ExpBloodChmsCreate
{
    public partial class UCExpBloodChmsCreate
    {

        private void cboImpMediStock_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    txtExpMediStock.Focus();
                    txtExpMediStock.SelectAll();
                }
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
                WaitingManager.Show();
                V_HIS_MEDI_STOCK stock = null;
                if (cboImpMediStock.EditValue != null)
                {
                    stock = listImpMediStock.FirstOrDefault(o => o.ID == Convert.ToInt64(cboImpMediStock.EditValue));
                }
                if (stock != null)
                {
                    txtImpMediStock.Properties.Buttons[1].Visible = true;
                    txtImpMediStock.Text = stock.MEDI_STOCK_NAME;
                }
                else
                {
                    txtImpMediStock.Properties.Buttons[1].Visible = false;
                    txtImpMediStock.Text = "";
                }
                //FillDataToCboExpMestByImpMest(stock);
                FillDataToCboMediStock(true);
                ResetValueControlCommon();
                ResetGridControlDetail();
                FillDataToGridExpMest();
                if (cboExpMediStock.EditValue == null)
                {
                    LoadDataToGridBloodType(null);
                }
                WaitingManager.Hide();
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
                if (e.KeyCode == Keys.Enter)
                {
                    bool valid = false;
                    if (!String.IsNullOrEmpty(txtImpMediStock.Text))
                    {
                        string key = txtImpMediStock.Text.ToLower();
                        var listData = listImpMediStock.Where(o => o.MEDI_STOCK_CODE.ToLower().Contains(key) || o.MEDI_STOCK_NAME.ToLower().Contains(key)).ToList();
                        if (listData != null && listData.Count == 1)
                        {
                            valid = true;
                            cboImpMediStock.EditValue = listData.First().ID;
                        }
                    }
                    if (!valid)
                    {
                        cboImpMediStock.Focus();
                        cboImpMediStock.ShowPopup();
                    }
                }
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
                if (txtImpMediStock.EditValue != txtImpMediStock.OldEditValue && String.IsNullOrEmpty(txtImpMediStock.Text) && cboImpMediStock.EditValue != null)
                {
                    cboImpMediStock.EditValue = null;
                }
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
                WaitingManager.Show();
                V_HIS_MEDI_STOCK mestRoom = null;
                if (cboExpMediStock.EditValue != null)
                {
                    mestRoom = listExpMediStock.FirstOrDefault(o => o.ID == Convert.ToInt64(cboExpMediStock.EditValue));
                }
                if (mestRoom != null)
                {
                    txtExpMediStock.Properties.Buttons[1].Visible = true;
                    txtExpMediStock.Text = mestRoom.MEDI_STOCK_NAME;
                }
                else
                {
                    txtExpMediStock.Properties.Buttons[1].Visible = false;
                    txtExpMediStock.Text = "";
                }
                FillDataToCboMediStock(false);
                LoadDataToGridBloodType(mestRoom);
                ResetGridControlDetail();

                WaitingManager.Hide();
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
                if (e.KeyCode == Keys.Enter)
                {
                    bool valid = false;
                    if (!String.IsNullOrEmpty(txtExpMediStock.Text))
                    {
                        string key = txtExpMediStock.Text.ToLower();
                        var listData = listExpMediStock.Where(o => o.MEDI_STOCK_CODE.ToLower().Contains(key) || o.MEDI_STOCK_NAME.ToLower().Contains(key)).ToList();
                        if (listData != null && listData.Count == 1)
                        {
                            valid = true;
                            cboExpMediStock.EditValue = listData.First().ID;
                        }
                    }
                    if (!valid)
                    {
                        cboExpMediStock.Focus();
                        cboExpMediStock.ShowPopup();
                    }
                }
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
                if (txtExpMediStock.EditValue != txtExpMediStock.OldEditValue && String.IsNullOrEmpty(txtExpMediStock.Text) && cboExpMediStock.EditValue != null)
                {
                    cboExpMediStock.EditValue = null;
                }
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
                    txtSearch.Focus();
                    txtSearch.SelectAll();
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
                    cboChooseABO.Focus();
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

        private void cboChooseABO_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (cboChooseABO.EditValue != null)
                {
                    cboChooseRH.Focus();
                }
                else
                {
                    cboChooseABO.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void cboChooseRH_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (cboChooseRH.EditValue != null)
                {
                    txtNote.Focus();
                    txtNote.SelectAll();
                }
                else
                {
                    cboChooseRH.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }            

        }
    }
}

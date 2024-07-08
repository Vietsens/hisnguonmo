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
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using His.UC.UCHein;
using His.UC.UCHein.Base;
using His.UC.UCHein.ControlProcess;
using His.UC.UCHein.Data;
using His.UC.UCHein.Resources;
using Inventec.Common.Logging;
using Inventec.Core;
using MOS.LibraryHein.Bhyt;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace His.UC.UCHein.Design.TemplateKskContract1
{
    public partial class Template__KskContract1 : UserControl
    {
        void InitDataToControl()
        {
            try
            {
                KskContractProcess.LoadDataToComboKskContract(this.cboKskContract, KskContracts);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtMaKskContract_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string strValue = (sender as DevExpress.XtraEditors.TextEdit).Text;
                    LoadKskContractCombo(strValue, false);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboKskContract_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboKskContract.EditValue != null)
                    {
                        var data = KskContracts.SingleOrDefault(o => o.ID == (long)(cboKskContract.EditValue));
                        if (data != null)
                        {
                            txtKskContractCode.Text = data.KSK_CONTRACT_CODE;
                        }
                        FocusmoveOut();
                    }
                    else
                    {
                        FocusmoveOut();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboKskContract_Properties_GetNotInListValue(object sender, DevExpress.XtraEditors.Controls.GetNotInListValueEventArgs e)
        {
            try
            {
                var item = ((List<MOS.EFMODEL.DataModels.HIS_KSK_CONTRACT>)cboKskContract.Properties.DataSource)[e.RecordIndex];
                if (item != null)
                {
                    if (e.FieldName == "RENDERER_RATIO")
                    {
                        e.Value = item.RATIO * 100 + "%";
                    }
                    else if (e.FieldName == "RENDERER_MAX_FEE")
                    {
                        e.Value = Inventec.Common.Number.Convert.NumberToStringMoneyAfterRound((item.MAX_FEE ?? 0));
                    }
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void cboKskContract_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cboKskContract.EditValue != null)
                    {
                        var data = KskContracts.SingleOrDefault(o => o.ID == (long)(cboKskContract.EditValue));
                        if (data != null)
                        {
                            txtKskContractCode.Text = data.KSK_CONTRACT_CODE;
                        }
                        FocusmoveOut();
                    }
                }
                else if (e.KeyCode == Keys.Delete)
                {
                    cboKskContract.EditValue = null;
                    txtKskContractCode.Text = "";
                    txtKskContractCode.Focus();
                    txtKskContractCode.SelectAll();
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void LoadKskContractCombo(string searchCode, bool isExpand)
        {
            try
            {
                if (String.IsNullOrEmpty(searchCode))
                {
                    cboKskContract.EditValue = null;
                    cboKskContract.Focus();
                    cboKskContract.ShowPopup();
                }
                else
                {
                    var data = KskContracts.Where(o => o.KSK_CONTRACT_CODE.Contains(searchCode)).ToList();
                    if (data != null)
                    {
                        if (data.Count == 1)
                        {
                            cboKskContract.EditValue = data[0].ID;
                            txtKskContractCode.Text = data[0].KSK_CONTRACT_CODE;
                            FocusmoveOut();
                        }
                        else
                        {
                            cboKskContract.EditValue = null;
                            cboKskContract.Focus();
                            cboKskContract.ShowPopup();
                        }
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

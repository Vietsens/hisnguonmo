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
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Inventec.Core;
using Inventec.Common.Controls.EditorLoader;
using MOS.EFMODEL.DataModels;

namespace HIS.Desktop.Plugins.DepositRequest
{
    public partial class UCDepositRequest : UserControl
    {
        internal List<V_HIS_DEPOSIT_REQ> listhas;
        private void loadStatusCombo(string _status)
        {
            try
            {
                CommonParam param = new CommonParam();
                MOS.Filter.HisDepositReqViewFilter hasfilter = new MOS.Filter.HisDepositReqViewFilter();

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        //private void loadPayFormCombo(string _payFormCode)
        //{
        //    try
        //    {
        //        List<MOS.EFMODEL.DataModels.HIS_PAY_FORM> listResult = new List<MOS.EFMODEL.DataModels.HIS_PAY_FORM>();
        //        listResult = ListPayForm.Where(o => (o.PAY_FORM_CODE != null && o.PAY_FORM_CODE.StartsWith(_payFormCode))).ToList();

        //        List<ColumnInfo> columnInfos = new List<ColumnInfo>();
        //        columnInfos.Add(new ColumnInfo("PAY_FORM_CODE", "", 100, 1));
        //        columnInfos.Add(new ColumnInfo("PAY_FORM_NAME", "", 250, 2));
        //        ControlEditorADO controlEditorADO = new ControlEditorADO("PAY_FORM_NAME", "ID", columnInfos, false, 350);
        //        ControlEditorLoader.Load(cboPayForm, listResult, controlEditorADO);
        //        if (listResult.Count == 1)
        //        {
        //            cboPayForm.EditValue = listResult[0].ID;
        //            txtPayFormCode.Text = listResult[0].PAY_FORM_CODE;
        //            txtDescription.Focus();
        //            txtDescription.SelectAll();
        //        }
        //        else if (listResult.Count > 1)
        //        {
        //            cboPayForm.EditValue = null;
        //            cboPayForm.Focus();
        //            cboPayForm.ShowPopup();
        //        }
        //        else
        //        {
        //            cboPayForm.EditValue = null;
        //            cboPayForm.Focus();
        //            cboPayForm.ShowPopup();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Warn(ex);
        //    }
        //}
    }
}

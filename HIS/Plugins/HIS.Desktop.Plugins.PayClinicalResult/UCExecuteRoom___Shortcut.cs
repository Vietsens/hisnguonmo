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
using System.Collections;
using Inventec.Desktop.Common.Message;
using Inventec.Core;
using Inventec.Common.Logging;
using HIS.Desktop.Controls.Session;
using MOS.Filter;
using Inventec.Common.Adapter;
using HIS.Desktop.ApiConsumer;
using HIS.UC.TreeSereServ7;
using MOS.SDO;
using MOS.EFMODEL.DataModels;
using HIS.Desktop.LocalStorage.LocalData;

namespace HIS.Desktop.Plugins.PayClinicalResult
{
    public partial class UCExecuteRoom : HIS.Desktop.Utility.UserControlBase
    {
        public void FindShortcut()
        {
            try
            {
                btnFind_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void ExecuteShortcut()
        {
            try
            {
                if (btnExecute.Enabled == true)
                    btnExecute_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void CallShorcut()
        {
            try
            {
                if (btnCallPatient.Enabled == true)
                    btnCallPatient_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void ReCallShorcut()
        {
            try
            {
                if (btnRecallPatient.Enabled == true)
                    btnRecallPatient_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void AssignServiceShortcut()
        {
            try
            {
                //btnAssignService_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void AssignPrescriptionShortcut()
        {
            try
            {
                //btnAssignPrescription_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void BordereauShortcut()
        {
            try
            {
                if (btnBordereau.Enabled == true)
                    btnBordereau_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void FindFocus()
        {
            try
            {
                txtSearchKey.Focus();
                txtSearchKey.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        
    }
}
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
using ACS.SDO;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.AssignPrescriptionCLS.Config;
using HIS.Desktop.Plugins.AssignPrescriptionCLS.Resources;
using HIS.UC.DateEditor;
using HIS.UC.PatientSelect;
using HIS.UC.PeriousExpMestList;
using HIS.UC.SecondaryIcd;
using HIS.UC.SecondaryIcd.ADO;
using HIS.UC.TreatmentFinish;
using Inventec.Common.Adapter;
using Inventec.Common.Logging;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.AssignPrescriptionCLS.AssignPrescription
{
    public partial class frmAssignPrescription : HIS.Desktop.Utility.FormBase
    {
        private void NextForcusSubIcd()
        {
            try
            {
                cboMediStockExport.Focus();
                cboMediStockExport.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void NextForcusSubIcdCause()
        {
            try
            {
                txtMediMatyForPrescription.Focus();
                txtMediMatyForPrescription.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        internal void UcSecondaryIcdReadOnly(bool isReadOnly)
        {
            try
            {
                if (isReadOnly)
                {
                    txtIcdSubCode.ReadOnly = true;
                    txtIcdText.ReadOnly = true;
                }
                else
                {
                    txtIcdSubCode.ReadOnly = false;
                    txtIcdText.ReadOnly = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadIcdToControlIcdSub(string icdSubCode, string icdText)
        {
            try
            {
                this.txtIcdSubCode.Text = icdSubCode;
                this.txtIcdText.Text = icdText;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }


        private void UcSecondaryIcdFocusComtrol()
        {
            try
            {
                txtIcdSubCode.Focus();
                txtIcdSubCode.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal object UcSecondaryIcdGetValue()
        {
            object result = null;
            try
            {
                SecondaryIcdDataADO outPut = new SecondaryIcdDataADO();

                if (!String.IsNullOrEmpty(txtIcdSubCode.Text))
                {
                    outPut.ICD_SUB_CODE = txtIcdSubCode.Text;
                }
                if (!String.IsNullOrEmpty(txtIcdText.Text))
                {
                    outPut.ICD_TEXT = txtIcdText.Text;
                }
                result = outPut;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private void LoadDataToIcdSub(string icdSubCode, string icdText)
        {
            try
            {
                this.txtIcdSubCode.Text = icdSubCode;
                this.txtIcdText.Text = icdText;                    
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}

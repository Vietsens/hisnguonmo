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
using HIS.Desktop.Plugins.AssignPrescriptionPK.Config;
using HIS.Desktop.Plugins.AssignPrescriptionPK.Resources;
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

namespace HIS.Desktop.Plugins.AssignPrescriptionPK.AssignPrescription
{
    public partial class frmAssignPrescription : HIS.Desktop.Utility.FormBase
    {
        private void NextForcusSubIcd()
        {
            try
            {
                txtIcdCodeCause.Focus();
                txtIcdCodeCause.SelectAll();
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
                List<string> IcdsubCodes = new List<string>();
                List<string> IcdsubText = new List<string>();
                if (!string.IsNullOrEmpty(icdText))
                {
                    IcdsubText = icdText.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries).ToList();
                }
                if (!string.IsNullOrEmpty(icdSubCode))
                {
                    var splt = icdSubCode.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries).ToList();
                    foreach (var item in splt)
                    {
                        if (!string.IsNullOrEmpty(item))
                        {
                            var i = currentIcds.FirstOrDefault(o => o.ICD_CODE == item.Trim());
                            if (i != null)
                            {
                                IcdsubCodes.Add(i.ICD_CODE);
                            }
                            else
                            {
                                if (IcdsubText != null && IcdsubText.Count > 0)
                                {
                                    var icdTextRemove = BackendDataWorker.Get<HIS_ICD>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList().FirstOrDefault(o => o.ICD_CODE == item.Trim());
                                    if (icdTextRemove != null)
                                        IcdsubText = IcdsubText.Where(o => !o.Equals(icdTextRemove.ICD_NAME)).ToList();
                                }
                            }
                        }
                    }
                }

                this.txtIcdSubCode.Text = string.Join(";", IcdsubCodes);
                this.txtIcdText.Text = string.Join(";", IcdsubText);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}

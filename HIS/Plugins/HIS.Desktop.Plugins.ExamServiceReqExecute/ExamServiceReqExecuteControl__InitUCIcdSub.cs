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
using HIS.Desktop.Utility;
using HIS.UC.SecondaryIcd.ADO;
using System;
using System.Linq;

namespace HIS.Desktop.Plugins.ExamServiceReqExecute
{
    public partial class ExamServiceReqExecuteControl : UserControlBase
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
                txtIcdSubCode.Focus();
                //txtIcdText.SelectAll();
                txtIcdSubCode.SelectionStart = txtIcdText.Text.Length;
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

        private void LoadIcdToControlIcdYHCT(string icdCodeYHCT, string icdNameYHCT, string icdSubCodeYHCT, string icdSubNameYHCT)
        {
            try
            {
                HIS.UC.Icd.ADO.IcdInputADO Icd = new HIS.UC.Icd.ADO.IcdInputADO();
                Icd.ICD_CODE = icdCodeYHCT;
                Icd.ICD_NAME = icdNameYHCT;

                if (ucIcdYHCT != null)
                {
                    icdProcessorYHCT.Reload(ucIcdYHCT, Icd);
                }

                HIS.UC.SecondaryIcd.ADO.SecondaryIcdDataADO subIcd = new HIS.UC.SecondaryIcd.ADO.SecondaryIcdDataADO();
                subIcd.ICD_SUB_CODE = icdSubCodeYHCT;
                subIcd.ICD_TEXT = icdSubNameYHCT; 

                if (ucSecondaryIcdYHCT != null)
                {
                    subIcdProcessorYHCT.Reload(ucSecondaryIcdYHCT, subIcd);
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

                if (!String.IsNullOrEmpty(txtIcdSubCode.Text.Trim()))
                {
                    outPut.ICD_SUB_CODE = txtIcdSubCode.Text.Trim();
                }
                if (!String.IsNullOrEmpty(txtIcdText.Text.Trim()))
                {
                    outPut.ICD_TEXT = txtIcdText.Text.Trim();
                }
                result = outPut;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private void LoadDataToIcdSub()
        {
            try
            {
                this.isNotProcessWhileChangedTextSubIcd = true;
                this.txtIcdSubCode.Text = (!String.IsNullOrEmpty(this.HisServiceReqView.ICD_CODE) || !String.IsNullOrEmpty(this.HisServiceReqView.ICD_SUB_CODE))
                    ? this.HisServiceReqView.ICD_SUB_CODE : treatment != null && !String.IsNullOrEmpty(treatment.ICD_SUB_CODE)
                    ? treatment.ICD_SUB_CODE : null;
                this.txtIcdText.Text = (!String.IsNullOrEmpty(this.HisServiceReqView.ICD_CODE) || !String.IsNullOrEmpty(this.HisServiceReqView.ICD_SUB_CODE))
                    ? this.HisServiceReqView.ICD_TEXT : treatment != null && !String.IsNullOrEmpty(treatment.ICD_TEXT)
                    ? treatment.ICD_TEXT : null;


                string[] codes = this.txtIcdSubCode.Text.Split(IcdUtil.seperator.ToCharArray());
                this.icdSubcodeAdoChecks = (from m in this.currentIcds select new ADO.IcdADO(m, codes)).ToList();

                customGridViewSubIcdName.BeginUpdate();
                customGridViewSubIcdName.GridControl.DataSource = this.icdSubcodeAdoChecks;
                customGridViewSubIcdName.EndUpdate();
                this.isNotProcessWhileChangedTextSubIcd = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

    }
}

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
using Inventec.Common.Controls.EditorLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.EventLogControl.Design.Template2
{
    internal partial class Template2
    {

        public void MeShow()
        {
            try
            {
                SetDefaultValueControl();
                FillDataToControl();
                LoadDefaultData();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }


        private void SetDefaultValueControl()
        {
            try
            {
                dtTime.EditValue = DateTime.Now;
                dtTime.Properties.VistaCalendarViewStyle = DevExpress.XtraEditors.VistaCalendarViewStyle.MonthView;
                dtTime.Properties.EditMask = "dd/MM/yyyy";
                dtTime.Properties.DisplayFormat.FormatString = "dd/MM/yyyy";
                if (!String.IsNullOrWhiteSpace(this.currentData.expMestCode))
                {
                    // btnCodeFind.Text = typeCodeFind__MaPX;
                    txtKeyWord.Text = "EXP_MEST_CODE:" + " " + this.currentData.expMestCode;
                }
                else if (!String.IsNullOrWhiteSpace(this.currentData.impMestCode))
                {
                    //  this.typeCodeFind = typeCodeFind__MaPN;
                    txtKeyWord.Text = "IMP_MEST_CODE:" + " " + this.currentData.impMestCode;
                }
                else if (!String.IsNullOrWhiteSpace(this.currentData.patientCode))
                {
                    // this.typeCodeFind = typeCodeFind__MaBN;
                    txtKeyWord.Text = "PATIENT_CODE:" + " " + this.currentData.patientCode;
                }
                else if (!String.IsNullOrWhiteSpace(this.currentData.treatmentCode))
                {
                    // this.typeCodeFind = typeCodeFind__MaDT;
                    txtKeyWord.Text = "TREATMENT_CODE:" + " " + this.currentData.treatmentCode;
                }
                else if (!String.IsNullOrWhiteSpace(this.currentData.serviceRequestCode))
                {
                    //this.typeCodeFind = typeCodeFind__MaYL;
                    txtKeyWord.Text = "SERVICE_REQ_CODE:" + " " + this.currentData.serviceRequestCode;
                } 
                else if (!String.IsNullOrWhiteSpace(this.currentData.bidNumber))
                {
                     //btnCodeFind.Text = typeCodeFind__SoQDT;
                    txtKeyWord.Text = "BID_NUMBER:" + " " + this.currentData.bidNumber;
                }
                else
                {
                    txtKeyWord.Text = "";
                }
                // txtKeyWord.Text = "";
                txtKeyWord.Focus();
                txtKeyWord.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

    }
}

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
using HIS.UC.TreatmentInfo.ADO;

namespace HIS.UC.TreatmentInfo
{
    internal partial class UCTreatmentInfo : UserControl
    {
        InitADO ado = null;

        public UCTreatmentInfo(InitADO data)
        {
            InitializeComponent();
            try
            {
                this.ado = data;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void UCTreatmentInfo_Load(object sender, EventArgs e)
        {
            try
            {
                if (this.ado != null)
                {
                    //this.layoutDob.Text = this.ado.LayoutDob;
                    //this.layoutGenderName.Text = this.ado.LayoutGenderName;
                    this.layoutHeinCardFromTime.Text = this.ado.LayoutHeinCardFromTime;
                    this.layoutHeinCardNumber.Text = this.ado.LayoutHeinCardNumber;
                    this.layoutHeinCardToTime.Text = this.ado.LayoutHeinCardToTime;
                    //this.layoutPatientCode.Text = this.ado.LayoutPatientCode;
                    this.layoutPatientTypeName.Text = this.ado.LayoutPatientTypeName;
                    //this.layoutTreatmentCode.Text = this.ado.LayoutTreatmentCode;
                    //this.layoutVirPatientName.Text = this.ado.LayoutVirPatientName;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        internal void SetValueToControl(TreatmentInfoADO info)
        {
            try
            {
                if (info != null)
                {
                    //this.lblDob.Text = info.Dob;
                    //this.lblGenderName.Text = info.GenderName;
                    this.lblHeinCardFromTime.Text = info.HeinCardFromTime;
                    this.lblHeinCardNumber.Text = info.HeinCardNumber;
                    this.lblHeinCardToTime.Text = info.HeinCardToTime;
                    //this.lblPatientCode.Text = info.PatientCode;
                    this.lblPatientTypeName.Text = info.PatientTypeName;
                    //this.lblTreatmentCode.Text = info.TreatmentCode;
                    //this.lblVirPatientName.Text = info.VirPatientName;
                }
                else
                {
                    //this.lblDob.Text = "";
                    //this.lblGenderName.Text = "";
                    this.lblHeinCardFromTime.Text = "";
                    this.lblHeinCardNumber.Text = "";
                    this.lblHeinCardToTime.Text = "";
                    //this.lblPatientCode.Text = "";
                    this.lblPatientTypeName.Text = "";
                    //this.lblTreatmentCode.Text = "";
                    //this.lblVirPatientName.Text = "";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}

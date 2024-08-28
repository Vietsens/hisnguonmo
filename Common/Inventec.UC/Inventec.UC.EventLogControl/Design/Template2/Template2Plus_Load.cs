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

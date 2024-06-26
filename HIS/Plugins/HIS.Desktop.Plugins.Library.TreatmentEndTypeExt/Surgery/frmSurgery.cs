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
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Common;
using HIS.Desktop.Plugins.Library.TreatmentEndTypeExt.Data;
using HIS.Desktop.Utility;
using HIS.UC.SurgeryAppointment;
using HIS.UC.SurgeryAppointment.ADO;
using Inventec.Common.Adapter;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.Library.TreatmentEndTypeExt.Surgery
{
    public partial class frmSurgery : FormBase
    {
        long treatmentId;
        HIS_TREATMENT treatment;
        DelegateSelectData ReloadDataTreatmentEndTypeExt;
        TreatmentEndTypeExtData TreatmentEndTypeExtData;
        FormEnum.TYPE type;

        SurgeryAppointmentProcessor surgeryAppointmentProcessor;
        UserControl ucSurgeryAppointment;
        Inventec.Desktop.Common.Modules.Module moduleData;
        bool IsShowCheckPrint;

        public frmSurgery(Inventec.Desktop.Common.Modules.Module module, long _treatmentId, FormEnum.TYPE _type, TreatmentEndTypeExtData _surgeryLeaveData, DelegateSelectData _reloadDataTreatmentEndTypeExt, bool isShowCheckPrint)
            : base(module)
        {
            InitializeComponent();
            try
            {
                this.treatmentId = _treatmentId;
                this.ReloadDataTreatmentEndTypeExt = _reloadDataTreatmentEndTypeExt;
                this.TreatmentEndTypeExtData = _surgeryLeaveData;
                this.type = _type;
                this.moduleData = module;
                this.IsShowCheckPrint = isShowCheckPrint;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void frmSurgery_Load(object sender, EventArgs e)
        {
            try
            {
                LoadDefault();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDefault()
        {
            try
            {
                if (this.TreatmentEndTypeExtData != null)
                {
                    this.treatment = new HIS_TREATMENT();
                    this.treatment.SURGERY_APPOINTMENT_TIME = this.TreatmentEndTypeExtData.SurgeryAppointmentTime;
                    this.treatment.ADVISE = this.TreatmentEndTypeExtData.Advise;
                    this.treatment.APPOINTMENT_SURGERY = this.TreatmentEndTypeExtData.AppointmentSurgery;
                }
                else
                {
                    CommonParam param = new CommonParam();
                    HisTreatmentFilter filter = new HisTreatmentFilter();
                    filter.ID = this.treatmentId;
                    var treatments = new BackendAdapter(param)
                        .Get<List<MOS.EFMODEL.DataModels.HIS_TREATMENT>>("api/HisTreatment/Get", ApiConsumers.MosConsumer, filter, param);
                    this.treatment = treatments != null ? treatments.FirstOrDefault() : null;
                }

                SurgeryAppointmentInitADO surgeryInitADO = new SurgeryAppointmentInitADO();
                surgeryInitADO.CurrentHisTreatment = this.treatment;
                surgeryInitADO.InvisibleCheckPrint = this.IsShowCheckPrint;

                surgeryAppointmentProcessor = new SurgeryAppointmentProcessor();
                this.ucSurgeryAppointment = (UserControl)surgeryAppointmentProcessor.Run(surgeryInitADO);
                this.ucSurgeryAppointment.Dock = DockStyle.Fill;
                panelControlSurgery.Controls.Clear();
                panelControlSurgery.Controls.Add(this.ucSurgeryAppointment);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                TreatmentEndTypeExtData surgeryOut = new TreatmentEndTypeExtData();
                HisTreatmentFinishSDO surgery = this.surgeryAppointmentProcessor.GetValue(this.ucSurgeryAppointment) as HisTreatmentFinishSDO;

                if (surgery != null)
                {
                    surgeryOut.SurgeryAppointmentTime = surgery.SurgeryAppointmentTime;
                    surgeryOut.AppointmentSurgery = surgery.AppointmentSurgery;
                    surgeryOut.Advise = surgery.Advise;
                }

                surgeryOut.TreatmentEndTypeExtId = IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE_EXT.ID__HEN_MO;

                if (this.ReloadDataTreatmentEndTypeExt != null)
                    this.ReloadDataTreatmentEndTypeExt(surgeryOut);

                this.Close();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barButtonItem__Save_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnSave_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}

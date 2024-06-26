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
using MOS.EFMODEL.DataModels;
using Inventec.Common.Controls.EditorLoader;
using HIS.UC.ExamTreatmentFinish.Config;
using Inventec.Common.WebApiClient;
using HIS.Desktop.ApiConsumer;
using HIS.UC.ExamTreatmentFinish.ADO;

namespace HIS.UC.ExamTreatmentFinish.Run
{
    public partial class UCExamTreatmentFinish : UserControl
    {
        HIS_TREATMENT treatment;


        Inventec.Desktop.Common.Modules.Module currentModule;
        public void SetValue(Inventec.Desktop.Common.Modules.Module _currentModule, HIS_TREATMENT _treatment)
        {
            try
            {
                this.treatment = _treatment;
                this.currentModule = _currentModule;
                if (treatment != null)
                {
                    lblEndCode.Text = treatment.END_CODE;
                    lblOutCode.Text = treatment.OUT_CODE;
                    if (treatment.TREATMENT_END_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__HEN)
                    {
                        lciChiDinhDichVuhenKham.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                        //lciChiDinhDichVuhenKham.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                        //if (this._ucAdvise != null)
                        //{
                        //    AdviseADO ado = new AdviseADO()
                        //    {
                        //        Advise = treatment.ADVISE,
                        //        AppointmentExamRoomIds = treatment.APPOINTMENT_EXAM_ROOM_IDS
                        //    };
                        //    ado.currentRoomId = _currentModule.RoomId;
                        //    this._ucAdvise.SetValue(ado);
                        //}
                    }
                    else
                    {
                        lciChiDinhDichVuhenKham.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    }

                    if (treatment.TREATMENT_END_TYPE_EXT_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE_EXT.ID__NGHI_OM || treatment.TREATMENT_END_TYPE_EXT_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE_EXT.ID__NGHI_DUONG_THAI)
                    {
                        lciChiDinhDichVuhenKham.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                        if (this.ucSick != null)
                        {
                            this.sickProcessor.Reload(this.ucSick, treatment);
                        }
                    }
                    else if (treatment.TREATMENT_END_TYPE_EXT_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE_EXT.ID__HEN_MO)
                    {
                        //TODO
                    }
                }
                else
                {
                    lblEndCode.Text = "";
                    lblOutCode.Text = "";
                    lciChiDinhDichVuhenKham.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}

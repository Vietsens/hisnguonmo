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
using HIS.Desktop.LocalStorage.LocalData;
using MOS.Filter;
using Inventec.Desktop.Common.Message;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using Inventec.Common.Adapter;
using Inventec.Core;
using System.Collections;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.Data;
using HIS.Desktop.Controls.Session;
using Inventec.Desktop.Common.LanguageManager;
using HIS.UC.TreatmentFinish;
using HIS.UC.TreatmentFinish.ADO;
using HIS.UC.TreatmentFinish.Run;
using HIS.UC.TreatmentFinish.Reload;
using MOS.SDO;
using DevExpress.XtraEditors.Controls;
using HIS.Desktop.ADO;
using HIS.UC.TreatmentFinish.CloseTreatment;
using Inventec.Common.Controls.EditorLoader;
namespace HIS.UC.TreatmentFinish.Run
{
    public partial class UCTreatmentFinish : UserControl
    {
        private void InitTreatmentEndTypeExt()
        {
            try
            {
                TreatmentEndTypeExts = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get
                      <MOS.EFMODEL.DataModels.HIS_TREATMENT_END_TYPE_EXT>();


                InitComboCommon(cboTreatmentEndTypeExt, TreatmentEndTypeExts, "ID", "TREATMENT_END_TYPE_EXT_NAME", "TREATMENT_END_TYPE_EXT_CODE");

                //CommonParam param = new CommonParam();
                //HisTreatmentEndTypeExtFilter filter = new HisTreatmentEndTypeExtFilter();
                //TreatmentEndTypeExts = new BackendAdapter(param)
                //    .Get<List<MOS.EFMODEL.DataModels.HIS_TREATMENT_END_TYPE_EXT>>("api/HisTreatmentEndTypeExt/Get", ApiConsumers.MosConsumer, filter, param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public bool IsAutoCheckBKTheoDoiTuong()
        {
            bool result = false;
            try
            {

                string autoCheckBKTheoDoiTuong = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(ConfigKeyCFG.AUTO_CHECK_PRINT_BORDEREAU_BY_PATIENT_TYPE);
                if (!String.IsNullOrEmpty(autoCheckBKTheoDoiTuong) && this.treatmentId > 0)
                {
                    CommonParam param = new CommonParam();
                    HisPatientTypeAlterViewAppliedFilter filter = new HisPatientTypeAlterViewAppliedFilter();
                    filter.TreatmentId = this.treatmentId ?? 0;
                    filter.InstructionTime = Inventec.Common.DateTime.Get.Now() ?? 0;
                    V_HIS_PATIENT_TYPE_ALTER PatientTypeAlter = new BackendAdapter(param).Get<MOS.EFMODEL.DataModels.V_HIS_PATIENT_TYPE_ALTER>("api/HisPatientTypeAlter/GetApplied", ApiConsumers.MosConsumer, filter, param);

                    string[] autoCheckBKTheoDoiTuongArr = autoCheckBKTheoDoiTuong.Split(',');
                    if (autoCheckBKTheoDoiTuongArr != null
                        && autoCheckBKTheoDoiTuongArr.Length > 0
                        && PatientTypeAlter != null)
                    {


                        foreach (var item in autoCheckBKTheoDoiTuongArr)
                        {
                            if (item.ToLower().Trim() == PatientTypeAlter.PATIENT_TYPE_CODE.ToLower())
                            {
                                result = true;
                                break;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private void LoadDefautcboCareer(List<HIS_CAREER> HisCareers) 
        {
            try
            {
                if (this.Treatment == null || String.IsNullOrEmpty(this.Treatment.TDL_PATIENT_CAREER_CODE))
                {
                     HisTreatmentFilter filter = new HisTreatmentFilter();
                        filter.ID = this.treatmentId;
                        HisTreatment = new BackendAdapter(new CommonParam()).Get<List<HIS_TREATMENT>>("api/HisTreatment/Get", ApiConsumers.MosConsumer, filter, new CommonParam()).FirstOrDefault();
                }

                if ((this.Treatment != null && !String.IsNullOrEmpty(this.Treatment.TDL_PATIENT_CAREER_CODE)) || (HisTreatment != null && !String.IsNullOrEmpty(HisTreatment.TDL_PATIENT_CAREER_CODE)))
                {
                    List<HIS_CAREER> Career = new List<HIS_CAREER>();

                    if (this.Treatment != null && !String.IsNullOrEmpty(this.Treatment.TDL_PATIENT_CAREER_CODE))
                    {
                        Career = HisCareers.Where(o => o.CAREER_CODE.ToUpper() == this.Treatment.TDL_PATIENT_CAREER_CODE.ToUpper()).ToList();
                    }
                    else if (HisTreatment != null && !String.IsNullOrEmpty(HisTreatment.TDL_PATIENT_CAREER_CODE))
                    {
                        Career = HisCareers.Where(o => o.CAREER_CODE.ToUpper() == HisTreatment.TDL_PATIENT_CAREER_CODE.ToUpper()).ToList();
                    }
                    if (Career != null && Career.Count() == 1)
                    {
                        cboCareer.EditValue = Career.FirstOrDefault().ID;
                    }
                }
                else
                {
                    cboCareer.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}

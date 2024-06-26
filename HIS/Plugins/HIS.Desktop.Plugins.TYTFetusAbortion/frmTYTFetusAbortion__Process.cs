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
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.ViewInfo;
using HIS.Desktop.Utility;
using MOS.EFMODEL.DataModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TYT.EFMODEL.DataModels;

namespace TYT.Desktop.Plugins.FetusAbortion
{
    public partial class frmTYTFetusAbortion : FormBase
    {
        public void MakeDataFetusAbortion(ref TYT_FETUS_ABORTION fetusAbortionTemp)
        {
            try
            {
                if (dtLastMensesTime.EditValue != null)
                {
                    fetusAbortionTemp.LAST_MENSES_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtLastMensesTime.DateTime);
                }
                else
                {
                    fetusAbortionTemp.LAST_MENSES_TIME = null;
                }
                fetusAbortionTemp.IS_SINGLE = (short)(chkIsSingle.Checked ? 1 : 0);

                if (spinParaChildCount.EditValue != null)
                {
                    fetusAbortionTemp.PARA_CHILD_COUNT = (long)spinParaChildCount.Value;
                }
                else
                {
                    fetusAbortionTemp.PARA_CHILD_COUNT = null;
                }

                if (cboDiagnoseTest.EditValue != null)
                {
                    fetusAbortionTemp.DIAGNOSE_TEST = cboDiagnoseTest.SelectedIndex + 1;
                }
                else
                {
                    fetusAbortionTemp.DIAGNOSE_TEST = null;
                }

                if (cboSMResult.EditValue != null)
                {
                    fetusAbortionTemp.SM_RESULT = cboSMResult.SelectedIndex + 1;
                }
                else
                {
                    fetusAbortionTemp.SM_RESULT = null;
                }

                fetusAbortionTemp.OBSTETRIC_COMPLICATION = mmObstetricComplication.Text;
                fetusAbortionTemp.ABORTION_METHOD = mmAbortionMethod.Text;
                fetusAbortionTemp.EXECUTE_NAME = txtExecuteName.Text;
                fetusAbortionTemp.IS_DEATH = (short)(chkIsDeath.Checked ? 1 : 0);
                fetusAbortionTemp.EXAM_AFTER_TWO_WEEK = (short)(chkExamAfterTwoWeek.Checked ? 1 : 0);
                fetusAbortionTemp.NOTE = mmNote.Text;
                if (dtAbortionTime.EditValue != null)
                {
                    fetusAbortionTemp.ABORTION_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtAbortionTime.DateTime);
                }
                else
                {
                    fetusAbortionTemp.ABORTION_TIME = null;
                }

                if (actionType == TYPE.UPDATE && this.fetusAbortion != null)
                {
                    fetusAbortionTemp.ID = this.fetusAbortion.ID;
                    fetusAbortionTemp.BRANCH_CODE = this.fetusAbortion.BRANCH_CODE;
                    fetusAbortionTemp.PATIENT_CODE = this.fetusAbortion.PATIENT_CODE;
                    fetusAbortionTemp.PERSON_CODE = this.fetusAbortion.PERSON_CODE;
                    fetusAbortionTemp.FIRST_NAME = this.fetusAbortion.FIRST_NAME;
                    fetusAbortionTemp.LAST_NAME = this.fetusAbortion.LAST_NAME;
                    fetusAbortionTemp.VIR_PERSON_NAME = this.fetusAbortion.VIR_PERSON_NAME;
                    fetusAbortionTemp.DOB = this.fetusAbortion.DOB;
                    fetusAbortionTemp.IS_HAS_NOT_DAY_DOB = this.fetusAbortion.IS_HAS_NOT_DAY_DOB;
                    fetusAbortionTemp.GENDER_NAME = this.fetusAbortion.GENDER_NAME;
                    fetusAbortionTemp.PERSON_ADDRESS = this.fetusAbortion.PERSON_ADDRESS;
                    fetusAbortionTemp.CAREER_NAME = this.fetusAbortion.CAREER_NAME;
                    fetusAbortionTemp.ETHNIC_NAME = this.fetusAbortion.ETHNIC_NAME;
                    fetusAbortionTemp.BHYT_NUMBER = this.fetusAbortion.BHYT_NUMBER;
                }
                else
                {
                    HIS_BRANCH branch = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_BRANCH>().FirstOrDefault(o => o.ID == HIS.Desktop.LocalStorage.LocalData.WorkPlace.GetBranchId());
                    if (branch == null)
                        throw new Exception("Khong tim thay chi nhanh hien tai!");
                    fetusAbortionTemp.BRANCH_CODE = branch.BRANCH_CODE;
                    fetusAbortionTemp.PATIENT_CODE = patient.PATIENT_CODE;
                    fetusAbortionTemp.PERSON_CODE = patient.PERSON_CODE;
                    fetusAbortionTemp.FIRST_NAME = patient.FIRST_NAME;
                    fetusAbortionTemp.LAST_NAME = patient.LAST_NAME;
                    fetusAbortionTemp.VIR_PERSON_NAME = patient.VIR_PATIENT_NAME;
                    fetusAbortionTemp.DOB = patient.DOB;
                    fetusAbortionTemp.IS_HAS_NOT_DAY_DOB = patient.IS_HAS_NOT_DAY_DOB;
                    fetusAbortionTemp.GENDER_NAME = patient.GENDER_NAME;
                    fetusAbortionTemp.PERSON_ADDRESS = patient.VIR_ADDRESS;
                    fetusAbortionTemp.CAREER_NAME = patient.CAREER_NAME;
                    fetusAbortionTemp.ETHNIC_NAME = patient.ETHNIC_NAME;
                    fetusAbortionTemp.BHYT_NUMBER = patient.TDL_HEIN_CARD_NUMBER;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}

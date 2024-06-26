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

namespace TYT.Desktop.Plugins.Nerves
{
    public partial class frmTYTNerves : FormBase
    {
        public void MakeDataNerves(ref TYT_NERVES nervesTemp)
        {
            try
            {
                nervesTemp.DIAGNOSE_TTPL = mmTamThanPhanLiet.Text;
                nervesTemp.DIAGNOSE_DK = mmDongKinh.Text;
                nervesTemp.DIAGNOSE_TC = mmTramCam.Text;
                if (cboPHCN.EditValue != null)
                    nervesTemp.PHCN_RESULT = cboPHCN.SelectedIndex + 1;
                else
                    nervesTemp.PHCN_RESULT = null;
                nervesTemp.IS_HOME_CHECK = (short)(chkHomeCheck.Checked ? 1 : 0);
                nervesTemp.MONTHS = this.GetMedicineMonitor();

                if (actionType == TYPE.UPDATE && this.currentData != null)
                {
                    if (nervesId > 0)
                    {
                        nervesTemp.ID = nervesId;
                    }
                    else
                    {
                        nervesTemp.ID = this.currentData.ID;
                    }

                    nervesTemp.BRANCH_CODE = this.currentData.BRANCH_CODE;
                    nervesTemp.PATIENT_CODE = this.currentData.PATIENT_CODE;
                    nervesTemp.PERSON_CODE = this.currentData.PERSON_CODE;
                    nervesTemp.FIRST_NAME = this.currentData.FIRST_NAME;
                    nervesTemp.LAST_NAME = this.currentData.LAST_NAME;
                    nervesTemp.VIR_PERSON_NAME = this.currentData.VIR_PERSON_NAME;
                    nervesTemp.DOB = this.currentData.DOB;
                    nervesTemp.IS_HAS_NOT_DAY_DOB = this.currentData.IS_HAS_NOT_DAY_DOB;
                    nervesTemp.GENDER_NAME = this.currentData.GENDER_NAME;
                    nervesTemp.PERSON_ADDRESS = this.currentData.PERSON_ADDRESS;
                    nervesTemp.CAREER_NAME = this.currentData.CAREER_NAME;
                    nervesTemp.ETHNIC_NAME = this.currentData.ETHNIC_NAME;
                }
                else
                {
                    HIS_BRANCH branch = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_BRANCH>().FirstOrDefault(o => o.ID == HIS.Desktop.LocalStorage.LocalData.WorkPlace.GetBranchId());
                    if (branch == null)
                        throw new Exception("Khong tim thay chi nhanh hien tai!");
                    nervesTemp.BRANCH_CODE = branch.BRANCH_CODE;
                    nervesTemp.PATIENT_CODE = patient.PATIENT_CODE;
                    nervesTemp.PERSON_CODE = patient.PERSON_CODE;
                    nervesTemp.FIRST_NAME = patient.FIRST_NAME;
                    nervesTemp.LAST_NAME = patient.LAST_NAME;
                    nervesTemp.VIR_PERSON_NAME = patient.VIR_PATIENT_NAME;
                    nervesTemp.DOB = patient.DOB;
                    nervesTemp.IS_HAS_NOT_DAY_DOB = patient.IS_HAS_NOT_DAY_DOB;
                    nervesTemp.GENDER_NAME = patient.GENDER_NAME;
                    nervesTemp.PERSON_ADDRESS = patient.VIR_ADDRESS;
                    nervesTemp.CAREER_NAME = patient.CAREER_NAME;
                    nervesTemp.ETHNIC_NAME = patient.ETHNIC_NAME;
                    nervesTemp.YEAR = Int16.Parse(txtYear.Value.ToString());
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private string GetMedicineMonitor()
        {
            string result = "";
            try
            {
                List<string> listMonthWithYear = new List<string>();
                if (chk1.Checked)
                {
                    listMonthWithYear.Add("01");
                }
                if (chk2.Checked)
                {
                    listMonthWithYear.Add("02");
                }
                if (chk3.Checked)
                {
                    listMonthWithYear.Add("03");
                }
                if (chk4.Checked)
                {
                    listMonthWithYear.Add("04");
                }
                if (chk5.Checked)
                {
                    listMonthWithYear.Add("05");
                }
                if (chk6.Checked)
                {
                    listMonthWithYear.Add("06");
                }
                if (chk7.Checked)
                {
                    listMonthWithYear.Add("07");
                }
                if (chk8.Checked)
                {
                    listMonthWithYear.Add("08");
                }
                if (chk9.Checked)
                {
                    listMonthWithYear.Add("09");
                }
                if (chk10.Checked)
                {
                    listMonthWithYear.Add("10");
                }
                if (chk11.Checked)
                {
                    listMonthWithYear.Add("11");
                }
                if (chk12.Checked)
                {
                    listMonthWithYear.Add("12");
                }

                if (listMonthWithYear != null && listMonthWithYear.Count > 0)
                {
                    result = JsonConvert.SerializeObject(listMonthWithYear);
                }
            }
            catch (Exception ex)
            {
                result = "";
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private string GetMonthById(long monthId)
        {
            string result = "";
            try
            {
                switch (monthId)
                {
                    case 1:
                        result = "01";
                        break;
                    case 2:
                        result = "02";
                        break;
                    case 3:
                        result = "03";
                        break;
                    case 4:
                        result = "04";
                        break;
                    case 5:
                        result = "05";
                        break;
                    case 6:
                        result = "06";
                        break;
                    case 7:
                        result = "07";
                        break;
                    case 8:
                        result = "08";
                        break;
                    case 9:
                        result = "09";
                        break;
                    case 10:
                        result = "10";
                        break;
                    case 11:
                        result = "11";
                        break;
                    case 12:
                        result = "12";
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                result = "";
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

            return result;
        }
    }
}

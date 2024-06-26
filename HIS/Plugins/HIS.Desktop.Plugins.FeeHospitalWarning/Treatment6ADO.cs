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
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.Plugins.FeeHospitalWarning.Config;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.FeeHospitalWarning
{
    class Treatment6ADO : L_HIS_TREATMENT_1
    {
        public decimal? SoNo { get; set; }

        public string DOB_STR { get; set; }
        public decimal DaThu { get; set; }
        public decimal CanThuThem { get; set; }
        public decimal TOTAL_PRICE_STR { get; set; }
        public decimal TOTAL_HEIN_PRICE_STR { get; set; }
        public decimal TOTAL_PATIENT_PRICE_STR { get; set; }
        public string DEPARTMENT_NAME { get; set; }
        public bool IS_RIGHT_MEDI_ORG { get; set; }
        public string WARRING_LEVEL { get; set; }
        public string WARRING_NAME { get; set; }
        public string WARRING_COLOR { get; set; }
        public bool IS_BOLD { get; set; }

        public Treatment6ADO(L_HIS_TREATMENT_1 data)
        {
            if (data != null)
            {
                Inventec.Common.Mapper.DataObjectMapper.Map<Treatment6ADO>(this, data);

                this.DaThu = (data.TOTAL_DEPOSIT_AMOUNT ?? 0) + (data.TOTAL_BILL_AMOUNT ?? 0) - (data.TOTAL_BILL_TRANSFER_AMOUNT ?? 0) - (data.TOTAL_BILL_FUND ?? 0) - (data.TOTAL_REPAY_AMOUNT ?? 0) - (data.TOTAL_BILL_EXEMPTION ?? 0);
                SoNo = data.TOTAL_PATIENT_PRICE - DaThu - data.TOTAL_BILL_FUND - data.TOTAL_BILL_EXEMPTION;

                this.DOB_STR = Inventec.Common.DateTime.Convert.TimeNumberToDateString(data.TDL_PATIENT_DOB);

                this.CanThuThem = (data.TOTAL_PATIENT_PRICE ?? 0) - DaThu - (data.TOTAL_BILL_FUND ?? 0) - (data.TOTAL_BILL_EXEMPTION ?? 0);

                this.TOTAL_PRICE_STR = data.TOTAL_PRICE ?? 0;
                this.TOTAL_HEIN_PRICE_STR = data.TOTAL_HEIN_PRICE ?? 0;
                this.TOTAL_PATIENT_PRICE_STR = data.TOTAL_PATIENT_PRICE ?? 0;
                if (this.LAST_DEPARTMENT_ID.HasValue)
                {
                    var depart = BackendDataWorker.Get<HIS_DEPARTMENT>().FirstOrDefault(o => o.ID == this.LAST_DEPARTMENT_ID.Value);
                    if (depart != null)
                    {
                        this.DEPARTMENT_NAME = depart.DEPARTMENT_NAME;
                    }
                }
                if (!String.IsNullOrWhiteSpace(this.TDL_HEIN_MEDI_ORG_CODE))
                {
                    this.IS_RIGHT_MEDI_ORG = (this.TDL_HEIN_MEDI_ORG_CODE == BranchDataWorker.Branch.HEIN_MEDI_ORG_CODE);
                }

                if ((this.TOTAL_HEIN_PRICE ?? 0) > 0)
                {
                    HIS_WARNING_FEE_CFG warningFee = HisConfigWarning.GetWarningHeinFee(HisConfigCFG.PatientTypeId__BHYT, (this.TDL_TREATMENT_TYPE_ID ?? 0), this.IS_RIGHT_MEDI_ORG, (this.TOTAL_HEIN_PRICE ?? 0));
                    if (warningFee != null)
                    {
                        this.WARRING_LEVEL = warningFee.WARNING_FEE_CFG_NAME;
                        this.WARRING_COLOR = warningFee.COLOR_CODE;
                        if ((this.TOTAL_HEIN_PRICE ?? 0) > (warningFee.WARNING_PRICE ?? 0))
                        {
                            this.WARRING_NAME = "Vượt định mức";
                            this.IS_BOLD = true;
                        }
                        else
                        {
                            this.WARRING_NAME = "Bằng định mức";
                        }
                    }
                }
            }
        }
    }
}

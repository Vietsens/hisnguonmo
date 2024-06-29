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
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.AssignServiceEdit.ADO
{
    public class HisSereServADO : MOS.EFMODEL.DataModels.HIS_SERE_SERV
    {
        public bool isMulti { get; set; }
        public bool IsChecked { get; set; }
        public bool IsNotUseBhyt { get; set; }
        public long? ExecuteRoomId { get; set; }
        public bool IsExpend { get; set; }
        public short? IsAllowExpend { get; set; }
        public long serviceType { get; set; }
        public bool IsOutKtcFee { get; set; }
        public string SERVICE_CODE { get; set; }
        public string SERVICE_NAME { get; set; }
        public bool IsAssignDay { get; set; }
        public long? MIN_DURATION { get; set; }
        public string Instruction_Note { get; set; }
        public string SERVICE_CODE_HIDDEN { get; set; }
        public string SERVICE_NAME_HIDDEN { get; set; }
        public bool IsNotSetPrimaryPatientTypeId { get; set; }
        public bool IsNotChangePrimaryPaty { get; set; }

        public string PACKAGE_NAME { get; set; }
        public bool IS_NOT_FIXED_SERVICE { get; set; }

        public long? BILL_PATIENT_TYPE_ID { get; set; }
        public short? IS_NOT_CHANGE_BILL_PATY { get; set; }

        public bool IsNotChangePrimary { get; set; }
        public bool IsContainAppliedPatientType { get; set; }
        public bool IsContainAppliedPatientClassifyType { get; set; }

        public bool IsNotLoadDefaultPatientType { get; set; }
        public long? DEFAULT_PATIENT_TYPE_ID { get; set; }
        public long? AssignNumOrder { get; set; }
        public HisSereServADO()
        {
            this.IsChecked = false;
            this.IsExpend = false;
            this.IsNotUseBhyt = false;
        }

        public HisSereServADO(MOS.EFMODEL.DataModels.V_HIS_SERE_SERV_12 sereServ)
        {
            try
            {
                Inventec.Common.Mapper.DataObjectMapper.Map<ADO.HisSereServADO>(this, sereServ);
                this.IsChecked = false;
                this.IsExpend = false;
                this.IsNotUseBhyt = false;
                this.SERVICE_CODE_HIDDEN = convertToUnSign3(this.SERVICE_CODE) + this.SERVICE_CODE;
                this.SERVICE_NAME_HIDDEN = convertToUnSign3(this.SERVICE_CODE) + this.SERVICE_NAME;
                if (sereServ.PACKAGE_ID.HasValue)
                {
                    var package = BackendDataWorker.Get<HIS_PACKAGE>().FirstOrDefault(o => o.ID == sereServ.PACKAGE_ID);
                    if (package != null)
                    {
                        PACKAGE_NAME = package.PACKAGE_NAME;
                        IS_NOT_FIXED_SERVICE = !package.IS_NOT_FIXED_SERVICE.HasValue;
                    }
                }

                var service = BackendDataWorker.Get<V_HIS_SERVICE>().FirstOrDefault(o => o.ID == sereServ.SERVICE_ID);
                this.BILL_PATIENT_TYPE_ID = service != null ? service.BILL_PATIENT_TYPE_ID : null;
                this.IS_NOT_CHANGE_BILL_PATY = (service != null ? service.IS_NOT_CHANGE_BILL_PATY : null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public DevExpress.XtraEditors.DXErrorProvider.ErrorType ErrorTypeAmount { get; set; }
        public string ErrorMessageAmount { get; set; }
        public DevExpress.XtraEditors.DXErrorProvider.ErrorType ErrorTypePatientTypeId { get; set; }
        public string ErrorMessagePatientTypeId { get; set; }
        public DevExpress.XtraEditors.DXErrorProvider.ErrorType ErrorTypeIsAssignDay { get; set; }
        public string ErrorMessageIsAssignDay { get; set; }

        public string convertToUnSign3(string s)
        {
            if (String.IsNullOrWhiteSpace(s))
                return "";

            Regex regex = new Regex("\\p{IsCombiningDiacriticalMarks}+");
            string temp = s.Normalize(NormalizationForm.FormD);
            return regex.Replace(temp, String.Empty).Replace('\u0111', 'd').Replace('\u0110', 'D');
        }
    }
}

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
using Inventec.Common.Logging;
using Inventec.Core;
using MOS.Filter;
using SDA.EFMODEL.DataModels;
using System;
using System.Linq;
using SDA.Filter;
using System.Collections.Generic;
using Inventec.Common.LocalStorage.SdaConfig;
using HIS.Desktop.LocalStorage.BackendData;
using MOS.EFMODEL.DataModels;

namespace HIS.Desktop.LocalStorage.SdaConfigKey.Config
{
    public class HisPatientTypeCFG
    {
        private const string SDA_CONFIG__PATIENT_TYPE_CODE__IS_FREE = "DBCODE.HIS_RS.HIS_PATIENT_TYPE.PATIENT_TYPE_CODE.IS_FREE";//Doi tuong mien phi
        private const string SDA_CONFIG__PATIENT_TYPE_CODE__BHYT = "MOS.HIS_PATIENT_TYPE.PATIENT_TYPE_CODE.BHYT";//Doi tuong BHYT
        private const string SDA_CONFIG__PATIENT_TYPE_CODE__KSK = "DBCODE.HIS_RS.HIS_PATIENT_TYPE.PATIENT_TYPE_CODE.KSK";//Doi tuong khám sức khỏe DN
        private const string SDA_CONFIG__PATIENT_TYPE_CODE__FEE = "MOS.HIS_PATIENT_TYPE.HOSPITAL_FEE";
        private const string SDA_CONFIG__PATIENT_TYPE_CODE__BILL_INVOICE = "HIS.HIS_PATIENT_TYPE.PATIENT_TYPE_CODE.BILL_INVOICE";

        private const string SDA_CONFIG__PATIENT_TYPE_CODE__SERVICE = "HIS.HIS_PATIENT_TYPE.PATIENT_TYPE_CODE.SERVICE";
        private const string SDA_CONFIG__PATIENT_TYPE_CODE__THUPHI = "HIS.HIS_PATIENT_TYPE.PATIENT_TYPE_CODE.THUPHI";

        private static long patientTypeIdIsService;
        public static long PATIENT_TYPE_ID__IS_SERIVCE
        {
            get
            {
                if (patientTypeIdIsService == 0)
                {
                    patientTypeIdIsService = GetId(SdaConfigs.Get<string>(SDA_CONFIG__PATIENT_TYPE_CODE__SERVICE));
                }
                return patientTypeIdIsService;
            }
            set
            {
                patientTypeIdIsService = value;
            }
        }

        private static long patientTypeIdIsThuPhi;
        public static long PATIENT_TYPE_ID__IS_THUPHI
        {
            get
            {
                if (patientTypeIdIsThuPhi == 0)
                {
                    patientTypeIdIsThuPhi = GetId(SdaConfigs.Get<string>(SDA_CONFIG__PATIENT_TYPE_CODE__THUPHI));
                }
                return patientTypeIdIsThuPhi;
            }
            set
            {
                patientTypeIdIsThuPhi = value;
            }
        }

        private static long patientTypeIdIsFee;
        public static long PATIENT_TYPE_ID__IS_FEE
        {
            get
            {
                if (patientTypeIdIsFee == 0)
                {
                    patientTypeIdIsFee = GetId(SdaConfigs.Get<string>(SDA_CONFIG__PATIENT_TYPE_CODE__FEE));
                }
                return patientTypeIdIsFee;
            }
            set
            {
                patientTypeIdIsFee = value;
            }
        }


        private static long patientTypeIdIsHein;
        public static long PATIENT_TYPE_ID__BHYT
        {
            get
            {
                if (patientTypeIdIsHein == 0)
                {
                    patientTypeIdIsHein = GetId(SdaConfigs.Get<string>(ConfigKeys.DBCODE__HIS_RS__HIS_PATIENT_TYPE__PATIENT_TYPE_CODE__BHYT));
                }
                return patientTypeIdIsHein;
            }
            set
            {
                patientTypeIdIsHein = value;
            }
        }

        private static long patientTypeIdIsFree;
        public static long PATIENT_TYPE_ID__IS_FREE
        {
            get
            {
                if (patientTypeIdIsFree == 0)
                {
                    patientTypeIdIsFree = GetId(SDA_CONFIG__PATIENT_TYPE_CODE__IS_FREE);
                }
                return patientTypeIdIsFree;
            }
            set
            {
                patientTypeIdIsFree = value;
            }
        }

        private static long patientTypeIdKsk;
        public static long PATIENT_TYPE_ID__KSK
        {
            get
            {
                if (patientTypeIdKsk == 0)
                {
                    patientTypeIdKsk = GetId(SdaConfigs.Get<string>(ConfigKeys.DBCODE__HIS_RS__HIS_PATIENT_TYPE__PATIENT_TYPE_CODE__KSK));
                }
                return patientTypeIdKsk;
            }
            set
            {
                patientTypeIdKsk = value;
            }
        }

        private static long patientTypeIdBillInvoice;
        public static long PATIENT_TYPE_ID__BILL_INVOICE
        {
            get
            {
                if (patientTypeIdBillInvoice == 0)
                {
                    patientTypeIdBillInvoice = GetId(SdaConfigs.Get<string>(SDA_CONFIG__PATIENT_TYPE_CODE__BILL_INVOICE));
                }
                return patientTypeIdBillInvoice;
            }
            set
            {
                patientTypeIdBillInvoice = value;
            }
        }

        //Code
        private static string patientTypeCodeIsHein;
        public static string PATIENT_TYPE_CODE__BHYT
        {
            get
            {
                if (String.IsNullOrEmpty(patientTypeCodeIsHein))
                {
                    patientTypeCodeIsHein = SdaConfigs.Get<string>(ConfigKeys.DBCODE__HIS_RS__HIS_PATIENT_TYPE__PATIENT_TYPE_CODE__BHYT);
                }
                return patientTypeCodeIsHein;
            }
            set
            {
                patientTypeCodeIsHein = value;
            }
        }

        private static string patientTypeCodeIsFree;
        public static string PATIENT_TYPE_CODE__IS_FREE
        {
            get
            {
                if (String.IsNullOrEmpty(patientTypeCodeIsFree))
                {
                    patientTypeCodeIsFree = SdaConfigs.Get<string>(SDA_CONFIG__PATIENT_TYPE_CODE__IS_FREE);
                }
                return patientTypeCodeIsFree;
            }
            set
            {
                patientTypeCodeIsFree = value;
            }
        }

        private static string patientTypeCodeKsk;
        public static string PATIENT_TYPE_CODE__KSK
        {
            get
            {
                if (String.IsNullOrEmpty(patientTypeCodeKsk))
                {
                    patientTypeCodeKsk = SdaConfigs.Get<string>(ConfigKeys.DBCODE__HIS_RS__HIS_PATIENT_TYPE__PATIENT_TYPE_CODE__KSK);
                }
                return patientTypeCodeKsk;
            }
            set
            {
                patientTypeCodeKsk = value;
            }
        }

        private static long GetId(string code)
        {
            long result = 0;
            try
            {
                var data = BackendDataWorker.Get<HIS_PATIENT_TYPE>().FirstOrDefault(o => o.PATIENT_TYPE_CODE == code);
                if (!(data != null && data.ID > 0)) throw new ArgumentNullException(code + LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => code), code));
                result = data.ID;
            }
            catch (Exception ex)
            {
                LogSystem.Debug(ex);
                result = 0;
            }
            return result;
        }
    }
}
